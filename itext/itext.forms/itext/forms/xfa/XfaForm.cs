/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2023 Apryse Group NV
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
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;

namespace iText.Forms.Xfa
{
	
	/// <summary>Processes XFA forms.</summary>
	public class XfaForm
	{
		private const int INIT_SERIALIZER_BUFFER_SIZE = 16 * 1024;

		private XElement templateNode;

		private Xml2SomDatasets datasetsSom;

		private XElement datasetsNode;

		private AcroFieldsSearch acroFieldsSom;

		private bool xfaPresent = false;

		private XDocument domDocument;

		/// <summary>The URI for the XFA Data schema.</summary>
		public const String XFA_DATA_SCHEMA = "http://www.xfa.org/schema/xfa-data/1.0/";

		/// <summary>An empty constructor to build on.</summary>
		public XfaForm()
			: this(new MemoryStream(Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?><xdp:xdp xmlns:xdp=\"http://ns.adobe.com/xdp/\"><template xmlns=\"http://www.xfa.org/schema/xfa-template/3.3/\"></template><xfa:datasets xmlns:xfa=\"http://www.xfa.org/schema/xfa-data/1.0/\"><xfa:data></xfa:data></xfa:datasets></xdp:xdp>")))
		{
		}

        /// <summary>Creates an XFA form by the stream containing all xml information</summary>
        /// <param name="inputStream">
        /// The InputStream
        /// </param>

        public XfaForm(Stream inputStream)
		{
			try
			{
				InitXfaForm(inputStream);
			}
			catch (Exception e)
			{
				throw new PdfException(e.Message, e);
			}
		}

        /// <summary>
        /// Creates an XFA form by the
        /// <see cref="Document"/>
        /// containing all xml information
        /// </summary>
        /// <param name="domDocument">
        /// The document
        /// </param>
        public XfaForm(XDocument domDocument)
		{
			SetDomDocument(domDocument);
		}

		/// <summary>
		/// A constructor from a
		/// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
		/// . It is assumed, but not
		/// necessary for correct initialization, that the dictionary is actually a
		/// <see cref="iText.Forms.PdfAcroForm"/>
		/// . An entry in the dictionary with the <code>XFA</code>
		/// key must contain correct XFA syntax. If the <code>XFA</code> key is
		/// absent, then the constructor essentially does nothing.
		/// </summary>
		/// <param name="acroFormDictionary">the dictionary object to initialize from</param>
		public XfaForm(PdfDictionary acroFormDictionary)
		{
			PdfObject xfa = acroFormDictionary.Get(PdfName.XFA);
			if (xfa != null)
			{
				try
				{
					InitXfaForm(xfa);
				}
				catch (Exception e)
				{
					throw new PdfException(e.Message, e);
				}
			}
		}

		/// <summary>A constructor from a <CODE>PdfDocument</CODE>.</summary>
		/// <remarks>
		/// A constructor from a <CODE>PdfDocument</CODE>. It basically does everything
		/// from finding the XFA stream to the XML parsing.
		/// </remarks>
		/// <param name="pdfDocument">the PdfDocument instance</param>
		public XfaForm(PdfDocument pdfDocument)
		{
			PdfObject xfa = GetXfaObject(pdfDocument);
			if (xfa != null)
			{
				try
				{
					InitXfaForm(xfa);
				}
				catch (Exception e)
				{
					throw new PdfException(e.Message, e);
				}
			}
		}

	    /// <summary>Sets the XFA key from a byte array.</summary>
	    /// <remarks>Sets the XFA key from a byte array. The old XFA is erased.</remarks>
	    /// <param name="form">the data</param>
	    /// <param name="pdfDocument">pdfDocument</param>
	    public static void SetXfaForm(XfaForm form, PdfDocument pdfDocument) {
	        PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(pdfDocument, true);
	        SetXfaForm(form, acroForm);
	    }

