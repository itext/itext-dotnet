/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using System.Collections;
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Utils;

namespace iText.Kernel.Pdf {
    /// <summary>A representation of an array as described in the PDF specification.</summary>
    /// <remarks>
    /// A representation of an array as described in the PDF specification. A PdfArray can contain any
    /// subclass of
    /// <see cref="PdfObject"/>.
    /// </remarks>
    public class PdfArray : PdfObject, IEnumerable<PdfObject> {
        protected internal IList<PdfObject> list;

        /// <summary>Create a new, empty PdfArray.</summary>
        public PdfArray()
            : base() {
            list = new List<PdfObject>();
        }

        /// <summary>
        /// Create a new PdfArray with the provided PdfObject as the first item in the
        /// array.
        /// </summary>
        /// <param name="obj">first item in the array</param>
        public PdfArray(PdfObject obj)
            : this() {
            list.Add(obj);
        }

        /// <summary>Create a new PdfArray.</summary>
        /// <remarks>Create a new PdfArray. The array is filled with the items of the provided PdfArray.</remarks>
        /// <param name="arr">PdfArray containing items that will added to this PdfArray</param>
        public PdfArray(iText.Kernel.Pdf.PdfArray arr)
            : this() {
            list.AddAll(arr.list);
        }

        /// <summary>Create a new PdfArray.</summary>
        /// <remarks>
        /// Create a new PdfArray. The array is filled with the four values of the Rectangle in the
        /// following order: left, bottom, right, top.
        /// </remarks>
        /// <param name="rectangle">Rectangle whose 4 values will be added to the PdfArray</param>
        public PdfArray(Rectangle rectangle) {
            list = new List<PdfObject>(4);
            Add(new PdfNumber(rectangle.GetLeft()));
            Add(new PdfNumber(rectangle.GetBottom()));
            Add(new PdfNumber(rectangle.GetRight()));
            Add(new PdfNumber(rectangle.GetTop()));
        }

        /// <summary>Create a new PdfArray.</summary>
        /// <remarks>Create a new PdfArray. The PdfObjects in the list will be added to the PdfArray.</remarks>
        /// <param name="objects">List of PdfObjects to be added to this PdfArray</param>
        public PdfArray(IList<PdfObject> objects) {
            list = new List<PdfObject>(objects.Count);
            foreach (PdfObject element in objects) {
                Add(element);
            }
        }

        /// <summary>
        /// Create a new PdfArray filled with the values in the float[] as
        /// <see cref="PdfNumber"/>.
        /// </summary>
        /// <param name="numbers">values to be added to this PdfArray</param>
        public PdfArray(float[] numbers) {
            list = new List<PdfObject>(numbers.Length);
            foreach (float f in numbers) {
                list.Add(new PdfNumber(f));
            }
        }

        /// <summary>
        /// Create a new PdfArray filled with the values in the double[] as
        /// <see cref="PdfNumber"/>.
        /// </summary>
        /// <param name="numbers">values to be added to this PdfArray</param>
        public PdfArray(double[] numbers) {
            list = new List<PdfObject>(numbers.Length);
            foreach (double f in numbers) {
                list.Add(new PdfNumber(f));
            }
        }

        /// <summary>
        /// Create a new PdfArray filled with the values in the int[] as
        /// <see cref="PdfNumber"/>.
        /// </summary>
        /// <param name="numbers">values to be added to this PdfArray</param>
        public PdfArray(int[] numbers) {
            list = new List<PdfObject>(numbers.Length);
            foreach (float i in numbers) {
                list.Add(new PdfNumber(i));
            }
        }

        /// <summary>
        /// Create a new PdfArray filled with the values in the boolean[] as
        /// <see cref="PdfBoolean"/>.
        /// </summary>
        /// <param name="values">values to be added to this PdfArray</param>
        public PdfArray(bool[] values) {
            list = new List<PdfObject>(values.Length);
            foreach (bool b in values) {
                list.Add(PdfBoolean.ValueOf(b));
            }
        }

