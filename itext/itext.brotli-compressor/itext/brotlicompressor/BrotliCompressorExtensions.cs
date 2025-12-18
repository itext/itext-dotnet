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
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using iText.Commons.Utils;

//\cond DO_NOT_DOCUMENT
internal static class BrotliCompressorExtensions
{
    public static byte[] GetBytes(this String str) {
        return System.Text.Encoding.UTF8.GetBytes(str);
    }

    public static byte[] GetBytes(this String str, String encoding) {
        return EncodingUtil.GetEncoding(encoding).GetBytes(str);
    }

    public static byte[] GetBytes(this String str, Encoding encoding) {
        return encoding.GetBytes(str);
    }

    public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> col, TKey key)
    {
        TValue value = default(TValue);
        if (key != null)
        {
            col.TryGetValue(key, out value);
        }

        return value;
    }

    public static TValue Put<TKey, TValue>(this IDictionary<TKey, TValue> col, TKey key, TValue value)
    {
        TValue oldVal = col.Get(key);
        col[key] = value;
        return oldVal;
    }

    public static TValue Get<TKey, TValue>(this ConditionalWeakTable<TKey, TValue> table, TKey key) where
        TKey : class
        where TValue : class
    {
        TValue value = default(TValue);
        if (key != null)
        {
            table.TryGetValue(key, out value);
        }

        return value;
    }

    public static TValue Put<TKey, TValue>(this ConditionalWeakTable<TKey, TValue> table, TKey key, TValue value)
        where TKey : class where TValue : class
    {
        TValue oldVal = table.Get(key);
        if (oldVal != null)
        {
            table.Remove(key);
        }

        table.Add(key, value);
        return oldVal;
    }
}
//\endcond