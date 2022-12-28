/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Font;
using iText.StyledXmlParser.Resolver.Font;
using iText.StyledXmlParser.Resolver.Resource;
using iText.Svg.Css;
using iText.Svg.Exceptions;

namespace iText.Svg.Renderers {
    /// <summary>
    /// The SvgDrawContext keeps a stack of
    /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
    /// instances, which
    /// represent all levels of XObjects that are added to the root canvas.
    /// </summary>
    public class SvgDrawContext {
        private readonly IDictionary<String, ISvgNodeRenderer> namedObjects = new Dictionary<String, ISvgNodeRenderer
            >();

        private readonly LinkedList<PdfCanvas> canvases = new LinkedList<PdfCanvas>();

        private readonly LinkedList<Rectangle> viewports = new LinkedList<Rectangle>();

        private readonly Stack<String> useIds = new Stack<String>();

        private readonly Stack<String> patternIds = new Stack<String>();

        private readonly ResourceResolver resourceResolver;

        private readonly FontProvider fontProvider;

        private FontSet tempFonts;

        private SvgCssContext cssContext;

        private AffineTransform lastTextTransform = new AffineTransform();

        private float[] textMove = new float[] { 0.0f, 0.0f };

        private float[] previousElementTextMove;

        /// <summary>Create an instance of the context that is used to store information when converting SVG.</summary>
        /// <param name="resourceResolver">
        /// instance of
        /// <see cref="iText.StyledXmlParser.Resolver.Resource.ResourceResolver"/>
        /// </param>
        /// <param name="fontProvider">
        /// instance of
        /// <see cref="iText.Layout.Font.FontProvider"/>
        /// </param>
        public SvgDrawContext(ResourceResolver resourceResolver, FontProvider fontProvider) {
            if (resourceResolver == null) {
                resourceResolver = new ResourceResolver(null);
            }
            this.resourceResolver = resourceResolver;
            if (fontProvider == null) {
                fontProvider = new BasicFontProvider();
            }
            this.fontProvider = fontProvider;
            cssContext = new SvgCssContext();
        }

        /// <summary>Retrieves the current top of the stack, without modifying the stack.</summary>
        /// <returns>the current canvas that can be used for drawing operations.</returns>
        public virtual PdfCanvas GetCurrentCanvas() {
            return canvases.JGetFirst();
        }

        /// <summary>
        /// Retrieves the current top of the stack, thereby taking the current item
        /// off the stack.
        /// </summary>
        /// <returns>the current canvas that can be used for drawing operations.</returns>
        public virtual PdfCanvas PopCanvas() {
            PdfCanvas canvas = canvases.JGetFirst();
            canvases.RemoveFirst();
            return canvas;
        }

        /// <summary>
        /// Adds a
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// to the stack (by definition its top), for use in
        /// drawing operations.
        /// </summary>
        /// <param name="canvas">the new top of the stack</param>
        public virtual void PushCanvas(PdfCanvas canvas) {
            canvases.AddFirst(canvas);
        }

        /// <summary>
        /// Get the current size of the stack, signifying the nesting level of the
        /// XObjects.
        /// </summary>
        /// <returns>the current size of the stack.</returns>
        public virtual int Size() {
            return canvases.Count;
        }

        /// <summary>Adds a viewbox to the context.</summary>
        /// <param name="viewPort">rectangle representing the current viewbox</param>
        public virtual void AddViewPort(Rectangle viewPort) {
            viewports.AddFirst(viewPort);
        }

        /// <summary>Get the current viewbox.</summary>
        /// <returns>the viewbox as it is currently set</returns>
        public virtual Rectangle GetCurrentViewPort() {
            return viewports.JGetFirst();
        }

        /// <summary>Get the viewbox which is the root viewport for the current document.</summary>
        /// <returns>root viewbox.</returns>
        public virtual Rectangle GetRootViewPort() {
            return viewports.JGetLast();
        }

        /// <summary>Remove the currently set view box.</summary>
        public virtual void RemoveCurrentViewPort() {
            if (this.viewports.Count > 0) {
                viewports.RemoveFirst();
            }
        }

        /// <summary>Adds a named object to the draw context.</summary>
        /// <remarks>Adds a named object to the draw context. These objects can then be referenced from a different tag.
        ///     </remarks>
        /// <param name="name">name of the object</param>
        /// <param name="namedObject">object to be referenced</param>
        public virtual void AddNamedObject(String name, ISvgNodeRenderer namedObject) {
            if (namedObject == null) {
                throw new SvgProcessingException(SvgExceptionMessageConstant.NAMED_OBJECT_NULL);
            }
            if (name == null || String.IsNullOrEmpty(name)) {
                throw new SvgProcessingException(SvgExceptionMessageConstant.NAMED_OBJECT_NAME_NULL_OR_EMPTY);
            }
            if (!this.namedObjects.ContainsKey(name)) {
                this.namedObjects.Put(name, namedObject);
            }
        }

        /// <summary>Get a named object based on its name.</summary>
        /// <remarks>Get a named object based on its name. If the name isn't listed, this method will return null.</remarks>
        /// <param name="name">name of the object you want to reference</param>
        /// <returns>the referenced object</returns>
        public virtual ISvgNodeRenderer GetNamedObject(String name) {
            return this.namedObjects.Get(name);
        }

