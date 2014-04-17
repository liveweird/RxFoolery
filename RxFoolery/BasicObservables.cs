using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RxFoolery
{
    [TestClass]
    public class BasicObservables
    {
        [TestMethod]
        public void Never()
        {
            var scheduler = new TestScheduler();
            var never = Observable.Never<int>();

            never.Timeout(TimeSpan.FromSeconds(30))
                 .Subscribe(i => Assert.Fail("No event's supposed to happen!"),
                            exception => Assert.Fail("No exception is planned!"),
                            () => Assert.Fail("Sequence will never complete!"));

            scheduler.Start();
        }

        [TestMethod]
        public void Empty()
        {
            var timedOut = true;
            var scheduler = new TestScheduler();
            var empty = Observable.Empty<int>();

            empty.Timeout(TimeSpan.FromSeconds(30))
                 .Subscribe(i => Assert.Fail("No event's supposed to happen!"),
                            exception => Assert.Fail("No exception is planned!"),
                            () => { timedOut = false; });

            scheduler.Start();

            Assert.IsFalse(timedOut);
        }

        [TestMethod]
        public void Interval()
        {
            var timedOut = true;
            var scheduler = new TestScheduler();
            var interval = Observable.Interval(TimeSpan.FromSeconds(1),
                                               scheduler)
                                     .Take(5);
            var events = new List<long>();

            interval.Subscribe(events.Add,
                               exception => Assert.Fail("No exception is planned!"),
                               () => { timedOut = false; });

            scheduler.Start();

            Assert.IsFalse(timedOut);
            CollectionAssert.AreEqual(new long[]
                                      {
                                          0, 1, 2, 3, 4
                                      },
                                      events);
        }
    }
}
