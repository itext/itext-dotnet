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
             + "HRvIGZpbmQgeW91ciBzb3VyY2UgY29kZSBieSBzZW5kaW5nIGEgbWFpbCB0byBhZ3Bs" + "QGFwcnlzZS5jb20KV2UnZCBiZSBob25vcmVkIHRvIGFkZCBpdCB0byBvdXIgbGlzdCB"
             + "vZiBBR1BMIHByb2plY3RzIGJ1aWx0IG9uIHRvcCBvZiBpVGV4dAphbmQgd2UnbGwgZX" + "hwbGFpbiBob3cgdG8gcmVtb3ZlIHRoaXMgbWVzc2FnZSBmcm9tIHlvdXIgZXJyb3Igb"
             + "G9ncy4KCklmIHRoaXMgd2Fzbid0IHlvdXIgaW50ZW50aW9uLCB5b3UgYXJlIHByb2Jh" + "Ymx5IHVzaW5nIGlUZXh0IGluIGEgbm9uLWZyZWUgZW52aXJvbm1lbnQuCkluIHRoaXM"
             + "gY2FzZSwgcGxlYXNlIGNvbnRhY3QgdXMgYnkgZmlsbGluZyBvdXQgdGhpcyBmb3JtOi" + "BodHRwOi8vaXRleHRwZGYuY29tL3NhbGVzCklmIHlvdSBhcmUgYSBjdXN0b21lciwgd"
             + "2UnbGwgZXhwbGFpbiBob3cgdG8gaW5zdGFsbCB5b3VyIGxpY2Vuc2Uga2V5IHRvIGF2" + "b2lkIHRoaXMgbWVzc2FnZS4KSWYgeW91J3JlIG5vdCBhIGN1c3RvbWVyLCB3ZSdsbCB"
             + "leHBsYWluIHRoZSBiZW5lZml0cyBvZiBiZWNvbWluZyBhIGN1c3RvbWVyLg==");

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
