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
        public static ImageData GetImage(System.Drawing.Image image, Color? color)
		{
			return GetImage(image, color, false);
		}

        /// <summary>
        /// Gets an instance of an Image from a System.Drwaing.Image.
        /// </summary>
        /// <param name="image">the System.Drawing.Image to convert</param>
        /// <param name="color">
        /// if different from null the transparency
        /// pixels are replaced by this color
        /// </param>
        /// <param name="forceBW">if true the image is treated as black and white</param>
        /// <returns>an object of type ImgRaw</returns>
        public static ImageData GetImage(System.Drawing.Image image, Color? color, bool forceBW)
        {
            System.Drawing.Bitmap bm = (System.Drawing.Bitmap)image;
            int w = bm.Width;
            int h = bm.Height;
            int pxv = 0;
            if (forceBW)
            {
                int byteWidth = (w / 8) + ((w & 7) != 0 ? 1 : 0);
                byte[] pixelsByte = new byte[byteWidth * h];

                int index = 0;
                int transColor = 1;
                if (color != null)
                {
                    transColor = (color.Value.R + color.Value.G + color.Value.B < 384) ? 0 : 1;
                }
                int[] transparency = null;
                int cbyte = 0x80;
                int wMarker = 0;
                int currByte = 0;
                if (color != null)
                {
                    for (int j = 0; j < h; j++)
                    {
                        for (int i = 0; i < w; i++)
                        {
                            int alpha = bm.GetPixel(i, j).A;
                            if (alpha < 250)
                            {
                                if (transColor == 1)
                                    currByte |= cbyte;
                            }
                            else
                            {
                                if ((bm.GetPixel(i, j).ToArgb() & 0x888) != 0)
                                    currByte |= cbyte;
                            }
                            cbyte >>= 1;
                            if (cbyte == 0 || wMarker + 1 >= w)
                            {
                                pixelsByte[index++] = (byte)currByte;
                                cbyte = 0x80;
                                currByte = 0;
                            }
                            ++wMarker;
                            if (wMarker >= w)
                                wMarker = 0;
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < h; j++)
                    {
                        for (int i = 0; i < w; i++)
                        {
                            if (transparency == null)
                            {
                                int alpha = bm.GetPixel(i, j).A;
                                if (alpha == 0)
                                {
                                    transparency = new int[2];
                                    transparency[0] = transparency[1] = ((bm.GetPixel(i, j).ToArgb() & 0x888) != 0) ? 1 : 0;
                                }
                            }
                            if ((bm.GetPixel(i, j).ToArgb() & 0x888) != 0)
                                currByte |= cbyte;
                            cbyte >>= 1;
                            if (cbyte == 0 || wMarker + 1 >= w)
                            {
                                pixelsByte[index++] = (byte)currByte;
                                cbyte = 0x80;
                                currByte = 0;
                            }
                            ++wMarker;
                            if (wMarker >= w)
                                wMarker = 0;
                        }
                    }
                }

                return ImageDataFactory.Create(w, h, 1, 1, pixelsByte, transparency);
            }
            else
            {
                byte[] pixelsByte = new byte[w * h * 3];
                byte[] smask = null;

                int index = 0;
                int red = 255;
                int green = 255;
                int blue = 255;
                if (color != null)
                {
                    red = color.Value.R;
                    green = color.Value.G;
                    blue = color.Value.B;
                }
                int[] transparency = null;
                if (color != null)
                {
                    for (int j = 0; j < h; j++)
                    {
                        for (int i = 0; i < w; i++)
                        {
                            int alpha = (bm.GetPixel(i, j).ToArgb() >> 24) & 0xff;
                            if (alpha < 250)
                            {
                                pixelsByte[index++] = (byte)red;
                                pixelsByte[index++] = (byte)green;
                                pixelsByte[index++] = (byte)blue;
                            }
                            else
                            {
                                pxv = bm.GetPixel(i, j).ToArgb();
                                pixelsByte[index++] = (byte)((pxv >> 16) & 0xff);
                                pixelsByte[index++] = (byte)((pxv >> 8) & 0xff);
                                pixelsByte[index++] = (byte)((pxv) & 0xff);
                            }
                        }
                    }
                }
                else
                {
                    int transparentPixel = 0;
                    smask = new byte[w * h];
                    bool shades = false;
                    int smaskPtr = 0;
                    for (int j = 0; j < h; j++)
                    {
                        for (int i = 0; i < w; i++)
                        {
                            pxv = bm.GetPixel(i, j).ToArgb();
                            byte alpha = smask[smaskPtr++] = (byte)((pxv >> 24) & 0xff);
                            /* bugfix by Chris Nokleberg */
                            if (!shades)
                            {
                                if (alpha != 0 && alpha != 255)
                                {
                                    shades = true;
                                }
                                else if (transparency == null)
                                {
                                    if (alpha == 0)
                                    {
                                        transparentPixel = pxv & 0xffffff;
                                        transparency = new int[6];
                                        transparency[0] = transparency[1] = (transparentPixel >> 16) & 0xff;
                                        transparency[2] = transparency[3] = (transparentPixel >> 8) & 0xff;
                                        transparency[4] = transparency[5] = transparentPixel & 0xff;
                                    }
                                }
                                else if ((pxv & 0xffffff) != transparentPixel)
                                {
                                    shades = true;
                                }
                            }
                            pixelsByte[index++] = (byte)((pxv >> 16) & 0xff);
                            pixelsByte[index++] = (byte)((pxv >> 8) & 0xff);
                            pixelsByte[index++] = (byte)(pxv & 0xff);
                        }
                    }
                    if (shades)
                        transparency = null;
                    else
                        smask = null;
                }
                ImageData img = ImageDataFactory.Create(w, h, 3, 8, pixelsByte, transparency);
                if (smask != null)
                {
                    ImageData sm = ImageDataFactory.Create(w, h, 1, 8, smask, null);
                    sm.MakeMask();
                    img.imageMask = sm;
                }
                return img;
            }
        }

        /// <summary>
        /// Converts a .NET image to a Native(PNG, JPG, GIF, WMF) image
        /// </summary>
        /// <param name="image"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public static ImageData GetImage(System.Drawing.Image image, System.Drawing.Imaging.ImageFormat format)
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, format);
            return ImageDataFactory.Create(ms.ToArray());
        }
	}
}
