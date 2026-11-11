/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using System.Collections.Generic;
using iText.StyledXmlParser.Jsoup.Helper;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>A HTML Form Element provides ready access to the form fields/controls that are associated with it.
    ///     </summary>
    /// <remarks>
    /// A HTML Form Element provides ready access to the form fields/controls that are associated with it. It also allows a
    /// form to easily be submitted.
    /// </remarks>
    public class FormElement : iText.StyledXmlParser.Jsoup.Nodes.Element {
        private readonly iText.StyledXmlParser.Jsoup.Select.Elements elements = new iText.StyledXmlParser.Jsoup.Select.Elements
            ();

        /// <summary>Create a new, standalone form element.</summary>
        /// <param name="tag">tag of this element</param>
        /// <param name="baseUri">the base URI</param>
        /// <param name="attributes">initial attributes</param>
        public FormElement(iText.StyledXmlParser.Jsoup.Parser.Tag tag, String baseUri, Attributes attributes)
            : base(tag, baseUri, attributes) {
        }

        /// <summary>Get the list of form control elements associated with this form.</summary>
        /// <returns>form controls associated with this element.</returns>
        public virtual iText.StyledXmlParser.Jsoup.Select.Elements Elements() {
            return elements;
        }

        /// <summary>Add a form control element to this form.</summary>
        /// <param name="element">form control to add</param>
        /// <returns>this form element, for chaining</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.FormElement AddElement(iText.StyledXmlParser.Jsoup.Nodes.Element
             element) {
            elements.Add(element);
            return this;
        }

        protected internal override void RemoveChild(iText.StyledXmlParser.Jsoup.Nodes.Node @out) {
            base.RemoveChild(@out);
            elements.Remove((iText.StyledXmlParser.Jsoup.Nodes.Element)@out);
        }

        /// <summary>Get the data that this form submits.</summary>
        /// <remarks>
        /// Get the data that this form submits. The returned list is a copy of the data, and changes to the contents of the
        /// list will not be reflected in the DOM.
        /// </remarks>
        /// <returns>a list of key vals</returns>
        public virtual IList<KeyVal> FormData() {
            List<KeyVal> data = new List<KeyVal>();
            // iterate the form control elements and accumulate their values
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Element el in elements) {
                if (!el.Tag().IsFormSubmittable()) {
                    continue;
                }
                // contents are form listable, superset of submitable
                if (el.HasAttr("disabled")) {
                    continue;
                }
                // skip disabled form inputs
                String name = el.Attr("name");
                if (name.Length == 0) {
                    continue;
                }
                String type = el.Attr("type");
                if (type.EqualsIgnoreCase("button")) {
                    continue;
                }
                // browsers don't submit these
                if ("select".Equals(el.NormalName())) {
                    iText.StyledXmlParser.Jsoup.Select.Elements options = el.Select("option[selected]");
                    bool set = false;
                    foreach (iText.StyledXmlParser.Jsoup.Nodes.Element option in options) {
                        data.Add(KeyVal.Create(name, option.Val()));
                        set = true;
                    }
                    if (!set) {
                        iText.StyledXmlParser.Jsoup.Nodes.Element option = el.SelectFirst("option");
                        if (option != null) {
                            data.Add(KeyVal.Create(name, option.Val()));
                        }
                    }
                }
                else {
                    if ("checkbox".EqualsIgnoreCase(type) || "radio".EqualsIgnoreCase(type)) {
                        // only add checkbox or radio if they have the checked attribute
                        if (el.HasAttr("checked")) {
                            String val = el.Val().Length > 0 ? el.Val() : "on";
                            data.Add(KeyVal.Create(name, val));
                        }
                    }
                    else {
                        data.Add(KeyVal.Create(name, el.Val()));
                    }
                }
            }
            return data;
        }

        public override Object Clone() {
            return (iText.StyledXmlParser.Jsoup.Nodes.FormElement)base.Clone();
        }
    }
}
