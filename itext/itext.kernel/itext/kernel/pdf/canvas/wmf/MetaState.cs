/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;

namespace iText.Kernel.Pdf.Canvas.Wmf {
    /// <summary>Class to keep the state.</summary>
    public class MetaState {
        public const int TA_NOUPDATECP = 0;

        public const int TA_UPDATECP = 1;

        public const int TA_LEFT = 0;

        public const int TA_RIGHT = 2;

        public const int TA_CENTER = 6;

        public const int TA_TOP = 0;

        public const int TA_BOTTOM = 8;

        public const int TA_BASELINE = 24;

        public const int TRANSPARENT = 1;

        public const int OPAQUE = 2;

        public const int ALTERNATE = 1;

        public const int WINDING = 2;

        /// <summary>Stack of saved states.</summary>
        public Stack<iText.Kernel.Pdf.Canvas.Wmf.MetaState> savedStates;

        /// <summary>List of MetaObjects.</summary>
        public IList<MetaObject> MetaObjects;

        /// <summary>Current Point.</summary>
        public Point currentPoint;

        /// <summary>Current Pen.</summary>
        public MetaPen currentPen;

        /// <summary>Current Brush.</summary>
        public MetaBrush currentBrush;

        /// <summary>Current Font.</summary>
        public MetaFont currentFont;

        /// <summary>The current background color.</summary>
        /// <remarks>The current background color. Default value is DeviceRgb#WHITE.</remarks>
        public Color currentBackgroundColor = ColorConstants.WHITE;

        /// <summary>Current text color.</summary>
        /// <remarks>Current text color. Default value is DeviceRgb#BLACK.</remarks>
        public Color currentTextColor = ColorConstants.BLACK;

        /// <summary>The current background mode.</summary>
        /// <remarks>The current background mode. Default value is OPAQUE.</remarks>
        public int backgroundMode = OPAQUE;

        /// <summary>Current polygon fill mode.</summary>
        /// <remarks>Current polygon fill mode. Default value is ALTERNATE.</remarks>
        public int polyFillMode = ALTERNATE;

        /// <summary>Curent line join.</summary>
        /// <remarks>Curent line join. Default value is 1.</remarks>
        public int lineJoin = 1;

        /// <summary>Current text alignment.</summary>
        public int textAlign;

        /// <summary>Current offset for Wx.</summary>
        public int offsetWx;

        /// <summary>Current offset for Wy.</summary>
        public int offsetWy;

        /// <summary>Current extent for Wx.</summary>
        public int extentWx;

        /// <summary>Current extent for Wy.</summary>
        public int extentWy;

        /// <summary>Current x value for scaling.</summary>
        public float scalingX;

        /// <summary>Current y value for scaling.</summary>
        public float scalingY;

        /// <summary>Creates new MetaState</summary>
        public MetaState() {
            savedStates = new Stack<iText.Kernel.Pdf.Canvas.Wmf.MetaState>();
            MetaObjects = new List<MetaObject>();
            currentPoint = new Point(0, 0);
            currentPen = new MetaPen();
            currentBrush = new MetaBrush();
            currentFont = new MetaFont();
        }

        /// <summary>Clones a new MetaState from the specified MetaState.</summary>
        /// <param name="state">the state to clone</param>
        public MetaState(iText.Kernel.Pdf.Canvas.Wmf.MetaState state) {
            SetMetaState(state);
        }

        /// <summary>Sets every field of this MetaState to the values of the fields of the specified MetaState.</summary>
        /// <param name="state">state to copy</param>
        public virtual void SetMetaState(iText.Kernel.Pdf.Canvas.Wmf.MetaState state) {
            savedStates = state.savedStates;
            MetaObjects = state.MetaObjects;
            currentPoint = state.currentPoint;
            currentPen = state.currentPen;
            currentBrush = state.currentBrush;
            currentFont = state.currentFont;
            currentBackgroundColor = state.currentBackgroundColor;
            currentTextColor = state.currentTextColor;
            backgroundMode = state.backgroundMode;
            polyFillMode = state.polyFillMode;
            textAlign = state.textAlign;
            lineJoin = state.lineJoin;
            offsetWx = state.offsetWx;
            offsetWy = state.offsetWy;
            extentWx = state.extentWx;
            extentWy = state.extentWy;
            scalingX = state.scalingX;
            scalingY = state.scalingY;
        }

        /// <summary>Add a MetaObject to the State.</summary>
        /// <param name="object">MetaObject to be added</param>
        public virtual void AddMetaObject(MetaObject @object) {
            for (int k = 0; k < MetaObjects.Count; ++k) {
                if (MetaObjects[k] == null) {
                    MetaObjects[k] = @object;
                    return;
                }
            }
            MetaObjects.Add(@object);
        }