        /// <summary>Create a new PdfArray filled with a list of Strings.</summary>
        /// <remarks>
        /// Create a new PdfArray filled with a list of Strings. The boolean value decides if the Strings
        /// should be added as
        /// <see cref="PdfName"/>
        /// (true) or as
        /// <see cref="PdfString"/>
        /// (false).
        /// </remarks>
        /// <param name="strings">list of strings to be added to the list</param>
        /// <param name="asNames">indicates whether the strings should be added as PdfName (true) or as PdfString (false)
        ///     </param>
        public PdfArray(IList<String> strings, bool asNames) {
            list = new List<PdfObject>(strings.Count);
            foreach (String s in strings) {
                list.Add(asNames ? (PdfObject)new PdfName(s) : new PdfString(s));
            }
        }

        /// <summary>Create a new PdfArray.</summary>
        /// <remarks>Create a new PdfArray. The PdfObjects in the iterable object will be added to the PdfArray.</remarks>
        /// <param name="objects">List of PdfObjects to be added to this PdfArray</param>
        /// <param name="initialCapacity">Initial capacity of this PdfArray</param>
        public PdfArray(IEnumerable<PdfObject> objects, int initialCapacity) {
            list = new List<PdfObject>(initialCapacity);
            foreach (PdfObject element in objects) {
                Add(element);
            }
        }

        public virtual int Size() {
            return list.Count;
        }

        public virtual bool IsEmpty() {
            return list.Count == 0;
        }

