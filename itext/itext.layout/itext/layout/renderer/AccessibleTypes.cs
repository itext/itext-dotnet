using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;

namespace iText.Layout.Renderer {
    /// <summary>
    /// This class is used to identify standard structure role type based only on it's name for the sake of applying
    /// standard structure attributes.
    /// </summary>
    /// <remarks>
    /// This class is used to identify standard structure role type based only on it's name for the sake of applying
    /// standard structure attributes.
    /// <p>
    /// These types mostly resemble structure type levels in the pdf 1.7 specification, however they are not exact.
    /// In pdf 2.0 some of these types are not even present and moreover, specific roles with the same name might belong
    /// to different type levels depending on context (which consists of kids, parents and their types).
    /// </p>
    /// <p>
    /// So, these types are mostly useful for the internal itext usage and are not backed by any spec. They are designed for
    /// the most part to return the value the most suitable and handy for the purposes of accessibility properties applying.
    /// </p>
    /// <p>
    /// Here are the main reasons to leave these types as is for now, even after introducing of PDF 2.0:
    /// <ul>
    /// <li>Standard structure types for pdf 1.7 and 2.0 are very alike. There are some differences, like new/removed roles
    /// and attributes, however they are not used in current layout auto tagging mechanism.
    /// </li>
    /// <li>Differentiating  possible types for the same role based on the context is not supported at the moment.</li>
    /// </ul>
    /// In general, the correct way to handle role types would be to have separate classes for every namespace that define type
    /// and apply attributes. However I believe, that for now it is not feasible at the moment to implement this approach.
    /// </p>
    /// The right time to improve and replace this class might be when new roles and attributes (specific to the different standard structure namespaces)
    /// will be more widely used in the auto tagging mechanism by default, and also when may be there will be more known
    /// practical examples of utilizing standard structure attributes.
    /// </remarks>
    internal class AccessibleTypes {
        internal static int Unknown = 0;

        internal static int Grouping = 1;

        internal static int BlockLevel = 2;

        internal static int InlineLevel = 3;

        internal static int Illustration = 4;

        internal static ICollection<PdfName> groupingRoles = new HashSet<PdfName>();

        internal static ICollection<PdfName> blockLevelRoles = new HashSet<PdfName>();

        internal static ICollection<PdfName> inlineLevelRoles = new HashSet<PdfName>();

        internal static ICollection<PdfName> illustrationRoles = new HashSet<PdfName>();

        static AccessibleTypes() {
            // Some tag roles are not in any of the sets that define types. Some - because we don't want to write any accessibility
            // properties for them, some - because they are ambiguous for different pdf versions or don't have any possible
            // properties to set at the moment.
            //        PdfName.Document
            //        PdfName.DocumentFragment
            //        PdfName.Artifact
            //        PdfName.THead
            //        PdfName.TBody
            //        PdfName.TFoot
            groupingRoles.Add(PdfName.Part);
            groupingRoles.Add(PdfName.Art);
            groupingRoles.Add(PdfName.Sect);
            groupingRoles.Add(PdfName.Div);
            groupingRoles.Add(PdfName.BlockQuote);
            groupingRoles.Add(PdfName.Caption);
            groupingRoles.Add(PdfName.TOC);
            groupingRoles.Add(PdfName.TOCI);
            groupingRoles.Add(PdfName.Index);
            groupingRoles.Add(PdfName.NonStruct);
            groupingRoles.Add(PdfName.Private);
            groupingRoles.Add(PdfName.Aside);
            blockLevelRoles.Add(PdfName.P);
            blockLevelRoles.Add(PdfName.H);
            blockLevelRoles.Add(PdfName.H1);
            blockLevelRoles.Add(PdfName.H2);
            blockLevelRoles.Add(PdfName.H3);
            blockLevelRoles.Add(PdfName.H4);
            blockLevelRoles.Add(PdfName.H5);
            blockLevelRoles.Add(PdfName.H6);
            // Hn type is handled separately in identifyType method
            blockLevelRoles.Add(PdfName.L);
            blockLevelRoles.Add(PdfName.Lbl);
            blockLevelRoles.Add(PdfName.LI);
            blockLevelRoles.Add(PdfName.LBody);
            blockLevelRoles.Add(PdfName.Table);
            blockLevelRoles.Add(PdfName.TR);
            blockLevelRoles.Add(PdfName.TH);
            blockLevelRoles.Add(PdfName.TD);
            blockLevelRoles.Add(PdfName.Title);
            blockLevelRoles.Add(PdfName.FENote);
            blockLevelRoles.Add(PdfName.Sub);
            blockLevelRoles.Add(PdfName.Caption);
            inlineLevelRoles.Add(PdfName.Span);
            inlineLevelRoles.Add(PdfName.Quote);
            inlineLevelRoles.Add(PdfName.Note);
            inlineLevelRoles.Add(PdfName.Reference);
            inlineLevelRoles.Add(PdfName.BibEntry);
            inlineLevelRoles.Add(PdfName.Code);
            inlineLevelRoles.Add(PdfName.Link);
            inlineLevelRoles.Add(PdfName.Annot);
            inlineLevelRoles.Add(PdfName.Ruby);
            inlineLevelRoles.Add(PdfName.Warichu);
            inlineLevelRoles.Add(PdfName.RB);
            inlineLevelRoles.Add(PdfName.RT);
            inlineLevelRoles.Add(PdfName.RP);
            inlineLevelRoles.Add(PdfName.WT);
            inlineLevelRoles.Add(PdfName.WP);
            inlineLevelRoles.Add(PdfName.Em);
            inlineLevelRoles.Add(PdfName.Strong);
            illustrationRoles.Add(PdfName.Figure);
            illustrationRoles.Add(PdfName.Formula);
            illustrationRoles.Add(PdfName.Form);
        }

        /// <summary>Identifies standard structure role type based only on it's name.</summary>
        /// <remarks>
        /// Identifies standard structure role type based only on it's name. The return types might be one of the constants:
        /// <ul>
        /// <li>
        /// <see cref="Unknown"/>
        /// </li>
        /// <li>
        /// <see cref="Grouping"/>
        /// </li>
        /// <li>
        /// <see cref="BlockLevel"/>
        /// </li>
        /// <li>
        /// <see cref="InlineLevel"/>
        /// </li>
        /// <li>
        /// <see cref="Illustration"/>
        /// </li>
        /// </ul>
        /// See also remarks in the
        /// <see cref="AccessibleTypes"/>
        /// class documentation.
        /// </remarks>
        internal static int IdentifyType(PdfName role) {
            if (groupingRoles.Contains(role)) {
                return Grouping;
            }
            else {
                if (blockLevelRoles.Contains(role) || StandardStructureNamespace.IsHnRole(role)) {
                    return BlockLevel;
                }
                else {
                    if (inlineLevelRoles.Contains(role)) {
                        return InlineLevel;
                    }
                    else {
                        if (illustrationRoles.Contains(role)) {
                            return Illustration;
                        }
                    }
                }
            }
            return Unknown;
        }
    }
}
