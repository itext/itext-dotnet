using System;
using iText.Commons.Utils;
using iText.Layout.Element;

namespace iText.Layout.Properties.Margins {
    /// <summary>
    /// Class to store information about page margin content represented by
    /// <see cref="iText.Layout.Element.IElement"/>
    /// linked to
    /// <see cref="MarginBoxName"/>.
    /// </summary>
    public class PageMarginContent : AbstractPageContent {
        private readonly MarginBoxName marginBoxName;

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
        public PageMarginContent(MarginBoxName marginBoxName, IElement marginContent)
            : base(marginContent) {
            this.marginBoxName = marginBoxName;
        }

        /// <summary>
        /// Creates new
        /// <see cref="PageMarginContent"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Creates new
        /// <see cref="PageMarginContent"/>
        /// instance.
        /// <para />
        /// The margin will have the specified size in points.
        /// </remarks>
        /// <param name="marginBoxName">
        /// 
        /// <see cref="MarginBoxName"/>
        /// specifying margin name based on its location on the page
        /// </param>
        /// <param name="marginInPoints"><c>float</c> specifying the margin in points</param>
        public PageMarginContent(MarginBoxName marginBoxName, float marginInPoints)
            : this(marginBoxName, GetStaticMarginContent(marginBoxName, marginInPoints)) {
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
        public PageMarginContent(iText.Layout.Properties.Margins.PageMarginContent other)
            : base(other) {
            this.marginBoxName = other.marginBoxName;
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

        /// <summary>
        /// Creates
        /// <see cref="iText.Layout.Element.Div"/>
        /// layout element of the fixed size to represent a static margin.
        /// </summary>
        /// <param name="marginBoxName">
        /// 
        /// <see cref="MarginBoxName"/>
        /// specifying margin name based on its location on the page
        /// </param>
        /// <param name="marginInPoints">
        /// 
        /// <c>float</c>
        /// specifying the margin in points
        /// </param>
        /// <returns>
        /// 
        /// <see cref="iText.Layout.Element.Div"/>
        /// layout element with static size
        /// </returns>
        private static Div GetStaticMarginContent(MarginBoxName marginBoxName, float marginInPoints) {
            Div staticMarginContent = new Div();
            if (marginBoxName == MarginBoxName.TOP || marginBoxName == MarginBoxName.BOTTOM) {
                staticMarginContent.SetHeight(marginInPoints);
            }
            else {
                if (marginBoxName == MarginBoxName.LEFT || marginBoxName == MarginBoxName.RIGHT) {
                    staticMarginContent.SetWidth(marginInPoints);
                }
            }
            return staticMarginContent;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Layout.Properties.Margins.PageMarginContent that = (iText.Layout.Properties.Margins.PageMarginContent
                )o;
            return Object.Equals(marginBoxName, that.marginBoxName) && Object.Equals(GetContent(), that.GetContent());
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode((Object)marginBoxName, GetContent());
        }
    }
}