        public virtual bool Contains(PdfObject o) {
            if (list.Contains(o)) {
                return true;
            }
            if (o == null) {
                return false;
            }
            foreach (PdfObject pdfObject in this) {
                if (PdfObject.EqualContent(o, pdfObject)) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>Returns an iterator over an array of PdfObject elements.</summary>
        /// <remarks>
        /// Returns an iterator over an array of PdfObject elements.
        /// <br />
        /// <b>NOTE:</b> since 7.0.1 it returns collection of direct objects.
        /// If you want to get
        /// <see cref="PdfIndirectReference"/>
        /// instances for the indirect objects value,
        /// you shall use
        /// <see cref="Get(int, bool)"/>
        /// method.
        /// </remarks>
        /// <returns>an Iterator.</returns>
        public virtual IEnumerator<PdfObject> GetEnumerator() {
            return new PdfArrayDirectIterator(list);
        }

        public virtual void Add(PdfObject pdfObject) {
            list.Add(pdfObject);
        }

        /// <summary>Adds the specified PdfObject at the specified index.</summary>
        /// <remarks>Adds the specified PdfObject at the specified index. All objects after this index will be shifted by 1.
        ///     </remarks>
        /// <param name="index">position to insert the PdfObject</param>
        /// <param name="element">PdfObject to be added</param>
        /// <seealso cref="System.Collections.IList{E}.Add(int, System.Object)"/>
        public virtual void Add(int index, PdfObject element) {
            list.Add(index, element);
        }

        /// <summary>Sets the PdfObject at the specified index in the PdfArray.</summary>
        /// <param name="index">the position to set the PdfObject</param>
        /// <param name="element">PdfObject to be added</param>
        /// <returns>true if the operation changed the PdfArray</returns>
        /// <seealso cref="System.Collections.IList{E}.Set(int, System.Object)"/>
        public virtual PdfObject Set(int index, PdfObject element) {
            return list[index] = element;
        }

        /// <summary>Adds the Collection of PdfObjects.</summary>
        /// <param name="c">the Collection of PdfObjects to be added</param>
        /// <seealso cref="System.Collections.IList{E}.AddAll(System.Collections.ICollection{E})"/>
        public virtual void AddAll(ICollection<PdfObject> c) {
            list.AddAll(c);
        }

        /// <summary>
        /// Adds content of the
        /// <c>PdfArray</c>.
        /// </summary>
        /// <param name="a">
        /// the
        /// <c>PdfArray</c>
        /// to be added
        /// </param>
        /// <seealso cref="System.Collections.IList{E}.AddAll(System.Collections.ICollection{E})"/>
        public virtual void AddAll(iText.Kernel.Pdf.PdfArray a) {
            if (a != null) {
                AddAll(a.list);
            }
        }

        /// <summary>Gets the (direct) PdfObject at the specified index.</summary>
        /// <param name="index">index of the PdfObject in the PdfArray</param>
        /// <returns>the PdfObject at the position in the PdfArray</returns>
        public virtual PdfObject Get(int index) {
            return Get(index, true);
        }

        /// <summary>Removes the PdfObject at the specified index.</summary>
        /// <param name="index">position of the PdfObject to be removed</param>
        /// <seealso cref="System.Collections.IList{E}.JRemoveAt(int)"/>
        public virtual void Remove(int index) {
            list.JRemoveAt(index);
        }

        /// <summary>Removes the first occurrence of the specified PdfObject, if it is present.</summary>
        /// <param name="o">a PdfObject to be removed</param>
        /// <seealso cref="System.Collections.IList{E}.Remove(System.Object)"/>
        public virtual void Remove(PdfObject o) {
            if (list.Remove(o)) {
                return;
            }
            if (o == null) {
                return;
            }
            foreach (PdfObject pdfObject in list) {
                if (PdfObject.EqualContent(o, pdfObject)) {
                    list.Remove(pdfObject);
                    break;
                }
            }
        }

        public virtual void Clear() {
            list.Clear();
        }

        /// <summary>Gets the first index of the specified PdfObject.</summary>
        /// <param name="o">PdfObject to find the index of</param>
        /// <returns>index of the PdfObject</returns>
        /// <seealso cref="System.Collections.IList{E}.IndexOf(System.Object)"/>
        public virtual int IndexOf(PdfObject o) {
            if (o == null) {
                return list.IndexOf(null);
            }
            int index = 0;
            foreach (PdfObject pdfObject in this) {
                if (PdfObject.EqualContent(o, pdfObject)) {
                    return index;
                }
                index++;
            }
            return -1;
        }

        /// <summary>Returns a sublist of this PdfArray, starting at fromIndex (inclusive) and ending at toIndex (exclusive).
        ///     </summary>
        /// <param name="fromIndex">the position of the first element in the sublist (inclusive)</param>
        /// <param name="toIndex">the position of the last element in the sublist (exclusive)</param>
        /// <returns>List of PdfObjects</returns>
        /// <seealso cref="System.Collections.IList{E}.SubList(int, int)"/>
        public virtual IList<PdfObject> SubList(int fromIndex, int toIndex) {
            return list.SubList(fromIndex, toIndex);
        }

        /// <summary>
        /// Returns unmodifiable
        /// <see cref="System.Collections.IList{E}"/>
        /// representation of this PdfArray.
        /// </summary>
        /// <returns>
        /// unmodifiable
        /// <see cref="System.Collections.IList{E}"/>
        /// representation of this PdfArray
        /// </returns>
        public virtual IList<PdfObject> ToList() {
            return JavaCollectionsUtil.UnmodifiableList(list);
        }

        public override byte GetObjectType() {
            return ARRAY;
        }

        public override String ToString() {
            String @string = "[";
            foreach (PdfObject entry in list) {
                PdfIndirectReference indirectReference = entry.GetIndirectReference();
                @string = @string + (indirectReference == null ? entry.ToString() : indirectReference.ToString()) + " ";
            }
            @string += "]";
            return @string;
        }

        /// <param name="asDirect">true is to extract direct object always.</param>
        /// <param name="index">index of the element to return</param>
        /// <returns>the element at the specified position in this list</returns>
        public virtual PdfObject Get(int index, bool asDirect) {
            if (!asDirect) {
                return list[index];
            }
            else {
                PdfObject obj = list[index];
                if (obj != null && obj.GetObjectType() == INDIRECT_REFERENCE) {
                    return ((PdfIndirectReference)obj).GetRefersTo(true);
                }
                else {
                    return obj;
                }
            }
        }

        /// <summary>Returns the element at the specified index as a PdfArray.</summary>
        /// <remarks>Returns the element at the specified index as a PdfArray. If the element isn't a PdfArray, null is returned.
        ///     </remarks>
        /// <param name="index">position of the element to be returned</param>
        /// <returns>the element at the index as a PdfArray</returns>
        public virtual iText.Kernel.Pdf.PdfArray GetAsArray(int index) {
            PdfObject direct = Get(index, true);
            if (direct != null && direct.GetObjectType() == PdfObject.ARRAY) {
                return (iText.Kernel.Pdf.PdfArray)direct;
            }
            return null;
        }

        /// <summary>Returns the element at the specified index as a PdfDictionary.</summary>
        /// <remarks>Returns the element at the specified index as a PdfDictionary. If the element isn't a PdfDictionary, null is returned.
        ///     </remarks>
        /// <param name="index">position of the element to be returned</param>
        /// <returns>the element at the index as a PdfDictionary</returns>
        public virtual PdfDictionary GetAsDictionary(int index) {
            PdfObject direct = Get(index, true);
            if (direct != null && direct.GetObjectType() == PdfObject.DICTIONARY) {
                return (PdfDictionary)direct;
            }
            return null;
        }

        /// <summary>Returns the element at the specified index as a PdfStream.</summary>
        /// <remarks>Returns the element at the specified index as a PdfStream. If the element isn't a PdfStream, null is returned.
        ///     </remarks>
        /// <param name="index">position of the element to be returned</param>
        /// <returns>the element at the index as a PdfStream</returns>
        public virtual PdfStream GetAsStream(int index) {
            PdfObject direct = Get(index, true);
            if (direct != null && direct.GetObjectType() == PdfObject.STREAM) {
                return (PdfStream)direct;
            }
            return null;
        }

        /// <summary>Returns the element at the specified index as a PdfNumber.</summary>
        /// <remarks>Returns the element at the specified index as a PdfNumber. If the element isn't a PdfNumber, null is returned.
        ///     </remarks>
        /// <param name="index">position of the element to be returned</param>
        /// <returns>the element at the index as a PdfNumber</returns>
        public virtual PdfNumber GetAsNumber(int index) {
            PdfObject direct = Get(index, true);
            if (direct != null && direct.GetObjectType() == PdfObject.NUMBER) {
                return (PdfNumber)direct;
            }
            return null;
        }

        /// <summary>Returns the element at the specified index as a PdfName.</summary>
        /// <remarks>Returns the element at the specified index as a PdfName. If the element isn't a PdfName, null is returned.
        ///     </remarks>
        /// <param name="index">position of the element to be returned</param>
        /// <returns>the element at the index as a PdfName</returns>
        public virtual PdfName GetAsName(int index) {
            PdfObject direct = Get(index, true);
            if (direct != null && direct.GetObjectType() == PdfObject.NAME) {
                return (PdfName)direct;
            }
            return null;
        }

        /// <summary>Returns the element at the specified index as a PdfString.</summary>
        /// <remarks>Returns the element at the specified index as a PdfString. If the element isn't a PdfString, null is returned.
        ///     </remarks>
        /// <param name="index">position of the element to be returned</param>
        /// <returns>the element at the index as a PdfString</returns>
        public virtual PdfString GetAsString(int index) {
            PdfObject direct = Get(index, true);
            if (direct != null && direct.GetObjectType() == PdfObject.STRING) {
                return (PdfString)direct;
            }
            return null;
        }

        /// <summary>Returns the element at the specified index as a PdfBoolean.</summary>
        /// <remarks>Returns the element at the specified index as a PdfBoolean. If the element isn't a PdfBoolean, null is returned.
        ///     </remarks>
        /// <param name="index">position of the element to be returned</param>
        /// <returns>the element at the index as a PdfBoolean</returns>
        public virtual PdfBoolean GetAsBoolean(int index) {
            PdfObject direct = Get(index, true);
            if (direct != null && direct.GetObjectType() == PdfObject.BOOLEAN) {
                return (PdfBoolean)direct;
            }
            return null;
        }

        /// <summary>Returns the first four elements of this array as a PdfArray.</summary>
        /// <remarks>
        /// Returns the first four elements of this array as a PdfArray. The first four values need to be
        /// PdfNumbers, if not a PdfException will be thrown.
        /// </remarks>
        /// <returns>Rectangle of the first four values</returns>
        public virtual Rectangle ToRectangle() {
            try {
                float x1 = GetAsNumber(0).FloatValue();
                float y1 = GetAsNumber(1).FloatValue();
                float x2 = GetAsNumber(2).FloatValue();
                float y2 = GetAsNumber(3).FloatValue();
                float llx;
                float lly;
                float urx;
                float ury;
                //Find the lower-left and upper-right of these 4 points
                llx = Math.Min(x1, x2);
                lly = Math.Min(y1, y2);
                urx = Math.Max(x1, x2);
                ury = Math.Max(y1, y2);
                return new Rectangle(llx, lly, urx - llx, ury - lly);
            }
            catch (Exception e) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_CONVERT_PDF_ARRAY_TO_RECTANGLE, e, this);
            }
        }

