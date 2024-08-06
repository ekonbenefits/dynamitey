//
//  Copyright 2013 Ekon Benefits
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

using Dynamitey.Internal.Optimization;
using System.Collections;
using System.Reflection;

namespace Dynamitey
{
	/// <summary>
	/// Dynamically Dealing with Tuples
	/// </summary>

	public static class Tupler
	{
		private class TuplerFix
		{
			private Tuple<T1, T2, T3, T4, T5, T6, T7, T8> Create<T1, T2, T3, T4, T5, T6, T7, T8>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8)
			{
				return new Tuple<T1, T2, T3, T4, T5, T6, T7, T8>(item1, item2, item3, item4, item5, item6, item7, item8);
			}
		}

		private static TuplerFix TuplerHelper = new TuplerFix();
		private static InvokeContext StaticTuple = InvokeContext.CreateStatic(typeof(Tuple));

		/// <summary>
		/// Creates a Tuple with arg runtime types.
		/// </summary>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		public static dynamic Create(params object[] args)
		{
			return args.ToTuple();
		}

		/// <summary>
		/// Enumerable to tuple.
		/// </summary>
		/// <param name="enumerable">The enumerable.</param>
		/// <returns></returns>
		public static dynamic ToTuple(this IEnumerable enumerable)
		{
			var items = enumerable as IEnumerable<object> ?? enumerable.Cast<object>();
			if (items.Count() < 8)
			{
				return Dynamic.InvokeMember(StaticTuple, "Create", items.ToArray());
			}

			return Dynamic.InvokeMember(TuplerHelper, "Create",
										items.Take(7).Concat(new object[] { items.Skip(7).ToTuple() }).ToArray());
		}

		/// <summary>
		/// Firsts item of the specified tuple.
		/// </summary>
		/// <param name="tuple">The tuple.</param>
		/// <returns></returns>
		public static dynamic First(object tuple)
		{
			return Index(tuple, 0);
		}

		/// <summary>
		/// Second item of the specified tuple.
		/// </summary>
		/// <param name="tuple">The tuple.</param>
		/// <returns></returns>
		public static dynamic Second(object tuple)
		{
			return Index(tuple, 1);
		}

		/// <summary>
		/// Lasts item of the specified tuple.
		/// </summary>
		/// <param name="tuple">The tuple.</param>
		/// <returns></returns>
		public static dynamic Last(object tuple)
		{
			return Index(tuple, Size(tuple)-1);
		}

		/// <summary>
		/// Convert to list.
		/// </summary>
		/// <param name="tuple">The tuple.</param>
		/// <returns></returns>
		public static IList<dynamic> ToList(object tuple)
		{
			var list = new List<dynamic>();
			HelperToList(list, tuple, safe: false);
			return list;
		}

		private static void HelperToList(List<dynamic> list, object tuple, bool safe)
		{
			if (HelperIsTuple(tuple, out var type, out var generic, out var size, safe))
			{
				for (int i = 0; i < 7 && i < size; i++)
				{
					list.Add(HelperIndex(tuple, i, safe: true));
				}

				if (size == 8)
				{
					HelperToList(list, (object)(((dynamic)tuple).Rest), true);
				}
			}
		}

		/// <summary>
		/// Gets value at the index of the specified tuple.
		/// </summary>
		/// <param name="tuple">The tuple.</param>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentException">index must be greater than or equalto 0;index</exception>
		public static dynamic Index(object tuple, int index)
		{
			return HelperIndex(tuple, index, false);
		}

		private static dynamic HelperIndex(object tuple, int index, bool safe)
		{
			var item = index + 1;
			if (!safe && item < 1)
			{
				throw new ArgumentException("index must be greater than or equalto 0", nameof(index));
			}

			if (!safe && item > Size(tuple))
			{
				throw new ArgumentException("index must be less than size", nameof(index));
			}

			if (!safe && !IsTuple(tuple))
			{
				return tuple;
			}

			if (item < 8)
				return InvokeHelper.TupleItem(tuple, item);

			object newtarget = ((dynamic)tuple).Rest;
			return HelperIndex(newtarget, item - 8, true);
		}

		/// <summary>
		/// Determines whether the specified target is a tuple.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <returns>
		///   <c>true</c> if the specified target is tuple; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsTuple(object target)
		{
			return HelperIsTuple(target, out var type, out var genericType, out var size, false);
		}

		private static bool HelperIsTuple(object target, out Type type, out Type genericeType, out int size, bool safe)
		{
			genericeType = typeof(object);
			size = 1;
			type = null;
			if (target == null)
				return false;
			type = target as Type ?? target.GetType();

			if (safe || type.GetTypeInfo().IsGenericType)
			{
				genericeType = type.GetGenericTypeDefinition();
			}

			return InvokeHelper.TupleArgs.TryGetValue(genericeType, out size);
		}

		/// <summary>
		/// Gets the size of the tuple
		/// </summary>
		/// <param name="tuple">The tuple.</param>
		/// <returns></returns>
		public static int Size(object tuple)
		{
			return HelperSize(tuple, false);
		}

		private static int HelperSize(object tuple, bool safe)
		{
			if (HelperIsTuple(tuple, out var type, out var genericType, out var size, safe))
			{
				if (size == 8)
				{
					var lasttype = type.GetTypeInfo().GetGenericArguments()[7];
					size = size + HelperSize(lasttype, true) - 1;
				}
			}
			return size;
		}
	}
}
