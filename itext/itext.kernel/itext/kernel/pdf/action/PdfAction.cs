/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using iText.Kernel;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Pdf.Navigation;

namespace iText.Kernel.Pdf.Action {
    public class PdfAction : PdfObjectWrapper<PdfDictionary> {
        /// <summary>a possible submitvalue</summary>
        public const int SUBMIT_EXCLUDE = 1;

        /// <summary>a possible submitvalue</summary>
        public const int SUBMIT_INCLUDE_NO_VALUE_FIELDS = 2;

        /// <summary>a possible submitvalue</summary>
        public const int SUBMIT_HTML_FORMAT = 4;

        /// <summary>a possible submitvalue</summary>
        public const int SUBMIT_HTML_GET = 8;

        /// <summary>a possible submitvalue</summary>
        public const int SUBMIT_COORDINATES = 16;

        /// <summary>a possible submitvalue</summary>
        public const int SUBMIT_XFDF = 32;

        /// <summary>a possible submitvalue</summary>
        public const int SUBMIT_INCLUDE_APPEND_SAVES = 64;

        /// <summary>a possible submitvalue</summary>
        public const int SUBMIT_INCLUDE_ANNOTATIONS = 128;

        /// <summary>a possible submitvalue</summary>
        public const int SUBMIT_PDF = 256;

        /// <summary>a possible submitvalue</summary>
        public const int SUBMIT_CANONICAL_FORMAT = 512;

        /// <summary>a possible submitvalue</summary>
        public const int SUBMIT_EXCL_NON_USER_ANNOTS = 1024;

        /// <summary>a possible submitvalue</summary>
        public const int SUBMIT_EXCL_F_KEY = 2048;

        /// <summary>a possible submitvalue</summary>
        public const int SUBMIT_EMBED_FORM = 8196;

        /// <summary>a possible submitvalue</summary>
        public const int RESET_EXCLUDE = 1;

        public PdfAction()
            : this(new PdfDictionary()) {
            Put(PdfName.Type, PdfName.Action);
        }

        public PdfAction(PdfDictionary pdfObject)
            : base(pdfObject) {
            MarkObjectAsIndirect(GetPdfObject());
        }

        public static iText.Kernel.Pdf.Action.PdfAction CreateGoTo(PdfDestination destination) {
            return new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.GoTo).Put(PdfName.D, destination.GetPdfObject
                ());
        }

        public static iText.Kernel.Pdf.Action.PdfAction CreateGoTo(String destination) {
            return CreateGoTo(new PdfStringDestination(destination));
        }

