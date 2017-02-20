using iText.Layout;
using iText.Layout.Element;

namespace iText.Layout.Properties {
    /// <summary>Interface for implementing custom symbols for lists</summary>
    public interface IListSymbolFactory {
        /// <summary>Creates symbol.</summary>
        /// <param name="index">- the positive (greater then zero) index of list item in list.</param>
        /// <param name="list">
        /// - the
        /// <see cref="iText.Layout.IPropertyContainer"/>
        /// with all properties of corresponding list.
        /// </param>
        /// <param name="listItem">
        /// - the
        /// <see cref="iText.Layout.IPropertyContainer"/>
        /// with all properties of corresponding list item.
        /// </param>
        /// <returns>
        /// the
        /// <see cref="iText.Layout.Element.IElement"/>
        /// representing symbol.
        /// </returns>
        IElement CreateSymbol(int index, IPropertyContainer list, IPropertyContainer listItem);
    }
}
