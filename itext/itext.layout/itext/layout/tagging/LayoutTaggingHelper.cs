/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Layout.Tagging {
    /// <summary>
    /// The class is a helper which is used to correctly create structure
    /// tree for layout element (with keeping right order for tags).
    /// </summary>
    public class LayoutTaggingHelper {
        private TagStructureContext context;

        private PdfDocument document;

        private bool immediateFlush;

        // kidsHints and parentHints fields represent tree of TaggingHintKey, where parentHints
        // stores a parent for the key, and kidsHints stores kids for key.
        private IDictionary<TaggingHintKey, IList<TaggingHintKey>> kidsHints;

        private IDictionary<TaggingHintKey, TaggingHintKey> parentHints;

        private IDictionary<IRenderer, TagTreePointer> autoTaggingPointerSavedPosition;

        private IDictionary<String, IList<ITaggingRule>> taggingRules;

        // dummiesForPreExistingTags is used to process TaggingDummyElement
        private IDictionary<PdfObject, TaggingDummyElement> dummiesForPreExistingTags;

        private readonly int RETVAL_NO_PARENT = -1;

        private readonly int RETVAL_PARENT_AND_KID_FINISHED = -2;

        public LayoutTaggingHelper(PdfDocument document, bool immediateFlush) {
            this.document = document;
            this.context = document.GetTagStructureContext();
            this.immediateFlush = immediateFlush;
            this.kidsHints = new LinkedDictionary<TaggingHintKey, IList<TaggingHintKey>>();
            this.parentHints = new LinkedDictionary<TaggingHintKey, TaggingHintKey>();
            this.autoTaggingPointerSavedPosition = new Dictionary<IRenderer, TagTreePointer>();
            this.taggingRules = new Dictionary<String, IList<ITaggingRule>>();
            RegisterRules(context.GetTagStructureTargetVersion());
            dummiesForPreExistingTags = new LinkedDictionary<PdfObject, TaggingDummyElement>();
        }

        public static void AddTreeHints(iText.Layout.Tagging.LayoutTaggingHelper taggingHelper, IRenderer rootRenderer
            ) {
            IList<IRenderer> childRenderers = rootRenderer.GetChildRenderers();
            if (childRenderers == null) {
                return;
            }
            taggingHelper.AddKidsHint(rootRenderer, childRenderers);
            foreach (IRenderer childRenderer in childRenderers) {
                AddTreeHints(taggingHelper, childRenderer);
            }
        }

        public static TaggingHintKey GetHintKey(IPropertyContainer container) {
            return container.GetProperty<TaggingHintKey>(Property.TAGGING_HINT_KEY);
        }

        public static TaggingHintKey GetOrCreateHintKey(IPropertyContainer container) {
            return GetOrCreateHintKey(container, true);
        }

        public virtual void AddKidsHint<_T0>(TagTreePointer parentPointer, IEnumerable<_T0> newKids)
            where _T0 : IPropertyContainer {
            PdfDictionary pointerStructElem = context.GetPointerStructElem(parentPointer).GetPdfObject();
            TaggingDummyElement dummy = dummiesForPreExistingTags.Get(pointerStructElem);
            if (dummy == null) {
                dummy = new TaggingDummyElement(parentPointer.GetRole());
                dummiesForPreExistingTags.Put(pointerStructElem, dummy);
            }
            context.GetWaitingTagsManager().AssignWaitingState(parentPointer, GetOrCreateHintKey(dummy));
            AddKidsHint(dummy, newKids);
        }

        public virtual void AddKidsHint<_T0>(IPropertyContainer parent, IEnumerable<_T0> newKids)
            where _T0 : IPropertyContainer {
            AddKidsHint(parent, newKids, -1);
        }

        public virtual void AddKidsHint<_T0>(IPropertyContainer parent, IEnumerable<_T0> newKids, int insertIndex)
            where _T0 : IPropertyContainer {
            if (parent is AreaBreakRenderer) {
                return;
            }
            TaggingHintKey parentKey = GetOrCreateHintKey(parent);
            IList<TaggingHintKey> newKidsKeys = new List<TaggingHintKey>();
            foreach (IPropertyContainer kid in newKids) {
                if (kid is AreaBreakRenderer) {
                    return;
                }
                newKidsKeys.Add(GetOrCreateHintKey(kid));
            }
            AddKidsHint(parentKey, newKidsKeys, insertIndex);
        }

        public virtual void AddKidsHint(TaggingHintKey parentKey, ICollection<TaggingHintKey> newKidsKeys) {
            AddKidsHint(parentKey, newKidsKeys, -1);
        }

        public virtual void AddKidsHint(TaggingHintKey parentKey, ICollection<TaggingHintKey> newKidsKeys, int insertIndex
            ) {
            AddKidsHint(parentKey, newKidsKeys, insertIndex, false);
        }

        public virtual void SetRoleHint(IPropertyContainer hintOwner, String role) {
            // It's unclear whether a role of already created tag should be changed
            // in this case. Also concerning rules, they won't be called for the new role
            // if this overriding role is set after some rule applying event. Already applied
            // rules won't be cancelled either.
            // Restricting this call on whether the finished state is set doesn't really
            // solve anything.
            // Probably this also should affect whether the hint is considered non-accessible
            GetOrCreateHintKey(hintOwner).SetOverriddenRole(role);
        }

        public virtual bool IsArtifact(IPropertyContainer hintOwner) {
            TaggingHintKey key = GetHintKey(hintOwner);
            if (key != null) {
                return key.IsArtifact();
            }
            else {
                IAccessibleElement aElem = null;
                if (hintOwner is IRenderer && ((IRenderer)hintOwner).GetModelElement() is IAccessibleElement) {
                    aElem = (IAccessibleElement)((IRenderer)hintOwner).GetModelElement();
                }
                else {
                    if (hintOwner is IAccessibleElement) {
                        aElem = (IAccessibleElement)hintOwner;
                    }
                }
                if (aElem != null) {
                    return StandardRoles.ARTIFACT.Equals(aElem.GetAccessibilityProperties().GetRole());
                }
            }
            return false;
        }

        public virtual void MarkArtifactHint(IPropertyContainer hintOwner) {
            TaggingHintKey hintKey = GetOrCreateHintKey(hintOwner);
            MarkArtifactHint(hintKey);
        }

        public virtual void MarkArtifactHint(TaggingHintKey hintKey) {
            hintKey.SetArtifact();
            hintKey.SetFinished();
            TagTreePointer existingArtifactTag = new TagTreePointer(document);
            if (context.GetWaitingTagsManager().TryMovePointerToWaitingTag(existingArtifactTag, hintKey)) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Tagging.LayoutTaggingHelper));
                logger.LogError(iText.IO.Logs.IoLogMessageConstant.ALREADY_TAGGED_HINT_MARKED_ARTIFACT);
                context.GetWaitingTagsManager().RemoveWaitingState(hintKey);
                if (immediateFlush) {
                    existingArtifactTag.FlushParentsIfAllKidsFlushed();
                }
            }
            IList<TaggingHintKey> kidsHint = GetKidsHint(hintKey);
            foreach (TaggingHintKey kidKey in kidsHint) {
                MarkArtifactHint(kidKey);
            }
            RemoveParentHint(hintKey);
        }

        public virtual TagTreePointer UseAutoTaggingPointerAndRememberItsPosition(IRenderer renderer) {
            TagTreePointer autoTaggingPointer = context.GetAutoTaggingPointer();
            TagTreePointer position = new TagTreePointer(autoTaggingPointer);
            autoTaggingPointerSavedPosition.Put(renderer, position);
            return autoTaggingPointer;
        }

        public virtual void RestoreAutoTaggingPointerPosition(IRenderer renderer) {
            TagTreePointer autoTaggingPointer = context.GetAutoTaggingPointer();
            TagTreePointer position = autoTaggingPointerSavedPosition.JRemove(renderer);
            if (position != null) {
                autoTaggingPointer.MoveToPointer(position);
            }
        }

        public virtual IList<TaggingHintKey> GetKidsHint(TaggingHintKey parent) {
            IList<TaggingHintKey> kidsHint = kidsHints.Get(parent);
            if (kidsHint == null) {
                return JavaCollectionsUtil.EmptyList<TaggingHintKey>();
            }
            return JavaCollectionsUtil.UnmodifiableList<TaggingHintKey>(kidsHint);
        }

        public virtual IList<TaggingHintKey> GetAccessibleKidsHint(TaggingHintKey parent) {
            IList<TaggingHintKey> kidsHint = kidsHints.Get(parent);
            if (kidsHint == null) {
                return JavaCollectionsUtil.EmptyList<TaggingHintKey>();
            }
            IList<TaggingHintKey> accessibleKids = new List<TaggingHintKey>();
            foreach (TaggingHintKey kid in kidsHint) {
                if (IsNonAccessibleHint(kid)) {
                    accessibleKids.AddAll(GetAccessibleKidsHint(kid));
                }
                else {
                    accessibleKids.Add(kid);
                }
            }
            return accessibleKids;
        }

        public virtual TaggingHintKey GetParentHint(IPropertyContainer hintOwner) {
            TaggingHintKey hintKey = GetHintKey(hintOwner);
            if (hintKey == null) {
                return null;
            }
            return GetParentHint(hintKey);
        }

        public virtual TaggingHintKey GetParentHint(TaggingHintKey hintKey) {
            return parentHints.Get(hintKey);
        }

        public virtual TaggingHintKey GetAccessibleParentHint(TaggingHintKey hintKey) {
            do {
                hintKey = GetParentHint(hintKey);
            }
            while (hintKey != null && IsNonAccessibleHint(hintKey));
            return hintKey;
        }

        public virtual void ReleaseFinishedHints() {
            ICollection<TaggingHintKey> allHints = new HashSet<TaggingHintKey>();
            foreach (KeyValuePair<TaggingHintKey, TaggingHintKey> entry in parentHints) {
                allHints.Add(entry.Key);
                allHints.Add(entry.Value);
            }
            foreach (TaggingHintKey hint in allHints) {
                if (!hint.IsFinished() || IsNonAccessibleHint(hint) || hint.GetAccessibleElement() is TaggingDummyElement) {
                    continue;
                }
                FinishDummyKids(GetKidsHint(hint));
            }
            ICollection<TaggingHintKey> hintsToBeHeld = new HashSet<TaggingHintKey>();
            foreach (TaggingHintKey hint in allHints) {
                if (!IsNonAccessibleHint(hint)) {
                    IList<TaggingHintKey> siblingsHints = GetAccessibleKidsHint(hint);
                    bool holdTheFirstFinishedToBeFound = false;
                    foreach (TaggingHintKey sibling in siblingsHints) {
                        if (!sibling.IsFinished()) {
                            holdTheFirstFinishedToBeFound = true;
                        }
                        else {
                            if (holdTheFirstFinishedToBeFound) {
                                // here true == sibling.isFinished
                                hintsToBeHeld.Add(sibling);
                                holdTheFirstFinishedToBeFound = false;
                            }
                        }
                    }
                }
            }
            foreach (TaggingHintKey hint in allHints) {
                if (hint.IsFinished()) {
                    ReleaseHint(hint, hintsToBeHeld, true);
                }
            }
        }

        public virtual void ReleaseAllHints() {
            foreach (TaggingDummyElement dummy in dummiesForPreExistingTags.Values) {
                FinishTaggingHint(dummy);
                FinishDummyKids(GetKidsHint(GetHintKey(dummy)));
            }
            dummiesForPreExistingTags.Clear();
            ReleaseFinishedHints();
            ICollection<TaggingHintKey> hangingHints = new HashSet<TaggingHintKey>();
            foreach (KeyValuePair<TaggingHintKey, TaggingHintKey> entry in parentHints) {
                hangingHints.Add(entry.Key);
                hangingHints.Add(entry.Value);
            }
            foreach (TaggingHintKey hint in hangingHints) {
                // In some situations we need to remove tagging hints of renderers that are thrown away for reasons like:
                // - fixed height clipping
                // - forced placement
                // - some other cases?
                // if (!hint.isFinished()) {
                //      Logger logger = LoggerFactory.getLogger(LayoutTaggingHelper.class);
                //      logger.warn(LogMessageConstant.TAGGING_HINT_NOT_FINISHED_BEFORE_CLOSE);
                // }
                ReleaseHint(hint, null, false);
            }
            System.Diagnostics.Debug.Assert(parentHints.IsEmpty());
            System.Diagnostics.Debug.Assert(kidsHints.IsEmpty());
        }

        public virtual bool CreateTag(IRenderer renderer, TagTreePointer tagPointer) {
            TaggingHintKey hintKey = GetHintKey(renderer);
            bool noHint = hintKey == null;
            if (noHint) {
                hintKey = GetOrCreateHintKey(renderer, false);
            }
            bool created = CreateTag(hintKey, tagPointer);
            if (noHint) {
                hintKey.SetFinished();
                context.GetWaitingTagsManager().RemoveWaitingState(hintKey);
            }
            return created;
        }

        public virtual bool CreateTag(TaggingHintKey hintKey, TagTreePointer tagPointer) {
            if (hintKey.IsArtifact()) {
                return false;
            }
            bool created = CreateSingleTag(hintKey, tagPointer);
            if (created) {
                IList<TaggingHintKey> kidsHint = GetAccessibleKidsHint(hintKey);
                foreach (TaggingHintKey hint in kidsHint) {
                    if (hint.GetAccessibleElement() is TaggingDummyElement) {
                        CreateTag(hint, new TagTreePointer(document));
                    }
                }
            }
            return created;
        }

        public virtual void FinishTaggingHint(IPropertyContainer hintOwner) {
            TaggingHintKey rendererKey = GetHintKey(hintOwner);
            // artifact is always finished
            if (rendererKey == null || rendererKey.IsFinished()) {
                return;
            }
            if (rendererKey.IsElementBasedFinishingOnly() && !(hintOwner is IElement)) {
                // avoid auto finishing of hints created based on IElements
                return;
            }
            if (!IsNonAccessibleHint(rendererKey)) {
                IAccessibleElement modelElement = rendererKey.GetAccessibleElement();
                String role = modelElement.GetAccessibilityProperties().GetRole();
                if (rendererKey.GetOverriddenRole() != null) {
                    role = rendererKey.GetOverriddenRole();
                }
                IList<ITaggingRule> rules = taggingRules.Get(role);
                bool ruleResult = true;
                if (rules != null) {
                    foreach (ITaggingRule rule in rules) {
                        ruleResult = ruleResult && rule.OnTagFinish(this, rendererKey);
                    }
                }
                if (!ruleResult) {
                    return;
                }
            }
            rendererKey.SetFinished();
        }

        public virtual int ReplaceKidHint(TaggingHintKey kidHintKey, ICollection<TaggingHintKey> newKidsHintKeys) {
            TaggingHintKey parentKey = GetParentHint(kidHintKey);
            if (parentKey == null) {
                return -1;
            }
            if (kidHintKey.IsFinished()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Tagging.LayoutTaggingHelper));
                logger.LogError(iText.IO.Logs.IoLogMessageConstant.CANNOT_REPLACE_FINISHED_HINT);
                // If kidHintKey is finished you won't be able to add it anywhere after replacing is ended.
                // If kidHintKey might be finished, use moveKidHint instead.
                // replaceKidHint should be used when parent might be finished.
                return -1;
            }
            int kidIndex = RemoveParentHint(kidHintKey);
            IList<TaggingHintKey> kidsToBeAdded = new List<TaggingHintKey>();
            foreach (TaggingHintKey newKidKey in newKidsHintKeys) {
                int i = RemoveParentHint(newKidKey);
                if (i == RETVAL_PARENT_AND_KID_FINISHED || i == RETVAL_NO_PARENT && newKidKey.IsFinished()) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Tagging.LayoutTaggingHelper));
                    logger.LogError(iText.IO.Logs.IoLogMessageConstant.CANNOT_MOVE_FINISHED_HINT);
                    continue;
                }
                kidsToBeAdded.Add(newKidKey);
            }
            AddKidsHint(parentKey, kidsToBeAdded, kidIndex, true);
            return kidIndex;
        }

        public virtual int MoveKidHint(TaggingHintKey hintKeyOfKidToMove, TaggingHintKey newParent) {
            return MoveKidHint(hintKeyOfKidToMove, newParent, -1);
        }

        public virtual int MoveKidHint(TaggingHintKey hintKeyOfKidToMove, TaggingHintKey newParent, int insertIndex
            ) {
            if (newParent.IsFinished()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Tagging.LayoutTaggingHelper));
                logger.LogError(iText.IO.Logs.IoLogMessageConstant.CANNOT_MOVE_HINT_TO_FINISHED_PARENT);
                return -1;
            }
            int removeRes = RemoveParentHint(hintKeyOfKidToMove);
            if (removeRes == RETVAL_PARENT_AND_KID_FINISHED || removeRes == RETVAL_NO_PARENT && hintKeyOfKidToMove.IsFinished
                ()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Tagging.LayoutTaggingHelper));
                logger.LogError(iText.IO.Logs.IoLogMessageConstant.CANNOT_MOVE_FINISHED_HINT);
                return -1;
            }
            AddKidsHint(newParent, JavaCollectionsUtil.SingletonList<TaggingHintKey>(hintKeyOfKidToMove), insertIndex, 
                true);
            return removeRes;
        }

        public virtual PdfDocument GetPdfDocument() {
            return document;
        }

        private static TaggingHintKey GetOrCreateHintKey(IPropertyContainer hintOwner, bool setProperty) {
            TaggingHintKey hintKey = hintOwner.GetProperty<TaggingHintKey>(Property.TAGGING_HINT_KEY);
            if (hintKey == null) {
                IAccessibleElement elem = null;
                if (hintOwner is IAccessibleElement) {
                    elem = (IAccessibleElement)hintOwner;
                }
                else {
                    if (hintOwner is IRenderer && ((IRenderer)hintOwner).GetModelElement() is IAccessibleElement) {
                        elem = (IAccessibleElement)((IRenderer)hintOwner).GetModelElement();
                    }
                }
                hintKey = new TaggingHintKey(elem, hintOwner is IElement);
                if (elem != null && StandardRoles.ARTIFACT.Equals(elem.GetAccessibilityProperties().GetRole())) {
                    hintKey.SetArtifact();
                    hintKey.SetFinished();
                }
                if (setProperty) {
                    if (elem is ILargeElement && !((ILargeElement)elem).IsComplete()) {
                        ((ILargeElement)elem).SetProperty(Property.TAGGING_HINT_KEY, hintKey);
                    }
                    else {
                        hintOwner.SetProperty(Property.TAGGING_HINT_KEY, hintKey);
                    }
                }
            }
            return hintKey;
        }

        private void AddKidsHint(TaggingHintKey parentKey, ICollection<TaggingHintKey> newKidsKeys, int insertIndex
            , bool skipFinishedChecks) {
            if (newKidsKeys.IsEmpty()) {
                return;
            }
            if (parentKey.IsArtifact()) {
                foreach (TaggingHintKey kid in newKidsKeys) {
                    MarkArtifactHint(kid);
                }
                return;
            }
            if (!skipFinishedChecks && parentKey.IsFinished()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Tagging.LayoutTaggingHelper));
                logger.LogError(iText.IO.Logs.IoLogMessageConstant.CANNOT_ADD_HINTS_TO_FINISHED_PARENT);
                return;
            }
            IList<TaggingHintKey> kidsHint = kidsHints.Get(parentKey);
            if (kidsHint == null) {
                kidsHint = new List<TaggingHintKey>();
            }
            TaggingHintKey parentTagHint = IsNonAccessibleHint(parentKey) ? GetAccessibleParentHint(parentKey) : parentKey;
            bool parentTagAlreadyCreated = parentTagHint != null && IsTagAlreadyExistsForHint(parentTagHint);
            foreach (TaggingHintKey kidKey in newKidsKeys) {
                if (kidKey.IsArtifact()) {
                    continue;
                }
                TaggingHintKey prevParent = GetParentHint(kidKey);
                if (prevParent != null) {
                    // Seems to be a legit use case to re-add hints to just ensure that hints are added
                    // Logger logger = LoggerFactory.getLogger(LayoutTaggingHelper.class);
                    // logger.error(LogMessageConstant.CANNOT_ADD_KID_HINT_WHICH_IS_ALREADY_ADDED_TO_ANOTHER_PARENT);
                    continue;
                }
                if (!skipFinishedChecks && kidKey.IsFinished()) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Tagging.LayoutTaggingHelper));
                    logger.LogError(iText.IO.Logs.IoLogMessageConstant.CANNOT_ADD_FINISHED_HINT_AS_A_NEW_KID_HINT);
                    continue;
                }
                if (insertIndex > -1) {
                    kidsHint.Add(insertIndex++, kidKey);
                }
                else {
                    kidsHint.Add(kidKey);
                }
                kidsHints.Put(parentKey, kidsHint);
                parentHints.Put(kidKey, parentKey);
                if (parentTagAlreadyCreated) {
                    if (kidKey.GetAccessibleElement() is TaggingDummyElement) {
                        CreateTag(kidKey, new TagTreePointer(document));
                    }
                    if (IsNonAccessibleHint(kidKey)) {
                        foreach (TaggingHintKey nestedKid in GetAccessibleKidsHint(kidKey)) {
                            if (nestedKid.GetAccessibleElement() is TaggingDummyElement) {
                                CreateTag(nestedKid, new TagTreePointer(document));
                            }
                            MoveKidTagIfCreated(parentTagHint, nestedKid);
                        }
                    }
                    else {
                        MoveKidTagIfCreated(parentTagHint, kidKey);
                    }
                }
            }
        }

        private bool CreateSingleTag(TaggingHintKey hintKey, TagTreePointer tagPointer) {
            if (hintKey.IsFinished()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Tagging.LayoutTaggingHelper));
                logger.LogError(iText.IO.Logs.IoLogMessageConstant.ATTEMPT_TO_CREATE_A_TAG_FOR_FINISHED_HINT);
                return false;
            }
            if (IsNonAccessibleHint(hintKey)) {
                // try move pointer to the nearest accessible parent in case any direct content will be
                // tagged with this tagPointer
                TaggingHintKey parentTagHint = GetAccessibleParentHint(hintKey);
                context.GetWaitingTagsManager().TryMovePointerToWaitingTag(tagPointer, parentTagHint);
                return false;
            }
            WaitingTagsManager waitingTagsManager = context.GetWaitingTagsManager();
            if (!waitingTagsManager.TryMovePointerToWaitingTag(tagPointer, hintKey)) {
                IAccessibleElement modelElement = hintKey.GetAccessibleElement();
                TaggingHintKey parentHint = GetAccessibleParentHint(hintKey);
                int ind = -1;
                if (parentHint != null) {
                    // if parent tag hasn't been created yet - it's ok, kid tags will be moved on it's creation
                    if (waitingTagsManager.TryMovePointerToWaitingTag(tagPointer, parentHint)) {
                        IList<TaggingHintKey> siblingsHint = GetAccessibleKidsHint(parentHint);
                        int i = siblingsHint.IndexOf(hintKey);
                        ind = GetNearestNextSiblingTagIndex(waitingTagsManager, tagPointer, siblingsHint, i);
                    }
                }
                tagPointer.AddTag(ind, modelElement.GetAccessibilityProperties());
                if (hintKey.GetOverriddenRole() != null) {
                    tagPointer.SetRole(hintKey.GetOverriddenRole());
                }
                waitingTagsManager.AssignWaitingState(tagPointer, hintKey);
                IList<TaggingHintKey> kidsHint = GetAccessibleKidsHint(hintKey);
                foreach (TaggingHintKey kidKey in kidsHint) {
                    MoveKidTagIfCreated(hintKey, kidKey);
                }
                return true;
            }
            return false;
        }

        private int RemoveParentHint(TaggingHintKey hintKey) {
            TaggingHintKey parentHint = parentHints.Get(hintKey);
            if (parentHint == null) {
                return RETVAL_NO_PARENT;
            }
            TaggingHintKey accessibleParentHint = GetAccessibleParentHint(hintKey);
            if (hintKey.IsFinished() && parentHint.IsFinished() && (accessibleParentHint == null || accessibleParentHint
                .IsFinished())) {
                return RETVAL_PARENT_AND_KID_FINISHED;
            }
            return RemoveParentHint(hintKey, parentHint);
        }

        private int RemoveParentHint(TaggingHintKey hintKey, TaggingHintKey parentHint) {
            parentHints.JRemove(hintKey);
            IList<TaggingHintKey> kidsHint = kidsHints.Get(parentHint);
            int i;
            int size = kidsHint.Count;
            for (i = 0; i < size; ++i) {
                if (kidsHint[i] == hintKey) {
                    kidsHint.JRemoveAt(i);
                    break;
                }
            }
            System.Diagnostics.Debug.Assert(i < size);
            if (kidsHint.IsEmpty()) {
                kidsHints.JRemove(parentHint);
            }
            return i;
        }

        private void FinishDummyKids(IList<TaggingHintKey> taggingHintKeys) {
            foreach (TaggingHintKey hintKey in taggingHintKeys) {
                bool isDummy = hintKey.GetAccessibleElement() is TaggingDummyElement;
                if (isDummy) {
                    FinishTaggingHint((IPropertyContainer)hintKey.GetAccessibleElement());
                }
                if (IsNonAccessibleHint(hintKey) || isDummy) {
                    FinishDummyKids(GetKidsHint(hintKey));
                }
            }
        }

        private void MoveKidTagIfCreated(TaggingHintKey parentKey, TaggingHintKey kidKey) {
            // both arguments shall be accessible, non-accessible are not handled inside this method
            TagTreePointer kidPointer = new TagTreePointer(document);
            WaitingTagsManager waitingTagsManager = context.GetWaitingTagsManager();
            if (!waitingTagsManager.TryMovePointerToWaitingTag(kidPointer, kidKey)) {
                return;
            }
            TagTreePointer parentPointer = new TagTreePointer(document);
            if (!waitingTagsManager.TryMovePointerToWaitingTag(parentPointer, parentKey)) {
                return;
            }
            int kidIndInParentKidsHint = GetAccessibleKidsHint(parentKey).IndexOf(kidKey);
            int ind = GetNearestNextSiblingTagIndex(waitingTagsManager, parentPointer, GetAccessibleKidsHint(parentKey
                ), kidIndInParentKidsHint);
            parentPointer.SetNextNewKidIndex(ind);
            kidPointer.Relocate(parentPointer);
        }

        private int GetNearestNextSiblingTagIndex(WaitingTagsManager waitingTagsManager, TagTreePointer parentPointer
            , IList<TaggingHintKey> siblingsHint, int start) {
            int ind = -1;
            TagTreePointer nextSiblingPointer = new TagTreePointer(document);
            while (++start < siblingsHint.Count) {
                if (waitingTagsManager.TryMovePointerToWaitingTag(nextSiblingPointer, siblingsHint[start]) && parentPointer
                    .IsPointingToSameTag(new TagTreePointer(nextSiblingPointer).MoveToParent())) {
                    ind = nextSiblingPointer.GetIndexInParentKidsList();
                    break;
                }
            }
            return ind;
        }

        private static bool IsNonAccessibleHint(TaggingHintKey hintKey) {
            return hintKey.GetAccessibleElement() == null || hintKey.GetAccessibleElement().GetAccessibilityProperties
                ().GetRole() == null;
        }

        private bool IsTagAlreadyExistsForHint(TaggingHintKey tagHint) {
            return context.GetWaitingTagsManager().IsObjectAssociatedWithWaitingTag(tagHint);
        }

        private void ReleaseHint(TaggingHintKey hint, ICollection<TaggingHintKey> hintsToBeHeld, bool checkContextIsFinished
            ) {
            TaggingHintKey parentHint = parentHints.Get(hint);
            IList<TaggingHintKey> kidsHint = kidsHints.Get(hint);
            if (checkContextIsFinished && parentHint != null) {
                if (IsSomeParentNotFinished(parentHint)) {
                    return;
                }
            }
            if (checkContextIsFinished && kidsHint != null) {
                if (IsSomeKidNotFinished(hint)) {
                    return;
                }
            }
            if (checkContextIsFinished && hintsToBeHeld != null) {
                if (hintsToBeHeld.Contains(hint)) {
                    return;
                }
            }
            if (parentHint != null) {
                RemoveParentHint(hint, parentHint);
            }
            if (kidsHint != null) {
                foreach (TaggingHintKey kidHint in kidsHint) {
                    parentHints.JRemove(kidHint);
                }
                kidsHints.JRemove(hint);
            }
            TagTreePointer tagPointer = new TagTreePointer(document);
            if (context.GetWaitingTagsManager().TryMovePointerToWaitingTag(tagPointer, hint)) {
                context.GetWaitingTagsManager().RemoveWaitingState(hint);
                if (immediateFlush) {
                    tagPointer.FlushParentsIfAllKidsFlushed();
                }
            }
            else {
                context.GetWaitingTagsManager().RemoveWaitingState(hint);
            }
        }

        private bool IsSomeParentNotFinished(TaggingHintKey parentHint) {
            TaggingHintKey hintKey = parentHint;
            while (true) {
                if (hintKey == null) {
                    return false;
                }
                if (!hintKey.IsFinished()) {
                    return true;
                }
                if (!IsNonAccessibleHint(hintKey)) {
                    return false;
                }
                hintKey = GetParentHint(hintKey);
            }
        }

        private bool IsSomeKidNotFinished(TaggingHintKey hint) {
            foreach (TaggingHintKey kidHint in GetKidsHint(hint)) {
                if (!kidHint.IsFinished()) {
                    return true;
                }
                if (IsNonAccessibleHint(kidHint) && IsSomeKidNotFinished(kidHint)) {
                    return true;
                }
            }
            return false;
        }

        private void RegisterRules(PdfVersion pdfVersion) {
            ITaggingRule tableRule = new TableTaggingRule();
            RegisterSingleRule(StandardRoles.TABLE, tableRule);
            RegisterSingleRule(StandardRoles.TFOOT, tableRule);
            RegisterSingleRule(StandardRoles.THEAD, tableRule);
            if (pdfVersion.CompareTo(PdfVersion.PDF_1_5) < 0) {
                TableTaggingPriorToOneFiveVersionRule priorToOneFiveRule = new TableTaggingPriorToOneFiveVersionRule();
                RegisterSingleRule(StandardRoles.TABLE, priorToOneFiveRule);
                RegisterSingleRule(StandardRoles.THEAD, priorToOneFiveRule);
                RegisterSingleRule(StandardRoles.TFOOT, priorToOneFiveRule);
            }
        }

        private void RegisterSingleRule(String role, ITaggingRule rule) {
            IList<ITaggingRule> rules = taggingRules.Get(role);
            if (rules == null) {
                rules = new List<ITaggingRule>();
                taggingRules.Put(role, rules);
            }
            rules.Add(rule);
        }
    }
}
