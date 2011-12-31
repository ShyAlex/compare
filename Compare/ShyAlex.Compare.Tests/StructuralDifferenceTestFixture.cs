using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace ShyAlex.Compare.Tests
{
    public class StructuralDifferenceTestFixture
    {
        [Test]
        public void WhenNullIsSuppliedToTheConstructorThenAnExceptionIsThrown()
        {
            Assert.Throws<ArgumentNullException>(() => new StructuralDifference(null));
        }

        [Test]
        public void WhenAValidStringIsSuppliedToTheConstructorThenTheDescriptionPropertyIsInitializedProperly()
        {
            var validDescription = "Some difference or other.";
            var diff = new StructuralDifference(validDescription);
            Assert.AreEqual(validDescription, diff.Description);
        }

        [Test]
        public void WhenToStringIsCalledThenTheDescriptionIsReturned()
        {
            var validDescription = "Some difference or other.";
            var diff = new StructuralDifference(validDescription);
            Assert.AreEqual(validDescription, diff.ToString());
        }

        [Test]
        public void WhenGetHashCodeIsCalledThenTheHashCodeOfTheUnderlyingStringIsReturned()
        {
            var validDescription = "Some difference or other.";
            var diff = new StructuralDifference(validDescription);
            Assert.AreEqual(validDescription.GetHashCode(), diff.GetHashCode());
        }

        [Test, TestCaseSource("GetEqualsCases")]
        public Boolean TestEquals(StructuralDifference diff1, StructuralDifference diff2)
        {
            return diff1.Equals(diff2);
        }

        private IEnumerable<ITestCaseData> GetEqualsCases()
        {
            var validDescription = "Some difference or other.";
            var anotherValidDescription = "A big difference.";

            yield return new TestCaseData(
                new StructuralDifference(validDescription),
                null)
                .Returns(false)
                .SetName("A non-null object is not equal to null");

            yield return new TestCaseData(
                new StructuralDifference(validDescription),
                new StructuralDifference(validDescription))
                .Returns(true)
                .SetName("A difference with a certain description is equal to another difference with that same description");

            yield return new TestCaseData(
                new StructuralDifference(validDescription),
                new StructuralDifference(anotherValidDescription))
                .Returns(false)
                .SetName("A difference with a certain description is not equal to another difference with a different description");
        }
    }
}
