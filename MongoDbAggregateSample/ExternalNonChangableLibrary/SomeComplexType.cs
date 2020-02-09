using System;
using System.Collections.Generic;
using System.Text;

namespace ExternalNonChangableLibrary
{
	public class SomeComplexType
	{
		public string ImAComplexTypeProperty { get; set; }

		public int SoAmI { get; set; }

		override public bool Equals(object obj)
		{
			if (!(obj is SomeComplexType))
				return false;

			var o = (SomeComplexType)obj;

			return ImAComplexTypeProperty == o.ImAComplexTypeProperty
				|| SoAmI == o.SoAmI;
		}

		override public int GetHashCode()
		{
			return SoAmI
				^ ImAComplexTypeProperty?.GetHashCode() ?? 0;
		}

		override public string ToString()
		{
			return $"{nameof(ImAComplexTypeProperty)}: {ImAComplexTypeProperty}, {nameof(SoAmI)}: {SoAmI}";
		}
	}
}