        /// <summary>Gets the ResourceResolver to be used during the drawing operations.</summary>
        /// <returns>resource resolver instance</returns>
        public virtual ResourceResolver GetResourceResolver() {
            return resourceResolver;
        }

        /// <summary>* Adds a number of named object to the draw context.</summary>
        /// <remarks>* Adds a number of named object to the draw context. These objects can then be referenced from a different tag.
        ///     </remarks>
        /// <param name="namedObjects">Map containing the named objects keyed to their ID strings</param>
        public virtual void AddNamedObjects(IDictionary<String, ISvgNodeRenderer> namedObjects) {
            this.namedObjects.AddAll(namedObjects);
        }

        /// <summary>Gets the FontProvider to be used during the drawing operations.</summary>
        /// <returns>font provider instance</returns>
        public virtual FontProvider GetFontProvider() {
            return fontProvider;
        }

        /// <summary>Gets list of temporary fonts from @font-face.</summary>
        /// <returns>font set instance</returns>
        public virtual FontSet GetTempFonts() {
            return tempFonts;
        }

        /// <summary>Sets the FontSet.</summary>
        /// <param name="tempFonts">font set to be used during drawing operations</param>
        public virtual void SetTempFonts(FontSet tempFonts) {
            this.tempFonts = tempFonts;
        }

        /// <summary>Returns true when this id has been used before</summary>
        /// <param name="elementId">element id to check</param>
        /// <returns>true if id has been encountered before through a use element</returns>
        public virtual bool IsIdUsedByUseTagBefore(String elementId) {
            return this.useIds.Contains(elementId);
        }

        /// <summary>Adds an ID that has been referenced by a use element.</summary>
        /// <param name="elementId">referenced element ID</param>
        public virtual void AddUsedId(String elementId) {
            this.useIds.Push(elementId);
        }

        /// <summary>Removes an ID that has been referenced by a use element.</summary>
        /// <param name="elementId">referenced element ID</param>
        public virtual void RemoveUsedId(String elementId) {
            this.useIds.Pop();
        }

        /// <summary>Get the text transformation that was last applied</summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Geom.AffineTransform"/>
        /// representing the last text transformation
        /// </returns>
        public virtual AffineTransform GetLastTextTransform() {
            if (lastTextTransform == null) {
                lastTextTransform = new AffineTransform();
            }
            return this.lastTextTransform;
        }

        /// <summary>Set the last text transformation</summary>
        /// <param name="newTransform">last text transformation</param>
        public virtual void SetLastTextTransform(AffineTransform newTransform) {
            this.lastTextTransform = newTransform;
        }

        /// <summary>Get the stored current text move</summary>
        /// <returns>[horizontal text move, vertical text move]</returns>
        public virtual float[] GetTextMove() {
            return textMove;
        }

        /// <summary>Reset the stored text move to [0f,0f]</summary>
        public virtual void ResetTextMove() {
            textMove = new float[] { 0.0f, 0.0f };
        }

        /// <summary>Increment the stored text move</summary>
        /// <param name="additionalMoveX">horizontal value to add</param>
        /// <param name="additionalMoveY">vertical value to add</param>
        public virtual void AddTextMove(float additionalMoveX, float additionalMoveY) {
            textMove[0] += additionalMoveX;
            textMove[1] += additionalMoveY;
        }

        /// <summary>Get the current canvas transformation</summary>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Geom.AffineTransform"/>
        /// representing the current canvas transformation
        /// </returns>
        public virtual AffineTransform GetCurrentCanvasTransform() {
            Matrix currentTransform = GetCurrentCanvas().GetGraphicsState().GetCtm();
            if (currentTransform != null) {
                return new AffineTransform(currentTransform.Get(0), currentTransform.Get(1), currentTransform.Get(3), currentTransform
                    .Get(4), currentTransform.Get(6), currentTransform.Get(7));
            }
            return new AffineTransform();
        }

        /// <summary>Gets the SVG CSS context.</summary>
        /// <returns>the SVG CSS context</returns>
        public virtual SvgCssContext GetCssContext() {
            return cssContext;
        }

        /// <summary>Sets the SVG CSS context.</summary>
        /// <param name="cssContext">the SVG CSS context</param>
        public virtual void SetCssContext(SvgCssContext cssContext) {
            this.cssContext = cssContext;
        }

        /// <summary>Add pattern id to stack.</summary>
        /// <remarks>
        /// Add pattern id to stack. Check if the id is already in the stack.
        /// If it is, then return
        /// <see langword="false"/>
        /// and not add, if it is not - add and return
        /// <see langword="true"/>.
        /// </remarks>
        /// <param name="patternId">pattern id</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if pattern id was not on the stack and was pushed;
        /// <see langword="false"/>
        /// if it is on the stack
        /// </returns>
        public virtual bool PushPatternId(String patternId) {
            if (this.patternIds.Contains(patternId)) {
                return false;
            }
            else {
                this.patternIds.Push(patternId);
                return true;
            }
        }

        /// <summary>Pops the last template id from the stack.</summary>
        public virtual void PopPatternId() {
            this.patternIds.Pop();
        }

        public virtual void SetPreviousElementTextMove(float[] previousElementTextMove) {
            this.previousElementTextMove = previousElementTextMove;
        }

        public virtual float[] GetPreviousElementTextMove() {
            return previousElementTextMove;
        }
    }
}
