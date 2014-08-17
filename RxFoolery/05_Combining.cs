using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;

namespace RxFoolery
{
    [TestClass]
    public class Combining
    {
        private static IObservable<string> CreateSeq(IScheduler scheduler,
                                                     double delay,
                                                     double interval,
                                                     string prefix,
                                                     int limit)
        {
            return Observable.Interval(TimeSpan.FromSeconds(interval),
                                       scheduler)
                             .Select(p => string.Format("{0}{1}",
                                                        prefix,
                                                        p))
                             .Delay(TimeSpan.FromSeconds(delay),
                                    scheduler)
                             .Take(limit);
        }

        [TestMethod]
        public void Concat()
        {
            var scheduler = new TestScheduler();

            var seq1 = CreateSeq(scheduler,
                                 delay: 0,
                                 interval: 1,
                                 prefix: "A",
                                 limit: 3);

            var seq2 = CreateSeq(scheduler,
                                 delay: 0,
                                 interval: 0.7,
                                 prefix: "B",
                                 limit: 3);

            var result = new List<string>();

            seq1.Concat(seq2)
                .Subscribe(result.Add);

            scheduler.Start();

            Check.That(result)
                 .ContainsExactly("A0","A1","A2","B0","B1","B2");
        }

        [TestMethod]
        public void Amb()
        {
            var scheduler = new TestScheduler();

            var seq1 = CreateSeq(scheduler,
                                 delay: 0.5,
                                 interval: 0.7,
                                 prefix: "A",
                                 limit: 3);

            var seq2 = CreateSeq(scheduler,
                                 delay: 0,
                                 interval: 1,
                                 prefix: "B",
                                 limit: 3);

            var result = new List<string>();

            seq1.Amb(seq2)
                .Subscribe(result.Add);

            scheduler.Start();

            Check.That(result)
                 .ContainsExactly("B0", "B1", "B2");
        }

        [TestMethod]
        public void Merge()
        {
            var scheduler = new TestScheduler();

            var seq1 = CreateSeq(scheduler,
                                 delay: 0.5,
                                 interval: 1,
                                 prefix: "A",
                                 limit: 3);

            var seq2 = CreateSeq(scheduler,
                                 delay: 0,
                                 interval: 1,
                                 prefix: "B",
                                 limit: 3);

            var result = new List<string>();

            seq1.Merge(seq2)
                .TimeInterval(scheduler)
                .Dump("merged")
                .Subscribe(p => result.Add(p.Value));

            scheduler.Start();

            Check.That(result)
                 .ContainsExactly("B0", "A0", "B1", "A1", "B2", "A2");
        }

        [TestMethod]
        public void Switch()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void CombineLatest()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Zip()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void AndThenWhen()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Join()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void GroupJoin()
        {
            Assert.Fail();
        }
    }
}
