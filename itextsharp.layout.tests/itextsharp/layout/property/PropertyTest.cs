using System;
using System.Collections.Generic;
using System.Reflection;
using iTextSharp.Test;

namespace iTextSharp.Layout.Property
{
	public class PropertyTest : ExtendedITextTest
	{
		/// <exception cref="System.MemberAccessException"/>
		[NUnit.Framework.Test]
		public virtual void PropertyUniquenessTest()
		{
			ICollection<int> fieldValues = new HashSet<int>();
			int maxFieldValue = 1;
			foreach (FieldInfo field in typeof(iTextSharp.Layout.Property.Property).GetFields
				())
			{
				if (field.FieldType == typeof(int))
				{
					int value = (int)field.GetValue(null);
					maxFieldValue = Math.Max(maxFieldValue, value);
					if (fieldValues.Contains(value))
					{
						NUnit.Framework.Assert.Fail("Multiple fields with same value");
					}
					fieldValues.Add(value);
				}
			}
			for (int i = 1; i <= maxFieldValue; i++)
			{
				if (!fieldValues.Contains(i))
				{
					NUnit.Framework.Assert.Fail(String.Format("Missing value: {0}", i));
				}
			}
			System.Console.Out.WriteLine(String.Format("Max field value: {0}", maxFieldValue)
				);
		}
	}
}
