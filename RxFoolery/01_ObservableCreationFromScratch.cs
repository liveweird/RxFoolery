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
    public class ObservableCreationFromScratch
    {
        [TestMethod]
        public void Never()
        {
            var scheduler = new TestScheduler();
            var never = Observable.Never<int>();
            var exceptionThrown = false;

            var observable = never.Timeout(TimeSpan.FromTicks(100),
                                           scheduler);

            //observable.Subscribe(i => Assert.Fail("No event's supposed to happen!"),
            //                     e =>
            //                     {
            //                         exceptionThrown = true;
            //                         Check.That(e)
            //                              .IsInstanceOf<TimeoutException>();
            //                     },
            //                     () => Assert.Fail("Sequence will never complete!"));

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

            var observable = empty.Timeout(TimeSpan.FromTicks(100),
                                           scheduler);

            //observable.Subscribe(i => Assert.Fail("No event's supposed to happen!"),
            //                     e => Assert.Fail("No exception is planned! {0}",
            //                                      e),
            //                     () => { completed = true; });

            scheduler.Start();

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

            var observable = manual.Timeout(TimeSpan.FromTicks(100),
                           scheduler);

            //observable.Subscribe(i => { result = i; },
            //                     e => Assert.Fail("No exception is planned! {0}",
            //                                      e),
            //                     () => { completed = true; });

            scheduler.Start();

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

            var observable = manual.Timeout(TimeSpan.FromTicks(100),
                                            scheduler);

            //observable.Subscribe(i => Assert.Fail("No value is planned!"),
            //                     e => { thrown = true; },
            //                     () => Assert.Fail("Sequence is not expected to end normally."));

            scheduler.Start();

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

            var observable = created.Timeout(TimeSpan.FromTicks(100),
                                             scheduler);

            //observable.Subscribe(results.Add,
            //                     e => Assert.Fail("No exception is planned! {0}",
            //                                      e),
            //                     () => { });

            scheduler.Start();

            Check.That(results)
                 .ContainsExactly(new[]
                                  {
                                      3, 6, 8
                                  });
        }

        [TestMethod]
        public void ErrorEndsTheSequence()
        {
            var scheduler = new TestScheduler();
            var created = Observable.Create<int>(o =>
            {
                o.OnNext(3);
                o.OnNext(6);
                o.OnError(new Exception());
                o.OnNext(8);
                o.OnCompleted();

                return Disposable.Create(() => { });
            });

            var results = new List<int>();
            var exception = false;
            var completed = false;

            var observable = created.Timeout(TimeSpan.FromTicks(100),
                                             scheduler);

            //observable.Subscribe(results.Add,
            //                     e => { exception = true; },
            //                     () => { completed = true; });

            scheduler.Start();

            Check.That(exception)
                 .IsTrue();

            Check.That(completed)
                 .IsFalse();

            Check.That(results)
                 .ContainsExactly(new[]
                                  {
                                      3, 6
                                  });
        }
    }
}