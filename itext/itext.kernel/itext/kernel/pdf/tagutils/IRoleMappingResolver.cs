using System;
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
    /// <summary>
    /// A helper interface that facilitates roles mapping resolving for the tag structures that are defined for different
    /// PDF document specification versions.
    /// </summary>
    /// <remarks>
    /// A helper interface that facilitates roles mapping resolving for the tag structures that are defined for different
    /// PDF document specification versions.
    /// <p>Be aware, that it is explicitly allowed for the document by the specification to have circular or transitive mappings.</p>
    /// </remarks>
    public interface IRoleMappingResolver {
        /// <summary>Defines the current role of the resolver.</summary>
        /// <remarks>
        /// Defines the current role of the resolver. On every successful resolving "step" the role returned by this method
        /// changes in order to reflect the mapping of the previous role.
        /// </remarks>
        /// <returns>
        /// the
        /// <see cref="System.String"/>
        /// which identifies current role of the resolver.
        /// </returns>
        String GetRole();

        /// <summary>Defines the namespace of the current role.</summary>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfNamespace"/>
        /// instance of the namespace dictionary wrapper. The role returned by the
        /// <see cref="GetRole()"/>
        /// method call is considered to belong to this namespace. Might be null, which means that role belongs to the
        /// default standard namespace.
        /// </returns>
        PdfNamespace GetNamespace();

        /// <summary>Checks if the current role belongs to one of the standard structure namespaces.</summary>
        /// <returns>true if the current namespace is a standard structure namespace and the current role is defined as standard role in it.
        ///     </returns>
        bool CurrentRoleIsStandard();

        /// <summary>
        /// Checks if the current role and namespace are specified to be obligatory mapped to the standard structure namespace
        /// in order to be a valid role in the Tagged PDF.
        /// </summary>
        /// <returns>
        /// true, if the current role in the current namespace either belongs to the standard structure roles or is in the
        /// domain specific namespace; otherwise false.
        /// </returns>
        bool CurrentRoleShallBeMappedToStandard();

        /// <summary>Performs a mapping resolving "step".</summary>
        /// <remarks>
        /// Performs a mapping resolving "step". Essentially finds the role and it's namespace to which the current role is mapped to.
        /// After this method call
        /// <see cref="GetRole()"/>
        /// and
        /// <see cref="GetNamespace()"/>
        /// methods might change their return value.
        /// </remarks>
        /// <returns>
        /// true if current role and/or namespace have changed their values; otherwise false which means that current
        /// role is not mapped.
        /// </returns>
        bool ResolveNextMapping();
    }
}
