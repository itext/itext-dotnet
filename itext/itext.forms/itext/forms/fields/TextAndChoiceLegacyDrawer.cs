/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Forms.Logs;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace iText.Forms.Fields {
//\cond DO_NOT_DOCUMENT
    internal sealed class TextAndChoiceLegacyDrawer {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Forms.Fields.TextAndChoiceLegacyDrawer
            ));

        private TextAndChoiceLegacyDrawer() {
        }

//\cond DO_NOT_DOCUMENT
        //Empty constructor.
        internal static bool RegenerateTextAndChoiceField(PdfFormAnnotation formAnnotation) {
            String value = formAnnotation.parent.GetDisplayValue();
            PdfName type = formAnnotation.parent.GetFormType();
            PdfPage page = PdfAnnotation.MakeAnnotation(formAnnotation.GetPdfObject()).GetPage();
            PdfArray bBox = formAnnotation.GetPdfObject().GetAsArray(PdfName.Rect);
            //Apply Page rotation
            int pageRotation = 0;
            if (page != null) {
                pageRotation = page.GetRotation();
                //Clockwise, so negative
                pageRotation *= -1;
            }
            PdfArray matrix;
            if (pageRotation % 90 == 0) {
                //Cast angle to [-360, 360]
                double angle = pageRotation % 360;
                //Get angle in radians
                angle = DegreeToRadians(angle);
                Rectangle initialBboxRectangle = bBox.ToRectangle();
                //rotate the bounding box
                Rectangle rect = initialBboxRectangle.Clone();
                //Calculate origin offset
                double translationWidth = 0;
                double translationHeight = 0;
                if (angle >= -1 * Math.PI && angle <= -1 * Math.PI / 2) {
                    translationWidth = rect.GetWidth();
                }
                if (angle <= -1 * Math.PI) {
                    translationHeight = rect.GetHeight();
                }
                //Store rotation and translation in the matrix
                matrix = new PdfArray(new double[] { Math.Cos(angle), -Math.Sin(angle), Math.Sin(angle), Math.Cos(angle), 
                    translationWidth, translationHeight });
                // If the angle is a multiple of 90 and not a multiple of 180, height and width of the bounding box
                // need to be switched
                if (angle % (Math.PI / 2) == 0 && angle % (Math.PI) != 0) {
                    rect.SetWidth(initialBboxRectangle.GetHeight());
                    rect.SetHeight(initialBboxRectangle.GetWidth());
                }
                // Adapt origin
                rect.SetX(rect.GetX() + (float)translationWidth);
                rect.SetY(rect.GetY() + (float)translationHeight);
                //Copy Bounding box
                bBox = new PdfArray(rect);
            }
            else {
                //Avoid NPE when handling corrupt pdfs
                LOGGER.LogError(FormsLogMessageConstants.INCORRECT_PAGE_ROTATION);
                matrix = new PdfArray(new double[] { 1, 0, 0, 1, 0, 0 });
            }
            //Apply field rotation
            float fieldRotation = 0;
            if (formAnnotation.GetPdfObject().GetAsDictionary(PdfName.MK) != null && formAnnotation.GetPdfObject().GetAsDictionary
                (PdfName.MK).Get(PdfName.R) != null) {
                fieldRotation = (float)formAnnotation.GetPdfObject().GetAsDictionary(PdfName.MK).GetAsFloat(PdfName.R);
                //Get relative field rotation
                fieldRotation += pageRotation;
            }
            if (fieldRotation % 90 == 0) {
                Rectangle initialBboxRectangle = bBox.ToRectangle();
                //Cast angle to [-360, 360]
                double angle = fieldRotation % 360;
                //Get angle in radians
                angle = DegreeToRadians(angle);
                //Calculate origin offset
                double translationWidth = CalculateTranslationWidthAfterFieldRot(initialBboxRectangle, DegreeToRadians(pageRotation
                    ), angle);
                double translationHeight = CalculateTranslationHeightAfterFieldRot(initialBboxRectangle, DegreeToRadians(pageRotation
                    ), angle);
                //Concatenate rotation and translation into the matrix
                Matrix currentMatrix = new Matrix(matrix.GetAsNumber(0).FloatValue(), matrix.GetAsNumber(1).FloatValue(), 
                    matrix.GetAsNumber(2).FloatValue(), matrix.GetAsNumber(3).FloatValue(), matrix.GetAsNumber(4).FloatValue
                    (), matrix.GetAsNumber(5).FloatValue());
                Matrix toConcatenate = new Matrix((float)Math.Cos(angle), (float)(-Math.Sin(angle)), (float)(Math.Sin(angle
                    )), (float)(Math.Cos(angle)), (float)translationWidth, (float)translationHeight);
                currentMatrix = currentMatrix.Multiply(toConcatenate);
                matrix = new PdfArray(new float[] { currentMatrix.Get(0), currentMatrix.Get(1), currentMatrix.Get(3), currentMatrix
                    .Get(4), currentMatrix.Get(6), currentMatrix.Get(7) });
                // Construct bounding box
                Rectangle rect = initialBboxRectangle.Clone();
                // If the angle is a multiple of 90 and not a multiple of 180, height and width of the bounding box
                // need to be switched
                if (angle % (Math.PI / 2) == 0 && angle % (Math.PI) != 0) {
                    rect.SetWidth(initialBboxRectangle.GetHeight());
                    rect.SetHeight(initialBboxRectangle.GetWidth());
                }
                rect.SetX(rect.GetX() + (float)translationWidth);
                rect.SetY(rect.GetY() + (float)translationHeight);
                // Copy Bounding box
                bBox = new PdfArray(rect);
            }
            // Create appearance
            Rectangle bboxRectangle = bBox.ToRectangle();
            PdfFormXObject appearance = new PdfFormXObject(new Rectangle(0, 0, bboxRectangle.GetWidth(), bboxRectangle
                .GetHeight()));
            appearance.Put(PdfName.Matrix, matrix);
            //Create text appearance
            if (PdfName.Tx.Equals(type)) {
                DrawCombTextAppearance(formAnnotation, bboxRectangle, formAnnotation.GetFont(), formAnnotation.GetFontSize
                    (bBox, value), value, appearance);
            }
            else {
                int topIndex = 0;
                if (!formAnnotation.parent.GetFieldFlag(PdfChoiceFormField.FF_COMBO)) {
                    PdfNumber topIndexNum = formAnnotation.GetParent().GetAsNumber(PdfName.TI);
                    PdfArray options = formAnnotation.parent.GetOptions();
                    if (null != options) {
                        topIndex = null != topIndexNum ? topIndexNum.IntValue() : 0;
                        PdfArray visibleOptions = topIndex > 0 ? new PdfArray(options.SubList(topIndex, options.Size())) : (PdfArray
                            )options.Clone();
                        value = PdfFormField.OptionsArrayToString(visibleOptions);
                    }
                }
                DrawChoiceAppearance(formAnnotation, bboxRectangle, formAnnotation.GetFontSize(bBox, value), value, appearance
                    , topIndex);
            }
            PdfDictionary ap = new PdfDictionary();
            ap.Put(PdfName.N, appearance.GetPdfObject());
            ap.SetModified();
            formAnnotation.Put(PdfName.AP, ap);
            return true;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static void DrawChoiceAppearance(PdfFormAnnotation formAnnotation, Rectangle rect, float fontSize
            , String value, PdfFormXObject appearance, int topIndex) {
            PdfStream stream = (PdfStream)new PdfStream().MakeIndirect(formAnnotation.GetDocument());
            PdfResources resources = appearance.GetResources();
            PdfCanvas canvas = new PdfCanvas(stream, resources, formAnnotation.GetDocument());
            float width = rect.GetWidth();
            float height = rect.GetHeight();
            float widthBorder = 6.0f;
            float heightBorder = 2.0f;
            formAnnotation.DrawBorder(canvas, appearance, width, height);
            canvas.BeginVariableText().SaveState().Rectangle(3, 3, width - widthBorder, height - heightBorder).Clip().
                EndPath();
            iText.Layout.Canvas modelCanvas = new iText.Layout.Canvas(canvas, new Rectangle(3, 0, Math.Max(0, width - 
                widthBorder), Math.Max(0, height - heightBorder)));
            modelCanvas.SetProperty(Property.APPEARANCE_STREAM_LAYOUT, true);
            PdfFormAnnotation.SetMetaInfoToCanvas(modelCanvas);
            Div div = new Div();
            if (formAnnotation.parent.GetFieldFlag(PdfChoiceFormField.FF_COMBO)) {
                div.SetVerticalAlignment(VerticalAlignment.MIDDLE);
            }
            div.SetHeight(Math.Max(0, height - heightBorder));
            IList<String> strings = formAnnotation.GetFont().SplitString(value, fontSize, width - widthBorder);
            for (int index = 0; index < strings.Count; index++) {
                bool? isFull = modelCanvas.GetRenderer().GetPropertyAsBoolean(Property.FULL);
                if (true.Equals(isFull)) {
                    break;
                }
                Paragraph paragraph = new Paragraph(strings[index]).SetFont(formAnnotation.GetFont()).SetFontSize(fontSize
                    ).SetMargins(0, 0, 0, 0).SetMultipliedLeading(1);
                paragraph.SetProperty(Property.FORCED_PLACEMENT, true);
                paragraph.SetTextAlignment(formAnnotation.parent.GetJustification());
                if (formAnnotation.GetColor() != null) {
                    paragraph.SetFontColor(formAnnotation.GetColor());
                }
                if (!formAnnotation.parent.GetFieldFlag(PdfChoiceFormField.FF_COMBO)) {
                    PdfArray indices = formAnnotation.GetParent().GetAsArray(PdfName.I);
                    if (indices != null && indices.Size() > 0) {
                        foreach (PdfObject ind in indices) {
                            if (!ind.IsNumber()) {
                                continue;
                            }
                            if (((PdfNumber)ind).GetValue() == index + topIndex) {
                                paragraph.SetBackgroundColor(new DeviceRgb(10, 36, 106));
                                paragraph.SetFontColor(ColorConstants.LIGHT_GRAY);
                            }
                        }
                    }
                }
                div.Add(paragraph);
            }
            modelCanvas.Add(div);
            canvas.RestoreState().EndVariableText();
            appearance.GetPdfObject().SetData(stream.GetBytes());
        }
//\endcond

        private static void DrawCombTextAppearance(PdfFormAnnotation formAnnotation, Rectangle rect, PdfFont font, 
            float fontSize, String value, PdfFormXObject appearance) {
            PdfStream stream = (PdfStream)new PdfStream().MakeIndirect(formAnnotation.GetDocument());
            PdfResources resources = appearance.GetResources();
            PdfCanvas canvas = new PdfCanvas(stream, resources, formAnnotation.GetDocument());
            float height = rect.GetHeight();
            float width = rect.GetWidth();
            PdfFormXObject xObject = new PdfFormXObject(new Rectangle(0, 0, width, height));
            formAnnotation.DrawBorder(canvas, xObject, width, height);
            if (formAnnotation.parent.IsPassword()) {
                value = ObfuscatePassword(value);
            }
            canvas.BeginVariableText().SaveState().EndPath();
            iText.Layout.Canvas modelCanvas = new iText.Layout.Canvas(canvas, new Rectangle(0, -height, 0, 2 * height)
                );
            modelCanvas.SetProperty(Property.APPEARANCE_STREAM_LAYOUT, true);
            PdfFormAnnotation.SetMetaInfoToCanvas(modelCanvas);
            Style paragraphStyle = new Style().SetFont(font).SetFontSize(fontSize);
            paragraphStyle.SetProperty(Property.LEADING, new Leading(Leading.MULTIPLIED, 1));
            if (formAnnotation.GetColor() != null) {
                paragraphStyle.SetProperty(Property.FONT_COLOR, new TransparentColor(formAnnotation.GetColor()));
            }
            int maxLen = PdfFormCreator.CreateTextFormField(formAnnotation.parent.GetPdfObject()).GetMaxLen();
            // check if /Comb has been set
            float widthPerCharacter = width / maxLen;
            int numberOfCharacters = Math.Min(maxLen, value.Length);
            int start;
            TextAlignment? textAlignment = formAnnotation.parent.GetJustification() == null ? TextAlignment.LEFT : formAnnotation
                .parent.GetJustification();
            switch (textAlignment) {
                case TextAlignment.RIGHT: {
                    start = (maxLen - numberOfCharacters);
                    break;
                }

                case TextAlignment.CENTER: {
                    start = (maxLen - numberOfCharacters) / 2;
                    break;
                }

                default: {
                    start = 0;
                    break;
                }
            }
            float startOffset = widthPerCharacter * (start + 0.5f);
            for (int i = 0; i < numberOfCharacters; i++) {
                modelCanvas.ShowTextAligned(new Paragraph(value.JSubstring(i, i + 1)).AddStyle(paragraphStyle), startOffset
                     + widthPerCharacter * i, rect.GetHeight() / 2, TextAlignment.CENTER, VerticalAlignment.MIDDLE);
            }
            canvas.RestoreState().EndVariableText();
            appearance.GetPdfObject().SetData(stream.GetBytes());
        }

        private static String ObfuscatePassword(String text) {
            char[] pchar = new char[text.Length];
            for (int i = 0; i < text.Length; i++) {
                pchar[i] = '*';
            }
            return new String(pchar);
        }

        private static float CalculateTranslationHeightAfterFieldRot(Rectangle bBox, double pageRotation, double relFieldRotation
            ) {
            if (relFieldRotation == 0) {
                return 0.0f;
            }
            if (pageRotation == 0) {
                if (relFieldRotation == Math.PI / 2) {
                    return bBox.GetHeight();
                }
                if (relFieldRotation == Math.PI) {
                    return bBox.GetHeight();
                }
            }
            if (pageRotation == -Math.PI / 2) {
                if (relFieldRotation == -Math.PI / 2) {
                    return bBox.GetWidth() - bBox.GetHeight();
                }
                if (relFieldRotation == Math.PI / 2) {
                    return bBox.GetHeight();
                }
                if (relFieldRotation == Math.PI) {
                    return bBox.GetWidth();
                }
            }
            if (pageRotation == -Math.PI) {
                if (relFieldRotation == -1 * Math.PI) {
                    return bBox.GetHeight();
                }
                if (relFieldRotation == -1 * Math.PI / 2) {
                    return bBox.GetHeight() - bBox.GetWidth();
                }
                if (relFieldRotation == Math.PI / 2) {
                    return bBox.GetWidth();
                }
            }
            if (pageRotation == -3 * Math.PI / 2) {
                if (relFieldRotation == -3 * Math.PI / 2) {
                    return bBox.GetWidth();
                }
                if (relFieldRotation == -Math.PI) {
                    return bBox.GetWidth();
                }
            }
            return 0.0f;
        }

        private static float CalculateTranslationWidthAfterFieldRot(Rectangle bBox, double pageRotation, double relFieldRotation
            ) {
            if (relFieldRotation == 0) {
                return 0.0f;
            }
            if (pageRotation == 0 && (relFieldRotation == Math.PI || relFieldRotation == 3 * Math.PI / 2)) {
                return bBox.GetWidth();
            }
            if (pageRotation == -Math.PI / 2) {
                if (relFieldRotation == -Math.PI / 2 || relFieldRotation == Math.PI) {
                    return bBox.GetHeight();
                }
            }
            if (pageRotation == -Math.PI) {
                if (relFieldRotation == -1 * Math.PI) {
                    return bBox.GetWidth();
                }
                if (relFieldRotation == -1 * Math.PI / 2) {
                    return bBox.GetHeight();
                }
                if (relFieldRotation == Math.PI / 2) {
                    return -1 * (bBox.GetHeight() - bBox.GetWidth());
                }
            }
            if (pageRotation == -3 * Math.PI / 2) {
                if (relFieldRotation == -3 * Math.PI / 2) {
                    return -1 * (bBox.GetWidth() - bBox.GetHeight());
                }
                if (relFieldRotation == -Math.PI) {
                    return bBox.GetHeight();
                }
                if (relFieldRotation == -Math.PI / 2) {
                    return bBox.GetWidth();
                }
            }
            return 0.0f;
        }

        private static double DegreeToRadians(double angle) {
            return Math.PI * angle / 180.0;
        }
    }
//\endcond
}
