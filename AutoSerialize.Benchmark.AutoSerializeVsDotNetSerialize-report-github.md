``` ini

BenchmarkDotNet=v0.11.1, OS=Windows 10.0.17134.228 (1803/April2018Update/Redstone4)
Intel Pentium CPU G3250 3.20GHz, 1 CPU, 2 logical and 2 physical cores
.NET Core SDK=2.1.202
  [Host]     : .NET Core 2.0.9 (CoreCLR 4.6.26614.01, CoreFX 4.6.26614.01), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.9 (CoreCLR 4.6.26614.01, CoreFX 4.6.26614.01), 64bit RyuJIT


```
|        Method |    N |          Mean |      Error |     StdDev | Rank |
|-------------- |----- |--------------:|-----------:|-----------:|-----:|
| **AutoSerialize** |    **1** |      **4.487 us** |  **0.0392 us** |  **0.0347 us** |    **1** |
|        DotNet |    1 |     25.443 us |  0.5109 us |  0.7802 us |    3 |
| **AutoSerialize** |   **10** |     **24.248 us** |  **0.1265 us** |  **0.1183 us** |    **2** |
|        DotNet |   10 |    136.728 us |  0.6791 us |  0.6353 us |    4 |
| **AutoSerialize** |  **100** |    **222.249 us** |  **1.6131 us** |  **1.4300 us** |    **5** |
|        DotNet |  100 |  1,258.465 us | 12.0459 us | 11.2677 us |    6 |
| **AutoSerialize** | **1000** |  **2,230.973 us** | **13.4219 us** | **12.5548 us** |    **7** |
|        DotNet | 1000 | 12,461.599 us | 66.0460 us | 61.7795 us |    8 |