	    /// <summary>Sets the XFA key from a byte array.</summary>
		/// <remarks>Sets the XFA key from a byte array. The old XFA is erased.</remarks>
		/// <param name="form">the data</param>
		/// <param name="acroForm">an AcroForm instance</param>
		public static void SetXfaForm(XfaForm form, PdfAcroForm acroForm)
		{
		    if (form == null || acroForm == null || acroForm.GetPdfDocument() == null) {
		        throw new ArgumentException("XfaForm, PdfAcroForm and PdfAcroForm's document shall not be null");
		    }
		    PdfDocument document = acroForm.GetPdfDocument();
		    PdfObject xfa = GetXfaObject(acroForm);
			if (xfa != null && xfa.IsArray())
			{
				PdfArray ar = (PdfArray)xfa;
				int t = -1;
				int d = -1;
				for (int k = 0; k < ar.Size(); k += 2)
				{
					PdfString s = ar.GetAsString(k);
					if ("template".Equals(s.ToString()))
					{
						t = k + 1;
					}
					if ("datasets".Equals(s.ToString()))
					{
						d = k + 1;
					}
				}
				if (t > -1 && d > -1)
				{
					//reader.killXref(ar.getAsIndirectObject(t));
					//reader.killXref(ar.getAsIndirectObject(d));
					PdfStream tStream = new PdfStream(SerializeDocument(form.templateNode));
					tStream.SetCompressionLevel(document.GetWriter().GetCompressionLevel());
					ar.Set(t, tStream);
					PdfStream dStream = new PdfStream(SerializeDocument(form.datasetsNode));
					dStream.SetCompressionLevel(document.GetWriter().GetCompressionLevel());
					ar.Set(d, dStream);
				    ar.SetModified();
				    ar.Flush();
				    acroForm.Put(PdfName.XFA, new PdfArray(ar));
				    acroForm.SetModified();
				    if (!acroForm.GetPdfObject().IsIndirect()) {
				        document.GetCatalog().SetModified();
				    }
				    return;
				}
			}
			//reader.killXref(af.get(PdfName.XFA));
			PdfStream stream = new PdfStream(SerializeDocument(form.domDocument));
			stream.SetCompressionLevel(document.GetWriter().GetCompressionLevel());
			stream.Flush();
		    acroForm.Put(PdfName.XFA, stream);
		    acroForm.SetModified();
		    if (!acroForm.GetPdfObject().IsIndirect()) {
		        document.GetCatalog().SetModified();
		    }
		}

		/// <summary>Extracts DOM nodes from an XFA document.</summary>
		/// <param name="domDocument">
		/// an XFA file as a
		/// <see cref="System.Xml.Linq.XDocument">
		/// DOM
		/// document
		/// </see>
		/// </param>
		/// <returns>
		/// a
		/// <see cref="System.Collections.IDictionary{K, V}"/>
		/// of XFA packet names and their associated
		/// <see cref="System.Xml.Linq.XNode">DOM nodes</see>
		/// </returns>
		public static IDictionary<String, XNode> ExtractXFANodes(XDocument domDocument)
		{
			IDictionary<String, XNode> xfaNodes = new Dictionary<String, XNode>();
		    XNode n = domDocument.FirstNode;
			while (!(n is XElement) || !((XElement)n).Nodes().Any())
			{
				n = n.NextNode;
			}
			n = ((XElement)n).FirstNode;
			while (n != null)
			{
				if (n is XElement)
				{
					String s = ((XElement)n).Name.LocalName;
					xfaNodes[s] = n;
				}
				n = n.NextNode;
			}
			return xfaNodes;
		}

		/// <summary>Write the XfaForm to the provided PdfDocument.</summary>
		/// <param name="document">the PdfDocument to write the XFA Form to</param>
		public virtual void Write(PdfDocument document)
		{
			SetXfaForm(this, document);
		}

	    /// <summary>Write the XfaForm to the provided PdfDocument.</summary>
	    /// <param name="acroForm">the PdfAcroForm to write the XFA Form to</param>
	    public virtual void Write(PdfAcroForm acroForm)
	    {
	        SetXfaForm(this, acroForm);
	    }

		/// <summary>Changes a field value in the XFA form.</summary>
		/// <param name="name">the name of the field to be changed</param>
		/// <param name="value">the new value</param>
		public virtual void SetXfaFieldValue(String name, String value)
		{
			if (IsXfaPresent())
			{
				name = FindFieldName(name);
				if (name != null)
				{
					String shortName = Xml2Som.GetShortName(name);
					XNode xn = FindDatasetsNode(shortName);
					if (xn == null)
					{
						xn = datasetsSom.InsertNode(GetDatasetsNode(), shortName);
					}
					SetNodeText(xn, value);
				}
			}
		}

