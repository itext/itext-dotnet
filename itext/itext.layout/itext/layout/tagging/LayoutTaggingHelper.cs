/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Commons.Internal.Runtime;
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
        private readonly TagStructureContext context;

        private readonly PdfDocument document;

        private readonly bool immediateFlush;

        // kidsHints and parentHints fields represent tree of TaggingHintKey, where parentHints
        // stores a parent for the key, and kidsHints stores kids for key.
        private readonly IDictionary<TaggingHintKey, IList<TaggingHintKey>> kidsHints;

        private readonly IDictionary<TaggingHintKey, TaggingHintKey> parentHints;

        private readonly IDictionary<IRenderer, TagTreePointer> autoTaggingPointerSavedPosition;

        private readonly IDictionary<String, IList<ITaggingRule>> taggingRules;

        // dummiesForPreExistingTags is used to process TaggingDummyElement
        private readonly IDictionary<PdfObject, TaggingDummyElement> dummiesForPreExistingTags;

        private readonly int RETVAL_NO_PARENT = -1;

        private readonly int RETVAL_PARENT_AND_KID_FINISHED = -2;

        private int lastId = 0;

        /// <summary>
        /// Instantiates a new
        /// <see cref="LayoutTaggingHelper"/>
        /// instance for managing layout-level tagging.
        /// </summary>
        /// <remarks>
        /// Instantiates a new
        /// <see cref="LayoutTaggingHelper"/>
        /// instance for managing layout-level tagging.
        /// <para />This helper maintains a tree of tagging hints that represent the logical structure of a PDF document
        /// and coordinates tag creation in the PDF structure tree. It automatically registers default tagging rules
        /// for standard roles (e.g., TABLE, THEAD, TFOOT, TH) based on the PDF version.
        /// </remarks>
        /// <param name="document">the PDF document being created or modified</param>
        /// <param name="immediateFlush">
        /// if
        /// <see langword="true"/>
        /// , parent tags will be flushed as soon as all their children are flushed;
        /// if
        /// <see langword="false"/>
        /// , tag flushing is deferred until explicitly requested
        /// </param>
        /// <seealso cref="ReleaseFinishedHints()"/>
        /// <seealso cref="ReleaseAllHints()"/>
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

        /// <summary>Recursively registers tagging hints from a renderer tree, preserving the logical structure.</summary>
        /// <remarks>
        /// Recursively registers tagging hints from a renderer tree, preserving the logical structure.
        /// <para />This utility method traverses the renderer hierarchy and calls
        /// <see cref="AddKidsHint(iText.Layout.IPropertyContainer, System.Collections.Generic.IEnumerable{T})"/>
        /// for each renderer and its children, ensuring all parent-child relationships are captured in the tagging system.
        /// </remarks>
        /// <param name="taggingHelper">the helper instance managing tags</param>
        /// <param name="rootRenderer">the root renderer of the tree to process recursively</param>
        /// <seealso cref="AddKidsHint(iText.Layout.IPropertyContainer, System.Collections.Generic.IEnumerable{T})"/>
        /// <seealso cref="iText.Layout.Renderer.IRenderer.GetChildRenderers()"/>
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
            if (rootRenderer is AbstractRenderer) {
                taggingHelper.AddKidsHint(rootRenderer, ((AbstractRenderer)rootRenderer).GetPositionenRenderers());
                foreach (IRenderer childRenderer in ((AbstractRenderer)rootRenderer).GetPositionenRenderers()) {
                    AddTreeHints(taggingHelper, childRenderer);
                }
            }
        }

        /// <summary>Retrieves an existing tagging hint key for the given container without creating one.</summary>
        /// <remarks>
        /// Retrieves an existing tagging hint key for the given container without creating one.
        /// <para />If no hint key has been created for this container, returns
        /// <see langword="null"/>.
        /// Use
        /// <see cref="GetOrCreateHintKey(iText.Layout.IPropertyContainer)"/>
        /// if you need to ensure a hint exists.
        /// </remarks>
        /// <param name="container">the element or renderer to retrieve the hint for</param>
        /// <returns>
        /// the
        /// <see cref="TaggingHintKey"/>
        /// associated with the container, or
        /// <see langword="null"/>
        /// if not yet created
        /// </returns>
        /// <seealso cref="GetOrCreateHintKey(iText.Layout.IPropertyContainer)"/>
        /// <seealso cref="iText.Layout.Properties.Property.TAGGING_HINT_KEY"/>
        public static TaggingHintKey GetHintKey(IPropertyContainer container) {
            return container.GetProperty<TaggingHintKey>(Property.TAGGING_HINT_KEY);
        }

        /// <summary>Gets or creates a tagging hint key for the given container.</summary>
        /// <remarks>
        /// Gets or creates a tagging hint key for the given container.
        /// <para />If a hint key already exists for this container, returns it. Otherwise, creates a new
        /// <see cref="TaggingHintKey"/>
        /// , stores it in the container's properties, and returns it.
        /// <para />For
        /// <see cref="iText.Layout.Element.ILargeElement"/>
        /// s that are not yet complete, the hint key is stored on the element itself
        /// rather than on the renderer, to preserve the hint across renderer recreation.
        /// <para />If the container's role is
        /// <see cref="iText.Kernel.Pdf.Tagging.StandardRoles.ARTIFACT"/>
        /// , the created hint is automatically
        /// marked as artifact and finished.
        /// </remarks>
        /// <param name="container">the element or renderer to get or create a hint for</param>
        /// <returns>
        /// the existing or newly created
        /// <see cref="TaggingHintKey"/>
        /// </returns>
        /// <seealso cref="GetHintKey(iText.Layout.IPropertyContainer)"/>
        /// <seealso cref="TaggingHintKey"/>
        /// <seealso cref="iText.Layout.Properties.Property.TAGGING_HINT_KEY"/>
        public static TaggingHintKey GetOrCreateHintKey(IPropertyContainer container) {
            return GetOrCreateHintKey(container, true);
        }

        /// <summary>Registers child hints for a pre-existing PDF tag (mapped via TagTreePointer).</summary>
        /// <remarks>
        /// Registers child hints for a pre-existing PDF tag (mapped via TagTreePointer).
        /// <para />This method is used when you have a pre-existing tag structure element (from a PDF that already
        /// contains tags) and need to associate new children with it. The helper creates a
        /// <see cref="TaggingDummyElement"/>
        /// wrapper to manage the pre-existing tag and adds the new children under it.
        /// <para />This is useful for merging external PDFs or handling documents that were partially tagged
        /// before layout processing.
        /// </remarks>
        /// <param name="parentPointer">
        /// the
        /// <see cref="iText.Kernel.Pdf.Tagutils.TagTreePointer"/>
        /// pointing to the pre-existing parent tag
        /// </param>
        /// <param name="newKids">the children to add under the parent tag</param>
        /// <seealso cref="TaggingDummyElement"/>
        /// <seealso cref="iText.Kernel.Pdf.Tagutils.WaitingTagsManager.AssignWaitingState(iText.Kernel.Pdf.Tagutils.TagTreePointer, System.Object)
        ///     "/>
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

        /// <summary>Registers children hints for a parent element or renderer (append mode).</summary>
        /// <remarks>
        /// Registers children hints for a parent element or renderer (append mode).
        /// <para />This method declares that the given children should appear as kids of the parent in the PDF structure tree.
        /// Children are appended to any existing children. This method creates
        /// <see cref="TaggingHintKey"/>
        /// s for each child
        /// if they don't already exist.
        /// </remarks>
        /// <param name="parent">the parent element or renderer</param>
        /// <param name="newKids">the children to add under the parent (can be elements or renderers)</param>
        /// <seealso cref="AddKidsHint(iText.Layout.IPropertyContainer, System.Collections.Generic.IEnumerable{T}, int)
        ///     "/>
        /// <seealso cref="FinishTaggingHint(iText.Layout.IPropertyContainer)"/>
        public virtual void AddKidsHint<_T0>(IPropertyContainer parent, IEnumerable<_T0> newKids)
            where _T0 : IPropertyContainer {
            AddKidsHint(parent, newKids, -1);
        }

        /// <summary>Registers children hints for a parent element or renderer (with insert position).</summary>
        /// <remarks>
        /// Registers children hints for a parent element or renderer (with insert position).
        /// <para />This method declares that the given children should appear as kids of the parent in the PDF structure tree,
        /// optionally at a specific position. If
        /// <paramref name="insertIndex"/>
        /// is negative, children are appended.
        /// <para />If the parent tag has already been created in the PDF structure tree, this method will relocate
        /// child tags into the parent.
        /// </remarks>
        /// <param name="parent">the parent element or renderer</param>
        /// <param name="newKids">the children to add under the parent</param>
        /// <param name="insertIndex">the position at which to insert the first child; negative means append at end</param>
        /// <seealso cref="AddKidsHint(iText.Layout.IPropertyContainer, System.Collections.Generic.IEnumerable{T})"/>
        /// <seealso cref="AddKidsHint(TaggingHintKey, System.Collections.Generic.ICollection{E}, int)"/>
        public virtual void AddKidsHint<_T0>(IPropertyContainer parent, IEnumerable<_T0> newKids, int insertIndex)
            where _T0 : IPropertyContainer {
            if (parent is AreaBreakRenderer || parent is SectionBreakRenderer) {
                return;
            }
            TaggingHintKey parentKey = GetOrCreateHintKey(parent);
            if (parent is IRenderer && this.GetPdfDocument().GetDiContainer().IsRegistered(typeof(ProhibitedTagRelationsResolver
                ))) {
                this.GetPdfDocument().GetDiContainer().GetInstance<ProhibitedTagRelationsResolver>().RepairTagStructure(this
                    , (IRenderer)parent);
            }
            IList<TaggingHintKey> newKidsKeys = new List<TaggingHintKey>();
            foreach (IPropertyContainer kid in newKids) {
                if (kid is AreaBreakRenderer || kid is SectionBreakRenderer) {
                    return;
                }
                TaggingHintKey kidHint = GetOrCreateHintKey(kid);
                newKidsKeys.Add(kidHint);
            }
            AddKidsHint(parentKey, newKidsKeys, insertIndex);
        }

        /// <summary>
        /// Registers children hints using
        /// <see cref="TaggingHintKey"/>
        /// s directly (append mode).
        /// </summary>
        /// <remarks>
        /// Registers children hints using
        /// <see cref="TaggingHintKey"/>
        /// s directly (append mode).
        /// <para />This variant works directly with
        /// <see cref="TaggingHintKey"/>
        /// objects instead of containers,
        /// useful when you already have the hint keys or when working with internal hint manipulation.
        /// </remarks>
        /// <param name="parentKey">the parent hint key</param>
        /// <param name="newKidsKeys">the hint keys of children to add</param>
        /// <seealso cref="AddKidsHint(TaggingHintKey, System.Collections.Generic.ICollection{E}, int)"/>
        public virtual void AddKidsHint(TaggingHintKey parentKey, ICollection<TaggingHintKey> newKidsKeys) {
            AddKidsHint(parentKey, newKidsKeys, -1);
        }

        /// <summary>
        /// Registers children hints using
        /// <see cref="TaggingHintKey"/>
        /// s directly (with insert position).
        /// </summary>
        /// <remarks>
        /// Registers children hints using
        /// <see cref="TaggingHintKey"/>
        /// s directly (with insert position).
        /// <para />This variant works directly with
        /// <see cref="TaggingHintKey"/>
        /// objects and supports specifying
        /// an insertion position. This is the core method that other
        /// <c>addKidsHint</c>
        /// overloads delegate to.
        /// </remarks>
        /// <param name="parentKey">the parent hint key</param>
        /// <param name="newKidsKeys">the hint keys of children to add</param>
        /// <param name="insertIndex">the position at which to insert the first child; negative means append at end</param>
        /// <seealso cref="AddKidsHint(TaggingHintKey, System.Collections.Generic.ICollection{E})"/>
        public virtual void AddKidsHint(TaggingHintKey parentKey, ICollection<TaggingHintKey> newKidsKeys, int insertIndex
            ) {
            AddKidsHint(parentKey, newKidsKeys, insertIndex, false);
        }

        /// <summary>Overrides the PDF role for an element's tag in the structure tree.</summary>
        /// <remarks>
        /// Overrides the PDF role for an element's tag in the structure tree.
        /// <para />By default, a tag's role is determined from the element's accessibility properties. This method
        /// allows you to override that role at runtime. The override is applied when the tag is created.
        /// <para /><strong>Important:</strong> Apply role overrides <em>before</em>
        /// calling
        /// <see cref="FinishTaggingHint(iText.Layout.IPropertyContainer)"/>.
        /// Once tagging rules have been applied during finishing, re-applying the same rules for a new role will not occur.
        /// </remarks>
        /// <param name="hintOwner">the element or renderer whose tag role should be overridden</param>
        /// <param name="role">
        /// the new PDF role (e.g.,
        /// <see cref="iText.Kernel.Pdf.Tagging.StandardRoles.SPAN"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.Tagging.StandardRoles.STRONG"/>
        /// )
        /// </param>
        /// <seealso cref="iText.Kernel.Pdf.Tagging.StandardRoles"/>
        /// <seealso cref="FinishTaggingHint(iText.Layout.IPropertyContainer)"/>
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

        /// <summary>Checks whether the given container is marked as an artifact (non-accessible).</summary>
        /// <remarks>
        /// Checks whether the given container is marked as an artifact (non-accessible).
        /// <para />An artifact is content that should not appear in the accessibility tree, such as decorative
        /// elements. Artifacts are not included in the PDF structure tree.
        /// <para />This method checks:
        /// <list type="number">
        /// <item><description>If a hint exists and is marked as artifact, returns
        /// <see langword="true"/>
        /// </description></item>
        /// <item><description>If the container's accessibility role is
        /// <see cref="iText.Kernel.Pdf.Tagging.StandardRoles.ARTIFACT"/>
        /// , returns
        /// <see langword="true"/>
        /// </description></item>
        /// <item><description>Otherwise returns
        /// <see langword="false"/>
        /// </description></item>
        /// </list>
        /// </remarks>
        /// <param name="hintOwner">the element or renderer to check</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the container is an artifact,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        /// <seealso cref="MarkArtifactHint(iText.Layout.IPropertyContainer)"/>
        /// <seealso cref="iText.Kernel.Pdf.Tagging.StandardRoles.ARTIFACT"/>
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

        /// <summary>Marks an element or renderer as an artifact (non-accessible content).</summary>
        /// <remarks>
        /// Marks an element or renderer as an artifact (non-accessible content).
        /// <para />Artifacts are excluded from the PDF accessibility tree and are not exposed to assistive technologies.
        /// Use this for decorative elements, borders, backgrounds, or other non-semantic content.
        /// <para />This method:
        /// <list type="bullet">
        /// <item><description>Marks the hint as artifact and finished
        /// </description></item>
        /// <item><description>Recursively marks all children as artifacts
        /// </description></item>
        /// <item><description>Removes the hint from its parent (orphaning it)
        /// </description></item>
        /// <item><description>Logs an error if the artifact tag was already created in the PDF
        /// </description></item>
        /// </list>
        /// </remarks>
        /// <param name="hintOwner">the element or renderer to mark as artifact</param>
        /// <seealso cref="MarkArtifactHint(TaggingHintKey)"/>
        /// <seealso cref="IsArtifact(iText.Layout.IPropertyContainer)"/>
        public virtual void MarkArtifactHint(IPropertyContainer hintOwner) {
            TaggingHintKey hintKey = GetOrCreateHintKey(hintOwner);
            MarkArtifactHint(hintKey);
        }

        /// <summary>Marks a hint key as an artifact (non-accessible content).</summary>
        /// <remarks>
        /// Marks a hint key as an artifact (non-accessible content).
        /// <para />This is the core implementation of artifact marking. It:
        /// <list type="bullet">
        /// <item><description>Sets the artifact and finished flags on the hint
        /// </description></item>
        /// <item><description>Recursively marks all children as artifacts
        /// </description></item>
        /// <item><description>Removes the hint from its parent
        /// </description></item>
        /// <item><description>Flushes the artifact tag pointer if already created
        /// </description></item>
        /// </list>
        /// </remarks>
        /// <param name="hintKey">the hint key to mark as artifact</param>
        /// <seealso cref="MarkArtifactHint(iText.Layout.IPropertyContainer)"/>
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

        /// <summary>Saves the current auto-tagging pointer position and returns it for temporary use.</summary>
        /// <remarks>
        /// Saves the current auto-tagging pointer position and returns it for temporary use.
        /// <para />This method is useful when a renderer needs to temporarily modify the auto-tagging pointer
        /// for custom tag creation or structure manipulation. The saved position can be restored later
        /// using
        /// <see cref="RestoreAutoTaggingPointerPosition(iText.Layout.Renderer.IRenderer)"/>.
        /// <para /><strong>Usage pattern (with try-finally):</strong>
        /// <pre>
        /// <c>
        /// TagTreePointer ptr = helper.useAutoTaggingPointerAndRememberItsPosition(renderer);
        /// try
        /// ptr.addTag("CustomRole");
        /// // ... custom operations ...
        /// </c>
        /// finally
        /// helper.restoreAutoTaggingPointerPosition(renderer);
        /// }
        /// }</pre>
        /// </remarks>
        /// <param name="renderer">the renderer whose position should be saved (used as a key for restoration)</param>
        /// <returns>the current auto-tagging pointer (position at the time of call)</returns>
        /// <seealso cref="RestoreAutoTaggingPointerPosition(iText.Layout.Renderer.IRenderer)"/>
        public virtual TagTreePointer UseAutoTaggingPointerAndRememberItsPosition(IRenderer renderer) {
            TagTreePointer autoTaggingPointer = context.GetAutoTaggingPointer();
            TagTreePointer position = new TagTreePointer(autoTaggingPointer);
            autoTaggingPointerSavedPosition.Put(renderer, position);
            return autoTaggingPointer;
        }

        /// <summary>Restores the auto-tagging pointer to a previously saved position.</summary>
        /// <remarks>
        /// Restores the auto-tagging pointer to a previously saved position.
        /// <para />This method retrieves the pointer position saved by
        /// <see cref="UseAutoTaggingPointerAndRememberItsPosition(iText.Layout.Renderer.IRenderer)"/>
        /// and moves the auto-tagging pointer back
        /// to that location. If no saved position exists for the renderer, does nothing.
        /// <para /><strong>Important:</strong> Always call this in a finally block or error handling path to ensure
        /// the pointer is restored even if an exception occurs during custom tagging operations.
        /// </remarks>
        /// <param name="renderer">the renderer whose position should be restored</param>
        /// <seealso cref="UseAutoTaggingPointerAndRememberItsPosition(iText.Layout.Renderer.IRenderer)"/>
        public virtual void RestoreAutoTaggingPointerPosition(IRenderer renderer) {
            TagTreePointer autoTaggingPointer = context.GetAutoTaggingPointer();
            TagTreePointer position = autoTaggingPointerSavedPosition.JRemove(renderer);
            if (position != null) {
                autoTaggingPointer.MoveToPointer(position);
            }
        }

        /// <summary>Gets the unmodifiable list of direct children for a parent hint.</summary>
        /// <remarks>
        /// Gets the unmodifiable list of direct children for a parent hint.
        /// <para />This method returns all direct children hints, including non-accessible intermediate nodes.
        /// For accessible children only, use
        /// <see cref="GetAccessibleKidsHint(TaggingHintKey)"/>.
        /// <para />Returns an empty list if the parent has no children.
        /// </remarks>
        /// <param name="parent">the parent hint key</param>
        /// <returns>an unmodifiable list of direct child hint keys</returns>
        /// <seealso cref="GetAccessibleKidsHint(TaggingHintKey)"/>
        public virtual IList<TaggingHintKey> GetKidsHint(TaggingHintKey parent) {
            IList<TaggingHintKey> kidsHint = kidsHints.Get(parent);
            if (kidsHint == null) {
                return JavaCollectionsUtil.EmptyList<TaggingHintKey>();
            }
            return JavaCollectionsUtil.UnmodifiableList<TaggingHintKey>(kidsHint);
        }

        /// <summary>Gets the list of accessible children for a parent hint, flattening non-accessible intermediate nodes.
        ///     </summary>
        /// <remarks>
        /// Gets the list of accessible children for a parent hint, flattening non-accessible intermediate nodes.
        /// <para />This method returns only accessible children (those with a non-null role). Non-accessible
        /// intermediate nodes (grouping nodes) are recursively flattened, and their accessible descendants
        /// are included in the returned list.
        /// <para />For example, if a parent has a non-accessible child that contains two accessible children,
        /// this method returns those two accessible children directly.
        /// <para />Returns an empty list if the parent has no accessible children.
        /// </remarks>
        /// <param name="parent">the parent hint key</param>
        /// <returns>an unmodifiable list of accessible child hint keys with non-accessible intermediates flattened</returns>
        /// <seealso cref="GetKidsHint(TaggingHintKey)"/>
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

        /// <summary>Gets the parent hint of a given element or renderer.</summary>
        /// <remarks>
        /// Gets the parent hint of a given element or renderer.
        /// <para />This method retrieves the direct parent hint for the given container by first obtaining
        /// its hint key and then looking up the parent.
        /// </remarks>
        /// <param name="hintOwner">the element or renderer whose parent should be retrieved</param>
        /// <returns>
        /// the parent
        /// <see cref="TaggingHintKey"/>
        /// , or
        /// <see langword="null"/>
        /// if this is a root or has no hint
        /// </returns>
        /// <seealso cref="GetParentHint(TaggingHintKey)"/>
        /// <seealso cref="GetAccessibleParentHint(TaggingHintKey)"/>
        public virtual TaggingHintKey GetParentHint(IPropertyContainer hintOwner) {
            TaggingHintKey hintKey = GetHintKey(hintOwner);
            if (hintKey == null) {
                return null;
            }
            return GetParentHint(hintKey);
        }

        /// <summary>Gets the direct parent hint of a hint key.</summary>
        /// <param name="hintKey">the child hint key</param>
        /// <returns>
        /// the parent
        /// <see cref="TaggingHintKey"/>
        /// , or
        /// <see langword="null"/>
        /// if this is a root
        /// </returns>
        /// <seealso cref="GetParentHint(iText.Layout.IPropertyContainer)"/>
        /// <seealso cref="GetAccessibleParentHint(TaggingHintKey)"/>
        public virtual TaggingHintKey GetParentHint(TaggingHintKey hintKey) {
            return parentHints.Get(hintKey);
        }

        /// <summary>Gets the nearest accessible parent hint, skipping non-accessible intermediate nodes.</summary>
        /// <remarks>
        /// Gets the nearest accessible parent hint, skipping non-accessible intermediate nodes.
        /// <para />This method traverses up the hint tree, skipping non-accessible hints (grouping nodes),
        /// and returns the first accessible parent found. Useful when you need to know the logical
        /// parent regardless of grouping structure.
        /// </remarks>
        /// <param name="hintKey">the child hint key</param>
        /// <returns>
        /// the nearest accessible parent
        /// <see cref="TaggingHintKey"/>
        /// , or
        /// <see langword="null"/>
        /// if no accessible parent exists
        /// </returns>
        /// <seealso cref="GetParentHint(TaggingHintKey)"/>
        public virtual TaggingHintKey GetAccessibleParentHint(TaggingHintKey hintKey) {
            do {
                hintKey = GetParentHint(hintKey);
            }
            while (hintKey != null && IsNonAccessibleHint(hintKey));
            return hintKey;
        }

        /// <summary>Incrementally finalizes and releases finished hints from the tagging structure.</summary>
        /// <remarks>
        /// Incrementally finalizes and releases finished hints from the tagging structure.
        /// <para />This method scans all hints and releases those that:
        /// <list type="bullet">
        /// <item><description>Are marked as finished
        /// </description></item>
        /// <item><description>Are accessible (not non-accessible grouping nodes)
        /// </description></item>
        /// <item><description>Have no unfinished parents (up the hierarchy)
        /// </description></item>
        /// <item><description>Have no unfinished children
        /// </description></item>
        /// <item><description>Are not followed by unfinished siblings
        /// </description></item>
        /// </list>
        /// <para />When a hint is released:
        /// <list type="bullet">
        /// <item><description>It is removed from the hint trees
        /// </description></item>
        /// <item><description>The associated PDF tag is finalized
        /// </description></item>
        /// <item><description>If
        /// <c>immediateFlush</c>
        /// is enabled, parent tags are flushed if all kids are flushed
        /// </description></item>
        /// </list>
        /// <para />This is an incremental operation useful for memory management. Call this periodically
        /// (e.g., at end of each page or logical boundary) to progressively finalize tags.
        /// </remarks>
        /// <seealso cref="ReleaseAllHints()"/>
        /// <seealso cref="FinishTaggingHint(iText.Layout.IPropertyContainer)"/>
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

        /// <summary>Forces finalization and release of all hints, clearing the entire tagging structure.</summary>
        /// <remarks>
        /// Forces finalization and release of all hints, clearing the entire tagging structure.
        /// <para />This is a comprehensive cleanup operation that:
        /// <list type="bullet">
        /// <item><description>Finishes all dummy elements (pre-existing tags)
        /// </description></item>
        /// <item><description>Recursively finishes all dummy children
        /// </description></item>
        /// <item><description>Calls
        /// <see cref="ReleaseFinishedHints()"/>
        /// to finalize any now-finished hints
        /// </description></item>
        /// <item><description>Releases all remaining unfinished hints (orphaned hints)
        /// </description></item>
        /// <item><description>Clears all internal maps
        /// </description></item>
        /// </list>
        /// <para />Call this at the end of document layout or when discarding the layout state entirely.
        /// This method should leave all internal structures empty after completion.
        /// </remarks>
        /// <seealso cref="ReleaseFinishedHints()"/>
        /// <seealso cref="FinishTaggingHint(iText.Layout.IPropertyContainer)"/>
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

        /// <summary>Creates or retrieves a PDF tag for a renderer, ensuring it exists in the structure tree.</summary>
        /// <remarks>
        /// Creates or retrieves a PDF tag for a renderer, ensuring it exists in the structure tree.
        /// <para />This method is typically called by a renderer before it writes marked content to ensure the tag
        /// is positioned correctly in the PDF structure tree. If the tag already exists, it is not recreated.
        /// <para />For artifacts, returns
        /// <see langword="false"/>
        /// without creating a tag. For non-accessible hints,
        /// the pointer is positioned at the nearest accessible parent. For accessible hints, a tag is
        /// created with the correct sibling index.
        /// </remarks>
        /// <param name="renderer">the renderer whose tag should be created</param>
        /// <param name="tagPointer">the tag tree pointer to use for positioning; the pointer may be moved during tag creation
        ///     </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if a tag was created,
        /// <see langword="false"/>
        /// if one already existed or
        /// hint is artifact/non-accessible
        /// </returns>
        /// <seealso cref="CreateTag(TaggingHintKey, iText.Kernel.Pdf.Tagutils.TagTreePointer)"/>
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

        /// <summary>Creates a PDF tag for a hint key, ensuring it exists in the structure tree.</summary>
        /// <remarks>
        /// Creates a PDF tag for a hint key, ensuring it exists in the structure tree.
        /// <para />This is the core tag creation method. It:
        /// <list type="bullet">
        /// <item><description>Returns
        /// <see langword="false"/>
        /// if the hint is an artifact
        /// </description></item>
        /// <item><description>Determines the correct parent tag and sibling index
        /// </description></item>
        /// <item><description>Creates the tag via
        /// <c>tagPointer.addTag(...)</c>
        /// </description></item>
        /// <item><description>Stores the pointer on the hint
        /// </description></item>
        /// <item><description>Assigns waiting state to the tag
        /// </description></item>
        /// <item><description>Recursively creates tags for dummy children
        /// </description></item>
        /// </list>
        /// <para />The pointer may be modified during this method to position it at the correct parent and index.
        /// That's why if auto-tagging pointer is to be used, make sure to rely on
        /// <see cref="UseAutoTaggingPointerAndRememberItsPosition(iText.Layout.Renderer.IRenderer)"/>
        /// and
        /// <see cref="RestoreAutoTaggingPointerPosition(iText.Layout.Renderer.IRenderer)"/>
        /// functionality.
        /// </remarks>
        /// <param name="hintKey">the hint key to create a tag for</param>
        /// <param name="tagPointer">the tag tree pointer to use for positioning</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if a tag was created,
        /// <see langword="false"/>
        /// if artifact or already exists
        /// </returns>
        /// <seealso cref="CreateTag(iText.Layout.Renderer.IRenderer, iText.Kernel.Pdf.Tagutils.TagTreePointer)"/>
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

        /// <summary>Marks an element or renderer as logically complete and applies tagging rules.</summary>
        /// <remarks>
        /// Marks an element or renderer as logically complete and applies tagging rules.
        /// <para />Call this method when an element has finished its layout or rendering and will not
        /// receive new children. This triggers:
        /// <list type="bullet">
        /// <item><description>Lookup of applicable
        /// <see cref="ITaggingRule"/>
        /// s for the element's role
        /// </description></item>
        /// <item><description>Invocation of each rule's
        /// <see cref="ITaggingRule.OnTagFinish(LayoutTaggingHelper, TaggingHintKey)"/>
        /// method
        /// </description></item>
        /// <item><description>If all rules return
        /// <see langword="true"/>
        /// , the hint is marked as finished
        /// </description></item>
        /// <item><description>If any rule returns
        /// <see langword="false"/>
        /// , the hint remains unfinished (rules can block finishing)
        /// </description></item>
        /// </list>
        /// <para /><strong>Important:</strong> A hint cannot receive new children or be relocated after finishing.
        /// Always try to finish hints in parent-to-child order (or at least ensure children are finished before parents)
        /// if possible.
        /// <para />For non-accessible hints, rules are bypassed and the hint is marked finished immediately.
        /// For artifacts, this method has no effect.
        /// </remarks>
        /// <param name="hintOwner">the element or renderer to finish</param>
        /// <seealso cref="ITaggingRule"/>
        /// <seealso cref="ReleaseFinishedHints()"/>
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

        /// <summary>Replaces one child hint with multiple new child hints.</summary>
        /// <remarks>
        /// Replaces one child hint with multiple new child hints.
        /// <para />This method is useful when a single renderer needs to expand into multiple child tags.
        /// It removes the old child from its parent and inserts the new children at the same position.
        /// <para />Errors are logged and the operation fails if:
        /// <list type="bullet">
        /// <item><description>The child hint is already finished
        /// </description></item>
        /// <item><description>Any new child is already finished and either has no parent or the parent is already finished too.
        /// </description></item>
        /// </list>
        /// <para />The method returns the index where the replacement occurred, which can be used for
        /// further hint tree manipulation if needed.
        /// </remarks>
        /// <param name="kidHintKey">the child hint to be replaced</param>
        /// <param name="newKidsHintKeys">the new child hints to insert at the replacement position</param>
        /// <returns>
        /// the index where the old child was removed, or
        /// <c>-1</c>
        /// if replacement failed
        /// </returns>
        /// <seealso cref="MoveKidHint(TaggingHintKey, TaggingHintKey)"/>
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

        /// <summary>Moves a child hint from its current parent to a new parent (appended).</summary>
        /// <remarks>
        /// Moves a child hint from its current parent to a new parent (appended).
        /// <para />This method removes a child from its current parent and re-parents it to the new parent,
        /// appending it to the new parent's children list.
        /// <para />For a specific insertion position in the new parent, use
        /// <see cref="MoveKidHint(TaggingHintKey, TaggingHintKey, int)"/>.
        /// </remarks>
        /// <param name="hintKeyOfKidToMove">the child hint to move</param>
        /// <param name="newParent">the new parent hint</param>
        /// <returns>
        /// the index where the child was removed from the old parent, or
        /// <c>-1</c>
        /// if move failed
        /// </returns>
        /// <seealso cref="MoveKidHint(TaggingHintKey, TaggingHintKey, int)"/>
        /// <seealso cref="ReplaceKidHint(TaggingHintKey, System.Collections.Generic.ICollection{E})"/>
        public virtual int MoveKidHint(TaggingHintKey hintKeyOfKidToMove, TaggingHintKey newParent) {
            return MoveKidHint(hintKeyOfKidToMove, newParent, -1);
        }

        /// <summary>Moves a child hint from its current parent to a new parent at a specific position.</summary>
        /// <remarks>
        /// Moves a child hint from its current parent to a new parent at a specific position.
        /// <para />This method is similar to
        /// <see cref="MoveKidHint(TaggingHintKey, TaggingHintKey)"/>
        /// but allows
        /// specifying the insertion index in the new parent's children list. Negative index means append.
        /// <para />Errors are logged if:
        /// <list type="bullet">
        /// <item><description>The new parent is already finished
        /// </description></item>
        /// <item><description>The child hint is already finished
        /// </description></item>
        /// </list>
        /// </remarks>
        /// <param name="hintKeyOfKidToMove">the child hint to move</param>
        /// <param name="newParent">the new parent hint</param>
        /// <param name="insertIndex">the position at which to insert the child; negative means append</param>
        /// <returns>
        /// the index where the child was removed from the old parent, or
        /// <c>-1</c>
        /// if move failed
        /// </returns>
        /// <seealso cref="MoveKidHint(TaggingHintKey, TaggingHintKey)"/>
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

        /// <summary>Created a unique id for a structureElement.</summary>
        /// <param name="prefix">a prefix to prepend to the id</param>
        /// <returns>a unique id</returns>
        public virtual String CreateStructureElementId(String prefix) {
            lastId++;
            return prefix + lastId;
        }

        /// <summary>Gets the PDF document associated with this helper.</summary>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// passed to the constructor
        /// </returns>
        public virtual PdfDocument GetPdfDocument() {
            return document;
        }

        /// <summary>Internal implementation of hint key creation/retrieval.</summary>
        /// <remarks>
        /// Internal implementation of hint key creation/retrieval.
        /// <para />This method implements the core logic for obtaining or creating hint keys:
        /// <list type="bullet">
        /// <item><description>Checks for existing hint on the container
        /// </description></item>
        /// <item><description>If not found, wraps the container's accessible element
        /// </description></item>
        /// <item><description>Automatically marks as artifact if role is ARTIFACT
        /// </description></item>
        /// <item><description>Optionally stores the hint on the container
        /// </description></item>
        /// </list>
        /// </remarks>
        /// <param name="hintOwner">the element or renderer</param>
        /// <param name="setProperty">
        /// if
        /// <see langword="true"/>
        /// , stores the hint in the container's properties
        /// </param>
        /// <returns>the existing or newly created hint key</returns>
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
                        ind = GetNearestNextSiblingIndex(waitingTagsManager, tagPointer, parentHint, hintKey);
                    }
                }
                AccessibilityProperties props = modelElement.GetAccessibilityProperties();
                if (hintKey.GetOverriddenRole() != null) {
                    props = new DefaultAccessibilityProperties(props).SetRole(hintKey.GetOverriddenRole());
                }
                tagPointer.AddTag(ind, props);
                hintKey.SetTagPointer(new TagTreePointer(tagPointer));
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
            int ind = GetNearestNextSiblingIndex(waitingTagsManager, parentPointer, parentKey, kidKey);
            parentPointer.SetNextNewKidIndex(ind);
            kidPointer.Relocate(parentPointer);
        }

        private static bool IsNonAccessibleHint(TaggingHintKey hintKey) {
            return !hintKey.IsAccessible();
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
            RegisterSingleRule(StandardRoles.TH, new THTaggingRule());
            if (pdfVersion.CompareTo(PdfVersion.PDF_1_5) < 0) {
                TableTaggingPriorToOneFiveVersionRule priorToOneFiveRule = new TableTaggingPriorToOneFiveVersionRule();
                RegisterSingleRule(StandardRoles.TABLE, priorToOneFiveRule);
                RegisterSingleRule(StandardRoles.THEAD, priorToOneFiveRule);
                RegisterSingleRule(StandardRoles.TFOOT, priorToOneFiveRule);
            }
            FootnoteTaggingRule footnoteRule = new FootnoteTaggingRule();
            RegisterSingleRule(StandardRoles.LBL, footnoteRule);
            RegisterSingleRule(StandardRoles.REFERENCE, footnoteRule);
        }

        private void RegisterSingleRule(String role, ITaggingRule rule) {
            IList<ITaggingRule> rules = taggingRules.Get(role);
            if (rules == null) {
                rules = new List<ITaggingRule>();
                taggingRules.Put(role, rules);
            }
            rules.Add(rule);
        }

        private int GetNearestNextSiblingIndex(WaitingTagsManager waitingTagsManager, TagTreePointer parentPointer
            , TaggingHintKey parentKey, TaggingHintKey kidKey) {
            LayoutTaggingHelper.ScanContext scanContext = new LayoutTaggingHelper.ScanContext();
            scanContext.waitingTagsManager = waitingTagsManager;
            scanContext.startHintKey = kidKey;
            scanContext.parentPointer = parentPointer;
            scanContext.nextSiblingPointer = new TagTreePointer(document);
            return ScanForNearestNextSiblingIndex(scanContext, null, parentKey);
        }

        private int ScanForNearestNextSiblingIndex(LayoutTaggingHelper.ScanContext scanContext, TaggingHintKey toCheck
            , TaggingHintKey parent) {
            if (scanContext.startVerifying) {
                if (scanContext.waitingTagsManager.TryMovePointerToWaitingTag(scanContext.nextSiblingPointer, toCheck) && 
                    scanContext.parentPointer.IsPointingToSameTag(new TagTreePointer(scanContext.nextSiblingPointer).MoveToParent
                    ())) {
                    return scanContext.nextSiblingPointer.GetIndexInParentKidsList();
                }
            }
            if (toCheck != null && !IsNonAccessibleHint(toCheck)) {
                return -1;
            }
            IList<TaggingHintKey> kidsHintList = kidsHints.Get(parent);
            if (kidsHintList == null) {
                return -1;
            }
            int startIndex = -1;
            if (!scanContext.startVerifying) {
                for (int i = kidsHintList.Count - 1; i >= 0; i--) {
                    if (scanContext.startHintKey == kidsHintList[i]) {
                        scanContext.startVerifying = true;
                        startIndex = i;
                        break;
                    }
                }
            }
            for (int j = startIndex + 1; j < kidsHintList.Count; j++) {
                TaggingHintKey kid = kidsHintList[j];
                int interMediateResult = ScanForNearestNextSiblingIndex(scanContext, kid, kid);
                if (interMediateResult != -1) {
                    return interMediateResult;
                }
            }
            return -1;
        }

        private class ScanContext {
//\cond DO_NOT_DOCUMENT
            internal WaitingTagsManager waitingTagsManager;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal TaggingHintKey startHintKey;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal bool startVerifying;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal TagTreePointer parentPointer;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal TagTreePointer nextSiblingPointer;
//\endcond
        }
    }
}
