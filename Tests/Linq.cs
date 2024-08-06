﻿using Python.Runtime;
using System.Collections;
using System.Reflection;

namespace Dynamitey.Tests
{
	[TestFixture]
	public class Linq
	{
		[Test]
		public void SimpleLinqDynamicLinq()
		{
			var expected = Enumerable.Range(1, 10).Where(i => i > 5).Skip(1).Take(2).Max();
			var actual = Dynamic.Linq(Enumerable.Range(1, 10)).Where(new Func<int, bool>(i => i > 5)).Skip(1).Take(2).Max();

			Assert.That(expected, Is.EqualTo(actual));
		}

		[Test]
		public void MoreGenericsDynamicLinq()
		{
			var expected = Enumerable.Range(1, 10).Select(i => Tuple.Create(1, i)).Aggregate(0, (accum, each) => each.Item2);
			var actual = Dynamic.Linq(Enumerable.Range(1, 10))
				.Select(new Func<int, Tuple<int, int>>(i => Tuple.Create(1, i)))
				.Aggregate(0, new Func<int, Tuple<int, int>, int>((accum, each) => each.Item2));

			Assert.That(expected, Is.EqualTo(actual));
		}

		private dynamic RunPythonHelper(object linq, string code)
		{
			using (Py.GIL())
			{
				// Initialize a new Python dictionary to serve as the scope
				using (PyDict pyGlobals = new PyDict())
				{
					// Add the 'linq' variable to the Python scope
					pyGlobals.SetItem("linq", linq.ToPython());

					// Execute the provided code within the Python scope
					PythonEngine.Exec(code.Trim(), pyGlobals);

					// Retrieve and return the 'result' variable from the Python scope
					return pyGlobals.GetItem("result");
				}
			}
		}

		// Need to revisit these tests.  Commenting out for now.

		//        [Test]
		//        public void PythonDynamicLinqGenericArgs()
		//        {
		//            var start = new Object[] { 1, "string", 4, Guid.Empty, 6 };
		//            var expected = start.OfType<int>().Skip(1).First();
		//            var actual = RunPythonHelper(Dynamic.Linq(start), @"
		//import System
		//result = linq.OfType[System.Int32]().Skip(1).First()

		//");
		//            Assert.That(expected, actual);
		//        }

		//        [Test]
		//        public void PythonDynamicLinq()
		//        {
		//            var expected = Enumerable.Range(1, 10).Where(x => x < 5).OrderBy(x => 10 - x).First();

		//            var actual = RunPythonHelper(Dynamic.Linq(Enumerable.Range(1, 10)),
		//                                         @"
		//import System
		//result = linq.Where.Overloads[System.Func[int, bool]](lambda x: x < 5).OrderBy(lambda x: 10-x).First()

		//");

		//            Assert.That(expected, actual);
		//        }

		[Test]
		public void PrintOutInterface()
		{
			var tList =
				typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).OrderBy(it => it.Name).
					ToList();

			Console.WriteLine("public interface ILinq<TSource>:IEnumerable<TSource>");
			Console.WriteLine("{");
			foreach (var line in tList
			.Where(it => it.GetParameters().Any()
			&& (HelperIsGenericExtension(it, typeof(IEnumerable<>))
				|| it.GetParameters().First().ParameterType == typeof(IEnumerable))
			)
			.Select(HelperMakeName))
			{
				Console.WriteLine("\t" + line);
			}
			Console.WriteLine("}");
			Console.WriteLine();

			Console.WriteLine("public interface IOrderedLinq<TSource> : ILinq<TSource>, IOrderedEnumerable<TSource>");
			Console.WriteLine("{");
			foreach (var line in tList
			.Where(it => it.GetParameters().Any()
			&& HelperIsGenericExtension(it, typeof(IOrderedEnumerable<>))
			)
			.Select(HelperMakeName))
			{
				Console.WriteLine("\t" + line);
			}
			Console.WriteLine("}");
			Console.WriteLine();

			Console.WriteLine("//Skipped Methods");
			foreach (var line in tList
			.Where(it => it.GetParameters().Any()
			&& !(HelperIsGenericExtension(it, typeof(IEnumerable<>)))
			&& !(HelperIsGenericExtension(it, typeof(IOrderedEnumerable<>)))
			&& !(it.GetParameters().First().ParameterType == typeof(IEnumerable)))
			.Select(HelperMakeNameDebug))
			{
				Console.WriteLine("//" + line);
			}
		}

		private bool HelperIsGenericExtension(MethodInfo it, Type genericType)
		{
			return it.GetParameters().First().ParameterType.IsGenericType
				   && it.GetParameters().First().ParameterType.GetGenericTypeDefinition() == genericType
				   && HelperSingleGenericArgMatch(it.GetParameters().First().ParameterType.GetGenericArguments().Single());
		}

		private bool HelperSingleGenericArgMatch(Type info)
		{
			foreach (var name in new[] { "TSource", "TFirst", "TOuter" })
			{
				if (info.Name == name)
				{
					return true;
				}
			}

			return false;
		}

		// Define other methods and classes here
		private string HelperFormatType(Type it)
		{
			if (HelperSingleGenericArgMatch(it))
			{
				return "TSource";
			}

			if (it.IsGenericType)
			{
				return String.Format("{0}<{1}>", it.Name.Substring(0, it.Name.IndexOf("`")), String.Join(",", it.GetGenericArguments().Select(a => HelperFormatType(a))));
			}
			else
			{
				return it.Name;
			}
		}

		private string HelperGenericParams(Type[] it)
		{
			var tArgs = it.Where(t => !HelperSingleGenericArgMatch(t)).Select(t => HelperFormatType(t));
			if (!tArgs.Any())
			{
				return "";
			}
			return "<" + String.Join(",", tArgs) + ">";
		}

		private string HelperReturnTypeSub(Type it)
		{
			if (it.IsGenericType && (it.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
			{
				return String.Format("ILinq<{0}>", HelperFormatType(it.GetGenericArguments().Single()));
			}
			if (it.IsGenericType && (it.GetGenericTypeDefinition() == typeof(IOrderedEnumerable<>)))
			{
				return String.Format("IOrderedLinq<{0}>", HelperFormatType(it.GetGenericArguments().Single()));
			}
			return HelperFormatType(it);
		}

		private string HelperGetParams(ParameterInfo[] it)
		{
			var parms = it.Skip(1);
			return String.Join(",", parms.Select(p => HelperFormatType(p.ParameterType) + " " + p.Name));
		}

		private string HelperGetParamsDebug(ParameterInfo[] it)
		{
			var parms = it;
			return String.Join(",", parms.Select(p => HelperFormatType(p.ParameterType) + " " + p.Name));
		}

		private string HelperMakeName(MethodInfo it)
		{
			return String.Format("{0} {1}{2}({3});", HelperReturnTypeSub(it.ReturnType), it.Name, HelperGenericParams(it.GetGenericArguments()), HelperGetParams(it.GetParameters()));
		}

		private string HelperMakeNameDebug(MethodInfo it)
		{
			return String.Format("{0} {1}{2}({3});", HelperReturnTypeSub(it.ReturnType), it.Name, HelperGenericParams(it.GetGenericArguments()), HelperGetParamsDebug(it.GetParameters()));
		}
	}
}
