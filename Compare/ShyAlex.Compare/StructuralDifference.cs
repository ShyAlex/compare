using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShyAlex.Compare
{
	public class StructuralDifference
	{
		public String Descroption { get; private set; }

		public StructuralDifference(String description)
		{
			Descroption = description;
		}
	}
}
