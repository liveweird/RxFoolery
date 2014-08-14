using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading;
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
            var scheduler = new TestScheduler();
            var observable = Observable.Interval(TimeSpan.FromSeconds(1),
                                                 scheduler)
                                       .Sample(TimeSpan.FromSeconds(3),
                                               scheduler)
                                       .Take(3);

            var result = new List<long>();
            observable.Subscribe(result.Add);

            scheduler.Start();

            Check.That(result)
                 .ContainsExactly(1L,
                                  4,
                                  7);
        }

        [TestMethod]
        public void Throttle()
        {
            var scheduler = new TestScheduler();
            var observable = scheduler.CreateHotObservable(new Recorded<Notification<long>>(0,
                                                                                            Notification.CreateOnNext(1L)),
                                                           new Recorded<Notification<long>>(100,
                                                                                            Notification.CreateOnNext(2L)),
                                                           new Recorded<Notification<long>>(500,
                                                                                            Notification.CreateOnNext(3L)),
                                                           new Recorded<Notification<long>>(800,
                                                                                            Notification.CreateOnNext(4L)));
            
            var throttled = observable.Throttle(TimeSpan.FromMilliseconds(250),
                                                scheduler);

            var result = new List<long>();
            throttled.Subscribe(result.Add);

            scheduler.Start();

            Check.That(result)
                 .ContainsExactly(3, 4);

        }

        [TestMethod]
        public void GroupBy()
        {
            var scheduler = new TestScheduler();
            var observable = Observable.Interval(TimeSpan.FromSeconds(1),
                                                 scheduler)
                                       .Take(10);

            var grouped = observable.GroupBy(p => p % 4);
            
            var result = new Dictionary<long, List<long>>
                         {
                             { 0L, new List<long>() },
                             { 1L, new List<long>() },
                             { 2L, new List<long>() },
                             { 3L, new List<long>() }
                         };

            grouped.Subscribe(g => g.Subscribe(h => result[g.Key].Add(h)));

            scheduler.Start();

            Check.That(result[0])
                 .ContainsExactly(0L,
                                  4,
                                  8);

            Check.That(result[1])
                 .ContainsExactly(1L,
                                  5,
                                  9);

            Check.That(result[2])
                 .ContainsExactly(2L,
                                  6);

            Check.That(result[3])
                 .ContainsExactly(3L,
                                  7);
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