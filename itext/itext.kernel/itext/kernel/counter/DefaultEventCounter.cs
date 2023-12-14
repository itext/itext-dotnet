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
using Common.Logging;
using iText.IO.Codec;
using iText.IO.Util;
using iText.Kernel.Counter.Context;
using iText.Kernel.Counter.Event;

namespace iText.Kernel.Counter {
    /// <summary>
    /// Default implementation of the
    /// <see cref="EventCounter"/>
    /// interface that essentially doesn't do anything.
    /// </summary>
    public class DefaultEventCounter : EventCounter {
        private static readonly int[] REPEAT = new int[] { 10000, 5000, 1000 };

        private readonly AtomicLong count = new AtomicLong();

        private int level = 0;

        private int repeatLevel = 10000;

        private ILog logger;

        public DefaultEventCounter()
            : base(UnknownContext.RESTRICTIVE) {
        }

        private static byte[] message_1 = Convert.FromBase64String("DQoNCllvdSBhcmUgdXNpbmcgaVRleHQgdW5kZXIgdGhlIEFHUEwuDQoNCklmIHR"
             + "oaXMgaXMgeW91ciBpbnRlbnRpb24sIHlvdSBoYXZlIHB1Ymxpc2hlZCB5b3VyIG" + "93biBzb3VyY2UgY29kZSBhcyBBR1BMIHNvZnR3YXJlIHRvby4NClBsZWFzZSBsZ"
             + "XQgdXMga25vdyB3aGVyZSB0byBmaW5kIHlvdXIgc291cmNlIGNvZGUgYnkgc2Vu" + "ZGluZyBhIG1haWwgdG8gYWdwbEBpdGV4dHBkZi5jb20NCldlJ2QgYmUgaG9ub3J"
             + "lZCB0byBhZGQgaXQgdG8gb3VyIGxpc3Qgb2YgQUdQTCBwcm9qZWN0cyBidWlsdC" + "BvbiB0b3Agb2YgaVRleHQgb3IgaVRleHRTaGFycA0KYW5kIHdlJ2xsIGV4cGxha"
             + "W4gaG93IHRvIHJlbW92ZSB0aGlzIG1lc3NhZ2UgZnJvbSB5b3VyIGVycm9yIGxv" + "Z3MuDQoNCklmIHRoaXMgd2Fzbid0IHlvdXIgaW50ZW50aW9uLCB5b3UgYXJlIHB"
             + "yb2JhYmx5IHVzaW5nIGlUZXh0IGluIGEgbm9uLWZyZWUgZW52aXJvbm1lbnQuDQ" + "pJbiB0aGlzIGNhc2UsIHBsZWFzZSBjb250YWN0IHVzIGJ5IGZpbGxpbmcgb3V0I"
             + "HRoaXMgZm9ybTogaHR0cDovL2l0ZXh0cGRmLmNvbS9zYWxlcw0KSWYgeW91IGFy" + "ZSBhIGN1c3RvbWVyLCB3ZSdsbCBleHBsYWluIGhvdyB0byBpbnN0YWxsIHlvdXI"
             + "gbGljZW5zZSBrZXkgdG8gYXZvaWQgdGhpcyBtZXNzYWdlLg0KSWYgeW91J3JlIG" + "5vdCBhIGN1c3RvbWVyLCB3ZSdsbCBleHBsYWluIHRoZSBiZW5lZml0cyBvZiBiZ"
             + "WNvbWluZyBhIGN1c3RvbWVyLg0KDQo=");

        private static byte[] message_2 = Convert.FromBase64String("WW91ciBsaWNlbnNlIGhhcyBleHBpcmVkISBZb3UgYXJlIG5vdyB1c2luZyBpVGV"
             + "4dCB1bmRlciB0aGUgQUdQTC4NCg0KSWYgdGhpcyBpcyB5b3VyIGludGVudGlvbiwg" + "eW91IHNob3VsZCBoYXZlIHB1Ymxpc2hlZCB5b3VyIG93biBzb3VyY2UgY29kZSBhc"
             + "yBBR1BMIHNvZnR3YXJlIHRvby4NClBsZWFzZSBsZXQgdXMga25vdyB3aGVyZSB0by" + "BmaW5kIHlvdXIgc291cmNlIGNvZGUgYnkgc2VuZGluZyBhIG1haWwgdG8gYWdwbEB"
             + "pdGV4dHBkZi5jb20NCldlJ2QgYmUgaG9ub3JlZCB0byBhZGQgaXQgdG8gb3VyIGxp" + "c3Qgb2YgQUdQTCBwcm9qZWN0cyBidWlsdCBvbiB0b3Agb2YgaVRleHQgb3IgaVRle"
             + "HRTaGFycA0KYW5kIHdlJ2xsIGV4cGxhaW4gaG93IHRvIHJlbW92ZSB0aGlzIG1lc3" + "NhZ2UgZnJvbSB5b3VyIGVycm9yIGxvZ3MuDQoNCklmIHRoaXMgd2Fzbid0IHlvdXI"
             + "gaW50ZW50aW9uLCBwbGVhc2UgY29udGFjdCB1cyBieSBmaWxsaW5nIG91dCB0aGlz" + "IGZvcm06IGh0dHA6Ly9pdGV4dHBkZi5jb20vc2FsZXMgb3IgYnkgY29udGFjdGluZ"
             + "yBvdXIgc2FsZXMgZGVwYXJ0bWVudC4=");

        protected internal override void OnEvent(IEvent @event, IMetaInfo metaInfo) {
            if (count.IncrementAndGet() > repeatLevel) {
                if (iText.Kernel.Version.IsAGPLVersion() || iText.Kernel.Version.IsExpired()) {
                    String message = iText.IO.Util.JavaUtil.GetStringForBytes(message_1, iText.IO.Util.EncodingUtil.ISO_8859_1
                        );
                    if (iText.Kernel.Version.IsExpired()) {
                        message = iText.IO.Util.JavaUtil.GetStringForBytes(message_2, iText.IO.Util.EncodingUtil.ISO_8859_1);
                    }
                    level++;
                    if (level == 1) {
                        repeatLevel = REPEAT[1];
                    }
                    else {
                        repeatLevel = REPEAT[2];
                    }
                    if (logger == null) {
                        logger = LogManager.GetLogger(this.GetType());
                    }
                    logger.Info(message);
                }
                count.Set(0);
            }
        }
    }
}
