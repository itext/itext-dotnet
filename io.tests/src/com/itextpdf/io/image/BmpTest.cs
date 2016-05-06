using System;
using NUnit.Framework;

namespace com.itextpdf.io.image
{
	public class BmpTest
	{
		public const String sourceFolder = @"..\..\resources\image\";

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void OpenBmp1()
		{
			ImageData img = ImageDataFactory.Create(sourceFolder + "WP_20140410_001.bmp");
			NUnit.Framework.Assert.AreEqual(2592, img.GetWidth(), 0);
			NUnit.Framework.Assert.AreEqual(1456, img.GetHeight(), 0);
			NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void OpenBmp2()
		{
			ImageData img = ImageDataFactory.Create(sourceFolder + "WP_20140410_001_gray.bmp"
				);
			NUnit.Framework.Assert.AreEqual(2592, img.GetWidth(), 0);
			NUnit.Framework.Assert.AreEqual(1456, img.GetHeight(), 0);
			NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void OpenBmp3()
		{
			ImageData img = ImageDataFactory.Create(sourceFolder + "WP_20140410_001_monochrome.bmp"
				);
			NUnit.Framework.Assert.AreEqual(2592, img.GetWidth(), 0);
			NUnit.Framework.Assert.AreEqual(1456, img.GetHeight(), 0);
			NUnit.Framework.Assert.AreEqual(1, img.GetBpc());
		}
	}
}
