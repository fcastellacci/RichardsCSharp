using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace richardsCsharp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0] == "--richards")
                {
                    BenchmarkSuite benchmarkSuite = new BenchmarkSuite("Richards", 35302, new Richards("Richards", true, false, 8200, 32));
                    benchmarkSuite.RunSuites();
                } else if (args[0] == "--gaussian-blur")
                {
                    GaussianBlur gaussianBlur = new GaussianBlur();
                    gaussianBlur.buildKernel();
                    gaussianBlur.run();
                }
            } else
            {
                BenchmarkSuite benchmarkSuite = new BenchmarkSuite("Richards", 35302, new Richards("Richards", true, false, 8200, 32));
                benchmarkSuite.RunSuites();
            }
        }
    }
}
