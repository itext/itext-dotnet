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
﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace iText.Commons.Utils.Collections
{
    public class EnumSet<T> : SortedSet<T> where T : System.Enum
    {
        public static EnumSet<TE> Of<TE>(TE first, params TE[] elements)  where  TE : Enum
        {
            var set = new EnumSet<TE>();
            set.Add(first);
            set.AddAll(elements);
            return set;
        }

        public static EnumSet<TE> AllOf<TE>()  where  TE : Enum
        {
            var set = new EnumSet<TE>();
            foreach (var item in Enum.GetValues( typeof(TE)))
            {
                set.Add((TE) item);    
            }
            return set;
        }

        public static EnumSet<TE> ComplementOf<TE>(EnumSet<TE> other) where  TE : Enum
        {
            var set = new EnumSet<TE>();
            set.AddAll(AllOf<TE>().Except(other));
            return set;
        }
    }
}