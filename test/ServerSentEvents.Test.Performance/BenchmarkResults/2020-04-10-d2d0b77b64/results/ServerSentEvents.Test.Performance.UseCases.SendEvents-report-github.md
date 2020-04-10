``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18363.752 (1909/November2018Update/19H2)
Intel Core i5-6300U CPU 2.40GHz (Skylake), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=3.1.201
  [Host]     : .NET Core 3.1.3 (CoreCLR 4.700.20.11803, CoreFX 4.700.20.12001), X64 RyuJIT
  Job-PIBPHL : .NET Core 3.1.3 (CoreCLR 4.700.20.11803, CoreFX 4.700.20.12001), X64 RyuJIT

InvocationCount=1  UnrollFactor=1  

```
|                        Method | NumberOfIterations |     Mean |   Error |  StdDev |      Min |      Max |   Median |      Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------------ |------------------- |---------:|--------:|--------:|---------:|---------:|---------:|-----------:|------:|------:|----------:|
|      SendEventsToSingleClient |             100000 | 310.6 ms | 6.13 ms | 6.56 ms | 298.8 ms | 319.2 ms | 314.0 ms | 68000.0000 |     - |     - | 102.25 MB |
|    SendCommentsToSingleClient |             100000 | 188.7 ms | 3.76 ms | 5.26 ms | 181.0 ms | 201.4 ms | 190.2 ms | 34000.0000 |     - |     - |  51.12 MB |
| SendWaitRequestToSingleClient |             100000 | 114.2 ms | 2.28 ms | 3.12 ms | 110.1 ms | 119.2 ms | 114.2 ms | 17000.0000 |     - |     - |   26.7 MB |
