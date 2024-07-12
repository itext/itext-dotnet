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
using iText.Kernel.Geom;
using iText.Layout.Layout;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
//\cond DO_NOT_DOCUMENT
    internal sealed class RotationUtils {
        private RotationUtils() {
        }

        /// <summary>
        /// This method tries to calculate min-max-width of rotated element using heuristics
        /// of
        /// <see cref="iText.Layout.Minmaxwidth.RotationMinMaxWidth.Calculate(double, double, iText.Layout.Minmaxwidth.MinMaxWidth)
        ///     "/>.
        /// </summary>
        /// <remarks>
        /// This method tries to calculate min-max-width of rotated element using heuristics
        /// of
        /// <see cref="iText.Layout.Minmaxwidth.RotationMinMaxWidth.Calculate(double, double, iText.Layout.Minmaxwidth.MinMaxWidth)
        ///     "/>.
        /// This method may call
        /// <see cref="IRenderer.Layout(iText.Layout.Layout.LayoutContext)"/>
        /// once in best case
        /// (if the width is set on element, or if we are really lucky) and three times in worst case.
        /// </remarks>
        /// <param name="minMaxWidth">the minMaxWidth of NOT rotated renderer</param>
        /// <param name="renderer">the actual renderer</param>
        /// <returns>minMaxWidth of rotated renderer or original value in case rotated value can not be calculated, or renderer isn't rotated.
        ///     </returns>
        public static MinMaxWidth CountRotationMinMaxWidth(MinMaxWidth minMaxWidth, AbstractRenderer renderer) {
            RotationUtils.PropertiesBackup backup = new RotationUtils.PropertiesBackup(renderer);
            float? rotation = backup.StoreFloatProperty(Property.ROTATION_ANGLE);
            if (rotation != null) {
                float angle = (float)rotation;
                //This width results in more accurate values for min-width calculations.
                float layoutWidth = minMaxWidth.GetMaxWidth() + MinMaxWidthUtils.GetEps();
                LayoutResult layoutResult = renderer.Layout(new LayoutContext(new LayoutArea(1, new Rectangle(layoutWidth, 
                    AbstractRenderer.INF))));
                if (layoutResult.GetOccupiedArea() != null) {
                    Rectangle layoutBBox = layoutResult.GetOccupiedArea().GetBBox();
                    if (MinMaxWidthUtils.IsEqual(minMaxWidth.GetMinWidth(), minMaxWidth.GetMaxWidth())) {
                        backup.RestoreProperty(Property.ROTATION_ANGLE);
                        float rotatedWidth = (float)RotationMinMaxWidth.CalculateRotatedWidth(layoutBBox, angle);
                        return new MinMaxWidth(rotatedWidth, rotatedWidth, 0);
                    }
                    double area = layoutResult.GetOccupiedArea().GetBBox().GetWidth() * layoutResult.GetOccupiedArea().GetBBox
                        ().GetHeight();
                    RotationMinMaxWidth rotationMinMaxWidth = RotationMinMaxWidth.Calculate(angle, area, minMaxWidth);
                    float? rotatedMinWidth = GetLayoutRotatedWidth(renderer, (float)rotationMinMaxWidth.GetMinWidthOrigin(), layoutBBox
                        , angle);
                    if (rotatedMinWidth != null) {
                        if (rotatedMinWidth > rotationMinMaxWidth.GetMaxWidth()) {
                            rotationMinMaxWidth.SetChildrenMinWidth((float)rotatedMinWidth);
                            float? rotatedMaxWidth = GetLayoutRotatedWidth(renderer, (float)rotationMinMaxWidth.GetMaxWidthOrigin(), layoutBBox
                                , angle);
                            if (rotatedMaxWidth != null && rotatedMaxWidth > rotatedMinWidth) {
                                rotationMinMaxWidth.SetChildrenMaxWidth((float)rotatedMaxWidth);
                            }
                            else {
                                rotationMinMaxWidth.SetChildrenMaxWidth((float)rotatedMinWidth);
                            }
                        }
                        else {
                            rotationMinMaxWidth.SetChildrenMinWidth((float)rotatedMinWidth);
                        }
                        backup.RestoreProperty(Property.ROTATION_ANGLE);
                        return rotationMinMaxWidth;
                    }
                }
            }
            backup.RestoreProperty(Property.ROTATION_ANGLE);
            return minMaxWidth;
        }

        /// <summary>This method tries to calculate width of not rotated renderer, so after rotation it fits availableWidth.
        ///     </summary>
        /// <remarks>
        /// This method tries to calculate width of not rotated renderer, so after rotation it fits availableWidth.
        /// This method uses heuristics of
        /// <see cref="iText.Layout.Minmaxwidth.RotationMinMaxWidth.Calculate(double, double, iText.Layout.Minmaxwidth.MinMaxWidth, double)
        ///     "/>.
        /// It doesn't take into account any of height properties of renderer or height of layoutArea.
        /// The minMaxWidth calculations and initial layout may take long time, but they won't be called if the renderer have width property.
        /// </remarks>
        /// <param name="availableWidth">the width of layoutArea</param>
        /// <param name="renderer">the actual renderer</param>
        /// <returns>
        /// the width that should be set as width of layout area to properly layout element, or fallback to
        /// <see cref="AbstractRenderer.RetrieveWidth(float)"/>
        /// in case it can not be calculated, or renderer isn't rotated.
        /// </returns>
        public static float? RetrieveRotatedLayoutWidth(float availableWidth, AbstractRenderer renderer) {
            RotationUtils.PropertiesBackup backup = new RotationUtils.PropertiesBackup(renderer);
            float? rotation = backup.StoreFloatProperty(Property.ROTATION_ANGLE);
            if (rotation != null && renderer.GetProperty<UnitValue>(Property.WIDTH) == null) {
                float angle = (float)rotation;
                backup.StoreProperty<UnitValue>(Property.HEIGHT);
                backup.StoreProperty<UnitValue>(Property.MIN_HEIGHT);
                backup.StoreProperty<UnitValue>(Property.MAX_HEIGHT);
                backup.StoreBoolProperty(Property.FORCED_PLACEMENT);
                MinMaxWidth minMaxWidth = renderer.GetMinMaxWidth();
                // Using this width for initial layout helps in case of small elements. They may have more free spaces,
                // but it's more likely they fit.
                float length = (minMaxWidth.GetMaxWidth() + minMaxWidth.GetMinWidth()) / 2 + MinMaxWidthUtils.GetEps();
                LayoutResult layoutResult = renderer.Layout(new LayoutContext(new LayoutArea(1, new Rectangle(length, AbstractRenderer
                    .INF))));
                backup.RestoreProperty(Property.HEIGHT);
                backup.RestoreProperty(Property.MIN_HEIGHT);
                backup.RestoreProperty(Property.MAX_HEIGHT);
                backup.RestoreProperty(Property.FORCED_PLACEMENT);
                Rectangle additions = new Rectangle(0, 0);
                renderer.ApplyPaddings(additions, true);
                renderer.ApplyBorderBox(additions, true);
                renderer.ApplyMargins(additions, true);
                if (layoutResult.GetOccupiedArea() != null) {
                    double area = layoutResult.GetOccupiedArea().GetBBox().GetWidth() * layoutResult.GetOccupiedArea().GetBBox
                        ().GetHeight();
                    RotationMinMaxWidth result = RotationMinMaxWidth.Calculate(angle, area, minMaxWidth, availableWidth);
                    if (result != null) {
                        backup.RestoreProperty(Property.ROTATION_ANGLE);
                        if (result.GetMaxWidthHeight() > result.GetMinWidthHeight()) {
                            return (float)(result.GetMinWidthOrigin() - additions.GetWidth() + MinMaxWidthUtils.GetEps());
                        }
                        else {
                            return (float)(result.GetMaxWidthOrigin() - additions.GetWidth() + MinMaxWidthUtils.GetEps());
                        }
                    }
                }
            }
            backup.RestoreProperty(Property.ROTATION_ANGLE);
            return renderer.RetrieveWidth(availableWidth);
        }

        //Get actual width of element based on it's layout. May use occupied are of layout result of initial layout for time saving.
        private static float? GetLayoutRotatedWidth(AbstractRenderer renderer, float availableWidth, Rectangle previousBBox
            , double angle) {
            if (MinMaxWidthUtils.IsEqual(availableWidth, previousBBox.GetWidth())) {
                return (float)RotationMinMaxWidth.CalculateRotatedWidth(previousBBox, angle);
            }
            LayoutResult result = renderer.Layout(new LayoutContext(new LayoutArea(1, new Rectangle(availableWidth + MinMaxWidthUtils
                .GetEps(), AbstractRenderer.INF))));
            if (result.GetOccupiedArea() != null) {
                return (float)RotationMinMaxWidth.CalculateRotatedWidth(result.GetOccupiedArea().GetBBox(), angle);
            }
            return null;
        }

        private class PropertiesBackup {
            private AbstractRenderer renderer;

            private Dictionary<int, RotationUtils.PropertiesBackup.PropertyBackup> propertiesBackup = new Dictionary<int
                , RotationUtils.PropertiesBackup.PropertyBackup>();

            public PropertiesBackup(AbstractRenderer renderer) {
                this.renderer = renderer;
            }

            //workaround for autoport
            public virtual float? StoreFloatProperty(int property) {
                float? value = renderer.GetPropertyAsFloat(property);
                if (value != null) {
                    propertiesBackup.Put(property, new RotationUtils.PropertiesBackup.PropertyBackup(value, renderer.HasOwnProperty
                        (property)));
                    renderer.SetProperty(property, null);
                }
                return value;
            }

            public virtual bool? StoreBoolProperty(int property) {
                bool? value = renderer.GetPropertyAsBoolean(property);
                if (value != null) {
                    propertiesBackup.Put(property, new RotationUtils.PropertiesBackup.PropertyBackup(value, renderer.HasOwnProperty
                        (property)));
                    renderer.SetProperty(property, null);
                }
                return value;
            }

            public virtual T StoreProperty<T>(int property) {
                T value = renderer.GetProperty<T>(property);
                if (value != null) {
                    propertiesBackup.Put(property, new RotationUtils.PropertiesBackup.PropertyBackup(value, renderer.HasOwnProperty
                        (property)));
                    renderer.SetProperty(property, null);
                }
                return value;
            }

            public virtual void RestoreProperty(int property) {
                RotationUtils.PropertiesBackup.PropertyBackup backup = propertiesBackup.JRemove(property);
                if (backup != null) {
                    if (backup.IsOwnedByRender()) {
                        renderer.SetProperty(property, backup.GetValue());
                    }
                    else {
                        renderer.DeleteOwnProperty(property);
                    }
                }
            }

            private class PropertyBackup {
                private Object propertyValue;

                private bool isOwnedByRender;

                public PropertyBackup(Object propertyValue, bool isOwnedByRender) {
                    this.propertyValue = propertyValue;
                    this.isOwnedByRender = isOwnedByRender;
                }

                public virtual Object GetValue() {
                    return propertyValue;
                }

                public virtual bool IsOwnedByRender() {
                    return isOwnedByRender;
                }
            }
        }
    }
//\endcond
}
