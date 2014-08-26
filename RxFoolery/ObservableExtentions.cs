using System;
using System.Reactive.Linq;

namespace RxFoolery
{
    public static class ObservableExtentions
    {
        public static IObservable<T> Dump<T>(this IObservable<T> source,
                                             string name,
                                             Func<T, string> dumper)
        {
            return source.Do(evnt => Console.WriteLine("{0}: event - {1}",
                                                       name,
                                                       dumper(evnt)),
                             ex => Console.WriteLine("{0}: error - {1}",
                                                     name,
                                                     ex.Message),
                             () => Console.WriteLine("{0}: completed",
                                                     name));
        }

        public static IObservable<T> Dump<T>(this IObservable<T> source,
                                             string name)
        {
            return Dump(source,
                        name,
                        a => a.ToString());
        }
    }
}