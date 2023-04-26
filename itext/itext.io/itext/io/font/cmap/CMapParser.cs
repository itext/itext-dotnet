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
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.IO.Source;

namespace iText.IO.Font.Cmap {
    public class CMapParser {
        private const String def = "def";

        private const String endcidrange = "endcidrange";

        private const String endcidchar = "endcidchar";

        private const String endbfrange = "endbfrange";

        private const String endbfchar = "endbfchar";

        private const String endcodespacerange = "endcodespacerange";

        private const String usecmap = "usecmap";

        private const String Registry = "Registry";

        private const String Ordering = "Ordering";

        private const String Supplement = "Supplement";

        private const String CMapName = "CMapName";

        private const int MAX_LEVEL = 10;

        public static void ParseCid(String cmapName, AbstractCMap cmap, ICMapLocation location) {
            ParseCid(cmapName, cmap, location, 0);
        }

        private static void ParseCid(String cmapName, AbstractCMap cmap, ICMapLocation location, int level) {
            if (level >= MAX_LEVEL) {
                return;
            }
            PdfTokenizer inp = location.GetLocation(cmapName);
            try {
                IList<CMapObject> list = new List<CMapObject>();
                CMapContentParser cp = new CMapContentParser(inp);
                int maxExc = 50;
                while (true) {
                    try {
                        cp.Parse(list);
                    }
                    catch (Exception) {
                        if (--maxExc < 0) {
                            break;
                        }
                        continue;
                    }
                    if (list.Count == 0) {
                        break;
                    }
                    String last = list[list.Count - 1].ToString();
                    if (level == 0 && list.Count == 3 && last.Equals(def)) {
                        CMapObject cmapObject = list[0];
                        if (Registry.Equals(cmapObject.ToString())) {
                            cmap.SetRegistry(list[1].ToString());
                        }
                        else {
                            if (Ordering.Equals(cmapObject.ToString())) {
                                cmap.SetOrdering(list[1].ToString());
                            }
                            else {
                                if (CMapName.Equals(cmapObject.ToString())) {
                                    cmap.SetName(list[1].ToString());
                                }
                                else {
                                    if (Supplement.Equals(cmapObject.ToString())) {
                                        try {
                                            cmap.SetSupplement((int)list[1].GetValue());
                                        }
                                        catch (Exception) {
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else {
                        if ((last.Equals(endcidchar) || last.Equals(endbfchar)) && list.Count >= 3) {
                            int lMax = list.Count - 2;
                            for (int k = 0; k < lMax; k += 2) {
                                if (list[k].IsString()) {
                                    cmap.AddChar(list[k].ToString(), list[k + 1]);
                                }
                            }
                        }
                        else {
                            if ((last.Equals(endcidrange) || last.Equals(endbfrange)) && list.Count >= 4) {
                                int lMax = list.Count - 3;
                                for (int k = 0; k < lMax; k += 3) {
                                    if (list[k].IsString() && list[k + 1].IsString()) {
                                        cmap.AddRange(list[k].ToString(), list[k + 1].ToString(), list[k + 2]);
                                    }
                                }
                            }
                            else {
                                if (last.Equals(usecmap) && list.Count == 2 && list[0].IsName()) {
                                    ParseCid(list[0].ToString(), cmap, location, level + 1);
                                }
                                else {
                                    if (last.Equals(endcodespacerange)) {
                                        for (int i = 0; i < list.Count + 1; i += 2) {
                                            if (list[i].IsHexString() && list[i + 1].IsHexString()) {
                                                byte[] low = list[i].ToHexByteArray();
                                                byte[] high = list[i + 1].ToHexByteArray();
                                                cmap.AddCodeSpaceRange(low, high);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception) {
                ILogger logger = ITextLogManager.GetLogger(typeof(CMapParser));
                logger.LogError(iText.IO.Logs.IoLogMessageConstant.UNKNOWN_ERROR_WHILE_PROCESSING_CMAP);
            }
            finally {
                inp.Close();
            }
        }
        //    private static void encodeSequence(int size, byte[] seqs, char cid, ArrayList<char[]> planes) {
        //        --size;
        //        int nextPlane = 0;
        //        for (int idx = 0; idx < size; ++idx) {
        //            char[] plane = planes.get(nextPlane);
        //            int one = seqs[idx] & 0xff;
        //            char c = plane[one];
        //            if (c != 0 && (c & 0x8000) == 0)
        //                throw new PdfRuntimeException("inconsistent.mapping");
        //            if (c == 0) {
        //                planes.add(new char[256]);
        //                c = (char) (planes.size() - 1 | 0x8000);
        //                plane[one] = c;
        //            }
        //            nextPlane = c & 0x7fff;
        //        }
        //        char[] plane = planes.get(nextPlane);
        //        int one = seqs[size] & 0xff;
        //        char c = plane[one];
        //        if ((c & 0x8000) != 0)
        //            throw new PdfRuntimeException("inconsistent.mapping");
        //        plane[one] = cid;
        //    }
    }
}
