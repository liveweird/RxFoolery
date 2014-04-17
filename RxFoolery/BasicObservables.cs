using System;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            never.Timeout(TimeSpan.FromSeconds(30))
                 .Subscribe(i => Assert.Fail("No event's supposed to happen!"),
                            exception => Assert.Fail("No exception is planned!"),
                            () => Assert.Fail("Sequence will never complete!"));

            scheduler.Start();
        }

        [TestMethod]
        public void Empty()
        {
            var finished = false;
            var scheduler = new TestScheduler();
            var empty = Observable.Empty<int>();

            empty.Timeout(TimeSpan.FromSeconds(30))
                 .Subscribe(i => Assert.Fail("No event's supposed to happen!"),
                            exception => Assert.Fail("No exception is planned!"),
                            () => { finished = true; });

            scheduler.Start();

            Assert.IsTrue(finished);
        }
    }
}