		/// <summary>Gets the xfa field value.</summary>
		/// <param name="name">the fully qualified field name</param>
		/// <returns>the field value</returns>
		public virtual String GetXfaFieldValue(String name)
		{
			if (IsXfaPresent())
			{
				name = FindFieldName(name);
				if (name != null)
				{
					name = Xml2Som.GetShortName(name);
					return GetNodeText(FindDatasetsNode(name));
				}
			}
			return null;
		}

		/// <summary>Returns <CODE>true</CODE> if it is a XFA form.</summary>
		/// <returns><CODE>true</CODE> if it is a XFA form</returns>
		public virtual bool IsXfaPresent()
		{
			return xfaPresent;
		}

		/// <summary>Finds the complete field name from a partial name.</summary>
		/// <param name="name">the complete or partial name</param>
		/// <returns>the complete name or <CODE>null</CODE> if not found</returns>
		public virtual String FindFieldName(String name)
		{
            if (acroFieldsSom == null && xfaPresent && datasetsSom != null)
            {
                IDictionary<String, XNode> name2Node = datasetsSom.GetName2Node();
                ICollection<String> keys = name2Node.Keys;
                acroFieldsSom = new AcroFieldsSearch(keys);
            }

            if (acroFieldsSom != null && xfaPresent)
			{
                List<string> parts = new List<string>(new Stack<string>(Xml2Som.SplitParts(name)));

                bool containsKey = acroFieldsSom.GetAcroShort2LongName().ContainsKey(name);

                string inverse = acroFieldsSom.InverseSearchGlobal(parts);
                string normal = acroFieldsSom.GetAcroShort2LongName().Get(name);

                return containsKey ? normal : inverse;
			}
			return null;
		}

		/// <summary>
		/// Finds the complete SOM name contained in the datasets section from a
		/// possibly partial name.
		/// </summary>
		/// <param name="name">the complete or partial name</param>
		/// <returns>the complete name or <CODE>null</CODE> if not found</returns>
		public virtual String FindDatasetsName(String name)
		{
			return datasetsSom.GetName2Node().ContainsKey(name) ? name : datasetsSom.InverseSearchGlobal
				(new List<string>(new Stack<string>(Xml2Som.SplitParts(name))));
		}

		/// <summary>
		/// Finds the <CODE>Node</CODE> contained in the datasets section from a
		/// possibly partial name.
		/// </summary>
		/// <param name="name">the complete or partial name</param>
		/// <returns>the <CODE>Node</CODE> or <CODE>null</CODE> if not found</returns>
		public virtual XNode FindDatasetsNode(String name)
		{
			if (name == null)
			{
				return null;
			}
			name = FindDatasetsName(name);
			if (name == null)
			{
				return null;
			}
			return datasetsSom.GetName2Node().Get(name);
		}

		/// <summary>Gets all the text contained in the child nodes of this node.</summary>
		/// <param name="n">the <CODE>Node</CODE></param>
		/// <returns>the text found or "" if no text was found</returns>
		public static String GetNodeText(XNode n)
		{
			return n == null ? "" : GetNodeText(n, "");
		}

		/// <summary>Sets the text of this node.</summary>
		/// <remarks>
		/// Sets the text of this node. All the child's node are deleted and a new
		/// child text node is created.
		/// </remarks>
		/// <param name="n">the <CODE>Node</CODE> to add the text to</param>
		/// <param name="text">the text to add</param>
		public virtual void SetNodeText(XNode n, String text)
		{
			if (n == null)
			{
				return;
			}
			XNode nc = null;
			while ((nc = ((XElement)n).FirstNode) != null)
			{
                nc.Remove();
			}
		    XAttribute attr = ((XElement) n).Attribute((XNamespace) XFA_DATA_SCHEMA + "dataNode");
		    if (attr != null) {
		        attr.Remove();
		    }
            
			((XElement)n).Add(new XText(text));
		}

		/// <summary>Gets the top level DOM document.</summary>
		/// <returns>the top level DOM document</returns>
		public virtual XDocument GetDomDocument()
		{
			return domDocument;
		}

