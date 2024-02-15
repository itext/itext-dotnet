using iText.Layout.Element;

namespace iText.Pdfua.Checkers.Utils.Tables {
    /// <summary>Class that provides methods for checking PDF/UA compliance of table elements.</summary>
    public sealed class TableCheckUtil {
        /// <summary>
        /// Creates a new
        /// <see cref="TableCheckUtil"/>
        /// instance.
        /// </summary>
        private TableCheckUtil() {
        }

        // Empty constructor
        /// <summary>Checks if the table is pdf/ua compliant.</summary>
        /// <param name="table">the table to check.</param>
        public static void CheckLayoutTable(Table table) {
            new CellResultMatrix(table.GetNumberOfColumns(), table).CheckValidTableTagging();
        }
    }
}
