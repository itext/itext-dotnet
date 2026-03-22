using System;
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Kernel.XMP;
using iText.Kernel.XMP.Options;
using iText.Kernel.XMP.Properties;

namespace iText.Kernel.Pdf {
//\cond DO_NOT_DOCUMENT
    /// <summary>Utility class for extracting and setting PDF conformance information in XMP metadata.</summary>
    /// <remarks>
    /// Utility class for extracting and setting PDF conformance information in XMP metadata.
    /// <para />This class handles the mapping between
    /// <see cref="PdfConformance"/>
    /// instances and
    /// their XMP metadata representations for PDF/A, PDF/UA, and Well Tagged PDF (WTPDF)
    /// conformance levels.
    /// </remarks>
    internal sealed class PdfConformanceXmpMetaDataUtil {
        private PdfConformanceXmpMetaDataUtil() {
        }

        // Utility class, no need to create an instance.
        /// <summary>XMP property path for the first conformsTo declaration inside the declarations bag.</summary>
        private const String FIRST_CONFORMS_TO_PATH = XMPConst.DECLARATIONS + "/[1]/" + XMPConst.CONFORMS_TO;

        private const String WELL_TAGGED_FOR_ACCESSIBILITY_SCHEMA = " <x:xmpmeta xmlns:x=\"adobe:ns:meta/\">\n" + 
            "  <rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\">\n" + "   <rdf:Description rdf:about=\"\" xmlns:pdfd=\"http://pdfa.org/declarations/\">\n"
             + "    <pdfd:declarations>\n" + "     <rdf:Bag>\n" + "      <rdf:li rdf:parseType=\"Resource\">\n" + 
            "       <pdfd:conformsTo>http://pdfa.org/declarations/wtpdf#accessibility1.0</pdfd:conformsTo>\n" + "      </rdf:li>\n"
             + "     </rdf:Bag>\n" + "    </pdfd:declarations>\n" + "   </rdf:Description>\n" + "  </rdf:RDF>\n" +
             " </x:xmpmeta>";

        private const String WELL_TAGGED_FOR_REUSE_SCHEMA = " <x:xmpmeta xmlns:x=\"adobe:ns:meta/\">\n" + "  <rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\">\n"
             + "   <rdf:Description rdf:about=\"\" xmlns:pdfd=\"http://pdfa.org/declarations/\">\n" + "    <pdfd:declarations>\n"
             + "     <rdf:Bag>\n" + "      <rdf:li rdf:parseType=\"Resource\">\n" + "       <pdfd:conformsTo>http://pdfa.org/declarations/wtpdf#reuse1.0</pdfd:conformsTo>\n"
             + "      </rdf:li>\n" + "     </rdf:Bag>\n" + "    </pdfd:declarations>\n" + "   </rdf:Description>\n"
             + "  </rdf:RDF>\n" + " </x:xmpmeta>";

