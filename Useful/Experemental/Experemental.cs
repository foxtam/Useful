using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Useful.Collections;

namespace Useful.Experemental
{
	public static class Experemental
	{
		/// <summary>
		/// Perform a deep Copy of the object.
		/// </summary>
		/// <typeparam name="T">The type of object being copied.</typeparam>
		/// <param name="source">The object instance to copy.</param>
		/// <returns>The copied object.</returns>
		static T CloneSerialize<T>(this T source)
		{
			if (!typeof(T).IsSerializable)
				throw new ArgumentException("The type must be serializable.", "source");

			// Don't serialize a null object, simply return the default for that object
			if (Object.ReferenceEquals(source, null))
				return default(T);

			IFormatter formatter = new BinaryFormatter();
			Stream stream = new MemoryStream();
			using (stream)
			{
				formatter.Serialize(stream, source);
				stream.Seek(0, SeekOrigin.Begin);
				return (T)formatter.Deserialize(stream);
			}
		}

		public static (T, T) Tuple2<T>(this IEnumerable<T> sourse)
			=> (sourse.First(), sourse.ElementAt(1));

		public static (T, T, T) Tuple3<T>(this IEnumerable<T> sourse)
			=> (sourse.First(), sourse.ElementAt(1), sourse.ElementAt(2));

		public static (T, T, T, T) Tuple4<T>(this IEnumerable<T> sourse)
			=> (sourse.First(), sourse.ElementAt(1), sourse.ElementAt(2), sourse.ElementAt(3));

		public static (T, T, T, T, T) Tuple5<T>(this IEnumerable<T> sourse)
			=> (sourse.First(), sourse.ElementAt(1), sourse.ElementAt(2), sourse.ElementAt(3), sourse.ElementAt(4));

		public static IEnumerable<T> Iter<T>(this (T, T) sourse)
		{
			yield return sourse.Item1;
			yield return sourse.Item2;
		}

		public static Dictionary<TK, TV> ToDictionary<TK, TV>(this IEnumerable<(TK, TV)> sourse)
		{
			var dict = new Dictionary<TK, TV>();
			foreach (var (key, value) in sourse)
				dict[key] = value;
			return dict;
		}

		public static MultiSet<T> ToMultiSet<T>(this IEnumerable<T> source) => new MultiSet<T>(source);

		public static MultiSet<TKey> ToMultiSet<TKey>(this IDictionary<TKey, int> source) => new MultiSet<TKey>(source);
	}
}