        /// <summary>Select the MetaObject at the specified index and prepare the PdfCanvas.</summary>
        /// <param name="index">position of the MetaObject</param>
        /// <param name="cb">PdfCanvas to prepare</param>
        public virtual void SelectMetaObject(int index, PdfCanvas cb) {
            MetaObject obj = MetaObjects[index];
            if (obj == null) {
                return;
            }
            int style;
            switch (obj.GetObjectType()) {
                case MetaObject.META_BRUSH: {
                    currentBrush = (MetaBrush)obj;
                    style = currentBrush.GetStyle();
                    if (style == MetaBrush.BS_SOLID) {
                        Color color = currentBrush.GetColor();
                        cb.SetFillColor(color);
                    }
                    else {
                        if (style == MetaBrush.BS_HATCHED) {
                            Color color = currentBackgroundColor;
                            cb.SetFillColor(color);
                        }
                    }
                    break;
                }

                case MetaObject.META_PEN: {
                    currentPen = (MetaPen)obj;
                    style = currentPen.GetStyle();
                    if (style != MetaPen.PS_NULL) {
                        Color color = currentPen.GetColor();
                        cb.SetStrokeColor(color);
                        cb.SetLineWidth(Math.Abs(currentPen.GetPenWidth() * scalingX / extentWx));
                        switch (style) {
                            case MetaPen.PS_DASH: {
                                cb.SetLineDash(18, 6, 0);
                                break;
                            }

                            case MetaPen.PS_DASHDOT: {
                                cb.WriteLiteral("[9 6 3 6]0 d\n");
                                break;
                            }

                            case MetaPen.PS_DASHDOTDOT: {
                                cb.WriteLiteral("[9 3 3 3 3 3]0 d\n");
                                break;
                            }

                            case MetaPen.PS_DOT: {
                                cb.SetLineDash(3, 0);
                                break;
                            }

                            default: {
                                cb.SetLineDash(0);
                                break;
                            }
                        }
                    }
                    break;
                }

                case MetaObject.META_FONT: {
                    currentFont = (MetaFont)obj;
                    break;
                }
            }
        }

        /// <summary>Deletes the MetaObject at the specified index.</summary>
        /// <param name="index">index of the MetaObject to delete</param>
        public virtual void DeleteMetaObject(int index) {
            MetaObjects[index] = null;
        }

        /// <summary>Saves the state of this MetaState object.</summary>
        /// <param name="cb">PdfCanvas object on which saveState() will be called</param>
        public virtual void SaveState(PdfCanvas cb) {
            cb.SaveState();
            iText.Kernel.Pdf.Canvas.Wmf.MetaState state = new iText.Kernel.Pdf.Canvas.Wmf.MetaState(this);
            savedStates.Push(state);
        }

        /// <summary>Restores the state to the next state on the saved states stack.</summary>
        /// <param name="index">index of the state to be restored</param>
        /// <param name="cb">PdfCanvas object on which restoreState() will be called</param>
        public virtual void RestoreState(int index, PdfCanvas cb) {
            int pops;
            if (index < 0) {
                pops = Math.Min(-index, savedStates.Count);
            }
            else {
                pops = Math.Max(savedStates.Count - index, 0);
            }
            if (pops == 0) {
                return;
            }
            iText.Kernel.Pdf.Canvas.Wmf.MetaState state = null;
            while (pops-- != 0) {
                cb.RestoreState();
                state = savedStates.Pop();
            }
            SetMetaState(state);
        }

        /// <summary>Restres the state of the specified PdfCanvas object for as many times as there are saved states on the stack.
        ///     </summary>
        /// <param name="cb">PdfCanvas object</param>
        public virtual void Cleanup(PdfCanvas cb) {
            int k = savedStates.Count;
            while (k-- > 0) {
                cb.RestoreState();
            }
        }

        /// <summary>Transform the specified value.</summary>
        /// <param name="x">the value to transform</param>
        /// <returns>the transformed value</returns>
        public virtual float TransformX(int x) {
            return ((float)x - offsetWx) * scalingX / extentWx;
        }

        /// <summary>Transform the specified value.</summary>
        /// <param name="y">the value to transform</param>
        /// <returns>transformed value</returns>
        public virtual float TransformY(int y) {
            return (1f - ((float)y - offsetWy) / extentWy) * scalingY;
        }

        /// <summary>Sets the x value for scaling.</summary>
        /// <param name="scalingX">x value for scaling</param>
        public virtual void SetScalingX(float scalingX) {
            this.scalingX = scalingX;
        }

        /// <summary>Sets the y value for scaling.</summary>
        /// <param name="scalingY">y value for scaling</param>
        public virtual void SetScalingY(float scalingY) {
            this.scalingY = scalingY;
        }

        /// <summary>Sets the Wx offset value.</summary>
        /// <param name="offsetWx">Wx offset value</param>
        public virtual void SetOffsetWx(int offsetWx) {
            this.offsetWx = offsetWx;
        }

        /// <summary>Sets the Wy offset value.</summary>
        /// <param name="offsetWy">Wy offset value</param>
        public virtual void SetOffsetWy(int offsetWy) {
            this.offsetWy = offsetWy;
        }

        /// <summary>Sets the Wx extent value.</summary>
        /// <param name="extentWx">Wx extent value</param>
        public virtual void SetExtentWx(int extentWx) {
            this.extentWx = extentWx;
        }

