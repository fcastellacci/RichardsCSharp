﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace richardsCsharp
{
    class GaussianBlur : Benchmark
    {

        int sigma = 10;
        double[][] kernel;
        int kernelSize;
        double kernelSum;
        int width = 400;
        int height = 267;

        public void buildKernel()
        {
            int ss = sigma * sigma;
            double factor = 2 * Math.PI * ss;
            int i = 0, j;
            kernel = new double[7][];
            kernel[0] = new double[7]; 

            do
            {
                double g = Math.Exp(-(i * i) / (2 * ss)) / factor;
                if (g < 1e-3) break;
                kernel[0][i] = g;
                ++i;
            } while (i < 7);

            kernelSize = i;
            for (j = 1; j < kernelSize; ++j)
            {
                kernel[j] = new double[kernelSize];
                for (i = 0; i < kernelSize; ++i)
                {
                    double g = Math.Exp(-(i * i + j * j) / (2 * ss)) / factor;
                    kernel[j][i] = g;
                }
            }
            kernelSum = 0;
            for (j = 1 - kernelSize; j < kernelSize; ++j)
            {
                for (i = 1 - kernelSize; i < kernelSize; ++i)
                {
                    kernelSum += kernel[Math.Abs(j)][Math.Abs(i)];
                }
            }
        }

        override public void run()
        {
            gaussianBlur();
        }
        public double[] gaussianBlur()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    double r = 0, g = 0, b = 0, a = 0;
                    for (int j = 1 - kernelSize; j < kernelSize; ++j)
                    {
                        if (y + j < 0 || y + j >= height) continue;
                        for (int i = 1 - kernelSize; i < kernelSize; ++i)
                        {
                            if (x + i < 0 || x + i >= width) continue;
                            int idxTerm = 4 * ((y + j) * width + (x + i));
                            int jAbs = Math.Abs(j);
                            int iAbs = Math.Abs(i);
                            r += squidImageData[idxTerm + 0] * kernel[jAbs][iAbs];
                            g += squidImageData[idxTerm + 1] * kernel[jAbs][iAbs];
                            b += squidImageData[idxTerm + 2] * kernel[jAbs][iAbs];
                            a += squidImageData[idxTerm + 3] * kernel[jAbs][iAbs];
                        }
                    }
                    int idxTerm2 = 4 * (y * width + x);
                    squidImageData[idxTerm2 + 0] = r / kernelSum;
                    squidImageData[idxTerm2 + 1] = g / kernelSum;
                    squidImageData[idxTerm2 + 2] = b / kernelSum;
                    squidImageData[idxTerm2 + 3] = a / kernelSum;
                }
            }
            long elapsed = sw.ElapsedMilliseconds;
            Console.WriteLine("### TIME: " + elapsed + " ms");
            return squidImageData;
        }
    }
}