using iText.Kernel.Pdf;

namespace iText.Kernel.Mac {
    /// <summary>
    /// Strategy interface, which is responsible for
    /// <see cref="AbstractMacIntegrityProtector"/>
    /// container location.
    /// </summary>
    /// <remarks>
    /// Strategy interface, which is responsible for
    /// <see cref="AbstractMacIntegrityProtector"/>
    /// container location.
    /// Expected to be used in
    /// <see cref="iText.Commons.Utils.DIContainer"/>.
    /// </remarks>
    public interface IMacContainerLocator {
        /// <summary>
        /// Locates
        /// <see cref="AbstractMacIntegrityProtector"/>
        /// container.
        /// </summary>
        /// <param name="macIntegrityProtector">
        /// 
        /// <see cref="AbstractMacIntegrityProtector"/>
        /// container to be located
        /// </param>
        void LocateMacContainer(AbstractMacIntegrityProtector macIntegrityProtector);

        /// <summary>
        /// Creates
        /// <see cref="AbstractMacIntegrityProtector"/>
        /// from explicitly provided MAC properties.
        /// </summary>
        /// <param name="document">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// for which MAC container shall be created
        /// </param>
        /// <param name="macProperties">
        /// 
        /// <see cref="MacProperties"/>
        /// to be used for MAC container creation
        /// </param>
        /// <returns>
        /// 
        /// <see cref="AbstractMacIntegrityProtector"/>
        /// which specific implementation depends on interface implementation.
        /// </returns>
        AbstractMacIntegrityProtector CreateMacIntegrityProtector(PdfDocument document, MacProperties macProperties
            );

        /// <summary>
        /// Creates
        /// <see cref="AbstractMacIntegrityProtector"/>
        /// from already existing AuthCode dictionary.
        /// </summary>
        /// <param name="document">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// for which MAC container shall be created
        /// </param>
        /// <param name="authDictionary">
        /// AuthCode
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// which contains MAC related information
        /// </param>
        /// <returns>
        /// 
        /// <see cref="AbstractMacIntegrityProtector"/>
        /// which specific implementation depends on interface implementation.
        /// </returns>
        AbstractMacIntegrityProtector CreateMacIntegrityProtector(PdfDocument document, PdfDictionary authDictionary
            );
    }
}
