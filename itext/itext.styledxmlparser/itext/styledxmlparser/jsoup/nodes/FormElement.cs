/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using iText.StyledXmlParser.Jsoup.Helper;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>A HTML Form Element provides ready access to the form fields/controls that are associated with it.
    ///     </summary>
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

        //    /**
        //     * Prepare to submit this form. A Connection object is created with the request set up from the form values. You
        //     * can then set up other options (like user-agent, timeout, cookies), then execute it.
        //     * @return a connection prepared from the values of this form.
        //     * @throws IllegalArgumentException if the form's absolute action URL cannot be determined. Make sure you pass the
        //     * document's base URI when parsing.
        //     */
        //    public Connection submit() {
        //        String action = hasAttr("action") ? absUrl("action") : baseUri();
        //        Validate.notEmpty(action, "Could not determine a form action URL for submit. Ensure you set a base URI when parsing.");
        //        Connection.Method method = attr("method").toUpperCase().equals("POST") ?
        //                Connection.Method.POST : Connection.Method.GET;
        //
        //        return Jsoup.connect(action)
        //                .data(formData())
        //                .method(method);
        //    }
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
                if ("select".Equals(el.TagName())) {
                    iText.StyledXmlParser.Jsoup.Select.Elements options = el.Select("option[selected]");
                    bool set = false;
                    foreach (iText.StyledXmlParser.Jsoup.Nodes.Element option in options) {
                        data.Add(KeyVal.Create(name, option.Val()));
                        set = true;
                    }
                    if (!set) {
                        iText.StyledXmlParser.Jsoup.Nodes.Element option = el.Select("option").First();
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
    }
}
