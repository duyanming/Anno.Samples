using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anno.Plugs.Proxy.Common
{
    /// <summary>
    /// 集合扩展
    /// </summary>
    public static partial class EnumerableExtentions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="process"></param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> process)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (process == null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            foreach (T item in source)
            {
                process(item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="source"></param>
        /// <param name="process"></param>
        public static void ForEach<T, R>(this IEnumerable<T> source, Func<T, R> process)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (process == null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            foreach (T item in source)
            {
                process(item);
            }
        }

    }
}
