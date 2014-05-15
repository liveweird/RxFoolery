using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;

namespace RxFoolery
{
    [TestClass]
    public class TimeBasedObservables
    {
        [TestMethod]
        public void Interval()
        {
            var scheduler = new TestScheduler();
            var interval = Observable.Interval(TimeSpan.FromSeconds(1),
                                               scheduler)
                                     .Take(5);
            var events = new List<long>();

            interval.Subscribe(events.Add,
                               e => Assert.Fail("No exception is planned! {0}", e),
                               () => { });

            scheduler.Start();

            Check.That(events)
                 .IsOnlyMadeOf(
                               (long)0,
                               1,
                               2,
                               3,
                               4);
        }

        [TestMethod]
        public void Timer()
        {
            var scheduler = new TestScheduler();
            var timer = Observable.Timer(TimeSpan.FromSeconds(1),
                                         scheduler);
            var result = long.MinValue;
            var completed = false;

            timer.Subscribe(i => { result = i; },
                            e => Assert.Fail("No exception is planned! {0}",
                                             e),
                            () => { completed = true; });

            scheduler.Start();

            Check.That(result)
                 .IsEqualTo(0);

            Check.That(completed)
                 .IsTrue();
        }
    }
}