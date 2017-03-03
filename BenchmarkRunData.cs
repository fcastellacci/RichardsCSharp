namespace richardsCsharp
{
    public class BenchmarkRunData
    {
        internal long elapsed;
        internal int runs;

        public BenchmarkRunData(int runs, int elapsed)
        {
            this.runs = runs;
            this.elapsed = elapsed;
        }
    }
}