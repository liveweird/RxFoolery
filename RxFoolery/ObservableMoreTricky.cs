using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;

namespace RxFoolery
{
    [TestClass]
    public class ObservableMoreTricky
    {
        [TestMethod]
        public void Generate()
        {
            var scheduler = new TestScheduler();
            var generated = Observable.Generate(0,
                                                i => i < 5,
                                                i => i + 1,
                                                i => i*i);

            var results = new List<int>();

            generated.Timeout(TimeSpan.FromTicks(100),
                              scheduler)
                     .Subscribe(results.Add,
                                e => Assert.Fail("No exception is planned! {0}",
                                                 e),
                                () => { });

            scheduler.Start();

            Check.That(results)
                 .ContainsExactly(new[]
                                  {
                                      0, 1, 4, 9, 16
                                  });
        }

        [TestMethod]
        public void FromDelegate()
        {
            var completed = false;
            var executed = false;
            var returned = false;
            var scheduler = new TestScheduler();
            var delegated = Observable.Start(() => { executed = true; },
                                             scheduler);

            delegated.Timeout(TimeSpan.FromTicks(100),
                              scheduler)
                     .Subscribe(i =>
                                {
                                    returned = true;
                                },
                                e => Assert.Fail("No exception is planned! {0}",
                                                 e),
                                () =>
                                {
                                    completed = true;
                                });

            scheduler.Start();

            Check.That(completed)
                 .IsTrue();

            Check.That(executed)
                 .IsTrue();

            Check.That(returned)
                 .IsTrue();
        }

        protected class TestEventArgs : EventArgs
        {
            private readonly int _a;

            public TestEventArgs(int a)
            {
                _a = a;
            }

            public int A
            {
                get { return _a; }
            }
        }
        protected static event EventHandler<TestEventArgs> TestEvent;

        [TestMethod]
        public void FromEventPattern()
        {
            var scheduler = new TestScheduler();
            var fromEvent = Observable.FromEventPattern<TestEventArgs>(p => TestEvent += p,
                                                                       p => TestEvent -= p,
                                                                       scheduler);

            var results = new List<int>();

            fromEvent.Subscribe(i => results.Add(i.EventArgs.A),
                                e => Assert.Fail("No exception is planned! {0}",
                                                 e),
                                () => { });

            scheduler.Start();

            TestEvent(null,
                      new TestEventArgs(1));
            TestEvent(null,
                      new TestEventArgs(2));
            TestEvent(null,
                      new TestEventArgs(3));

            Check.That(results)
                 .ContainsExactly(new[]
                                  {
                                      1, 2, 3
                                  });
        }

        [TestMethod]
        public void FromTask()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Cancel()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void FilteringWithWhere()
        {
            var scheduler = new TestScheduler();
            var interval = Observable.Interval(TimeSpan.FromSeconds(1),
                                               scheduler);

            var result = new List<long>();

            interval.Take(5)
                    .Where(p => p >= 3)
                    .Take(3)
                    .Subscribe(result.Add);

            scheduler.Start();

            Check.That(result)
                 .IsOnlyMadeOf((long)3,
                               4,
                               5);
        }

        [TestMethod]
        public void Mapping()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Reducing()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Aggregate()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Partition()
        {
            Assert.Fail();
        }

        // select many
        // combining
        // hot & cold observables
        // subscribe on

        [TestMethod]
        public void Subject()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void ReplaySubject()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void BehaviorSubject()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void AsyncSubject()
        {
            Assert.Fail();
        }
    }
}
