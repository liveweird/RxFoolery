using System;

namespace RxFoolery
{
    public static class ObservableExtentions
    {
        public static IObservable<T> Dump<T>(this IObservable<T> source,
                                             string name)
        {
            source.Subscribe(evnt => Console.WriteLine("{0}: event - {1}",
                                                       name,
                                                       evnt),
                             ex => Console.WriteLine("{0}: error - {1}",
                                                     name,
                                                     ex.Message),
                             () => Console.WriteLine("{0}: completed",
                                                     name));

            return source;
        }
    }
}