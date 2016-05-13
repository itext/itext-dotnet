/*
$Id: d1e630447fd33b3db3f975bc9ed379d8baacf326 $

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
using com.itextpdf.io;
using com.itextpdf.io.log;
using com.itextpdf.layout.layout;
using com.itextpdf.layout.property;

namespace com.itextpdf.layout.renderer
{
	public abstract class RootRenderer : AbstractRenderer
	{
		protected internal bool immediateFlush = true;

		protected internal LayoutArea currentArea;

		protected internal int currentPageNumber;

		public override void AddChild(IRenderer renderer)
		{
			base.AddChild(renderer);
			if (currentArea == null)
			{
				UpdateCurrentArea(null);
			}
			// Static layout
			if (currentArea != null && !childRenderers.IsEmpty() && childRenderers[childRenderers
				.Count - 1] == renderer)
			{
				IList<IRenderer> resultRenderers = new List<IRenderer>();
				LayoutResult result = null;
				LayoutArea storedArea = null;
				LayoutArea nextStoredArea = null;
				while (currentArea != null && renderer != null && (result = renderer.Layout(new LayoutContext
					(currentArea.Clone()))).GetStatus() != LayoutResult.FULL)
				{
					if (result.GetStatus() == LayoutResult.PARTIAL)
					{
						if (result.GetOverflowRenderer() is ImageRenderer)
						{
							((ImageRenderer)result.GetOverflowRenderer()).AutoScale(currentArea);
						}
						else
						{
							ProcessRenderer(result.GetSplitRenderer(), resultRenderers);
							if (nextStoredArea != null)
							{
								currentArea = nextStoredArea;
								currentPageNumber = nextStoredArea.GetPageNumber();
								nextStoredArea = null;
							}
							else
							{
								UpdateCurrentArea(result);
							}
						}
					}
					else
					{
						if (result.GetStatus() == LayoutResult.NOTHING)
						{
							if (result.GetOverflowRenderer() is ImageRenderer)
							{
								if (currentArea.GetBBox().GetHeight() < ((ImageRenderer)result.GetOverflowRenderer
									()).imageHeight && !currentArea.IsEmptyArea())
								{
									UpdateCurrentArea(result);
								}
								((ImageRenderer)result.GetOverflowRenderer()).AutoScale(currentArea);
							}
							else
							{
								if (currentArea.IsEmptyArea() && !(renderer is AreaBreakRenderer))
								{
									if (bool.ValueOf(true).Equals(result.GetOverflowRenderer().GetModelElement().GetProperty
										(Property.KEEP_TOGETHER)))
									{
										result.GetOverflowRenderer().GetModelElement().SetProperty(Property.KEEP_TOGETHER
											, false);
										Logger logger = LoggerFactory.GetLogger(typeof(RootRenderer));
										logger.Warn(String.Format(LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, "KeepTogether property will be ignored."
											));
										if (storedArea != null)
										{
											nextStoredArea = currentArea;
											currentArea = storedArea;
											currentPageNumber = storedArea.GetPageNumber();
										}
										storedArea = currentArea;
									}
									else
									{
										result.GetOverflowRenderer().SetProperty(Property.FORCED_PLACEMENT, true);
										Logger logger = LoggerFactory.GetLogger(typeof(RootRenderer));
										logger.Warn(String.Format(LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, ""));
									}
									renderer = result.GetOverflowRenderer();
									continue;
								}
								storedArea = currentArea;
								UpdateCurrentArea(result);
							}
						}
					}
					renderer = result.GetOverflowRenderer();
				}
				if (currentArea != null)
				{
					currentArea.GetBBox().SetHeight(currentArea.GetBBox().GetHeight() - result.GetOccupiedArea
						().GetBBox().GetHeight());
					currentArea.SetEmptyArea(false);
					if (renderer != null)
					{
						ProcessRenderer(renderer, resultRenderers);
					}
				}
				childRenderers.RemoveAt(childRenderers.Count - 1);
				if (!immediateFlush)
				{
					childRenderers.AddAll(resultRenderers);
				}
			}
			else
			{
				if (positionedRenderers.Count > 0 && positionedRenderers[positionedRenderers.Count
					 - 1] == renderer)
				{
					int positionedPageNumber = renderer.GetProperty(Property.PAGE_NUMBER);
					if (positionedPageNumber == null)
					{
						positionedPageNumber = currentPageNumber;
					}
					renderer.Layout(new LayoutContext(new LayoutArea(positionedPageNumber, currentArea
						.GetBBox().Clone())));
					if (immediateFlush)
					{
						FlushSingleRenderer(renderer);
						positionedRenderers.RemoveAt(positionedRenderers.Count - 1);
					}
				}
			}
		}

		// Drawing of content. Might need to rename.
		public virtual void Flush()
		{
			foreach (IRenderer resultRenderer in childRenderers)
			{
				FlushSingleRenderer(resultRenderer);
			}
			foreach (IRenderer resultRenderer_1 in positionedRenderers)
			{
				FlushSingleRenderer(resultRenderer_1);
			}
			childRenderers.Clear();
			positionedRenderers.Clear();
		}

		public override LayoutResult Layout(LayoutContext layoutContext)
		{
			throw new InvalidOperationException("Layout is not supported for root renderers."
				);
		}

		public virtual LayoutArea GetCurrentArea()
		{
			if (currentArea == null)
			{
				UpdateCurrentArea(null);
			}
			return currentArea;
		}

		protected internal abstract void FlushSingleRenderer(IRenderer resultRenderer);

		protected internal abstract LayoutArea UpdateCurrentArea(LayoutResult overflowResult
			);

		private void ProcessRenderer(IRenderer renderer, IList<IRenderer> resultRenderers
			)
		{
			AlignChildHorizontally(renderer, currentArea.GetBBox().GetWidth());
			if (immediateFlush)
			{
				FlushSingleRenderer(renderer);
			}
			else
			{
				resultRenderers.Add(renderer);
			}
		}
	}
}
