//Copyright (c) 2006, Adobe Systems Incorporated
//All rights reserved.
//
//        Redistribution and use in source and binary forms, with or without
//        modification, are permitted provided that the following conditions are met:
//        1. Redistributions of source code must retain the above copyright
//        notice, this list of conditions and the following disclaimer.
//        2. Redistributions in binary form must reproduce the above copyright
//        notice, this list of conditions and the following disclaimer in the
//        documentation and/or other materials provided with the distribution.
//        3. All advertising materials mentioning features or use of this software
//        must display the following acknowledgement:
//        This product includes software developed by the Adobe Systems Incorporated.
//        4. Neither the name of the Adobe Systems Incorporated nor the
//        names of its contributors may be used to endorse or promote products
//        derived from this software without specific prior written permission.
//
//        THIS SOFTWARE IS PROVIDED BY ADOBE SYSTEMS INCORPORATED ''AS IS'' AND ANY
//        EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//        WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//        DISCLAIMED. IN NO EVENT SHALL ADOBE SYSTEMS INCORPORATED BE LIABLE FOR ANY
//        DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//        (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//        LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//        ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//        (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//        SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//        http://www.adobe.com/devnet/xmp/library/eula-xmp-library-java.html
using System;
using System.Collections;
using iText.Kernel.XMP.Properties;

namespace iText.Kernel.XMP {
    /// <summary>
    /// The schema registry keeps track of all namespaces and aliases used in the XMP
    /// metadata.
    /// </summary>
    /// <remarks>
    /// The schema registry keeps track of all namespaces and aliases used in the XMP
    /// metadata. At initialisation time, the default namespaces and default aliases
    /// are automatically registered. <b>Namespaces</b> must be registered before
    /// used in namespace URI parameters or path expressions. Within the XMP Toolkit
    /// the registered namespace URIs and prefixes must be unique. Additional
    /// namespaces encountered when parsing RDF are automatically registered. The
    /// namespace URI should always end in an XML name separator such as '/' or '#'.
    /// This is because some forms of RDF shorthand catenate a namespace URI with an
    /// element name to form a new URI.
    /// <para />
    /// <b>Aliases</b> in XMP serve the same purpose as Windows file shortcuts,
    /// Macintosh file aliases, or UNIX file symbolic links. The aliases are simply
    /// multiple names for the same property. One distinction of XMP aliases is that
    /// they are ordered, there is an alias name pointing to an actual name. The
    /// primary significance of the actual name is that it is the preferred name for
    /// output, generally the most widely recognized name.
    /// <para />
    /// The names that can be aliased in XMP are restricted. The alias must be a top
    /// level property name, not a field within a structure or an element within an
    /// array. The actual may be a top level property name, the first element within
    /// a top level array, or the default element in an alt-text array. This does not
    /// mean the alias can only be a simple property. It is OK to alias a top level
    /// structure or array to an identical top level structure or array, or to the
    /// first item of an array of structures.
    /// </remarks>
    /// <since>27.01.2006</since>
    public interface XMPSchemaRegistry {
        // ---------------------------------------------------------------------------------------------
        // Namespace Functions
        /// <summary>Register a namespace URI with a suggested prefix.</summary>
        /// <remarks>
        /// Register a namespace URI with a suggested prefix. It is not an error if
        /// the URI is already registered, no matter what the prefix is. If the URI
        /// is not registered but the suggested prefix is in use, a unique prefix is
        /// created from the suggested one. The actual registeed prefix is always
        /// returned. The function result tells if the registered prefix is the
        /// suggested one.
        /// <para />
        /// Note: No checking is presently done on either the URI or the prefix.
        /// </remarks>
        /// <param name="namespaceURI">The URI for the namespace. Must be a valid XML URI.</param>
        /// <param name="suggestedPrefix">
        /// The suggested prefix to be used if the URI is not yet
        /// registered. Must be a valid XML name.
        /// </param>
        /// <returns>
        /// Returns the registered prefix for this URI, is equal to the
        /// suggestedPrefix if the namespace hasn't been registered before,
        /// otherwise the existing prefix.
        /// </returns>
        String RegisterNamespace(String namespaceURI, String suggestedPrefix);

        /// <summary>Obtain the prefix for a registered namespace URI.</summary>
        /// <remarks>
        /// Obtain the prefix for a registered namespace URI.
        /// <para />
        /// It is not an error if the namespace URI is not registered.
        /// </remarks>
        /// <param name="namespaceURI">
        /// The URI for the namespace. Must not be null or the empty
        /// string.
        /// </param>
        /// <returns>Returns the prefix registered for this namespace URI or null.</returns>
        String GetNamespacePrefix(String namespaceURI);

        /// <summary>Obtain the URI for a registered namespace prefix.</summary>
        /// <remarks>
        /// Obtain the URI for a registered namespace prefix.
        /// <para />
        /// It is not an error if the namespace prefix is not registered.
        /// </remarks>
        /// <param name="namespacePrefix">
        /// The prefix for the namespace. Must not be null or the empty
        /// string.
        /// </param>
        /// <returns>Returns the URI registered for this prefix or null.</returns>
        String GetNamespaceURI(String namespacePrefix);

        /// <returns>
        /// Returns the registered prefix/namespace-pairs as map, where the keys are the
        /// namespaces and the values are the prefixes.
        /// </returns>
        IDictionary GetNamespaces();

        /// <returns>
        /// Returns the registered namespace/prefix-pairs as map, where the keys are the
        /// prefixes and the values are the namespaces.
        /// </returns>
        IDictionary GetPrefixes();

        /// <summary>Deletes a namespace from the registry.</summary>
        /// <remarks>
        /// Deletes a namespace from the registry.
        /// <para />
        /// Does nothing if the URI is not registered, or if the namespaceURI
        /// parameter is null or the empty string.
        /// <para />
        /// Note: Not yet implemented.
        /// </remarks>
        /// <param name="namespaceURI">The URI for the namespace.</param>
        void DeleteNamespace(String namespaceURI);

        // ---------------------------------------------------------------------------------------------
        // Alias Functions
        /// <summary>Determines if a name is an alias, and what it is aliased to.</summary>
        /// <param name="aliasNS">
        /// The namespace URI of the alias. Must not be <c>null</c> or the empty
        /// string.
        /// </param>
        /// <param name="aliasProp">
        /// The name of the alias. May be an arbitrary path expression
        /// path, must not be <c>null</c> or the empty string.
        /// </param>
        /// <returns>
        /// Returns the <c>XMPAliasInfo</c> for the given alias namespace and property or
        /// <c>null</c> if there is no such alias.
        /// </returns>
        XMPAliasInfo ResolveAlias(String aliasNS, String aliasProp);

        /// <summary>Collects all aliases that are contained in the provided namespace.</summary>
        /// <remarks>
        /// Collects all aliases that are contained in the provided namespace.
        /// If nothing is found, an empty array is returned.
        /// </remarks>
        /// <param name="aliasNS">a schema namespace URI</param>
        /// <returns>Returns all alias infos from aliases that are contained in the provided namespace.</returns>
        XMPAliasInfo[] FindAliases(String aliasNS);

        /// <summary>Searches for registered aliases.</summary>
        /// <param name="qname">an XML conform qname</param>
        /// <returns>
        /// Returns if an alias definition for the given qname to another
        /// schema and property is registered.
        /// </returns>
        XMPAliasInfo FindAlias(String qname);

        /// <returns>
        /// Returns the registered aliases as map, where the key is the "qname" (prefix and name)
        /// and the value an <c>XMPAliasInfo</c>-object.
        /// </returns>
        IDictionary GetAliases();
    }
}
