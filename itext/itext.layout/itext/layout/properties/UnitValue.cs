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
using iText.Commons.Utils;

namespace iText.Layout.Properties {
    /// <summary>A specialized class that holds a value and the unit it is measured in.</summary>
    public class UnitValue {
        public const int POINT = 1;

        public const int PERCENT = 2;

        protected internal int unitType;

        protected internal float value;

        /// <summary>Creates a UnitValue object with a specified type and value.</summary>
        /// <param name="unitType">
        /// either
        /// <see cref="POINT"/>
        /// or a
        /// <see cref="PERCENT"/>
        /// </param>
        /// <param name="value">the value to be stored.</param>
        public UnitValue(int unitType, float value) {
            this.unitType = unitType;
            System.Diagnostics.Debug.Assert(!float.IsNaN(value));
            this.value = value;
        }

        /// <summary>Creates a copy of UnitValue object.</summary>
        /// <param name="unitValue">the value to be stored</param>
        public UnitValue(iText.Layout.Properties.UnitValue unitValue)
            : this(unitValue.unitType, unitValue.value) {
        }

        /// <summary>Creates a UnitValue POINT object with a specified value.</summary>
        /// <param name="value">the value to be stored.</param>
        /// <returns>
        /// a new
        /// <see cref="POINT"/>
        /// 
        /// <see cref="UnitValue"/>
        /// </returns>
        public static iText.Layout.Properties.UnitValue CreatePointValue(float value) {
            return new iText.Layout.Properties.UnitValue(POINT, value);
        }

        /// <summary>Creates a UnitValue PERCENT object with a specified value.</summary>
        /// <param name="value">the value to be stored.</param>
        /// <returns>
        /// a new
        /// <see cref="PERCENT"/>
        /// 
        /// <see cref="UnitValue"/>
        /// </returns>
        public static iText.Layout.Properties.UnitValue CreatePercentValue(float value) {
            return new iText.Layout.Properties.UnitValue(PERCENT, value);
        }

        /// <summary>Creates an array of UnitValue PERCENT objects with specified values.</summary>
        /// <param name="values">the values to be stored.</param>
        /// <returns>
        /// a new normalized (Σ=100%) array of
        /// <see cref="PERCENT"/>
        /// 
        /// <see cref="UnitValue"/>.
        /// </returns>
        public static iText.Layout.Properties.UnitValue[] CreatePercentArray(float[] values) {
            iText.Layout.Properties.UnitValue[] resultArray = new iText.Layout.Properties.UnitValue[values.Length];
            float sum = 0;
            foreach (float val in values) {
                sum += val;
            }
            for (int i = 0; i < values.Length; i++) {
                resultArray[i] = iText.Layout.Properties.UnitValue.CreatePercentValue(100 * values[i] / sum);
            }
            return resultArray;
        }

        /// <summary>Creates an array of UnitValue PERCENT objects with equal values.</summary>
        /// <param name="size">of the resulted array.</param>
        /// <returns>
        /// a array of equal
        /// <see cref="PERCENT"/>
        /// 
        /// <see cref="UnitValue"/>.
        /// </returns>
        public static iText.Layout.Properties.UnitValue[] CreatePercentArray(int size) {
            iText.Layout.Properties.UnitValue[] resultArray = new iText.Layout.Properties.UnitValue[size];
            for (int i = 0; i < size; i++) {
                resultArray[i] = iText.Layout.Properties.UnitValue.CreatePercentValue(100f / size);
            }
            return resultArray;
        }

        /// <summary>Creates an array of UnitValue POINT objects with specified values.</summary>
        /// <param name="values">the values to be stored.</param>
        /// <returns>
        /// a new array of
        /// <see cref="POINT"/>
        /// 
        /// <see cref="UnitValue"/>
        /// </returns>
        public static iText.Layout.Properties.UnitValue[] CreatePointArray(float[] values) {
            iText.Layout.Properties.UnitValue[] resultArray = new iText.Layout.Properties.UnitValue[values.Length];
            for (int i = 0; i < values.Length; i++) {
                resultArray[i] = iText.Layout.Properties.UnitValue.CreatePointValue(values[i]);
            }
            return resultArray;
        }

        /// <summary>Returns the unit this value is stored in, either points (pt) or percent(%)</summary>
        /// <returns>
        /// either 1 for
        /// <see cref="POINT"/>
        /// or 2 for
        /// <see cref="PERCENT"/>
        /// </returns>
        public virtual int GetUnitType() {
            return unitType;
        }

        /// <summary>Sets the unit this value is stored in, either points (pt) or percent(%)</summary>
        /// <param name="unitType">
        /// either
        /// <see cref="POINT"/>
        /// or
        /// <see cref="PERCENT"/>
        /// </param>
        public virtual void SetUnitType(int unitType) {
            this.unitType = unitType;
        }

        /// <summary>Gets the measured value stored in this object</summary>
        /// <returns>the value, as a <c>float</c></returns>
        public virtual float GetValue() {
            return value;
        }

        /// <summary>Sets the measured value stored in this object</summary>
        /// <param name="value">a <c>float</c></param>
        public virtual void SetValue(float value) {
            System.Diagnostics.Debug.Assert(!float.IsNaN(value));
            this.value = value;
        }

        /// <summary>Returns whether or not the value is stored in points (pt)</summary>
        /// <returns><c>true</c> if stored in points</returns>
        public virtual bool IsPointValue() {
            return unitType == POINT;
        }

        /// <summary>Returns whether or not the value is stored in percent (%)</summary>
        /// <returns><c>true</c> if stored in percent</returns>
        public virtual bool IsPercentValue() {
            return unitType == PERCENT;
        }

        public override bool Equals(Object obj) {
            if (GetType() != obj.GetType()) {
                return false;
            }
            iText.Layout.Properties.UnitValue other = (iText.Layout.Properties.UnitValue)obj;
            return JavaUtil.IntegerCompare(unitType, other.unitType) == 0 && JavaUtil.FloatCompare(value, other.value)
                 == 0;
        }

        public override int GetHashCode() {
            int hash = 7;
            hash = 71 * hash + this.unitType;
            hash = 71 * hash + JavaUtil.FloatToIntBits(this.value);
            return hash;
        }

        public override String ToString() {
            return MessageFormatUtil.Format((unitType == PERCENT ? "{0}%" : "{0}pt"), value);
        }
    }
}
