using System;
using System.Collections.Generic;
using System.Text;

namespace ExternalNonChangableLibrary
{
	public class ASuperComplexTypeProperty
	{
		public string MegaComplex { get; set; }

		public int HugelyComplex { get; set; }

		public SomeComplexType ZOMG { get; set; }

		override public string ToString()
		{
			return $"{nameof(MegaComplex)}: {MegaComplex}, {nameof(HugelyComplex)}: {HugelyComplex}, {nameof(ZOMG)}: {ZOMG.ToString()}";
		}
	}
}