        /// <summary>Returns this array as an array of floats.</summary>
        /// <remarks>Returns this array as an array of floats. Will throw a PdfException when it encounters an issue.</remarks>
        /// <returns>this array as an array of floats</returns>
        public virtual float[] ToFloatArray() {
            try {
                float[] rslt = new float[Size()];
                for (int k = 0; k < rslt.Length; ++k) {
                    rslt[k] = GetAsNumber(k).FloatValue();
                }
                return rslt;
            }
            catch (Exception e) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_CONVERT_PDF_ARRAY_TO_FLOAT_ARRAY, e, this);
            }
        }

        /// <summary>Returns this array as an array of doubles.</summary>
        /// <remarks>Returns this array as an array of doubles. Will throw a PdfException when it encounters an issue.
        ///     </remarks>
        /// <returns>this array as an array of doubles</returns>
        public virtual double[] ToDoubleArray() {
            try {
                double[] rslt = new double[Size()];
                for (int k = 0; k < rslt.Length; ++k) {
                    rslt[k] = GetAsNumber(k).DoubleValue();
                }
                return rslt;
            }
            catch (Exception e) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_CONVERT_PDF_ARRAY_TO_DOUBLE_ARRAY, e, this);
            }
        }

        /// <summary>Returns this array as an array of longs.</summary>
        /// <remarks>Returns this array as an array of longs. Will throw a PdfException when it encounters an issue.</remarks>
        /// <returns>this array as an array of longs</returns>
        public virtual long[] ToLongArray() {
            try {
                long[] rslt = new long[Size()];
                for (int k = 0; k < rslt.Length; ++k) {
                    rslt[k] = GetAsNumber(k).LongValue();
                }
                return rslt;
            }
            catch (Exception e) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_CONVERT_PDF_ARRAY_TO_LONG_ARRAY, e, this);
            }
        }

        /// <summary>Returns this array as an array of ints.</summary>
        /// <remarks>Returns this array as an array of ints. Will throw a PdfException when it encounters an issue.</remarks>
        /// <returns>this array as an array of ints</returns>
        public virtual int[] ToIntArray() {
            try {
                int[] rslt = new int[Size()];
                for (int k = 0; k < rslt.Length; ++k) {
                    rslt[k] = GetAsNumber(k).IntValue();
                }
                return rslt;
            }
            catch (Exception e) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_CONVERT_PDF_ARRAY_TO_INT_ARRAY, e, this);
            }
        }

        /// <summary>Returns this array as an array of booleans.</summary>
        /// <remarks>Returns this array as an array of booleans. Will throw a PdfException when it encounters an issue.
        ///     </remarks>
        /// <returns>this array as an array of booleans</returns>
        public virtual bool[] ToBooleanArray() {
            bool[] rslt = new bool[Size()];
            PdfBoolean tmp;
            for (int k = 0; k < rslt.Length; ++k) {
                tmp = GetAsBoolean(k);
                if (tmp == null) {
                    throw new PdfException(KernelExceptionMessageConstant.CANNOT_CONVERT_PDF_ARRAY_TO_AN_ARRAY_OF_BOOLEANS, this
                        );
                }
                rslt[k] = tmp.GetValue();
            }
            return rslt;
        }

        protected internal override PdfObject NewInstance() {
            return new iText.Kernel.Pdf.PdfArray();
        }

        protected internal override void CopyContent(PdfObject from, PdfDocument document, ICopyFilter copyFilter) {
            base.CopyContent(from, document, copyFilter);
            iText.Kernel.Pdf.PdfArray array = (iText.Kernel.Pdf.PdfArray)from;
            foreach (PdfObject entry in array.list) {
                if (copyFilter.ShouldProcess(this, null, entry)) {
                    Add(entry.ProcessCopying(document, false, copyFilter));
                }
            }
        }

        /// <summary>Release content of PdfArray.</summary>
        protected internal virtual void ReleaseContent() {
            list = null;
        }

        /// <summary><inheritDoc/></summary>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

    }
}
