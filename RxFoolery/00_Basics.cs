using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;

namespace RxFoolery
{
    public class TestObservable : IObservable<long>
    {
        private readonly List<IObserver<long>> _observers = new List<IObserver<long>>(); 

        public void DoSomething()
        {
            _observers.ForEach(p =>
                               {
                                   p.OnNext(123);
                                   p.OnError(new Exception("Shit happens"));
                               });
        }

        public void DoSomethingElse()
        {
            _observers.ForEach(p =>
                               {
                                   p.OnNext((long) -7.5);
                                   p.OnCompleted();
                               });
        }

        public IDisposable Subscribe(IObserver<long> observer)
        {
            _observers.Add(observer);

            return new TestSubscription(this,
                                        observer);
        }

        public void StopObserving(IObserver<long> observer)
        {
            if (_observers.Contains(observer))
            {
                _observers.Remove(observer);
            }
        }
    }

    public class TestObserver : IObserver<long>
    {
        public TestObserver()
        {
            ActionLog = new List<long>();
        }

        public List<long> ActionLog { get; private set; }

        public void OnNext(long value)
        {
            ActionLog.Add(value);
        }

        public void OnError(Exception error)
        {
            ActionLog.Add(long.MaxValue);
        }

        public void OnCompleted()
        {
            ActionLog.Add(long.MinValue);
        }
    }

    public class TestSubscription : IDisposable
    {
        private readonly TestObservable _observable;
        private readonly IObserver<long> _observer;

        public TestSubscription(TestObservable observable,
                                IObserver<long> observer)
        {
            _observable = observable;
            _observer = observer;
        }

        public void Dispose()
        {
            if (_observable != null &&
                _observer != null)
            {
                _observable.StopObserving(_observer);
            }
        }
    }

    [TestClass]
    public class Basics
    {
        // Observable
        [TestMethod]
        public void ObservableTest()
        {
            var observable = new TestObservable();
            var observer1 = new TestObserver();
            var observer2 = new TestObserver();
            var observer3 = new TestObserver();

            using (observable.Subscribe(observer1))
            {
                using (observable.Subscribe(observer2))
                {
                    observable.DoSomething();
                }

                using (observable.Subscribe(observer3))
                {
                    observable.DoSomethingElse();
                }
            }

            Check.That(observer1.ActionLog)
                 .ContainsExactly(123,
                                  long.MaxValue,
                                  (long) -7.5,
                                  long.MinValue);

            Check.That(observer2.ActionLog)
                 .ContainsExactly(123,
                                  long.MaxValue);

            Check.That(observer3.ActionLog)
                 .ContainsExactly((long)-7.5,
                                  long.MinValue);
        }

        [TestMethod]
        public void IsItMultithreadedByDesign()
        {
            var id = Thread.CurrentThread.ManagedThreadId;

            var threads = new List<int>();

            var scheduler = new TestScheduler();
            var observable = Observable.Interval(TimeSpan.FromSeconds(1),
                                                 scheduler)
                                       .Take(3);

            observable.Subscribe(p => threads.Add(Thread.CurrentThread.ManagedThreadId));

            scheduler.Start();

            Check.That(threads)
                 .IsOnlyMadeOf(id);
        }

        [TestMethod]
        public void ObserveOn()
        {
            var id = Thread.CurrentThread.ManagedThreadId;

            var threads = new List<int>();

            var scheduler = new TestScheduler();
            var observerScheduler = Scheduler.NewThread;

            var observable = Observable.Interval(TimeSpan.FromSeconds(1),
                                                 scheduler)
                                       .Take(3);

            observable.ObserveOn(observerScheduler)
                      .Subscribe(p => threads.Add(Thread.CurrentThread.ManagedThreadId));

            scheduler.Start();

            Check.That(threads)
                 .Not
                 .Contains(id);
        }
    }
}