        private const String PDF_UA_EXTENSION = "    <x:xmpmeta xmlns:x=\"adobe:ns:meta/\">\n" + "      <rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\">\n"
             + "        <rdf:Description rdf:about=\"\" xmlns:pdfaExtension=\"http://www.aiim" + ".org/pdfa/ns/extension/\" xmlns:pdfaSchema=\"http://www.aiim.org/pdfa/ns/schema#\" "
             + "xmlns:pdfaProperty=\"http://www.aiim.org/pdfa/ns/property#\">\n" + "          <pdfaExtension:schemas>\n"
             + "            <rdf:Bag>\n" + "              <rdf:li rdf:parseType=\"Resource\">\n" + "                <pdfaSchema:namespaceURI rdf:resource=\"http://www.aiim.org/pdfua/ns/id/\"/>\n"
             + "                <pdfaSchema:prefix>pdfuaid</pdfaSchema:prefix>\n" + "                <pdfaSchema:schema>PDF/UA identification schema</pdfaSchema:schema>\n"
             + "                <pdfaSchema:property>\n" + "                  <rdf:Seq>\n" + "                    <rdf:li rdf:parseType=\"Resource\">\n"
             + "                      <pdfaProperty:category>internal</pdfaProperty:category>\n" + "                      <pdfaProperty:description>PDF/UA version "
             + "identifier</pdfaProperty:description>\n" + "                      <pdfaProperty:name>part</pdfaProperty:name>\n"
             + "                      <pdfaProperty:valueType>Integer</pdfaProperty:valueType>\n" + "                    </rdf:li>\n"
             + "                    <rdf:li rdf:parseType=\"Resource\">\n" + "                      <pdfaProperty:category>internal</pdfaProperty:category>\n"
             + "                      <pdfaProperty:description>PDF/UA amendment " + "identifier</pdfaProperty:description>\n"
             + "                      <pdfaProperty:name>amd</pdfaProperty:name>\n" + "                      <pdfaProperty:valueType>Text</pdfaProperty:valueType>\n"
             + "                    </rdf:li>\n" + "                    <rdf:li rdf:parseType=\"Resource\">\n" + "                      <pdfaProperty:category>internal</pdfaProperty:category>\n"
             + "                      <pdfaProperty:description>PDF/UA corrigenda " + "identifier</pdfaProperty:description>\n"
             + "                      <pdfaProperty:name>corr</pdfaProperty:name>\n" + "                      <pdfaProperty:valueType>Text</pdfaProperty:valueType>\n"
             + "                    </rdf:li>\n" + "                  </rdf:Seq>\n" + "                </pdfaSchema:property>\n"
             + "              </rdf:li>\n" + "            </rdf:Bag>\n" + "          </pdfaExtension:schemas>\n" +
             "        </rdf:Description>\n" + "      </rdf:RDF>\n" + "    </x:xmpmeta>";

//\cond DO_NOT_DOCUMENT
        /// <summary>Sets the required XMP metadata properties for the given PDF conformance.</summary>
        /// <remarks>
        /// Sets the required XMP metadata properties for the given PDF conformance.
        /// <para />Existing property values are preserved; only missing properties are populated.
        /// This ensures that if something was invalid in the source document, it is left as-is.
        /// However, if a required property is absent (e.g. revision for PDF/A-4), it will be added.
        /// </remarks>
        /// <param name="conformance">the conformance whose properties should be written</param>
        /// <param name="xmpMeta">the XMP metadata instance to update</param>
        internal static void SetConformanceToXmp(PdfConformance conformance, XMPMeta xmpMeta) {
            if (conformance.IsPdfUA()) {
                PdfUAConformance uaConformance = conformance.GetUAConformance();
                if (xmpMeta.GetProperty(XMPConst.NS_PDFUA_ID, XMPConst.PART) == null) {
                    xmpMeta.SetPropertyInteger(XMPConst.NS_PDFUA_ID, XMPConst.PART, Convert.ToInt32(uaConformance.GetPart(), System.Globalization.CultureInfo.InvariantCulture
                        ), new PropertyOptions(PropertyOptions.SEPARATE_NODE));
                }
                if (conformance.ConformsTo(PdfUAConformance.PDF_UA_2) && xmpMeta.GetProperty(XMPConst.NS_PDFUA_ID, XMPConst
                    .REV) == null) {
                    xmpMeta.SetPropertyInteger(XMPConst.NS_PDFUA_ID, XMPConst.REV, 2024);
                }
            }
            bool missingConformsTo = xmpMeta.GetProperty(XMPConst.NS_DECLARATIONS, FIRST_CONFORMS_TO_PATH) == null;
            if (missingConformsTo) {
                if (conformance.ConformsTo(WellTaggedPdfConformance.FOR_ACCESSIBILITY) || conformance.ConformsTo(PdfUAConformance
                    .PDF_UA_2)) {
                    XMPMeta wtpdfMeta = XMPMetaFactory.ParseFromString(WELL_TAGGED_FOR_ACCESSIBILITY_SCHEMA);
                    XMPUtils.AppendProperties(wtpdfMeta, xmpMeta, true, false, true);
                }
                if (conformance.ConformsTo(WellTaggedPdfConformance.FOR_REUSE)) {
                    XMPMeta wtpdfMeta = XMPMetaFactory.ParseFromString(WELL_TAGGED_FOR_REUSE_SCHEMA);
                    XMPUtils.AppendProperties(wtpdfMeta, xmpMeta, true, false, true);
                }
            }
            if (conformance.IsPdfA()) {
                PdfAConformance aConformance = conformance.GetAConformance();
                if (xmpMeta.GetProperty(XMPConst.NS_PDFA_ID, XMPConst.PART) == null) {
                    xmpMeta.SetProperty(XMPConst.NS_PDFA_ID, XMPConst.PART, aConformance.GetPart());
                }
                if (aConformance.GetLevel() != null && xmpMeta.GetProperty(XMPConst.NS_PDFA_ID, XMPConst.CONFORMANCE) == null
                    ) {
                    xmpMeta.SetProperty(XMPConst.NS_PDFA_ID, XMPConst.CONFORMANCE, aConformance.GetLevel());
                }
                if ("4".Equals(aConformance.GetPart()) && xmpMeta.GetProperty(XMPConst.NS_PDFA_ID, XMPConst.REV) == null) {
                    xmpMeta.SetProperty(XMPConst.NS_PDFA_ID, XMPConst.REV, PdfConformance.PDF_A_4_REVISION);
                }
                if (xmpMeta.GetPropertyInteger(XMPConst.NS_PDFUA_ID, XMPConst.PART) != null) {
                    XMPMeta taggedExtensionMeta = XMPMetaFactory.ParseFromString(PDF_UA_EXTENSION);
                    XMPUtils.AppendProperties(taggedExtensionMeta, xmpMeta, true, false);
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Extracts all Well Tagged PDF conformance levels from the XMP metadata declarations bag.</summary>
        /// <remarks>
        /// Extracts all Well Tagged PDF conformance levels from the XMP metadata declarations bag.
        /// <para />The declarations bag may contain multiple entries (e.g. both accessibility and reuse),
        /// so this method iterates over all items in the bag.
        /// </remarks>
        /// <param name="meta">the XMP metadata to inspect</param>
        /// <returns>
        /// a list of
        /// <see cref="WellTaggedPdfConformance"/>
        /// values found; never
        /// <see langword="null"/>
        /// , may be empty
        /// </returns>
        internal static IList<WellTaggedPdfConformance> GetWtpdfConformanceFromXmp(XMPMeta meta) {
            IList<WellTaggedPdfConformance> wtpdfConformanceList = new List<WellTaggedPdfConformance>();
            try {
                int itemCount = meta.CountArrayItems(XMPConst.NS_DECLARATIONS, XMPConst.DECLARATIONS);
                for (int i = 1; i <= itemCount; i++) {
                    String path = XMPConst.DECLARATIONS + "/[" + i + "]/" + XMPConst.CONFORMS_TO;
                    XMPProperty wtpdfProperty = meta.GetProperty(XMPConst.NS_DECLARATIONS, path);
                    if (wtpdfProperty == null) {
                        continue;
                    }
                    if (XMPConst.NS_WTPDF_ACCESSIBILITY_ID.Equals(wtpdfProperty.GetValue())) {
                        wtpdfConformanceList.Add(WellTaggedPdfConformance.FOR_ACCESSIBILITY);
                    }
                    else {
                        if (XMPConst.NS_WTPDF_REUSE_ID.Equals(wtpdfProperty.GetValue())) {
                            wtpdfConformanceList.Add(WellTaggedPdfConformance.FOR_REUSE);
                        }
                    }
                }
            }
            catch (XMPException) {
            }
            // If the declarations property is absent or malformed, return an empty list.
            return wtpdfConformanceList;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Extracts the PDF/A conformance level from the XMP metadata.</summary>
        /// <param name="meta">the XMP metadata to inspect</param>
        /// <returns>
        /// the
        /// <see cref="PdfAConformance"/>
        /// found, or
        /// <see langword="null"/>
        /// if none is present
        /// </returns>
        internal static PdfAConformance GetAConformance(XMPMeta meta) {
            XMPProperty conformanceAXmpProperty = null;
            XMPProperty partAXmpProperty = null;
            PdfAConformance aLevel = null;
            try {
                conformanceAXmpProperty = meta.GetProperty(XMPConst.NS_PDFA_ID, XMPConst.CONFORMANCE);
                partAXmpProperty = meta.GetProperty(XMPConst.NS_PDFA_ID, XMPConst.PART);
            }
            catch (XMPException) {
            }
            if (partAXmpProperty != null && (conformanceAXmpProperty != null || "4".Equals(partAXmpProperty.GetValue()
                ))) {
                aLevel = GetAConformance(partAXmpProperty.GetValue(), conformanceAXmpProperty == null ? null : conformanceAXmpProperty
                    .GetValue());
            }
            return aLevel;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Extracts the PDF/UA conformance level from the XMP metadata.</summary>
        /// <param name="meta">the XMP metadata to inspect</param>
        /// <returns>
        /// the
        /// <see cref="PdfUAConformance"/>
        /// found, or
        /// <see langword="null"/>
        /// if none is present
        /// </returns>
        internal static PdfUAConformance GetUAConformanceFromXmp(XMPMeta meta) {
            XMPProperty partUAXmpProperty = null;
            PdfUAConformance uaLevel = null;
            try {
                partUAXmpProperty = meta.GetProperty(XMPConst.NS_PDFUA_ID, XMPConst.PART);
            }
            catch (XMPException) {
            }
            if (partUAXmpProperty != null) {
                uaLevel = GetUAConformance(partUAXmpProperty.GetValue());
            }
            return uaLevel;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Maps a PDF/A part and level string to the corresponding
        /// <see cref="PdfAConformance"/>
        /// enum constant.
        /// </summary>
        /// <param name="part">
        /// the PDF/A part (e.g.
        /// <c>"1"</c>
        /// ,
        /// <c>"2"</c>
        /// ,
        /// <c>"3"</c>
        /// , or
        /// <c>"4"</c>
        /// )
        /// </param>
        /// <param name="level">
        /// the PDF/A conformance level (e.g.
        /// <c>"A"</c>
        /// ,
        /// <c>"B"</c>
        /// ,
        /// <c>"U"</c>
        /// ,
        /// <c>"E"</c>
        /// , or
        /// <c>"F"</c>
        /// ); may be
        /// <see langword="null"/>
        /// for part 4
        /// </param>
        /// <returns>
        /// the matching
        /// <see cref="PdfAConformance"/>
        /// , or
        /// <see langword="null"/>
        /// if the combination is not recognised
        /// </returns>
        internal static PdfAConformance GetAConformance(String part, String level) {
            String upperLevel = StringNormalizer.ToUpperCase(level);
            bool aLevel = "A".Equals(upperLevel);
            bool bLevel = "B".Equals(upperLevel);
            bool uLevel = "U".Equals(upperLevel);
            bool eLevel = "E".Equals(upperLevel);
            bool fLevel = "F".Equals(upperLevel);
            switch (part) {
                case "1": {
                    if (aLevel) {
                        return PdfAConformance.PDF_A_1A;
                    }
                    if (bLevel) {
                        return PdfAConformance.PDF_A_1B;
                    }
                    break;
                }

                case "2": {
                    if (aLevel) {
                        return PdfAConformance.PDF_A_2A;
                    }
                    if (bLevel) {
                        return PdfAConformance.PDF_A_2B;
                    }
                    if (uLevel) {
                        return PdfAConformance.PDF_A_2U;
                    }
                    break;
                }

                case "3": {
                    if (aLevel) {
                        return PdfAConformance.PDF_A_3A;
                    }
                    if (bLevel) {
                        return PdfAConformance.PDF_A_3B;
                    }
                    if (uLevel) {
                        return PdfAConformance.PDF_A_3U;
                    }
                    break;
                }

                case "4": {
                    if (eLevel) {
                        return PdfAConformance.PDF_A_4E;
                    }
                    if (fLevel) {
                        return PdfAConformance.PDF_A_4F;
                    }
                    return PdfAConformance.PDF_A_4;
                }
            }
            return null;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Maps a PDF/UA part string to the corresponding
        /// <see cref="PdfUAConformance"/>
        /// enum constant.
        /// </summary>
        /// <param name="part">
        /// the PDF/UA part (e.g.
        /// <c>"1"</c>
        /// or
        /// <c>"2"</c>
        /// )
        /// </param>
        /// <returns>
        /// the matching
        /// <see cref="PdfUAConformance"/>
        /// , or
        /// <see langword="null"/>
        /// if the part is not recognised
        /// </returns>
        internal static PdfUAConformance GetUAConformance(String part) {
            if ("1".Equals(part)) {
                return PdfUAConformance.PDF_UA_1;
            }
            if ("2".Equals(part)) {
                return PdfUAConformance.PDF_UA_2;
            }
            return null;
        }
//\endcond
    }
//\endcond
}
