/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;

namespace iText.Kernel.Utils {
    /// <summary>Utility class which provides functionality to merge ECMA scripts from pdf documents</summary>
    public class PdfScriptMerger {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(PdfScriptMerger));

        private static readonly ICollection<PdfName> allowedAAEntries = JavaCollectionsUtil.UnmodifiableSet(new HashSet
            <PdfName>(JavaUtil.ArraysAsList(PdfName.WC, PdfName.WS, PdfName.DS, PdfName.WP)));

        /// <summary>
        /// Merges ECMA scripts from source to destinations from all possible places for them,
        /// it only copies first action in chain for AA and OpenAction entries
        /// </summary>
        /// <param name="source">source document from which script will be copied</param>
        /// <param name="destination">destination document to which script will be copied</param>
        public static void MergeScripts(PdfDocument source, PdfDocument destination) {
            MergeOpenActionsScripts(source, destination);
            MergeAdditionalActionsScripts(source, destination);
            MergeNamesScripts(source, destination);
        }

        /// <summary>Copies AA catalog entry ECMA scripts, it only copies first action in chain</summary>
        /// <param name="source">source document from which script will be copied</param>
        /// <param name="destination">destination document to which script will be copied</param>
        public static void MergeAdditionalActionsScripts(PdfDocument source, PdfDocument destination) {
            PdfDictionary sourceAA = source.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.AA);
            PdfDictionary destinationAA = destination.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.AA);
            if (sourceAA == null || sourceAA.IsEmpty()) {
                return;
            }
            if (destinationAA == null) {
                destinationAA = new PdfDictionary();
                destination.GetCatalog().GetPdfObject().Put(PdfName.AA, destinationAA);
            }
            foreach (KeyValuePair<PdfName, PdfObject> entry in sourceAA.EntrySet()) {
                if (destinationAA.ContainsKey(entry.Key)) {
                    LOGGER.LogError(MessageFormatUtil.Format(KernelLogMessageConstant.CANNOT_MERGE_ENTRY, entry.Key));
                    return;
                }
                if (!allowedAAEntries.Contains(entry.Key)) {
                    continue;
                }
                destinationAA.Put(entry.Key, CopyECMAScriptActionsDictionary(destination, (PdfDictionary)entry.Value));
            }
        }

        /// <summary>Copies open actions catalog entry ECMA scripts, it only copies first action in chain</summary>
        /// <param name="source">source document from which script will be copied</param>
        /// <param name="destination">destination document to which script will be copied</param>
        public static void MergeOpenActionsScripts(PdfDocument source, PdfDocument destination) {
            PdfObject sourceOpenAction = source.GetCatalog().GetPdfObject().Get(PdfName.OpenAction);
            if (sourceOpenAction is PdfArray) {
                return;
            }
            PdfDictionary sourceOpenActionDict = source.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.OpenAction
                );
            if (sourceOpenActionDict == null || sourceOpenActionDict.IsEmpty() || !PdfName.JavaScript.Equals(sourceOpenActionDict
                .Get(PdfName.S))) {
                return;
            }
            PdfObject destinationOpenAction = destination.GetCatalog().GetPdfObject().Get(PdfName.OpenAction);
            if (destinationOpenAction != null) {
                LOGGER.LogError(MessageFormatUtil.Format(KernelLogMessageConstant.CANNOT_MERGE_ENTRY, PdfName.OpenAction));
                return;
            }
            destination.GetCatalog().GetPdfObject().Put(PdfName.OpenAction, CopyECMAScriptActionsDictionary(destination
                , sourceOpenActionDict));
        }

        /// <summary>Copies ECMA scripts from Names catalog entry</summary>
        /// <param name="source">source document from which script will be copied</param>
        /// <param name="destination">destination document to which script will be copied</param>
        public static void MergeNamesScripts(PdfDocument source, PdfDocument destination) {
            PdfDictionary namesDict = source.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.Names);
            if (namesDict == null || !namesDict.ContainsKey(PdfName.JavaScript)) {
                return;
            }
            PdfDictionary destinationNamesDict = destination.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.Names
                );
            if ((destinationNamesDict != null && destinationNamesDict.Get(PdfName.JavaScript) != null) || destination.
                GetCatalog().NameTreeContainsKey(PdfName.JavaScript)) {
                LOGGER.LogError(MessageFormatUtil.Format(KernelLogMessageConstant.CANNOT_MERGE_ENTRY, PdfName.JavaScript));
                return;
            }
            PdfNameTree sourceTree = new PdfNameTree(source.GetCatalog(), PdfName.JavaScript);
            PdfNameTree destinationTree = destination.GetCatalog().GetNameTree(PdfName.JavaScript);
            foreach (KeyValuePair<PdfString, PdfObject> entry in sourceTree.GetNames()) {
                PdfDictionary ECMAScriptActionsDirectCopy = CopyECMAScriptActionsDictionary(destination, entry.Value.IsIndirect
                    () ? (PdfDictionary)entry.Value.GetIndirectReference().GetRefersTo() : (PdfDictionary)entry.Value);
                destinationTree.AddEntry(entry.Key, ECMAScriptActionsDirectCopy);
            }
        }

        private static PdfDictionary CopyECMAScriptActionsDictionary(PdfDocument destination, PdfDictionary actions
            ) {
            PdfObject originalScriptSource = actions.Get(PdfName.JS);
            PdfObject scriptType = actions.Get(PdfName.S);
            PdfDictionary actionsCopy = new PdfDictionary();
            if (originalScriptSource != null) {
                actionsCopy.Put(PdfName.JS, originalScriptSource.CopyTo(destination));
            }
            if (scriptType != null) {
                actionsCopy.Put(PdfName.S, scriptType.CopyTo(destination));
            }
            return actionsCopy;
        }
    }
}
