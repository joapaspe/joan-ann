
using System;
using System.Collections.Generic;
using System.IO;

namespace ANN
{
    class testNeuron
    {
        static void Main()
        {
            Network xorNet = new Network("res/mininet.txt");
            Console.WriteLine(xorNet.output.ToString());

            double[][] testMatrix = new double[][]
            {
                new double[] { 0.0, 0.0 },
                new double[] { 0.0, 1.0 },
                new double[] { 1.0, 0.0 },
                new double[] { 1.0, 1.0 }
            };

            foreach (double[] sample in testMatrix)
            {
                xorNet.computeNetwork(sample);
                Console.WriteLine($"Input {string.Join(", ", sample)}, output {xorNet.output}");
            }

            // myANN.doFeedForward();
            /*
            double[] v = new double[2];
            v[0] = 0;
            v[1] = 0;
            myANN.computeNetwork(v);
            Console.WriteLine(myANN.output.ToString());
            */

            Console.Read();
        }
    }
}
