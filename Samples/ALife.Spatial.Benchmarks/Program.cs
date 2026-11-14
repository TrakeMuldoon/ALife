using System.Diagnostics;
using System.Globalization;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using ALife.Spatial;
using ALife.Spatial.Benchmarks;

// Times every index on every scenario and writes the results as Markdown tables.
//   --readme <path>      replace the results between the markers in that file instead of printing them
//   --quick              fewer frames per measurement, for a rough look
//   --scenario <name>    only the scenarios whose names contain this
//   --max <count>        skip counts above this
string? readme = null;
string? only = null;
bool quick = false;
int maxCount = int.MaxValue;
for (int i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--readme":
            readme = args[++i];
            break;
        case "--quick":
            quick = true;
            break;
        case "--scenario":
            only = args[++i];
            break;
        case "--max":
            maxCount = int.Parse(args[++i], CultureInfo.InvariantCulture);
            break;
        default:
            Console.Error.WriteLine($"Unknown argument {args[i]}");
            return 1;
    }
}

#if DEBUG
Console.Error.WriteLine("This is a Debug build, and its numbers mean little. Run with -c Release.");
#endif

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
Bench bench = new(quick);
StringBuilder output = new();
output.AppendLine(bench.Machine());
foreach (Scenario scenario in Scenario.All)
{
    if (only is null || scenario.Name.Contains(only, StringComparison.OrdinalIgnoreCase))
    {
        output.AppendLine();
        output.Append(bench.Run(scenario, scenario.Counts.Where(count => count <= maxCount).ToArray()));
    }
}

if (readme is null)
{
    Console.WriteLine(output);
    return 0;
}

const string Start = "<!-- results:start -->";
const string End = "<!-- results:end -->";
string text = File.ReadAllText(readme);
int from = text.IndexOf(Start, StringComparison.Ordinal), to = text.IndexOf(End, StringComparison.Ordinal);
if (from < 0 || to < from)
{
    Console.Error.WriteLine($"{readme} has no {Start} ... {End} markers.");
    return 1;
}

File.WriteAllText(readme, text[..(from + Start.Length)] + "\n\n" + output.ToString().TrimEnd() + "\n\n" + text[to..]);
Console.Error.WriteLine($"Updated {readme}.");
return 0;

/// <summary>Measures frames and lays the results out.</summary>
internal sealed class Bench(bool quick)
{
    // A run stops once it has this many frames and has taken this long, or has this many frames.
    private readonly int _minFrames = quick ? 3 : 5;
    private readonly int _maxFrames = quick ? 10 : 40;
    private readonly TimeSpan _budget = TimeSpan.FromSeconds(quick ? 0.25 : 1);
    private readonly TimeSpan _warmup = TimeSpan.FromSeconds(quick ? 0.3 : 0.5);

    /// <summary>Past this, an index isn't measured at bigger counts in the same scenario.</summary>
    private const double SlowFrameMilliseconds = 1000;

    public string Machine()
    {
        string build = "Release";
#if DEBUG
        build = "Debug (numbers not meaningful)";
#endif
        return $"Measured on {CpuName()} ({Environment.ProcessorCount} cores), {RuntimeInformation.OSDescription}, "
            + $"{RuntimeInformation.FrameworkDescription} {RuntimeInformation.ProcessArchitecture}, {build}"
            + $"{(GCSettings.IsServerGC ? ", server GC" : "")}, on {DateTime.Now:yyyy-MM-dd}{(quick ? ", with --quick" : "")}.";
    }

    public string Run(Scenario scenario, int[] counts)
    {
        List<Contender> contenders = Contender.All().ToList();
        Result?[,] results = new Result?[contenders.Count, counts.Length];
        long[] pairs = new long[counts.Length];
        Aabb[] worlds = new Aabb[counts.Length];
        for (int c = 0; c < counts.Length; c++)
        {
            Crowd reference = scenario.Create(counts[c]);
            worlds[c] = reference.Bounds;
            pairs[c] = reference.CountTouchingPairs();
            for (int k = 0; k < contenders.Count; k++)
            {
                Result? previous = c > 0 ? results[k, c - 1] : null;
                if (c > 0 && (previous is null || previous.Value.Total > SlowFrameMilliseconds))
                {
                    continue;
                }

                // A fresh index and crowd for each, so no run inherits another's memory layout.
                Contender contender = Contender.All().ElementAt(k);
                Crowd crowd = scenario.Create(counts[c]);
                Sizing sizing = new(crowd.Bounds, scenario.MinRadius, scenario.CommonRadius, scenario.MaxRadius);
                results[k, c] = Measure(contender, sizing, crowd);
                Console.Error.WriteLine($"{scenario.Name,-14} {counts[c],9:N0} {contender.Name,-18} {contender.Search,-34} {contender.Threads,-10} {results[k, c]!.Value.Total,9:0.00} ms");
            }
        }

        return Layout(scenario, counts, contenders, results, pairs, worlds);
    }

    private Result Measure(Contender contender, Sizing sizing, Crowd crowd)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        contender.Setup(sizing, crowd);

