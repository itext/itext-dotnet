/*

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
using iTextSharp.IO.Color;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Canvas;
using iTextSharp.Kernel.Pdf.Colorspace;

namespace iTextSharp.Pdfa.Checker
{
    public abstract class PdfAChecker
    {
        public const String ICC_COLOR_SPACE_RGB = "RGB ";

        public const String ICC_COLOR_SPACE_CMYK = "CMYK";

        public const String ICC_COLOR_SPACE_GRAY = "GRAY";

        public const String ICC_DEVICE_CLASS_OUTPUT_PROFILE = "prtr";

        public const String ICC_DEVICE_CLASS_MONITOR_PROFILE = "mntr";

        public const int maxGsStackDepth = 28;

        protected internal PdfAConformanceLevel conformanceLevel;

        protected internal String pdfAOutputIntentColorSpace;

        protected internal int gsStackDepth = 0;

        protected internal bool rgbIsUsed = false;

        protected internal bool cmykIsUsed = false;

        protected internal bool grayIsUsed = false;

        /// <summary>Contains some objects that are already checked.</summary>
        /// <remarks>
        /// Contains some objects that are already checked.
        /// NOTE: Not all objects that were checked are stored in that set. This set is used for avoiding double checks for
        /// actions, xObjects and page objects; and for letting those objects to be manually flushed.
        /// Use this mechanism carefully: objects that are able to be changed (or at least if object's properties
        /// that shall be checked are able to be changed) shouldn't be marked as checked if they are not to be
        /// flushed immediately.
        /// </remarks>
        protected internal ICollection<PdfObject> checkedObjects = new HashSet<PdfObject>
            ();

        protected internal IDictionary<PdfObject, PdfColorSpace> checkedObjectsColorspace
             = new Dictionary<PdfObject, PdfColorSpace>();

        protected internal PdfAChecker(PdfAConformanceLevel conformanceLevel)
        {
            this.conformanceLevel = conformanceLevel;
        }

        public virtual void CheckDocument(PdfCatalog catalog)
        {
            PdfDictionary catalogDict = catalog.GetPdfObject();
            SetPdfAOutputIntentColorSpace(catalogDict);
            CheckOutputIntents(catalogDict);
            CheckMetaData(catalogDict);
            CheckCatalogValidEntries(catalogDict);
            CheckTrailer(catalog.GetDocument().GetTrailer());
            CheckLogicalStructure(catalogDict);
            CheckForm(catalogDict.GetAsDictionary(PdfName.AcroForm));
            CheckOutlines(catalogDict);
            CheckPages(catalog.GetDocument());
            CheckOpenAction(catalogDict.Get(PdfName.OpenAction));
            CheckColorsUsages();
        }

        public virtual void CheckSinglePage(PdfPage page)
        {
            CheckPage(page);
        }

        public virtual void CheckPdfObject(PdfObject obj)
        {
            switch (obj.GetObjectType())
            {
                case PdfObject.NUMBER:
                {
                    CheckPdfNumber((PdfNumber)obj);
                    break;
                }

                case PdfObject.STREAM:
                {
                    PdfStream stream = (PdfStream)obj;
                    //form xObjects, annotation appearance streams, patterns and type3 glyphs may have their own resources dictionary
                    CheckResources(stream.GetAsDictionary(PdfName.Resources));
                    CheckPdfStream(stream);
                    break;
                }

                case PdfObject.STRING:
                {
                    CheckPdfString((PdfString)obj);
                    break;
                }

                case PdfObject.DICTIONARY:
                {
                    PdfDictionary dict = (PdfDictionary)obj;
                    PdfName type = dict.GetAsName(PdfName.Type);
                    if (PdfName.Filespec.Equals(type))
                    {
                        CheckFileSpec(dict);
                    }
                    break;
                }
            }
        }

        public virtual PdfAConformanceLevel GetConformanceLevel()
        {
            return conformanceLevel;
        }

        public virtual bool ObjectIsChecked(PdfObject @object)
        {
            return checkedObjects.Contains(@object);
        }

        public abstract void CheckCanvasStack(char stackOperation);

        public abstract void CheckInlineImage(PdfStream inlineImage, PdfDictionary currentColorSpaces
            );

        public abstract void CheckColor(iTextSharp.Kernel.Color.Color color, PdfDictionary
             currentColorSpaces, bool? fill);

        public abstract void CheckColorSpace(PdfColorSpace colorSpace, PdfDictionary currentColorSpaces
            , bool checkAlternate, bool? fill);

        public abstract void CheckRenderingIntent(PdfName intent);

        public abstract void CheckExtGState(CanvasGraphicsState extGState);

        protected internal abstract ICollection<PdfName> GetForbiddenActions();

        protected internal abstract ICollection<PdfName> GetAllowedNamedActions();

        protected internal abstract void CheckAction(PdfDictionary action);

        protected internal abstract void CheckAnnotation(PdfDictionary annotDic);

        protected internal abstract void CheckCatalogValidEntries(PdfDictionary catalogDict
            );

        protected internal abstract void CheckColorsUsages();

        protected internal abstract void CheckImage(PdfStream image, PdfDictionary currentColorSpaces
            );

        protected internal abstract void CheckFileSpec(PdfDictionary fileSpec);

        protected internal abstract void CheckForm(PdfDictionary form);

        protected internal abstract void CheckFormXObject(PdfStream form);

        protected internal abstract void CheckLogicalStructure(PdfDictionary catalog);

        protected internal abstract void CheckMetaData(PdfDictionary catalog);

        protected internal abstract void CheckOutputIntents(PdfDictionary catalog);

        protected internal abstract void CheckPageObject(PdfDictionary page, PdfDictionary
             pageResources);

        protected internal abstract void CheckPageSize(PdfDictionary page);

        protected internal abstract void CheckPdfNumber(PdfNumber number);

        protected internal abstract void CheckPdfStream(PdfStream stream);

        protected internal abstract void CheckPdfString(PdfString @string);

        protected internal abstract void CheckTrailer(PdfDictionary trailer);

        protected internal virtual void CheckResources(PdfDictionary resources)
        {
            if (resources == null)
            {
                return;
            }
            PdfDictionary xObjects = resources.GetAsDictionary(PdfName.XObject);
            PdfDictionary shadings = resources.GetAsDictionary(PdfName.Shading);
            if (xObjects != null)
            {
                foreach (PdfObject xObject in xObjects.Values())
                {
                    PdfStream xObjStream = (PdfStream)xObject;
                    PdfObject subtype = xObjStream.Get(PdfName.Subtype);
                    if (PdfName.Image.Equals(subtype))
                    {
                        CheckImage(xObjStream, resources.GetAsDictionary(PdfName.ColorSpace));
                    }
                    else
                    {
                        if (PdfName.Form.Equals(subtype))
                        {
                            CheckFormXObject(xObjStream);
                        }
                    }
                }
            }
            if (shadings != null)
            {
                foreach (PdfObject shading in shadings.Values())
                {
                    PdfDictionary shadingDict = (PdfDictionary)shading;
                    CheckColorSpace(PdfColorSpace.MakeColorSpace(shadingDict.Get(PdfName.ColorSpace))
                        , resources.GetAsDictionary(PdfName.ColorSpace), true, null);
                }
            }
        }

        protected internal static bool CheckFlag(int flags, int flag)
        {
            return (flags & flag) != 0;
        }

        protected internal static bool CheckStructure(PdfAConformanceLevel conformanceLevel
            )
        {
            return conformanceLevel == PdfAConformanceLevel.PDF_A_1A || conformanceLevel == PdfAConformanceLevel
                .PDF_A_2A || conformanceLevel == PdfAConformanceLevel.PDF_A_3A;
        }

        protected internal virtual bool IsAlreadyChecked(PdfDictionary dictionary)
        {
            if (checkedObjects.Contains(dictionary))
            {
                return true;
            }
            checkedObjects.Add(dictionary);
            return false;
        }

        private void CheckPages(PdfDocument document)
        {
            for (int i = 1; i <= document.GetNumberOfPages(); i++)
            {
                CheckPage(document.GetPage(i));
            }
        }

        private void CheckPage(PdfPage page)
        {
            PdfDictionary pageDict = page.GetPdfObject();
            if (IsAlreadyChecked(pageDict))
            {
                return;
            }
            CheckPageObject(pageDict, page.GetResources().GetPdfObject());
            PdfDictionary pageResources = page.GetResources().GetPdfObject();
            CheckResources(pageResources);
            CheckAnnotations(pageDict);
            CheckPageSize(pageDict);
            int contentStreamCount = page.GetContentStreamCount();
            for (int j = 0; j < contentStreamCount; ++j)
            {
                checkedObjects.Add(page.GetContentStream(j));
            }
        }

        private void CheckOpenAction(PdfObject openAction)
        {
            if (openAction == null)
            {
                return;
            }
            if (openAction.IsDictionary())
            {
                CheckAction((PdfDictionary)openAction);
            }
            else
            {
                if (openAction.IsArray())
                {
                    PdfArray actions = (PdfArray)openAction;
                    foreach (PdfObject action in actions)
                    {
                        CheckAction((PdfDictionary)action);
                    }
                }
            }
        }

        private void CheckAnnotations(PdfDictionary page)
        {
            PdfArray annots = page.GetAsArray(PdfName.Annots);
            if (annots != null)
            {
                // explicit iteration to resolve indirect references on get().
                // TODO DEVSIX-591
                for (int i = 0; i < annots.Size(); i++)
                {
                    PdfDictionary annot = annots.GetAsDictionary(i);
                    CheckAnnotation(annot);
                    PdfDictionary action = annot.GetAsDictionary(PdfName.A);
                    if (action != null)
                    {
                        CheckAction(action);
                    }
                }
            }
        }

        private void CheckOutlines(PdfDictionary catalogDict)
        {
            PdfDictionary outlines = catalogDict.GetAsDictionary(PdfName.Outlines);
            if (outlines != null)
            {
                foreach (PdfDictionary outline in GetOutlines(outlines))
                {
                    PdfDictionary action = outline.GetAsDictionary(PdfName.A);
                    if (action != null)
                    {
                        CheckAction(action);
                    }
                }
            }
        }

        private IList<PdfDictionary> GetOutlines(PdfDictionary item)
        {
            IList<PdfDictionary> outlines = new List<PdfDictionary>();
            outlines.Add(item);
            PdfDictionary processItem = item.GetAsDictionary(PdfName.First);
            if (processItem != null)
            {
                outlines.AddAll(GetOutlines(processItem));
            }
            processItem = item.GetAsDictionary(PdfName.Next);
            if (processItem != null)
            {
                outlines.AddAll(GetOutlines(processItem));
            }
            return outlines;
        }

        private void SetPdfAOutputIntentColorSpace(PdfDictionary catalog)
        {
            PdfArray outputIntents = catalog.GetAsArray(PdfName.OutputIntents);
            if (outputIntents == null)
            {
                return;
            }
            PdfDictionary pdfAOutputIntent = GetPdfAOutputIntent(outputIntents);
            SetCheckerOutputIntent(pdfAOutputIntent);
        }

        private PdfDictionary GetPdfAOutputIntent(PdfArray outputIntents)
        {
            for (int i = 0; i < outputIntents.Size(); ++i)
            {
                PdfName outputIntentSubtype = outputIntents.GetAsDictionary(i).GetAsName(PdfName.
                    S);
                if (PdfName.GTS_PDFA1.Equals(outputIntentSubtype))
                {
                    return outputIntents.GetAsDictionary(i);
                }
            }
            return null;
        }

        private void SetCheckerOutputIntent(PdfDictionary outputIntent)
        {
            if (outputIntent != null)
            {
                PdfStream destOutputProfile = outputIntent.GetAsStream(PdfName.DestOutputProfile);
                if (destOutputProfile != null)
                {
                    String intentCS = IccProfile.GetIccColorSpaceName(destOutputProfile.GetBytes());
                    this.pdfAOutputIntentColorSpace = intentCS;
                }
            }
        }
    }
}
