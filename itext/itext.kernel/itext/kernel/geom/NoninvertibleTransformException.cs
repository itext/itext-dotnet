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

namespace iText.Kernel.Geom {
    /// <summary>NoninvertibleTransformException</summary>
    /// <author>Denis M. Kishenko</author>
    public class NoninvertibleTransformException : Exception {
        public const String DETERMINANT_IS_ZERO_CANNOT_INVERT_TRANSFORMATION = "Determinant is zero. Cannot invert transformation.";

        /// <summary>Creates a new NoninvertibleTransformException.</summary>
        /// <param name="message">the detail message.</param>
        public NoninvertibleTransformException(String message)
            : base(message) {
        }
    }
}
