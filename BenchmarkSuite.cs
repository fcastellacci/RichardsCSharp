using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace richardsCsharp
{
    abstract class Benchmark
    {
        public string name;
        public int deterministicIterations;
        public bool doWarmup;
        public bool doDeterministic;
        public int minIterations;

        abstract public void run();
    }
    class BenchmarkSuite
    {
        private string name;
        private long reference;
        private Benchmark benchmark;
        private List<double> scores;
        private List<BenchmarkResult> results;


        // Suites of benchmarks consist of a name and the set of benchmarks in
        // addition to the reference timing that the final score will be based
        // on. This way, all scores are relative to a reference run and higher
        // scores implies better performance.
        public BenchmarkSuite(string name, long reference, Benchmark benchmark)
        {
            this.name = name; ;
            this.reference = reference;
            this.benchmark = benchmark;
            this.scores = new List<double>();
            this.results = new List<BenchmarkResult>();
        }
        // Scores are not comparable across versions. Bump the version if
        // you're making changes that will affect that scores, e.g. if you add
        // a new benchmark or change an existing one.
        static int version = '9';

        // Runs all registered benchmark suites and optionally yields between
        // each individual benchmark to avoid running for too long in the
        // context of browsers. Once done, the final score is reported to the
        // runner.
        public void RunSuites()
        {
            RunSingleBenchmark(this.benchmark);
            NotifyResult();
        }

        // Runs a single benchmark for at least a second and computes the
        // average time it takes to run a single iteration.
        public void RunSingleBenchmark(Benchmark benchmark)
        {
            bool doWarmup = benchmark.doWarmup;
            bool doDeterministic = benchmark.doDeterministic;

            // Sets up data in order to skip or not the warmup phase.
            BenchmarkRunData data = new BenchmarkRunData(0, 0);
            if (doWarmup)
            {
                Measure(data, doDeterministic);
                data = new BenchmarkRunData(0, 0);
            }
            while (data.runs < benchmark.minIterations)
            {
                Measure(data, doDeterministic);
            }
             long usec = (data.elapsed * 1000) / data.runs;
            this.NotifyStep(new BenchmarkResult(benchmark, usec));
        }

        public void Measure(BenchmarkRunData data, bool doDeterministic)
        {
            long elapsed = 0;
            Stopwatch sw = new Stopwatch();

            // Run either for 1 second or for the number of iterations specified
            // by minIterations, depending on the config flag doDeterministic.
            sw.Start();
            int runs = 0;
            for (int i = 0; (doDeterministic ?
              i < benchmark.deterministicIterations : elapsed < 1000); i++)
            {
                benchmark.run();
                elapsed = sw.ElapsedMilliseconds;
                runs++;
            }
            if (data != null)
            {
                data.runs += runs;
                data.elapsed += elapsed;
            }
        }

        public double GeometricMean(List<double> numbers)
        {
            double log = 0;
            for (int i = 0; i < numbers.Count; i++)
            {
                log += Math.Log(numbers[i]);
            }
            return Math.Pow(Math.E, log / numbers.Count);
        }

        // Computes the geometric mean of a set of throughput time measurements.
        public double GeometricMeanTime(List<BenchmarkResult> measurements)
        {
            double log = 0;
            for (int i = 0; i < measurements.Count; i++)
            {
                log += Math.Log(measurements[i].timeInMs);
            }
            return Math.Pow(Math.E, log / measurements.Count);
        }

        // Converts a score value to a string with at least three significant
        // digits.
        string FormatScore(double value)
        {
            if (value > 100)
            {
                return value.ToString("0");
            }
            else
            {
                return value.ToString("0.###");
            }
        }

        // Notifies the runner that we're done running a single benchmark in
        // the benchmark suite. This can be useful to report progress.
        public void NotifyStep(BenchmarkResult result)
        {
            this.results.Add(result);
        }


        // Notifies the runner that we're done with running a suite and that
        // we have a result which can be reported to the user if needed.
        public void NotifyResult()
        {
            double mean = GeometricMeanTime(this.results);
            double score = this.reference / mean;
            this.scores.Add(score);
            string formatted = FormatScore(100 * score);
            Console.WriteLine("### SCORE: " + formatted); ;
        }

        // Notifies the runner that running a benchmark resulted in an error.
        public void NotifyError(Exception error)
        {
            Console.WriteLine(this.name + " : ERROR : " + error.StackTrace);
        }
    }
}
