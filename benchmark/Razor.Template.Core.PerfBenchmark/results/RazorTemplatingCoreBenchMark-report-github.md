```

BenchmarkDotNet v0.15.1, Windows 11 (10.0.26100.4349/24H2/2024Update/HudsonValley)
Unknown processor
.NET SDK 9.0.301
  [Host]     : .NET 8.0.16 (8.0.1625.21506), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.16 (8.0.1625.21506), X64 RyuJIT AVX2


```
| Method                   | Mean     | Error    | StdDev   | Gen0   | Allocated |
|------------------------- |---------:|---------:|---------:|-------:|----------:|
| RenderViewWithModelAsync | 30.69 μs | 0.781 μs | 2.277 μs | 5.6152 |  23.16 KB |
