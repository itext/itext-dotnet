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

namespace iText.Commons.Utils.Collections
{
    public static class MapExtensions
    {
        public static V ComputeIfAbsent<K, V>(this IDictionary<K, V> dict, K key, Func<K, V> calculator)
        {
            if (!dict.ContainsKey(key))
            {
                var value = calculator(key);
                dict[key] = value;
                return value;
            }

            return dict[key];
        }
        
        public static V GetOrDefault<K, V>(this IDictionary<K, V> dict, K key, V defaultValue)
        {
            if (!dict.ContainsKey(key))
            {
                return defaultValue;
            }
            return dict[key];
        }
    }
}