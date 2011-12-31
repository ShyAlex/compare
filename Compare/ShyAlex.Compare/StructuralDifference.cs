using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShyAlex.Compare
{
	public class StructuralDifference
	{
		public String Description { get; private set; }

		public StructuralDifference(String description)
		{
            if (description == null)
            {
                throw new ArgumentNullException("description");
            }

			Description = description;
		}

        public override String ToString()
        {
            return Description;
        }

        public override Boolean Equals(Object obj)
        {
            var other = obj as StructuralDifference;

            if (other == null)
            {
                return false;
            }

            return Description.Equals(other.Description);
        }

        public override Int32 GetHashCode()
        {
            return Description.GetHashCode();
        }
	}
}
