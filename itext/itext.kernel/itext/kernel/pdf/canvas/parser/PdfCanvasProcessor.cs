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
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Source;
using iText.Kernel.Colors;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser.Util;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Extgstate;

namespace iText.Kernel.Pdf.Canvas.Parser {
    /// <summary>Processor for a PDF content stream.</summary>
    public class PdfCanvasProcessor {
        public const String DEFAULT_OPERATOR = "DefaultOperator";

        /// <summary>Listener that will be notified of render events</summary>
        protected internal readonly IEventListener eventListener;

        /// <summary>
        /// Cache supported events in case the user's
        /// <see cref="iText.Kernel.Pdf.Canvas.Parser.Listener.IEventListener.GetSupportedEvents()"/>
        /// method is not very efficient
        /// </summary>
        protected internal readonly ICollection<EventType> supportedEvents;

        protected internal Path currentPath = new Path();

        /// <summary>
        /// Indicates whether the current clipping path should be modified by
        /// intersecting it with the current path.
        /// </summary>
        protected internal bool isClip;

        /// <summary>
        /// Specifies the filling rule which should be applied while calculating
        /// new clipping path.
        /// </summary>
        protected internal int clippingRule;

        /// <summary>A map with all supported operators (PDF syntax).</summary>
        private IDictionary<String, IContentOperator> operators;

        /// <summary>Resources for the content stream.</summary>
        /// <remarks>
        /// Resources for the content stream.
        /// Current resources are always at the top of the stack.
        /// Stack is needed in case if some "inner" content stream with it's own resources
        /// is encountered (like Form XObject).
        /// </remarks>
        private IList<PdfResources> resourcesStack;

        /// <summary>Stack keeping track of the graphics state.</summary>
        private readonly Stack<ParserGraphicsState> gsStack = new Stack<ParserGraphicsState>();

        private Matrix textMatrix;

        private Matrix textLineMatrix;

        /// <summary>A map with all supported XObject handlers</summary>
        private IDictionary<PdfName, IXObjectDoHandler> xobjectDoHandlers;

        /// <summary>The font cache</summary>
        private IDictionary<int, WeakReference> cachedFonts = new Dictionary<int, WeakReference>();

        /// <summary>A stack containing marked content info.</summary>
        private Stack<CanvasTag> markedContentStack = new Stack<CanvasTag>();

        /// <summary>A memory limits handler.</summary>
        private MemoryLimitsAwareHandler memoryLimitsHandler = null;

        /// <summary>Page size in bytes.</summary>
        private long pageSize = 0;

        /// <summary>
        /// Creates a new PDF Content Stream Processor that will send its output to the
        /// designated render listener.
        /// </summary>
        /// <param name="eventListener">
        /// the
        /// <see cref="iText.Kernel.Pdf.Canvas.Parser.Listener.IEventListener"/>
        /// that will receive rendering notifications
        /// </param>
        public PdfCanvasProcessor(IEventListener eventListener) {
            this.eventListener = eventListener;
            this.supportedEvents = eventListener.GetSupportedEvents();
            operators = new Dictionary<String, IContentOperator>();
            PopulateOperators();
            xobjectDoHandlers = new Dictionary<PdfName, IXObjectDoHandler>();
            PopulateXObjectDoHandlers();
            Reset();
        }

        /// <summary>
        /// Creates a new PDF Content Stream Processor that will send its output to the
        /// designated render listener.
        /// </summary>
        /// <remarks>
        /// Creates a new PDF Content Stream Processor that will send its output to the
        /// designated render listener.
        /// Also allows registration of custom IContentOperators that can influence
        /// how (and whether or not) the PDF instructions will be parsed.
        /// </remarks>
        /// <param name="eventListener">
        /// the
        /// <see cref="iText.Kernel.Pdf.Canvas.Parser.Listener.IEventListener"/>
        /// that will receive rendering notifications
        /// </param>
        /// <param name="additionalContentOperators">
        /// an optional map of custom
        /// <see cref="IContentOperator"/>
        /// s for rendering instructions
        /// </param>
        public PdfCanvasProcessor(IEventListener eventListener, IDictionary<String, IContentOperator> additionalContentOperators
            )
            : this(eventListener) {
            foreach (KeyValuePair<String, IContentOperator> entry in additionalContentOperators) {
                RegisterContentOperator(entry.Key, entry.Value);
            }
        }

        /// <summary>Registers a Do handler that will be called when Do for the provided XObject subtype is encountered during content processing.
        ///     </summary>
        /// <remarks>
        /// Registers a Do handler that will be called when Do for the provided XObject subtype is encountered during content processing.
        /// <br />
        /// If you register a handler, it is a very good idea to pass the call on to the existing registered handler (returned by this call), otherwise you
        /// may inadvertently change the internal behavior of the processor.
        /// </remarks>
        /// <param name="xobjectSubType">the XObject subtype this handler will process, or PdfName.DEFAULT for a catch-all handler
        ///     </param>
        /// <param name="handler">the handler that will receive notification when the Do operator for the specified subtype is encountered
        ///     </param>
        /// <returns>the existing registered handler, if any</returns>
        public virtual IXObjectDoHandler RegisterXObjectDoHandler(PdfName xobjectSubType, IXObjectDoHandler handler
            ) {
            return xobjectDoHandlers.Put(xobjectSubType, handler);
        }

        /// <summary>Registers a content operator that will be called when the specified operator string is encountered during content processing.
        ///     </summary>
        /// <remarks>
        /// Registers a content operator that will be called when the specified operator string is encountered during content processing.
        /// <br />
        /// If you register an operator, it is a very good idea to pass the call on to the existing registered operator (returned by this call), otherwise you
        /// may inadvertently change the internal behavior of the processor.
        /// </remarks>
        /// <param name="operatorString">the operator id, or DEFAULT_OPERATOR for a catch-all operator</param>
        /// <param name="operator">the operator that will receive notification when the operator is encountered</param>
        /// <returns>the existing registered operator, if any</returns>
        public virtual IContentOperator RegisterContentOperator(String operatorString, IContentOperator @operator) {
            return operators.Put(operatorString, @operator);
        }

        /// <summary>
        /// Gets the
        /// <see cref="System.Collections.ICollection{E}"/>
        /// containing all the registered operators strings.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="System.Collections.ICollection{E}"/>
        /// containing all the registered operators strings.
        /// </returns>
        public virtual ICollection<String> GetRegisteredOperatorStrings() {
            return new List<String>(operators.Keys);
        }

        /// <summary>Resets the graphics state stack, matrices and resources.</summary>
        public virtual void Reset() {
            memoryLimitsHandler = null;
            pageSize = 0;
            gsStack.Clear();
            gsStack.Push(new ParserGraphicsState());
            textMatrix = null;
            textLineMatrix = null;
            resourcesStack = new List<PdfResources>();
            isClip = false;
            currentPath = new Path();
        }

        /// <summary>
        /// Gets the current
        /// <see cref="ParserGraphicsState"/>
        /// </summary>
        /// <returns>
        /// the current
        /// <see cref="ParserGraphicsState"/>
        /// </returns>
        public virtual ParserGraphicsState GetGraphicsState() {
            return gsStack.Peek();
        }

