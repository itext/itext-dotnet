/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Commons.Internal.Runtime;
using iText.Commons.Logs;
using iText.Kernel.Geom;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Logs;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
//\cond DO_NOT_DOCUMENT
    internal sealed class RotationUtils {
        private static readonly LazyLogger LOGGER = new LazyLogger(typeof(iText.Layout.Renderer.RotationUtils));

        private RotationUtils() {
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Calculates a rotated
        /// <see cref="iText.Layout.Minmaxwidth.MinMaxWidth"/>
        /// for a renderer that has
        /// <see cref="iText.Layout.Properties.Property.ROTATION_ANGLE"/>
        /// set.
        /// </summary>
        /// <remarks>
        /// Calculates a rotated
        /// <see cref="iText.Layout.Minmaxwidth.MinMaxWidth"/>
        /// for a renderer that has
        /// <see cref="iText.Layout.Properties.Property.ROTATION_ANGLE"/>
        /// set.
        /// <para />
        /// This method lays out the renderer using the original (non-rotated) min and max widths,
        /// then converts both resulting occupied area bounding boxes to rotated widths.
        /// The smaller rotated value becomes the returned min width and the larger one becomes
        /// the returned max width.
        /// <para />
        /// This is an approximation, not an exact rotated min/max width calculation.
        /// Only two sample points are measured (original min and original max width),
        /// so it does not account for cases where the rotated extrema occur at an intermediate width.
        /// </remarks>
        /// <param name="minMaxWidth">min/max width calculated for the non-rotated renderer</param>
        /// <param name="renderer">the renderer</param>
        /// <returns>rotated min/max width</returns>
        internal static MinMaxWidth CalculateRotationMinMaxWidth(MinMaxWidth minMaxWidth, AbstractRenderer renderer
            ) {
            RotationUtils.PropertiesBackup backup = new RotationUtils.PropertiesBackup(renderer);
            float? rotation = backup.StoreFloatProperty(Property.ROTATION_ANGLE);
            if (rotation != null) {
                float angle = (float)rotation;
                // Measure rotated widths at min and max widths
                float? rotatedAtMinWidth = GetLayoutRotatedWidth(renderer, minMaxWidth.GetMinWidth(), angle);
                float? rotatedAtMaxWidth = GetLayoutRotatedWidth(renderer, minMaxWidth.GetMaxWidth(), angle);
                if (rotatedAtMinWidth != null && rotatedAtMaxWidth != null) {
                    backup.RestoreProperty(Property.ROTATION_ANGLE);
                    return new MinMaxWidth(Math.Min(rotatedAtMinWidth.Value, rotatedAtMaxWidth.Value), Math.Max(rotatedAtMinWidth
                        .Value, rotatedAtMaxWidth.Value), 0);
                }
            }
            backup.RestoreProperty(Property.ROTATION_ANGLE);
            return minMaxWidth;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>This method tries to calculate width of not rotated renderer, so after rotation it fits availableWidth.
        ///     </summary>
        /// <remarks>
        /// This method tries to calculate width of not rotated renderer, so after rotation it fits availableWidth.
        /// This method uses heuristics of
        /// <see cref="iText.Layout.Minmaxwidth.RotationMinMaxWidth.Calculate(double, double, iText.Layout.Minmaxwidth.MinMaxWidth, double)
        ///     "/>
        /// as a fallback if we could not find an appropriate rotated width by laying out element without rotation.
        /// The minMaxWidth calculations and initial layout may take long time.
        /// </remarks>
        /// <param name="availableWidth">the width of layoutArea</param>
        /// <param name="availableHeight">the height of layoutArea</param>
        /// <param name="renderer">the actual renderer</param>
        /// <returns>
        /// the width that should be set as width of layout area to properly layout element, or fallback to
        /// <see cref="AbstractRenderer.RetrieveWidth(float)"/>
        /// in case it can not be calculated, or renderer isn't rotated.
        /// </returns>
        internal static float? RetrieveRotatedLayoutWidth(float availableWidth, float availableHeight, AbstractRenderer
             renderer) {
            RotationUtils.PropertiesBackup backup = new RotationUtils.PropertiesBackup(renderer);
            float? rotation = backup.StoreFloatProperty(Property.ROTATION_ANGLE);
            try {
                UnitValue widthProperty = renderer.GetProperty<UnitValue>(Property.WIDTH);
                float? resolvedWidth = renderer.RetrieveWidth(availableWidth);
                // Some renderers (like CellRenderer) may have WIDTH property set, but retrieveWidth() still returns null
                // because width depends on a parent context (table column width). In that case optimization is needed.
                if (rotation != null && (widthProperty == null || resolvedWidth == null)) {
                    float angle = (float)rotation;
                    // Backup FORCED_PLACEMENT property to avoid successful layout of rotated element
                    // in case it doesn't fit. It also prevents mutating the renderer's state during layout.
                    backup.StoreBoolProperty(Property.FORCED_PLACEMENT);
                    MinMaxWidth minMaxWidth = renderer.GetMinMaxWidth();
                    float? heuristicsWidth = FallbackHeuristicsAlgo(renderer, minMaxWidth, angle, availableWidth);
                    Rectangle additions = new Rectangle(0, 0);
                    renderer.ApplyPaddings(additions, true);
                    renderer.ApplyBorderBox(additions, true);
                    renderer.ApplyMargins(additions, true);
                    RotationUtils.RotatedMetrics bestFit = FindBestFittingMetrics(renderer, minMaxWidth, heuristicsWidth, availableWidth
                        , availableHeight, angle);
                    if (bestFit != null) {
                        return bestFit.originalWidth - additions.GetWidth() + MinMaxWidthUtils.GetEps();
                    }
                    else {
                        if (heuristicsWidth != null) {
                            return heuristicsWidth.Value - additions.GetWidth() + MinMaxWidthUtils.GetEps();
                        }
                    }
                }
                return renderer.RetrieveWidth(availableWidth);
            }
            finally {
                backup.RestoreProperty(Property.ROTATION_ANGLE);
                backup.RestoreProperty(Property.FORCED_PLACEMENT);
            }
        }
//\endcond

        private static float? FallbackHeuristicsAlgo(AbstractRenderer renderer, MinMaxWidth minMaxWidth, float angle
            , float availableWidth) {
            float? width = null;
            RotationUtils.PropertiesBackup backup = new RotationUtils.PropertiesBackup(renderer);
            backup.StoreProperty<UnitValue>(Property.HEIGHT);
            backup.StoreProperty<UnitValue>(Property.MIN_HEIGHT);
            backup.StoreProperty<UnitValue>(Property.MAX_HEIGHT);
            float length = (minMaxWidth.GetMaxWidth() + minMaxWidth.GetMinWidth()) / 2 + MinMaxWidthUtils.GetEps();
            // Using this width for initial layout helps in case of small elements. They may have more free spaces,
            // but it's more likely they fit.
            LayoutResult layoutResult = renderer.Layout(new LayoutContext(new LayoutArea(1, new Rectangle(length, AbstractRenderer
                .INF))));
            if (layoutResult.GetOccupiedArea() != null) {
                double area = layoutResult.GetOccupiedArea().GetBBox().GetWidth() * layoutResult.GetOccupiedArea().GetBBox
                    ().GetHeight();
                RotationMinMaxWidth result = RotationMinMaxWidth.Calculate(angle, area, minMaxWidth, availableWidth);
                if (result != null) {
                    width = result.GetMaxWidthHeight() > result.GetMinWidthHeight() ? (float)result.GetMinWidthOrigin() : (float
                        )result.GetMaxWidthOrigin();
                }
            }
            else {
                LOGGER.Warn(() => LayoutLogMessageConstant.ROTATED_LAYOUT_ELEMENT_DOES_NOT_FIT_AREA);
            }
            backup.RestoreProperty(Property.HEIGHT);
            backup.RestoreProperty(Property.MIN_HEIGHT);
            backup.RestoreProperty(Property.MAX_HEIGHT);
            return width;
        }

        // Get actual width of element based on its layout
        private static float? GetLayoutRotatedWidth(AbstractRenderer renderer, float layoutWidth, double angle) {
            RotationUtils.RotatedMetrics rotatedMetrics = GetLayoutRotatedMetrics(renderer, layoutWidth, angle);
            if (rotatedMetrics != null) {
                return rotatedMetrics.rotatedWidth;
            }
            else {
                return null;
            }
        }

        private static RotationUtils.RotatedMetrics GetLayoutRotatedMetrics(AbstractRenderer renderer, float layoutWidth
            , double angle) {
            LayoutResult result = renderer.Layout(new LayoutContext(new LayoutArea(1, new Rectangle(layoutWidth + MinMaxWidthUtils
                .GetEps(), AbstractRenderer.INF))));
            if (result.GetOccupiedArea() == null || result.GetStatus() != LayoutResult.FULL) {
                if (result.GetOccupiedArea() == null) {
                    LOGGER.Warn(() => LayoutLogMessageConstant.ROTATED_LAYOUT_ELEMENT_DOES_NOT_FIT_AREA);
                }
                return null;
            }
            Rectangle bbox = result.GetOccupiedArea().GetBBox();
            float rotatedWidth = (float)RotationMinMaxWidth.CalculateRotatedWidth(bbox, angle);
            float rotatedHeight = (float)RotationMinMaxWidth.CalculateRotatedHeight(bbox, angle);
            return new RotationUtils.RotatedMetrics(layoutWidth, rotatedWidth, rotatedHeight);
        }

        private static RotationUtils.RotatedMetrics FindBestFittingMetrics(AbstractRenderer renderer, MinMaxWidth 
            minMaxWidth, float? heuristicsWidth, float availableWidth, float availableHeight, double angle) {
            float eps = MinMaxWidthUtils.GetEps();
            float minOriginWidth = minMaxWidth.GetMinWidth() + eps;
            float maxOriginWidth = minMaxWidth.GetMaxWidth() + eps;
            RotationUtils.RotatedMetrics bestFit = null;
            AbstractRenderer r = renderer;
            if (renderer is CellRenderer) {
                // Use a special renderer for cells
                Cell cellModel = (Cell)renderer.GetModelElement();
                AbstractRenderer subTree = (AbstractRenderer)cellModel.CreateRendererSubTree();
                r = new RotationUtils.RotatedCellRenderer(cellModel);
                r.SetParent(renderer.GetParent());
                r.AddAllChildRenderers(subTree.GetChildRenderers());
                r.AddAllProperties(renderer.GetOwnProperties());
            }
            if (heuristicsWidth != null) {
                RotationUtils.RotatedMetrics metricsAtHeuristics = GetLayoutRotatedMetrics(r, heuristicsWidth.Value, angle
                    );
                bestFit = ChooseLowerHeightFit(bestFit, metricsAtHeuristics, availableWidth, availableHeight);
            }
            // Try different options
            foreach (float currentWidth in GetCandidateWidths(minOriginWidth, maxOriginWidth, bestFit)) {
                RotationUtils.RotatedMetrics candidate = GetLayoutRotatedMetrics(r, currentWidth, angle);
                bestFit = ChooseLowerHeightFit(bestFit, candidate, availableWidth + eps, availableHeight + eps);
            }
            return bestFit;
        }

        private static RotationUtils.RotatedMetrics ChooseLowerHeightFit(RotationUtils.RotatedMetrics currentBest, 
            RotationUtils.RotatedMetrics candidate, float availableWidth, float availableHeight) {
            if (candidate == null || candidate.rotatedWidth > availableWidth || candidate.rotatedHeight > availableHeight
                ) {
                return currentBest;
            }
            if (currentBest == null || candidate.rotatedHeight < currentBest.rotatedHeight) {
                return candidate;
            }
            return currentBest;
        }

        private static float[] GetCandidateWidths(float minOriginWidth, float maxOriginWidth, RotationUtils.RotatedMetrics
             metricsAtHeuristics) {
            // If we already found something suitable using heuristics, let's not try hard to improve it further
            // If heuristics failed completely, let's try to find some result by using smaller steps and more iterations
            int AMOUNT_OF_STEPS = metricsAtHeuristics == null ? 10 : 4;
            float[] widths = new float[AMOUNT_OF_STEPS];
            for (int i = 0; i < AMOUNT_OF_STEPS; i++) {
                widths[i] = minOriginWidth + (maxOriginWidth - minOriginWidth) / (AMOUNT_OF_STEPS - 1) * i;
            }
            return widths;
        }

        private sealed class RotatedMetrics {
            public readonly float originalWidth;

            public readonly float rotatedWidth;

            public readonly float rotatedHeight;

            public RotatedMetrics(float originalWidth, float rotatedWidth, float rotatedHeight) {
                this.originalWidth = originalWidth;
                this.rotatedWidth = rotatedWidth;
                this.rotatedHeight = rotatedHeight;
            }
        }

        private sealed class PropertiesBackup {
            private AbstractRenderer renderer;

            private Dictionary<int, RotationUtils.PropertiesBackup.PropertyBackup> propertiesBackup = new Dictionary<int
                , RotationUtils.PropertiesBackup.PropertyBackup>();

            public PropertiesBackup(AbstractRenderer renderer) {
                this.renderer = renderer;
            }

            //workaround for autoport
            public float? StoreFloatProperty(int property) {
                float? value = renderer.GetPropertyAsFloat(property);
                if (value != null) {
                    propertiesBackup.Put(property, new RotationUtils.PropertiesBackup.PropertyBackup(value, renderer.HasOwnProperty
                        (property)));
                    renderer.SetProperty(property, null);
                }
                return value;
            }

            public bool? StoreBoolProperty(int property) {
                bool? value = renderer.GetPropertyAsBoolean(property);
                if (value != null) {
                    propertiesBackup.Put(property, new RotationUtils.PropertiesBackup.PropertyBackup(value, renderer.HasOwnProperty
                        (property)));
                    renderer.SetProperty(property, null);
                }
                return value;
            }

            public T StoreProperty<T>(int property) {
                T value = renderer.GetProperty<T>(property);
                if (value != null) {
                    propertiesBackup.Put(property, new RotationUtils.PropertiesBackup.PropertyBackup(value, renderer.HasOwnProperty
                        (property)));
                    renderer.SetProperty(property, null);
                }
                return value;
            }

            public void RestoreProperty(int property) {
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

        /// <summary>This renderer is used for calculations of rotated area.</summary>
        /// <remarks>
        /// This renderer is used for calculations of rotated area.
        /// processNotFullChildResult switches off wasHeightClipped parameter because it allows LayoutResult.FULL
        /// even if the element doesn't fit by height.
        /// </remarks>
        private sealed class RotatedCellRenderer : CellRenderer {
            public RotatedCellRenderer(Cell modelElement)
                : base(modelElement) {
            }

            public override IRenderer GetNextRenderer() {
                return new RotationUtils.RotatedCellRenderer((Cell)modelElement);
            }

//\cond DO_NOT_DOCUMENT
            internal override LayoutResult ProcessNotFullChildResult(LayoutContext layoutContext, IDictionary<int, IRenderer
                > waitingFloatsSplitRenderers, IList<IRenderer> waitingOverflowFloatRenderers, bool wasHeightClipped, 
                IList<Rectangle> floatRendererAreas, bool marginsCollapsingEnabled, float clearHeightCorrection, Border
                [] borders, UnitValue[] paddings, IList<Rectangle> areas, int currentAreaPos, Rectangle layoutBox, ICollection
                <Rectangle> nonChildFloatingRendererAreas, IRenderer causeOfNothing, bool anythingPlaced, int childPos
                , LayoutResult result) {
                return base.ProcessNotFullChildResult(layoutContext, waitingFloatsSplitRenderers, waitingOverflowFloatRenderers
                    , false, floatRendererAreas, marginsCollapsingEnabled, clearHeightCorrection, borders, paddings, areas
                    , currentAreaPos, layoutBox, nonChildFloatingRendererAreas, causeOfNothing, anythingPlaced, childPos, 
                    result);
            }
//\endcond
        }
    }
//\endcond
}
