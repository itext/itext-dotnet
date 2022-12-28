/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Actions;
using iText.Commons.Actions.Confirmations;
using iText.Commons.Utils;

namespace iText.Commons.Actions.Processors {
    /// <summary>Defines a default strategy of product event processing.</summary>
    public class DefaultITextProductEventProcessor : AbstractITextProductEventProcessor {
        internal static readonly byte[] MESSAGE_FOR_LOGGING = Convert.FromBase64String("WW91IGFyZSB1c2luZyBpVGV4dCB1bmRlciB0aGUgQUdQTC4KCklmIHRoaXMgaXMgeW9"
             + "1ciBpbnRlbnRpb24sIHlvdSBoYXZlIHB1Ymxpc2hlZCB5b3VyIG93biBzb3VyY2UgY2" + "9kZSBhcyBBR1BMIHNvZnR3YXJlIHRvby4KUGxlYXNlIGxldCB1cyBrbm93IHdoZXJlI"
             + "HRvIGZpbmQgeW91ciBzb3VyY2UgY29kZSBieSBzZW5kaW5nIGEgbWFpbCB0byBhZ3Bs" + "QGl0ZXh0cGRmLmNvbQpXZSdkIGJlIGhvbm9yZWQgdG8gYWRkIGl0IHRvIG91ciBsaXN"
             + "0IG9mIEFHUEwgcHJvamVjdHMgYnVpbHQgb24gdG9wIG9mIGlUZXh0IDcKYW5kIHdlJ2" + "xsIGV4cGxhaW4gaG93IHRvIHJlbW92ZSB0aGlzIG1lc3NhZ2UgZnJvbSB5b3VyIGVyc"
             + "m9yIGxvZ3MuCgpJZiB0aGlzIHdhc24ndCB5b3VyIGludGVudGlvbiwgeW91IGFyZSBw" + "cm9iYWJseSB1c2luZyBpVGV4dCBpbiBhIG5vbi1mcmVlIGVudmlyb25tZW50LgpJbiB"
             + "0aGlzIGNhc2UsIHBsZWFzZSBjb250YWN0IHVzIGJ5IGZpbGxpbmcgb3V0IHRoaXMgZm" + "9ybTogaHR0cDovL2l0ZXh0cGRmLmNvbS9zYWxlcwpJZiB5b3UgYXJlIGEgY3VzdG9tZ"
             + "XIsIHdlJ2xsIGV4cGxhaW4gaG93IHRvIGluc3RhbGwgeW91ciBsaWNlbnNlIGtleSB0" + "byBhdm9pZCB0aGlzIG1lc3NhZ2UuCklmIHlvdSdyZSBub3QgYSBjdXN0b21lciwgd2U"
             + "nbGwgZXhwbGFpbiB0aGUgYmVuZWZpdHMgb2YgYmVjb21pbmcgYSBjdXN0b21lci4=");

        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Commons.Actions.Processors.DefaultITextProductEventProcessor
            ));

        private static readonly long[] REPEAT = new long[] { 10000L, 5000L, 1000L };

        private static readonly int MAX_LVL = REPEAT.Length - 1;

        private readonly Object Lock = new Object();

        private readonly AtomicLong counter = new AtomicLong(0);

        private readonly AtomicLong level = new AtomicLong(0);

        private readonly AtomicLong repeatLevel;

        /// <summary>Creates an instance of product event processor.</summary>
        /// <param name="productName">is a product name</param>
        public DefaultITextProductEventProcessor(String productName)
            : base(productName) {
            repeatLevel = new AtomicLong(AcquireRepeatLevel((int)level.Get()));
        }

        public override void OnEvent(AbstractProductProcessITextEvent @event) {
            if (!(@event is ConfirmEvent)) {
                return;
            }
            bool isNeededToLogMessage = false;
            lock (Lock) {
                if (counter.IncrementAndGet() > repeatLevel.Get()) {
                    counter.Set(0);
                    if (level.IncrementAndGet() > MAX_LVL) {
                        level.Set(MAX_LVL);
                    }
                    repeatLevel.Set(AcquireRepeatLevel((int)level.Get()));
                    isNeededToLogMessage = true;
                }
            }
            if (isNeededToLogMessage) {
                String message = iText.Commons.Utils.JavaUtil.GetStringForBytes(MESSAGE_FOR_LOGGING, iText.Commons.Utils.EncodingUtil.ISO_8859_1
                    );
                LOGGER.LogInformation(message);
                // System out added with purpose. This is not a debug code
                System.Console.Out.WriteLine(message);
            }
        }

        public override String GetUsageType() {
            return "AGPL";
        }

        internal virtual long AcquireRepeatLevel(int lvl) {
            return REPEAT[lvl];
        }
    }
}