		/// <summary>Sets the top DOM document.</summary>
		/// <param name="domDocument">the top DOM document</param>
		public virtual void SetDomDocument(XDocument domDocument)
		{
			this.domDocument = domDocument;
			ExtractNodes();
		}

		/// <summary>Gets the <CODE>Node</CODE> that corresponds to the datasets part.</summary>
		/// <returns>the <CODE>Node</CODE> that corresponds to the datasets part</returns>
		public virtual XElement GetDatasetsNode()
		{
			return datasetsNode;
		}

		/// <summary>Replaces the XFA data under datasets/data.</summary>
		/// <remarks>
		/// Replaces the XFA data under datasets/data. Accepts a
		/// <see cref="System.IO.FileInfo">
		/// file
		/// object
		/// </see>
		/// to fill this object with XFA data. The resulting DOM document may
		/// be modified.
		/// </remarks>
		/// <param name="file">
		/// the
		/// <see cref="System.IO.FileInfo"/>
		/// </param>
		public virtual void FillXfaForm(FileInfo file)
		{
			FillXfaForm(file, false);
		}

		/// <summary>Replaces the XFA data under datasets/data.</summary>
		/// <remarks>
		/// Replaces the XFA data under datasets/data. Accepts a
		/// <see cref="System.IO.FileInfo">
		/// file
		/// object
		/// </see>
		/// to fill this object with XFA data.
		/// </remarks>
		/// <param name="file">
		/// the
		/// <see cref="System.IO.FileInfo"/>
		/// </param>
		/// <param name="readOnly">whether or not the resulting DOM document may be modified</param>
		public virtual void FillXfaForm(FileInfo file, bool readOnly)
		{
			FillXfaForm(new FileStream(file.FullName, FileMode.Open, FileAccess.Read), readOnly);
		}

		/// <summary>Replaces the XFA data under datasets/data.</summary>
		/// <remarks>
		/// Replaces the XFA data under datasets/data. Accepts an
		/// <see cref="System.IO.Stream"/>
		/// to fill this object with XFA data. The resulting DOM document may be
		/// modified.
		/// </remarks>
		/// <param name="is">
		/// the
		/// <see cref="System.IO.Stream"/>
		/// </param>
		public virtual void FillXfaForm(Stream @is)
		{
			FillXfaForm(@is, false);
		}

		/// <summary>Replaces the XFA data under datasets/data.</summary>
		/// <remarks>
		/// Replaces the XFA data under datasets/data. Accepts an
		/// <see cref="System.IO.Stream"/>
		/// to fill this object with XFA data.
		/// </remarks>
		/// <param name="is">
		/// the
		/// <see cref="System.IO.Stream"/>
		/// </param>
		/// <param name="readOnly">whether or not the resulting DOM document may be modified</param>
		public virtual void FillXfaForm(Stream @is, bool readOnly)
		{
			XmlReaderSettings settings = new XmlReaderSettings {NameTable = new NameTable()};
			XmlNamespaceManager xmlns = new XmlNamespaceManager(settings.NameTable);
			xmlns.AddNamespace("xfa", "http://www.xfa.org/schema/xci/1.0/");
			XmlReader reader =
				XmlProcessorCreator.CreateSafeXmlReader(@is, new XmlParserContext(null, xmlns, "", XmlSpace.Default));
			try
			{
				FillXfaForm(reader, readOnly);
			}
			catch (Exception e)
			{
				throw new PdfException(e.Message, e);
			}
		}

		/// <summary>Replaces the XFA data under datasets/data.</summary>
		/// <remarks>
		/// Replaces the XFA data under datasets/data. Accepts an
		/// <see cref="System.Xml.XmlReader">XML reader</see>
		/// to fill this object with XFA data. The resulting DOM
		/// document may be modified.
		/// </remarks>
		/// <param name="is">
		/// the
		/// <see cref="System.Xml.XmlReader">XML reader</see>
		/// </param>
		public virtual void FillXfaForm(XmlReader @is)
		{
			FillXfaForm(@is, false);
		}

