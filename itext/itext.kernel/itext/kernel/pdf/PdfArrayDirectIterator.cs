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
using System.Collections;
using System.Collections.Generic;

namespace iText.Kernel.Pdf {
    internal class PdfArrayDirectIterator : IEnumerator<PdfObject>
    {
        private IEnumerator<PdfObject> parentEnumerator;

        internal PdfArrayDirectIterator(IList<PdfObject> array)
        {
            this.parentEnumerator = array.GetEnumerator();
        }

        public void Dispose()
        {
            parentEnumerator.Dispose();
        }

        public bool MoveNext()
        {
            if (parentEnumerator.MoveNext())
            {
                PdfObject obj = parentEnumerator.Current;
                if (obj.IsIndirectReference())
                {
                    obj = ((PdfIndirectReference)obj).GetRefersTo(true);
                }
                Current = obj;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            parentEnumerator.Reset();
        }

        public PdfObject Current { get; private set; }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
}
