using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ShyAlex.Compare
{
	public class StructuralComparison
	{
		private readonly Stack<Object> expected;

		private readonly Stack<Object> actual;

		public StructuralComparison(Object expected, Object actual)
		{
			var expectedStack = new Stack<Object>();
			expectedStack.Push(expected);
			var actualStack = new Stack<Object>();
			actualStack.Push(actual);

			this.expected = expectedStack;
			this.actual = actualStack;
		}

		public void AssertStructurallyEqual()
		{
			if (CheckNull())
			{
				return;
			}

			CheckTypes();

			if (CheckCircularRefs())
			{
				return;
			}

			Compare();
		}

		private void Compare()
		{
			var expectedObj = expected.Peek();
			var actualObj = actual.Peek();
			var expectedType = expectedObj.GetType();

			if (expectedType.IsPrimitive || expectedType.Equals(typeof(String)))
			{
				if (!expectedObj.Equals(actualObj))
				{
					throw new Exception(String.Format("Expected: {0}, but actual was: {1}", expectedObj, actualObj));
				}

				return;
			}

			if (expectedObj is IEnumerable)
			{
				CompareEnumerables((IEnumerable)expectedObj, (IEnumerable)actualObj);
			}

			CompareEnumerables(GetFields(expectedObj), GetFields(actualObj));
		}

		private void CompareEnumerables(IEnumerable expectedSequence, IEnumerable actualSequence)
		{
			var expectedEnumerator = expectedSequence.GetEnumerator();
			var actualEnumerator = actualSequence.GetEnumerator();

			while (expectedEnumerator.MoveNext())
			{
				if (!actualEnumerator.MoveNext())
				{
					throw new Exception("expected collection is larger than actual collection");
				}

				expected.Push(expectedEnumerator.Current);
				actual.Push(actualEnumerator.Current);
				AssertStructurallyEqual();
				actual.Pop();
				expected.Pop();
			}

			if (actualEnumerator.MoveNext())
			{
				throw new Exception("actual collection is larger than expected collection");
			}
		}

		private IEnumerable<Object> GetFields(Object obj)
		{
			return obj.GetType().GetFields(
				BindingFlags.FlattenHierarchy |
				BindingFlags.GetField |
				BindingFlags.Instance |
				BindingFlags.NonPublic |
				BindingFlags.Public)
				.OrderBy(f => f.Name)
				.Select(f => f.GetValue(obj));
		}

		private Boolean CheckNull()
		{
			var expectedObj = expected.Peek();
			var actualObj = actual.Peek();

			if (expectedObj == null)
			{
				if (actualObj != null)
				{
					throw new Exception(String.Format("Expected: null, but actual was: {0}", actualObj));
				}

				return true;
			}

			if (actualObj == null)
			{
				throw new Exception(String.Format("Expected: {0}, but actual was: null", expectedObj));
			}

			return false;
		}

		private Boolean CheckCircularRefs()
		{
			var expectedCircRefIndexes = expected.Select((o, i) => new { Index = i, Object = o })
												 .Where(obj => Object.ReferenceEquals(obj.Object, expected.Peek()))
												 .Select(obj => obj.Index)
												 .ToArray();

			var actualCircRefIndexes = actual.Select((o, i) => new { Index = i, Object = o })
											 .Where(obj => Object.ReferenceEquals(obj.Object, actual.Peek()))
											 .Select(obj => obj.Index)
											 .ToArray();

			var expectedCircRefString = String.Join(",", expectedCircRefIndexes.Select(n => n.ToString()));
			var actualCircRefString = String.Join(",", actualCircRefIndexes.Select(n => n.ToString()));
			
			if (expectedCircRefString != actualCircRefString)
			{
				throw new Exception("Expected to find circular references: " + expectedCircRefString + ", but got: " + actualCircRefString);
			}

			return expectedCircRefIndexes.Length > 1;
		}

		private void CheckTypes()
		{
			var expectedObj = expected.Peek();
			var actualObj = actual.Peek();

			var expectedType = expectedObj.GetType();
			var actualType = actualObj.GetType();

			if (expectedType != actualType)
			{
				throw new Exception(String.Format("Expected type: {0}, but actual type was: {1}", expectedType, actualType));
			}
		}
	}
}