		/// <summary>Replaces the XFA data under datasets/data.</summary>
		/// <remarks>
		/// Replaces the XFA data under datasets/data. Accepts an
		/// <see cref="System.Xml.XmlReader">XML reader</see>
		/// to fill this object with XFA data.
		/// </remarks>
		/// <param name="is">
		/// the
		/// <see cref="System.Xml.XmlReader">XML reader</see>
		/// </param>
		/// <param name="readOnly">whether or not the resulting DOM document may be modified</param>
		public virtual void FillXfaForm(XmlReader @is, bool readOnly) {
		    FillXfaForm(XDocument.Load(@is, LoadOptions.PreserveWhitespace), readOnly);
		}

		/// <summary>Replaces the XFA data under datasets/data.</summary>
		/// <param name="node">
		/// the input
		/// <see cref="System.Xml.Linq.XNode"/>
		/// </param>
		public virtual void FillXfaForm(XNode node)
		{
			FillXfaForm(node, false);
		}

		/// <summary>Replaces the XFA data under datasets/data.</summary>
		/// <param name="node">
		/// the input
		/// <see cref="System.Xml.Linq.XNode"/>
		/// </param>
		/// <param name="readOnly">whether or not the resulting DOM document may be modified</param>
		public virtual void FillXfaForm(XNode node, bool readOnly)
		{
			if (readOnly)
			{
				IEnumerable<XElement> nodeList = domDocument.Elements("field");
				for (int i = 0; i < nodeList.Count(); i++)
				{
					nodeList.ElementAt(i).SetAttributeValue("access", "readOnly");
				}
			}
		    IEnumerable<XNode> allChilds = ((XElement) datasetsNode).Nodes();
		    int len = allChilds.Count();
			XNode data = null;
			for (int k = 0; k < len; ++k) {
			    XNode n = allChilds.ElementAt(k);
				if (n is XElement && ((XElement)n).Name.LocalName.Equals("data") && XFA_DATA_SCHEMA
					.Equals(((XElement)n).Name.NamespaceName))
				{
					data = n;
					break;
				}
			}
			if (data == null) {
				data = new XElement((XNamespace) XFA_DATA_SCHEMA + "data");
				datasetsNode.Add(data);
			}
		    IEnumerable<XNode> list = ((XElement) data).Nodes();
			if (list.Count() == 0)
			{
				((XElement)data).Add(node is XDocument ? ((XDocument)node).Root : node);
			}
			else
			{
				// There's a possibility that first child node of XFA data is not an ELEMENT but simply a TEXT. In this case data will be duplicated.
				// data.replaceChild(domDocument.importNode(node, true), data.getFirstChild());
				XNode firstNode = GetFirstElementNode(data);
				if (firstNode != null)
				{
					firstNode.ReplaceWith(node is XDocument ? ((XDocument)node).Root : node);
				}
			}
			ExtractNodes();
		}

		private static String GetNodeText(XNode n, String name) {
		    XNode n2 = ((XElement) n).FirstNode;
			while (n2 != null)
			{
				if (n2 is XElement)
				{
					name = GetNodeText(n2, name);
				}
				else
				{
					if (n2 is XText) {
					    name += ((XText) n2).Value;
					}
				}
			    n2 = ((XElement) n2).NextNode;
			}
			return name;
		}

		/// <summary>Return the XFA Object, could be an array, could be a Stream.</summary>
		/// <remarks>
		/// Return the XFA Object, could be an array, could be a Stream.
		/// Returns null if no XFA Object is present.
		/// </remarks>
		/// <param name="pdfDocument">a PdfDocument instance</param>
		/// <returns>the XFA object</returns>
		private static PdfObject GetXfaObject(PdfDocument pdfDocument)
		{
			PdfDictionary af = pdfDocument.GetCatalog().GetPdfObject().GetAsDictionary(PdfName
				.AcroForm);
			return af == null ? null : af.Get(PdfName.XFA);
		}

	    /// <summary>Return the XFA Object, could be an array, could be a Stream.</summary>
	    /// <remarks>
	    /// Return the XFA Object, could be an array, could be a Stream.
	    /// Returns null if no XFA Object is present.
	    /// </remarks>
	    /// <param name="acroForm">a PdfAcroForm instance</param>
	    /// <returns>the XFA object</returns>
	    private static PdfObject GetXfaObject(PdfAcroForm acroForm)
	    {
	        return acroForm == null || acroForm.GetPdfObject() == null ? null : acroForm.GetPdfObject().Get(PdfName.XFA);
	    }

