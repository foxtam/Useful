using System;
using System.Collections.Generic;

namespace Useful.Collections
{
	public class DefaultDict<TKey, TValue> : Dictionary<TKey, TValue>
	{
		private readonly Func<TValue> _factory;

		public DefaultDict() : this(() => default)
		{
		}

		public DefaultDict(Func<TValue> factory) : this(new Dictionary<TKey, TValue>(), factory)
		{
		}

		public DefaultDict(IDictionary<TKey, TValue> dictionary) : this(dictionary, () => default)
		{
		}

		public DefaultDict(IDictionary<TKey, TValue> dictionary, Func<TValue> factory) : base(dictionary)
		{
			_factory = factory;
		}

		public new TValue this[TKey key]
		{
			get => TryGetValue(key, out TValue value) ? value : _factory();
			set => base[key] = value;
		}
	}
}