```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7462/25H2/2025Update/HudsonValley2)
Intel Core i7-10510U CPU 1.80GHz (Max: 2.30GHz), 1 CPU, 8 logical and 4 physical cores
.NET SDK 10.0.101
  [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3


```
| Method                   | Mean     | Error    | StdDev   | Gen0   | Allocated |
|------------------------- |---------:|---------:|---------:|-------:|----------:|
| RenderViewWithModelAsync | 28.73 μs | 0.403 μs | 0.357 μs | 5.8594 |     24 KB |
