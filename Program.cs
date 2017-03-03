using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace richardsCsharp
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSuite benchmarkSuite = new BenchmarkSuite("Richards", 35302, new Richards("Richards", true, false, 8200, 32));
            benchmarkSuite.RunSuites();
        }
    }
}
