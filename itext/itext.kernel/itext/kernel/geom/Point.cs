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
using iText.IO.Util;

namespace iText.Kernel.Geom {
    public class Point
#if !NETSTANDARD1_6
 : ICloneable
#endif
 {
        public double x;

        public double y;

        public Point() {
            SetLocation(0, 0);
        }

        public Point(int x, int y) {
            SetLocation(x, y);
        }

        public Point(double x, double y) {
            SetLocation(x, y);
        }

        public Point(iText.Kernel.Geom.Point p) {
            SetLocation(p.x, p.y);
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
            return String.Format("Point: [x={0},y={1}]", x, y);
        }

        //$NON-NLS-1$ //$NON-NLS-2$ //$NON-NLS-3$
        public virtual double GetX() {
            return x;
        }

        public virtual double GetY() {
            return y;
        }

        public virtual iText.Kernel.Geom.Point GetLocation() {
            return new iText.Kernel.Geom.Point(x, y);
        }

        public virtual void SetLocation(iText.Kernel.Geom.Point p) {
            SetLocation(p.x, p.y);
        }

        public virtual void SetLocation(int x, int y) {
            SetLocation((double)x, (double)y);
        }

        public virtual void SetLocation(double x, double y) {
            this.x = x;
            this.y = y;
        }

        public virtual void Move(double x, double y) {
            SetLocation(x, y);
        }

        public virtual void Translate(double dx, double dy) {
            x += dx;
            y += dy;
        }

        public override int GetHashCode() {
            HashCode hash = new HashCode();
            hash.Append(GetX());
            hash.Append(GetY());
            return hash.GetHashCode();
        }

        public static double DistanceSq(double x1, double y1, double x2, double y2) {
            x2 -= x1;
            y2 -= y1;
            return x2 * x2 + y2 * y2;
        }

        public virtual double DistanceSq(double px, double py) {
            return DistanceSq(GetX(), GetY(), px, py);
        }

        public virtual double DistanceSq(iText.Kernel.Geom.Point p) {
            return DistanceSq(GetX(), GetY(), p.GetX(), p.GetY());
        }

        public static double Distance(double x1, double y1, double x2, double y2) {
            return Math.Sqrt(DistanceSq(x1, y1, x2, y2));
        }

        public virtual double Distance(double px, double py) {
            return Math.Sqrt(DistanceSq(px, py));
        }

        public virtual double Distance(iText.Kernel.Geom.Point p) {
            return Math.Sqrt(DistanceSq(p));
        }

        public virtual Object Clone() {
            return new iText.Kernel.Geom.Point(x, y);
        }
    }
}
