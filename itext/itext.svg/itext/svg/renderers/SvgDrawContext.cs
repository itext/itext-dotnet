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
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Font;
using iText.StyledXmlParser.Resolver.Font;
using iText.StyledXmlParser.Resolver.Resource;
using iText.Svg.Css;
using iText.Svg.Exceptions;
using iText.Svg.Utils;

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

        private SvgTextProperties textProperties = new SvgTextProperties();

        private FontSet tempFonts;

        private SvgCssContext cssContext;

        private AffineTransform rootTransform;

        private float[] textMove = new float[] { 0.0f, 0.0f };

        private float[] relativePosition;

        private Rectangle customViewport;

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

        /// <summary>Gets the custom viewport of SVG.</summary>
        /// <remarks>
        /// Gets the custom viewport of SVG.
        /// <para />
        /// The custom viewport is used to resolve percent values of the top level svg.
        /// </remarks>
        /// <returns>the custom viewport</returns>
        public virtual Rectangle GetCustomViewport() {
            return customViewport;
        }

        /// <summary>Sets the custom viewport of SVG.</summary>
        /// <remarks>
        /// Sets the custom viewport of SVG.
        /// <para />
        /// The custom viewport is used to resolve percent values of the top level svg.
        /// </remarks>
        /// <param name="customViewport">the custom viewport</param>
        public virtual void SetCustomViewport(Rectangle customViewport) {
            this.customViewport = customViewport;
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
            if (viewports.IsEmpty()) {
                return null;
            }
            return viewports.JGetFirst();
        }

        /// <summary>Get the viewbox which is the root viewport for the current document.</summary>
        /// <returns>root viewbox.</returns>
        public virtual Rectangle GetRootViewPort() {
            return viewports.JGetLast();
        }

        /// <summary>Remove the currently set view box.</summary>
        public virtual void RemoveCurrentViewPort() {
            if (!this.viewports.IsEmpty()) {
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

        /// <summary>Get the text transformation that was last applied.</summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Geom.AffineTransform"/>
        /// representing the last text transformation
        /// </returns>
        [System.ObsoleteAttribute(@"in favour of GetRootTransform()")]
        public virtual AffineTransform GetLastTextTransform() {
            return new AffineTransform();
        }

        /// <summary>Set the last text transformation.</summary>
        /// <param name="newTransform">last text transformation</param>
        [System.ObsoleteAttribute(@"in favour of SetRootTransform(iText.Kernel.Geom.AffineTransform)")]
        public virtual void SetLastTextTransform(AffineTransform newTransform) {
        }

        // Do nothing.
        /// <summary>Get the current root transformation that was last applied.</summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Geom.AffineTransform"/>
        /// representing the root transformation.
        /// </returns>
        public virtual AffineTransform GetRootTransform() {
            if (rootTransform == null) {
                rootTransform = new AffineTransform();
            }
            return this.rootTransform;
        }

        /// <summary>Set the current root transformation.</summary>
        /// <param name="newTransform">root transformation.</param>
        public virtual void SetRootTransform(AffineTransform newTransform) {
            this.rootTransform = newTransform;
        }

        /// <summary>Get the stored current text move.</summary>
        /// <returns>[horizontal text move, vertical text move]</returns>
        public virtual float[] GetTextMove() {
            return textMove;
        }

        /// <summary>Reset the stored text move to [0f,0f]</summary>
        public virtual void ResetTextMove() {
            textMove = new float[] { 0.0f, 0.0f };
        }

        /// <summary>Increment the stored text move.</summary>
        /// <param name="additionalMoveX">horizontal value to add</param>
        /// <param name="additionalMoveY">vertical value to add</param>
        public virtual void AddTextMove(float additionalMoveX, float additionalMoveY) {
            textMove[0] += additionalMoveX;
            textMove[1] += additionalMoveY;
        }

        /// <summary>Get the current canvas transformation.</summary>
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

        [Obsolete]
        public virtual void SetPreviousElementTextMove(float[] previousElementTextMove) {
        }

        // Do nothing.
        [Obsolete]
        public virtual float[] GetPreviousElementTextMove() {
            return new float[] { 0.0f, 0.0f };
        }

        /// <summary>
        /// Retrieves
        /// <see cref="iText.Svg.Utils.SvgTextProperties"/>
        /// for text SVG elements.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Svg.Utils.SvgTextProperties"/>
        /// text properties
        /// </returns>
        public virtual SvgTextProperties GetSvgTextProperties() {
            return textProperties;
        }

        /// <summary>
        /// Sets
        /// <see cref="iText.Svg.Utils.SvgTextProperties"/>
        /// for textSVG elements.
        /// </summary>
        /// <param name="textProperties">
        /// 
        /// <see cref="iText.Svg.Utils.SvgTextProperties"/>
        /// to set
        /// </param>
        public virtual void SetSvgTextProperties(SvgTextProperties textProperties) {
            this.textProperties = textProperties;
        }

        /// <summary>
        /// Retrieves relative position for the current text SVG element relative to the last origin
        /// identified by absolute position.
        /// </summary>
        /// <returns>relative position for the current text SVG element</returns>
        public virtual float[] GetRelativePosition() {
            return relativePosition;
        }

        /// <summary>Adds move to the current relative position for the text SVG element.</summary>
        /// <param name="dx">x-axis movement</param>
        /// <param name="dy">y-axis movement</param>
        public virtual void MoveRelativePosition(float dx, float dy) {
            relativePosition[0] += dx;
            relativePosition[1] += dy;
        }

        /// <summary>Resets current relative position for the text SVG element.</summary>
        public virtual void ResetRelativePosition() {
            relativePosition = new float[] { 0.0f, 0.0f };
        }
    }
}