		/// <summary>Serializes a XML document to a byte array.</summary>
		/// <param name="n">the XML document</param>
		/// <returns>the serialized XML document</returns>
		private static byte[] SerializeDocument(XNode n)
		{
		    MemoryStream fout = new MemoryStream(INIT_SERIALIZER_BUFFER_SIZE);
		    if (n != null) {
		        if (n is XDocument) {
		            fout.Write("<?xml version=\"1.0\" encoding=\"UTF-8\"?>".GetBytes(Encoding.UTF8));
		        }
		        XmlWriterSettings settings = new XmlWriterSettings {
		            Encoding = new UTF8Encoding(false),
		            OmitXmlDeclaration = true,
			        NewLineChars = "\n",
		        };
		        using (XmlWriter writer = XmlWriter.Create(fout, settings)) {
			        n.WriteTo(writer);
		        }
		    }
            fout.Dispose();
		    return fout.ToArray();
		}

		private void InitXfaForm(PdfObject xfa)
		{
			MemoryStream bout = new MemoryStream();
			if (xfa.IsArray())
			{
				PdfArray ar = (PdfArray)xfa;
				for (int k = 1; k < ar.Size(); k += 2)
				{
					PdfObject ob = ar.Get(k);
					if (ob is PdfStream)
					{
						byte[] b = ((PdfStream)ob).GetBytes();
						bout.Write(b);
					}
				}
			}
			else
			{
				if (xfa is PdfStream)
				{
					byte[] b = ((PdfStream)xfa).GetBytes();
					bout.Write(b);
				}
			}
			bout.Dispose();
			InitXfaForm(new MemoryStream(bout.ToArray()));
		}

		private void InitXfaForm(Stream inputStream)
		{
			XmlReader reader = XmlProcessorCreator.CreateSafeXmlReader(inputStream);
			SetDomDocument(XDocument.Load(reader, LoadOptions.PreserveWhitespace));
			xfaPresent = true;
		}

		/// <summary>Extracts the nodes from the domDocument.</summary>
		private void ExtractNodes()
		{
			 IDictionary<String, XNode> xfaNodes = ExtractXFANodes(domDocument);
			if (xfaNodes.ContainsKey("template")) {
			    templateNode = (XElement)xfaNodes["template"];
			}
			if (xfaNodes.ContainsKey("datasets"))
			{
				datasetsNode = (XElement)xfaNodes["datasets"];
                XElement dataNode = FindDataNode(datasetsNode);
                datasetsSom = new Xml2SomDatasets(dataNode != null ? dataNode : datasetsNode.FirstNode);
			}
			if (datasetsNode == null)
			{
				CreateDatasetsNode(domDocument.FirstNode);
			}
		}

		/// <summary>Some XFA forms don't have a datasets node.</summary>
		/// <remarks>
		/// Some XFA forms don't have a datasets node.
		/// If this is the case, we have to add one.
		/// </remarks>
		private void CreateDatasetsNode(XNode n)
		{
			while (!(n is XElement) || !((XElement)n).Nodes().Any()) {
			    n = n.NextNode;
			}
			if (n != null)
			{
				XElement e = new XElement((XNamespace)XFA_DATA_SCHEMA + "datasets", 
					new XAttribute(XNamespace.Xmlns + "xfa", XFA_DATA_SCHEMA));
				datasetsNode = e;
				((XElement)n).Add(datasetsNode);
			}
		}

		private XNode GetFirstElementNode(XNode src)
		{
			XNode result = null;
		    IEnumerable<XNode> list = ((XElement) src).Nodes();
			for (int i = 0; i < list.Count(); i++)
			{
				if (list.ElementAt(i) is XElement)
				{
					result = list.ElementAt(i);
					break;
				}
			}
			return result;
		}

        private XElement FindDataNode(XElement datasetsNode)
	    {
            //return datasetsNode.Element("{xfa}data");
            XName name = XName.Get("data", "http://www.xfa.org/schema/xfa-data/1.0/");
            return datasetsNode.Element(name);
	    }
    }
}
