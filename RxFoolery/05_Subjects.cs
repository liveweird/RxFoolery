using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;

namespace RxFoolery
{
    [TestClass]
    public class Subjects
    {
        [TestMethod]
        public void Subject()
        {
            var results = new List<long>();
            var completed = false;
            var subject = new Subject<long>();

            subject.Subscribe(results.Add,
                              (ex) => Assert.Fail("No exception is planned! {0}",
                                                  ex),
                              () => { completed = true; });

            subject.OnNext(10);
            subject.OnNext(-235);
            subject.OnNext((long)0.123);
            subject.OnCompleted();

            Check.That(completed)
                 .IsTrue();

            Check.That(results)
                 .ContainsExactly(new[]
                                  {
                                      10, -235, (long) 0.123
                                  });
        }

        [TestMethod]
        public void ReplaySubject()
        {
            var subjectResult = new List<long>();
            var replayResult = new List<long>();
            var subject = new Subject<long>();
            var replay = new ReplaySubject<long>();

            var subjects = new List<ISubject<long>>
                           {
                               subject,
                               replay
                           };

            subjects.ForEach(p =>
            {
                p.OnNext(10);
                p.OnNext(-235);
            });

            subject.Subscribe(subjectResult.Add,
                  (ex) => Assert.Fail("No exception is planned! {0}",
                                      ex),
                  () => { });

            replay.Subscribe(replayResult.Add,
                  (ex) => Assert.Fail("No exception is planned! {0}",
                                      ex),
                  () => { });

            subjects.ForEach(p =>
            {
                p.OnNext((long)0.123);
                p.OnCompleted();
            });

            Check.That(subjectResult)
                 .ContainsExactly(new[]
                                  {
                                      (long) 0.123
                                  });

            Check.That(replayResult)
                 .ContainsExactly(new[]
                                  {
                                      10, -235, (long) 0.123
                                  });
        }

        [TestMethod]
        public void BehaviorSubjectNonEmpty()
        {
            var subjectResult = new List<long>();
            var behaviorResult = new List<long>();
            var subject = new Subject<long>();
            var behavior = new BehaviorSubject<long>(-666);

            var subjects = new List<ISubject<long>>
                           {
                               subject,
                               behavior
                           };

            subjects.ForEach(p =>
            {
                p.OnNext(10);
                p.OnNext(-235);
            });

            subject.Subscribe(subjectResult.Add,
                  (ex) => Assert.Fail("No exception is planned! {0}",
                                      ex),
                  () => { });

            behavior.Subscribe(behaviorResult.Add,
                  (ex) => Assert.Fail("No exception is planned! {0}",
                                      ex),
                  () => { });

            subjects.ForEach(p =>
            {
                p.OnNext((long)0.123);
                p.OnCompleted();
            });

            Check.That(subjectResult)
                 .ContainsExactly(new[]
                                  {
                                      (long) 0.123
                                  });

            Check.That(behaviorResult)
                 .ContainsExactly(new[]
                                  {
                                      -235, (long) 0.123
                                  });
        }

        [TestMethod]
        public void BehaviorSubjectEmpty()
        {
            var subjectResult = new List<long>();
            var behaviorResult = new List<long>();
            var subject = new Subject<long>();
            var behavior = new BehaviorSubject<long>(-666);

            var subjects = new List<ISubject<long>>
                           {
                               subject,
                               behavior
                           };

            subject.Subscribe(subjectResult.Add,
                  (ex) => Assert.Fail("No exception is planned! {0}",
                                      ex),
                  () => { });

            behavior.Subscribe(behaviorResult.Add,
                  (ex) => Assert.Fail("No exception is planned! {0}",
                                      ex),
                  () => { });

            subjects.ForEach(p =>
            {
                p.OnNext((long)0.123);
                p.OnCompleted();
            });

            Check.That(subjectResult)
                 .ContainsExactly(new[]
                                  {
                                      (long) 0.123
                                  });

            Check.That(behaviorResult)
                 .ContainsExactly(new[]
                                  {
                                      -666, (long) 0.123
                                  });
        }

        [TestMethod]
        public void AsyncSubject()
        {
            var subjectResult = new List<long>();
            var asyncResult = new List<long>();
            var subject = new Subject<long>();
            var async = new AsyncSubject<long>();

            var subjects = new List<ISubject<long>>
                           {
                               subject,
                               async
                           };

            subjects.ForEach(p =>
            {
                p.OnNext(10);
                p.OnNext(-235);
            });

            subject.Subscribe(subjectResult.Add,
                  (ex) => Assert.Fail("No exception is planned! {0}",
                                      ex),
                  () => { });

            async.Subscribe(asyncResult.Add,
                  (ex) => Assert.Fail("No exception is planned! {0}",
                                      ex),
                  () => { });

            subjects.ForEach(p =>
            {
                p.OnNext((long)0.123);
                p.OnNext((long)34.67);
                p.OnCompleted();
            });

            Check.That(subjectResult)
                 .ContainsExactly(new[]
                                  {
                                      (long) 0.123,
                                      (long) 34.67
                                  });

            Check.That(asyncResult)
                 .ContainsExactly(new[]
                                  {
                                      (long) 34.67
                                  });
        }         
    }
}