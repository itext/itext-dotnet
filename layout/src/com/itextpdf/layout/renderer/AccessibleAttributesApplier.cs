/*
$Id: 9ce50a5ebb7d3d0ab8edb51e824ea54979d7d84e $

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
using System.Collections;
using System.Collections.Generic;
using com.itextpdf.kernel.color;
using com.itextpdf.kernel.geom;
using com.itextpdf.kernel.pdf;
using com.itextpdf.kernel.pdf.tagging;
using com.itextpdf.kernel.pdf.tagutils;
using com.itextpdf.layout;
using com.itextpdf.layout.border;
using com.itextpdf.layout.element;

namespace com.itextpdf.layout.renderer
{
	/// <summary>
	/// Writes standard structure attributes to the IAccessibleElement based on the layout element properties
	/// and renderer layout result.
	/// </summary>
	public class AccessibleAttributesApplier
	{
		public static void ApplyLayoutAttributes(PdfName role, AbstractRenderer renderer, 
			PdfDocument doc)
		{
			if (!(renderer.GetModelElement() is IAccessibleElement))
			{
				return;
			}
			int tagType = PdfStructElem.IdentifyType(doc, role);
			PdfDictionary attributes = new PdfDictionary();
			PdfName attributesType = PdfName.Layout;
			attributes.Put(PdfName.O, attributesType);
			PdfDictionary roleMap = doc.GetStructTreeRoot().GetRoleMap();
			if (roleMap.ContainsKey(role))
			{
				role = roleMap.GetAsName(role);
			}
			//TODO WritingMode attribute applying when needed
			ApplyCommonLayoutAttributes(renderer, attributes);
			if (tagType == PdfStructElem.BlockLevel)
			{
				ApplyBlockLevelLayoutAttributes(role, renderer, attributes, doc);
			}
			if (tagType == PdfStructElem.InlineLevel)
			{
				ApplyInlineLevelLayoutAttributes(renderer, attributes);
			}
			if (tagType == PdfStructElem.Illustration)
			{
				ApplyIllustrationLayoutAttributes(renderer, attributes);
			}
			if (attributes.Size() > 1)
			{
				AccessibilityProperties properties = ((IAccessibleElement)renderer.GetModelElement
					()).GetAccessibilityProperties();
				RemoveSameAttributesTypeIfPresent(properties, attributesType);
				properties.AddAttributes(attributes);
			}
		}

		public static void ApplyListAttributes(AbstractRenderer renderer)
		{
			if (!(renderer.GetModelElement() is List))
			{
				return;
			}
			PdfDictionary attributes = new PdfDictionary();
			PdfName attributesType = PdfName.List;
			attributes.Put(PdfName.O, attributesType);
			Object listSymbol = renderer.GetProperty(Property.LIST_SYMBOL);
			if (listSymbol is Property.ListNumberingType)
			{
				Property.ListNumberingType numberingType = (Property.ListNumberingType)listSymbol;
				attributes.Put(PdfName.ListNumbering, TransformNumberingTypeToName(numberingType)
					);
			}
			if (attributes.Size() > 1)
			{
				AccessibilityProperties properties = ((IAccessibleElement)renderer.GetModelElement
					()).GetAccessibilityProperties();
				RemoveSameAttributesTypeIfPresent(properties, attributesType);
				properties.AddAttributes(attributes);
			}
		}

		public static void ApplyTableAttributes(AbstractRenderer renderer)
		{
			if (!(renderer.GetModelElement() is IAccessibleElement))
			{
				return;
			}
			IAccessibleElement accessibleElement = (IAccessibleElement)renderer.GetModelElement
				();
			PdfDictionary attributes = new PdfDictionary();
			PdfName attributesType = PdfName.Table;
			attributes.Put(PdfName.O, attributesType);
			if (accessibleElement is Cell)
			{
				Cell cell = (Cell)accessibleElement;
				if (cell.GetRowspan() != 1)
				{
					attributes.Put(PdfName.RowSpan, new PdfNumber(cell.GetRowspan()));
				}
				if (cell.GetColspan() != 1)
				{
					attributes.Put(PdfName.ColSpan, new PdfNumber(cell.GetColspan()));
				}
			}
			if (attributes.Size() > 1)
			{
				AccessibilityProperties properties = accessibleElement.GetAccessibilityProperties
					();
				RemoveSameAttributesTypeIfPresent(properties, attributesType);
				properties.AddAttributes(attributes);
			}
		}

		private static void ApplyCommonLayoutAttributes(AbstractRenderer renderer, PdfDictionary
			 attributes)
		{
			Color backgroundColor = renderer.GetPropertyAsColor(Property.BACKGROUND);
			if (backgroundColor != null && backgroundColor is DeviceRgb)
			{
				attributes.Put(PdfName.BackgroundColor, new PdfArray(backgroundColor.GetColorValue
					()));
			}
			//TODO NOTE: applying border attributes for cells is temporarily turned off on purpose. Remove this 'if' in future.
			// The reason is that currently, we can't distinguish if all cells have same border style or not.
			// Therefore for every cell in every table we have to write the same border attributes, which creates lots of clutter.
			if (!(renderer.GetModelElement() is Cell))
			{
				ApplyBorderAttributes(renderer, attributes);
			}
			ApplyPaddingAttribute(renderer, attributes);
			Color color = renderer.GetPropertyAsColor(Property.FONT_COLOR);
			if (color != null && color is DeviceRgb)
			{
				attributes.Put(PdfName.Color, new PdfArray(color.GetColorValue()));
			}
		}

		private static void ApplyBlockLevelLayoutAttributes(PdfName role, AbstractRenderer
			 renderer, PdfDictionary attributes, PdfDocument doc)
		{
			float[] margins = new float[] { renderer.GetPropertyAsFloat(Property.MARGIN_TOP), 
				renderer.GetPropertyAsFloat(Property.MARGIN_BOTTOM), renderer.GetPropertyAsFloat
				(Property.MARGIN_LEFT), renderer.GetPropertyAsFloat(Property.MARGIN_RIGHT) };
			int[] marginsOrder = new int[] { 0, 1, 2, 3 };
			//TODO set depending on writing direction
			float spaceBefore = margins[marginsOrder[0]];
			if (spaceBefore != null && spaceBefore != 0)
			{
				attributes.Put(PdfName.SpaceBefore, new PdfNumber(spaceBefore));
			}
			float spaceAfter = margins[marginsOrder[1]];
			if (spaceAfter != null && spaceAfter != 0)
			{
				attributes.Put(PdfName.SpaceAfter, new PdfNumber(spaceAfter));
			}
			float startIndent = margins[marginsOrder[2]];
			if (startIndent != null && startIndent != 0)
			{
				attributes.Put(PdfName.StartIndent, new PdfNumber(startIndent));
			}
			float endIndent = margins[marginsOrder[3]];
			if (endIndent != null && endIndent != 0)
			{
				attributes.Put(PdfName.EndIndent, new PdfNumber(endIndent));
			}
			float firstLineIndent = renderer.GetProperty(Property.FIRST_LINE_INDENT);
			if (firstLineIndent != null && firstLineIndent != 0)
			{
				attributes.Put(PdfName.TextIndent, new PdfNumber(firstLineIndent));
			}
			Property.TextAlignment textAlignment = renderer.GetProperty(Property.TEXT_ALIGNMENT
				);
			if (textAlignment != null && (!role.Equals(PdfName.TH) && !role.Equals(PdfName.TD
				)))
			{
				//for table cells there is an InlineAlign attribute (see below)
				attributes.Put(PdfName.TextAlign, TransformTextAlignmentValueToName(textAlignment
					));
			}
			bool connectedToTag = doc.GetTagStructureContext().IsElementConnectedToTag((IAccessibleElement
				)renderer.GetModelElement());
			bool elementIsOnSinglePage = !connectedToTag && renderer.isLastRendererForModelElement;
			if (elementIsOnSinglePage)
			{
				Rectangle bbox = renderer.GetOccupiedArea().GetBBox();
				attributes.Put(PdfName.BBox, new PdfArray(bbox));
			}
			if (role.Equals(PdfName.TH) || role.Equals(PdfName.TD) || role.Equals(PdfName.Table
				))
			{
				Property.UnitValue width = renderer.GetProperty(Property.WIDTH);
				if (width != null && width.IsPointValue())
				{
					attributes.Put(PdfName.Width, new PdfNumber(width.GetValue()));
				}
				float height = renderer.GetPropertyAsFloat(Property.HEIGHT);
				if (height != null)
				{
					attributes.Put(PdfName.Height, new PdfNumber(height));
				}
			}
			if (role.Equals(PdfName.TH) || role.Equals(PdfName.TD))
			{
				Property.HorizontalAlignment horizontalAlignment = renderer.GetProperty(Property.
					HORIZONTAL_ALIGNMENT);
				if (horizontalAlignment != null)
				{
					attributes.Put(PdfName.BlockAlign, TransformBlockAlignToName(horizontalAlignment)
						);
				}
				if (textAlignment != null && (textAlignment != Property.TextAlignment.JUSTIFIED &&
					 textAlignment != Property.TextAlignment.JUSTIFIED_ALL))
				{
					//there is no justified alignment for InlineAlign attribute
					attributes.Put(PdfName.InlineAlign, TransformTextAlignmentValueToName(textAlignment
						));
				}
			}
		}

		private static void ApplyInlineLevelLayoutAttributes(AbstractRenderer renderer, PdfDictionary
			 attributes)
		{
			float textRise = renderer.GetPropertyAsFloat(Property.TEXT_RISE);
			if (textRise != null && textRise != 0)
			{
				attributes.Put(PdfName.BaselineShift, new PdfNumber(textRise));
			}
			Object underlines = renderer.GetProperty(Property.UNDERLINE);
			if (underlines != null)
			{
				float fontSize = renderer.GetPropertyAsFloat(Property.FONT_SIZE);
				Property.Underline underline = null;
				if (underlines is IList && !((IList)underlines).IsEmpty() && ((IList)underlines)[
					0] is Property.Underline)
				{
					// in standard attributes only one text decoration could be described for an element. That's why we take only the first underline from the list.
					underline = (Property.Underline)((IList)underlines)[0];
				}
				else
				{
					if (underlines is Property.Underline)
					{
						underline = (Property.Underline)underlines;
					}
				}
				if (underline != null)
				{
					attributes.Put(PdfName.TextDecorationType, underline.GetYPosition(fontSize) > 0 ? 
						PdfName.LineThrough : PdfName.Underline);
					if (underline.GetColor() is DeviceRgb)
					{
						attributes.Put(PdfName.TextDecorationColor, new PdfArray(underline.GetColor().GetColorValue
							()));
					}
					attributes.Put(PdfName.TextDecorationThickness, new PdfNumber(underline.GetThickness
						(fontSize)));
				}
			}
		}

		private static void ApplyIllustrationLayoutAttributes(AbstractRenderer renderer, 
			PdfDictionary attributes)
		{
			Rectangle bbox = renderer.GetOccupiedArea().GetBBox();
			attributes.Put(PdfName.BBox, new PdfArray(bbox));
			Property.UnitValue width = renderer.GetProperty(Property.WIDTH);
			if (width != null && width.IsPointValue())
			{
				attributes.Put(PdfName.Width, new PdfNumber(width.GetValue()));
			}
			else
			{
				attributes.Put(PdfName.Width, new PdfNumber(bbox.GetWidth()));
			}
			float height = renderer.GetPropertyAsFloat(Property.HEIGHT);
			if (height != null)
			{
				attributes.Put(PdfName.Height, new PdfNumber(height));
			}
			else
			{
				attributes.Put(PdfName.Height, new PdfNumber(bbox.GetHeight()));
			}
		}

		private static void ApplyPaddingAttribute(AbstractRenderer renderer, PdfDictionary
			 attributes)
		{
			float[] paddings = new float[] { renderer.GetPropertyAsFloat(Property.PADDING_TOP
				), renderer.GetPropertyAsFloat(Property.PADDING_RIGHT), renderer.GetPropertyAsFloat
				(Property.PADDING_BOTTOM), renderer.GetPropertyAsFloat(Property.PADDING_LEFT) };
			PdfObject padding = null;
			if (paddings[0] == paddings[1] && paddings[0] == paddings[2] && paddings[0] == paddings
				[3])
			{
				if (paddings[0] != 0)
				{
					padding = new PdfNumber(paddings[0]);
				}
			}
			else
			{
				PdfArray paddingArray = new PdfArray();
				int[] paddingsOrder = new int[] { 0, 1, 2, 3 };
				//TODO set depending on writing direction
				foreach (int i in paddingsOrder)
				{
					paddingArray.Add(new PdfNumber(paddings[i]));
				}
				padding = paddingArray;
			}
			if (padding != null)
			{
				attributes.Put(PdfName.Padding, padding);
			}
		}

		private static void ApplyBorderAttributes(AbstractRenderer renderer, PdfDictionary
			 attributes)
		{
			bool specificBorderProperties = renderer.GetProperty(Property.BORDER_TOP) != null
				 || renderer.GetProperty(Property.BORDER_RIGHT) != null || renderer.GetProperty(
				Property.BORDER_BOTTOM) != null || renderer.GetProperty(Property.BORDER_LEFT) !=
				 null;
			bool generalBorderProperties = !specificBorderProperties && renderer.GetProperty(
				Property.BORDER) != null;
			if (generalBorderProperties)
			{
				Border generalBorder = renderer.GetProperty(Property.BORDER);
				Color generalBorderColor = generalBorder.GetColor();
				int borderType = generalBorder.GetType();
				float borderWidth = generalBorder.GetWidth();
				if (generalBorderColor is DeviceRgb)
				{
					attributes.Put(PdfName.BorderColor, new PdfArray(generalBorderColor.GetColorValue
						()));
					attributes.Put(PdfName.BorderStyle, TransformBorderTypeToName(borderType));
					attributes.Put(PdfName.BorderThikness, new PdfNumber(borderWidth));
				}
			}
			if (specificBorderProperties)
			{
				PdfArray borderColors = new PdfArray();
				PdfArray borderTypes = new PdfArray();
				PdfArray borderWidths = new PdfArray();
				bool atLeastOneRgb = false;
				Border[] borders = renderer.GetBorders();
				bool allColorsEqual = true;
				bool allTypesEqual = true;
				bool allWidthsEqual = true;
				for (int i = 1; i < borders.Length; i++)
				{
					Border border = borders[i];
					if (border != null)
					{
						if (!border.GetColor().Equals(borders[0].GetColor()))
						{
							allColorsEqual = false;
						}
						if (border.GetWidth() != borders[0].GetWidth())
						{
							allWidthsEqual = false;
						}
						if (border.GetType() != borders[0].GetType())
						{
							allTypesEqual = false;
						}
					}
				}
				int[] borderOrder = new int[] { 0, 1, 2, 3 };
				//TODO set depending on writing direction
				foreach (int i_1 in borderOrder)
				{
					if (borders[i_1] != null)
					{
						if (borders[i_1].GetColor() is DeviceRgb)
						{
							borderColors.Add(new PdfArray(borders[i_1].GetColor().GetColorValue()));
							atLeastOneRgb = true;
						}
						else
						{
							borderColors.Add(PdfNull.PDF_NULL);
						}
						borderTypes.Add(TransformBorderTypeToName(borders[i_1].GetType()));
						borderWidths.Add(new PdfNumber(borders[i_1].GetWidth()));
					}
					else
					{
						borderColors.Add(PdfNull.PDF_NULL);
						borderTypes.Add(PdfName.None);
						borderWidths.Add(PdfNull.PDF_NULL);
					}
				}
				if (atLeastOneRgb)
				{
					if (allColorsEqual)
					{
						attributes.Put(PdfName.BorderColor, borderColors.Get(0));
					}
					else
					{
						attributes.Put(PdfName.BorderColor, borderColors);
					}
				}
				if (allTypesEqual)
				{
					attributes.Put(PdfName.BorderStyle, borderTypes.Get(0));
				}
				else
				{
					attributes.Put(PdfName.BorderStyle, borderTypes);
				}
				if (allWidthsEqual)
				{
					attributes.Put(PdfName.BorderThikness, borderWidths.Get(0));
				}
				else
				{
					attributes.Put(PdfName.BorderThikness, borderWidths);
				}
			}
		}

		private static PdfName TransformTextAlignmentValueToName(Property.TextAlignment textAlignment
			)
		{
			//TODO set rightToLeft value according with actual text content if it is possible.
			bool isLeftToRight = true;
			switch (textAlignment)
			{
				case Property.TextAlignment.LEFT:
				{
					if (isLeftToRight)
					{
						return PdfName.Start;
					}
					else
					{
						return PdfName.End;
					}
					goto case Property.TextAlignment.CENTER;
				}

				case Property.TextAlignment.CENTER:
				{
					return PdfName.Center;
				}

				case Property.TextAlignment.RIGHT:
				{
					if (isLeftToRight)
					{
						return PdfName.End;
					}
					else
					{
						return PdfName.Start;
					}
					goto case Property.TextAlignment.JUSTIFIED;
				}

				case Property.TextAlignment.JUSTIFIED:
				case Property.TextAlignment.JUSTIFIED_ALL:
				{
					return PdfName.Justify;
				}

				default:
				{
					return PdfName.Start;
				}
			}
		}

		private static PdfName TransformBlockAlignToName(Property.HorizontalAlignment horizontalAlignment
			)
		{
			//TODO set rightToLeft value according with actual text content if it is possible.
			bool isLeftToRight = true;
			switch (horizontalAlignment)
			{
				case Property.HorizontalAlignment.LEFT:
				{
					if (isLeftToRight)
					{
						return PdfName.Before;
					}
					else
					{
						return PdfName.After;
					}
					goto case Property.HorizontalAlignment.CENTER;
				}

				case Property.HorizontalAlignment.CENTER:
				{
					return PdfName.Middle;
				}

				case Property.HorizontalAlignment.RIGHT:
				{
					if (isLeftToRight)
					{
						return PdfName.After;
					}
					else
					{
						return PdfName.Before;
					}
					goto default;
				}

				default:
				{
					return PdfName.Before;
				}
			}
		}

		private static PdfName TransformBorderTypeToName(int borderType)
		{
			switch (borderType)
			{
				case Border.SOLID:
				{
					return PdfName.Solid;
				}

				case Border.DASHED:
				{
					return PdfName.Dashed;
				}

				case Border.DOTTED:
				{
					return PdfName.Dotted;
				}

				case Border.ROUND_DOTS:
				{
					return PdfName.Dotted;
				}

				case Border.DOUBLE:
				{
					return PdfName.Double;
				}

				case Border._3D_GROOVE:
				{
					return PdfName.Groove;
				}

				case Border._3D_INSET:
				{
					return PdfName.Inset;
				}

				case Border._3D_OUTSET:
				{
					return PdfName.Outset;
				}

				case Border._3D_RIDGE:
				{
					return PdfName.Ridge;
				}

				default:
				{
					return PdfName.Solid;
				}
			}
		}

		private static PdfName TransformNumberingTypeToName(Property.ListNumberingType numberingType
			)
		{
			switch (numberingType)
			{
				case Property.ListNumberingType.DECIMAL:
				{
					return PdfName.Decimal;
				}

				case Property.ListNumberingType.ROMAN_UPPER:
				{
					return PdfName.UpperRoman;
				}

				case Property.ListNumberingType.ROMAN_LOWER:
				{
					return PdfName.LowerRoman;
				}

				case Property.ListNumberingType.ENGLISH_UPPER:
				case Property.ListNumberingType.GREEK_UPPER:
				{
					return PdfName.UpperAlpha;
				}

				case Property.ListNumberingType.ENGLISH_LOWER:
				case Property.ListNumberingType.GREEK_LOWER:
				{
					return PdfName.LowerAlpha;
				}

				default:
				{
					return PdfName.None;
				}
			}
		}

		/// <summary>The same layout element instance can be added several times to the document.
		/// 	</summary>
		/// <remarks>
		/// The same layout element instance can be added several times to the document.
		/// In that case it will already have attributes which belong to the previous positioning on the page, and because of
		/// that we want to remove those old irrelevant attributes.
		/// </remarks>
		private static void RemoveSameAttributesTypeIfPresent(AccessibilityProperties properties
			, PdfName attributesType)
		{
			IList<PdfDictionary> attributesList = properties.GetAttributesList();
			int i;
			for (i = 0; i < attributesList.Count; i++)
			{
				PdfDictionary attr = attributesList[i];
				if (attributesType.Equals(attr.Get(PdfName.O)))
				{
					break;
				}
			}
			if (i < attributesList.Count)
			{
				attributesList.RemoveAt(i);
			}
		}
	}
}
