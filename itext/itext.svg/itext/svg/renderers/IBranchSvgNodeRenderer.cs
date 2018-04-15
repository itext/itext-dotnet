/*
* To change this license header, choose License Headers in Project Properties.
* To change this template file, choose Tools | Templates
* and open the template in the editor.
*/
using System.Collections.Generic;

namespace iText.Svg.Renderers {
    /// <summary>Interface that defines branches in the NodeRenderer structure.</summary>
    /// <remarks>
    /// Interface that defines branches in the NodeRenderer structure. Differs from a leaf renderer
    /// in that a branch has children and as such methods that can add or retrieve those children.
    /// </remarks>
    public interface IBranchSvgNodeRenderer : ISvgNodeRenderer {
        /// <summary>Adds a renderer as the last element of the list of children.</summary>
        /// <param name="child">any renderer</param>
        void AddChild(ISvgNodeRenderer child);

        /// <summary>Gets all child renderers of this object.</summary>
        /// <returns>the list of child renderers (in the order that they were added)</returns>
        IList<ISvgNodeRenderer> GetChildren();
    }
}
