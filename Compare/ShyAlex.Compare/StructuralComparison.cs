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
        private class Field
        {
            public String Name { get; private set; }

            public Object Value { get; private set; }

            public Field(String name, Object value)
            {
                Name = name;
                Value = value;
            }
        }

		private readonly Stack<Field> expected;

		private readonly Stack<Field> actual;

		public StructuralComparison(Object expected, Object actual)
		{
			var expectedStack = new Stack<Field>();
			expectedStack.Push(new Field("root-object", expected));
			var actualStack = new Stack<Field>();
			actualStack.Push(new Field("root-object", actual));

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
			var expectedObj = expected.Peek().Value;
			var actualObj = actual.Peek().Value;
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
            var i = 0;

			while (expectedEnumerator.MoveNext())
			{
				if (!actualEnumerator.MoveNext())
				{
					return new StructuralDifference("expected collection is larger than actual collection");
				}

                var fieldName = String.Format("[{0}]", i++);
				expected.Push(new Field(fieldName, expectedEnumerator.Current));
				actual.Push(new Field(fieldName, actualEnumerator.Current));
				
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
			var expectedObj = expected.Peek().Value;
			var actualObj = actual.Peek().Value;
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
			var expectedCircRefIndexes = expected.Select((f, i) => new { Index = i, Field = f })
												 .Where(obj => Object.ReferenceEquals(obj.Field.Value, expected.Peek().Value))
												 .Select(obj => obj.Index)
												 .ToArray();

			var actualCircRefIndexes = actual.Select((f, i) => new { Index = i, Field = f })
											 .Where(obj => Object.ReferenceEquals(obj.Field.Value, actual.Peek().Value))
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
            var expectedObj = expected.Peek().Value;
            var actualObj = actual.Peek().Value;

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
