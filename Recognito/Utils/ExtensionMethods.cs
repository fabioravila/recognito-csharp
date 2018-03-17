using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recognito
{
    public static class ExtensionMethods
    {
        public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> source)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            foreach (var element in source)
            {
                target.Add(element);
            }
        }

        public static void Add<T>(this ICollection<T> target, T item)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.Add(item);
        }

        public static void AddRange<T, V>(this ConcurrentDictionary<T, V> target, IDictionary<T, V> source)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            foreach (var element in source)
            {
                target.AddOrUpdate(element.Key, element.Value, (k, old) => element.Value);
            }
        }

        public static byte[] ToArray(this Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(ms);

                return ms.ToArray();
            }
        }

        public static long SeekBegin(this Stream stream)
        {
            if (stream == null)
                return -1;

            return stream.Seek(0, SeekOrigin.Begin);
        }
    }
}
