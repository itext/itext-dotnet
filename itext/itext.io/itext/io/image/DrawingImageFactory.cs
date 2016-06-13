using System;
using System.Drawing;
using System.IO;

namespace iText.IO.Image
{
	internal class DrawingImageFactory
	{
		/// <summary>Gets an instance of an Image from a java.awt.Image</summary>
		/// <param name="image">the java.awt.Image to convert</param>
		/// <param name="color">if different from <CODE>null</CODE> the transparency pixels are replaced by this color
		/// 	</param>
		/// <returns>RawImage</returns>
		/// <exception cref="System.IO.IOException"/>
        public static ImageData GetImage(System.Drawing.Image image, System.Drawing.Color color)
		{
			return GetImage(image, color, false);
		}

	    /// <summary>Gets an instance of an Image from a java.awt.Image.</summary>
	    /// <param name="image">the <CODE>java.awt.Image</CODE> to convert</param>
	    /// <param name="color">if different from <CODE>null</CODE> the transparency pixels are replaced by this color
	    /// 	</param>
	    /// <param name="forceBW">if <CODE>true</CODE> the image is treated as black and white
	    /// 	</param>
	    /// <returns>RawImage</returns>
	    /// <exception cref="System.IO.IOException"/>
        public static ImageData GetImage(System.Drawing.Image image, System.Drawing.Color color, bool forceBW)
	    {
	        throw new NotImplementedException();
	    }
	}
}
