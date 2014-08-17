using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;

namespace RxFoolery
{
    [TestClass]
    public class BeyondBasics
    {
        [TestMethod]
        public void ToEnumerable()
        {
            var seq = Observable.Range(0,
                                       3);

            var result = seq.ToEnumerable();

            Check.That(result)
                 .ContainsExactly(0, 1, 2);
        }

        [TestMethod]
        public void Catch()
        {
            var source = Observable.Create<long>(o =>
                                                 {
                                                     o.OnNext(1L);
                                                     o.OnNext(2L);
                                                     o.OnError(new Exception("ABC"));
                                                     o.OnNext(3L);
                                                     o.OnCompleted();

                                                     return Disposable.Empty;
                                                 });

            var backup = Observable.Create<long>(o =>
                                                 {
                                                     o.OnNext(7L);
                                                     o.OnNext(8L);
                                                     o.OnCompleted();

                                                     return Disposable.Empty;
                                                 });

            var exception = source.Catch(backup);

            var result = new List<long>();
            exception.Subscribe(result.Add);

            Check.That(result)
                 .ContainsExactly(1L,
                                  2,
                                  7,
                                  8);
        }

        [TestMethod]
        public void Retry()
        {
            var loop = 0;
            var error = false;
            var completed = false;
            var result = new List<long>();

            var seq = Observable.Create<long>(o =>
                                              {
                                                  o.OnNext(1);

                                                  if ((loop++) < 2)
                                                  {
                                                      o.OnError(new Exception());
                                                  }
                                                  else
                                                  {
                                                      o.OnCompleted();   
                                                  }

                                                  return Disposable.Empty;
                                              })
                                .Retry(5);

            seq.Subscribe(result.Add,
                          e => { error = true; },
                          () => { completed = true; });

            Check.That(error)
                 .IsFalse();

            Check.That(completed)
                 .IsTrue();

            Check.That(result)
                 .ContainsExactly(1L,
                                  1,
                                  1);
        }
    }
}
