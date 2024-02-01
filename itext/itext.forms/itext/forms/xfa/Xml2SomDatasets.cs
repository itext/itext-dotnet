/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2024 Apryse Group NV
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
using System.Linq;
using System.Xml.Linq;

namespace iText.Forms.Xfa
{
	/// <summary>Processes the datasets section in the XFA form.</summary>
	internal class Xml2SomDatasets : Xml2Som
	{
		/// <summary>Creates a new instance from the datasets node.</summary>
		/// <remarks>
		/// Creates a new instance from the datasets node. This expects
		/// not the datasets but the data node that comes below.
		/// </remarks>
		/// <param name="n">the datasets node</param>
		public Xml2SomDatasets(XNode n)
		{
			order = new List<String>();
			name2Node = new Dictionary<String, XNode>();
			stack = new Stack<String>();
			anform = 0;
			inverseSearch = new Dictionary<String, InverseStore>();
			ProcessDatasetsInternal(n);
		}

		/// <summary>Inserts a new <CODE>Node</CODE> that will match the short name.</summary>
		/// <param name="n">the datasets top <CODE>Node</CODE></param>
		/// <param name="shortName">the short name</param>
		/// <returns>the new <CODE>Node</CODE> of the inserted name</returns>
		public virtual XNode InsertNode(XNode n, String shortName)
		{
			Stack<String> localStack = SplitParts(shortName);
			XDocument doc = n.Document;
			XNode n2 = null;
		    n = ((XElement) n).FirstNode;
			while (n is XElement) {
			    n = ((XElement) n).NextNode;
			}
			for (int k = 0; k < localStack.Count; ++k)
			{
				String part = localStack.ElementAt(k);
				int idx = part.LastIndexOf('[');
				String name = part.JSubstring(0, idx);
				idx = System.Convert.ToInt32(part.JSubstring(idx + 1, part.Length - 1));
				int found = -1;
				for (n2 = ((XElement)n).FirstNode; n2 != null; n2 = ((XNode)n2).NextNode)
				{
					if (n2 is XElement)
					{
						String s = EscapeSom(((XElement)n2).Name.LocalName);
						if (s.Equals(name))
						{
							++found;
							if (found == idx)
							{
								break;
							}
						}
					}
				}
				for (; found < idx; ++found) {
				    n2 = new XElement(name);
                    ((XElement) n).Add(n2);
				    ((XElement) n2).SetAttributeValue((XNamespace) XfaForm.XFA_DATA_SCHEMA + "dataNode", "dataGroup");
				}
				n = n2;
			}
			InverseSearchAdd(inverseSearch, localStack, shortName);
			name2Node[shortName] = n2;
			order.Add(shortName);
			return n2;
		}

		private static bool HasChildren(XNode n)
		{
			XAttribute dataNodeN = ((XElement)n).Attribute((XNamespace)XfaForm.XFA_DATA_SCHEMA + "dataNode");
			if (dataNodeN != null)
			{
				String dataNode = dataNodeN.Value;
				if ("dataGroup".Equals(dataNode))
				{
					return true;
				}
				else
				{
					if ("dataValue".Equals(dataNode))
					{
						return false;
					}
				}
			}
			if (!((XElement)n).Nodes().Any())
			{
				return false;
			}
		    XNode n2 = ((XElement) n).FirstNode;
			while (n2 != null)
			{
				if (n2 is XElement)
				{
					return true;
				}
			    n2 = n2.NextNode;
			}
			return false;
		}

		private void ProcessDatasetsInternal(XNode n)
		{
			if (n != null)
			{
				IDictionary<String, int?> ss = new Dictionary<String, int?>();
				XNode n2 = n is XElement ? ((XElement)n).FirstNode : null;
				while (n2 != null)
				{
					if (n2 is XElement) {
					    String s = EscapeSom(((XElement) n2).Name.LocalName);
						int? i = ss.Get(s);
						if (i == null)
                            i = 0;
                        else
                            i = i + 1;
                        ss[s] = i;
                        stack.Push(string.Format("{0}[{1}]", s, i));
                        if (HasChildren(n2))
                        {
                            ProcessDatasetsInternal(n2);
                        }
                        String unstack = PrintStack();
                        order.Add(unstack);
                        InverseSearchAdd(unstack);
                        name2Node[unstack] = n2;
                        stack.Pop();

					}
				    n2 = n2 is XElement ? ((XElement) n2).NextNode : (n2 is XText ? ((XText)n2).NextNode : null);
				}
			}
		}
	}
}