        /// <summary>Sets the Wy extent value.</summary>
        /// <param name="extentWy">Wy extent value</param>
        public virtual void SetExtentWy(int extentWy) {
            this.extentWy = extentWy;
        }

        /// <summary>Transforms the specified angle.</summary>
        /// <remarks>
        /// Transforms the specified angle. If scalingY is less than 0, the angle is multiplied by -1. If scalingX is less
        /// than 0, the angle is subtracted from Math.PI.
        /// </remarks>
        /// <param name="angle">the angle to transform</param>
        /// <returns>the transformed angle</returns>
        public virtual float TransformAngle(float angle) {
            float ta = scalingY < 0 ? -angle : angle;
            return (float)(scalingX < 0 ? Math.PI - ta : ta);
        }

        /// <summary>Sets the current Point to the specified Point.</summary>
        /// <param name="p">Point to set</param>
        public virtual void SetCurrentPoint(Point p) {
            currentPoint = p;
        }

        /// <summary>Returns the current Point.</summary>
        /// <returns>current Point</returns>
        public virtual Point GetCurrentPoint() {
            return currentPoint;
        }

        /// <summary>Returns the current MetaBrush object.</summary>
        /// <returns>current MetaBrush</returns>
        public virtual MetaBrush GetCurrentBrush() {
            return currentBrush;
        }

        /// <summary>Returns the current MetaPen object.</summary>
        /// <returns>current MetaPen</returns>
        public virtual MetaPen GetCurrentPen() {
            return currentPen;
        }

        /// <summary>Returns the current MetaFont object.</summary>
        /// <returns>current MetaFont</returns>
        public virtual MetaFont GetCurrentFont() {
            return currentFont;
        }

        /// <summary>Getter for property currentBackgroundColor.</summary>
        /// <returns>Value of property currentBackgroundColor.</returns>
        public virtual Color GetCurrentBackgroundColor() {
            return currentBackgroundColor;
        }

        /// <summary>Setter for property currentBackgroundColor.</summary>
        /// <param name="currentBackgroundColor">New value of property currentBackgroundColor.</param>
        public virtual void SetCurrentBackgroundColor(Color currentBackgroundColor) {
            this.currentBackgroundColor = currentBackgroundColor;
        }

        /// <summary>Getter for property currentTextColor.</summary>
        /// <returns>Value of property currentTextColor.</returns>
        public virtual Color GetCurrentTextColor() {
            return currentTextColor;
        }

        /// <summary>Setter for property currentTextColor.</summary>
        /// <param name="currentTextColor">New value of property currentTextColor.</param>
        public virtual void SetCurrentTextColor(Color currentTextColor) {
            this.currentTextColor = currentTextColor;
        }

        /// <summary>Getter for property backgroundMode.</summary>
        /// <returns>Value of property backgroundMode.</returns>
        public virtual int GetBackgroundMode() {
            return backgroundMode;
        }

        /// <summary>Setter for property backgroundMode.</summary>
        /// <param name="backgroundMode">New value of property backgroundMode.</param>
        public virtual void SetBackgroundMode(int backgroundMode) {
            this.backgroundMode = backgroundMode;
        }

        /// <summary>Getter for property textAlign.</summary>
        /// <returns>Value of property textAlign.</returns>
        public virtual int GetTextAlign() {
            return textAlign;
        }

        /// <summary>Setter for property textAlign.</summary>
        /// <param name="textAlign">New value of property textAlign.</param>
        public virtual void SetTextAlign(int textAlign) {
            this.textAlign = textAlign;
        }

        /// <summary>Getter for property polyFillMode.</summary>
        /// <returns>Value of property polyFillMode.</returns>
        public virtual int GetPolyFillMode() {
            return polyFillMode;
        }

        /// <summary>Setter for property polyFillMode.</summary>
        /// <param name="polyFillMode">New value of property polyFillMode.</param>
        public virtual void SetPolyFillMode(int polyFillMode) {
            this.polyFillMode = polyFillMode;
        }

        /// <summary>
        /// Sets the line join style to
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.LineJoinStyle.MITER"/>
        /// if lineJoin isn't 0.
        /// </summary>
        /// <param name="cb">PdfCanvas to set the line join style</param>
        public virtual void SetLineJoinRectangle(PdfCanvas cb) {
            if (lineJoin != 0) {
                lineJoin = 0;
                cb.SetLineJoinStyle(PdfCanvasConstants.LineJoinStyle.MITER);
            }
        }

        /// <summary>
        /// Sets the line join style to
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.LineJoinStyle.ROUND"/>
        /// if lineJoin is 0.
        /// </summary>
        /// <param name="cb">PdfCanvas to set the line join style</param>
        public virtual void SetLineJoinPolygon(PdfCanvas cb) {
            if (lineJoin == 0) {
                lineJoin = 1;
                cb.SetLineJoinStyle(PdfCanvasConstants.LineJoinStyle.ROUND);
            }
        }

        /// <summary>Returns true if lineJoin is 0.</summary>
        /// <returns>true if lineJoin is 0</returns>
        public virtual bool GetLineNeutral() {
            return lineJoin == 0;
        }
    }
}
