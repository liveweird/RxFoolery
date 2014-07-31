using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;

namespace RxFoolery
{
    [TestClass]
    public class LetsGetCreative
    {
        [TestMethod]
        public void Buffering()
        {
            var scheduler = new TestScheduler();
            var result = new List<IEnumerable<long>>();
            var completed = false;

            var observable = Observable.Interval(TimeSpan.FromSeconds(1),
                                                 scheduler)
                                       .Delay(TimeSpan.FromMilliseconds(100),
                                              scheduler);
            var buffered = observable.Buffer(TimeSpan.FromSeconds(3),
                                             scheduler);
            buffered.Take(3)
                    .Subscribe(result.Add,
                               ex => Assert.Fail("No exception was expected! {0}",
                                                 ex),
                               () => { completed = true; });

            scheduler.Start();

            Check.That(completed)
                 .IsTrue();

            Check.That(result.Count)
                 .IsEqualTo(3);

            Check.That(result[0]).IsOnlyMadeOf(
                new long[] { 0, 1 }
            );

            Check.That(result[1]).IsOnlyMadeOf(
                new long[] { 2, 3, 4 }
            );

            Check.That(result[2]).IsOnlyMadeOf(
                new long[] { 5, 6, 7 }
            );
        }

        [TestMethod]
        public void AverageWithinWindow()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void BufferingWithPresetBatchSize()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void BufferingWithPreferredBatchSize()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Sample()
        {
            Assert.Fail();
        }
    }
}