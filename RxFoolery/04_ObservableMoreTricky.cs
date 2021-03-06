﻿using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
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

            var observable = generated.Timeout(TimeSpan.FromTicks(100),
                                               scheduler);

            //observable.Subscribe(results.Add,
            //                     e => Assert.Fail("No exception is planned! {0}",
            //                                      e),
            //                     () => { });

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

            var observable = delegated.Timeout(TimeSpan.FromTicks(100),
                                               scheduler);

            //observable.Subscribe(i => { returned = true; },
            //                     e => Assert.Fail("No exception is planned! {0}",
            //                                      e),
            //                     () => { completed = true; });

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

            //using (fromEvent.Subscribe(i => results.Add(i.EventArgs.A),
            //                           e => Assert.Fail("No exception is planned! {0}",
            //                                            e),
            //                           () => { }))
            {
                scheduler.Start();

                TestEvent(null,
                          new TestEventArgs(1));
                TestEvent(null,
                          new TestEventArgs(2));
                TestEvent(null,
                          new TestEventArgs(3));
            }

            Check.That(results)
                 .ContainsExactly(new[]
                                  {
                                      1, 2, 3
                                  });
        }

        [TestMethod]
        public void FromTask()
        {
            var executed = false;
            var completed = false;
            var returned = false;

            var scheduler = new TestScheduler();
            var fromTask = Observable.FromAsync(() => Task.Run(() => { executed = true; }));
            var observable = fromTask.Timeout(TimeSpan.FromSeconds(100),
                                              scheduler);

            //using (observable.Subscribe(i => { returned = true; },
            //                            e => Assert.Fail("No exception is planned! {0}",
            //                                             e),
            //                            () => { completed = true; }))
            {
                scheduler.Start();
            }

            Check.That(completed)
                 .IsTrue();

            Check.That(executed)
                 .IsTrue();

            Check.That(returned)
                 .IsTrue();
        }

        [TestMethod]
        public void FromEnumerable()
        {
            IEnumerable<long> source = new List<long>
                                       {
                                           0,
                                           666,
                                           -23,
                                           (long) 3.75,
                                           (long) -99.1
                                       };

            var testScheduler = new TestScheduler();
            var observable = source.ToObservable(testScheduler);

            var results = new List<long>();
            //observable.Subscribe(results.Add);

            testScheduler.Start();

            Check.That(source)
                 .ContainsExactly(results);
        }

        protected static event EventHandler<EventArgs> CancelEvent;

        [TestMethod]
        public void CancelWithEvent()
        {
            var completed = false;

            var scheduler = new TestScheduler();
            var observable = Observable.Interval(TimeSpan.FromSeconds(1),
                                                 scheduler);

            var fromEvent = Observable.FromEventPattern<EventArgs>(p => CancelEvent += p,
                                                                   p => CancelEvent -= p,
                                                                   scheduler);

            var cancellable = observable.TakeUntil(fromEvent);

            var results = new List<long>();

            //using (cancellable.Subscribe(results.Add,
            //                             e => Assert.Fail("No exception is planned! {0}",
            //                                              e),
            //                             () => { completed = true; }))
            {                
                scheduler.AdvanceBy(TimeSpan.FromSeconds(3.5).Ticks);

                CancelEvent(null,
                            new EventArgs());

                scheduler.AdvanceBy(TimeSpan.FromSeconds(3).Ticks);
            }

            Check.That(completed)
                 .IsTrue();

            Check.That(results)
                 .ContainsExactly(new long[]
                                  {
                                      0, 1, 2
                                  });
        }

        [TestMethod]
        public void ReduceWithWhere()
        {
            var scheduler = new TestScheduler();
            var interval = Observable.Interval(TimeSpan.FromSeconds(1),
                                               scheduler);

            var result = new List<long>();

            var observable = interval.Take(5)
                                     .Where(p => p >= 3);

            //observable.Subscribe(result.Add);

            scheduler.Start();

            Check.That(result)
                 .Not
                 .IsEmpty();

            Check.That(result)
                 .IsOnlyMadeOf((long)3,
                               4,
                               5);
        }

        [TestMethod]
        public void ReduceWithDistinct()
        {
            var scheduler = new TestScheduler();
            var created = Observable.Create<int>(o =>
            {
                o.OnNext(1);
                o.OnNext(2);
                o.OnNext(1);
                o.OnNext(3);
                o.OnNext(2);
                o.OnNext(2);
                o.OnCompleted();

                return Disposable.Create(() => { });
            });
            var results = new List<int>();

            var observable = created.Distinct();

            //observable.Subscribe(results.Add,
            //                     e => Assert.Fail("No exception is planned! {0}",
            //                                      e),
            //                     () => { });

            scheduler.Start();

            Check.That(results)
                 .ContainsExactly(new[]
                                  {
                                      1, 2, 3
                                  });
        }

        [TestMethod]
        public void ReduceWithDistinctUntilChanged()
        {
            var scheduler = new TestScheduler();
            var created = Observable.Create<int>(o =>
            {
                o.OnNext(1);
                o.OnNext(2);
                o.OnNext(1);
                o.OnNext(1);
                o.OnNext(1);
                o.OnNext(3);
                o.OnNext(2);
                o.OnNext(2);
                o.OnCompleted();

                return Disposable.Create(() => { });
            });
            var results = new List<int>();

            var observable = created.DistinctUntilChanged();

            //observable.Subscribe(results.Add,
            //                     e => Assert.Fail("No exception is planned! {0}",
            //                                      e),
            //                     () => { });

            scheduler.Start();

            Check.That(results)
                 .ContainsExactly(new[]
                                  {
                                      1, 2, 1, 3, 2
                                  });
        }

        [TestMethod]
        public void InspectWithContains()
        {
            var scheduler = new TestScheduler();
            var created = Observable.Create<int>(o =>
                                                 {
                                                     o.OnNext(1);
                                                     o.OnNext(2);
                                                     o.OnNext(1);
                                                     o.OnNext(1);
                                                     o.OnNext(1);
                                                     o.OnNext(3);
                                                     o.OnNext(2);
                                                     o.OnNext(2);
                                                     o.OnCompleted();

                                                     return Disposable.Create(() => { });
                                                 });

            var result1 = new List<bool>();
            var result2 = new List<bool>();

            var filtered1 = created.Contains(2);
            //filtered1.Subscribe(result1.Add,
            //                    e => Assert.Fail("No exception is planned! {0}",
            //                                     e),
            //                    () => { });

            var filtered2 = created.Contains(6);
            //filtered2.Subscribe(result2.Add,
            //                    e => Assert.Fail("No exception is planned! {0}",
            //                                     e),
            //                    () => { });

            scheduler.Start();

            Check.That(result1)
                 .ContainsExactly(new[]
                                  {
                                      true
                                  });

            Check.That(result2)
                 .ContainsExactly(new[]
                                  {
                                      false
                                  });
        }

        [TestMethod]
        public void InspectWithAny()
        {
            var scheduler = new TestScheduler();
            var created = Observable.Create<int>(o =>
            {
                o.OnNext(1);
                o.OnNext(2);
                o.OnNext(1);
                o.OnNext(1);
                o.OnNext(1);
                o.OnNext(3);
                o.OnNext(2);
                o.OnNext(2);
                o.OnCompleted();

                return Disposable.Create(() => { });
            });

            var result1 = new List<bool>();
            var result2 = new List<bool>();

            var filtered1 = created.Any(p => p > 2);
            //filtered1.Subscribe(result1.Add,
            //                    e => Assert.Fail("No exception is planned! {0}",
            //                                     e),
            //                    () => { });

            var filtered2 = created.Any(p => p < 1);
            //filtered2.Subscribe(result2.Add,
            //                    e => Assert.Fail("No exception is planned! {0}",
            //                                     e),
            //                    () => { });

            scheduler.Start();

            Check.That(result1)
                 .ContainsExactly(new[]
                                  {
                                      true
                                  });

            Check.That(result2)
                 .ContainsExactly(new[]
                                  {
                                      false
                                  });
        }
    }
}
