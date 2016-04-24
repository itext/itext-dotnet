/*
$Id: 63ed87dc94632fd914acf873907dc9d889531dc5 $

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
using System.Text;
using com.itextpdf.kernel.geom;
using com.itextpdf.layout;
using com.itextpdf.layout.border;
using com.itextpdf.layout.element;
using com.itextpdf.layout.layout;

namespace com.itextpdf.layout.renderer
{
	public class ParagraphRenderer : BlockRenderer
	{
		protected internal float previousDescent = 0;

		protected internal IList<LineRenderer> lines = null;

		public ParagraphRenderer(Paragraph modelElement)
			: base(modelElement)
		{
		}

		public override LayoutResult Layout(LayoutContext layoutContext)
		{
			int pageNumber = layoutContext.GetArea().GetPageNumber();
			Rectangle parentBBox = layoutContext.GetArea().GetBBox().Clone();
			if (GetProperty(Property.ROTATION_ANGLE) != null)
			{
				parentBBox.MoveDown(AbstractRenderer.INF - parentBBox.GetHeight()).SetHeight(AbstractRenderer
					.INF);
			}
			ApplyMargins(parentBBox, false);
			ApplyBorderBox(parentBBox, false);
			if (IsPositioned())
			{
				float x = GetPropertyAsFloat(Property.X);
				float relativeX = IsFixedLayout() ? 0 : parentBBox.GetX();
				parentBBox.SetX(relativeX + x);
			}
			float blockWidth = RetrieveWidth(parentBBox.GetWidth());
			if (blockWidth != null && (blockWidth < parentBBox.GetWidth() || IsPositioned()))
			{
				parentBBox.SetWidth(blockWidth);
			}
			ApplyPaddings(parentBBox, false);
			IList<Rectangle> areas;
			if (IsPositioned())
			{
				areas = java.util.Collections.SingletonList(parentBBox);
			}
			else
			{
				areas = InitElementAreas(new LayoutArea(pageNumber, parentBBox));
			}
			occupiedArea = new LayoutArea(pageNumber, new Rectangle(parentBBox.GetX(), parentBBox
				.GetY() + parentBBox.GetHeight(), parentBBox.GetWidth(), 0));
			int currentAreaPos = 0;
			Rectangle layoutBox = areas[0].Clone();
			bool anythingPlaced = false;
			bool firstLineInBox = true;
			lines = new List<LineRenderer>();
			LineRenderer currentRenderer = (LineRenderer)new LineRenderer().SetParent(this);
			foreach (IRenderer child in childRenderers)
			{
				currentRenderer.AddChild(child);
			}
			if (0 == childRenderers.Count)
			{
				anythingPlaced = true;
				currentRenderer = null;
				SetProperty(Property.MARGIN_TOP, 0);
				SetProperty(Property.MARGIN_RIGHT, 0);
				SetProperty(Property.MARGIN_BOTTOM, 0);
				SetProperty(Property.MARGIN_LEFT, 0);
				SetProperty(Property.PADDING_TOP, 0);
				SetProperty(Property.PADDING_RIGHT, 0);
				SetProperty(Property.PADDING_BOTTOM, 0);
				SetProperty(Property.PADDING_LEFT, 0);
				SetProperty(Property.BORDER, Border.NO_BORDER);
			}
			float lastYLine = layoutBox.GetY() + layoutBox.GetHeight();
			Property.Leading leading = GetProperty(Property.LEADING);
			float leadingValue = 0;
			float lastLineHeight = 0;
			while (currentRenderer != null)
			{
				currentRenderer.SetProperty(Property.TAB_DEFAULT, GetPropertyAsFloat(Property.TAB_DEFAULT
					));
				currentRenderer.SetProperty(Property.TAB_STOPS, GetProperty(Property.TAB_STOPS));
				float lineIndent = anythingPlaced ? 0 : GetPropertyAsFloat(Property.FIRST_LINE_INDENT
					);
				float availableWidth = layoutBox.GetWidth() - lineIndent;
				Rectangle childLayoutBox = new Rectangle(layoutBox.GetX() + lineIndent, layoutBox
					.GetY(), availableWidth, layoutBox.GetHeight());
				LineLayoutResult result = ((LineLayoutResult)currentRenderer.Layout(new LayoutContext
					(new LayoutArea(pageNumber, childLayoutBox))));
				LineRenderer processedRenderer = null;
				if (result.GetStatus() == LayoutResult.FULL)
				{
					processedRenderer = currentRenderer;
				}
				else
				{
					if (result.GetStatus() == LayoutResult.PARTIAL)
					{
						processedRenderer = (LineRenderer)result.GetSplitRenderer();
					}
				}
				Property.TextAlignment textAlignment = GetProperty(Property.TEXT_ALIGNMENT);
				if (result.GetStatus() == LayoutResult.PARTIAL && textAlignment == Property.TextAlignment
					.JUSTIFIED && !result.IsSplitForcedByNewline() || textAlignment == Property.TextAlignment
					.JUSTIFIED_ALL)
				{
					if (processedRenderer != null)
					{
						processedRenderer.Justify(layoutBox.GetWidth() - lineIndent);
					}
				}
				else
				{
					if (textAlignment != null && textAlignment != Property.TextAlignment.LEFT && processedRenderer
						 != null)
					{
						float deltaX = availableWidth - processedRenderer.GetOccupiedArea().GetBBox().GetWidth
							();
						switch (textAlignment)
						{
							case Property.TextAlignment.RIGHT:
							{
								processedRenderer.Move(deltaX, 0);
								break;
							}

							case Property.TextAlignment.CENTER:
							{
								processedRenderer.Move(deltaX / 2, 0);
								break;
							}
						}
					}
				}
				leadingValue = processedRenderer != null && leading != null ? processedRenderer.GetLeadingValue
					(leading) : 0;
				if (processedRenderer != null && processedRenderer.ContainsImage())
				{
					leadingValue -= previousDescent;
				}
				bool doesNotFit = result.GetStatus() == LayoutResult.NOTHING;
				float deltaY = 0;
				if (!doesNotFit)
				{
					lastLineHeight = processedRenderer.GetOccupiedArea().GetBBox().GetHeight();
					deltaY = lastYLine - leadingValue - processedRenderer.GetYLine();
					// for the first and last line in a paragraph, leading is smaller
					if (firstLineInBox)
					{
						deltaY = -(leadingValue - lastLineHeight) / 2;
					}
					doesNotFit = leading != null && processedRenderer.GetOccupiedArea().GetBBox().GetY
						() + deltaY < layoutBox.GetY();
				}
				if (doesNotFit)
				{
					if (currentAreaPos + 1 < areas.Count)
					{
						layoutBox = areas[++currentAreaPos].Clone();
						lastYLine = layoutBox.GetY() + layoutBox.GetHeight();
						firstLineInBox = true;
						continue;
					}
					else
					{
						bool keepTogether = GetProperty(Property.KEEP_TOGETHER);
						if (keepTogether)
						{
							return new LayoutResult(LayoutResult.NOTHING, occupiedArea, null, this);
						}
						else
						{
							ApplyPaddings(occupiedArea.GetBBox(), true);
							ApplyBorderBox(occupiedArea.GetBBox(), true);
							ApplyMargins(occupiedArea.GetBBox(), true);
							com.itextpdf.layout.renderer.ParagraphRenderer[] split = Split();
							split[0].lines = lines;
							foreach (LineRenderer line in lines)
							{
								split[0].childRenderers.AddAll(line.GetChildRenderers());
							}
							if (processedRenderer != null)
							{
								split[1].childRenderers.AddAll(processedRenderer.GetChildRenderers());
							}
							if (result.GetOverflowRenderer() != null)
							{
								split[1].childRenderers.AddAll(result.GetOverflowRenderer().GetChildRenderers());
							}
							if (anythingPlaced)
							{
								return new LayoutResult(LayoutResult.PARTIAL, occupiedArea, split[0], split[1]);
							}
							else
							{
								if (GetPropertyAsBoolean(Property.FORCED_PLACEMENT))
								{
									parent.SetProperty(Property.FULL, true);
									lines.Add(currentRenderer);
									return new LayoutResult(LayoutResult.FULL, occupiedArea, null, this);
								}
								else
								{
									return new LayoutResult(LayoutResult.NOTHING, occupiedArea, null, this);
								}
							}
						}
					}
				}
				else
				{
					if (leading != null)
					{
						processedRenderer.Move(0, deltaY);
						lastYLine = processedRenderer.GetYLine();
					}
					occupiedArea.SetBBox(Rectangle.GetCommonRectangle(occupiedArea.GetBBox(), processedRenderer
						.GetOccupiedArea().GetBBox()));
					layoutBox.SetHeight(processedRenderer.GetOccupiedArea().GetBBox().GetY() - layoutBox
						.GetY());
					lines.Add(processedRenderer);
					anythingPlaced = true;
					firstLineInBox = false;
					currentRenderer = (LineRenderer)result.GetOverflowRenderer();
					previousDescent = processedRenderer.GetMaxDescent();
				}
			}
			if (!IsPositioned())
			{
				float moveDown = Math.Min((leadingValue - lastLineHeight) / 2, occupiedArea.GetBBox
					().GetY() - layoutBox.GetY());
				occupiedArea.GetBBox().MoveDown(moveDown);
				occupiedArea.GetBBox().SetHeight(occupiedArea.GetBBox().GetHeight() + moveDown);
			}
			float blockHeight = GetPropertyAsFloat(Property.HEIGHT);
			ApplyPaddings(occupiedArea.GetBBox(), true);
			if (blockHeight != null && blockHeight > occupiedArea.GetBBox().GetHeight())
			{
				occupiedArea.GetBBox().MoveDown(blockHeight - occupiedArea.GetBBox().GetHeight())
					.SetHeight(blockHeight);
				ApplyVerticalAlignment();
			}
			if (IsPositioned())
			{
				float y = GetPropertyAsFloat(Property.Y);
				float relativeY = IsFixedLayout() ? 0 : layoutBox.GetY();
				Move(0, relativeY + y - occupiedArea.GetBBox().GetY());
			}
			ApplyBorderBox(occupiedArea.GetBBox(), true);
			ApplyMargins(occupiedArea.GetBBox(), true);
			if (GetProperty(Property.ROTATION_ANGLE) != null)
			{
				ApplyRotationLayout(layoutContext.GetArea().GetBBox().Clone());
				if (IsNotFittingHeight(layoutContext.GetArea()))
				{
					if (!layoutContext.GetArea().IsEmptyArea())
					{
						return new LayoutResult(LayoutResult.NOTHING, occupiedArea, null, this);
					}
				}
			}
			return new LayoutResult(LayoutResult.FULL, occupiedArea, null, null);
		}

		public override IRenderer GetNextRenderer()
		{
			return new com.itextpdf.layout.renderer.ParagraphRenderer((Paragraph)modelElement
				);
		}

		public override T GetDefaultProperty<T>(Property property)
		{
			if ((property == Property.MARGIN_TOP || property == Property.MARGIN_BOTTOM) && parent
				 is CellRenderer)
			{
				return (T)float.ValueOf(0);
			}
			return base.GetDefaultProperty(property);
		}

		protected internal virtual com.itextpdf.layout.renderer.ParagraphRenderer CreateOverflowRenderer
			()
		{
			com.itextpdf.layout.renderer.ParagraphRenderer overflowRenderer = ((com.itextpdf.layout.renderer.ParagraphRenderer
				)GetNextRenderer());
			// Reset first line indent in case of overflow.
			float firstLineIndent = GetPropertyAsFloat(Property.FIRST_LINE_INDENT);
			if (firstLineIndent != 0)
			{
				overflowRenderer.SetProperty(Property.FIRST_LINE_INDENT, 0);
			}
			return overflowRenderer;
		}

		protected internal virtual com.itextpdf.layout.renderer.ParagraphRenderer CreateSplitRenderer
			()
		{
			return ((com.itextpdf.layout.renderer.ParagraphRenderer)GetNextRenderer());
		}

		protected internal virtual com.itextpdf.layout.renderer.ParagraphRenderer[] Split
			()
		{
			com.itextpdf.layout.renderer.ParagraphRenderer splitRenderer = CreateSplitRenderer
				();
			splitRenderer.occupiedArea = occupiedArea.Clone();
			splitRenderer.parent = parent;
			splitRenderer.isLastRendererForModelElement = false;
			com.itextpdf.layout.renderer.ParagraphRenderer overflowRenderer = CreateOverflowRenderer
				();
			overflowRenderer.parent = parent;
			return new com.itextpdf.layout.renderer.ParagraphRenderer[] { splitRenderer, overflowRenderer
				 };
		}

		public override String ToString()
		{
			StringBuilder sb = new StringBuilder();
			if (lines != null && lines.Count > 0)
			{
				foreach (LineRenderer lineRenderer in lines)
				{
					sb.Append(lineRenderer.ToString()).Append("\n");
				}
			}
			else
			{
				foreach (IRenderer renderer in childRenderers)
				{
					sb.Append(renderer.ToString());
				}
			}
			return sb.ToString();
		}

		public override void DrawChildren(DrawContext drawContext)
		{
			if (lines != null)
			{
				foreach (LineRenderer line in lines)
				{
					line.Draw(drawContext);
				}
			}
		}

		public override void Move(float dxRight, float dyUp)
		{
			occupiedArea.GetBBox().MoveRight(dxRight);
			occupiedArea.GetBBox().MoveUp(dyUp);
			foreach (LineRenderer line in lines)
			{
				line.Move(dxRight, dyUp);
			}
		}

		protected internal override float GetFirstYLineRecursively()
		{
			if (lines == null || lines.Count == 0)
			{
				return null;
			}
			return lines[0].GetFirstYLineRecursively();
		}
	}
}
