namespace richardsCsharp
{
    internal class BenchmarkResult
    {
        public Benchmark benchmark;
        public long timeInMs;

        public BenchmarkResult(Benchmark benchmark, long timeInMs)
        {
            this.benchmark = benchmark;
            this.timeInMs = timeInMs;
        }
    }
}