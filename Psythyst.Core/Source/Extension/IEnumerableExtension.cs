using System.Collections.Generic;
using System;

namespace T2rkus.Spark.Core
{
    /// <summary>
    /// IEnumerableExtension Class.
    /// </summary>
    public static class IEnumerableExtension
    {
        public static void Each<T>(this IEnumerable<T> Collection, Action<T> Action)
        {
            foreach (var Item in Collection) Action.Invoke(Item);
        }

        public static void TryEach<T>(this IEnumerable<T> Collection, Action<T> Action, Action<T, Exception> OnError)
        {
            foreach (var Item in Collection){
                try {
                    Action.Invoke(Item);
                }
                catch (Exception Ex){
                    OnError?.Invoke(Item, Ex);
                }
            }
        }
    }
}