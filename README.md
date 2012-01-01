ShyAlex.Compare
===============

ShyAlex.Compare is a small .NET library containing logic allowing objects to be compared structurally.

The comparison works by treating each object as a structure of primitive types (int, char, float etc.) which have known methods for computing equality. If both objects are the same type and have the same primitive values, then they are equal.

The algorithm also takes into account objects that implement IEnumerable such as arrays and lists.

Usage
-----

Wire up ShyAlex.Compare to your unit testing framework:

    static class StructuralAssert
    {
        public static void AreEqual(Object expected, Object actual)
        {
            var comparison = new StructuralComparison(expected, actual);
            var difference = comparison.GetDifference();
            if (difference != null)
            {
                Assert.Fail(difference.Description);
            }
        }
    }

Then simply assert using:

    StructuralAssert.AreEqual(expectedComplexObject, actualComplexObject);

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

    However, this results in potentially unnecessary code going into production, which should ideally be backed by tests. In many cases the default "reference equals" behaviour or some other equality behaviour is desirable for production.

  - Write code to compare objects of those types as a part of the test code, perhaps in an IComparer&lt;Person&gt; or IComparer so that it can be reused amongst tests.

        Assert.AreEqual(0, new PersonComparer().Compare(expectedPerson, actualPerson));

    This allows the comparison code to be defined once outside of production code, but requires maintenance to keep it in line with the production code and does not necessarily provide an informative failure message ("expected: 0; actual: -1").

ShyAlex.Compare allows any two objects to be compared for equality, and provides informative descriptions of any difference.

Issues
------

  - The comparison checks *every* private member variable. As such, you may encounter issues with some types where some values are of little relevance to observed behaviour.

    An example of this is the BCL's List&lt;T&gt; type, which features a backing array. It's possible to have an empty list with a backing array of size 0, but it's also likely that some empty lists will have a backing array of size 4. This difference will be detected by the algorithm, but it's probably insignificant for most tests, giving undesirable behaviour.

    In the case of lists, the sizing of the backing array is fairly predictable and can usually be worked around using the List&lt;T&gt;(Int32) constructor. However, there are other types in the BCL which won't work well with this library, such as the ADO.NET System.Data types, which appear to have unique IDs acquired from a static variable.

  - While supporting IEnumerables, the algorithm does not currently support the comparison of collection objects which only implement a GetEnumerator() method, and do not explicitly implement IEnumerable.

  - The messages regarding circular references are not clear, and refer to indexes in a member chain where the same object was found. Hopefully differences in circular references are pretty rare.

  - Messages for multidimensional arrays are not ideal (we're talking T[,]s here; T[][]s are fine). They're treated like 1-D arrays, so a difference at T[1,1] will read as a difference at T[3].

  - Messages for differences where backing fields for auto-implemented properties are involved can be a little unclear because the names of the backing fields are a little obscure.

  - Messages for some common types can be a little unintuitive. For example, a sample DateTime difference message is:

        Difference at <root-object>.dateData. Expected: 599266080000000000, but actual was: 599266944000000000

    However, that should be a good enough indication of what's wrong.