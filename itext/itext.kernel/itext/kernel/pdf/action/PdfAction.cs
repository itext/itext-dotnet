/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Pdf.Navigation;

namespace iText.Kernel.Pdf.Action {
    /// <summary>A wrapper for action dictionaries (ISO 32000-1 section 12.6).</summary>
    /// <remarks>
    /// A wrapper for action dictionaries (ISO 32000-1 section 12.6).
    /// An action dictionary defines the characteristics and behaviour of an action.
    /// </remarks>
    public class PdfAction : PdfObjectWrapper<PdfDictionary> {
        /// <summary>A possible submit value</summary>
        public const int SUBMIT_EXCLUDE = 1;

        /// <summary>A possible submit value</summary>
        public const int SUBMIT_INCLUDE_NO_VALUE_FIELDS = 2;

        /// <summary>A possible submit value</summary>
        public const int SUBMIT_HTML_FORMAT = 4;

        /// <summary>A possible submit value</summary>
        public const int SUBMIT_HTML_GET = 8;

        /// <summary>A possible submit value</summary>
        public const int SUBMIT_COORDINATES = 16;

        /// <summary>A possible submit value</summary>
        public const int SUBMIT_XFDF = 32;

        /// <summary>A possible submit value</summary>
        public const int SUBMIT_INCLUDE_APPEND_SAVES = 64;

        /// <summary>A possible submit value</summary>
        public const int SUBMIT_INCLUDE_ANNOTATIONS = 128;

        /// <summary>A possible submit value</summary>
        public const int SUBMIT_PDF = 256;

        /// <summary>A possible submit value</summary>
        public const int SUBMIT_CANONICAL_FORMAT = 512;

        /// <summary>A possible submit value</summary>
        public const int SUBMIT_EXCL_NON_USER_ANNOTS = 1024;

        /// <summary>A possible submit value</summary>
        public const int SUBMIT_EXCL_F_KEY = 2048;

        /// <summary>A possible submit value</summary>
        public const int SUBMIT_EMBED_FORM = 8196;

        /// <summary>A possible submit value</summary>
        public const int RESET_EXCLUDE = 1;

        /// <summary>Constructs an empty action that can be further modified.</summary>
        public PdfAction()
            : this(new PdfDictionary()) {
            Put(PdfName.Type, PdfName.Action);
        }

        /// <summary>
        /// Constructs a
        /// <see cref="PdfAction"/>
        /// instance with a given dictionary.
        /// </summary>
        /// <remarks>
        /// Constructs a
        /// <see cref="PdfAction"/>
        /// instance with a given dictionary. It can be used for handy
        /// property reading in reading mode or modifying in stamping mode.
        /// </remarks>
        /// <param name="pdfObject">the dictionary to construct the wrapper around</param>
        public PdfAction(PdfDictionary pdfObject)
            : base(pdfObject) {
            MarkObjectAsIndirect(GetPdfObject());
        }

        /// <summary>Creates a GoTo action (section 12.6.4.2 of ISO 32000-1) via a given destination.</summary>
        /// <param name="destination">the desired destination of the action</param>
        /// <returns>created action</returns>
        public static iText.Kernel.Pdf.Action.PdfAction CreateGoTo(PdfDestination destination) {
            ValidateNotRemoteDestination(destination);
            return new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.GoTo).Put(PdfName.D, destination.GetPdfObject
                ());
        }

        /// <summary>
        /// Creates a GoTo action (section 12.6.4.2 of ISO 32000-1) via a given
        /// <see cref="iText.Kernel.Pdf.Navigation.PdfStringDestination"/>
        /// name.
        /// </summary>
        /// <param name="destination">
        /// 
        /// <see cref="iText.Kernel.Pdf.Navigation.PdfStringDestination"/>
        /// name
        /// </param>
        /// <returns>created action</returns>
        public static iText.Kernel.Pdf.Action.PdfAction CreateGoTo(String destination) {
            return CreateGoTo(new PdfStringDestination(destination));
        }

