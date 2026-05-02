using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;

namespace ALife.Core.Utility;

public sealed class SysInfo
{
    private static SysInfo? _instance;
    
    private string _cpuName;
    
    static SysInfo() {}

    private SysInfo()
    {
        if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            _cpuName = GetCpuNameWindows();
        }
        else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            _cpuName = GetCpuNameLinux();
        }
        else if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            _cpuName = GetCpuNameMac();
        }
        else
        {
            _cpuName = "Unsupported OS";
        }
    }
    
    public static SysInfo Instance => _instance ??= new SysInfo();
    
    public string CpuName => _cpuName;
    
    private static string GetCpuNameWindows()
    {
        using var searcher = new ManagementObjectSearcher("select * from Win32_Processor");

        foreach (var obj in searcher.Get())
        {
            return obj["Name"]?.ToString() ?? "Unknown CPU";
        }

        return "Unknown CPU";
    }
    
    private string GetCpuNameLinux()
    {
        return File.ReadAllText("/proc/cpuinfo")
            .Split('\n')
            .FirstOrDefault(l => l.StartsWith("model name"))?
            .Split(':')[1].Trim() ?? "Unknown CPU";
    }
    
    private string GetCpuNameMac()
    {
        var p = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "sysctl",
                Arguments = "-n machdep.cpu.brand_string",
                RedirectStandardOutput = true,
                UseShellExecute = false
            }
        };
        p.Start();
        string result = p.StandardOutput.ReadToEnd().Trim();
        p.WaitForExit();
        return result;
    }
}