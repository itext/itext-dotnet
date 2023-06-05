using System;
using System.Collections.Generic;
using iText.Layout.Renderer;

namespace iText.Layout.Element {
    /// <summary>represents a container of the column objects.</summary>
    public class ColumnContainer : Div {
        /// <summary>
        /// Creates new
        /// <see cref="ColumnContainer"/>
        /// instance.
        /// </summary>
        public ColumnContainer()
            : base() {
        }

        /// <summary>
        /// Copies all properties of
        /// <see cref="ColumnContainer"/>
        /// to its child elements.
        /// </summary>
        public virtual void CopyAllPropertiesToChildren() {
            foreach (IElement child in this.GetChildren()) {
                foreach (KeyValuePair<int, Object> entry in this.properties) {
                    child.SetProperty(entry.Key, entry.Value);
                }
            }
        }

        protected internal override IRenderer MakeNewRenderer() {
            return new ColumnContainerRenderer(this);
        }
    }
}
