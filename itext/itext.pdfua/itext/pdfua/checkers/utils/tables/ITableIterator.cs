using iText.Kernel.Pdf;

namespace iText.Pdfua.Checkers.Utils.Tables {
    /// <summary>Interface that provides methods for iterating over the elements of a table.</summary>
    internal interface ITableIterator<T> {
        /// <summary>Checks if there is a next element in the iteration.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if there is a next element,
        /// <see langword="false"/>
        /// otherwise.
        /// </returns>
        bool HasNext();

        /// <summary>Gets the next element in the iteration.</summary>
        /// <returns>The next element.</returns>
        T Next();

        /// <summary>Gets the number of rows in the body of the table.</summary>
        /// <returns>The number of rows in the body of the table.</returns>
        int GetAmountOfRowsBody();

        /// <summary>Gets the number of rows in the header of the table.</summary>
        /// <returns>The number of rows in the header of the table.</returns>
        int GetAmountOfRowsHeader();

        /// <summary>Gets the number of rows in the footer of the table.</summary>
        /// <returns>The number of rows in the footer of the table.</returns>
        int GetAmountOfRowsFooter();

        /// <summary>Gets the location of the current element in the table.</summary>
        /// <returns>The location of the current element in the table.</returns>
        PdfName GetLocation();
    }
}