        // Warm up for long enough that the JIT has finished optimizing the hot code (tiering up waits for 100 ms without
        // new methods), or for one frame if frames are slow, since by then the smaller counts have warmed it.
        List<Result> frames = [];
        Stopwatch elapsed = Stopwatch.StartNew();
        for (int warmup = 0; warmup < 2 || elapsed.Elapsed < _warmup; warmup++)
        {
            Result frame = Frame(contender, crowd);
            if (frame.Total > 200)
            {
                break;
            }
        }

        elapsed.Restart();
        while (frames.Count < _maxFrames && (frames.Count < _minFrames || elapsed.Elapsed < _budget))
        {
            frames.Add(Frame(contender, crowd));
            if (frames[^1].Total > SlowFrameMilliseconds && frames.Count >= 3)
            {
                break;
            }
        }

        // The median frame, so the odd GC or scheduling hiccup doesn't count.
        frames.Sort((a, b) => a.Total.CompareTo(b.Total));
        return frames[frames.Count / 2];
    }

    private static Result Frame(Contender contender, Crowd crowd)
    {
        long start = Stopwatch.GetTimestamp();
        contender.BeforeStep(crowd);
        double before = Stopwatch.GetElapsedTime(start).TotalMilliseconds;

        crowd.Step();

        start = Stopwatch.GetTimestamp();
        contender.Update(crowd);
        double update = Stopwatch.GetElapsedTime(start).TotalMilliseconds + before;

        start = Stopwatch.GetTimestamp();
        contender.FindContacts(crowd);
        double search = Stopwatch.GetElapsedTime(start).TotalMilliseconds;
        return new Result(update, search);
    }

    private static string Layout(Scenario scenario, int[] counts, List<Contender> contenders, Result?[,] results, long[] pairs, Aabb[] worlds)
    {
        StringBuilder text = new();
        text.AppendLine($"### {scenario.Name}");
        text.AppendLine();
        text.AppendLine(scenario.Description);
        text.AppendLine();
        text.AppendLine("Milliseconds per frame, update plus contact search. Fastest in each column in bold; — means it took over a second at the previous count.");
        text.AppendLine();
        text.AppendLine($"| Index | Search | Threads | {string.Join(" | ", counts.Select(Count))} |");
        text.AppendLine($"|---|---|---|{string.Concat(counts.Select(_ => "---:|"))}");
        text.AppendLine($"| *World* | | | {string.Join(" | ", worlds.Select(w => $"*{w.Width:0}×{w.Height:0}*"))} |");
        text.AppendLine($"| *Touching pairs* | | | {string.Join(" | ", pairs.Select(p => $"*{p:N0}*"))} |");
        for (int k = 0; k < contenders.Count; k++)
        {
            List<string> cells = [];
            for (int c = 0; c < counts.Length; c++)
            {
                double best = Enumerable.Range(0, contenders.Count).Select(i => results[i, c]?.Total ?? double.MaxValue).Min();
                cells.Add(results[k, c] is Result result
                    ? result.Total == best ? $"**{Milliseconds(result.Total)}**" : Milliseconds(result.Total)
                    : "—");
            }

            text.AppendLine($"| {contenders[k].Name} | {contenders[k].Search} | {contenders[k].Threads} | {string.Join(" | ", cells)} |");
        }

        // Where the time goes at the biggest count each index reached.
        text.AppendLine();
        text.AppendLine("<details><summary>Update and search separately, at the biggest count each reached</summary>");
        text.AppendLine();
        text.AppendLine("| Index | Search | Threads | Agents | Update | Search | Total |");
        text.AppendLine("|---|---|---|---:|---:|---:|---:|");
        for (int k = 0; k < contenders.Count; k++)
        {
            for (int c = counts.Length - 1; c >= 0; c--)
            {
                if (results[k, c] is Result result)
                {
                    text.AppendLine($"| {contenders[k].Name} | {contenders[k].Search} | {contenders[k].Threads} | {Count(counts[c])} | "
                        + $"{Milliseconds(result.Update)} | {Milliseconds(result.Search)} | {Milliseconds(result.Total)} |");
                    break;
                }
            }
        }

        text.AppendLine();
        text.AppendLine("</details>");
        return text.ToString();
    }

    private static string Count(int count) => count >= 1_000_000 ? $"{count / 1_000_000}M" : $"{count / 1000}k";

    private static string Milliseconds(double value) => value switch
    {
        < 1 => value.ToString("0.00"),
        < 100 => value.ToString("0.0"),
        _ => value.ToString("N0"),
    };

    private static string CpuName()
    {
        try
        {
            if (OperatingSystem.IsMacOS())
            {
                using Process process = Process.Start(new ProcessStartInfo("sysctl", "-n machdep.cpu.brand_string") { RedirectStandardOutput = true })!;
                string name = process.StandardOutput.ReadToEnd().Trim();
                process.WaitForExit();
                return name;
            }

            if (OperatingSystem.IsLinux())
            {
                string? line = File.ReadLines("/proc/cpuinfo").FirstOrDefault(l => l.StartsWith("model name", StringComparison.Ordinal));
                return line?[(line.IndexOf(':') + 1)..].Trim() ?? "an unknown CPU";
            }

            return Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER") ?? "an unknown CPU";
        }
        catch (Exception)
        {
            return "an unknown CPU";
        }
    }

    private readonly record struct Result(double Update, double Search)
    {
        public double Total => Update + Search;
    }
}
