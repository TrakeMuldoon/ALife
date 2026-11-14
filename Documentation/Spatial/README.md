# ALife.Spatial performance

How fast each `ALife.Spatial` index finds a frame's collisions, in five kinds of crowd from 1,000 to 1,000,000 circles. The tables are written by [`Samples/ALife.Spatial.Benchmarks`](../../Samples/ALife.Spatial.Benchmarks); rerun it to measure your own machine (see [Rerunning](#rerunning)).

## Takeaways

These are from the run in [Results](#results): Apple M3 Max, 14 cores.

- **Up to about 10,000 circles, anything works.** Every index fits easily in a 60 fps frame. The exception is `UniformGrid` when a few elements are much bigger than the rest.
- **Past that, query-each indexes fall away.** `QuadTree`, `LooseQuadTree`, `LooseTightGrid` and `UniformGrid` run one `Query` per agent:
  - At 50,000 in the demo world they take 35 to 93 ms; at 200,000, 333 to 863 ms.
  - Most of that is the search, not the update. `QuadTree` is the exception: its `Move`s alone take 262 ms at 200,000.
  - Among them, `UniformGrid` is fastest when every circle is the same size and the crowd isn't packed tight. But its one element size must fit the biggest circle, so in "A few giants" it's 18 times slower than with even spread (467 vs 25.8 ms at 100,000).
- **Pair searches are 3 to 12 times faster on a single thread.** These are `LayeredGrid.ForEachPair`, the packed indexes and `PhasedGrid`. In the demo world at 200,000 they take 70 to 104 ms on one thread, against 333 ms or more for querying each agent. Threads then cut that again.
- **`PhasedGrid` with `ForEachPairInParallel` is the fastest from 100,000 circles up, in every scenario.**
  - Demo world, 200,000: 9.1 ms on 14 threads. Next best are `PackedLayeredGrid` at 13.8 and `PackedGrid` at 14.8.
  - At 1,000,000, reordered: 11.2 to 16.2 ms, against 27.3 ms or more for every other index.
  - Its edge comes from doing each pair's collision math once while still running in parallel, which pays off most where there are most pairs, and from a build that keeps getting faster with more threads.
- **Reordering pays off with big crowds.** Keeping the agents in the grid's `Order` changes little at 200,000 in the demo world (8.6 vs 9.1 ms). At 1,000,000 it saves 9 to 25%: neighbors then sit near each other in memory, which cuts the search by a third to a half, far more than the copy costs.
- **In huge worlds, the per-frame build can be the bottleneck.**
  - At 1,000,000 the other per-frame indexes spend 16 to 53 ms just building, whatever the thread count. On many threads, that's as much as or more than they spend searching. `PackedLayeredGrid` with mixed sizes is the worst: 52.7 of its 75.8 ms.
  - `PhasedGrid`'s build scales with threads instead. In "Mixed sizes" at 1,000,000 it takes 28.3 ms on 1 thread, 9.3 on 4 and 4.8 on 14; in "Even spread", 20.8, 5.5 and 3.6.
- **Clumps didn't make trees win.** Even with four in five circles packed into 8 clumps, the grids led:
  - `PhasedGrid` reordered took 15.2 ms at 1,000,000, and `PackedGrid.ForEachNeighbor` 38.8.
  - `PackedQuadTree` took 61.1, and `QuadTree` took 2.7 seconds.
- **`PackedQuadTree` never won.** On one thread it's 1.2 to 3.4 times slower than `PackedGrid`, and never faster. Use it only when you can't pick a cell size at all.
- **Rare giant elements are handled well by the layered grids, the packed grids and `PhasedGrid`.** Giants go in a top layer or a side tree. At 1,000,000, costs stay within about 15% of "Even spread", except `PackedGrid.ForEachNeighbor`: 35.9 ms vs 27.3.

### Which to use

| You have | Use |
|---|---|
| A few thousand elements, or you need to query arbitrary areas between frames | An [ISpatialIndex](ISpatialIndex.md): `UniformGrid` if every element fits one size, else `LooseTightGrid` |
| Tens of thousands of moving elements or more, with all their pairs every frame | [PhasedGrid](Phased/PhasedGrid.md) with `ForEachPairInParallel`, with your arrays kept in its `Order` if you can. Give it as many threads as you can spare: more help from about 50,000 elements up. |
| A per-frame index whose visitor can't write both elements of a pair | [PackedGrid](Packed/PackedGrid.md), or [PackedLayeredGrid](Packed/PackedLayeredGrid.md) for a range of sizes, with `ForEachNeighbor` |

Numbers on your machine will differ, especially with fewer cores, so [rerun](#rerunning) before choosing on a near tie.

## The indexes

| Index | Kind | How it's searched here |
|---|---|---|
| [QuadTree](QuadTree.md) | Kept up to date with `Move`; an element in every leaf it overlaps | `Query` for each agent |
| [LooseQuadTree](LooseQuadTree.md) | Kept up to date; an element in one leaf, with loose node boxes | `Query` for each agent |
| [LooseTightGrid](LooseTightGrid.md) | Kept up to date; loose cells indexed by a coarser tight grid | `Query` for each agent |
| [UniformGrid](UniformGrid.md) | Kept up to date; elements of one shared size, by center | `Query` for each agent |
| [LayeredGrid](LayeredGrid.md) | Kept up to date; a stack of ever-finer grids | `ForEachPair` on one thread, or `ForEachNeighbor` in parallel |
| [PackedGrid](Packed/PackedGrid.md) | Rebuilt every frame; one counting-sorted grid | `ForEachPair`, or `ForEachNeighbor` on the thread pool |
| [PackedQuadTree](Packed/PackedQuadTree.md) | Rebuilt every frame; a Morton-sorted loose quadtree | `ForEachPair`, or `ForEachNeighbor` on the thread pool |
| [PackedLayeredGrid](Packed/PackedLayeredGrid.md) | Rebuilt every frame; counting-sorted grids in layers | `ForEachPair`, or `ForEachNeighbor` on a set number of threads |
| [PhasedGrid](Phased/PhasedGrid.md) | Rebuilt every frame; counting-sorted grids in layers, searched in checkerboard phases | `ForEachPair`, or `ForEachPairInParallel` on a set number of threads, optionally with the agents kept in its `Order` |

The **Threads** column says how many threads the search (and, for the layered and phased grids, the build) may use. "(pool)" means the index hands its work to `Parallel.For` with no limit, so it takes every core.

## How it's measured

Each scenario makes a crowd of moving circles. A frame is:

1. **Move** every circle a sixtieth of a second along its velocity, bouncing off the edges of the world, and work out its box. Not timed: it's the same for every index.
2. **Update** (timed): bring the index up to date. The kept-up-to-date indexes `Move` every element and then `Cleanup`; the per-frame ones `Build` from the boxes. For "reordered", this also includes copying the agents into the grid's last `Order`.
3. **Search** (timed): find every pair of overlapping boxes, and for each one, check the circles really touch and work out a push and a bounce, as the demo does.

The table cells are **update plus search, in milliseconds**, for the median of 5 to 40 frames, after at least half a second of warm-up frames so the JIT has finished optimizing. A frame budget at 60 fps is 16.7 ms.

Details that matter when reading the numbers:

- **Contacts are worked out but never applied**, so circles pass through each other and a crowd keeps its shape (and its number of touching pairs) however long a run takes.
- **Each pair's collision math runs once**:
  - Searches that report a pair once (`Query` keeping only the higher id, `ForEachPair`, `ForEachPairInParallel`) work out both circles' halves together.
  - `ForEachNeighbor` reports each pair from both sides, once per circle, and each call works out that circle's half. That's twice the visits but no shared writes, which is why it can run in parallel.
- **The searches only see boxes.** The visitor does the circle test, so an index that hands over fewer false candidates spends less time in it.
- **Indexes are sized the way their pages advise.** Every index is built fresh for each count, and `R` is the scenario's common radius:

  | Index | Sizing |
  |---|---|
  | `QuadTree`, `LooseQuadTree`, `PackedQuadTree` | 8 elements per leaf. Depth 8 for a 1280-wide world, plus 1 each time the world doubles (at most 16). |
  | `LooseTightGrid` | Loose cells about 3.3`R` across, tight cells four times that |
  | `UniformGrid` | Cells about 6.6`R` across. Its element half size is the scenario's *largest* radius, since every element must fit it. |
  | `PackedGrid` | Cells about 2.1`R` across |
  | `LayeredGrid`, `PackedLayeredGrid`, `PhasedGrid` | Enough layers to run from cells about 2.1× the smallest radius at the bottom to ones that fit the largest radius at the top: 1 layer for one size, 3 layers for radius 3 to 12, 6 layers for radius 3 to 80. |

- **A dash** means the index wasn't measured at that count, because a frame took over a second at the count before.
- **Machine noise** is a few percent between runs. Differences under about 10% are a tie, and so are cells under about 0.1 ms.

## Scenarios

- **Demo world**: the demo's fixed 1280×720 world, with radius-3 circles. More circles means more crowding, up to about 24 overlaps per circle at 200,000.
- **Even spread**: radius-3 circles in a world grown to keep half the area covered. That's about 2 overlaps per circle at every count: a bigger level with more units, not a more crowded one.
- **Mixed sizes**: radii 3 to 12, mostly small, half the area covered.
- **Clustered**: radius 3. Four in five circles are in 8 dense clumps and the rest spread thin, a quarter of the area covered on average.
- **A few giants**: radius 3, except one in a thousand circles of radius 40 to 80. Half the area covered.

Every count uses its own fixed random seed, so every index sees the same crowd.

## Results

<!-- results:start -->

Measured on Apple M3 Max (14 cores), macOS 26.6.2, .NET 10.0.10 Arm64, Release, on 2026-11-03.

### Demo world

Circles of radius 3 spread evenly over the demo's fixed 1280×720 world, so it gets more crowded as the count rises: at 200,000 every circle overlaps about 24 others. The worst case for the narrow phase; it's what the demo shows.

Milliseconds per frame, update plus contact search. Fastest in each column in bold; — means it took over a second at the previous count.

| Index | Search | Threads | 1k | 10k | 50k | 100k | 200k |
|---|---|---|---:|---:|---:|---:|---:|
| *World* | | | *1280×720* | *1280×720* | *1280×720* | *1280×720* | *1280×720* |
| *Touching pairs* | | | *46* | *6,263* | *153,516* | *613,545* | *2,451,178* |
| QuadTree | Query each | 1 | 0.28 | 6.3 | 92.5 | 298 | 863 |
| LooseQuadTree | Query each | 1 | 0.25 | 4.8 | 44.3 | 138 | 333 |
| LooseTightGrid | Query each | 1 | 0.11 | 3.1 | 35.9 | 120 | 417 |
| UniformGrid | Query each | 1 | 0.04 | 1.5 | 34.9 | 135 | 539 |
| LayeredGrid | ForEachPair | 1 | **0.04** | 0.59 | 8.8 | 30.3 | 92.5 |
| LayeredGrid | ForEachNeighbor | 4 | 0.05 | 0.44 | 4.8 | 14.8 | 50.5 |
| LayeredGrid | ForEachNeighbor | 14 | 0.06 | **0.26** | 2.4 | 6.2 | 18.1 |
| PackedGrid | ForEachPair | 1 | 0.07 | 0.45 | 5.8 | 21.3 | 70.4 |
| PackedGrid | ForEachNeighbor | 14 (pool) | 0.10 | 0.29 | 2.1 | 5.6 | 14.8 |
| PackedQuadTree | ForEachPair | 1 | 0.10 | 1.5 | 13.0 | 37.2 | 104 |
| PackedQuadTree | ForEachNeighbor | 14 (pool) | 0.08 | 0.64 | 4.1 | 8.7 | 22.7 |
| PackedLayeredGrid | ForEachPair | 1 | 0.08 | 0.54 | 6.5 | 21.4 | 72.0 |
| PackedLayeredGrid | ForEachNeighbor | 4 | 0.14 | 0.41 | 3.6 | 11.2 | 38.4 |
| PackedLayeredGrid | ForEachNeighbor | 14 | 0.14 | 0.33 | 1.9 | 4.4 | 13.8 |
| PhasedGrid | ForEachPair | 1 | 0.05 | 0.49 | 6.2 | 19.6 | 71.1 |
| PhasedGrid | ForEachPairInParallel | 4 | 0.07 | 0.28 | 1.9 | 5.7 | 20.1 |
| PhasedGrid | ForEachPairInParallel | 14 | 0.08 | 0.28 | 1.2 | 3.0 | 9.1 |
| PhasedGrid | ForEachPairInParallel, reordered | 14 | 0.08 | 0.30 | **1.1** | **2.9** | **8.6** |

<details><summary>Update and search separately, at the biggest count each reached</summary>

| Index | Search | Threads | Agents | Update | Search | Total |
|---|---|---|---:|---:|---:|---:|
| QuadTree | Query each | 1 | 200k | 262 | 601 | 863 |
| LooseQuadTree | Query each | 1 | 200k | 39.3 | 294 | 333 |
| LooseTightGrid | Query each | 1 | 200k | 9.4 | 407 | 417 |
| UniformGrid | Query each | 1 | 200k | 8.7 | 530 | 539 |
| LayeredGrid | ForEachPair | 1 | 200k | 1.8 | 90.7 | 92.5 |
| LayeredGrid | ForEachNeighbor | 4 | 200k | 1.9 | 48.7 | 50.5 |
| LayeredGrid | ForEachNeighbor | 14 | 200k | 2.3 | 15.8 | 18.1 |
| PackedGrid | ForEachPair | 1 | 200k | 1.9 | 68.5 | 70.4 |
| PackedGrid | ForEachNeighbor | 14 (pool) | 200k | 1.9 | 12.9 | 14.8 |
| PackedQuadTree | ForEachPair | 1 | 200k | 6.2 | 97.9 | 104 |
| PackedQuadTree | ForEachNeighbor | 14 (pool) | 200k | 5.3 | 17.3 | 22.7 |
| PackedLayeredGrid | ForEachPair | 1 | 200k | 2.1 | 69.9 | 72.0 |
| PackedLayeredGrid | ForEachNeighbor | 4 | 200k | 2.3 | 36.1 | 38.4 |
| PackedLayeredGrid | ForEachNeighbor | 14 | 200k | 2.2 | 11.6 | 13.8 |
| PhasedGrid | ForEachPair | 1 | 200k | 2.3 | 68.7 | 71.1 |
| PhasedGrid | ForEachPairInParallel | 4 | 200k | 0.74 | 19.4 | 20.1 |
| PhasedGrid | ForEachPairInParallel | 14 | 200k | 0.60 | 8.5 | 9.1 |
| PhasedGrid | ForEachPairInParallel, reordered | 14 | 200k | 1.2 | 7.4 | 8.6 |

</details>

### Even spread

Circles of radius 3 spread evenly, with the world grown to keep the crowding the same (half the area covered, about 2 overlaps per circle), as in a big level with more units in it.

Milliseconds per frame, update plus contact search. Fastest in each column in bold; — means it took over a second at the previous count.

| Index | Search | Threads | 1k | 10k | 100k | 250k | 1M |
|---|---|---|---:|---:|---:|---:|---:|
| *World* | | | *317×178* | *1003×564* | *3171×1783* | *5013×2820* | *10027×5640* |
| *Touching pairs* | | | *915* | *10,093* | *99,733* | *250,331* | *1,000,663* |
| QuadTree | Query each | 1 | 0.57 | 7.5 | 78.8 | 226 | 1,990 |
| LooseQuadTree | Query each | 1 | 0.37 | 4.9 | 64.7 | 212 | 1,636 |
| LooseTightGrid | Query each | 1 | 0.30 | 3.5 | 40.4 | 118 | 1,040 |
| UniformGrid | Query each | 1 | 0.18 | 2.1 | 25.8 | 72.7 | 591 |
| LayeredGrid | ForEachPair | 1 | 0.07 | 0.79 | 8.1 | 24.3 | 164 |
| LayeredGrid | ForEachNeighbor | 4 | 0.05 | 0.51 | 5.4 | 14.8 | 85.0 |
| LayeredGrid | ForEachNeighbor | 14 | 0.04 | 0.29 | 2.9 | 7.4 | 40.1 |
| PackedGrid | ForEachPair | 1 | 0.05 | 0.54 | 5.6 | 14.8 | 84.3 |
| PackedGrid | ForEachNeighbor | 14 (pool) | **0.04** | 0.29 | 2.3 | 5.8 | 27.3 |
| PackedQuadTree | ForEachPair | 1 | 0.14 | 1.5 | 16.7 | 50.9 | 238 |
| PackedQuadTree | ForEachNeighbor | 14 (pool) | 0.13 | 0.61 | 5.2 | 12.8 | 58.6 |
| PackedLayeredGrid | ForEachPair | 1 | 0.06 | 0.61 | 6.8 | 19.1 | 99.4 |
| PackedLayeredGrid | ForEachNeighbor | 4 | 0.05 | 0.43 | 4.5 | 11.5 | 58.3 |
| PackedLayeredGrid | ForEachNeighbor | 14 | 0.05 | **0.27** | 2.5 | 6.5 | 33.5 |
| PhasedGrid | ForEachPair | 1 | 0.06 | 0.57 | 5.7 | 16.6 | 106 |
| PhasedGrid | ForEachPairInParallel | 4 | 0.04 | 0.29 | 2.0 | 5.2 | 28.9 |
| PhasedGrid | ForEachPairInParallel | 14 | 0.06 | 0.28 | **1.3** | **2.9** | 14.9 |
| PhasedGrid | ForEachPairInParallel, reordered | 14 | 0.06 | 0.30 | 1.3 | 3.0 | **11.2** |

<details><summary>Update and search separately, at the biggest count each reached</summary>

| Index | Search | Threads | Agents | Update | Search | Total |
|---|---|---|---:|---:|---:|---:|
| QuadTree | Query each | 1 | 1M | 755 | 1,235 | 1,990 |
| LooseQuadTree | Query each | 1 | 1M | 543 | 1,093 | 1,636 |
| LooseTightGrid | Query each | 1 | 1M | 79.0 | 961 | 1,040 |
| UniformGrid | Query each | 1 | 1M | 23.5 | 567 | 591 |
| LayeredGrid | ForEachPair | 1 | 1M | 20.4 | 143 | 164 |
| LayeredGrid | ForEachNeighbor | 4 | 1M | 20.6 | 64.5 | 85.0 |
| LayeredGrid | ForEachNeighbor | 14 | 1M | 16.7 | 23.4 | 40.1 |
| PackedGrid | ForEachPair | 1 | 1M | 16.5 | 67.9 | 84.3 |
| PackedGrid | ForEachNeighbor | 14 (pool) | 1M | 16.3 | 11.0 | 27.3 |
| PackedQuadTree | ForEachPair | 1 | 1M | 34.1 | 204 | 238 |
| PackedQuadTree | ForEachNeighbor | 14 (pool) | 1M | 34.8 | 23.8 | 58.6 |
| PackedLayeredGrid | ForEachPair | 1 | 1M | 16.8 | 82.6 | 99.4 |
| PackedLayeredGrid | ForEachNeighbor | 4 | 1M | 18.1 | 40.1 | 58.3 |
| PackedLayeredGrid | ForEachNeighbor | 14 | 1M | 20.5 | 13.0 | 33.5 |
| PhasedGrid | ForEachPair | 1 | 1M | 20.8 | 85.1 | 106 |
| PhasedGrid | ForEachPairInParallel | 4 | 1M | 5.5 | 23.4 | 28.9 |
| PhasedGrid | ForEachPairInParallel | 14 | 1M | 3.6 | 11.4 | 14.9 |
| PhasedGrid | ForEachPairInParallel, reordered | 14 | 1M | 5.4 | 5.9 | 11.2 |

</details>

### Mixed sizes

Radii from 3 to 12, mostly small (as with the demo's V key), spread evenly over a world covered half over.

Milliseconds per frame, update plus contact search. Fastest in each column in bold; — means it took over a second at the previous count.

| Index | Search | Threads | 1k | 10k | 100k | 250k | 1M |
|---|---|---|---:|---:|---:|---:|---:|
| *World* | | | *608×342* | *1950×1097* | *6166×3469* | *9765×5493* | *19508×10973* |
| *Touching pairs* | | | *864* | *9,160* | *90,562* | *226,354* | *904,349* |
| QuadTree | Query each | 1 | 0.55 | 7.1 | 79.6 | 225 | 2,026 |
| LooseQuadTree | Query each | 1 | 0.36 | 4.9 | 67.6 | 211 | 1,714 |
| LooseTightGrid | Query each | 1 | 0.28 | 3.4 | 40.3 | 114 | 1,014 |
| UniformGrid | Query each | 1 | 0.24 | 2.8 | 35.1 | 102 | 845 |
| LayeredGrid | ForEachPair | 1 | 0.13 | 1.3 | 15.4 | 45.4 | 285 |
| LayeredGrid | ForEachNeighbor | 4 | 0.08 | 0.76 | 8.3 | 23.1 | 109 |
| LayeredGrid | ForEachNeighbor | 14 | **0.06** | 0.49 | 4.6 | 11.9 | 54.8 |
| PackedGrid | ForEachPair | 1 | 0.08 | 0.92 | 11.5 | 31.0 | 191 |
| PackedGrid | ForEachNeighbor | 14 (pool) | 0.07 | 0.48 | 4.9 | 13.2 | 63.2 |
| PackedQuadTree | ForEachPair | 1 | 0.13 | 1.5 | 17.4 | 47.1 | 229 |
| PackedQuadTree | ForEachNeighbor | 14 (pool) | 0.13 | 0.68 | 5.3 | 12.9 | 56.7 |
| PackedLayeredGrid | ForEachPair | 1 | 0.10 | 1.0 | 11.6 | 33.9 | 190 |
| PackedLayeredGrid | ForEachNeighbor | 4 | 0.08 | 0.70 | 7.4 | 20.9 | 104 |
| PackedLayeredGrid | ForEachNeighbor | 14 | 0.06 | 0.49 | 4.4 | 12.0 | 75.8 |
| PhasedGrid | ForEachPair | 1 | 0.11 | 0.97 | 10.9 | 29.8 | 165 |
| PhasedGrid | ForEachPairInParallel | 4 | 0.06 | 0.43 | 3.4 | 9.7 | 47.9 |
| PhasedGrid | ForEachPairInParallel | 14 | 0.09 | **0.37** | **1.9** | **4.5** | 19.9 |
| PhasedGrid | ForEachPairInParallel, reordered | 14 | 0.09 | 0.41 | 2.0 | 4.6 | **16.2** |

<details><summary>Update and search separately, at the biggest count each reached</summary>

| Index | Search | Threads | Agents | Update | Search | Total |
|---|---|---|---:|---:|---:|---:|
| QuadTree | Query each | 1 | 1M | 857 | 1,169 | 2,026 |
| LooseQuadTree | Query each | 1 | 1M | 476 | 1,238 | 1,714 |
| LooseTightGrid | Query each | 1 | 1M | 65.8 | 949 | 1,014 |
| UniformGrid | Query each | 1 | 1M | 11.2 | 834 | 845 |
| LayeredGrid | ForEachPair | 1 | 1M | 24.4 | 261 | 285 |
| LayeredGrid | ForEachNeighbor | 4 | 1M | 23.9 | 85.5 | 109 |
| LayeredGrid | ForEachNeighbor | 14 | 1M | 23.8 | 30.9 | 54.8 |
| PackedGrid | ForEachPair | 1 | 1M | 20.9 | 170 | 191 |
| PackedGrid | ForEachNeighbor | 14 (pool) | 1M | 21.3 | 41.9 | 63.2 |
| PackedQuadTree | ForEachPair | 1 | 1M | 31.2 | 198 | 229 |
| PackedQuadTree | ForEachNeighbor | 14 (pool) | 1M | 33.5 | 23.1 | 56.7 |
| PackedLayeredGrid | ForEachPair | 1 | 1M | 45.7 | 144 | 190 |
| PackedLayeredGrid | ForEachNeighbor | 4 | 1M | 47.7 | 56.1 | 104 |
| PackedLayeredGrid | ForEachNeighbor | 14 | 1M | 52.7 | 23.1 | 75.8 |
| PhasedGrid | ForEachPair | 1 | 1M | 28.3 | 137 | 165 |
| PhasedGrid | ForEachPairInParallel | 4 | 1M | 9.3 | 38.5 | 47.9 |
| PhasedGrid | ForEachPairInParallel | 14 | 1M | 4.8 | 15.1 | 19.9 |
| PhasedGrid | ForEachPairInParallel, reordered | 14 | 1M | 5.9 | 10.3 | 16.2 |

</details>

### Clustered

Circles of radius 3, four in five of them bunched into 8 dense clumps and the rest spread thin, over a world covered a quarter over on average. Most grid cells sit empty while a few are packed: the case trees are meant for.

Milliseconds per frame, update plus contact search. Fastest in each column in bold; — means it took over a second at the previous count.

| Index | Search | Threads | 1k | 10k | 100k | 250k | 1M |
|---|---|---|---:|---:|---:|---:|---:|
| *World* | | | *448×252* | *1418×798* | *4484×2522* | *7090×3988* | *14180×7976* |
| *Touching pairs* | | | *1,751* | *17,458* | *179,809* | *450,799* | *1,787,294* |
| QuadTree | Query each | 1 | 0.40 | 6.1 | 115 | 299 | 2,720 |
| LooseQuadTree | Query each | 1 | 0.31 | 4.4 | 72.7 | 214 | 1,699 |
| LooseTightGrid | Query each | 1 | 0.21 | 2.6 | 51.8 | 161 | 1,490 |
| UniformGrid | Query each | 1 | 0.12 | 1.3 | 42.0 | 123 | 1,098 |
| LayeredGrid | ForEachPair | 1 | 0.05 | 0.52 | 11.0 | 37.6 | 204 |
| LayeredGrid | ForEachNeighbor | 4 | 0.04 | 0.38 | 6.7 | 21.2 | 104 |
| LayeredGrid | ForEachNeighbor | 14 | 0.04 | 0.24 | 3.3 | 9.2 | 47.1 |
| PackedGrid | ForEachPair | 1 | 0.04 | 0.41 | 7.1 | 21.3 | 117 |
| PackedGrid | ForEachNeighbor | 14 (pool) | 0.04 | 0.28 | 2.7 | 7.6 | 38.8 |
| PackedQuadTree | ForEachPair | 1 | 0.12 | 1.4 | 19.8 | 54.3 | 271 |
| PackedQuadTree | ForEachNeighbor | 14 (pool) | 0.11 | 0.62 | 5.3 | 14.5 | 61.1 |
| PackedLayeredGrid | ForEachPair | 1 | 0.05 | 0.49 | 8.5 | 24.1 | 132 |
| PackedLayeredGrid | ForEachNeighbor | 4 | **0.04** | 0.40 | 5.3 | 15.3 | 78.0 |
| PackedLayeredGrid | ForEachNeighbor | 14 | 0.04 | **0.24** | 2.9 | 7.7 | 43.0 |
| PhasedGrid | ForEachPair | 1 | 0.05 | 0.45 | 8.0 | 23.9 | 119 |
| PhasedGrid | ForEachPairInParallel | 4 | 0.05 | 0.24 | 1.9 | 7.0 | 38.8 |
| PhasedGrid | ForEachPairInParallel | 14 | 0.07 | 0.26 | 1.2 | **3.8** | 17.7 |
| PhasedGrid | ForEachPairInParallel, reordered | 14 | 0.07 | 0.29 | **1.1** | 3.9 | **15.2** |

<details><summary>Update and search separately, at the biggest count each reached</summary>

| Index | Search | Threads | Agents | Update | Search | Total |
|---|---|---|---:|---:|---:|---:|
| QuadTree | Query each | 1 | 1M | 889 | 1,832 | 2,720 |
| LooseQuadTree | Query each | 1 | 1M | 493 | 1,206 | 1,699 |
| LooseTightGrid | Query each | 1 | 1M | 90.6 | 1,400 | 1,490 |
| UniformGrid | Query each | 1 | 1M | 24.8 | 1,073 | 1,098 |
| LayeredGrid | ForEachPair | 1 | 1M | 18.1 | 186 | 204 |
| LayeredGrid | ForEachNeighbor | 4 | 1M | 19.3 | 84.9 | 104 |
| LayeredGrid | ForEachNeighbor | 14 | 1M | 18.3 | 28.8 | 47.1 |
| PackedGrid | ForEachPair | 1 | 1M | 22.2 | 94.4 | 117 |
| PackedGrid | ForEachNeighbor | 14 (pool) | 1M | 22.8 | 16.0 | 38.8 |
| PackedQuadTree | ForEachPair | 1 | 1M | 35.8 | 235 | 271 |
| PackedQuadTree | ForEachNeighbor | 14 (pool) | 1M | 32.4 | 28.7 | 61.1 |
| PackedLayeredGrid | ForEachPair | 1 | 1M | 26.2 | 106 | 132 |
| PackedLayeredGrid | ForEachNeighbor | 4 | 1M | 25.2 | 52.8 | 78.0 |
| PackedLayeredGrid | ForEachNeighbor | 14 | 1M | 25.1 | 18.0 | 43.0 |
| PhasedGrid | ForEachPair | 1 | 1M | 19.1 | 99.4 | 119 |
| PhasedGrid | ForEachPairInParallel | 4 | 1M | 5.0 | 33.8 | 38.8 |
| PhasedGrid | ForEachPairInParallel | 14 | 1M | 3.1 | 14.6 | 17.7 |
| PhasedGrid | ForEachPairInParallel, reordered | 14 | 1M | 5.3 | 10.0 | 15.2 |

</details>

### A few giants

Circles of radius 3, except one in a thousand of radius 40 to 80, spread evenly over a world covered half over. Tests how each index copes with rare elements far bigger than its cells.

Milliseconds per frame, update plus contact search. Fastest in each column in bold; — means it took over a second at the previous count.

| Index | Search | Threads | 1k | 10k | 100k | 250k | 1M |
|---|---|---|---:|---:|---:|---:|---:|
| *World* | | | *406×228* | *1301×732* | *3717×2091* | *5943×3343* | *11939×6716* |
| *Touching pairs* | | | *786* | *8,269* | *87,444* | *217,201* | *866,233* |
| QuadTree | Query each | 1 | 0.54 | 6.6 | 75.2 | 234 | 1,650 |
| LooseQuadTree | Query each | 1 | 0.37 | 4.8 | 65.4 | 196 | 1,437 |
| LooseTightGrid | Query each | 1 | 0.28 | 3.0 | 36.5 | 113 | 923 |
| UniformGrid | Query each | 1 | 1.9 | 28.7 | 467 | 1,325 | — |
| LayeredGrid | ForEachPair | 1 | 0.07 | 0.69 | 8.0 | 25.8 | 171 |
| LayeredGrid | ForEachNeighbor | 4 | 0.10 | 0.64 | 5.9 | 15.9 | 86.9 |
| LayeredGrid | ForEachNeighbor | 14 | 0.10 | 0.48 | 4.0 | 8.8 | 44.4 |
| PackedGrid | ForEachPair | 1 | 0.05 | 0.49 | 5.6 | 14.9 | 85.0 |
| PackedGrid | ForEachNeighbor | 14 (pool) | **0.04** | **0.36** | 2.7 | 6.8 | 35.9 |
| PackedQuadTree | ForEachPair | 1 | 0.13 | 1.5 | 16.6 | 45.9 | 228 |
| PackedQuadTree | ForEachNeighbor | 14 (pool) | 0.14 | 0.67 | 5.2 | 12.7 | 60.8 |
| PackedLayeredGrid | ForEachPair | 1 | 0.06 | 0.58 | 6.3 | 17.3 | 112 |
| PackedLayeredGrid | ForEachNeighbor | 4 | 0.09 | 0.56 | 4.9 | 12.6 | 64.8 |
| PackedLayeredGrid | ForEachNeighbor | 14 | 0.09 | 0.42 | 3.4 | 7.7 | 36.4 |
| PhasedGrid | ForEachPair | 1 | 0.06 | 0.52 | 5.8 | 15.7 | 103 |
| PhasedGrid | ForEachPairInParallel | 4 | 0.06 | 0.43 | 2.2 | 4.7 | 26.2 |
| PhasedGrid | ForEachPairInParallel | 14 | 0.06 | 0.43 | **1.4** | **2.7** | 12.9 |
| PhasedGrid | ForEachPairInParallel, reordered | 14 | 0.06 | 0.44 | 1.5 | 3.0 | **11.7** |

<details><summary>Update and search separately, at the biggest count each reached</summary>

| Index | Search | Threads | Agents | Update | Search | Total |
|---|---|---|---:|---:|---:|---:|
| QuadTree | Query each | 1 | 1M | 665 | 985 | 1,650 |
| LooseQuadTree | Query each | 1 | 1M | 472 | 965 | 1,437 |
| LooseTightGrid | Query each | 1 | 1M | 87.3 | 836 | 923 |
| UniformGrid | Query each | 1 | 250k | 2.4 | 1,323 | 1,325 |
| LayeredGrid | ForEachPair | 1 | 1M | 21.1 | 150 | 171 |
| LayeredGrid | ForEachNeighbor | 4 | 1M | 20.3 | 66.6 | 86.9 |
| LayeredGrid | ForEachNeighbor | 14 | 1M | 20.8 | 23.6 | 44.4 |
| PackedGrid | ForEachPair | 1 | 1M | 18.2 | 66.8 | 85.0 |
| PackedGrid | ForEachNeighbor | 14 (pool) | 1M | 19.4 | 16.5 | 35.9 |
| PackedQuadTree | ForEachPair | 1 | 1M | 33.0 | 195 | 228 |
| PackedQuadTree | ForEachNeighbor | 14 (pool) | 1M | 36.6 | 24.2 | 60.8 |
| PackedLayeredGrid | ForEachPair | 1 | 1M | 21.9 | 90.4 | 112 |
| PackedLayeredGrid | ForEachNeighbor | 4 | 1M | 21.8 | 43.1 | 64.8 |
| PackedLayeredGrid | ForEachNeighbor | 14 | 1M | 20.2 | 16.2 | 36.4 |
| PhasedGrid | ForEachPair | 1 | 1M | 18.7 | 84.6 | 103 |
| PhasedGrid | ForEachPairInParallel | 4 | 1M | 4.9 | 21.3 | 26.2 |
| PhasedGrid | ForEachPairInParallel | 14 | 1M | 3.7 | 9.3 | 12.9 |
| PhasedGrid | ForEachPairInParallel, reordered | 14 | 1M | 5.9 | 5.8 | 11.7 |

</details>

<!-- results:end -->

## Rerunning

```bash
dotnet run -c Release --project Samples/ALife.Spatial.Benchmarks -- --readme Documentation/Spatial/README.md
```

That takes several minutes and rewrites the [Results](#results) section. Close other busy programs first, and run on mains power. Without `--readme` it prints the tables instead. Other options:

| Option | Does |
|---|---|
| `--quick` | Fewer and shorter frames, for a rough look in under a minute |
| `--scenario <text>` | Only the scenarios whose names contain the text, e.g. `--scenario mixed` |
| `--max <count>` | Skips counts above this, e.g. `--max 100000` |

These numbers are the index alone. To see a whole frame in the demo, drawing included, give it a number of seconds to profile for, e.g. `dotnet run -c Release --project Samples/ALife.Spatial.Demo -- 9 200000 3` (see [ISpatialIndex](ISpatialIndex.md#example) for the other arguments).

To add an index or a way of searching one, add a `Contender` to `Contender.All()` in [Contenders.cs](../../Samples/ALife.Spatial.Benchmarks/Contenders.cs). To add a crowd, add a `Scenario` to `Scenario.All` in [Scenario.cs](../../Samples/ALife.Spatial.Benchmarks/Scenario.cs).

## See also

- [ISpatialIndex](ISpatialIndex.md): the kept-up-to-date indexes, and which to pick
- [IPackedSpatialIndex](Packed/IPackedSpatialIndex.md): the per-frame indexes
- [PhasedGrid](Phased/PhasedGrid.md)
- [Shape](Shapes/Shape.md), [Collide](Collisions/Collide.md) and [RigidBody](Physics/RigidBody.md): the narrow phase and physics the demo's shape modes use
- [Sector](Shapes/Sector.md) and [Overlap](Collisions/Overlap.md): yes-or-no overlap tests, sectors included, which ALife.Core's [CollisionDetector](../Core/Collision/CollisionDetector.md) is built on
- [SpatialCollisionMap](../Core/Collision/SpatialCollisionMap.md): how ALife.Core's `Planet` uses a `LayeredGrid`
