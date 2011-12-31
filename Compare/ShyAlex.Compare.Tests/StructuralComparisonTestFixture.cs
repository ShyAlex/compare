using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace ShyAlex.Compare.Tests
{
	public class StructuralComparisonTestFixture
	{
		[Test]
		[TestCaseSource("GetStandardCases")]
		[TestCaseSource("GetCollectionCases")]
		[TestCaseSource("GetCircularReferenceCases")]
		public void GetDifference(Object expected, Object actual)
		{
			var sc = new StructuralComparison(expected, actual);
			var difference = sc.GetDifference();

			if (difference != null)
			{
				throw new Exception(difference.Description);
			}
		}

		private IEnumerable<ITestCaseData> GetStandardCases()
		{
			yield return new TestCaseData(
				null,
				null)
				.SetName("Two nulls are equal");

			yield return new TestCaseData(
				null,
				new Object())
				.Throws(typeof(Exception))
				.SetName("A null and an object instance are not equal");

			yield return new TestCaseData(
				new Object(),
				null)
				.Throws(typeof(Exception))
				.SetName("An object instance and a null are not equal");

			yield return new TestCaseData(
				new Object(),
				new Object())
				.SetName("Two object instances are equal");

			yield return new TestCaseData(
				new Object(),
				String.Empty)
				.Throws(typeof(Exception))
				.SetName("Two objects of different types are not equal");

			yield return new TestCaseData(
				'a',
				'a')
				.SetName("Two a chars are equal");

			yield return new TestCaseData(
				'a',
				'b')
				.Throws(typeof(Exception))
				.SetName("Two differing chars are not equal");

			yield return new TestCaseData(
				String.Empty,
				String.Empty)
				.SetName("Two empty strings are equal");

			yield return new TestCaseData(
				"Hello",
				"There")
				.Throws(typeof(Exception))
				.SetName("Two differing strings are not equal");

			yield return new TestCaseData(
				new DateTime(1900, 1, 1),
				new DateTime(1900, 1, 1))
				.SetName("Two DateTimes representing the same date are equal");

			yield return new TestCaseData(
				new DateTime(1900, 1, 1),
				new DateTime(1900, 1, 2))
				.Throws(typeof(Exception))
				.SetName("Two differing DateTimes are not equal");

			yield return new TestCaseData(
				new Exception("I am some message"),
				new Exception("I am some message"))
				.SetName("Two exceptions with the same message are equal");

			yield return new TestCaseData(
				new Exception("I am some message"),
				new Exception("I am some massage"))
				.Throws(typeof(Exception))
				.SetName("Two exceptions with different messages are not equal");

			yield return new TestCaseData(
				new KeyValuePair<KeyValuePair<KeyValuePair<DateTime, Exception>, String>, Encoding>(
					new KeyValuePair<KeyValuePair<DateTime, Exception>, String>(
						new KeyValuePair<DateTime, Exception>(
							new DateTime(1900, 1, 1),
							new Exception("Hello")),
						"Fish"),
					Encoding.ASCII),
				new KeyValuePair<KeyValuePair<KeyValuePair<DateTime, Exception>, String>, Encoding>(
					new KeyValuePair<KeyValuePair<DateTime, Exception>, String>(
						new KeyValuePair<DateTime, Exception>(
							new DateTime(1900, 1, 1),
							new Exception("Hello")),
						"Fish"),
					Encoding.ASCII))
				.SetName("Two crazy equal composite types are equal");

			yield return new TestCaseData(
				new KeyValuePair<KeyValuePair<KeyValuePair<DateTime, Exception>, String>, Encoding>(
					new KeyValuePair<KeyValuePair<DateTime, Exception>, String>(
						new KeyValuePair<DateTime, Exception>(
							new DateTime(1900, 1, 1),
							new Exception("Hello")),
						"Fish"),
					Encoding.ASCII),
				new KeyValuePair<KeyValuePair<KeyValuePair<DateTime, Exception>, String>, Encoding>(
					new KeyValuePair<KeyValuePair<DateTime, Exception>, String>(
						new KeyValuePair<DateTime, Exception>(
							new DateTime(1900, 1, 1),
							new Exception("Hello")),
						"Fish"),
					Encoding.Unicode))
				.Throws(typeof(Exception))
				.SetName("Two crazy non-equal composite types are not equal");
		}

		private IEnumerable<ITestCaseData> GetCollectionCases()
		{
			yield return new TestCaseData(
				new String[] { "Hello", "There", "Matey", "Arrgghhh" },
				new String[] { "Hello", "There", "Matey", "Arrgghhh" })
				.SetName("Two equal arrays are equal");

			yield return new TestCaseData(
				new String[] { "Hello", "There", "Matey", "Arrgghhh" },
				new String[] { "Hello", "There", "Mate", "Arrgghhh" })
				.Throws(typeof(Exception))
				.SetName("Two differing arrays are not equal");

			yield return new TestCaseData(
				new String[] { "Hello", "There", "Matey" },
				new String[] { "Hello", "There", "Matey", "Arrgghhh" })
				.Throws(typeof(Exception))
				.SetName("Two arrays where the first is shorter than the second are not equal");

			yield return new TestCaseData(
				new String[] { "Hello", "There", "Matey", "Arrgghhh" },
				new String[] { "Hello", "There", "Matey" })
				.Throws(typeof(Exception))
				.SetName("Two arrays where the first is longer than the second are not equal");

			yield return new TestCaseData(
				new String[,] { { "A", "B" }, { "C", "D" } },
				new String[,] { { "A", "B" }, { "C", "D" } })
				.SetName("Two similar square arrays are equal");

			yield return new TestCaseData(
				new String[,] { { "A", "B" }, { "C", "D" } },
				new String[,] { { "A", "B" }, { "C", "d" } })
				.Throws(typeof(Exception))
				.SetName("Two differing square arrays are not equal");

			yield return new TestCaseData(
				new String[][] { new String[] { "A", "B" }, new String[] { "C", "D", "E" } },
				new String[][] { new String[] { "A", "B" }, new String[] { "C", "D", "E" } })
				.SetName("Two similar multi dimenstional arrays are equal");

			yield return new TestCaseData(
				new String[][] { new String[] { "A", "B" }, new String[] { "C", "D", "E" } },
				new String[][] { new String[] { "A", "B" }, new String[] { "C", "d", "E" } })
				.Throws(typeof(Exception))
				.SetName("Two differing multi dimenstional arrays are not equal");
		}

		private IEnumerable<ITestCaseData> GetCircularReferenceCases()
		{
			var array = new Object[] { null, 1, "Hello", new DateTime(2010, 1, 1), new Exception("Hello There!") };
			array[0] = array;

			var array2 = new Object[] { null, 1, "Hello", new DateTime(2010, 1, 1), new Exception("Hello There!") };
			array2[0] = array2;

			yield return new TestCaseData(
				array,
				array2)
				.SetName("The compare can cope with circular references");

			var array3 = new Object[] { array, null, "Hello", new DateTime(2010, 1, 1), new Exception("Hello There!") };
			array3[1] = array3;

			yield return new TestCaseData(
				array,
				array3)
				.Throws(typeof(Exception))
				.SetName("The compare can cope with objects with circular references that aren't equal");

			// Here, the circular reference is on the second node, back to the first node.
			var root1 = new Container();
			var root1node1 = new Container();
			root1.Property = root1node1;
			root1node1.Property = root1;

			// Here, the circular reference is on the second node, back to the second node.
			var root2 = new Container();
			var root2node1 = new Container();
			root2.Property = root2node1;
			root2node1.Property = root2node1;

			yield return new TestCaseData(
				root1,
				root2)
				.Throws(typeof(Exception))
				.SetName("Two node graphs which contain different circular references are not equal");

			// root3 is similar to root1
			var root3 = new Container();
			var root3node1 = new Container();
			root3.Property = root3node1;
			root3node1.Property = root3;

			yield return new TestCaseData(
				root1,
				root3)
				.SetName("Two structurally similar node graphs are equal");
		}

		private class Container
		{
			public Object Property { get; set; }
		}
	}
}