        /// <summary>Creates a GoToR action, or remote action (section 12.6.4.3 of ISO 32000-1).</summary>
        /// <param name="fileSpec">the file in which the destination shall be located</param>
        /// <param name="destination">the destination in the remote document to jump to</param>
        /// <param name="newWindow">a flag specifying whether to open the destination document in a new window</param>
        /// <returns>created action</returns>
        public static iText.Kernel.Pdf.Action.PdfAction CreateGoToR(PdfFileSpec fileSpec, PdfDestination destination
            , bool newWindow) {
            return CreateGoToR(fileSpec, destination).Put(PdfName.NewWindow, PdfBoolean.ValueOf(newWindow));
        }

        /// <summary>Creates a GoToR action, or remote action (section 12.6.4.3 of ISO 32000-1).</summary>
        /// <param name="fileSpec">the file in which the destination shall be located</param>
        /// <param name="destination">the destination in the remote document to jump to</param>
        /// <returns>created action</returns>
        public static iText.Kernel.Pdf.Action.PdfAction CreateGoToR(PdfFileSpec fileSpec, PdfDestination destination
            ) {
            ValidateRemoteDestination(destination);
            return new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.GoToR).Put(PdfName.F, fileSpec.GetPdfObject
                ()).Put(PdfName.D, destination.GetPdfObject());
        }

        /// <summary>Creates a GoToR action, or remote action (section 12.6.4.3 of ISO 32000-1).</summary>
        /// <param name="filename">the remote destination file to jump to</param>
        /// <param name="pageNum">the remote destination document page to jump to</param>
        /// <returns>created action</returns>
        public static iText.Kernel.Pdf.Action.PdfAction CreateGoToR(String filename, int pageNum) {
            return CreateGoToR(filename, pageNum, false);
        }

        /// <summary>Creates a GoToR action, or remote action (section 12.6.4.3 of ISO 32000-1).</summary>
        /// <param name="filename">the remote destination file to jump to</param>
        /// <param name="pageNum">the remote destination document page to jump to</param>
        /// <param name="newWindow">a flag specifying whether to open the destination document in a new window</param>
        /// <returns>created action</returns>
        public static iText.Kernel.Pdf.Action.PdfAction CreateGoToR(String filename, int pageNum, bool newWindow) {
            return CreateGoToR(new PdfStringFS(filename), PdfExplicitRemoteGoToDestination.CreateFitH(pageNum, 10000), 
                newWindow);
        }

        /// <summary>Creates a GoToR action, or remote action (section 12.6.4.3 of ISO 32000-1).</summary>
        /// <param name="filename">the remote destination file to jump to</param>
        /// <param name="destination">the string destination in the remote document to jump to</param>
        /// <param name="newWindow">a flag specifying whether to open the destination document in a new window</param>
        /// <returns>created action</returns>
        public static iText.Kernel.Pdf.Action.PdfAction CreateGoToR(String filename, String destination, bool newWindow
            ) {
            return CreateGoToR(new PdfStringFS(filename), new PdfStringDestination(destination), newWindow);
        }

        /// <summary>Creates a GoToR action, or remote action (section 12.6.4.3 of ISO 32000-1).</summary>
        /// <param name="filename">the remote destination file to jump to</param>
        /// <param name="destination">the string destination in the remote document to jump to</param>
        /// <returns>created action</returns>
        public static iText.Kernel.Pdf.Action.PdfAction CreateGoToR(String filename, String destination) {
            return CreateGoToR(filename, destination, false);
        }

        /// <summary>Creates a GoToE action, or embedded file action (section 12.6.4.4 of ISO 32000-1).</summary>
        /// <param name="destination">the destination in the target to jump to</param>
        /// <param name="newWindow">
        /// if true, the destination document should be opened in a new window;
        /// if false, the destination document should replace the current document in the same window
        /// </param>
        /// <param name="targetDictionary">
        /// A target dictionary specifying path information to the target document.
        /// Each target dictionary specifies one element in the full path to the target and
        /// may have nested target dictionaries specifying additional elements
        /// </param>
        /// <returns>created action</returns>
        public static iText.Kernel.Pdf.Action.PdfAction CreateGoToE(PdfDestination destination, bool newWindow, PdfTarget
             targetDictionary) {
            return CreateGoToE(null, destination, newWindow, targetDictionary);
        }

        /// <summary>Creates a GoToE action, or embedded file action (section 12.6.4.4 of ISO 32000-1).</summary>
        /// <param name="fileSpec">The root document of the target relative to the root document of the source</param>
        /// <param name="destination">the destination in the target to jump to</param>
        /// <param name="newWindow">
        /// if true, the destination document should be opened in a new window;
        /// if false, the destination document should replace the current document in the same window
        /// </param>
        /// <param name="targetDictionary">
        /// A target dictionary specifying path information to the target document.
        /// Each target dictionary specifies one element in the full path to the target and
        /// may have nested target dictionaries specifying additional elements
        /// </param>
        /// <returns>created action</returns>
        public static iText.Kernel.Pdf.Action.PdfAction CreateGoToE(PdfFileSpec fileSpec, PdfDestination destination
            , bool newWindow, PdfTarget targetDictionary) {
            iText.Kernel.Pdf.Action.PdfAction action = new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.
                GoToE).Put(PdfName.NewWindow, PdfBoolean.ValueOf(newWindow));
            if (fileSpec != null) {
                action.Put(PdfName.F, fileSpec.GetPdfObject());
            }
            if (destination != null) {
                ValidateRemoteDestination(destination);
                action.Put(PdfName.D, destination.GetPdfObject());
            }
            else {
                ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.Action.PdfAction)).LogWarning(iText.IO.Logs.IoLogMessageConstant
                    .EMBEDDED_GO_TO_DESTINATION_NOT_SPECIFIED);
            }
            if (targetDictionary != null) {
                action.Put(PdfName.T, targetDictionary.GetPdfObject());
            }
            return action;
        }

        /// <summary>Creates a Launch action (section 12.6.4.5 of ISO 32000-1).</summary>
        /// <param name="fileSpec">the application that shall be launched or the document that shall beopened or printed
        ///     </param>
        /// <param name="newWindow">a flag specifying whether to open the destination document in a new window</param>
        /// <returns>created action</returns>
        public static iText.Kernel.Pdf.Action.PdfAction CreateLaunch(PdfFileSpec fileSpec, bool newWindow) {
            return CreateLaunch(fileSpec).Put(PdfName.NewWindow, new PdfBoolean(newWindow));
        }

        /// <summary>Creates a Launch action (section 12.6.4.5 of ISO 32000-1).</summary>
        /// <param name="fileSpec">the application that shall be launched or the document that shall beopened or printed
        ///     </param>
        /// <returns>created action</returns>
        public static iText.Kernel.Pdf.Action.PdfAction CreateLaunch(PdfFileSpec fileSpec) {
            iText.Kernel.Pdf.Action.PdfAction action = new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.
                Launch);
            if (fileSpec != null) {
                action.Put(PdfName.F, fileSpec.GetPdfObject());
            }
            return action;
        }

        /// <summary>Creates a Thread action (section 12.6.4.6 of ISO 32000-1).</summary>
        /// <remarks>
        /// Creates a Thread action (section 12.6.4.6 of ISO 32000-1).
        /// A thread action jumps to a specified bead on an article thread (see 12.4.3, "Articles"),
        /// in either the current document or a different one. Table 205 shows the action dictionary
        /// entries specific to this type of action.
        /// </remarks>
        /// <param name="fileSpec">the file containing the thread. If this entry is absent, the thread is in the current file
        ///     </param>
        /// <param name="destinationThread">the destination thread</param>
        /// <param name="bead">the bead in the destination thread</param>
        /// <returns>created action</returns>
        public static iText.Kernel.Pdf.Action.PdfAction CreateThread(PdfFileSpec fileSpec, PdfObject destinationThread
            , PdfObject bead) {
            iText.Kernel.Pdf.Action.PdfAction action = new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.
                Launch).Put(PdfName.D, destinationThread).Put(PdfName.B, bead);
            if (fileSpec != null) {
                action.Put(PdfName.F, fileSpec.GetPdfObject());
            }
            return action;
        }

        /// <summary>Creates a Thread action (section 12.6.4.6 of ISO 32000-1).</summary>
        /// <remarks>
        /// Creates a Thread action (section 12.6.4.6 of ISO 32000-1).
        /// A thread action jumps to a specified bead on an article thread (see 12.4.3, "Articles"),
        /// in either the current document or a different one. Table 205 shows the action dictionary
        /// entries specific to this type of action.
        /// </remarks>
        /// <param name="fileSpec">the file containing the thread. If this entry is absent, the thread is in the current file
        ///     </param>
        /// <returns>created action</returns>
        public static iText.Kernel.Pdf.Action.PdfAction CreateThread(PdfFileSpec fileSpec) {
            return CreateThread(fileSpec, null, null);
        }

        /// <summary>Creates a URI action (section 12.6.4.7 of ISO 32000-1).</summary>
        /// <param name="uri">the uniform resource identifier to resolve</param>
        /// <returns>created action</returns>
        public static iText.Kernel.Pdf.Action.PdfAction CreateURI(String uri) {
            return CreateURI(uri, false);
        }

        /// <summary>Creates a URI action (section 12.6.4.7 of ISO 32000-1).</summary>
        /// <param name="uri">the uniform resource identifier to resolve</param>
        /// <param name="isMap">a flag specifying whether to track the mouse position when the URI is resolved</param>
        /// <returns>created action</returns>
        public static iText.Kernel.Pdf.Action.PdfAction CreateURI(String uri, bool isMap) {
            return new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.URI).Put(PdfName.URI, new PdfString(
                uri)).Put(PdfName.IsMap, PdfBoolean.ValueOf(isMap));
        }

        /// <summary>Creates a Sound action (section 12.6.4.8 of ISO 32000-1).</summary>
        /// <remarks>Creates a Sound action (section 12.6.4.8 of ISO 32000-1). Deprecated in PDF 2.0.</remarks>
        /// <param name="sound">a sound object defining the sound that shall be played (see section 13.3 of ISO 32000-1)
        ///     </param>
        /// <returns>created action</returns>
        public static iText.Kernel.Pdf.Action.PdfAction CreateSound(PdfStream sound) {
            return new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.Sound).Put(PdfName.Sound, sound);
        }

        /// <summary>Creates a Sound action (section 12.6.4.8 of ISO 32000-1).</summary>
        /// <remarks>Creates a Sound action (section 12.6.4.8 of ISO 32000-1). Deprecated in PDF 2.0.</remarks>
        /// <param name="sound">a sound object defining the sound that shall be played (see section 13.3 of ISO 32000-1)
        ///     </param>
        /// <param name="volume">the volume at which to play the sound, in the range -1.0 to 1.0. Default value: 1.0</param>
        /// <param name="synchronous">
        /// a flag specifying whether to play the sound synchronously or asynchronously.
        /// If this flag is <c>true</c>, the conforming reader retains control, allowing no further user
        /// interaction other than canceling the sound, until the sound has been completely played.
        /// Default value: <c>false</c>
        /// </param>
        /// <param name="repeat">
        /// a flag specifying whether to repeat the sound indefinitely
        /// If this entry is present, the Synchronous entry shall be ignored. Default value: <c>false</c>
        /// </param>
        /// <param name="mix">a flag specifying whether to mix this sound with any other sound already playing</param>
        /// <returns>created action</returns>
        public static iText.Kernel.Pdf.Action.PdfAction CreateSound(PdfStream sound, float volume, bool synchronous
            , bool repeat, bool mix) {
            if (volume < -1 || volume > 1) {
                throw new ArgumentException("volume");
            }
            return new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.Sound).Put(PdfName.Sound, sound).Put
                (PdfName.Volume, new PdfNumber(volume)).Put(PdfName.Synchronous, PdfBoolean.ValueOf(synchronous)).Put(
                PdfName.Repeat, PdfBoolean.ValueOf(repeat)).Put(PdfName.Mix, PdfBoolean.ValueOf(mix));
        }

        /// <summary>Creates a Movie annotation (section 12.6.4.9 of ISO 32000-1).</summary>
        /// <remarks>Creates a Movie annotation (section 12.6.4.9 of ISO 32000-1). Deprecated in PDF 2.0.</remarks>
        /// <param name="annotation">a movie annotation identifying the movie that shall be played</param>
        /// <param name="title">the title of a movie annotation identifying the movie that shall be played</param>
        /// <param name="operation">
        /// the operation that shall be performed on the movie. Shall be one of the following:
        /// <see cref="iText.Kernel.Pdf.PdfName.Play"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.Stop"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.Pause"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.Resume"/>
        /// </param>
        /// <returns>created annotation</returns>
        public static iText.Kernel.Pdf.Action.PdfAction CreateMovie(PdfAnnotation annotation, String title, PdfName
             operation) {
            iText.Kernel.Pdf.Action.PdfAction action = new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.
                Movie).Put(PdfName.T, new PdfString(title)).Put(PdfName.Operation, operation);
            if (annotation != null) {
                action.Put(PdfName.Annotation, annotation.GetPdfObject());
            }
            return action;
        }

        /// <summary>Creates a Hide action (section 12.6.4.10 of ISO 32000-1).</summary>
        /// <param name="annotation">the annotation to be hidden or shown</param>
        /// <param name="hidden">a flag indicating whether to hide the annotation (<c>true</c>) or show it (<c>false</c>)
        ///     </param>
        /// <returns>created action</returns>
        public static iText.Kernel.Pdf.Action.PdfAction CreateHide(PdfAnnotation annotation, bool hidden) {
            return new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.Hide).Put(PdfName.T, annotation.GetPdfObject
                ()).Put(PdfName.H, PdfBoolean.ValueOf(hidden));
        }

        /// <summary>Creates a Hide action (section 12.6.4.10 of ISO 32000-1).</summary>
        /// <param name="annotations">the annotations to be hidden or shown</param>
        /// <param name="hidden">a flag indicating whether to hide the annotation (<c>true</c>) or show it (<c>false</c>)
        ///     </param>
        /// <returns>created action</returns>
        public static iText.Kernel.Pdf.Action.PdfAction CreateHide(PdfAnnotation[] annotations, bool hidden) {
            return new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.Hide).Put(PdfName.T, GetPdfArrayFromAnnotationsList
                (annotations)).Put(PdfName.H, PdfBoolean.ValueOf(hidden));
        }

        /// <summary>Creates a Hide action (section 12.6.4.10 of ISO 32000-1).</summary>
        /// <param name="text">
        /// a text string giving the fully qualified field name of an interactive form field whose
        /// associated widget annotation or annotations are to be affected
        /// </param>
        /// <param name="hidden">a flag indicating whether to hide the annotation (<c>true</c>) or show it (<c>false</c>)
        ///     </param>
        /// <returns>created action</returns>
        public static iText.Kernel.Pdf.Action.PdfAction CreateHide(String text, bool hidden) {
            return new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.Hide).Put(PdfName.T, new PdfString(text
                )).Put(PdfName.H, PdfBoolean.ValueOf(hidden));
        }

        /// <summary>Creates a Hide action (section 12.6.4.10 of ISO 32000-1).</summary>
        /// <param name="text">
        /// a text string array giving the fully qualified field names of interactive form fields whose
        /// associated widget annotation or annotations are to be affected
        /// </param>
        /// <param name="hidden">a flag indicating whether to hide the annotation (<c>true</c>) or show it (<c>false</c>)
        ///     </param>
        /// <returns>created action</returns>
        public static iText.Kernel.Pdf.Action.PdfAction CreateHide(String[] text, bool hidden) {
            return new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.Hide).Put(PdfName.T, GetArrayFromStringList
                (text)).Put(PdfName.H, PdfBoolean.ValueOf(hidden));
        }

        /// <summary>Creates a Named action (section 12.6.4.11 of ISO 32000-1).</summary>
        /// <param name="namedAction">
        /// the name of the action that shall be performed. Shall be one of the following:
        /// <see cref="iText.Kernel.Pdf.PdfName.NextPage"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.PrevPage"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.FirstPage"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.LastPage"/>
        /// </param>
        /// <returns>created action</returns>
        public static iText.Kernel.Pdf.Action.PdfAction CreateNamed(PdfName namedAction) {
            return new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.Named).Put(PdfName.N, namedAction);
        }

        /// <summary>Creates a Set-OCG-State action (section 12.6.4.12 of ISO 32000-1).</summary>
        /// <param name="states">
        /// a list of
        /// <see cref="PdfActionOcgState"/>
        /// state descriptions
        /// </param>
        /// <returns>created action</returns>
        public static iText.Kernel.Pdf.Action.PdfAction CreateSetOcgState(IList<PdfActionOcgState> states) {
            return CreateSetOcgState(states, false);
        }

        /// <summary>Creates a Set-OCG-State action (section 12.6.4.12 of ISO 32000-1).</summary>
        /// <param name="states">
        /// states a list of
        /// <see cref="PdfActionOcgState"/>
        /// state descriptions
        /// </param>
        /// <param name="preserveRb">
        /// If true, indicates that radio-button state relationships between optional content groups
        /// should be preserved when the states are applied
        /// </param>
        /// <returns>created action</returns>
        public static iText.Kernel.Pdf.Action.PdfAction CreateSetOcgState(IList<PdfActionOcgState> states, bool preserveRb
            ) {
            PdfArray stateArr = new PdfArray();
            foreach (PdfActionOcgState state in states) {
                stateArr.AddAll(state.GetObjectList());
            }
            return new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.SetOCGState).Put(PdfName.State, stateArr
                ).Put(PdfName.PreserveRB, PdfBoolean.ValueOf(preserveRb));
        }

        /// <summary>Creates a Rendition action (section 12.6.4.13 of ISO 32000-1).</summary>
        /// <param name="file">the name of the media clip, for use in the user interface.</param>
        /// <param name="fileSpec">a full file specification or form XObject that specifies the actual media data</param>
        /// <param name="mimeType">an ASCII string identifying the type of data</param>
        /// <param name="screenAnnotation">a screen annotation</param>
        /// <returns>created action</returns>
        public static iText.Kernel.Pdf.Action.PdfAction CreateRendition(String file, PdfFileSpec fileSpec, String 
            mimeType, PdfAnnotation screenAnnotation) {
            return new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.Rendition).Put(PdfName.OP, new PdfNumber
                (0)).Put(PdfName.AN, screenAnnotation.GetPdfObject()).Put(PdfName.R, new PdfRendition(file, fileSpec, 
                mimeType).GetPdfObject());
        }

        /// <summary>Creates a JavaScript action (section 12.6.4.16 of ISO 32000-1).</summary>
        /// <param name="javaScript">a text string containing the JavaScript script to be executed.</param>
        /// <returns>created action</returns>
        public static iText.Kernel.Pdf.Action.PdfAction CreateJavaScript(String javaScript) {
            return new iText.Kernel.Pdf.Action.PdfAction().Put(PdfName.S, PdfName.JavaScript).Put(PdfName.JS, new PdfString
                (javaScript));
        }

        /// <summary>Creates a Submit-Form Action (section 12.7.5.2 of ISO 32000-1).</summary>
        /// <param name="file">a uniform resource locator, as described in 7.11.5, "URL Specifications"</param>
        /// <param name="names">
        /// an array identifying which fields to include in the submission or which to exclude,
        /// depending on the setting of the Include/Exclude flag in the Flags entry.
        /// This is an optional parameter and can be <c>null</c>
        /// </param>
        /// <param name="flags">
        /// a set of flags specifying various characteristics of the action (see Table 237 of ISO 32000-1).
        /// Default value to be passed: 0.
        /// </param>
        /// <returns>created action</returns>
        public static iText.Kernel.Pdf.Action.PdfAction CreateSubmitForm(String file, Object[] names, int flags) {
            iText.Kernel.Pdf.Action.PdfAction action = new iText.Kernel.Pdf.Action.PdfAction();
            action.Put(PdfName.S, PdfName.SubmitForm);
            PdfDictionary urlFileSpec = new PdfDictionary();
            urlFileSpec.Put(PdfName.F, new PdfString(file));
            urlFileSpec.Put(PdfName.FS, PdfName.URL);
            action.Put(PdfName.F, urlFileSpec);
            if (names != null) {
                action.Put(PdfName.Fields, BuildArray(names));
            }
            action.Put(PdfName.Flags, new PdfNumber(flags));
            return action;
        }

        /// <summary>Creates a Reset-Form Action (section 12.7.5.3 of ISO 32000-1).</summary>
        /// <param name="names">
        /// an array identifying which fields to reset or which to exclude from resetting,
        /// depending on the setting of the Include/Exclude flag in the Flags entry (see Table 239 of ISO 32000-1).
        /// </param>
        /// <param name="flags">
        /// a set of flags specifying various characteristics of the action (see Table 239 of ISO 32000-1).
        /// Default value to be passed: 0.
        /// </param>
        /// <returns>created action</returns>
        public static iText.Kernel.Pdf.Action.PdfAction CreateResetForm(Object[] names, int flags) {
            iText.Kernel.Pdf.Action.PdfAction action = new iText.Kernel.Pdf.Action.PdfAction();
            action.Put(PdfName.S, PdfName.ResetForm);
            if (names != null) {
                action.Put(PdfName.Fields, BuildArray(names));
            }
            action.Put(PdfName.Flags, new PdfNumber(flags));
            return action;
        }

        /// <summary>
        /// Adds an additional action to the provided
        /// <see cref="iText.Kernel.Pdf.PdfObjectWrapper{T}"/>
        /// &lt;
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// &gt; wrapper.
        /// </summary>
        /// <param name="wrapper">the wrapper to add an additional action to</param>
        /// <param name="key">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// specifying the name of an additional action
        /// </param>
        /// <param name="action">
        /// the
        /// <see cref="PdfAction"/>
        /// to add as an additional action
        /// </param>
        public static void SetAdditionalAction(PdfObjectWrapper<PdfDictionary> wrapper, PdfName key, iText.Kernel.Pdf.Action.PdfAction
             action) {
            PdfDictionary dic;
            PdfObject obj = wrapper.GetPdfObject().Get(PdfName.AA);
            bool aaExists = obj != null && obj.IsDictionary();
            if (aaExists) {
                dic = (PdfDictionary)obj;
            }
            else {
                dic = new PdfDictionary();
            }
            dic.Put(key, action.GetPdfObject());
            dic.SetModified();
            wrapper.GetPdfObject().Put(PdfName.AA, dic);
            if (!aaExists || !dic.IsIndirect()) {
                wrapper.GetPdfObject().SetModified();
            }
        }

        /// <summary>Adds a chained action.</summary>
        /// <param name="nextAction">the next action or sequence of actions that shall be performed after the current action
        ///     </param>
        public virtual void Next(iText.Kernel.Pdf.Action.PdfAction nextAction) {
            PdfObject currentNextAction = GetPdfObject().Get(PdfName.Next);
            if (currentNextAction == null) {
                Put(PdfName.Next, nextAction.GetPdfObject());
            }
            else {
                if (currentNextAction.IsDictionary()) {
                    PdfArray array = new PdfArray(currentNextAction);
                    array.Add(nextAction.GetPdfObject());
                    Put(PdfName.Next, array);
                }
                else {
                    ((PdfArray)currentNextAction).Add(nextAction.GetPdfObject());
                }
            }
        }

        /// <summary>
        /// Inserts the value into the underlying object of this
        /// <see cref="PdfAction"/>
        /// and associates it with the specified key.
        /// </summary>
        /// <remarks>
        /// Inserts the value into the underlying object of this
        /// <see cref="PdfAction"/>
        /// and associates it with the specified key.
        /// If the key is already present in this
        /// <see cref="PdfAction"/>
        /// , this method will override the old value with the specified one.
        /// </remarks>
        /// <param name="key">key to insert or to override</param>
        /// <param name="value">the value to associate with the specified key</param>
        /// <returns>
        /// this
        /// <see cref="PdfAction"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.Action.PdfAction Put(PdfName key, PdfObject value) {
            GetPdfObject().Put(key, value);
            SetModified();
            return this;
        }

        /// <summary>
        /// To manually flush a
        /// <c>PdfObject</c>
        /// behind this wrapper, you have to ensure
        /// that this object is added to the document, i.e. it has an indirect reference.
        /// </summary>
        /// <remarks>
        /// To manually flush a
        /// <c>PdfObject</c>
        /// behind this wrapper, you have to ensure
        /// that this object is added to the document, i.e. it has an indirect reference.
        /// Basically this means that before flushing you need to explicitly call
        /// <see cref="iText.Kernel.Pdf.PdfObjectWrapper{T}.MakeIndirect(iText.Kernel.Pdf.PdfDocument)"/>.
        /// For example: wrapperInstance.makeIndirect(document).flush();
        /// Note that not every wrapper require this, only those that have such warning in documentation.
        /// </remarks>
        public override void Flush() {
            base.Flush();
        }

        /// <summary><inheritDoc/></summary>
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
                        throw new PdfException("The array must contain string or PDFAnnotation");
                    }
                }
            }
            return array;
        }

        private static void ValidateRemoteDestination(PdfDestination destination) {
            // No page object can be specified for a destination associated with a remote go-to action because the
            // destination page is in a different PDF document. In this case, the page parameter specifies an integer
            // page number within the remote document instead of a page object in the current document.
            // See section 12.3.2.2 of ISO 32000-1.
            if (destination is PdfExplicitDestination) {
                PdfObject firstObj = ((PdfArray)destination.GetPdfObject()).Get(0);
                if (firstObj.IsDictionary()) {
                    throw new ArgumentException("Explicit destinations shall specify page number in remote go-to actions instead of page dictionary"
                        );
                }
            }
            else {
                if (destination is PdfStructureDestination) {
                    // No structure element dictionary can be specified for a structure destination associated with a remote
                    // go-to action because the destination structure element is in a
                    // different PDF document. In this case, the indirect reference to the structure element dictionary shall be
                    // replaced by a byte string representing a structure element ID
                    PdfObject firstObj = ((PdfArray)destination.GetPdfObject()).Get(0);
                    if (firstObj.IsDictionary()) {
                        PdfDictionary structElemObj = (PdfDictionary)firstObj;
                        PdfString id = structElemObj.GetAsString(PdfName.ID);
                        if (id == null) {
                            throw new ArgumentException("Structure destinations shall specify structure element ID in remote go-to actions. Structure element that has no ID is specified instead"
                                );
                        }
                        else {
                            ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.Action.PdfAction)).LogWarning(iText.IO.Logs.IoLogMessageConstant
                                .STRUCTURE_ELEMENT_REPLACED_BY_ITS_ID_IN_STRUCTURE_DESTINATION);
                            ((PdfArray)destination.GetPdfObject()).Set(0, id);
                            destination.GetPdfObject().SetModified();
                        }
                    }
                }
            }
        }

        /// <summary>Validates not remote destination against the PDF specification and in case of invalidity logs a warning.
        ///     </summary>
        /// <remarks>
        /// Validates not remote destination against the PDF specification and in case of invalidity logs a warning.
        /// See section 12.3.2.2 of ISO 32000-1.
        /// </remarks>
        /// <param name="destination">
        /// the
        /// <see cref="iText.Kernel.Pdf.Navigation.PdfDestination">destination</see>
        /// to be validated
        /// </param>
        private static void ValidateNotRemoteDestination(PdfDestination destination) {
            if (destination is PdfExplicitRemoteGoToDestination) {
                ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.Action.PdfAction)).LogWarning(iText.IO.Logs.IoLogMessageConstant
                    .INVALID_DESTINATION_TYPE);
            }
            else {
                if (destination is PdfExplicitDestination) {
                    // No page number can be specified for a destination associated with a not remote go-to action because the
                    // destination page is in a current PDF document. See section 12.3.2.2 of ISO 32000-1.
                    PdfObject firstObj = ((PdfArray)destination.GetPdfObject()).Get(0);
                    if (firstObj.IsNumber()) {
                        ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.Action.PdfAction)).LogWarning(iText.IO.Logs.IoLogMessageConstant
                            .INVALID_DESTINATION_TYPE);
                    }
                }
            }
        }
    }
}
