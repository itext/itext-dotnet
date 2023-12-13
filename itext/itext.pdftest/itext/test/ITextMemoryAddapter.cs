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
using Common.Logging;
using Common.Logging.Simple;

namespace iText.Test {
    public class ITextMemoryAddapter : CapturingLoggerFactoryAdapter {
        private HashSet<String> expectedTemplates = new HashSet<string>();
        
        public void SetExpectedTemplates(HashSet<String> expectedTemplates) {
            this.expectedTemplates.Clear();
            this.expectedTemplates.UnionWith(expectedTemplates);
        }
        public void Clear() {
            base.Clear();
            expectedTemplates.Clear();
        }
        
        public override void AddEvent(CapturingLoggerEvent le) {
            Console.WriteLine(le.Source.Name + ": " + le.RenderedMessage);
            if (le.Level >= LogLevel.Warn || IsExpectedMessage(le.RenderedMessage)) {
                base.AddEvent(le);
            }
        }
        
        private bool IsExpectedMessage(String message) {
            if (message != null) {
                foreach (var template in expectedTemplates) {
                    if (LogListenerHelper.EqualsMessageByTemplate(message, template)) {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
