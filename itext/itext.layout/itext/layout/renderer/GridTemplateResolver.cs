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
using iText.Layout.Exceptions;
using iText.Layout.Properties.Grid;

namespace iText.Layout.Renderer {
//\cond DO_NOT_DOCUMENT
    internal class GridTemplateResolver {
        private readonly float space;

        private readonly float gap;

        private bool containsIntrinsicOrFlexible = false;

        private GridTemplateResolver.AutoRepeatResolver autoRepeatResolver = null;

        private GridTemplateResolver.Result result = new GridTemplateResolver.Result(new List<GridValue>());

//\cond DO_NOT_DOCUMENT
        internal GridTemplateResolver(float space, float gap) {
            this.space = space;
            this.gap = gap;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Determines if auto-fit repeat was encountered during processing.</summary>
        /// <returns>true if auto-fit repeat was encountered, false otherwise</returns>
        internal virtual bool IsCollapseNullLines() {
            return autoRepeatResolver != null;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Determines how many fixed values (all template values except auto-fit/fill repeat) in the result.
        ///     </summary>
        /// <returns>number of fixed values in template list</returns>
        internal virtual int GetFixedValuesCount() {
            if (autoRepeatResolver == null) {
                return result.Size();
            }
            return result.Size() - (autoRepeatResolver.end - autoRepeatResolver.start);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Shrinks template list to fit the given size by reducing number of auto-fit/fill repetitions.</summary>
        /// <param name="sizeToFit">size to fit template list</param>
        internal virtual IList<GridValue> ShrinkTemplatesToFitSize(int sizeToFit) {
            if (autoRepeatResolver == null) {
                return result.GetList();
            }
            return autoRepeatResolver.ShrinkTemplatesToFitSize(sizeToFit);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Resolves template values to grid values by flatting repeats.</summary>
        /// <param name="template">template values list</param>
        /// <returns>grid values list</returns>
        internal virtual IList<GridValue> ResolveTemplate(IList<TemplateValue> template) {
            if (template == null) {
                return null;
            }
            try {
                float leftSpace = this.space;
                foreach (TemplateValue value in template) {
                    leftSpace -= ProcessValue(value);
                    leftSpace -= gap;
                }
                leftSpace += gap;
                if (autoRepeatResolver != null) {
                    if (autoRepeatResolver.start == result.Size()) {
                        // This additional gap is needed when auto-repeat is located at the end of a template
                        // It's for simplifying the logic of auto-repeat, because it always adds gap after last element
                        leftSpace += gap;
                    }
                    autoRepeatResolver.Resolve(leftSpace);
                }
                return result.GetList();
            }
            catch (InvalidOperationException exception) {
                ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.GridTemplateResolver)).LogWarning(exception.Message
                    );
                Reset();
            }
            return null;
        }
//\endcond

        private float ProcessValue(TemplateValue value) {
            switch (value.GetType()) {
                case TemplateValue.ValueType.MIN_CONTENT:
                case TemplateValue.ValueType.MAX_CONTENT:
                case TemplateValue.ValueType.AUTO:
                case TemplateValue.ValueType.FLEX:
                case TemplateValue.ValueType.FIT_CONTENT: {
                    result.AddValue((GridValue)value);
                    containsIntrinsicOrFlexible = true;
                    break;
                }

                case TemplateValue.ValueType.POINT: {
                    result.AddValue((GridValue)value);
                    return ((PointValue)value).GetValue();
                }

                case TemplateValue.ValueType.PERCENT: {
                    result.AddValue((GridValue)value);
                    return space > 0.0f ? ((PercentValue)value).GetValue() / 100 * space : 0.0f;
                }

                case TemplateValue.ValueType.MINMAX: {
                    result.AddValue((GridValue)value);
                    result.SetFreeze(true);
                    // Treating each track as its max track sizing function if that is definite
                    // or as its minimum track sizing function otherwise
                    // if encountered intrinsic or flexible before, then it doesn't matter what to process
                    bool currentValue = containsIntrinsicOrFlexible;
                    MinMaxValue minMaxValue = (MinMaxValue)value;
                    if (minMaxValue.GetMin().GetType() == TemplateValue.ValueType.FLEX) {
                        // A future level of CSS Grid spec may allow <flex> minimums, but not now
                        throw new InvalidOperationException(LayoutExceptionMessageConstant.FLEXIBLE_ARENT_ALLOWED_AS_MINIMUM_IN_MINMAX
                            );
                    }
                    float length = ProcessValue(minMaxValue.GetMax());
                    if (containsIntrinsicOrFlexible) {
                        length = ProcessValue(minMaxValue.GetMin());
                    }
                    containsIntrinsicOrFlexible = currentValue;
                    result.SetFreeze(false);
                    return length;
                }

                case TemplateValue.ValueType.FIXED_REPEAT: {
                    float usedSpace = 0.0f;
                    FixedRepeatValue repeat = (FixedRepeatValue)value;
                    for (int i = 0; i < repeat.GetRepeatCount(); ++i) {
                        foreach (GridValue element in repeat.GetValues()) {
                            usedSpace += ProcessValue(element);
                        }
                        usedSpace += (repeat.GetValues().Count - 1) * gap;
                    }
                    return usedSpace;
                }

                case TemplateValue.ValueType.AUTO_REPEAT: {
                    if (autoRepeatResolver != null) {
                        throw new InvalidOperationException(LayoutExceptionMessageConstant.GRID_AUTO_REPEAT_CAN_BE_USED_ONLY_ONCE);
                    }
                    autoRepeatResolver = new GridTemplateResolver.AutoRepeatResolver(this, (AutoRepeatValue)value, result.Size
                        ());
                    break;
                }
            }
            return 0.0f;
        }

        private void Reset() {
            autoRepeatResolver = null;
            result.GetList().Clear();
            result.SetInsertPoint(-1);
        }

        private class AutoRepeatResolver {
//\cond DO_NOT_DOCUMENT
            internal readonly AutoRepeatValue repeat;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal readonly int start;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal int end = -1;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal AutoRepeatResolver(GridTemplateResolver _enclosing, AutoRepeatValue repeat, int pos) {
                this._enclosing = _enclosing;
                this.repeat = repeat;
                this.start = pos;
            }
//\endcond

            /// <summary>Resolves auto-fit/fill repeat if it was encountered.</summary>
            /// <remarks>
            /// Resolves auto-fit/fill repeat if it was encountered.
            /// If given space is less than 0, only one iteration will be performed.
            /// </remarks>
            /// <param name="leftSpace">space to fit repeat values on</param>
            public virtual void Resolve(float leftSpace) {
                float usedSpace = 0.0f;
                float usedSpacePerIteration = -1.0f;
                int fixedTemplatesCount = this._enclosing.result.Size();
                do {
                    this._enclosing.result.SetInsertPoint(this.start);
                    foreach (GridValue value in this.repeat.GetValues()) {
                        usedSpace += this._enclosing.ProcessValue(value);
                        usedSpace += this._enclosing.gap;
                    }
                    if (usedSpacePerIteration < 0.0f) {
                        usedSpacePerIteration = usedSpace;
                    }
                    if (this._enclosing.containsIntrinsicOrFlexible) {
                        throw new InvalidOperationException(LayoutExceptionMessageConstant.GRID_AUTO_REPEAT_CANNOT_BE_COMBINED_WITH_INDEFINITE_SIZES
                            );
                    }
                }
                while (usedSpace + usedSpacePerIteration <= leftSpace);
                this.end = this.start + this._enclosing.result.Size() - fixedTemplatesCount;
            }

//\cond DO_NOT_DOCUMENT
            /// <summary>Shrinks template list to fit the given size by reducing number of auto-fit/fill repetitions.</summary>
            /// <param name="sizeToFit">size to fit template list</param>
            internal virtual IList<GridValue> ShrinkTemplatesToFitSize(int sizeToFit) {
                // Getting max number of available repetitions
                int allowedRepeatValuesCount = this.GetAllowedRepeatValuesCount(sizeToFit);
                // It could be done with .subList(), but this is not portable on .NET
                IList<GridValue> shrankResult = new List<GridValue>(this._enclosing.result.Size());
                IList<GridValue> previousResult = this._enclosing.result.GetList();
                for (int i = 0; i < this.start; ++i) {
                    shrankResult.Add(previousResult[i]);
                }
                for (int i = 0; i < allowedRepeatValuesCount; ++i) {
                    shrankResult.AddAll(this.repeat.GetValues());
                }
                for (int i = this.end; i < previousResult.Count; ++i) {
                    shrankResult.Add(previousResult[i]);
                }
                this._enclosing.result = new GridTemplateResolver.Result(shrankResult);
                return this._enclosing.result.GetList();
            }
//\endcond

            private int GetAllowedRepeatValuesCount(int sizeToFit) {
                // int division with rounding down
                int allowedRepeatValuesCount = (Math.Min(sizeToFit - this._enclosing.GetFixedValuesCount(), this.end - this
                    .start)) / this.repeat.GetValues().Count * this.repeat.GetValues().Count;
                // if space was indefinite than repeat can be used only once
                if (this._enclosing.space < 0.0f && allowedRepeatValuesCount > 0) {
                    allowedRepeatValuesCount = 1;
                }
                return allowedRepeatValuesCount;
            }

            private readonly GridTemplateResolver _enclosing;
        }

        private class Result {
//\cond DO_NOT_DOCUMENT
            internal readonly IList<GridValue> result;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal int insertPoint = -1;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal bool freeze = false;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal Result(IList<GridValue> result) {
                this.result = result;
            }
//\endcond

            public virtual void AddValue(GridValue value) {
                if (freeze) {
                    return;
                }
                if (insertPoint < 0) {
                    result.Add(value);
                }
                else {
                    result.Add(insertPoint++, value);
                }
            }

            public virtual void SetInsertPoint(int insertPoint) {
                this.insertPoint = insertPoint;
            }

            public virtual int Size() {
                return result.Count;
            }

            public virtual IList<GridValue> GetList() {
                return result;
            }

            public virtual void SetFreeze(bool freeze) {
                this.freeze = freeze;
            }
        }
    }
//\endcond
}
