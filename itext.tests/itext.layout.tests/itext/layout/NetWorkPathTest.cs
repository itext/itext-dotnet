using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using iText.IO.Image;
using iText.Layout.Element;

namespace iText.Layout
{
    // This test is present only in c#
    class NetWorkPathTest
    {
        [NUnit.Framework.Test]
        public virtual void NetworkPathImageTest()
        {
            var fullImagePath = @"\\someVeryRandomWords\SomeVeryRandomName.img";
            string startOfMsg = null;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            try
            {
                Image drawing = new Image(ImageDataFactory.Create(fullImagePath));
            }
            catch (Exception e)
            {
                if (e.InnerException != null && e.InnerException.Message.Length > 18)
                    startOfMsg = e.InnerException.Message.Substring(0, 19);
            }
            NUnit.Framework.Assert.IsNotNull(startOfMsg);
            NUnit.Framework.Assert.AreNotEqual("Could not find file", startOfMsg);
        }
    }
}
