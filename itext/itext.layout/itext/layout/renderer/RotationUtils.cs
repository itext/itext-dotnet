using System;
using System.Collections.Generic;
using iText.Kernel.Geom;
using iText.Layout.Layout;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    internal class RotationUtils {
        private RotationUtils() {
        }

        /// <summary>
        /// This method tries to calculate min-max-width of rotated element using heuristics
        /// of
        /// <see cref="iText.Layout.Minmaxwidth.RotationMinMaxWidth.Calculate(double, double, iText.Layout.Minmaxwidth.MinMaxWidth)
        ///     "/>
        /// .
        /// This method may call
        /// <see cref="IRenderer.Layout(iText.Layout.Layout.LayoutContext)"/>
        /// once in best case
        /// (if the width is set on element, or if we are really lucky) and three times in worst case.
        /// </summary>
        /// <param name="minMaxWidth">the minMaxWidth of NOT rotated renderer</param>
        /// <param name="renderer">the actual renderer</param>
        /// <returns>minMaxWidth of rotated renderer or original value in case rotated value can not be calculated, or renderer isn't rotated.
        ///     </returns>
        public static MinMaxWidth CountRotationMinMaxWidth(MinMaxWidth minMaxWidth, AbstractRenderer renderer) {
            RotationUtils.PropertiesBackup backup = new RotationUtils.PropertiesBackup(renderer);
            float? rotation = backup.StoreFloatProperty(Property.ROTATION_ANGLE);
            if (rotation != null) {
                float angle = rotation.Value;
                //This width results in more accurate values for min-width calculations.
                float layoutWidth = minMaxWidth.GetMaxWidth() + MinMaxWidthUtils.GetEps();
                LayoutResult layoutResult = renderer.Layout(new LayoutContext(new LayoutArea(1, new Rectangle(layoutWidth, 
                    AbstractRenderer.INF))));
                if (layoutResult.GetOccupiedArea() != null) {
                    Rectangle layoutBBox = layoutResult.GetOccupiedArea().GetBBox();
                    if (MinMaxWidthUtils.IsEqual(minMaxWidth.GetMinWidth(), minMaxWidth.GetMaxWidth())) {
                        backup.RestoreProperty(Property.ROTATION_ANGLE);
                        float rotatedWidth = (float)RotationMinMaxWidth.CalculateRotatedWidth(layoutBBox, angle);
                        return new MinMaxWidth(0, MinMaxWidthUtils.GetMax(), rotatedWidth, rotatedWidth);
                    }
                    double area = layoutResult.GetOccupiedArea().GetBBox().GetWidth() * layoutResult.GetOccupiedArea().GetBBox
                        ().GetHeight();
                    RotationMinMaxWidth rotationMinMaxWidth = RotationMinMaxWidth.Calculate(angle, area, minMaxWidth);
                    float? rotatedMinWidth = GetLayoutRotatedWidth(renderer, (float)rotationMinMaxWidth.GetMinWidthOrigin(), layoutBBox
                        , angle);
                    if (rotatedMinWidth != null) {
                        if (rotatedMinWidth > rotationMinMaxWidth.GetMaxWidth()) {
                            rotationMinMaxWidth.SetChildrenMinWidth(rotatedMinWidth.Value);
                            float? rotatedMaxWidth = GetLayoutRotatedWidth(renderer, (float)rotationMinMaxWidth.GetMaxWidthOrigin(), layoutBBox
                                , angle);
                            if (rotatedMaxWidth != null && rotatedMaxWidth > rotatedMinWidth) {
                                rotationMinMaxWidth.SetChildrenMaxWidth(rotatedMaxWidth.Value);
                            }
                            else {
                                rotationMinMaxWidth.SetChildrenMaxWidth(rotatedMinWidth.Value);
                            }
                        }
                        else {
                            rotationMinMaxWidth.SetChildrenMinWidth(rotatedMinWidth.Value);
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
        ///     "/>
        /// .
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
                float angle = rotation.Value;
                backup.StoreFloatProperty(Property.HEIGHT);
                backup.StoreFloatProperty(Property.MIN_HEIGHT);
                backup.StoreFloatProperty(Property.MAX_HEIGHT);
                MinMaxWidth minMaxWidth = renderer.GetMinMaxWidth(MinMaxWidthUtils.GetMax());
                //Using this width for initial layout helps in case of small elements. They may have more free spaces but it's more likely they fit.
                float length = (minMaxWidth.GetMaxWidth() + minMaxWidth.GetMinWidth()) / 2 + MinMaxWidthUtils.GetEps();
                LayoutResult layoutResult = renderer.Layout(new LayoutContext(new LayoutArea(1, new Rectangle(length, AbstractRenderer
                    .INF))));
                backup.RestoreProperty(Property.HEIGHT);
                backup.RestoreProperty(Property.MIN_HEIGHT);
                backup.RestoreProperty(Property.MAX_HEIGHT);
                if (layoutResult.GetOccupiedArea() != null) {
                    double area = layoutResult.GetOccupiedArea().GetBBox().GetWidth() * layoutResult.GetOccupiedArea().GetBBox
                        ().GetHeight();
                    RotationMinMaxWidth result = RotationMinMaxWidth.Calculate(angle, area, minMaxWidth, availableWidth);
                    if (result != null) {
                        backup.RestoreProperty(Property.ROTATION_ANGLE);
                        if (result.GetMaxWidthHeight() > result.GetMinWidthHeight()) {
                            return (float)result.GetMinWidthOrigin() + MinMaxWidthUtils.GetEps();
                        }
                        else {
                            return (float)result.GetMaxWidthOrigin() + MinMaxWidthUtils.GetEps();
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
}
