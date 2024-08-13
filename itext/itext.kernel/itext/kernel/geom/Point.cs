/*
*  Licensed to the Apache Software Foundation (ASF) under one or more
*  contributor license agreements.  See the NOTICE file distributed with
*  this work for additional information regarding copyright ownership.
*  The ASF licenses this file to You under the Apache License, Version 2.0
*  (the "License"); you may not use this file except in compliance with
*  the License.  You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
*  Unless required by applicable law or agreed to in writing, software
*  distributed under the License is distributed on an "AS IS" BASIS,
*  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*  See the License for the specific language governing permissions and
*  limitations under the License.
*
*  This code was originally part of the Apache Harmony project.
*  The Apache Harmony project has been discontinued.
*  That's why we imported the code into iText.
*/
using System;
using iText.Commons.Utils;
using iText.IO.Util;

namespace iText.Kernel.Geom {
    /// <summary>Class that represent point object with x and y coordinates.</summary>
    public class Point
#if !NETSTANDARD2_0
 : ICloneable
#endif
 {
        private double x;

        private double y;

        /// <summary>
        /// Instantiates a new
        /// <see cref="Point"/>
        /// instance with 0 x and y.
        /// </summary>
        public Point() {
            SetLocation(0, 0);
        }

        /// <summary>
        /// Instantiates a new
        /// <see cref="Point"/>
        /// instance based on passed x and y.
        /// </summary>
        /// <param name="x">the x coordinates of the point</param>
        /// <param name="y">the y coordinates of the point</param>
        public Point(double x, double y) {
            SetLocation(x, y);
        }

        /// <summary>
        /// Instantiates a new
        /// <see cref="Point"/>
        /// instance based on another point.
        /// </summary>
        /// <param name="p">the point which will be copied</param>
        public Point(iText.Kernel.Geom.Point p) {
            SetLocation(p.x, p.y);
        }

        /// <summary>Gets x coordinate of the point.</summary>
        /// <returns>the x coordinate</returns>
        public virtual double GetX() {
            return x;
        }

        /// <summary>Gets y coordinate of the point.</summary>
        /// <returns>the y coordinate</returns>
        public virtual double GetY() {
            return y;
        }

        /// <summary>Gets location of point by creating a new copy.</summary>
        /// <returns>the copy of this point</returns>
        public virtual iText.Kernel.Geom.Point GetLocation() {
            return new iText.Kernel.Geom.Point(x, y);
        }

        /// <summary>Sets x and y double coordinates of the point.</summary>
        /// <param name="x">the x coordinate</param>
        /// <param name="y">the y coordinate</param>
        public virtual void SetLocation(double x, double y) {
            this.x = x;
            this.y = y;
        }

        /// <summary>Moves the point by the specified offset.</summary>
        /// <param name="dx">the x-axis offset</param>
        /// <param name="dy">the y-axis offset</param>
        public virtual void Move(double dx, double dy) {
            x += dx;
            y += dy;
        }

        /// <summary>The distance between this point and the second point which is defined by passed x and y coordinates.
        ///     </summary>
        /// <param name="px">the x coordinate of the second point</param>
        /// <param name="py">the y coordinate of the second point</param>
        /// <returns>the distance between points</returns>
        public virtual double Distance(double px, double py) {
            return Math.Sqrt(DistanceSq(GetX(), GetY(), px, py));
        }

        /// <summary>The distance between this point and the second point.</summary>
        /// <param name="p">the second point to calculate distance</param>
        /// <returns>the distance between points</returns>
        public virtual double Distance(iText.Kernel.Geom.Point p) {
            return Distance(p.GetX(), p.GetY());
        }

        public override bool Equals(Object obj) {
            if (obj == this) {
                return true;
            }
            if (obj is iText.Kernel.Geom.Point) {
                iText.Kernel.Geom.Point p = (iText.Kernel.Geom.Point)obj;
                return x == p.x && y == p.y;
            }
            return false;
        }

        public override String ToString() {
            return MessageFormatUtil.Format("Point: [x={0},y={1}]", x, y);
        }

        public override int GetHashCode() {
            HashCode hash = new HashCode();
            hash.Append(GetX());
            hash.Append(GetY());
            return hash.GetHashCode();
        }

        public virtual Object Clone() {
            return new iText.Kernel.Geom.Point(x, y);
        }

        private static double DistanceSq(double x1, double y1, double x2, double y2) {
            x2 -= x1;
            y2 -= y1;
            return x2 * x2 + y2 * y2;
        }
    }
}
