using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;

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
            var exceptionThrown = false;

            never.Timeout(TimeSpan.FromTicks(100),
                          scheduler)
                 .Subscribe(i => Assert.Fail("No event's supposed to happen!"),
                            e =>
                            {
                                exceptionThrown = true;
                                Check.That(e)
                                     .IsInstanceOf<TimeoutException>();
                            },
                            () => Assert.Fail("Sequence will never complete!"));

            Check.That(exceptionThrown)
                 .IsFalse();
 
            scheduler.Start();
            
            Check.That(exceptionThrown)
                 .IsTrue();
        }

        [TestMethod]
        public void Empty()
        {
            var completed = false;
            var scheduler = new TestScheduler();
            var empty = Observable.Empty<int>();

            empty.Timeout(TimeSpan.FromTicks(100),
                          scheduler)
                 .Subscribe(i => Assert.Fail("No event's supposed to happen!"),
                            e => Assert.Fail("No exception is planned! {0}", e),
                            () => { completed = true; });

            Check.That(completed)
                 .IsEqualTo(true);
        }

        [TestMethod]
        public void Return()
        {
            var completed = false;
            var scheduler = new TestScheduler();
            var manual = Observable.Return(4);
            var result = -1;

            manual.Timeout(TimeSpan.FromTicks(100),
                           scheduler)
                  .Subscribe(i => { result = i; },
                             e => Assert.Fail("No exception is planned! {0}", e),
                             () => { completed = true; });

            Check.That(completed)
                 .IsTrue();

            Check.That(result)
                 .IsEqualTo(4);
        }

        [TestMethod]
        public void Throw()
        {
            var scheduler = new TestScheduler();
            var manual = Observable.Throw<Exception>(new Exception("Kabooom!"));
            var thrown = false;

            manual.Timeout(TimeSpan.FromTicks(100), scheduler)
                  .Subscribe(i => Assert.Fail("No value is planned!"),
                             e =>
                             {
                                 thrown = true;
                             },
                             () => Assert.Fail("Sequence is not expected to end normally."));

            Check.That(thrown)
                 .IsTrue();
        }

        [TestMethod]
        public void Create()
        {
            var scheduler = new TestScheduler();
            var created = Observable.Create<int>(o =>
                                                 {
                                                     o.OnNext(3);
                                                     o.OnNext(6);
                                                     o.OnNext(8);
                                                     o.OnCompleted();

                                                     return Disposable.Create(() => { });
                                                 });
            var results = new List<int>();

            created.Timeout(TimeSpan.FromTicks(100),
                            scheduler)
                   .Subscribe(results.Add,
                              e => Assert.Fail("No exception is planned! {0}", e),
                              () => { });
            
            scheduler.Start();

            Check.That(results)
                 .ContainsExactly(new[]
                                  {
                                      3, 6, 8
                                  });
        }

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
                               (long) 0,
                               1,
                               2,
                               3,
                               4);
        }

        [TestMethod]
        public void Unsubscribe()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Filtering()
        {
            var timedOut = true;
            var scheduler = new TestScheduler();
            var interval = Observable.Interval(TimeSpan.FromSeconds(1),
                                               scheduler)
                                     .Take(5);

            interval.Where(p => p > 3);

            Assert.Fail();
        }

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
