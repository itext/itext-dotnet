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
using System.Collections.Generic;
using System.Linq;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Action {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfAnnotationAdditionalActionsTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void OnEnterTest() {
            PdfAnnotationAdditionalActions aa = new PdfAnnotationAdditionalActions(new PdfDictionary());
            PdfAction action = PdfAction.CreateHide("test", false);
            aa.SetOnEnter(action);
            NUnit.Framework.Assert.AreEqual(action.GetPdfObject().GetAsString(PdfName.T), aa.GetOnEnter().GetPdfObject
                ().GetAsString(PdfName.T));
        }

        [NUnit.Framework.Test]
        public virtual void OnExitTest() {
            PdfAnnotationAdditionalActions aa = new PdfAnnotationAdditionalActions(new PdfDictionary());
            PdfAction action = PdfAction.CreateHide("test", false);
            aa.SetOnExit(action);
            NUnit.Framework.Assert.AreEqual(action.GetPdfObject().GetAsString(PdfName.T), aa.GetOnExit().GetPdfObject(
                ).GetAsString(PdfName.T));
        }

        [NUnit.Framework.Test]
        public virtual void OnFocusTest() {
            PdfAnnotationAdditionalActions aa = new PdfAnnotationAdditionalActions(new PdfDictionary());
            PdfAction action = PdfAction.CreateHide("test", false);
            aa.SetOnFocus(action);
            NUnit.Framework.Assert.AreEqual(action.GetPdfObject().GetAsString(PdfName.T), aa.GetOnFocus().GetPdfObject
                ().GetAsString(PdfName.T));
        }

        [NUnit.Framework.Test]
        public virtual void OnLostFocusTest() {
            PdfAnnotationAdditionalActions aa = new PdfAnnotationAdditionalActions(new PdfDictionary());
            PdfAction action = PdfAction.CreateHide("test", false);
            aa.SetOnLostFocus(action);
            NUnit.Framework.Assert.AreEqual(action.GetPdfObject().GetAsString(PdfName.T), aa.GetOnLostFocus().GetPdfObject
                ().GetAsString(PdfName.T));
        }

        [NUnit.Framework.Test]
        public virtual void OnMouseDownTest() {
            PdfAnnotationAdditionalActions aa = new PdfAnnotationAdditionalActions(new PdfDictionary());
            PdfAction action = PdfAction.CreateHide("test", false);
            aa.SetOnMouseDown(action);
            NUnit.Framework.Assert.AreEqual(action.GetPdfObject().GetAsString(PdfName.T), aa.GetOnMouseDown().GetPdfObject
                ().GetAsString(PdfName.T));
        }

        [NUnit.Framework.Test]
        public virtual void OnMouseUpTest() {
            PdfAnnotationAdditionalActions aa = new PdfAnnotationAdditionalActions(new PdfDictionary());
            PdfAction action = PdfAction.CreateHide("test", false);
            aa.SetOnMouseUp(action);
            NUnit.Framework.Assert.AreEqual(action.GetPdfObject().GetAsString(PdfName.T), aa.GetOnMouseUp().GetPdfObject
                ().GetAsString(PdfName.T));
        }

        [NUnit.Framework.Test]
        public virtual void OnPageClosedTest() {
            PdfAnnotationAdditionalActions aa = new PdfAnnotationAdditionalActions(new PdfDictionary());
            PdfAction action = PdfAction.CreateHide("test", false);
            aa.SetOnPageClosed(action);
            NUnit.Framework.Assert.AreEqual(action.GetPdfObject().GetAsString(PdfName.T), aa.GetOnPageClosed().GetPdfObject
                ().GetAsString(PdfName.T));
        }

        [NUnit.Framework.Test]
        public virtual void OnPageLostViewTest() {
            PdfAnnotationAdditionalActions aa = new PdfAnnotationAdditionalActions(new PdfDictionary());
            PdfAction action = PdfAction.CreateHide("test", false);
            aa.SetOnPageLostView(action);
            NUnit.Framework.Assert.AreEqual(action.GetPdfObject().GetAsString(PdfName.T), aa.GetOnPageLostView().GetPdfObject
                ().GetAsString(PdfName.T));
        }

        [NUnit.Framework.Test]
        public virtual void OnPageOpenedTest() {
            PdfAnnotationAdditionalActions aa = new PdfAnnotationAdditionalActions(new PdfDictionary());
            PdfAction action = PdfAction.CreateHide("test", false);
            aa.SetOnPageOpened(action);
            NUnit.Framework.Assert.AreEqual(action.GetPdfObject().GetAsString(PdfName.T), aa.GetOnPageOpened().GetPdfObject
                ().GetAsString(PdfName.T));
        }

        [NUnit.Framework.Test]
        public virtual void OnPageVisibleTest() {
            PdfAnnotationAdditionalActions aa = new PdfAnnotationAdditionalActions(new PdfDictionary());
            PdfAction action = PdfAction.CreateHide("test", false);
            aa.SetOnPageVisible(action);
            NUnit.Framework.Assert.AreEqual(action.GetPdfObject().GetAsString(PdfName.T), aa.GetOnPageVisible().GetPdfObject
                ().GetAsString(PdfName.T));
        }

        [NUnit.Framework.Test]
        public virtual void GetAllKnownActions() {
            PdfAnnotationAdditionalActions aa = new PdfAnnotationAdditionalActions(new PdfDictionary());
            PdfAction actionPageVisible = PdfAction.CreateHide("PageVisible", false);
            PdfAction actionPageOpened = PdfAction.CreateHide("PageOpened", false);
            PdfAction actionPageLostView = PdfAction.CreateHide("PageLostView", false);
            PdfAction actionPageClosed = PdfAction.CreateHide("PageClosed", false);
            PdfAction actionMouseUp = PdfAction.CreateHide("MouseUp", false);
            PdfAction actionMouseDown = PdfAction.CreateHide("MouseDown", false);
            PdfAction actionLostFocus = PdfAction.CreateHide("LostFocus", false);
            PdfAction actionFocus = PdfAction.CreateHide("Focus", false);
            PdfAction actionExit = PdfAction.CreateHide("Exit", false);
            PdfAction actionEnter = PdfAction.CreateHide("Enter", false);
            aa.SetOnPageVisible(actionPageVisible);
            aa.SetOnPageClosed(actionPageClosed);
            aa.SetOnPageOpened(actionPageOpened);
            aa.SetOnPageLostView(actionPageLostView);
            aa.SetOnExit(actionExit);
            aa.SetOnEnter(actionEnter);
            aa.SetOnMouseUp(actionMouseUp);
            aa.SetOnMouseDown(actionMouseDown);
            aa.SetOnFocus(actionFocus);
            aa.SetOnLostFocus(actionLostFocus);
            IList<PdfAction> result = aa.GetAllKnownActions();
            NUnit.Framework.Assert.AreEqual(10, result.Count);
            NUnit.Framework.Assert.IsTrue(result.Any((a) => a.GetPdfObject().GetAsString(PdfName.T).GetValue() == "PageVisible"
                ));
            NUnit.Framework.Assert.IsTrue(result.Any((a) => a.GetPdfObject().GetAsString(PdfName.T).GetValue() == "PageClosed"
                ));
            NUnit.Framework.Assert.IsTrue(result.Any((a) => a.GetPdfObject().GetAsString(PdfName.T).GetValue() == "PageOpened"
                ));
            NUnit.Framework.Assert.IsTrue(result.Any((a) => a.GetPdfObject().GetAsString(PdfName.T).GetValue() == "PageLostView"
                ));
            NUnit.Framework.Assert.IsTrue(result.Any((a) => a.GetPdfObject().GetAsString(PdfName.T).GetValue() == "Exit"
                ));
            NUnit.Framework.Assert.IsTrue(result.Any((a) => a.GetPdfObject().GetAsString(PdfName.T).GetValue() == "Enter"
                ));
            NUnit.Framework.Assert.IsTrue(result.Any((a) => a.GetPdfObject().GetAsString(PdfName.T).GetValue() == "MouseUp"
                ));
            NUnit.Framework.Assert.IsTrue(result.Any((a) => a.GetPdfObject().GetAsString(PdfName.T).GetValue() == "MouseDown"
                ));
            NUnit.Framework.Assert.IsTrue(result.Any((a) => a.GetPdfObject().GetAsString(PdfName.T).GetValue() == "Focus"
                ));
            NUnit.Framework.Assert.IsTrue(result.Any((a) => a.GetPdfObject().GetAsString(PdfName.T).GetValue() == "LostFocus"
                ));
        }
    }
}
