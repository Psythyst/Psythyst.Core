using System.Collections.Generic;
using System;

namespace Psythyst.Core
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
    }
}