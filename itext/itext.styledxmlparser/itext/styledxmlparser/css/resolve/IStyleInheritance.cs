using System;

namespace iText.StyledXmlParser.Css.Resolve {
    /// <summary>Interface for attribute and style-inheritance logic</summary>
    public interface IStyleInheritance {
        /// <summary>Checks if a property or attribute is inheritable is inheritable.</summary>
        /// <param name="propertyIdentifier">the identifier for property</param>
        /// <returns>true, if the property is inheritable, false otherwise</returns>
        bool IsInheritable(String propertyIdentifier);
    }
}
