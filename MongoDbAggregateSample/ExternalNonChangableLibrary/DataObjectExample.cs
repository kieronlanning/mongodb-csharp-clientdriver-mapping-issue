
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExternalNonChangableLibrary
{
	public class DataObjectExample : DataObjectRoot
	{
		public int SomeProperty { get; private set; }
		
		public string[] AStringArray { get; private set; } = new string[0];
		
		public SomeComplexType[] AComplexTypeArray { get; private set; } = new SomeComplexType[0];

		public DateTimeOffset CreatedDateTime { get; private set; }

		public void SetSomeProperty(int someProperty)
		{
			// ...shortened for example.

			SomeProperty = someProperty;
		}

		public void AddString(string item)
		{
			// ...shortened for example.

			AStringArray = AStringArray.Append(item).ToArray();
		}

		public void RemoveString(string item)
		{
			// ...shortened for example.

			AStringArray = AStringArray.Except(new[] { item }).ToArray();
		}

		public void ClearStrings()
		{
			// ...shortened for example.

			AStringArray = new string[0];
		}

		public void AddComplexType(SomeComplexType item)
		{
			// ...shortened for example.

			AComplexTypeArray = AComplexTypeArray.Append(item).ToArray();
		}

		public void RemoveComplexType(SomeComplexType item)
		{
			// ...shortened for example.

			AComplexTypeArray = AComplexTypeArray.Except(new[] { item }).ToArray();
		}

		public void ClearComplexTypes()
		{
			// ...shortened for example.

			AComplexTypeArray = new SomeComplexType[0];
		}

		public void SetDateTime(DateTimeOffset dateTime)
		{
			CreatedDateTime = dateTime;
		}
	}
}
