/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using iText.Commons.Utils;

namespace iText.Layout.Properties {
    /// <summary>Class to hold background-position property.</summary>
    public class BackgroundPosition {
        private const double EPS = 1e-4F;

        private const int FULL_VALUE = 100;

        private const int HALF_VALUE = 50;

        private BackgroundPosition.PositionX positionX;

        private BackgroundPosition.PositionY positionY;

        private UnitValue xShift;

        private UnitValue yShift;

        /// <summary>
        /// Creates a new
        /// <see cref="BackgroundPosition"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Creates a new
        /// <see cref="BackgroundPosition"/>
        /// instance. Fills it with default values.
        /// </remarks>
        public BackgroundPosition() {
            xShift = new UnitValue(UnitValue.POINT, 0);
            yShift = new UnitValue(UnitValue.POINT, 0);
            positionX = BackgroundPosition.PositionX.LEFT;
            positionY = BackgroundPosition.PositionY.TOP;
        }

        /// <summary>Converts all percentage and enum values to point equivalent.</summary>
        /// <param name="fullWidth">container width to calculate percentage.</param>
        /// <param name="fullHeight">container height to calculate percentage.</param>
        /// <param name="outXValue">
        /// 
        /// <see cref="UnitValue"/>
        /// to store processed xPosition.
        /// </param>
        /// <param name="outYValue">
        /// 
        /// <see cref="UnitValue"/>
        /// to store processed yPosition.
        /// </param>
        public virtual void CalculatePositionValues(float fullWidth, float fullHeight, UnitValue outXValue, UnitValue
             outYValue) {
            int posMultiplier = ParsePositionXToUnitValueAndReturnMultiplier(outXValue);
            if (posMultiplier == 0 && xShift != null && Math.Abs(xShift.GetValue()) > EPS) {
                outXValue.SetValue(0);
            }
            else {
                outXValue.SetValue(CalculateValue(outXValue, fullWidth) + CalculateValue(xShift, fullWidth) * posMultiplier
                    );
            }
            outXValue.SetUnitType(UnitValue.POINT);
            posMultiplier = ParsePositionYToUnitValueAndReturnMultiplier(outYValue);
            if (posMultiplier == 0 && yShift != null && Math.Abs(yShift.GetValue()) > EPS) {
                outYValue.SetValue(0);
            }
            else {
                outYValue.SetValue(CalculateValue(outYValue, fullHeight) + CalculateValue(yShift, fullHeight) * posMultiplier
                    );
            }
            outYValue.SetUnitType(UnitValue.POINT);
        }

        /// <summary>Gets horizontal position.</summary>
        /// <returns>position in x-dimension</returns>
        public virtual BackgroundPosition.PositionX GetPositionX() {
            return positionX;
        }

        /// <summary>Sets horizontal position.</summary>
        /// <param name="xPosition">position in x-dimension</param>
        /// <returns>
        /// 
        /// <see cref="BackgroundPosition"/>
        /// </returns>
        public virtual iText.Layout.Properties.BackgroundPosition SetPositionX(BackgroundPosition.PositionX xPosition
            ) {
            this.positionX = xPosition;
            return this;
        }

        /// <summary>Gets vertical position.</summary>
        /// <returns>position in y-dimension</returns>
        public virtual BackgroundPosition.PositionY GetPositionY() {
            return positionY;
        }

        /// <summary>Sets vertical position.</summary>
        /// <param name="yPosition">position in y-dimension</param>
        /// <returns>
        /// 
        /// <see cref="BackgroundPosition"/>
        /// </returns>
        public virtual iText.Layout.Properties.BackgroundPosition SetPositionY(BackgroundPosition.PositionY yPosition
            ) {
            this.positionY = yPosition;
            return this;
        }

        /// <summary>Gets horizontal shift.</summary>
        /// <returns>shift in x-dimension from left</returns>
        public virtual UnitValue GetXShift() {
            return xShift;
        }

        /// <summary>Sets horizontal shift.</summary>
        /// <param name="xShift">shift in x-dimension from left</param>
        /// <returns>
        /// 
        /// <see cref="BackgroundPosition"/>
        /// </returns>
        public virtual iText.Layout.Properties.BackgroundPosition SetXShift(UnitValue xShift) {
            this.xShift = xShift;
            return this;
        }

        /// <summary>Gets vertical shift.</summary>
        /// <returns>shift in y-dimension from top</returns>
        public virtual UnitValue GetYShift() {
            return yShift;
        }

        /// <summary>Sets vertical shift.</summary>
        /// <param name="yShift">shift in y-dimension</param>
        /// <returns>
        /// 
        /// <see cref="BackgroundPosition"/>
        /// </returns>
        public virtual iText.Layout.Properties.BackgroundPosition SetYShift(UnitValue yShift) {
            this.yShift = yShift;
            return this;
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>true if every field equals. False otherwise.</returns>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Layout.Properties.BackgroundPosition position = (iText.Layout.Properties.BackgroundPosition)o;
            return Object.Equals(positionX, position.positionX) && Object.Equals(positionY, position.positionY) && Object.Equals
                (xShift, position.xShift) && Object.Equals(yShift, position.yShift);
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>object's hashCode.</returns>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode((int)(positionX), (int)(positionY), xShift, (Object)yShift);
        }

        /// <summary>
        /// Parses positionX to
        /// <see cref="UnitValue"/>.
        /// </summary>
        /// <param name="outValue">
        /// 
        /// <see cref="UnitValue"/>
        /// in which positionX will be parsed
        /// </param>
        /// <returns>multiplier by which the xShift will be multiplied</returns>
        private int ParsePositionXToUnitValueAndReturnMultiplier(UnitValue outValue) {
            outValue.SetUnitType(UnitValue.PERCENT);
            switch (positionX) {
                case BackgroundPosition.PositionX.LEFT: {
                    outValue.SetValue(0);
                    return 1;
                }

                case BackgroundPosition.PositionX.RIGHT: {
                    outValue.SetValue(FULL_VALUE);
                    return -1;
                }

                case BackgroundPosition.PositionX.CENTER: {
                    outValue.SetValue(HALF_VALUE);
                    return 0;
                }

                default: {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Parses positionY to
        /// <see cref="UnitValue"/>.
        /// </summary>
        /// <param name="outValue">
        /// 
        /// <see cref="UnitValue"/>
        /// in which positionY will be parsed
        /// </param>
        /// <returns>multiplier by which the yShift will be multiplied</returns>
        private int ParsePositionYToUnitValueAndReturnMultiplier(UnitValue outValue) {
            outValue.SetUnitType(UnitValue.PERCENT);
            switch (positionY) {
                case BackgroundPosition.PositionY.TOP: {
                    outValue.SetValue(0);
                    return 1;
                }

                case BackgroundPosition.PositionY.BOTTOM: {
                    outValue.SetValue(FULL_VALUE);
                    return -1;
                }

                case BackgroundPosition.PositionY.CENTER: {
                    outValue.SetValue(HALF_VALUE);
                    return 0;
                }

                default: {
                    return 0;
                }
            }
        }

        private static float CalculateValue(UnitValue value, float fullValue) {
            if (value == null) {
                return 0;
            }
            return value.IsPercentValue() ? (value.GetValue() / 100 * fullValue) : value.GetValue();
        }

        /// <summary>A specialized enum containing positions in x-dimension (horizontal positions).</summary>
        public enum PositionX {
            LEFT,
            RIGHT,
            CENTER
        }

        /// <summary>A specialized enum containing positions in y-dimension (vertical positions).</summary>
        public enum PositionY {
            TOP,
            BOTTOM,
            CENTER
        }
    }
}
