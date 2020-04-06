``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18363.752 (1909/November2018Update/19H2)
Intel Core i5-6300U CPU 2.40GHz (Skylake), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=3.1.201
  [Host]     : .NET Core 3.1.3 (CoreCLR 4.700.20.11803, CoreFX 4.700.20.12001), X64 RyuJIT
  Job-XJCNFT : .NET Core 3.1.3 (CoreCLR 4.700.20.11803, CoreFX 4.700.20.12001), X64 RyuJIT

InvocationCount=1  UnrollFactor=1  

```
|                        Method | NumberOfIterations |     Mean |   Error |  StdDev |      Min |      Max |   Median |      Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------------ |------------------- |---------:|--------:|--------:|---------:|---------:|---------:|-----------:|------:|------:|----------:|
|      SendEventsToSingleClient |             100000 | 335.0 ms | 5.28 ms | 9.52 ms | 324.8 ms | 371.1 ms | 333.3 ms | 71000.0000 |     - |     - | 106.81 MB |
|    SendCommentsToSingleClient |             100000 | 193.4 ms | 3.61 ms | 3.86 ms | 184.8 ms | 198.7 ms | 195.0 ms | 37000.0000 |     - |     - |  55.69 MB |
| SendWaitRequestToSingleClient |             100000 | 119.0 ms | 2.35 ms | 2.71 ms | 114.7 ms | 124.0 ms | 119.2 ms | 17000.0000 |     - |     - |   26.7 MB |
