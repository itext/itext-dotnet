/*
$Id$

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
namespace iTextSharp.Kernel.Pdf
{
	public class PdfViewerPreferences : PdfObjectWrapper<PdfDictionary>
	{
		private const long serialVersionUID = -6885879361985241602L;

		public enum PdfViewerPreferencesConstants
		{
			USE_NONE,
			USE_OUTLINES,
			USE_THUMBS,
			USE_OC,
			LEFT_TO_RIGHT,
			RIGHT_TO_LEFT,
			MEDIA_BOX,
			CROP_BOX,
			BLEED_BOX,
			TRIM_BOX,
			ART_BOX,
			VIEW_AREA,
			VIEW_CLIP,
			PRINT_AREA,
			PRINT_CLIP,
			NONE,
			APP_DEFAULT,
			SIMPLEX,
			DUPLEX_FLIP_SHORT_EDGE,
			DUPLEX_FLIP_LONG_EDGE
		}

		public PdfViewerPreferences()
			: this(new PdfDictionary())
		{
		}

		public PdfViewerPreferences(PdfDictionary pdfObject)
			: base(pdfObject)
		{
		}

		/// <summary>This method sets HideToolBar flag to true or false</summary>
		/// <param name="hideToolbar"/>
		/// <returns/>
		public virtual iTextSharp.Kernel.Pdf.PdfViewerPreferences SetHideToolbar(bool hideToolbar
			)
		{
			return Put(PdfName.HideToolbar, new PdfBoolean(hideToolbar));
		}

		/// <summary>This method sets HideMenuBar flag to true or false</summary>
		/// <param name="hideMenubar"/>
		/// <returns/>
		public virtual iTextSharp.Kernel.Pdf.PdfViewerPreferences SetHideMenubar(bool hideMenubar
			)
		{
			return Put(PdfName.HideMenubar, new PdfBoolean(hideMenubar));
		}

		/// <summary>This method sets HideWindowUI flag to true or false</summary>
		/// <param name="hideWindowUI"/>
		/// <returns/>
		public virtual iTextSharp.Kernel.Pdf.PdfViewerPreferences SetHideWindowUI(bool hideWindowUI
			)
		{
			return Put(PdfName.HideWindowUI, new PdfBoolean(hideWindowUI));
		}

		/// <summary>This method sets FitWindow flag to true or false</summary>
		/// <param name="fitWindow"/>
		/// <returns/>
		public virtual iTextSharp.Kernel.Pdf.PdfViewerPreferences SetFitWindow(bool fitWindow
			)
		{
			return Put(PdfName.FitWindow, new PdfBoolean(fitWindow));
		}

		/// <summary>This method sets CenterWindow flag to true or false</summary>
		/// <param name="centerWindow"/>
		/// <returns/>
		public virtual iTextSharp.Kernel.Pdf.PdfViewerPreferences SetCenterWindow(bool centerWindow
			)
		{
			return Put(PdfName.CenterWindow, new PdfBoolean(centerWindow));
		}

		/// <summary>This method sets DisplayDocTitle flag to true or false</summary>
		/// <param name="displayDocTitle"/>
		/// <returns/>
		public virtual iTextSharp.Kernel.Pdf.PdfViewerPreferences SetDisplayDocTitle(bool
			 displayDocTitle)
		{
			return Put(PdfName.DisplayDocTitle, new PdfBoolean(displayDocTitle));
		}

		/// <summary>This method sets NonFullScreenPageMode property.</summary>
		/// <remarks>
		/// This method sets NonFullScreenPageMode property. Allowed values are UseNone, UseOutlines, useThumbs, UseOC.
		/// This entry is meaningful only if the value of the PageMode entry in the Catalog dictionary is FullScreen
		/// </remarks>
		/// <param name="nonFullScreenPageMode"/>
		/// <returns/>
		public virtual iTextSharp.Kernel.Pdf.PdfViewerPreferences SetNonFullScreenPageMode
			(PdfViewerPreferences.PdfViewerPreferencesConstants nonFullScreenPageMode)
		{
			switch (nonFullScreenPageMode)
			{
				case PdfViewerPreferences.PdfViewerPreferencesConstants.USE_NONE:
				{
					Put(PdfName.NonFullScreenPageMode, PdfName.UseNone);
					break;
				}

				case PdfViewerPreferences.PdfViewerPreferencesConstants.USE_OUTLINES:
				{
					Put(PdfName.NonFullScreenPageMode, PdfName.UseOutlines);
					break;
				}

				case PdfViewerPreferences.PdfViewerPreferencesConstants.USE_THUMBS:
				{
					Put(PdfName.NonFullScreenPageMode, PdfName.UseThumbs);
					break;
				}

				case PdfViewerPreferences.PdfViewerPreferencesConstants.USE_OC:
				{
					Put(PdfName.NonFullScreenPageMode, PdfName.UseOC);
					break;
				}

				default:
				{
					break;
				}
			}
			return this;
		}

		/// <summary>This method sets predominant reading order of text.</summary>
		/// <param name="direction"/>
		/// <returns/>
		public virtual iTextSharp.Kernel.Pdf.PdfViewerPreferences SetDirection(PdfViewerPreferences.PdfViewerPreferencesConstants
			 direction)
		{
			switch (direction)
			{
				case PdfViewerPreferences.PdfViewerPreferencesConstants.LEFT_TO_RIGHT:
				{
					Put(PdfName.Direction, PdfName.L2R);
					break;
				}

				case PdfViewerPreferences.PdfViewerPreferencesConstants.RIGHT_TO_LEFT:
				{
					Put(PdfName.Direction, PdfName.R2L);
					break;
				}

				default:
				{
					break;
				}
			}
			return this;
		}

		/// <summary>
		/// This method sets the name of the page boundary representing the area of a page that shall be displayed when
		/// viewing the document on the screen.
		/// </summary>
		/// <param name="pageBoundary"/>
		/// <returns/>
		public virtual iTextSharp.Kernel.Pdf.PdfViewerPreferences SetViewArea(PdfViewerPreferences.PdfViewerPreferencesConstants
			 pageBoundary)
		{
			return SetPageBoundary(PdfViewerPreferences.PdfViewerPreferencesConstants.VIEW_AREA
				, pageBoundary);
		}

		/// <summary>
		/// This method sets the name of the page boundary to which the contents of a page shall be clipped when
		/// viewing the document on the screen.
		/// </summary>
		/// <param name="pageBoundary"/>
		/// <returns/>
		public virtual iTextSharp.Kernel.Pdf.PdfViewerPreferences SetViewClip(PdfViewerPreferences.PdfViewerPreferencesConstants
			 pageBoundary)
		{
			return SetPageBoundary(PdfViewerPreferences.PdfViewerPreferencesConstants.VIEW_CLIP
				, pageBoundary);
		}

		/// <summary>
		/// This method sets the name of the page boundary representing the area of a page that shall be
		/// rendered when printing the document.
		/// </summary>
		/// <param name="pageBoundary"/>
		/// <returns/>
		public virtual iTextSharp.Kernel.Pdf.PdfViewerPreferences SetPrintArea(PdfViewerPreferences.PdfViewerPreferencesConstants
			 pageBoundary)
		{
			return SetPageBoundary(PdfViewerPreferences.PdfViewerPreferencesConstants.PRINT_AREA
				, pageBoundary);
		}

		/// <summary>
		/// This method sets the name of the page boundary to which the contents of a page shall be clipped when
		/// printing the document.
		/// </summary>
		/// <param name="pageBoundary"/>
		/// <returns/>
		public virtual iTextSharp.Kernel.Pdf.PdfViewerPreferences SetPrintClip(PdfViewerPreferences.PdfViewerPreferencesConstants
			 pageBoundary)
		{
			return SetPageBoundary(PdfViewerPreferences.PdfViewerPreferencesConstants.PRINT_CLIP
				, pageBoundary);
		}

		/// <summary>
		/// This method sets the page scaling option that shall be selected when a print dialog is displayed for this
		/// document.
		/// </summary>
		/// <remarks>
		/// This method sets the page scaling option that shall be selected when a print dialog is displayed for this
		/// document. Valid values are None and AppDefault.
		/// </remarks>
		/// <param name="printScaling"/>
		/// <returns/>
		public virtual iTextSharp.Kernel.Pdf.PdfViewerPreferences SetPrintScaling(PdfViewerPreferences.PdfViewerPreferencesConstants
			 printScaling)
		{
			switch (printScaling)
			{
				case PdfViewerPreferences.PdfViewerPreferencesConstants.NONE:
				{
					Put(PdfName.PrintScaling, PdfName.None);
					break;
				}

				case PdfViewerPreferences.PdfViewerPreferencesConstants.APP_DEFAULT:
				{
					Put(PdfName.PrintScaling, PdfName.AppDefault);
					break;
				}

				default:
				{
					break;
				}
			}
			return this;
		}

		/// <summary>This method sets the paper handling option that shall be used when printing the file from the print dialog.
		/// 	</summary>
		/// <remarks>
		/// This method sets the paper handling option that shall be used when printing the file from the print dialog.
		/// The following values are valid: Simplex, DuplexFlipShortEdge, DuplexFlipLongEdge.
		/// </remarks>
		/// <param name="duplex"/>
		/// <returns/>
		public virtual iTextSharp.Kernel.Pdf.PdfViewerPreferences SetDuplex(PdfViewerPreferences.PdfViewerPreferencesConstants
			 duplex)
		{
			switch (duplex)
			{
				case PdfViewerPreferences.PdfViewerPreferencesConstants.SIMPLEX:
				{
					Put(PdfName.Duplex, PdfName.Simplex);
					break;
				}

				case PdfViewerPreferences.PdfViewerPreferencesConstants.DUPLEX_FLIP_SHORT_EDGE:
				{
					Put(PdfName.Duplex, PdfName.DuplexFlipShortEdge);
					break;
				}

				case PdfViewerPreferences.PdfViewerPreferencesConstants.DUPLEX_FLIP_LONG_EDGE:
				{
					Put(PdfName.Duplex, PdfName.DuplexFlipLongEdge);
					break;
				}

				default:
				{
					break;
				}
			}
			return this;
		}

		/// <summary>This method sets PickTrayByPDFSize flag to true or false.</summary>
		/// <param name="pickTrayByPdfSize"/>
		/// <returns/>
		public virtual iTextSharp.Kernel.Pdf.PdfViewerPreferences SetPickTrayByPDFSize(bool
			 pickTrayByPdfSize)
		{
			return Put(PdfName.PickTrayByPDFSize, new PdfBoolean(pickTrayByPdfSize));
		}

		/// <summary>This method sets the page numbers used to initialize the print dialog box when the file is printed.
		/// 	</summary>
		/// <param name="printPageRange"/>
		/// <returns/>
		public virtual iTextSharp.Kernel.Pdf.PdfViewerPreferences SetPrintPageRange(int[]
			 printPageRange)
		{
			return Put(PdfName.PrintPageRange, new PdfArray(printPageRange));
		}

		/// <summary>This method sets the number of copies that shall be printed when the print dialog is opened for this file.
		/// 	</summary>
		/// <param name="numCopies"/>
		/// <returns/>
		public virtual iTextSharp.Kernel.Pdf.PdfViewerPreferences SetNumCopies(int numCopies
			)
		{
			return Put(PdfName.NumCopies, new PdfNumber(numCopies));
		}

		public virtual iTextSharp.Kernel.Pdf.PdfViewerPreferences Put(PdfName key, PdfObject
			 value)
		{
			GetPdfObject().Put(key, value);
			return this;
		}

		protected internal override bool IsWrappedObjectMustBeIndirect()
		{
			return false;
		}

		private iTextSharp.Kernel.Pdf.PdfViewerPreferences SetPageBoundary(PdfViewerPreferences.PdfViewerPreferencesConstants
			 viewerPreferenceType, PdfViewerPreferences.PdfViewerPreferencesConstants pageBoundary
			)
		{
			PdfName type = null;
			switch (viewerPreferenceType)
			{
				case PdfViewerPreferences.PdfViewerPreferencesConstants.VIEW_AREA:
				{
					type = PdfName.ViewArea;
					break;
				}

				case PdfViewerPreferences.PdfViewerPreferencesConstants.VIEW_CLIP:
				{
					type = PdfName.ViewArea;
					break;
				}

				case PdfViewerPreferences.PdfViewerPreferencesConstants.PRINT_AREA:
				{
					type = PdfName.ViewArea;
					break;
				}

				case PdfViewerPreferences.PdfViewerPreferencesConstants.PRINT_CLIP:
				{
					type = PdfName.ViewArea;
					break;
				}

				default:
				{
					break;
				}
			}
			if (type != null)
			{
				switch (pageBoundary)
				{
					case PdfViewerPreferences.PdfViewerPreferencesConstants.MEDIA_BOX:
					{
						Put(type, PdfName.MediaBox);
						break;
					}

					case PdfViewerPreferences.PdfViewerPreferencesConstants.CROP_BOX:
					{
						Put(type, PdfName.CropBox);
						break;
					}

					case PdfViewerPreferences.PdfViewerPreferencesConstants.BLEED_BOX:
					{
						Put(type, PdfName.BleedBox);
						break;
					}

					case PdfViewerPreferences.PdfViewerPreferencesConstants.TRIM_BOX:
					{
						Put(type, PdfName.TrimBox);
						break;
					}

					case PdfViewerPreferences.PdfViewerPreferencesConstants.ART_BOX:
					{
						Put(type, PdfName.ArtBox);
						break;
					}

					default:
					{
						break;
					}
				}
			}
			return this;
		}
	}
}
