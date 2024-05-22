namespace iText.Signatures {
    /// <summary>Access permissions value to be set to certification signature as a part of DocMDP configuration.</summary>
    public enum AccessPermissions {
        /// <summary>Unspecified access permissions value which makes signature "approval" rather than "certification".
        ///     </summary>
        UNSPECIFIED,
        /// <summary>Access permissions level 1 which indicates that no changes are permitted except for DSS and DTS creation.
        ///     </summary>
        NO_CHANGES_PERMITTED,
        /// <summary>
        /// Access permissions level 2 which indicates that permitted changes, with addition to level 1, are:
        /// filling in forms, instantiating page templates, and signing.
        /// </summary>
        FORM_FIELDS_MODIFICATION,
        /// <summary>
        /// Access permissions level 3 which indicates that permitted changes, with addition to level 2, are:
        /// annotation creation, deletion and modification.
        /// </summary>
        ANNOTATION_MODIFICATION
    }
}