        public static iText.Kernel.Pdf.Action.PdfAction CreateGoToR(PdfFileSpec fileSpec, PdfDestination destination
            , bool newWindow) {
            return new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.GoToR).Put(PdfName.F, fileSpec.GetPdfObject
                ()).Put(PdfName.D, destination.GetPdfObject()).Put(PdfName.NewWindow, new PdfBoolean(newWindow));
        }

        public static iText.Kernel.Pdf.Action.PdfAction CreateGoToR(PdfFileSpec fileSpec, PdfDestination destination
            ) {
            return new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.GoToR).Put(PdfName.F, fileSpec.GetPdfObject
                ()).Put(PdfName.D, destination.GetPdfObject());
        }

        public static iText.Kernel.Pdf.Action.PdfAction CreateGoToR(String filename, int pageNum) {
            return CreateGoToR(filename, pageNum, false);
        }

        public static iText.Kernel.Pdf.Action.PdfAction CreateGoToR(String filename, int pageNum, bool newWindow) {
            return CreateGoToR(new PdfStringFS(filename), PdfExplicitDestination.CreateFitH(pageNum, 10000), newWindow
                );
        }

        public static iText.Kernel.Pdf.Action.PdfAction CreateGoToR(String filename, String destination, bool newWindow
            ) {
            return CreateGoToR(new PdfStringFS(filename), new PdfStringDestination(destination), newWindow);
        }

        public static iText.Kernel.Pdf.Action.PdfAction CreateGoToR(String filename, String destination) {
            return CreateGoToR(filename, destination, false);
        }

        public static iText.Kernel.Pdf.Action.PdfAction CreateGoToE(PdfDestination destination, bool newWindow, PdfTargetDictionary
             targetDictionary) {
            return CreateGoToE(null, destination, newWindow, targetDictionary);
        }

        public static iText.Kernel.Pdf.Action.PdfAction CreateGoToE(PdfFileSpec fileSpec, PdfDestination destination
            , bool newWindow, PdfTargetDictionary targetDictionary) {
            iText.Kernel.Pdf.Action.PdfAction action = new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.
                GoToE).Put(PdfName.NewWindow, new PdfBoolean(newWindow));
            if (fileSpec != null) {
                action.Put(PdfName.F, fileSpec.GetPdfObject());
            }
            if (destination != null) {
                action.Put(PdfName.D, destination.GetPdfObject());
            }
            if (targetDictionary != null) {
                action.Put(PdfName.T, targetDictionary.GetPdfObject());
            }
            return action;
        }

        public static iText.Kernel.Pdf.Action.PdfAction CreateLaunch(PdfFileSpec fileSpec, bool newWindow) {
            return CreateLaunch(fileSpec, null, newWindow);
        }

        public static iText.Kernel.Pdf.Action.PdfAction CreateLaunch(PdfFileSpec fileSpec) {
            iText.Kernel.Pdf.Action.PdfAction action = new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.
                Launch);
            if (fileSpec != null) {
                action.Put(PdfName.F, fileSpec.GetPdfObject());
            }
            return action;
        }

        public static iText.Kernel.Pdf.Action.PdfAction CreateLaunch(PdfFileSpec fileSpec, PdfWin win, bool newWindow
            ) {
            iText.Kernel.Pdf.Action.PdfAction action = new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.
                Launch).Put(PdfName.NewWindow, new PdfBoolean(newWindow));
            if (fileSpec != null) {
                action.Put(PdfName.F, fileSpec.GetPdfObject());
            }
            if (win != null) {
                action.Put(PdfName.Win, win.GetPdfObject());
            }
            return action;
        }

        public static iText.Kernel.Pdf.Action.PdfAction CreateThread(PdfFileSpec fileSpec, PdfObject destinationThread
            , PdfObject bead) {
            iText.Kernel.Pdf.Action.PdfAction action = new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.
                Launch).Put(PdfName.D, destinationThread).Put(PdfName.B, bead);
            if (fileSpec != null) {
                action.Put(PdfName.F, fileSpec.GetPdfObject());
            }
            return action;
        }

        public static iText.Kernel.Pdf.Action.PdfAction CreateThread(PdfFileSpec fileSpec) {
            return CreateThread(fileSpec, null, null);
        }

        public static iText.Kernel.Pdf.Action.PdfAction CreateURI(String uri) {
            return CreateURI(uri, false);
        }

        public static iText.Kernel.Pdf.Action.PdfAction CreateURI(String uri, bool isMap) {
            return new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.URI).Put(PdfName.URI, new PdfString(
                uri)).Put(PdfName.IsMap, new PdfBoolean(isMap));
        }

        public static iText.Kernel.Pdf.Action.PdfAction CreateSound(PdfStream sound) {
            return new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.Sound).Put(PdfName.Sound, sound);
        }

        public static iText.Kernel.Pdf.Action.PdfAction CreateSound(PdfStream sound, float volume, bool synchronous
            , bool repeat, bool mix) {
            return new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.Sound).Put(PdfName.Sound, sound).Put
                (PdfName.Volume, new PdfNumber(volume)).Put(PdfName.Synchronous, new PdfBoolean(synchronous)).Put(PdfName
                .Repeat, new PdfBoolean(repeat)).Put(PdfName.Mix, new PdfBoolean(mix));
        }

        public static iText.Kernel.Pdf.Action.PdfAction CreateMovie(PdfAnnotation annotation, String title, PdfName
             operation) {
            iText.Kernel.Pdf.Action.PdfAction action = new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.
                Movie).Put(PdfName.T, new PdfString(title)).Put(PdfName.Operation, operation);
            if (annotation != null) {
                action.Put(PdfName.Annotation, annotation.GetPdfObject());
            }
            return action;
        }

        public static iText.Kernel.Pdf.Action.PdfAction CreateHide(PdfAnnotation annotation, bool hidden) {
            return new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.Hide).Put(PdfName.T, annotation.GetPdfObject
                ()).Put(PdfName.H, new PdfBoolean(hidden));
        }

        public static iText.Kernel.Pdf.Action.PdfAction CreateHide(PdfAnnotation[] annotations, bool hidden) {
            return new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.Hide).Put(PdfName.T, GetPdfArrayFromAnnotationsList
                (annotations)).Put(PdfName.H, new PdfBoolean(hidden));
        }

        public static iText.Kernel.Pdf.Action.PdfAction CreateHide(String text, bool hidden) {
            return new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.Hide).Put(PdfName.T, new PdfString(text
                )).Put(PdfName.H, new PdfBoolean(hidden));
        }

        public static iText.Kernel.Pdf.Action.PdfAction CreateHide(String[] text, bool hidden) {
            return new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.Hide).Put(PdfName.T, GetArrayFromStringList
                (text)).Put(PdfName.H, new PdfBoolean(hidden));
        }

        public static iText.Kernel.Pdf.Action.PdfAction CreateNamed(PdfName namedAction) {
            return new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.Named).Put(PdfName.N, namedAction);
        }

        public static iText.Kernel.Pdf.Action.PdfAction CreateSetOcgState(IList<PdfActionOcgState> states) {
            return CreateSetOcgState(states, false);
        }

        public static iText.Kernel.Pdf.Action.PdfAction CreateSetOcgState(IList<PdfActionOcgState> states, bool preserveRb
            ) {
            PdfArray stateArr = new PdfArray();
            foreach (PdfActionOcgState state in states) {
                stateArr.AddAll(state.GetObjectList());
            }
            return new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.SetOCGState).Put(PdfName.State, stateArr
                ).Put(PdfName.PreserveRB, new PdfBoolean(preserveRb));
        }

        public static iText.Kernel.Pdf.Action.PdfAction CreateRendition(String file, PdfFileSpec fileSpec, String 
            mimeType, PdfAnnotation screenAnnotation) {
            return new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.Rendition).Put(PdfName.OP, new PdfNumber
                (0)).Put(PdfName.AN, screenAnnotation.GetPdfObject()).Put(PdfName.R, new PdfRendition(file, fileSpec, 
                mimeType).GetPdfObject());
        }

        public static iText.Kernel.Pdf.Action.PdfAction CreateJavaScript(String javaScript) {
            return new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.JavaScript).Put(PdfName.JS, new PdfString
                (javaScript));
        }

        public static iText.Kernel.Pdf.Action.PdfAction CreateSubmitForm(String file, Object[] names, int flags) {
            iText.Kernel.Pdf.Action.PdfAction action = new iText.Kernel.Pdf.Action.PdfAction();
            action.Put(PdfName.S, PdfName.SubmitForm);
            PdfDictionary dic = new PdfDictionary();
            dic.Put(PdfName.F, new PdfString(file));
            dic.Put(PdfName.FS, PdfName.URL);
            action.Put(PdfName.F, dic);
            if (names != null) {
                action.Put(PdfName.Fields, BuildArray(names));
            }
            action.Put(PdfName.Flags, new PdfNumber(flags));
            return action;
        }

        public static iText.Kernel.Pdf.Action.PdfAction CreateResetForm(Object[] names, int flags) {
            iText.Kernel.Pdf.Action.PdfAction action = new iText.Kernel.Pdf.Action.PdfAction();
            action.Put(PdfName.S, PdfName.ResetForm);
            if (names != null) {
                action.Put(PdfName.Fields, BuildArray(names));
            }
            action.Put(PdfName.Flags, new PdfNumber(flags));
            return action;
        }

        public static void SetAdditionalAction(PdfObjectWrapper<PdfDictionary> wrapper, PdfName key, iText.Kernel.Pdf.Action.PdfAction
             action) {
            PdfDictionary dic;
            PdfObject obj = wrapper.GetPdfObject().Get(PdfName.AA);
            if (obj != null && obj.IsDictionary()) {
                dic = (PdfDictionary)obj;
            }
            else {
                dic = new PdfDictionary();
            }
            dic.Put(key, action.GetPdfObject());
            wrapper.GetPdfObject().Put(PdfName.AA, dic);
        }

        /// <summary>Add a chained action.</summary>
        /// <param name="na"/>
        public virtual void Next(iText.Kernel.Pdf.Action.PdfAction na) {
            PdfObject nextAction = GetPdfObject().Get(PdfName.Next);
            if (nextAction == null) {
                Put(PdfName.Next, na.GetPdfObject());
            }
            else {
                if (nextAction.IsDictionary()) {
                    PdfArray array = new PdfArray(nextAction);
                    array.Add(na.GetPdfObject());
                    Put(PdfName.Next, array);
                }
                else {
                    ((PdfArray)nextAction).Add(na.GetPdfObject());
                }
            }
        }

        public virtual iText.Kernel.Pdf.Action.PdfAction Put(PdfName key, PdfObject value) {
            GetPdfObject().Put(key, value);
            return this;
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }

        private static PdfArray GetPdfArrayFromAnnotationsList(PdfAnnotation[] wrappers) {
            PdfArray arr = new PdfArray();
            foreach (PdfAnnotation wrapper in wrappers) {
                arr.Add(wrapper.GetPdfObject());
            }
            return arr;
        }

        private static PdfArray GetArrayFromStringList(String[] strings) {
            PdfArray arr = new PdfArray();
            foreach (String @string in strings) {
                arr.Add(new PdfString(@string));
            }
            return arr;
        }

        private static PdfArray BuildArray(Object[] names) {
            PdfArray array = new PdfArray();
            foreach (Object obj in names) {
                if (obj is String) {
                    array.Add(new PdfString((String)obj));
                }
                else {
                    if (obj is PdfAnnotation) {
                        array.Add(((PdfAnnotation)obj).GetPdfObject());
                    }
                    else {
                        throw new PdfException("the.array.must.contain.string.or.pdfannotation");
                    }
                }
            }
            return array;
        }
    }
}
