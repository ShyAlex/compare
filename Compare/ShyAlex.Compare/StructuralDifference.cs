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
			Description = description;
		}
	}
}
