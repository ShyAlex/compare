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
		public StructuralDifference GetDifference(Object expected, Object actual)
		{
			var sc = new StructuralComparison(expected, actual);
			return sc.GetDifference();
		}

		private IEnumerable<ITestCaseData> GetStandardCases()
		{
			yield return new TestCaseData(
				null,
				null)
                .Returns(null)
				.SetName("Two nulls are equal");

			yield return new TestCaseData(
				null,
				new Object())
                .Returns(new StructuralDifference("Difference at <root-object>. Expected: null, but actual was: System.Object"))
				.SetName("A null and an object instance are not equal");

			yield return new TestCaseData(
				new Object(),
				null)
				.Returns(new StructuralDifference("Difference at <root-object>. Expected: System.Object, but actual was: null"))
				.SetName("An object instance and a null are not equal");

			yield return new TestCaseData(
				new Object(),
				new Object())
                .Returns(null)
				.SetName("Two object instances are equal");

			yield return new TestCaseData(
				new Object(),
				String.Empty)
				.Returns(new StructuralDifference("Difference at <root-object>. Expected type: System.Object, but actual type was: System.String"))
				.SetName("Two objects of different types are not equal");

			yield return new TestCaseData(
				'a',
				'a')
                .Returns(null)
				.SetName("Two a chars are equal");

			yield return new TestCaseData(
				'a',
				'b')
				.Returns(new StructuralDifference("Difference at <root-object>. Expected: a, but actual was: b"))
				.SetName("Two differing chars are not equal");

			yield return new TestCaseData(
				String.Empty,
				String.Empty)
                .Returns(null)
				.SetName("Two empty strings are equal");

			yield return new TestCaseData(
				"Hello",
				"There")
				.Returns(new StructuralDifference("Difference at <root-object>. Expected: Hello, but actual was: There"))
				.SetName("Two differing strings are not equal");

			yield return new TestCaseData(
				new DateTime(1900, 1, 1),
				new DateTime(1900, 1, 1))
                .Returns(null)
				.SetName("Two DateTimes representing the same date are equal");

			yield return new TestCaseData(
				new DateTime(1900, 1, 1),
				new DateTime(1900, 1, 2))
                .Returns(new StructuralDifference("Difference at <root-object>.dateData. Expected: 599266080000000000, but actual was: 599266944000000000"))
				.SetName("Two differing DateTimes are not equal");

			yield return new TestCaseData(
				new Exception("I am some message"),
				new Exception("I am some message"))
                .Returns(null)
				.SetName("Two exceptions with the same message are equal");

			yield return new TestCaseData(
				new Exception("I am some message"),
				new Exception("I am some massage"))
                .Returns(new StructuralDifference("Difference at <root-object>._message. Expected: I am some message, but actual was: I am some massage"))
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
                .Returns(null)
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
                .Returns(new StructuralDifference("Difference at <root-object>.value. Expected type: System.Text.ASCIIEncoding, but actual type was: System.Text.UnicodeEncoding"))
				.SetName("Two crazy non-equal composite types are not equal");
		}

		private IEnumerable<ITestCaseData> GetCollectionCases()
		{
			yield return new TestCaseData(
				new String[] { "Hello", "There", "Matey", "Arrgghhh" },
				new String[] { "Hello", "There", "Matey", "Arrgghhh" })
                .Returns(null)
				.SetName("Two equal arrays are equal");

			yield return new TestCaseData(
				new String[] { "Hello", "There", "Matey", "Arrgghhh" },
				new String[] { "Hello", "There", "Mate", "Arrgghhh" })
                .Returns(new StructuralDifference("Difference at <root-object>.[2]. Expected: Matey, but actual was: Mate"))
				.SetName("Two differing arrays are not equal");

			yield return new TestCaseData(
				new String[] { "Hello", "There", "Matey" },
				new String[] { "Hello", "There", "Matey", "Arrgghhh" })
                .Returns(new StructuralDifference("Difference at <root-object>. Expected collection is smaller than actual collection"))
				.SetName("Two arrays where the first is shorter than the second are not equal");

			yield return new TestCaseData(
				new String[] { "Hello", "There", "Matey", "Arrgghhh" },
				new String[] { "Hello", "There", "Matey" })
                .Returns(new StructuralDifference("Difference at <root-object>. Expected collection is larger than actual collection"))
				.SetName("Two arrays where the first is longer than the second are not equal");

			yield return new TestCaseData(
				new String[,] { { "A", "B" }, { "C", "D" } },
				new String[,] { { "A", "B" }, { "C", "D" } })
                .Returns(null)
				.SetName("Two similar square arrays are equal");

			yield return new TestCaseData(
				new String[,] { { "A", "B" }, { "C", "D" } },
				new String[,] { { "A", "B" }, { "C", "d" } })
                .Returns(new StructuralDifference("Difference at <root-object>.[3]. Expected: D, but actual was: d"))
				.SetName("Two differing square arrays are not equal");

			yield return new TestCaseData(
				new String[][] { new String[] { "A", "B" }, new String[] { "C", "D", "E" } },
				new String[][] { new String[] { "A", "B" }, new String[] { "C", "D", "E" } })
                .Returns(null)
				.SetName("Two similar multi dimenstional arrays are equal");

			yield return new TestCaseData(
				new String[][] { new String[] { "A", "B" }, new String[] { "C", "D", "E" } },
				new String[][] { new String[] { "A", "B" }, new String[] { "C", "d", "E" } })
                .Returns(new StructuralDifference("Difference at <root-object>.[1].[1]. Expected: D, but actual was: d"))
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
                .Returns(null)
				.SetName("The compare can cope with circular references");

			var array3 = new Object[] { array, null, "Hello", new DateTime(2010, 1, 1), new Exception("Hello There!") };
			array3[1] = array3;

			yield return new TestCaseData(
				array,
				array3)
                .Returns(new StructuralDifference("Difference at <root-object>.[0]. Expected to find circular references: 0,1, but got: 0"))
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
                .Returns(new StructuralDifference("Difference at <root-object>.<Property>k__BackingField.<Property>k__BackingField. Expected to find circular references: 0,2, but got: 0,1"))
				.SetName("Two node graphs which contain different circular references are not equal");

			// root3 is similar to root1
			var root3 = new Container();
			var root3node1 = new Container();
			root3.Property = root3node1;
			root3node1.Property = root3;

			yield return new TestCaseData(
				root1,
				root3)
                .Returns(null)
				.SetName("Two structurally similar node graphs are equal");
		}

		private class Container
		{
			public Object Property { get; set; }
		}
	}
}
