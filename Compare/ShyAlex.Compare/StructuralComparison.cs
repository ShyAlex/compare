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

		public StructuralDifference GetDifference()
		{
			StructuralDifference difference;

            if (GetNullDifference(out difference))
            {
                return null;
            }
            if (difference != null)
			{
				return difference;
			}
			
			difference = GetTypeDifference();

			if (difference != null)
			{
				return difference;
			}

			if (GetCircularRefsDifference(out difference))
			{
				return null;
			}

			if (difference != null)
			{
				return difference;
			}

			return GetStructuralDifference();
		}

		private StructuralDifference GetStructuralDifference()
		{
			var expectedObj = expected.Peek();
			var actualObj = actual.Peek();
			var expectedType = expectedObj.GetType();

			if (expectedType.IsPrimitive || expectedType.Equals(typeof(String)))
			{
				if (!expectedObj.Equals(actualObj))
				{
					return new StructuralDifference(String.Format("Expected: {0}, but actual was: {1}", expectedObj, actualObj));
				}

				return null;
			}

			if (expectedObj is IEnumerable)
			{
				var difference = CompareEnumerables((IEnumerable)expectedObj, (IEnumerable)actualObj);

				if (difference != null)
				{
					return difference;
				}
			}

			return CompareEnumerables(GetFields(expectedObj), GetFields(actualObj));
		}

		private StructuralDifference CompareEnumerables(IEnumerable expectedSequence, IEnumerable actualSequence)
		{
			var expectedEnumerator = expectedSequence.GetEnumerator();
			var actualEnumerator = actualSequence.GetEnumerator();

			while (expectedEnumerator.MoveNext())
			{
				if (!actualEnumerator.MoveNext())
				{
					return new StructuralDifference("expected collection is larger than actual collection");
				}

				expected.Push(expectedEnumerator.Current);
				actual.Push(actualEnumerator.Current);
				
				var difference = GetDifference();

				if (difference != null)
				{
					return difference;
				}
				
				actual.Pop();
				expected.Pop();
			}

			if (actualEnumerator.MoveNext())
			{
				return new StructuralDifference("actual collection is larger than expected collection");
			}

			return null;
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

		private Boolean GetNullDifference(out StructuralDifference difference)
		{
			var expectedObj = expected.Peek();
			var actualObj = actual.Peek();
            difference = null;

			if (expectedObj == null)
			{
				if (actualObj != null)
				{
					difference = new StructuralDifference(String.Format("Expected: null, but actual was: {0}", actualObj));
                    return false;
				}

				return true;
			}

			if (actualObj == null)
			{
				difference = new StructuralDifference(String.Format("Expected: {0}, but actual was: null", expectedObj));
			}

            return false;
		}

		private Boolean GetCircularRefsDifference(out StructuralDifference difference)
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
				difference = new StructuralDifference("Expected to find circular references: " + expectedCircRefString + ", but got: " + actualCircRefString);
				return false;
			}

			difference = null;
			return expectedCircRefIndexes.Length > 1;
		}

        private StructuralDifference GetTypeDifference()
        {
            var expectedObj = expected.Peek();
            var actualObj = actual.Peek();

            var expectedType = expectedObj.GetType();
            var actualType = actualObj.GetType();

            if (expectedType != actualType)
            {
                return new StructuralDifference(String.Format("Expected type: {0}, but actual type was: {1}", expectedType, actualType));
            }

            return null;
        }
	}
}
