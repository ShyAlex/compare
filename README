ShyAlex.Compare
===============

ShyAlex.Compare is a small .NET library containing logic allowing objects to be compared structurally.

Purpose
-------

ShyAlex.Compare is useful when writing tests which check for the correctness of complex output values.

Imagine a type such as:

    class Person
    {
        IList<Holiday> Holidays { get; private set; }
        String Name { get; private set; }
        Department Department { get; private set; }
    }

with the following related types:

    enum HolidayType
    {
        BankHoliday,
        AnnualLeave
    }

    class Holiday
    {
        DateTime Date { get; private set; }
        HolidayType HolidayType { get; private set; }
    }

    class Department
    {
        String Name { get; private set; }
        String Code { get; private set; };
    }

Typically, there are a few options for checking that complex values such as Person are correct:

  - Write multiple assertions for different parts of the object:

        Assert.AreEqual(expectedPerson.Name, actualPerson.Name);
        Assert.AreEqual(expectedPerson.Department.Code, actualPerson.Department.Code);
        Assert.AreEqual(expectedPerson.Department.Name, actualPerson.Department.Name);
        ...

    However, this results in bloated test code which focuses on the mechanism of how to test rather than the behaviour the test demonstrates.

  - Override Equals and GetHashCode for each class *in production code* and use those methods in the test.

        Assert.AreEqual(expectedPerson, actualPerson);

    However, this results in potentially unnecessary code going into production, which should ideally be backed by tests, and in many cases the default "reference equals" behaviour or other equality behaviour is desirable.

  - Write code to compare objects of those types as a part of the test code, perhaps in an IComparer&lt;Person&gt; or IComparer so that it can be reused amongst tests.

        Assert.AreEqual(0, new PersonComparer().Compare(expectedPerson, actualPerson));

    This allows the comparison code to be defined once outside of production code, but requires maintenance to keep it in line with the production code and does not necessarily provide an informative failure message ("expected: 0; actual: -1").

ShyAlex.Compare allows any two objects to be compared for equality, and provides informative descriptions of any difference.

Usage
-----

Wire up ShyAlex.Compare to your unit testing framework:

    static class StructuralAssert
    {
        public static void AreEqual(Object expected, Object actual)
        {
            var comparison = new StructuralComparison(expected, actual);
            var difference = comparison.AssertStructurallyEqual()
            if (difference != null)
            {
                Assert.Fail(difference.Description);
            }
        }
    }

Then simply assert using:

    StructuralAssert.AreEqual(expectedComplexObject, actualComplexObject);