using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;

namespace RxFoolery
{
    [TestClass]
    public class LetsGetCreative
    {
        [TestMethod]
        public void Buffering()
        {
            var scheduler = new TestScheduler();
            var result = new List<IEnumerable<long>>();
            var completed = false;

            var observable = Observable.Interval(TimeSpan.FromSeconds(1),
                                                 scheduler);
            var buffered = observable.Buffer(TimeSpan.FromSeconds(3),
                                             scheduler);
            buffered.Take(3)
                    .Subscribe(result.Add,
                               ex => Assert.Fail("No exception was expected! {0}",
                                                 ex),
                               () => { completed = true; });

            scheduler.Start();

            Check.That(completed)
                 .IsTrue();

            Check.That(result.Count)
                 .IsEqualTo(3);

            Check.That(result[0]).IsOnlyMadeOf(
                new long[] { 0, 1 }
            );

            Check.That(result[1]).IsOnlyMadeOf(
                new long[] { 2, 3, 4 }
            );

            Check.That(result[2]).IsOnlyMadeOf(
                new long[] { 5, 6, 7 }
            );
        }

        [TestMethod]
        public void Average()
        {
            var scheduler = new TestScheduler();
            var result = new List<double>();
            var completed = false;
            var observable = Observable.Interval(TimeSpan.FromSeconds(1),
                                                 scheduler);

            observable.Take(6)
                      .Dump("x")
                      .Average()
                      .Subscribe(result.Add,
                               ex => Assert.Fail("No exception was expected! {0}",
                                                 ex),
                               () => { completed = true; });

            scheduler.Start();

            Check.That(completed)
                 .IsTrue();

            Check.That(result)
                 .IsOnlyMadeOf(2.5);
        }

        [TestMethod]
        public void AverageWithinWindow()
        {
            var scheduler = new TestScheduler();
            var result = new List<double>();
            var completed = false;
            var observable = Observable.Interval(TimeSpan.FromSeconds(1),
                                                 scheduler);
            var buffered = observable.Buffer(TimeSpan.FromSeconds(3),
                                             scheduler);                                     

            buffered.Take(3)
                    .Select(p => p.Average())
                    .Subscribe(result.Add,
                               ex => Assert.Fail("No exception was expected! {0}",
                                                 ex),
                               () => { completed = true; });

            scheduler.Start();

            Check.That(completed)
                 .IsTrue();

            Check.That(result)
                 .IsOnlyMadeOf(0.5, 3, 6);
        }

        [TestMethod]
        public void BufferingWithShift()
        {
            var scheduler = new TestScheduler();
            var result = new List<IEnumerable<long>>();
            var completed = false;

            var observable = Observable.Interval(TimeSpan.FromSeconds(1),
                                                 scheduler)
                                       .Delay(TimeSpan.FromSeconds(1.5),
                                              scheduler);
            var buffered = observable.Buffer(TimeSpan.FromSeconds(3),
                                             scheduler);
            buffered.Take(3)
                    .Dump("x",
                          a => String.Join(",",
                                           a.Select(p => p.ToString(CultureInfo.InvariantCulture))))
                    .Subscribe(result.Add,
                               ex => Assert.Fail("No exception was expected! {0}",
                                                 ex),
                               () => { completed = true; });

            scheduler.Start();

            Check.That(completed)
                 .IsTrue();

            Check.That(result.Count)
                 .IsEqualTo(3);

            Check.That(result[0]).IsOnlyMadeOf(
                new long[] { 0 }
            );

            Check.That(result[1]).IsOnlyMadeOf(
                new long[] { 1, 2, 3 }
            );

            Check.That(result[2]).IsOnlyMadeOf(
                new long[] { 4, 5, 6 }
            );
        }

        [TestMethod]
        public void BufferingWithPreferredBatchSize()
        {
            var scheduler = new TestScheduler();
            var result = new List<IEnumerable<long>>();
            var completed = false;

            var observable = Observable.Interval(TimeSpan.FromSeconds(1),
                                                 scheduler);
            var buffered = observable.Buffer(TimeSpan.FromSeconds(5),
                                             3,
                                             scheduler);

            buffered.Take(3)
                    .Dump("x",
                          a => String.Join(",",
                                           a.Select(p => p.ToString(CultureInfo.InvariantCulture))))
                    .Subscribe(result.Add,
                               ex => Assert.Fail("No exception was expected! {0}",
                                                 ex),
                               () => { completed = true; });

            scheduler.Start();

            Check.That(completed)
                 .IsTrue();

            Check.That(result.Count)
                 .IsEqualTo(3);

            Check.That(result[0]).IsOnlyMadeOf(
                new long[] { 0, 1, 2 }
            );

            Check.That(result[1]).IsOnlyMadeOf(
                new long[] { 3, 4, 5 }
            );

            Check.That(result[2]).IsOnlyMadeOf(
                new long[] { 6, 7, 8 }
            );
        }

        [TestMethod]
        public void BufferingWithPresetBatchSize()
        {
            var scheduler = new TestScheduler();
            var result = new List<IEnumerable<long>>();
            var completed = false;

            var observable = Observable.Interval(TimeSpan.FromSeconds(1),
                                                 scheduler);
            var buffered = observable.Buffer(4);

            buffered.Take(3)
                    .Dump("x",
                          a => String.Join(",",
                                           a.Select(p => p.ToString(CultureInfo.InvariantCulture))))
                    .Subscribe(result.Add,
                               ex => Assert.Fail("No exception was expected! {0}",
                                                 ex),
                               () => { completed = true; });

            scheduler.Start();

            Check.That(completed)
                 .IsTrue();

            Check.That(result.Count)
                 .IsEqualTo(3);

            Check.That(result[0]).IsOnlyMadeOf(
                new long[] { 0, 1, 2, 3 }
            );

            Check.That(result[1]).IsOnlyMadeOf(
                new long[] { 4, 5, 6, 7 }
            );

            Check.That(result[2]).IsOnlyMadeOf(
                new long[] { 8, 9, 10, 11 }
            );
        }

        [TestMethod]
        public void Sample()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Throttle()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void GroupBy()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Materialize()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Dematerialize()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void SelectMany()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Scan()
        {
            Assert.Fail();
        }
    }
}