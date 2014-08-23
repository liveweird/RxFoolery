using System;
using System.Collections.Generic;
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
            var scheduler = new TestScheduler();

            var seq2 = CreateSeq(scheduler,
                                 delay: 0,
                                 interval: 2.5,
                                 prefix: "A",
                                 limit: 3)
                .Select(p => CreateSeq(scheduler,
                                       delay: 0,
                                       interval: 1,
                                       prefix: p,
                                       limit: 3));

            var result = new List<string>();

            seq2.Switch()
                .TimeInterval(scheduler)
                .Dump("switched")
                .Subscribe(p => result.Add(p.Value));

            scheduler.Start();

            Check.That(result)
                 .ContainsExactly("A00", "A01", "A10", "A11", "A20", "A21", "A22");
        }

        [TestMethod]
        public void CombineLatest()
        {
            var scheduler = new TestScheduler();

            var seq1 = CreateSeq(scheduler,
                                 delay: 0.5,
                                 interval: 1,
                                 prefix: "A",
                                 limit: 3);

            var seq2 = CreateSeq(scheduler,
                                 delay: 0,
                                 interval: 0.7,
                                 prefix: "B",
                                 limit: 3);

            var result = new List<string>();

            seq1.CombineLatest(seq2, (p,q) => string.Format("{0}-{1}",
                                                            p,
                                                            q))
                .TimeInterval(scheduler)
                .Dump("combined")
                .Subscribe(p => result.Add(p.Value));

            scheduler.Start();

            Check.That(result)
                 .ContainsExactly("A0-B1", "A0-B2", "A1-B2", "A2-B2");
        }

        [TestMethod]
        public void Zip()
        {
            var scheduler = new TestScheduler();

            var seq1 = CreateSeq(scheduler,
                                 delay: 0.5,
                                 interval: 1,
                                 prefix: "A",
                                 limit: 3);

            var seq2 = CreateSeq(scheduler,
                                 delay: 0,
                                 interval: 0.7,
                                 prefix: "B",
                                 limit: 4);

            var result = new List<string>();

            seq1.Zip(seq2, (p, q) => string.Format("{0}-{1}",
                                                            p,
                                                            q))
                .TimeInterval(scheduler)
                .Dump("zipped")
                .Subscribe(p => result.Add(p.Value));

            scheduler.Start();

            Check.That(result)
                 .ContainsExactly("A0-B0", "A1-B1", "A2-B2");
        }

        [TestMethod]
        public void AndThenWhen()
        {
            var scheduler = new TestScheduler();

            var seq1 = CreateSeq(scheduler,
                                 delay: 0.5,
                                 interval: 1,
                                 prefix: "A",
                                 limit: 3);

            var seq2 = CreateSeq(scheduler,
                                 delay: 0,
                                 interval: 0.7,
                                 prefix: "B",
                                 limit: 3);

            var seq3 = CreateSeq(scheduler,
                                 delay: 1.0,
                                 interval: 0.3,
                                 prefix: "C",
                                 limit: 5);

            var result = new List<string>();

            Observable.When(seq1.And(seq2)
                                .And(seq3)
                                .Then((p,
                                       q,
                                       r) => string.Format("{0}-{1}-{2}",
                                                           p,
                                                           q,
                                                           r)))
                      .TimeInterval(scheduler)
                      .Dump("merged")
                      .Subscribe(p => result.Add(p.Value));

            scheduler.Start();

            Check.That(result)
                 .ContainsExactly("A0-B0-C0", "A1-B1-C1", "A2-B2-C2");
        }

        [TestMethod]
        public void Join()
        {
            var scheduler = new TestScheduler();

            var seq1 = CreateSeq(scheduler,
                                 delay: 0.5,
                                 interval: 1,
                                 prefix: "A",
                                 limit: 3);

            var seq2 = CreateSeq(scheduler,
                                 delay: 0,
                                 interval: 0.7,
                                 prefix: "B",
                                 limit: 3);

            var result = new List<string>();

            seq1.Join(seq2,
                      p => Observable.Timer(TimeSpan.FromSeconds(0.5),
                                            scheduler),
                      p => Observable.Timer(TimeSpan.FromSeconds(0.5),
                                            scheduler),
                      (p,
                       q) => string.Format("{0}-{1}",
                                           p,
                                           q))
                .TimeInterval(scheduler)
                .Dump("joined")
                .Subscribe(p => result.Add(p.Value));

            scheduler.Start();

            Check.That(result)
                 .ContainsExactly("A0-B1", "A1-B2");
        }
    }
}
