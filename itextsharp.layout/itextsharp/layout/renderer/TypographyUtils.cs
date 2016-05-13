/*
$Id: 0052e99e2ffa349c4c7771d1feba7dce4041b081 $

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using System.Reflection;
using Java.Lang;
using iTextSharp.IO.Font;
using iTextSharp.IO.Font.Otf;
using iTextSharp.IO.Log;
using iTextSharp.Kernel.Font;
using iTextSharp.Layout.Property;

namespace iTextSharp.Layout.Renderer
{
	internal class TypographyUtils
	{
		private static readonly Logger logger = LoggerFactory.GetLogger(typeof(TypographyUtils
			));

		private const String TYPOGRAPHY_PACKAGE = "com.itextpdf.typography.";

		private static readonly ICollection<Character.UnicodeScript> SUPPORTED_SCRIPTS;

		private static readonly bool TYPOGRAPHY_MODULE_INITIALIZED;

		static TypographyUtils()
		{
			bool moduleFound = false;
			try
			{
				iTextSharp.GetType("com.itextpdf.typography.shaping.Shaper");
				moduleFound = true;
			}
			catch (TypeLoadException)
			{
			}
			TYPOGRAPHY_MODULE_INITIALIZED = moduleFound;
			if (moduleFound)
			{
				SUPPORTED_SCRIPTS = GetSupportedScripts();
			}
			else
			{
				SUPPORTED_SCRIPTS = null;
			}
		}

		internal static void ApplyOtfScript(FontProgram fontProgram, GlyphLine text, Character.UnicodeScript
			 script)
		{
			if (!TYPOGRAPHY_MODULE_INITIALIZED)
			{
				logger.Warn("Cannot find advanced typography module, which was implicitly required by one of the layout properties"
					);
			}
			else
			{
				CallMethod(TYPOGRAPHY_PACKAGE + "shaping.Shaper", "applyOtfScript", new Type[] { 
					typeof(TrueTypeFont), typeof(GlyphLine), typeof(Character.UnicodeScript) }, fontProgram
					, text, script);
			}
		}

		//Shaper.applyOtfScript((TrueTypeFont)font.getFontProgram(), text, script);
		internal static void ApplyKerning(FontProgram fontProgram, GlyphLine text)
		{
			if (!TYPOGRAPHY_MODULE_INITIALIZED)
			{
				logger.Warn("Cannot find advanced typography module, which was implicitly required by one of the layout properties"
					);
			}
			else
			{
				CallMethod(TYPOGRAPHY_PACKAGE + "shaping.Shaper", "applyKerning", new Type[] { typeof(
					FontProgram), typeof(GlyphLine) }, fontProgram, text);
			}
		}

		//Shaper.applyKerning(font.getFontProgram(), text);
		internal static byte[] GetBidiLevels(BaseDirection baseDirection, int[] unicodeIds
			)
		{
			if (!TYPOGRAPHY_MODULE_INITIALIZED)
			{
				logger.Warn("Cannot find advanced typography module, which was implicitly required by one of the layout properties"
					);
			}
			else
			{
				byte direction;
				switch (baseDirection)
				{
					case BaseDirection.LEFT_TO_RIGHT:
					{
						direction = 0;
						break;
					}

					case BaseDirection.RIGHT_TO_LEFT:
					{
						direction = 1;
						break;
					}

					case BaseDirection.DEFAULT_BIDI:
					default:
					{
						direction = 2;
						break;
					}
				}
				int len = unicodeIds.Length;
				byte[] types = (byte[])CallMethod(TYPOGRAPHY_PACKAGE + "bidi.BidiCharacterMap", "getCharacterTypes"
					, new Type[] { typeof(int[]), typeof(int), typeof(int) }, unicodeIds, 0, len);
				//byte[] types = BidiCharacterMap.getCharacterTypes(unicodeIds, 0, text.end - text.start;
				byte[] pairTypes = (byte[])CallMethod(TYPOGRAPHY_PACKAGE + "bidi.BidiBracketMap", 
					"getBracketTypes", new Type[] { typeof(int[]), typeof(int), typeof(int) }, unicodeIds
					, 0, len);
				//byte[] pairTypes = BidiBracketMap.getBracketTypes(unicodeIds, 0, text.end - text.start);
				int[] pairValues = (int[])CallMethod(TYPOGRAPHY_PACKAGE + "bidi.BidiBracketMap", 
					"getBracketValues", new Type[] { typeof(int[]), typeof(int), typeof(int) }, unicodeIds
					, 0, len);
				//int[] pairValues = BidiBracketMap.getBracketValues(unicodeIds, 0, text.end - text.start);
				Object bidiReorder = CallConstructor(TYPOGRAPHY_PACKAGE + "bidi.BidiAlgorithm", new 
					Type[] { typeof(byte[]), typeof(byte[]), typeof(int[]), typeof(byte) }, types, pairTypes
					, pairValues, direction);
				//BidiAlgorithm bidiReorder = new BidiAlgorithm(types, pairTypes, pairValues, direction);
				return (byte[])CallMethod(TYPOGRAPHY_PACKAGE + "bidi.BidiAlgorithm", "getLevels", 
					bidiReorder, new Type[] { typeof(int[]) }, new int[] { len });
			}
			//levels = bidiReorder.getLevels(new int[]{text.end - text.start});
			return null;
		}

		internal static int[] ReorderLine(IList<LineRenderer.RendererGlyph> line, byte[] 
			lineLevels, byte[] levels)
		{
			if (!TYPOGRAPHY_MODULE_INITIALIZED)
			{
				logger.Warn("Cannot find advanced typography module, which was implicitly required by one of the layout properties"
					);
			}
			else
			{
				if (levels == null)
				{
					return null;
				}
				int[] reorder = (int[])CallMethod(TYPOGRAPHY_PACKAGE + "bidi.BidiAlgorithm", "computeReordering"
					, new Type[] { typeof(byte[]) }, lineLevels);
				//int[] reorder = BidiAlgorithm.computeReordering(lineLevels);
				IList<LineRenderer.RendererGlyph> reorderedLine = new List<LineRenderer.RendererGlyph
					>(lineLevels.Length);
				for (int i = 0; i < line.Count; i++)
				{
					reorderedLine.Add(line[reorder[i]]);
					// Mirror RTL glyphs
					if (levels[reorder[i]] % 2 == 1)
					{
						if (reorderedLine[i].glyph.HasValidUnicode())
						{
							int pairedBracket = (int)CallMethod(TYPOGRAPHY_PACKAGE + "bidi.BidiBracketMap", "getPairedBracket"
								, new Type[] { typeof(int) }, reorderedLine[i].glyph.GetUnicode());
							PdfFont font = reorderedLine[i].renderer.GetPropertyAsFont(iTextSharp.Layout.Property.Property
								.FONT);
							//BidiBracketMap.getPairedBracket(reorderedLine.get(i).getUnicode())
							reorderedLine[i] = new LineRenderer.RendererGlyph(font.GetGlyph(pairedBracket), reorderedLine
								[i].renderer);
						}
					}
				}
				line.Clear();
				line.AddAll(reorderedLine);
				return reorder;
			}
			return null;
		}

		internal static ICollection<Character.UnicodeScript> GetSupportedScripts()
		{
			if (!TYPOGRAPHY_MODULE_INITIALIZED)
			{
				logger.Warn("Cannot find advanced typography module, which was implicitly required by one of the layout properties"
					);
				return null;
			}
			else
			{
				if (SUPPORTED_SCRIPTS != null)
				{
					return SUPPORTED_SCRIPTS;
				}
				else
				{
					return (ICollection<Character.UnicodeScript>)CallMethod(TYPOGRAPHY_PACKAGE + "shaping.Shaper"
						, "getSupportedScripts", new Type[] {  });
				}
			}
		}

		internal static bool IsTypographyModuleInitialized()
		{
			return TYPOGRAPHY_MODULE_INITIALIZED;
		}

		private static Object CallMethod(String className, String methodName, Type[] parameterTypes
			, params Object[] args)
		{
			return CallMethod(className, methodName, null, parameterTypes, args);
		}

		private static Object CallMethod(String className, String methodName, Object target
			, Type[] parameterTypes, params Object[] args)
		{
			try
			{
				MethodInfo method = iTextSharp.GetType(className).GetMethod(methodName, parameterTypes
					);
				return method.Invoke(target, args);
			}
			catch (MissingMethodException)
			{
				logger.Warn(String.Format("Cannot find method {0} for class {1}", methodName, className
					));
			}
			catch (TypeLoadException)
			{
				logger.Warn(String.Format("Cannot find class {0}", className));
			}
			catch (TargetInvocationException e)
			{
				throw new Exception(e.InnerException != null ? e.InnerException : null);
			}
			catch (Exception exc)
			{
				throw new Exception(exc);
			}
			return null;
		}

		private static Object CallConstructor(String className, Type[] parameterTypes, params 
			Object[] args)
		{
			try
			{
				ConstructorInfo constructor = iTextSharp.GetType(className).GetConstructor(parameterTypes
					);
				return constructor.NewInstance(args);
			}
			catch (MissingMethodException)
			{
				logger.Warn(String.Format("Cannot find constructor for class {0}", className));
			}
			catch (TypeLoadException)
			{
				logger.Warn(String.Format("Cannot find class {0}", className));
			}
			catch (TargetInvocationException e)
			{
				throw new Exception(e.InnerException != null ? e.InnerException : null);
			}
			catch (Exception exc)
			{
				throw new Exception(exc);
			}
			return null;
		}
	}
}
