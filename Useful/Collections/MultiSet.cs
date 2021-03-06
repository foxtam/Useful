﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Useful.Collections
{
    public class MultiSet<T>
    {
	    private readonly DefaultDict<T, int> _multiSet;

	    public MultiSet()
	    {
		    _multiSet = new DefaultDict<T, int>();
	    }

	    public MultiSet(IEnumerable<T> source) : this()
	    {
		    foreach (T item in source)
			    ++_multiSet[item];
	    }

	    public MultiSet(IDictionary<T, int> dict)
	    {
		    _multiSet = new DefaultDict<T, int>(dict, () => 0);
	    }

	    public IEnumerable<T> Elements() => _multiSet.SelectMany(p => p.Key.Repeat(p.Value));

	    public IEnumerable<KeyValuePair<T, int>> MostCommon(int n) => _multiSet.OrderByDescending(p => p.Value).Take(n);

	    public static MultiSet<T> operator +(MultiSet<T> lhs, MultiSet<T> rhs)
	    {
		    SwapIfLeftLess(ref lhs, ref rhs);
			var result = new MultiSet<T>(lhs._multiSet);
		    foreach (KeyValuePair<T, int> pair in rhs._multiSet)
			    result._multiSet[pair.Key] += pair.Value;
		    return result;
	    }

	    public static MultiSet<T> operator -(MultiSet<T> lhs, MultiSet<T> rhs)
	    {
		    SwapIfLeftLess(ref lhs, ref rhs);
			var result = new MultiSet<T>(lhs._multiSet);
		    foreach (KeyValuePair<T, int> pair in rhs._multiSet)
			    if (result._multiSet.TryGetValue(pair.Key, out int value))
			    {
				    if (value > pair.Value)
					    result._multiSet[pair.Key] = value - pair.Value;
				    else
					    result._multiSet.Remove(pair.Key);
			    }
		    return result;
	    }

	    public static MultiSet<T> operator &(MultiSet<T> lhs, MultiSet<T> rhs)
	    {
		    SwapIfLeftLess(ref lhs, ref rhs);
		    var result = new MultiSet<T>();
		    foreach (KeyValuePair<T, int> pair in rhs._multiSet)
			    if (lhs._multiSet.TryGetValue(pair.Key, out int value))
				    result._multiSet[pair.Key] = Math.Min(value, pair.Value);
		    return result;
	    }

	    public static MultiSet<T> operator |(MultiSet<T> lhs, MultiSet<T> rhs)
	    {
		    SwapIfLeftLess(ref lhs, ref rhs);
		    var result = new MultiSet<T>(lhs._multiSet);
		    foreach (KeyValuePair<T, int> pair in rhs._multiSet)
			    if (!lhs._multiSet.TryGetValue(pair.Key, out int value) || value < pair.Value)
				    result._multiSet[pair.Key] = pair.Value;
		    return result;
	    }

		private static void SwapIfLeftLess(ref MultiSet<T> lhs, ref MultiSet<T> rhs)
	    {
		    if (lhs._multiSet.Count < rhs._multiSet.Count)
			    Use.Swap(ref lhs, ref rhs);
	    }
	}
}