        /// <summary>Processes PDF syntax.</summary>
        /// <remarks>
        /// Processes PDF syntax.
        /// <b>Note:</b> If you re-use a given
        /// <see cref="PdfCanvasProcessor"/>
        /// , you must call
        /// <see cref="Reset()"/>
        /// </remarks>
        /// <param name="contentBytes">the bytes of a content stream</param>
        /// <param name="resources">the resources of the content stream. Must not be null.</param>
        public virtual void ProcessContent(byte[] contentBytes, PdfResources resources) {
            if (resources == null) {
                throw new PdfException(KernelExceptionMessageConstant.RESOURCES_CANNOT_BE_NULL);
            }
            if (memoryLimitsHandler != null) {
                pageSize += (long)contentBytes.Length;
                memoryLimitsHandler.CheckIfPageSizeExceedsTheLimit(this.pageSize);
            }
            this.resourcesStack.Add(resources);
            PdfTokenizer tokeniser = new PdfTokenizer(new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateSource
                (contentBytes)));
            PdfCanvasParser ps = new PdfCanvasParser(tokeniser, resources);
            IList<PdfObject> operands = new List<PdfObject>();
            try {
                while (ps.Parse(operands).Count > 0) {
                    PdfLiteral @operator = (PdfLiteral)operands[operands.Count - 1];
                    InvokeOperator(@operator, operands);
                }
            }
            catch (System.IO.IOException e) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_PARSE_CONTENT_STREAM, e);
            }
            this.resourcesStack.JRemoveAt(resourcesStack.Count - 1);
        }

        /// <summary>Processes PDF syntax.</summary>
        /// <remarks>
        /// Processes PDF syntax.
        /// <strong>Note:</strong> If you re-use a given
        /// <see cref="PdfCanvasProcessor"/>
        /// , you must call
        /// <see cref="Reset()"/>
        /// </remarks>
        /// <param name="page">the page to process</param>
        public virtual void ProcessPageContent(PdfPage page) {
            this.memoryLimitsHandler = page.GetDocument().GetMemoryLimitsAwareHandler();
            InitClippingPath(page);
            ParserGraphicsState gs = GetGraphicsState();
            EventOccurred(new ClippingPathInfo(gs, gs.GetClippingPath(), gs.GetCtm()), EventType.CLIP_PATH_CHANGED);
            ProcessContent(page.GetContentBytes(), page.GetResources());
        }

        /// <summary>
        /// Accessor method for the
        /// <see cref="iText.Kernel.Pdf.Canvas.Parser.Listener.IEventListener"/>
        /// object maintained in this class.
        /// </summary>
        /// <remarks>
        /// Accessor method for the
        /// <see cref="iText.Kernel.Pdf.Canvas.Parser.Listener.IEventListener"/>
        /// object maintained in this class.
        /// Necessary for implementing custom ContentOperator implementations.
        /// </remarks>
        /// <returns>the renderListener</returns>
        public virtual IEventListener GetEventListener() {
            return eventListener;
        }

        /// <summary>Loads all the supported graphics and text state operators in a map.</summary>
        protected internal virtual void PopulateOperators() {
            RegisterContentOperator(DEFAULT_OPERATOR, new PdfCanvasProcessor.IgnoreOperator());
            RegisterContentOperator("q", new PdfCanvasProcessor.PushGraphicsStateOperator());
            RegisterContentOperator("Q", new PdfCanvasProcessor.PopGraphicsStateOperator());
            RegisterContentOperator("cm", new PdfCanvasProcessor.ModifyCurrentTransformationMatrixOperator());
            RegisterContentOperator("Do", new PdfCanvasProcessor.DoOperator());
            RegisterContentOperator("BMC", new PdfCanvasProcessor.BeginMarkedContentOperator());
            RegisterContentOperator("BDC", new PdfCanvasProcessor.BeginMarkedContentDictionaryOperator());
            RegisterContentOperator("EMC", new PdfCanvasProcessor.EndMarkedContentOperator());
            if (supportedEvents == null || supportedEvents.Contains(EventType.RENDER_TEXT) || supportedEvents.Contains
                (EventType.RENDER_PATH) || supportedEvents.Contains(EventType.CLIP_PATH_CHANGED)) {
                RegisterContentOperator("g", new PdfCanvasProcessor.SetGrayFillOperator());
                RegisterContentOperator("G", new PdfCanvasProcessor.SetGrayStrokeOperator());
                RegisterContentOperator("rg", new PdfCanvasProcessor.SetRGBFillOperator());
                RegisterContentOperator("RG", new PdfCanvasProcessor.SetRGBStrokeOperator());
                RegisterContentOperator("k", new PdfCanvasProcessor.SetCMYKFillOperator());
                RegisterContentOperator("K", new PdfCanvasProcessor.SetCMYKStrokeOperator());
                RegisterContentOperator("cs", new PdfCanvasProcessor.SetColorSpaceFillOperator());
                RegisterContentOperator("CS", new PdfCanvasProcessor.SetColorSpaceStrokeOperator());
                RegisterContentOperator("sc", new PdfCanvasProcessor.SetColorFillOperator());
                RegisterContentOperator("SC", new PdfCanvasProcessor.SetColorStrokeOperator());
                RegisterContentOperator("scn", new PdfCanvasProcessor.SetColorFillOperator());
                RegisterContentOperator("SCN", new PdfCanvasProcessor.SetColorStrokeOperator());
                RegisterContentOperator("gs", new PdfCanvasProcessor.ProcessGraphicsStateResourceOperator());
            }
            if (supportedEvents == null || supportedEvents.Contains(EventType.RENDER_IMAGE)) {
                RegisterContentOperator("EI", new PdfCanvasProcessor.EndImageOperator());
            }
            if (supportedEvents == null || supportedEvents.Contains(EventType.RENDER_TEXT) || supportedEvents.Contains
                (EventType.BEGIN_TEXT) || supportedEvents.Contains(EventType.END_TEXT)) {
                RegisterContentOperator("BT", new PdfCanvasProcessor.BeginTextOperator());
                RegisterContentOperator("ET", new PdfCanvasProcessor.EndTextOperator());
            }
            if (supportedEvents == null || supportedEvents.Contains(EventType.RENDER_TEXT)) {
                PdfCanvasProcessor.SetTextCharacterSpacingOperator tcOperator = new PdfCanvasProcessor.SetTextCharacterSpacingOperator
                    ();
                RegisterContentOperator("Tc", tcOperator);
                PdfCanvasProcessor.SetTextWordSpacingOperator twOperator = new PdfCanvasProcessor.SetTextWordSpacingOperator
                    ();
                RegisterContentOperator("Tw", twOperator);
                RegisterContentOperator("Tz", new PdfCanvasProcessor.SetTextHorizontalScalingOperator());
                PdfCanvasProcessor.SetTextLeadingOperator tlOperator = new PdfCanvasProcessor.SetTextLeadingOperator();
                RegisterContentOperator("TL", tlOperator);
                RegisterContentOperator("Tf", new PdfCanvasProcessor.SetTextFontOperator());
                RegisterContentOperator("Tr", new PdfCanvasProcessor.SetTextRenderModeOperator());
                RegisterContentOperator("Ts", new PdfCanvasProcessor.SetTextRiseOperator());
                PdfCanvasProcessor.TextMoveStartNextLineOperator tdOperator = new PdfCanvasProcessor.TextMoveStartNextLineOperator
                    ();
                RegisterContentOperator("Td", tdOperator);
                RegisterContentOperator("TD", new PdfCanvasProcessor.TextMoveStartNextLineWithLeadingOperator(tdOperator, 
                    tlOperator));
                RegisterContentOperator("Tm", new PdfCanvasProcessor.TextSetTextMatrixOperator());
                PdfCanvasProcessor.TextMoveNextLineOperator tstarOperator = new PdfCanvasProcessor.TextMoveNextLineOperator
                    (tdOperator);
                RegisterContentOperator("T*", tstarOperator);
                PdfCanvasProcessor.ShowTextOperator tjOperator = new PdfCanvasProcessor.ShowTextOperator();
                RegisterContentOperator("Tj", tjOperator);
                PdfCanvasProcessor.MoveNextLineAndShowTextOperator tickOperator = new PdfCanvasProcessor.MoveNextLineAndShowTextOperator
                    (tstarOperator, tjOperator);
                RegisterContentOperator("'", tickOperator);
                RegisterContentOperator("\"", new PdfCanvasProcessor.MoveNextLineAndShowTextWithSpacingOperator(twOperator
                    , tcOperator, tickOperator));
                RegisterContentOperator("TJ", new PdfCanvasProcessor.ShowTextArrayOperator());
            }
            if (supportedEvents == null || supportedEvents.Contains(EventType.CLIP_PATH_CHANGED) || supportedEvents.Contains
                (EventType.RENDER_PATH)) {
                RegisterContentOperator("w", new PdfCanvasProcessor.SetLineWidthOperator());
                RegisterContentOperator("J", new PdfCanvasProcessor.SetLineCapOperator());
                RegisterContentOperator("j", new PdfCanvasProcessor.SetLineJoinOperator());
                RegisterContentOperator("M", new PdfCanvasProcessor.SetMiterLimitOperator());
                RegisterContentOperator("d", new PdfCanvasProcessor.SetLineDashPatternOperator());
                int fillStroke = PathRenderInfo.FILL | PathRenderInfo.STROKE;
                RegisterContentOperator("m", new PdfCanvasProcessor.MoveToOperator());
                RegisterContentOperator("l", new PdfCanvasProcessor.LineToOperator());
                RegisterContentOperator("c", new PdfCanvasProcessor.CurveOperator());
                RegisterContentOperator("v", new PdfCanvasProcessor.CurveFirstPointDuplicatedOperator());
                RegisterContentOperator("y", new PdfCanvasProcessor.CurveFourhPointDuplicatedOperator());
                RegisterContentOperator("h", new PdfCanvasProcessor.CloseSubpathOperator());
                RegisterContentOperator("re", new PdfCanvasProcessor.RectangleOperator());
                RegisterContentOperator("S", new PdfCanvasProcessor.PaintPathOperator(PathRenderInfo.STROKE, -1, false));
                RegisterContentOperator("s", new PdfCanvasProcessor.PaintPathOperator(PathRenderInfo.STROKE, -1, true));
                RegisterContentOperator("f", new PdfCanvasProcessor.PaintPathOperator(PathRenderInfo.FILL, PdfCanvasConstants.FillingRule
                    .NONZERO_WINDING, false));
                RegisterContentOperator("F", new PdfCanvasProcessor.PaintPathOperator(PathRenderInfo.FILL, PdfCanvasConstants.FillingRule
                    .NONZERO_WINDING, false));
                RegisterContentOperator("f*", new PdfCanvasProcessor.PaintPathOperator(PathRenderInfo.FILL, PdfCanvasConstants.FillingRule
                    .EVEN_ODD, false));
                RegisterContentOperator("B", new PdfCanvasProcessor.PaintPathOperator(fillStroke, PdfCanvasConstants.FillingRule
                    .NONZERO_WINDING, false));
                RegisterContentOperator("B*", new PdfCanvasProcessor.PaintPathOperator(fillStroke, PdfCanvasConstants.FillingRule
                    .EVEN_ODD, false));
                RegisterContentOperator("b", new PdfCanvasProcessor.PaintPathOperator(fillStroke, PdfCanvasConstants.FillingRule
                    .NONZERO_WINDING, true));
                RegisterContentOperator("b*", new PdfCanvasProcessor.PaintPathOperator(fillStroke, PdfCanvasConstants.FillingRule
                    .EVEN_ODD, true));
                RegisterContentOperator("n", new PdfCanvasProcessor.PaintPathOperator(PathRenderInfo.NO_OP, -1, false));
                RegisterContentOperator("W", new PdfCanvasProcessor.ClipPathOperator(PdfCanvasConstants.FillingRule.NONZERO_WINDING
                    ));
                RegisterContentOperator("W*", new PdfCanvasProcessor.ClipPathOperator(PdfCanvasConstants.FillingRule.EVEN_ODD
                    ));
            }
        }

        /// <summary>Displays the current path.</summary>
        /// <param name="operation">
        /// One of the possible combinations of
        /// <see cref="iText.Kernel.Pdf.Canvas.Parser.Data.PathRenderInfo.STROKE"/>
        /// and
        /// <see cref="iText.Kernel.Pdf.Canvas.Parser.Data.PathRenderInfo.FILL"/>
        /// values or
        /// <see cref="iText.Kernel.Pdf.Canvas.Parser.Data.PathRenderInfo.NO_OP"/>
        /// </param>
        /// <param name="rule">
        /// Either
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.FillingRule.NONZERO_WINDING"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.FillingRule.EVEN_ODD"/>
        /// In case it isn't applicable pass any <c>byte</c> value.
        /// </param>
        protected internal virtual void PaintPath(int operation, int rule) {
            ParserGraphicsState gs = GetGraphicsState();
            PathRenderInfo renderInfo = new PathRenderInfo(this.markedContentStack, gs, currentPath, operation, rule, 
                isClip, clippingRule);
            EventOccurred(renderInfo, EventType.RENDER_PATH);
            if (isClip) {
                isClip = false;
                gs.Clip(currentPath, clippingRule);
                EventOccurred(new ClippingPathInfo(gs, gs.GetClippingPath(), gs.GetCtm()), EventType.CLIP_PATH_CHANGED);
            }
            currentPath = new Path();
        }

        /// <summary>Invokes an operator.</summary>
        /// <param name="operator">the PDF Syntax of the operator</param>
        /// <param name="operands">a list with operands</param>
        protected internal virtual void InvokeOperator(PdfLiteral @operator, IList<PdfObject> operands) {
            IContentOperator op = operators.Get(@operator.ToString());
            if (op == null) {
                op = operators.Get(DEFAULT_OPERATOR);
            }
            op.Invoke(this, @operator, operands);
        }

        protected internal virtual PdfStream GetXObjectStream(PdfName xobjectName) {
            PdfDictionary xobjects = GetResources().GetResource(PdfName.XObject);
            return xobjects.GetAsStream(xobjectName);
        }

        protected internal virtual PdfResources GetResources() {
            return resourcesStack[resourcesStack.Count - 1];
        }

        protected internal virtual void PopulateXObjectDoHandlers() {
            RegisterXObjectDoHandler(PdfName.Default, new PdfCanvasProcessor.IgnoreXObjectDoHandler());
            RegisterXObjectDoHandler(PdfName.Form, new PdfCanvasProcessor.FormXObjectDoHandler());
            if (supportedEvents == null || supportedEvents.Contains(EventType.RENDER_IMAGE)) {
                RegisterXObjectDoHandler(PdfName.Image, new PdfCanvasProcessor.ImageXObjectDoHandler());
            }
        }

        /// <summary>
        /// Creates a
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// object by a font dictionary.
        /// </summary>
        /// <remarks>
        /// Creates a
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// object by a font dictionary. The font may have been cached in case
        /// it is an indirect object.
        /// </remarks>
        /// <param name="fontDict">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDictionary">font dictionary</see>
        /// to create the font from
        /// </param>
        /// <returns>the created font</returns>
        protected internal virtual PdfFont GetFont(PdfDictionary fontDict) {
            if (fontDict.GetIndirectReference() == null) {
                return PdfFontFactory.CreateFont(fontDict);
            }
            else {
                int n = fontDict.GetIndirectReference().GetObjNumber();
                WeakReference fontRef = cachedFonts.Get(n);
                PdfFont font = (PdfFont)(fontRef == null ? null : fontRef.Target);
                if (font == null) {
                    font = PdfFontFactory.CreateFont(fontDict);
                    cachedFonts.Put(n, new WeakReference(font));
                }
                return font;
            }
        }

        /// <summary>Add to the marked content stack</summary>
        /// <param name="tag">the tag of the marked content</param>
        /// <param name="dict">the PdfDictionary associated with the marked content</param>
        protected internal virtual void BeginMarkedContent(PdfName tag, PdfDictionary dict) {
            markedContentStack.Push(new CanvasTag(tag).SetProperties(dict));
        }

        /// <summary>Remove the latest marked content from the stack.</summary>
        /// <remarks>Remove the latest marked content from the stack.  Keeps track of the BMC, BDC and EMC operators.</remarks>
        protected internal virtual void EndMarkedContent() {
            markedContentStack.Pop();
        }

        /// <summary>Used to trigger beginTextBlock on the renderListener</summary>
        private void BeginText() {
            EventOccurred(null, EventType.BEGIN_TEXT);
        }

        /// <summary>Used to trigger endTextBlock on the renderListener</summary>
        private void EndText() {
            EventOccurred(null, EventType.END_TEXT);
        }

        /// <summary>This is a proxy to pass only those events to the event listener which are supported by it.</summary>
        /// <param name="data">event data</param>
        /// <param name="type">event type</param>
        protected internal virtual void EventOccurred(IEventData data, EventType type) {
            if (supportedEvents == null || supportedEvents.Contains(type)) {
                eventListener.EventOccurred(data, type);
            }
            if (data is AbstractRenderInfo) {
                ((AbstractRenderInfo)data).ReleaseGraphicsState();
            }
        }

        /// <summary>Displays text.</summary>
        /// <param name="string">the text to display</param>
        private void DisplayPdfString(PdfString @string) {
            TextRenderInfo renderInfo = new TextRenderInfo(@string, GetGraphicsState(), textMatrix, markedContentStack
                );
            textMatrix = new Matrix(renderInfo.GetUnscaledWidth(), 0).Multiply(textMatrix);
            EventOccurred(renderInfo, EventType.RENDER_TEXT);
        }

        /// <summary>Displays an XObject using the registered handler for this XObject's subtype</summary>
        /// <param name="resourceName">the name of the XObject to retrieve from the resource dictionary</param>
        private void DisplayXObject(PdfName resourceName) {
            PdfStream xobjectStream = GetXObjectStream(resourceName);
            PdfName subType = xobjectStream.GetAsName(PdfName.Subtype);
            IXObjectDoHandler handler = xobjectDoHandlers.Get(subType);
            if (handler == null) {
                handler = xobjectDoHandlers.Get(PdfName.Default);
            }
            handler.HandleXObject(this, this.markedContentStack, xobjectStream, resourceName);
        }

        private void DisplayImage(Stack<CanvasTag> canvasTagHierarchy, PdfStream imageStream, PdfName resourceName
            , bool isInline) {
            PdfDictionary colorSpaceDic = GetResources().GetResource(PdfName.ColorSpace);
            ImageRenderInfo renderInfo = new ImageRenderInfo(canvasTagHierarchy, GetGraphicsState(), GetGraphicsState(
                ).GetCtm(), imageStream, resourceName, colorSpaceDic, isInline);
            EventOccurred(renderInfo, EventType.RENDER_IMAGE);
        }

        /// <summary>Adjusts the text matrix for the specified adjustment value (see TJ operator in the PDF spec for information)
        ///     </summary>
        /// <param name="tj">the text adjustment</param>
        private void ApplyTextAdjust(float tj) {
            float adjustBy = FontProgram.ConvertTextSpaceToGlyphSpace(-tj) * GetGraphicsState().GetFontSize() * (GetGraphicsState
                ().GetHorizontalScaling() / 100F);
            textMatrix = new Matrix(adjustBy, 0).Multiply(textMatrix);
        }

        private void InitClippingPath(PdfPage page) {
            Path clippingPath = new Path();
            clippingPath.Rectangle(page.GetCropBox());
            GetGraphicsState().SetClippingPath(clippingPath);
        }

        /// <summary>A handler that implements operator (unregistered).</summary>
        private class IgnoreOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
            }
            // ignore the operator
        }

        /// <summary>A handler that implements operator (TJ).</summary>
        /// <remarks>A handler that implements operator (TJ). For more information see Table 51 ISO-32000-1</remarks>
        private class ShowTextArrayOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                PdfArray array = (PdfArray)operands[0];
                float tj = 0;
                foreach (PdfObject entryObj in array) {
                    if (entryObj is PdfString) {
                        processor.DisplayPdfString((PdfString)entryObj);
                        tj = 0;
                    }
                    else {
                        tj = ((PdfNumber)entryObj).FloatValue();
                        processor.ApplyTextAdjust(tj);
                    }
                }
            }
        }

        /// <summary>A handler that implements operator (").</summary>
        /// <remarks>A handler that implements operator ("). For more information see Table 51 ISO-32000-1</remarks>
        private class MoveNextLineAndShowTextWithSpacingOperator : IContentOperator {
            private readonly PdfCanvasProcessor.SetTextWordSpacingOperator setTextWordSpacing;

            private readonly PdfCanvasProcessor.SetTextCharacterSpacingOperator setTextCharacterSpacing;

            private readonly PdfCanvasProcessor.MoveNextLineAndShowTextOperator moveNextLineAndShowText;

            /// <summary>Create new instance of this handler.</summary>
            /// <param name="setTextWordSpacing">the handler for Tw operator</param>
            /// <param name="setTextCharacterSpacing">the handler for Tc operator</param>
            /// <param name="moveNextLineAndShowText">the handler for ' operator</param>
            public MoveNextLineAndShowTextWithSpacingOperator(PdfCanvasProcessor.SetTextWordSpacingOperator setTextWordSpacing
                , PdfCanvasProcessor.SetTextCharacterSpacingOperator setTextCharacterSpacing, PdfCanvasProcessor.MoveNextLineAndShowTextOperator
                 moveNextLineAndShowText) {
                this.setTextWordSpacing = setTextWordSpacing;
                this.setTextCharacterSpacing = setTextCharacterSpacing;
                this.moveNextLineAndShowText = moveNextLineAndShowText;
            }

            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                PdfNumber aw = (PdfNumber)operands[0];
                PdfNumber ac = (PdfNumber)operands[1];
                PdfString @string = (PdfString)operands[2];
                IList<PdfObject> twOperands = new List<PdfObject>(1);
                twOperands.Add(0, aw);
                setTextWordSpacing.Invoke(processor, null, twOperands);
                IList<PdfObject> tcOperands = new List<PdfObject>(1);
                tcOperands.Add(0, ac);
                setTextCharacterSpacing.Invoke(processor, null, tcOperands);
                IList<PdfObject> tickOperands = new List<PdfObject>(1);
                tickOperands.Add(0, @string);
                moveNextLineAndShowText.Invoke(processor, null, tickOperands);
            }
        }

        /// <summary>A handler that implements operator (').</summary>
        /// <remarks>A handler that implements operator ('). For more information see Table 51 ISO-32000-1</remarks>
        private class MoveNextLineAndShowTextOperator : IContentOperator {
            private readonly PdfCanvasProcessor.TextMoveNextLineOperator textMoveNextLine;

            private readonly PdfCanvasProcessor.ShowTextOperator showText;

            /// <summary>Creates the new instance of this handler</summary>
            /// <param name="textMoveNextLine">the handler for T* operator</param>
            /// <param name="showText">the handler for Tj operator</param>
            public MoveNextLineAndShowTextOperator(PdfCanvasProcessor.TextMoveNextLineOperator textMoveNextLine, PdfCanvasProcessor.ShowTextOperator
                 showText) {
                this.textMoveNextLine = textMoveNextLine;
                this.showText = showText;
            }

            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                textMoveNextLine.Invoke(processor, null, new List<PdfObject>(0));
                showText.Invoke(processor, null, operands);
            }
        }

        /// <summary>A handler that implements operator (Tj).</summary>
        /// <remarks>A handler that implements operator (Tj). For more information see Table 51 ISO-32000-1</remarks>
        private class ShowTextOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                PdfString @string = (PdfString)operands[0];
                processor.DisplayPdfString(@string);
            }
        }

        /// <summary>A handler that implements operator (T*).</summary>
        /// <remarks>A handler that implements operator (T*). For more information see Table 51 ISO-32000-1</remarks>
        private class TextMoveNextLineOperator : IContentOperator {
            private readonly PdfCanvasProcessor.TextMoveStartNextLineOperator moveStartNextLine;

            public TextMoveNextLineOperator(PdfCanvasProcessor.TextMoveStartNextLineOperator moveStartNextLine) {
                this.moveStartNextLine = moveStartNextLine;
            }

            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                IList<PdfObject> tdoperands = new List<PdfObject>(2);
                tdoperands.Add(0, new PdfNumber(0));
                tdoperands.Add(1, new PdfNumber(-processor.GetGraphicsState().GetLeading()));
                moveStartNextLine.Invoke(processor, null, tdoperands);
            }
        }

        /// <summary>A handler that implements operator (Tm).</summary>
        /// <remarks>A handler that implements operator (Tm). For more information see Table 51 ISO-32000-1</remarks>
        private class TextSetTextMatrixOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                float a = ((PdfNumber)operands[0]).FloatValue();
                float b = ((PdfNumber)operands[1]).FloatValue();
                float c = ((PdfNumber)operands[2]).FloatValue();
                float d = ((PdfNumber)operands[3]).FloatValue();
                float e = ((PdfNumber)operands[4]).FloatValue();
                float f = ((PdfNumber)operands[5]).FloatValue();
                processor.textLineMatrix = new Matrix(a, b, c, d, e, f);
                processor.textMatrix = processor.textLineMatrix;
            }
        }

        /// <summary>A handler that implements operator (TD).</summary>
        /// <remarks>A handler that implements operator (TD). For more information see Table 51 ISO-32000-1</remarks>
        private class TextMoveStartNextLineWithLeadingOperator : IContentOperator {
            private readonly PdfCanvasProcessor.TextMoveStartNextLineOperator moveStartNextLine;

            private readonly PdfCanvasProcessor.SetTextLeadingOperator setTextLeading;

            public TextMoveStartNextLineWithLeadingOperator(PdfCanvasProcessor.TextMoveStartNextLineOperator moveStartNextLine
                , PdfCanvasProcessor.SetTextLeadingOperator setTextLeading) {
                this.moveStartNextLine = moveStartNextLine;
                this.setTextLeading = setTextLeading;
            }

            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                float ty = ((PdfNumber)operands[1]).FloatValue();
                IList<PdfObject> tlOperands = new List<PdfObject>(1);
                tlOperands.Add(0, new PdfNumber(-ty));
                setTextLeading.Invoke(processor, null, tlOperands);
                moveStartNextLine.Invoke(processor, null, operands);
            }
        }

        /// <summary>A handler that implements operator (Td).</summary>
        /// <remarks>A handler that implements operator (Td). For more information see Table 51 ISO-32000-1</remarks>
        private class TextMoveStartNextLineOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                float tx = ((PdfNumber)operands[0]).FloatValue();
                float ty = ((PdfNumber)operands[1]).FloatValue();
                Matrix translationMatrix = new Matrix(tx, ty);
                processor.textMatrix = translationMatrix.Multiply(processor.textLineMatrix);
                processor.textLineMatrix = processor.textMatrix;
            }
        }

        /// <summary>A handler that implements operator (Tf).</summary>
        /// <remarks>A handler that implements operator (Tf). For more information see Table 51 ISO-32000-1</remarks>
        private class SetTextFontOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                PdfName fontResourceName = (PdfName)operands[0];
                float size = ((PdfNumber)operands[1]).FloatValue();
                PdfDictionary fontsDictionary = processor.GetResources().GetResource(PdfName.Font);
                PdfDictionary fontDict = fontsDictionary.GetAsDictionary(fontResourceName);
                PdfFont font = null;
                font = processor.GetFont(fontDict);
                processor.GetGraphicsState().SetFont(font);
                processor.GetGraphicsState().SetFontSize(size);
            }
        }

        /// <summary>A handler that implements operator (Tr).</summary>
        /// <remarks>A handler that implements operator (Tr). For more information see Table 51 ISO-32000-1</remarks>
        private class SetTextRenderModeOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                PdfNumber render = (PdfNumber)operands[0];
                processor.GetGraphicsState().SetTextRenderingMode(render.IntValue());
            }
        }

        /// <summary>A handler that implements operator (Ts).</summary>
        /// <remarks>A handler that implements operator (Ts). For more information see Table 51 ISO-32000-1</remarks>
        private class SetTextRiseOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                PdfNumber rise = (PdfNumber)operands[0];
                processor.GetGraphicsState().SetTextRise(rise.FloatValue());
            }
        }

        /// <summary>A handler that implements operator (TL).</summary>
        /// <remarks>A handler that implements operator (TL). For more information see Table 51 ISO-32000-1</remarks>
        private class SetTextLeadingOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                PdfNumber leading = (PdfNumber)operands[0];
                processor.GetGraphicsState().SetLeading(leading.FloatValue());
            }
        }

        /// <summary>A handler that implements operator (Tz).</summary>
        /// <remarks>A handler that implements operator (Tz). For more information see Table 51 ISO-32000-1</remarks>
        private class SetTextHorizontalScalingOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                PdfNumber scale = (PdfNumber)operands[0];
                processor.GetGraphicsState().SetHorizontalScaling(scale.FloatValue());
            }
        }

        /// <summary>A handler that implements operator (Tc).</summary>
        /// <remarks>A handler that implements operator (Tc). For more information see Table 51 ISO-32000-1</remarks>
        private class SetTextCharacterSpacingOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                PdfNumber charSpace = (PdfNumber)operands[0];
                processor.GetGraphicsState().SetCharSpacing(charSpace.FloatValue());
            }
        }

        /// <summary>A handler that implements operator (Tw).</summary>
        /// <remarks>A handler that implements operator (Tw). For more information see Table 51 ISO-32000-1</remarks>
        private class SetTextWordSpacingOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                PdfNumber wordSpace = (PdfNumber)operands[0];
                processor.GetGraphicsState().SetWordSpacing(wordSpace.FloatValue());
            }
        }

        /// <summary>A handler that implements operator (gs).</summary>
        /// <remarks>A handler that implements operator (gs). For more information see Table 51 ISO-32000-1</remarks>
        private class ProcessGraphicsStateResourceOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                PdfName dictionaryName = (PdfName)operands[0];
                PdfDictionary extGState = processor.GetResources().GetResource(PdfName.ExtGState);
                if (extGState == null) {
                    throw new PdfException(KernelExceptionMessageConstant.RESOURCES_DO_NOT_CONTAIN_EXTGSTATE_ENTRY_UNABLE_TO_PROCESS_THIS_OPERATOR
                        ).SetMessageParams(@operator);
                }
                PdfDictionary gsDic = extGState.GetAsDictionary(dictionaryName);
                if (gsDic == null) {
                    gsDic = extGState.GetAsStream(dictionaryName);
                    if (gsDic == null) {
                        throw new PdfException(KernelExceptionMessageConstant.UNKNOWN_GRAPHICS_STATE_DICTIONARY).SetMessageParams(
                            dictionaryName);
                    }
                }
                PdfArray fontParameter = gsDic.GetAsArray(PdfName.Font);
                if (fontParameter != null) {
                    PdfFont font = processor.GetFont(fontParameter.GetAsDictionary(0));
                    float size = fontParameter.GetAsNumber(1).FloatValue();
                    processor.GetGraphicsState().SetFont(font);
                    processor.GetGraphicsState().SetFontSize(size);
                }
                PdfExtGState pdfExtGState = new PdfExtGState(gsDic.Clone(JavaCollectionsUtil.SingletonList(PdfName.Font)));
                processor.GetGraphicsState().UpdateFromExtGState(pdfExtGState);
            }
        }

        /// <summary>A handler that implements operator (q).</summary>
        /// <remarks>A handler that implements operator (q). For more information see Table 51 ISO-32000-1</remarks>
        private class PushGraphicsStateOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                ParserGraphicsState gs = processor.gsStack.Peek();
                ParserGraphicsState copy = new ParserGraphicsState(gs);
                processor.gsStack.Push(copy);
            }
        }

        /// <summary>A handler that implements operator (cm).</summary>
        /// <remarks>A handler that implements operator (cm). For more information see Table 51 ISO-32000-1</remarks>
        private class ModifyCurrentTransformationMatrixOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                float a = ((PdfNumber)operands[0]).FloatValue();
                float b = ((PdfNumber)operands[1]).FloatValue();
                float c = ((PdfNumber)operands[2]).FloatValue();
                float d = ((PdfNumber)operands[3]).FloatValue();
                float e = ((PdfNumber)operands[4]).FloatValue();
                float f = ((PdfNumber)operands[5]).FloatValue();
                Matrix matrix = new Matrix(a, b, c, d, e, f);
                try {
                    processor.GetGraphicsState().UpdateCtm(matrix);
                }
                catch (PdfException exception) {
                    if (!(exception.InnerException is NoninvertibleTransformException)) {
                        throw;
                    }
                    else {
                        ILogger logger = ITextLogManager.GetLogger(typeof(PdfCanvasProcessor));
                        logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.FAILED_TO_PROCESS_A_TRANSFORMATION_MATRIX
                            ));
                    }
                }
            }
        }

        /// <summary>Gets a color based on a list of operands and Color space.</summary>
        private static Color GetColor(PdfColorSpace pdfColorSpace, IList<PdfObject> operands, PdfResources resources
            ) {
            PdfObject pdfObject;
            if (pdfColorSpace.GetPdfObject().IsIndirectReference()) {
                pdfObject = ((PdfIndirectReference)pdfColorSpace.GetPdfObject()).GetRefersTo();
            }
            else {
                pdfObject = pdfColorSpace.GetPdfObject();
            }
            if (pdfObject.IsName()) {
                if (PdfName.DeviceGray.Equals(pdfObject)) {
                    return new DeviceGray(GetColorants(operands)[0]);
                }
                else {
                    if (PdfName.Pattern.Equals(pdfObject)) {
                        if (operands[0] is PdfName) {
                            PdfPattern pattern = resources.GetPattern((PdfName)operands[0]);
                            if (pattern != null) {
                                return new PatternColor(pattern);
                            }
                        }
                    }
                }
                if (PdfName.DeviceRGB.Equals(pdfObject)) {
                    float[] c = GetColorants(operands);
                    return new DeviceRgb(c[0], c[1], c[2]);
                }
                else {
                    if (PdfName.DeviceCMYK.Equals(pdfObject)) {
                        float[] c = GetColorants(operands);
                        return new DeviceCmyk(c[0], c[1], c[2], c[3]);
                    }
                }
            }
            else {
                if (pdfObject.IsArray()) {
                    PdfArray array = (PdfArray)pdfObject;
                    PdfName csType = array.GetAsName(0);
                    if (PdfName.CalGray.Equals(csType)) {
                        return new CalGray((PdfCieBasedCs.CalGray)pdfColorSpace, GetColorants(operands)[0]);
                    }
                    else {
                        if (PdfName.CalRGB.Equals(csType)) {
                            return new CalRgb((PdfCieBasedCs.CalRgb)pdfColorSpace, GetColorants(operands));
                        }
                        else {
                            if (PdfName.Lab.Equals(csType)) {
                                return new Lab((PdfCieBasedCs.Lab)pdfColorSpace, GetColorants(operands));
                            }
                            else {
                                if (PdfName.ICCBased.Equals(csType)) {
                                    return new IccBased((PdfCieBasedCs.IccBased)pdfColorSpace, GetColorants(operands));
                                }
                                else {
                                    if (PdfName.Indexed.Equals(csType)) {
                                        return new Indexed(pdfColorSpace, (int)GetColorants(operands)[0]);
                                    }
                                    else {
                                        if (PdfName.Separation.Equals(csType)) {
                                            return new Separation((PdfSpecialCs.Separation)pdfColorSpace, GetColorants(operands)[0]);
                                        }
                                        else {
                                            if (PdfName.DeviceN.Equals(csType)) {
                                                return new DeviceN((PdfSpecialCs.DeviceN)pdfColorSpace, GetColorants(operands));
                                            }
                                            else {
                                                if (PdfName.Pattern.Equals(csType)) {
                                                    IList<PdfObject> underlyingOperands = new List<PdfObject>(operands);
                                                    PdfObject patternName = underlyingOperands.JRemoveAt(operands.Count - 2);
                                                    PdfColorSpace underlyingCs = ((PdfSpecialCs.UncoloredTilingPattern)pdfColorSpace).GetUnderlyingColorSpace(
                                                        );
                                                    if (patternName is PdfName) {
                                                        PdfPattern pattern = resources.GetPattern((PdfName)patternName);
                                                        if (pattern is PdfPattern.Tiling && !((PdfPattern.Tiling)pattern).IsColored()) {
                                                            return new PatternColor((PdfPattern.Tiling)pattern, underlyingCs, GetColorants(underlyingOperands));
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            ILogger logger = ITextLogManager.GetLogger(typeof(PdfCanvasProcessor));
            logger.LogWarning(MessageFormatUtil.Format(KernelLogMessageConstant.UNABLE_TO_PARSE_COLOR_WITHIN_COLORSPACE
                , JavaUtil.ArraysToString((Object[])operands.ToArray()), pdfColorSpace.GetPdfObject()));
            return null;
        }

        /// <summary>Gets a color based on a list of operands.</summary>
        private static Color GetColor(int nOperands, IList<PdfObject> operands) {
            float[] c = new float[nOperands];
            for (int i = 0; i < nOperands; i++) {
                c[i] = ((PdfNumber)operands[i]).FloatValue();
            }
            switch (nOperands) {
                case 1: {
                    return new DeviceGray(c[0]);
                }

                case 3: {
                    return new DeviceRgb(c[0], c[1], c[2]);
                }

                case 4: {
                    return new DeviceCmyk(c[0], c[1], c[2], c[3]);
                }
            }
            return null;
        }

        private static float[] GetColorants(IList<PdfObject> operands) {
            float[] c = new float[operands.Count - 1];
            for (int i = 0; i < operands.Count - 1; i++) {
                c[i] = ((PdfNumber)operands[i]).FloatValue();
            }
            return c;
        }

        /// <summary>A handler that implements operator (Q).</summary>
        /// <remarks>A handler that implements operator (Q). For more information see Table 51 ISO-32000-1</remarks>
        protected internal class PopGraphicsStateOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                processor.gsStack.Pop();
                ParserGraphicsState gs = processor.GetGraphicsState();
                processor.EventOccurred(new ClippingPathInfo(gs, gs.GetClippingPath(), gs.GetCtm()), EventType.CLIP_PATH_CHANGED
                    );
            }
        }

        /// <summary>A handler that implements operator (g).</summary>
        /// <remarks>A handler that implements operator (g). For more information see Table 51 ISO-32000-1</remarks>
        private class SetGrayFillOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                processor.GetGraphicsState().SetFillColor(GetColor(1, operands));
            }
        }

        /// <summary>A handler that implements operator (G).</summary>
        /// <remarks>A handler that implements operator (G). For more information see Table 51 ISO-32000-1</remarks>
        private class SetGrayStrokeOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                processor.GetGraphicsState().SetStrokeColor(GetColor(1, operands));
            }
        }

        /// <summary>A handler that implements operator (rg).</summary>
        /// <remarks>A handler that implements operator (rg). For more information see Table 51 ISO-32000-1</remarks>
        private class SetRGBFillOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                processor.GetGraphicsState().SetFillColor(GetColor(3, operands));
            }
        }

        /// <summary>A handler that implements operator (RG).</summary>
        /// <remarks>A handler that implements operator (RG). For more information see Table 51 ISO-32000-1</remarks>
        private class SetRGBStrokeOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                processor.GetGraphicsState().SetStrokeColor(GetColor(3, operands));
            }
        }

        /// <summary>A handler that implements operator (k).</summary>
        /// <remarks>A handler that implements operator (k). For more information see Table 51 ISO-32000-1</remarks>
        private class SetCMYKFillOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                processor.GetGraphicsState().SetFillColor(GetColor(4, operands));
            }
        }

        /// <summary>A handler that implements operator (K).</summary>
        /// <remarks>A handler that implements operator (K). For more information see Table 51 ISO-32000-1</remarks>
        private class SetCMYKStrokeOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                processor.GetGraphicsState().SetStrokeColor(GetColor(4, operands));
            }
        }

        /// <summary>A handler that implements operator (CS).</summary>
        /// <remarks>A handler that implements operator (CS). For more information see Table 51 ISO-32000-1</remarks>
        private class SetColorSpaceFillOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                PdfColorSpace pdfColorSpace = DetermineColorSpace((PdfName)operands[0], processor);
                processor.GetGraphicsState().SetFillColor(Color.MakeColor(pdfColorSpace));
            }

            internal static PdfColorSpace DetermineColorSpace(PdfName colorSpace, PdfCanvasProcessor processor) {
                PdfColorSpace pdfColorSpace;
                if (PdfColorSpace.DIRECT_COLOR_SPACES.Contains(colorSpace)) {
                    pdfColorSpace = PdfColorSpace.MakeColorSpace(colorSpace);
                }
                else {
                    PdfResources pdfResources = processor.GetResources();
                    PdfDictionary resourceColorSpace = pdfResources.GetPdfObject().GetAsDictionary(PdfName.ColorSpace);
                    pdfColorSpace = PdfColorSpace.MakeColorSpace(resourceColorSpace.Get(colorSpace));
                }
                return pdfColorSpace;
            }
        }

        /// <summary>A handler that implements operator (cs).</summary>
        /// <remarks>A handler that implements operator (cs). For more information see Table 51 ISO-32000-1</remarks>
        private class SetColorSpaceStrokeOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                PdfColorSpace pdfColorSpace = PdfCanvasProcessor.SetColorSpaceFillOperator.DetermineColorSpace((PdfName)operands
                    [0], processor);
                processor.GetGraphicsState().SetStrokeColor(Color.MakeColor(pdfColorSpace));
            }
        }

        /// <summary>A handler that implements operator (sc / scn).</summary>
        /// <remarks>A handler that implements operator (sc / scn). For more information see Table 51 ISO-32000-1</remarks>
        private class SetColorFillOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                processor.GetGraphicsState().SetFillColor(GetColor(processor.GetGraphicsState().GetFillColor().GetColorSpace
                    (), operands, processor.GetResources()));
            }
        }

        /// <summary>A handler that implements operator (SC / SCN).</summary>
        /// <remarks>A handler that implements operator (SC / SCN). For more information see Table 51 ISO-32000-1</remarks>
        private class SetColorStrokeOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                processor.GetGraphicsState().SetStrokeColor(GetColor(processor.GetGraphicsState().GetStrokeColor().GetColorSpace
                    (), operands, processor.GetResources()));
            }
        }

        /// <summary>A handler that implements operator (BT).</summary>
        /// <remarks>A handler that implements operator (BT). For more information see Table 51 ISO-32000-1</remarks>
        private class BeginTextOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                processor.textMatrix = new Matrix();
                processor.textLineMatrix = processor.textMatrix;
                processor.BeginText();
            }
        }

        /// <summary>A handler that implements operator (ET).</summary>
        /// <remarks>A handler that implements operator (ET). For more information see Table 51 ISO-32000-1</remarks>
        private class EndTextOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                processor.textMatrix = null;
                processor.textLineMatrix = null;
                processor.EndText();
            }
        }

        /// <summary>A handler that implements operator (BMC).</summary>
        /// <remarks>A handler that implements operator (BMC). For more information see Table 51 ISO-32000-1</remarks>
        private class BeginMarkedContentOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                processor.BeginMarkedContent((PdfName)operands[0], null);
            }
        }

        /// <summary>A handler that implements operator (BDC).</summary>
        /// <remarks>A handler that implements operator (BDC). For more information see Table 51 ISO-32000-1</remarks>
        private class BeginMarkedContentDictionaryOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                PdfObject properties = operands[1];
                processor.BeginMarkedContent((PdfName)operands[0], GetPropertiesDictionary(properties, processor.GetResources
                    ()));
            }

            internal virtual PdfDictionary GetPropertiesDictionary(PdfObject operand1, PdfResources resources) {
                if (operand1.IsDictionary()) {
                    return (PdfDictionary)operand1;
                }
                PdfName dictionaryName = ((PdfName)operand1);
                PdfDictionary properties = resources.GetResource(PdfName.Properties);
                if (null == properties) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(PdfCanvasProcessor));
                    logger.LogWarning(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PDF_REFERS_TO_NOT_EXISTING_PROPERTY_DICTIONARY
                        , PdfName.Properties));
                    return null;
                }
                PdfDictionary propertiesDictionary = properties.GetAsDictionary(dictionaryName);
                if (null == propertiesDictionary) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(PdfCanvasProcessor));
                    logger.LogWarning(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PDF_REFERS_TO_NOT_EXISTING_PROPERTY_DICTIONARY
                        , dictionaryName));
                    return null;
                }
                return properties.GetAsDictionary(dictionaryName);
            }
        }

        /// <summary>A handler that implements operator (EMC).</summary>
        /// <remarks>A handler that implements operator (EMC). For more information see Table 51 ISO-32000-1</remarks>
        private class EndMarkedContentOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                processor.EndMarkedContent();
            }
        }

        /// <summary>A handler that implements operator (Do).</summary>
        /// <remarks>A handler that implements operator (Do). For more information see Table 51 ISO-32000-1</remarks>
        private class DoOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                PdfName resourceName = (PdfName)operands[0];
                processor.DisplayXObject(resourceName);
            }
        }

        /// <summary>A handler that implements operator (EI).</summary>
        /// <remarks>
        /// A handler that implements operator (EI). For more information see Table 51 ISO-32000-1
        /// BI and ID operators are parsed along with this operator.
        /// This not a usual operator, it will have a single operand, which will be a PdfStream object which
        /// encapsulates inline image dictionary and bytes
        /// </remarks>
        private class EndImageOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                PdfStream imageStream = (PdfStream)operands[0];
                processor.DisplayImage(processor.markedContentStack, imageStream, null, true);
            }
        }

        /// <summary>A handler that implements operator (w).</summary>
        /// <remarks>A handler that implements operator (w). For more information see Table 51 ISO-32000-1</remarks>
        private class SetLineWidthOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral oper, IList<PdfObject> operands) {
                float lineWidth = ((PdfNumber)operands[0]).FloatValue();
                processor.GetGraphicsState().SetLineWidth(lineWidth);
            }
        }

        /// <summary>A handler that implements operator (J).</summary>
        /// <remarks>A handler that implements operator (J). For more information see Table 51 ISO-32000-1</remarks>
        private class SetLineCapOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral oper, IList<PdfObject> operands) {
                int lineCap = ((PdfNumber)operands[0]).IntValue();
                processor.GetGraphicsState().SetLineCapStyle(lineCap);
            }
        }

        /// <summary>A handler that implements operator (j).</summary>
        /// <remarks>A handler that implements operator (j). For more information see Table 51 ISO-32000-1</remarks>
        private class SetLineJoinOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral oper, IList<PdfObject> operands) {
                int lineJoin = ((PdfNumber)operands[0]).IntValue();
                processor.GetGraphicsState().SetLineJoinStyle(lineJoin);
            }
        }

        /// <summary>A handler that implements operator (M).</summary>
        /// <remarks>A handler that implements operator (M). For more information see Table 51 ISO-32000-1</remarks>
        private class SetMiterLimitOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral oper, IList<PdfObject> operands) {
                float miterLimit = ((PdfNumber)operands[0]).FloatValue();
                processor.GetGraphicsState().SetMiterLimit(miterLimit);
            }
        }

        /// <summary>A handler that implements operator (d).</summary>
        /// <remarks>A handler that implements operator (d). For more information see Table 51 ISO-32000-1</remarks>
        private class SetLineDashPatternOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral oper, IList<PdfObject> operands) {
                processor.GetGraphicsState().SetDashPattern(new PdfArray(JavaUtil.ArraysAsList(operands[0], operands[1])));
            }
        }

        /// <summary>An XObject subtype handler for FORM</summary>
        private class FormXObjectDoHandler : IXObjectDoHandler {
            public virtual void HandleXObject(PdfCanvasProcessor processor, Stack<CanvasTag> canvasTagHierarchy, PdfStream
                 xObjectStream, PdfName xObjectName) {
                PdfDictionary resourcesDic = xObjectStream.GetAsDictionary(PdfName.Resources);
                PdfResources resources;
                if (resourcesDic == null) {
                    resources = processor.GetResources();
                }
                else {
                    resources = new PdfResources(resourcesDic);
                }
                // we read the content bytes up here so if it fails we don't leave the graphics state stack corrupted
                // this is probably not necessary (if we fail on this, probably the entire content stream processing
                // operation should be rejected
                byte[] contentBytes;
                contentBytes = xObjectStream.GetBytes();
                PdfArray matrix = xObjectStream.GetAsArray(PdfName.Matrix);
                new PdfCanvasProcessor.PushGraphicsStateOperator().Invoke(processor, null, null);
                if (matrix != null) {
                    float a = matrix.GetAsNumber(0).FloatValue();
                    float b = matrix.GetAsNumber(1).FloatValue();
                    float c = matrix.GetAsNumber(2).FloatValue();
                    float d = matrix.GetAsNumber(3).FloatValue();
                    float e = matrix.GetAsNumber(4).FloatValue();
                    float f = matrix.GetAsNumber(5).FloatValue();
                    Matrix formMatrix = new Matrix(a, b, c, d, e, f);
                    processor.GetGraphicsState().UpdateCtm(formMatrix);
                }
                processor.ProcessContent(contentBytes, resources);
                new PdfCanvasProcessor.PopGraphicsStateOperator().Invoke(processor, null, null);
            }
        }

        /// <summary>An XObject subtype handler for IMAGE</summary>
        private class ImageXObjectDoHandler : IXObjectDoHandler {
            public virtual void HandleXObject(PdfCanvasProcessor processor, Stack<CanvasTag> canvasTagHierarchy, PdfStream
                 xObjectStream, PdfName resourceName) {
                processor.DisplayImage(canvasTagHierarchy, xObjectStream, resourceName, false);
            }
        }

        /// <summary>An XObject subtype handler that does nothing</summary>
        private class IgnoreXObjectDoHandler : IXObjectDoHandler {
            public virtual void HandleXObject(PdfCanvasProcessor processor, Stack<CanvasTag> canvasTagHierarchy, PdfStream
                 xObjectStream, PdfName xObjectName) {
            }
            // ignore XObject subtype
        }

        /// <summary>A handler that implements operator (m).</summary>
        /// <remarks>A handler that implements operator (m). For more information see Table 51 ISO-32000-1</remarks>
        private class MoveToOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                float x = ((PdfNumber)operands[0]).FloatValue();
                float y = ((PdfNumber)operands[1]).FloatValue();
                processor.currentPath.MoveTo(x, y);
            }
        }

        /// <summary>A handler that implements operator (l).</summary>
        /// <remarks>A handler that implements operator (l). For more information see Table 51 ISO-32000-1</remarks>
        private class LineToOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                float x = ((PdfNumber)operands[0]).FloatValue();
                float y = ((PdfNumber)operands[1]).FloatValue();
                processor.currentPath.LineTo(x, y);
            }
        }

        /// <summary>A handler that implements operator (c).</summary>
        /// <remarks>A handler that implements operator (c). For more information see Table 51 ISO-32000-1</remarks>
        private class CurveOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                float x1 = ((PdfNumber)operands[0]).FloatValue();
                float y1 = ((PdfNumber)operands[1]).FloatValue();
                float x2 = ((PdfNumber)operands[2]).FloatValue();
                float y2 = ((PdfNumber)operands[3]).FloatValue();
                float x3 = ((PdfNumber)operands[4]).FloatValue();
                float y3 = ((PdfNumber)operands[5]).FloatValue();
                processor.currentPath.CurveTo(x1, y1, x2, y2, x3, y3);
            }
        }

        /// <summary>A handler that implements operator (v).</summary>
        /// <remarks>A handler that implements operator (v). For more information see Table 51 ISO-32000-1</remarks>
        private class CurveFirstPointDuplicatedOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                float x2 = ((PdfNumber)operands[0]).FloatValue();
                float y2 = ((PdfNumber)operands[1]).FloatValue();
                float x3 = ((PdfNumber)operands[2]).FloatValue();
                float y3 = ((PdfNumber)operands[3]).FloatValue();
                processor.currentPath.CurveTo(x2, y2, x3, y3);
            }
        }

        /// <summary>A handler that implements operator (y).</summary>
        /// <remarks>A handler that implements operator (y). For more information see Table 51 ISO-32000-1</remarks>
        private class CurveFourhPointDuplicatedOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                float x1 = ((PdfNumber)operands[0]).FloatValue();
                float y1 = ((PdfNumber)operands[1]).FloatValue();
                float x3 = ((PdfNumber)operands[2]).FloatValue();
                float y3 = ((PdfNumber)operands[3]).FloatValue();
                processor.currentPath.CurveFromTo(x1, y1, x3, y3);
            }
        }

        /// <summary>A handler that implements operator (h).</summary>
        /// <remarks>A handler that implements operator (h). For more information see Table 51 ISO-32000-1</remarks>
        private class CloseSubpathOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                processor.currentPath.CloseSubpath();
            }
        }

        /// <summary>A handler that implements operator (re).</summary>
        /// <remarks>A handler that implements operator (re). For more information see Table 51 ISO-32000-1</remarks>
        private class RectangleOperator : IContentOperator {
            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                float x = ((PdfNumber)operands[0]).FloatValue();
                float y = ((PdfNumber)operands[1]).FloatValue();
                float w = ((PdfNumber)operands[2]).FloatValue();
                float h = ((PdfNumber)operands[3]).FloatValue();
                processor.currentPath.Rectangle(x, y, w, h);
            }
        }

        /// <summary>A handler that implements operator (S, s, f, F, f*, B, B*, b, b*).</summary>
        /// <remarks>A handler that implements operator (S, s, f, F, f*, B, B*, b, b*). For more information see Table 51 ISO-32000-1
        ///     </remarks>
        private class PaintPathOperator : IContentOperator {
            private int operation;

            private int rule;

            private bool close;

            /// <summary>Constructs PainPath object.</summary>
            /// <param name="operation">
            /// One of the possible combinations of
            /// <see cref="iText.Kernel.Pdf.Canvas.Parser.Data.PathRenderInfo.STROKE"/>
            /// and
            /// <see cref="iText.Kernel.Pdf.Canvas.Parser.Data.PathRenderInfo.FILL"/>
            /// values or
            /// <see cref="iText.Kernel.Pdf.Canvas.Parser.Data.PathRenderInfo.NO_OP"/>
            /// </param>
            /// <param name="rule">
            /// Either
            /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.FillingRule.NONZERO_WINDING"/>
            /// or
            /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.FillingRule.EVEN_ODD"/>
            /// In case it isn't applicable pass any value.
            /// </param>
            /// <param name="close">Indicates whether the path should be closed or not.</param>
            public PaintPathOperator(int operation, int rule, bool close) {
                this.operation = operation;
                this.rule = rule;
                this.close = close;
            }

            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                if (close) {
                    processor.currentPath.CloseSubpath();
                }
                processor.PaintPath(operation, rule);
            }
        }

        /// <summary>A handler that implements operator (W, W*).</summary>
        /// <remarks>A handler that implements operator (W, W*). For more information see Table 51 ISO-32000-1</remarks>
        private class ClipPathOperator : IContentOperator {
            private int rule;

            public ClipPathOperator(int rule) {
                this.rule = rule;
            }

            /// <summary><inheritDoc/></summary>
            public virtual void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands) {
                processor.isClip = true;
                processor.clippingRule = rule;
            }
        }
    }
}
