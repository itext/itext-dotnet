using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;

namespace iText.Layout.Renderer {
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
            // properties for them, some - because they are ambiguous for different pdf versions and don't have any possible
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

        internal static int IdentifyType(PdfDocument doc, PdfName role, PdfNamespace @namespace) {
            IRoleMappingResolver mappingResolver = doc.GetTagStructureContext().ResolveMappingToStandardOrDomainSpecificRole
                (role, @namespace);
            if (mappingResolver != null) {
                role = mappingResolver.GetRole();
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
            }
            return Unknown;
        }
    }
}
