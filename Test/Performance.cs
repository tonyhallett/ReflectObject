﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Test
{
    //https://stackoverflow.com/questions/969290/exact-time-measurement-for-performance-testing/16157458#16157458
    public class Clock
    {
        interface IStopwatch
        {
            bool IsRunning { get; }
            TimeSpan Elapsed { get; }

            void Start();
            void Stop();
            void Reset();
        }



        class TimeWatch : IStopwatch
        {
            Stopwatch stopwatch = new Stopwatch();

            public TimeSpan Elapsed
            {
                get { return stopwatch.Elapsed; }
            }

            public bool IsRunning
            {
                get { return stopwatch.IsRunning; }
            }



            public TimeWatch()
            {
                if (!Stopwatch.IsHighResolution)
                    throw new NotSupportedException("Your hardware doesn't support high resolution counter");

                //prevent the JIT Compiler from optimizing Fkt calls away
                long seed = Environment.TickCount;

                //use the second Core/Processor for the test
                Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(2);

                //prevent "Normal" Processes from interrupting Threads
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

                //prevent "Normal" Threads from interrupting this thread
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
            }



            public void Start()
            {
                stopwatch.Start();
            }

            public void Stop()
            {
                stopwatch.Stop();
            }

            public void Reset()
            {
                stopwatch.Reset();
            }
        }



        class CpuWatch : IStopwatch
        {
            TimeSpan startTime;
            TimeSpan endTime;
            bool isRunning;



            public TimeSpan Elapsed
            {
                get
                {
                    if (IsRunning)
                        throw new NotImplementedException("Getting elapsed span while watch is running is not implemented");

                    return endTime - startTime;
                }
            }

            public bool IsRunning
            {
                get { return isRunning; }
            }



            public void Start()
            {
                startTime = Process.GetCurrentProcess().TotalProcessorTime;
                isRunning = true;
            }

            public void Stop()
            {
                endTime = Process.GetCurrentProcess().TotalProcessorTime;
                isRunning = false;
            }

            public void Reset()
            {
                startTime = TimeSpan.Zero;
                endTime = TimeSpan.Zero;
            }
        }



        public static void BenchmarkTime(Action action, int iterations = 10000)
        {
            Benchmark<TimeWatch>(action, iterations);
        }

        static double[] Benchmark<T>(Action action, int iterations) where T : IStopwatch, new()
        {
            //clean Garbage
            GC.Collect();

            //wait for the finalizer queue to empty
            GC.WaitForPendingFinalizers();

            //clean Garbage
            GC.Collect();

            //warm up
            action();

            var stopwatch = new T();
            var timings = new double[5];
            for (int i = 0; i < timings.Length; i++)
            {
                stopwatch.Reset();
                stopwatch.Start();
                for (int j = 0; j < iterations; j++)
                    action();
                stopwatch.Stop();
                timings[i] = stopwatch.Elapsed.TotalMilliseconds;
            }
            return timings;
        }

        static double BenchmarkMean<T>(Action action, int iterations = 10000) where T : IStopwatch, new()
        {
            return Benchmark<T>(action, iterations).NormalizedMean();
        }

        public static double BenchmarkMeanCpu(Action action, int iterations = 10000)
        {
            return BenchmarkMean<CpuWatch>(action, iterations);
        }

        public static double BenchmarkMeanTime(Action action, int iterations = 10000)
        {
            return BenchmarkMean<TimeWatch>(action, iterations);
        }

        public static void BenchmarkCpu(Action action, int iterations = 10000)
        {
            Benchmark<CpuWatch>(action, iterations);
        }
    }

    public static class NormalizeExtensions
    {
        public static double NormalizedMean(this ICollection<double> values)
        {
            if (values.Count == 0)
                return double.NaN;

            var deviations = values.Deviations().ToArray();
            var meanDeviation = deviations.Sum(t => Math.Abs(t.Item2)) / values.Count;
            return deviations.Where(t => t.Item2 > 0 || Math.Abs(t.Item2) <= meanDeviation).Average(t => t.Item1);
        }

        public static IEnumerable<Tuple<double, double>> Deviations(this ICollection<double> values)
        {
            if (values.Count == 0)
                yield break;

            var avg = values.Average();
            foreach (var d in values)
                yield return Tuple.Create(d, avg - d);
        }
    }
}
