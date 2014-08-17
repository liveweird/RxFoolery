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
                                                     double seconds,
                                                     string prefix,
                                                     int threshold)
        {
            return Observable.Interval(TimeSpan.FromSeconds(seconds),
                                       scheduler)
                             .Select(p => string.Format("{0}{1}",
                                                        prefix,
                                                        p))
                             .Take(threshold);
        }

        [TestMethod]
        public void Concat()
        {
            var scheduler = new TestScheduler();

            var seq1 = CreateSeq(scheduler,
                                 0,
                                 1,
                                 "A",
                                 3);

            var seq2 = CreateSeq(scheduler,
                     0,
                     0.7,
                     "B",
                     3);

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
            Assert.Fail();
        }

        [TestMethod]
        public void Merge()
        {
            Assert.Fail();
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
