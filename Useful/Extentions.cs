using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Useful.Collections;

namespace Useful
{
	public static class Use
	{
		public static void Swap<T>(ref T lhs, ref T rhs)
		{
			T tmp = lhs;
			lhs = rhs;
			rhs = tmp;
		}

		public static IEnumerable<T> Generate<T>(Func<T> factory, int count)
		{
			for (int i = 0; i < count; i++)
				yield return factory();
		}

		public static TimeSpan Bench(Action func, int runs = 1)
		{
			Stopwatch sw = Stopwatch.StartNew();
			for (int i = 0; i < runs; i++)
			{
				func();
			}
			sw.Stop();
			return sw.Elapsed;
		}

		public static IEnumerable<T[]> Permutations<T>(IEnumerable<T> source)
		{
			T[] array = source.ToArray();
			int last = array.Length - 1;
			return Permutations(array, 0, last);
		}

		static IEnumerable<T[]> Permutations<T>(T[] source, int first, int last)
		{
			if (first == last)
			{
				var dest = new T[source.Length];
				source.CopyTo(dest, 0);
				yield return dest;
			}
			else
				for (int i = first; i <= last; i++)
				{
					Use.Swap(ref source[first], ref source[i]);
					foreach (T[] mutation in Permutations(source, first + 1, last))
						yield return mutation;
					Use.Swap(ref source[first], ref source[i]);
				}
		}
	}

	public static class UsefulExtentions
	{
		public static int IndexOf<T>(this IEnumerable<T> source, IEnumerable<T> items)
		{
			T[] sourceArray = source as T[] ?? source.ToArray();
			T[] itemsArray = items as T[] ?? items.ToArray();
			int sourceSize = sourceArray.Length;
			int itemsSize = itemsArray.Length;
			for (int i = 0; i <= sourceSize - itemsSize; ++i)
				if (sourceArray.Skip(i).Take(itemsSize).SequenceEqual(itemsArray))
					return i;

			return -1;
		}

		public static string StringLine<T>(this IEnumerable<T> source, string sep = ", ", string begin = "{",
			string end = "}")
		{
			return begin + string.Join(sep, source) + end;
		}

		public static bool IsClamped<T>(this T value, T min, T max) where T : IComparable<T>
		{
			return value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;
		}

		public static IEnumerable<int> Iota(this int begin, int end, int step = 1)
		{
			for (int number = begin; number < end; number += step)
				yield return number;
		}

		public static IEnumerable<T> Step<T>(this IEnumerable<T> source, int step)
		{
			using (IEnumerator<T> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					yield return enumerator.Current;
					for (int i = 0; i < step - 1; ++i)
						if (!enumerator.MoveNext())
							yield break;
				}
			}
		}

		public static IEnumerable<T> Sub<T>(this IEnumerable<T> source, int begin, int count)
			=> source.Skip(begin).Take(count);

		public static bool In<T>(this T source, params T[] list)
			=> list.Contains(source);

		public static string[] SplitClear(this string source, params char[] separators)
		{
			if (separators.Length == 0)
				separators = new[] {' '};
			return source.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		}

		public static IOrderedEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
		{
			var rnd = new Random();
			return source.OrderBy(_ => rnd.Next());
		}

		public static void ShuffleInPlace<T>(this IList<T> list)
		{
			Random rnd = new Random();
			for (int i = 0; i < list.Count; i++)
			{
				int j = rnd.Next(list.Count);
				T tmp = list[i];
				list[i] = list[j];
				list[j] = tmp;
			}
		}

		public static IEnumerable<T> Repeat<T>(this T item, int count) => Enumerable.Repeat(item, count);

		public static void Print<T>(this IEnumerable<T> source, string sep = ", ", string end = "\n")
		{
			Console.Write(source.StringLine(sep));
			Console.Write(end);
		}

		public static bool IsClose(this double x, double y, double relTol = 1e-09, double absTol = 1e-12)
		{
			double epsilon = Math.Max(Math.Abs(x), Math.Abs(y)) * relTol;
			return Math.Abs(x - y) <= Math.Max(epsilon, absTol);
		}

		public static Dictionary<TKey, TValue> ToDict<TKey, TValue>(this string source)
		{
			var dict = new Dictionary<TKey, TValue>();
			var regex = new Regex(@"(?'key'.+):(?'value'.+)");
			source = source.Replace('.', ',');
			string[] pairs = source.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string wordPair in pairs)
			{
				Match pair = regex.Match(wordPair);
				dict[pair.Groups["key"].Value.To<TKey>()] = pair.Groups["value"].Value.To<TValue>();
			}
			return dict;
		}
	}
}
