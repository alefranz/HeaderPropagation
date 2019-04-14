using BenchmarkDotNet.Running;
using System;

namespace HeaderPropagationBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<MessageHandlerBenchmark>();
        }
    }
}
