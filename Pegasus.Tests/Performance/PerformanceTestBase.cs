﻿// -----------------------------------------------------------------------
// <copyright file="PerformanceTestBase.cs" company="(none)">
//   Copyright © 2014 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Tests.Performance
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using NUnit.Framework;

    [TestFixture]
    [Category("Performance")]
    public abstract class PerformanceTestBase
    {
        protected readonly TestCaseData[] Methods;
        protected TimeSpan testTargetTime = TimeSpan.FromSeconds(2);
        protected TimeSpan warmupTargetTime = TimeSpan.FromSeconds(0.1);

        private static decimal[] tDistribution = new decimal[]
        {
            0.00m, 0.00m, 12.71m, 4.30m, 3.18m, 2.78m, 2.57m, 2.45m, 2.36m, 2.31m,
            2.26m, 2.23m, 2.20m, 2.18m, 2.16m, 2.14m, 2.13m, 2.12m, 2.11m, 2.10m,
            2.09m, 2.09m, 2.08m, 2.07m, 2.07m, 2.06m, 2.06m, 2.06m, 2.05m, 2.05m,
            2.05m, 2.04m, 2.04m, 2.04m, 2.03m, 2.03m, 2.03m, 2.03m, 2.03m, 2.02m,
            2.02m, 2.02m, 2.02m, 2.02m, 2.02m, 2.02m, 2.01m, 2.01m, 2.01m, 2.01m,
            2.01m, 2.01m, 2.01m, 2.01m, 2.01m, 2.00m, 2.00m, 2.00m, 2.00m, 2.00m,
            2.00m, 2.00m, 2.00m, 2.00m, 2.00m, 2.00m, 2.00m, 2.00m, 2.00m, 2.00m,
            1.99m, 1.99m, 1.99m, 1.99m, 1.99m, 1.99m, 1.99m, 1.99m, 1.99m, 1.99m,
            1.99m, 1.99m, 1.99m, 1.99m, 1.99m, 1.99m, 1.99m, 1.99m, 1.99m, 1.99m,
            1.99m, 1.99m, 1.99m, 1.99m, 1.99m, 1.99m, 1.99m, 1.98m, 1.98m, 1.98m,
            1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m,
            1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m,
            1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m,
            1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m,
            1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m,
            1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.96m
        };

        public PerformanceTestBase()
        {
            this.Methods = (from m in this.GetType().GetMethods()
                            from e in m.GetCustomAttributes(typeof(EvaluateAttribute), inherit: true).Cast<EvaluateAttribute>()
                            select new TestCaseData(m)).ToArray();
        }

        [TestCaseSource("Methods")]
        public void Evaluate(MethodInfo method)
        {
            var action = MakeAction(this, method);

            var measure = new Func<int, RunningStat>(samples =>
            {
                var runningStat = new RunningStat();
                var sw = new Stopwatch();

                while (samples-- > 0)
                {
                    sw.Restart();
                    action();
                    runningStat.Push((decimal)sw.Elapsed.TotalMilliseconds);
                }

                return runningStat;
            });

            var initialTime = measure(1);
            var baseTime = measure(1);

            var warmupSamples = Math.Max(1, (int)Math.Round(this.warmupTargetTime.TotalMilliseconds / (double)baseTime.Mean));
            var warmupTime = measure(warmupSamples);

            var testSamples = Math.Max(30, (int)Math.Round(this.testTargetTime.TotalMilliseconds / (double)warmupTime.Mean));
            var testTime = measure(testSamples);

            PublishResults(initialTime.Mean, baseTime.Mean, warmupSamples, warmupTime.Mean, warmupTime.StandardDeviation, testSamples, testTime.Mean, testTime.StandardDeviation);
        }

        private static string FormatTime(int count, decimal mean, decimal stdDev = 0)
        {
            string suffix;
            decimal rounded;

            if (count > 1 && stdDev != 0)
            {
                var interval = tDistribution[Math.Min(count, tDistribution.Length - 1)] * stdDev;
                var intervalScale = GetScale(interval);

                suffix = "±" + Math.Round(interval, 3 - intervalScale) + "ms";

                var scale = Math.Min(intervalScale, GetScale(mean));

                rounded = Math.Round(mean, 3 - scale);
            }
            else
            {
                suffix = "";
                rounded = Math.Round(mean, 3 - GetScale(mean));
            }

            return rounded + "ms" + suffix;
        }

        private static int GetScale(decimal d)
        {
            return d == 0 ? 0 : (int)Math.Floor(Math.Log10((double)Math.Abs(d))) + 1;
        }

        private static Action MakeAction(object fixture, MethodInfo method)
        {
            return (Action)Expression.Lambda(Expression.Call(Expression.Constant(fixture), method)).Compile();
        }

        private static void PublishResults(decimal initialTime, decimal baseTime, int warmupSamples, decimal warmupMean, decimal warmupStandardDeviation, int testSamples, decimal testMean, decimal testStandardDeviation)
        {
            Trace.WriteLine(string.Format("initialTime: {0}:", FormatTime(1, initialTime)));
            Trace.WriteLine(string.Format("baseTime: {0}:", FormatTime(1, baseTime)));
            Trace.WriteLine(string.Format("warmupSamples: {0}", warmupSamples));
            Trace.WriteLine(string.Format("warmupMean: {0}:", FormatTime(warmupSamples, warmupMean, warmupStandardDeviation)));
            Trace.WriteLine(string.Format("testSamples: {0}", testSamples));
            Trace.WriteLine(string.Format("testMean: {0}:", FormatTime(testSamples, testMean, testStandardDeviation)));

            var testName = TestContext.CurrentContext.Test.FullName;
            var resultsFolder = Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                "performance");
            var outputPath = Path.Combine(
                resultsFolder,
                testName + ".csv");

            var columns = "\"" + testName.Replace("\"", "\"\"") + "\",testSamples,testMax,testMin,testMean,testStandardDeviation,warmupSamples,warmupMean,warmupStandardDeviation,initialTime,baseTime,machine";

            if (File.Exists(outputPath))
            {
                var lines = File.ReadAllLines(outputPath);
                Assume.That(lines.Length, Is.GreaterThanOrEqualTo(1));
                Assume.That(lines[0], Is.EqualTo(columns));
            }
            else
            {
                if (!Directory.Exists(resultsFolder))
                {
                    Directory.CreateDirectory(resultsFolder);
                }

                File.WriteAllLines(outputPath, new[]
                {
                    columns
                });
            }

            var data = new[] { testSamples, testMean, testStandardDeviation, warmupSamples, warmupMean, warmupStandardDeviation, initialTime, baseTime }.Select(d => d.ToString(CultureInfo.InvariantCulture)).ToList();
            data.Insert(0, DateTime.UtcNow.ToString("O").Replace("T", " ").TrimEnd('Z'));
            data.Insert(2, "\"=INDIRECT(ADDRESS(ROW(),5))+CONFIDENCE(0.05,INDIRECT(ADDRESS(ROW(),6)),INDIRECT(ADDRESS(ROW(),2)))\"");
            data.Insert(3, "\"=INDIRECT(ADDRESS(ROW(),5))-CONFIDENCE(0.05,INDIRECT(ADDRESS(ROW(),6)),INDIRECT(ADDRESS(ROW(),2)))\"");
            data.Insert(11, Environment.MachineName);
            File.AppendAllLines(outputPath, new[]
            {
                string.Join(",", data),
            });
        }

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
        protected class EvaluateAttribute : Attribute
        {
        }

        private class RunningStat
        {
            private int n = 0;
            private decimal oldM, newM, oldS, newS;

            public int Count
            {
                get { return this.n; }
            }

            public decimal Mean
            {
                get { return this.n > 0 ? this.newM : 0.0m; }
            }

            public decimal StandardDeviation
            {
                get { return (decimal)Math.Sqrt((double)this.Variance); }
            }

            public decimal Variance
            {
                get { return this.n > 1 ? this.newS / (this.n - 1) : 0.0m; }
            }

            public void Clear()
            {
                this.n = 0;
            }

            public void Push(decimal value)
            {
                this.n++;

                // See Knuth TAOCP vol 2, 3rd edition, page 232
                if (this.n == 1)
                {
                    this.oldM = this.newM = value;
                    this.oldS = 0.0m;
                }
                else
                {
                    this.newM = this.oldM + (value - this.oldM) / this.n;
                    this.newS = this.oldS + (value - this.oldM) * (value - this.newM);

                    // set up for next iteration
                    this.oldM = this.newM;
                    this.oldS = this.newS;
                }
            }
        }
    }
}
