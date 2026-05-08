using System;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Layout.Element;

namespace iText.Layout.Properties.Margins {
    /// <summary>
    /// Class to store information about page margin content represented by
    /// <see cref="iText.Layout.Element.IElement"/>
    /// linked to
    /// <see cref="MarginBoxName"/>.
    /// </summary>
    public class PageMarginContent {
        private readonly MarginBoxName marginBoxName;

        private readonly IElement marginContent;

        private Rectangle pageMarginBoxRectangle;

        /// <summary>
        /// Creates new
        /// <see cref="PageMarginContent"/>
        /// instance.
        /// </summary>
        /// <param name="marginBoxName">
        /// 
        /// <see cref="MarginBoxName"/>
        /// specifying margin name based on its location on the page
        /// </param>
        /// <param name="marginContent">
        /// 
        /// <see cref="iText.Layout.Element.IElement"/>
        /// layout element with margin content
        /// </param>
        public PageMarginContent(MarginBoxName marginBoxName, IElement marginContent) {
            this.marginBoxName = marginBoxName;
            this.marginContent = marginContent;
        }

        /// <summary>
        /// Creates new
        /// <see cref="PageMarginContent"/>
        /// instance by copying existing one.
        /// </summary>
        /// <param name="other">
        /// 
        /// <see cref="PageMarginContent"/>
        /// instance to copy
        /// </param>
        public PageMarginContent(iText.Layout.Properties.Margins.PageMarginContent other) {
            this.marginBoxName = other.marginBoxName;
            this.marginContent = other.marginContent;
            this.pageMarginBoxRectangle = other.pageMarginBoxRectangle;
        }

        /// <summary>
        /// Gets the page margin box name
        /// <see cref="MarginBoxName"/>
        /// which is based on its location on the page.
        /// </summary>
        /// <returns>the margin box name</returns>
        public virtual MarginBoxName GetMarginBoxName() {
            return marginBoxName;
        }

        /// <summary>Returns renderer for layout element representing page margin content.</summary>
        /// <returns>
        /// 
        /// <see cref="iText.Layout.Element.IElement"/>
        /// layout element for page margin content
        /// </returns>
        public virtual IElement GetMarginContent() {
            return marginContent;
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Sets the rectangle in which page margin box contents are shown.</summary>
        /// <param name="pageMarginBoxRectangle">
        /// 
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// defining position and dimensions of the margin box content area
        /// </param>
        internal virtual void SetPageMarginBoxRectangle(Rectangle pageMarginBoxRectangle) {
            this.pageMarginBoxRectangle = pageMarginBoxRectangle;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Gets the rectangle in which page margin box contents should be shown.</summary>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// defining position and dimensions of the margin box content area
        /// </returns>
        internal virtual Rectangle GetPageMarginBoxRectangle() {
            return pageMarginBoxRectangle;
        }
//\endcond

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Layout.Properties.Margins.PageMarginContent that = (iText.Layout.Properties.Margins.PageMarginContent
                )o;
            return Object.Equals(marginBoxName, that.marginBoxName) && Object.Equals(marginContent, that.marginContent
                );
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode((Object)marginBoxName, marginContent);
        }
    }
}
