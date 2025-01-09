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
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Action {
    /// <summary>A wrapper for annotations additional actions dictionaries.</summary>
    /// <remarks>
    /// A wrapper for annotations additional actions dictionaries.
    /// See section 12.6.3 Table 197 of ISO 32000-1.
    /// An annotation additional actions dictionary defines the event handlers for annotations
    /// </remarks>
    public class PdfAnnotationAdditionalActions : PdfObjectWrapper<PdfDictionary> {
        private static readonly PdfName[] Events = new PdfName[] { PdfName.E, PdfName.X, PdfName.D, PdfName.U, PdfName
            .Fo, PdfName.Bl, PdfName.PO, PdfName.PC, PdfName.PV, PdfName.PI };

        public PdfAnnotationAdditionalActions(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        /// <summary>
        /// Returns the
        /// <see cref="PdfAction"/>
        /// for the OnEnter event if there is any, or null.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="PdfAction"/>
        /// or null
        /// </returns>
        public virtual PdfAction GetOnEnter() {
            return GetPdfActionForEvent(PdfName.E);
        }

        /// <summary>
        /// Sets the
        /// <see cref="PdfAction"/>
        /// to perform on the OnEnter event, or removes it when action is null.
        /// </summary>
        /// <param name="action">
        /// The
        /// <see cref="PdfAction"/>
        /// to set or null to remove the action
        /// </param>
        public virtual void SetOnEnter(PdfAction action) {
            SetPdfActionForEvent(PdfName.E, action);
        }

        /// <summary>
        /// Returns the
        /// <see cref="PdfAction"/>
        /// for the OnExit event if there is any, or null.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="PdfAction"/>
        /// or null
        /// </returns>
        public virtual PdfAction GetOnExit() {
            return GetPdfActionForEvent(PdfName.X);
        }

        /// <summary>
        /// Sets the
        /// <see cref="PdfAction"/>
        /// to perform on the OnExit event, or removes it when action is null.
        /// </summary>
        /// <param name="action">
        /// 
        /// <see cref="PdfAction"/>
        /// The action to set or null to remove the action
        /// </param>
        public virtual void SetOnExit(PdfAction action) {
            SetPdfActionForEvent(PdfName.X, action);
        }

        /// <summary>
        /// Returns the
        /// <see cref="PdfAction"/>
        /// for the OnMouseDown event if there is any, or null.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="PdfAction"/>
        /// or null
        /// </returns>
        public virtual PdfAction GetOnMouseDown() {
            return GetPdfActionForEvent(PdfName.D);
        }

        /// <summary>
        /// Sets the
        /// <see cref="PdfAction"/>
        /// to perform on the OnMouseDown event, or removes it when action is null.
        /// </summary>
        /// <param name="action">
        /// 
        /// <see cref="PdfAction"/>
        /// The action to set or null to remove the action
        /// </param>
        public virtual void SetOnMouseDown(PdfAction action) {
            SetPdfActionForEvent(PdfName.D, action);
        }

        /// <summary>
        /// Returns the
        /// <see cref="PdfAction"/>
        /// for the OnMouseUp event if there is any, or null.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="PdfAction"/>
        /// or null
        /// </returns>
        public virtual PdfAction GetOnMouseUp() {
            return GetPdfActionForEvent(PdfName.U);
        }

        /// <summary>
        /// Sets the
        /// <see cref="PdfAction"/>
        /// to perform on the OnMouseUp event, or removes it when action is null.
        /// </summary>
        /// <param name="action">
        /// 
        /// <see cref="PdfAction"/>
        /// The action to set or null to remove the action
        /// </param>
        public virtual void SetOnMouseUp(PdfAction action) {
            SetPdfActionForEvent(PdfName.U, action);
        }

        /// <summary>
        /// Returns the
        /// <see cref="PdfAction"/>
        /// for the OnFocus event if there is any, or null.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="PdfAction"/>
        /// or null
        /// </returns>
        public virtual PdfAction GetOnFocus() {
            return GetPdfActionForEvent(PdfName.Fo);
        }

        /// <summary>
        /// Sets the
        /// <see cref="PdfAction"/>
        /// to perform on the OnFocus event, or removes it when action is null.
        /// </summary>
        /// <param name="action">
        /// 
        /// <see cref="PdfAction"/>
        /// The action to set or null to remove the action
        /// </param>
        public virtual void SetOnFocus(PdfAction action) {
            SetPdfActionForEvent(PdfName.Fo, action);
        }

        /// <summary>
        /// Returns the
        /// <see cref="PdfAction"/>
        /// for the OnLostFocus event if there is any, or null.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="PdfAction"/>
        /// or null
        /// </returns>
        public virtual PdfAction GetOnLostFocus() {
            return GetPdfActionForEvent(PdfName.Bl);
        }

        /// <summary>
        /// Sets the
        /// <see cref="PdfAction"/>
        /// to perform on the OnLostFocus event, or removes it when action is null.
        /// </summary>
        /// <param name="action">
        /// 
        /// <see cref="PdfAction"/>
        /// The action to set or null to remove the action
        /// </param>
        public virtual void SetOnLostFocus(PdfAction action) {
            SetPdfActionForEvent(PdfName.Bl, action);
        }

        /// <summary>
        /// Returns the
        /// <see cref="PdfAction"/>
        /// for the OnPageOpened event if there is any, or null.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="PdfAction"/>
        /// or null
        /// </returns>
        public virtual PdfAction GetOnPageOpened() {
            return GetPdfActionForEvent(PdfName.PO);
        }

        /// <summary>
        /// Sets the
        /// <see cref="PdfAction"/>
        /// to perform on the OnPageOpened event, or removes it when action is null.
        /// </summary>
        /// <param name="action">
        /// 
        /// <see cref="PdfAction"/>
        /// The action to set or null to remove the action
        /// </param>
        public virtual void SetOnPageOpened(PdfAction action) {
            SetPdfActionForEvent(PdfName.PO, action);
        }

        /// <summary>
        /// Returns the
        /// <see cref="PdfAction"/>
        /// for the OnPageClosed event if there is any, or null.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="PdfAction"/>
        /// or null
        /// </returns>
        public virtual PdfAction GetOnPageClosed() {
            return GetPdfActionForEvent(PdfName.PC);
        }

        /// <summary>
        /// Sets the
        /// <see cref="PdfAction"/>
        /// to perform on the OnPageClosed event, or removes it when action is null.
        /// </summary>
        /// <param name="action">
        /// 
        /// <see cref="PdfAction"/>
        /// The action to set or null to remove the action
        /// </param>
        public virtual void SetOnPageClosed(PdfAction action) {
            SetPdfActionForEvent(PdfName.PC, action);
        }

        /// <summary>
        /// Returns the
        /// <see cref="PdfAction"/>
        /// for the OnPageVisible event if there is any, or null.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="PdfAction"/>
        /// or null
        /// </returns>
        public virtual PdfAction GetOnPageVisible() {
            return GetPdfActionForEvent(PdfName.PV);
        }

        /// <summary>
        /// Sets the
        /// <see cref="PdfAction"/>
        /// to perform on the OnPageVisible event, or removes it when action is null.
        /// </summary>
        /// <param name="action">
        /// 
        /// <see cref="PdfAction"/>
        /// The action to set or null to remove the action
        /// </param>
        public virtual void SetOnPageVisible(PdfAction action) {
            SetPdfActionForEvent(PdfName.PV, action);
        }

        /// <summary>
        /// Returns the
        /// <see cref="PdfAction"/>
        /// for the OnPageLostView event if there is any, or null.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="PdfAction"/>
        /// or null
        /// </returns>
        public virtual PdfAction GetOnPageLostView() {
            return GetPdfActionForEvent(PdfName.PI);
        }

        /// <summary>
        /// Sets the
        /// <see cref="PdfAction"/>
        /// to perform on the OnPageLostView event, or removes it when action is null.
        /// </summary>
        /// <param name="action">
        /// 
        /// <see cref="PdfAction"/>
        /// The action to set or null to remove the action
        /// </param>
        public virtual void SetOnPageLostView(PdfAction action) {
            SetPdfActionForEvent(PdfName.PI, action);
        }

        /// <summary>
        /// Lists every
        /// <see cref="PdfAction"/>
        /// for all documented events for an annotation's additional actions.
        /// </summary>
        /// <remarks>
        /// Lists every
        /// <see cref="PdfAction"/>
        /// for all documented events for an annotation's additional actions.
        /// See section 12.6.3 Table 197 of ISO 32000-1
        /// </remarks>
        /// <returns>The list of actions</returns>
        public virtual IList<PdfAction> GetAllKnownActions() {
            IList<PdfAction> result = new List<PdfAction>();
            foreach (PdfName @event in Events) {
                PdfAction action = GetPdfActionForEvent(@event);
                if (action != null) {
                    result.Add(action);
                }
            }
            return result;
        }

        /// <summary>
        /// If exists, returns the
        /// <see cref="PdfAction"/>
        /// for this event, otherwise returns null.
        /// </summary>
        /// <param name="eventName">
        /// The
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// for the event.
        /// </param>
        /// <returns>
        /// the
        /// <see cref="PdfAction"/>
        /// or null
        /// </returns>
        public virtual PdfAction GetPdfActionForEvent(PdfName eventName) {
            PdfObject action = GetPdfObject().Get(eventName);
            if (action == null || !action.IsDictionary()) {
                return null;
            }
            return new PdfAction((PdfDictionary)action);
        }

        /// <summary>Sets the action for an event, or removes it when the action is null.</summary>
        /// <param name="event">the event to set or remove the action for</param>
        /// <param name="action">
        /// the
        /// <see cref="PdfAction"/>
        /// to set or null
        /// </param>
        public virtual void SetPdfActionForEvent(PdfName @event, PdfAction action) {
            if (action == null) {
                GetPdfObject().Remove(@event);
            }
            else {
                GetPdfObject().Put(@event, action.GetPdfObject());
            }
            SetModified();
        }

        /// <summary><inheritDoc/></summary>
        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }
    }
}
