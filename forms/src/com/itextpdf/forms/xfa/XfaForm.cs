/*
$Id: 669c4831603d1e8a0f761a5bc795efdd002cc64f $

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

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
using System.IO;
using com.itextpdf.forms;
using com.itextpdf.io.util;
using com.itextpdf.kernel;
using com.itextpdf.kernel.pdf;
using com.itextpdf.kernel.xmp;
using java.io;
using javax.xml.parsers;
using org.w3c.dom;
using org.xml.sax;

namespace com.itextpdf.forms.xfa
{
	/// <summary>Processes XFA forms.</summary>
	public class XfaForm
	{
		private const String DEFAULT_XFA = "com/itextpdf/forms/xfa/default.xml";

		private Node templateNode;

		private Xml2SomDatasets datasetsSom;

		private Node datasetsNode;

		private AcroFieldsSearch acroFieldsSom;

		private bool xfaPresent = false;

		private Document domDocument;

		/// <summary>The URI for the XFA Data schema.</summary>
		public const String XFA_DATA_SCHEMA = "http://www.xfa.org/schema/xfa-data/1.0/";

		/// <summary>An empty constructor to build on.</summary>
		public XfaForm()
			: this(ResourceUtil.GetResourceStream(DEFAULT_XFA))
		{
		}

		/// <summary>Creates an XFA form by the stream containing all xml information</summary>
		public XfaForm(Stream inputStream)
		{
			try
			{
				InitXfaForm(inputStream);
			}
			catch (Exception e)
			{
				throw new PdfException(e);
			}
		}

		/// <summary>
		/// Creates an XFA form by the
		/// <see cref="org.w3c.dom.Document"/>
		/// containing all xml information
		/// </summary>
		public XfaForm(Document domDocument)
		{
			SetDomDocument(domDocument);
		}

		/// <summary>
		/// A constructor from a
		/// <see cref="com.itextpdf.kernel.pdf.PdfDictionary"/>
		/// . It is assumed, but not
		/// necessary for correct initialization, that the dictionary is actually a
		/// <see cref="com.itextpdf.forms.PdfAcroForm"/>
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
					throw new PdfException(e);
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
					throw new PdfException(e);
				}
			}
		}

		/// <summary>Sets the XFA key from a byte array.</summary>
		/// <remarks>Sets the XFA key from a byte array. The old XFA is erased.</remarks>
		/// <param name="form">the data</param>
		/// <param name="pdfDocument">pdfDocument</param>
		/// <exception cref="System.IO.IOException">on IO error</exception>
		public static void SetXfaForm(com.itextpdf.forms.xfa.XfaForm form, PdfDocument pdfDocument
			)
		{
			PdfDictionary af = PdfAcroForm.GetAcroForm(pdfDocument, true).GetPdfObject();
			PdfObject xfa = GetXfaObject(pdfDocument);
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
					tStream.SetCompressionLevel(pdfDocument.GetWriter().GetCompressionLevel());
					ar.Set(t, tStream);
					PdfStream dStream = new PdfStream(SerializeDocument(form.datasetsNode));
					dStream.SetCompressionLevel(pdfDocument.GetWriter().GetCompressionLevel());
					ar.Set(d, dStream);
					ar.Flush();
					af.Put(PdfName.XFA, new PdfArray(ar));
					return;
				}
			}
			//reader.killXref(af.get(PdfName.XFA));
			PdfStream stream = new PdfStream(SerializeDocument(form.domDocument));
			stream.SetCompressionLevel(pdfDocument.GetWriter().GetCompressionLevel());
			stream.Flush();
			af.Put(PdfName.XFA, stream);
			af.SetModified();
		}

		/// <summary>Extracts DOM nodes from an XFA document.</summary>
		/// <param name="domDocument">
		/// an XFA file as a
		/// <see cref="org.w3c.dom.Document">
		/// DOM
		/// document
		/// </see>
		/// </param>
		/// <returns>
		/// a
		/// <see cref="System.Collections.IDictionary{K, V}"/>
		/// of XFA packet names and their associated
		/// <see cref="org.w3c.dom.Node">DOM nodes</see>
		/// </returns>
		public static IDictionary<String, Node> ExtractXFANodes(Document domDocument)
		{
			IDictionary<String, Node> xfaNodes = new Dictionary<String, Node>();
			Node n = domDocument.GetFirstChild();
			while (n.GetChildNodes().GetLength() == 0)
			{
				n = n.GetNextSibling();
			}
			n = n.GetFirstChild();
			while (n != null)
			{
				if (n.GetNodeType() == Node.ELEMENT_NODE)
				{
					String s = n.GetLocalName();
					xfaNodes[s] = n;
				}
				n = n.GetNextSibling();
			}
			return xfaNodes;
		}

		/// <summary>Write the XfaForm to the provided PdfDocument.</summary>
		/// <param name="document">the PdfDocument to write the XFA Form to</param>
		/// <exception cref="System.IO.IOException"/>
		public virtual void Write(PdfDocument document)
		{
			SetXfaForm(this, document);
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
					Node xn = FindDatasetsNode(shortName);
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
					return com.itextpdf.forms.xfa.XfaForm.GetNodeText(FindDatasetsNode(name));
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
			if (acroFieldsSom == null && xfaPresent)
			{
				acroFieldsSom = new AcroFieldsSearch(datasetsSom.GetName2Node().Keys);
				return acroFieldsSom.GetAcroShort2LongName().ContainsKey(name) ? acroFieldsSom.GetAcroShort2LongName
					()[name] : acroFieldsSom.InverseSearchGlobal(Xml2Som.SplitParts(name));
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
				(Xml2Som.SplitParts(name));
		}

		/// <summary>
		/// Finds the <CODE>Node</CODE> contained in the datasets section from a
		/// possibly partial name.
		/// </summary>
		/// <param name="name">the complete or partial name</param>
		/// <returns>the <CODE>Node</CODE> or <CODE>null</CODE> if not found</returns>
		public virtual Node FindDatasetsNode(String name)
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
			return datasetsSom.GetName2Node()[name];
		}

		/// <summary>Gets all the text contained in the child nodes of this node.</summary>
		/// <param name="n">the <CODE>Node</CODE></param>
		/// <returns>the text found or "" if no text was found</returns>
		public static String GetNodeText(Node n)
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
		public virtual void SetNodeText(Node n, String text)
		{
			if (n == null)
			{
				return;
			}
			Node nc = null;
			while ((nc = n.GetFirstChild()) != null)
			{
				n.RemoveChild(nc);
			}
			if (n.GetAttributes().GetNamedItemNS(XFA_DATA_SCHEMA, "dataNode") != null)
			{
				n.GetAttributes().RemoveNamedItemNS(XFA_DATA_SCHEMA, "dataNode");
			}
			n.AppendChild(domDocument.CreateTextNode(text));
		}

		/// <summary>Gets the top level DOM document.</summary>
		/// <returns>the top level DOM document</returns>
		public virtual Document GetDomDocument()
		{
			return domDocument;
		}

		/// <summary>Sets the top DOM document.</summary>
		/// <param name="domDocument">the top DOM document</param>
		public virtual void SetDomDocument(Document domDocument)
		{
			this.domDocument = domDocument;
			ExtractNodes();
		}

		/// <summary>Gets the <CODE>Node</CODE> that corresponds to the datasets part.</summary>
		/// <returns>the <CODE>Node</CODE> that corresponds to the datasets part</returns>
		public virtual Node GetDatasetsNode()
		{
			return datasetsNode;
		}

		/// <summary>Replaces the XFA data under datasets/data.</summary>
		/// <remarks>
		/// Replaces the XFA data under datasets/data. Accepts a
		/// <see cref="java.io.File">
		/// file
		/// object
		/// </see>
		/// to fill this object with XFA data. The resulting DOM document may
		/// be modified.
		/// </remarks>
		/// <param name="file">
		/// the
		/// <see cref="java.io.File"/>
		/// </param>
		/// <exception cref="System.IO.IOException">
		/// on IO error on the
		/// <see cref="org.xml.sax.InputSource"/>
		/// </exception>
		public virtual void FillXfaForm(File file)
		{
			FillXfaForm(file, false);
		}

		/// <summary>Replaces the XFA data under datasets/data.</summary>
		/// <remarks>
		/// Replaces the XFA data under datasets/data. Accepts a
		/// <see cref="java.io.File">
		/// file
		/// object
		/// </see>
		/// to fill this object with XFA data.
		/// </remarks>
		/// <param name="file">
		/// the
		/// <see cref="java.io.File"/>
		/// </param>
		/// <param name="readOnly">whether or not the resulting DOM document may be modified</param>
		/// <exception cref="System.IO.IOException">
		/// on IO error on the
		/// <see cref="org.xml.sax.InputSource"/>
		/// </exception>
		public virtual void FillXfaForm(File file, bool readOnly)
		{
			FillXfaForm(new FileInputStream(file), readOnly);
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
		/// <exception cref="System.IO.IOException">
		/// on IO error on the
		/// <see cref="org.xml.sax.InputSource"/>
		/// </exception>
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
		/// <exception cref="System.IO.IOException">
		/// on IO error on the
		/// <see cref="org.xml.sax.InputSource"/>
		/// </exception>
		public virtual void FillXfaForm(Stream @is, bool readOnly)
		{
			FillXfaForm(new InputSource(@is), readOnly);
		}

		/// <summary>Replaces the XFA data under datasets/data.</summary>
		/// <remarks>
		/// Replaces the XFA data under datasets/data. Accepts a
		/// <see cref="org.xml.sax.InputSource">SAX input source</see>
		/// to fill this object with XFA data. The resulting DOM
		/// document may be modified.
		/// </remarks>
		/// <param name="is">
		/// the
		/// <see cref="org.xml.sax.InputSource">SAX input source</see>
		/// </param>
		/// <exception cref="System.IO.IOException">
		/// on IO error on the
		/// <see cref="org.xml.sax.InputSource"/>
		/// </exception>
		public virtual void FillXfaForm(InputSource @is)
		{
			FillXfaForm(@is, false);
		}

		/// <summary>Replaces the XFA data under datasets/data.</summary>
		/// <remarks>
		/// Replaces the XFA data under datasets/data. Accepts a
		/// <see cref="org.xml.sax.InputSource">SAX input source</see>
		/// to fill this object with XFA data.
		/// </remarks>
		/// <param name="is">
		/// the
		/// <see cref="org.xml.sax.InputSource">SAX input source</see>
		/// </param>
		/// <param name="readOnly">whether or not the resulting DOM document may be modified</param>
		/// <exception cref="System.IO.IOException">
		/// on IO error on the
		/// <see cref="org.xml.sax.InputSource"/>
		/// </exception>
		public virtual void FillXfaForm(InputSource @is, bool readOnly)
		{
			DocumentBuilderFactory dbf = DocumentBuilderFactory.NewInstance();
			DocumentBuilder db;
			try
			{
				db = dbf.NewDocumentBuilder();
				Document newdoc = db.Parse(@is);
				FillXfaForm(newdoc.GetDocumentElement(), readOnly);
			}
			catch (ParserConfigurationException e)
			{
				throw new PdfException(e);
			}
			catch (SAXException e)
			{
				throw new PdfException(e);
			}
		}

		/// <summary>Replaces the XFA data under datasets/data.</summary>
		/// <param name="node">
		/// the input
		/// <see cref="org.w3c.dom.Node"/>
		/// </param>
		public virtual void FillXfaForm(Node node)
		{
			FillXfaForm(node, false);
		}

		/// <summary>Replaces the XFA data under datasets/data.</summary>
		/// <param name="node">
		/// the input
		/// <see cref="org.w3c.dom.Node"/>
		/// </param>
		/// <param name="readOnly">whether or not the resulting DOM document may be modified</param>
		public virtual void FillXfaForm(Node node, bool readOnly)
		{
			if (readOnly)
			{
				NodeList nodeList = domDocument.GetElementsByTagName("field");
				for (int i = 0; i < nodeList.GetLength(); i++)
				{
					((Element)nodeList.Item(i)).SetAttribute("access", "readOnly");
				}
			}
			NodeList allChilds = datasetsNode.GetChildNodes();
			int len = allChilds.GetLength();
			Node data = null;
			for (int k = 0; k < len; ++k)
			{
				Node n = allChilds.Item(k);
				if (n.GetNodeType() == Node.ELEMENT_NODE && n.GetLocalName().Equals("data") && XFA_DATA_SCHEMA
					.Equals(n.GetNamespaceURI()))
				{
					data = n;
					break;
				}
			}
			if (data == null)
			{
				data = datasetsNode.GetOwnerDocument().CreateElementNS(XFA_DATA_SCHEMA, "xfa:data"
					);
				datasetsNode.AppendChild(data);
			}
			NodeList list = data.GetChildNodes();
			if (list.GetLength() == 0)
			{
				data.AppendChild(domDocument.ImportNode(node, true));
			}
			else
			{
				// There's a possibility that first child node of XFA data is not an ELEMENT but simply a TEXT. In this case data will be duplicated.
				// data.replaceChild(domDocument.importNode(node, true), data.getFirstChild());
				Node firstNode = GetFirstElementNode(data);
				if (firstNode != null)
				{
					data.ReplaceChild(domDocument.ImportNode(node, true), firstNode);
				}
			}
			ExtractNodes();
		}

		private static String GetNodeText(Node n, String name)
		{
			Node n2 = n.GetFirstChild();
			while (n2 != null)
			{
				if (n2.GetNodeType() == Node.ELEMENT_NODE)
				{
					name = GetNodeText(n2, name);
				}
				else
				{
					if (n2.GetNodeType() == Node.TEXT_NODE)
					{
						name += n2.GetNodeValue();
					}
				}
				n2 = n2.GetNextSibling();
			}
			return name;
		}

		/// <summary>Return the XFA Object, could be an array, could be a Stream.</summary>
		/// <remarks>
		/// Return the XFA Object, could be an array, could be a Stream.
		/// Returns null f no XFA Object is present.
		/// </remarks>
		/// <param name="pdfDocument">a PdfDocument instance</param>
		/// <returns>the XFA object</returns>
		private static PdfObject GetXfaObject(PdfDocument pdfDocument)
		{
			PdfDictionary af = pdfDocument.GetCatalog().GetPdfObject().GetAsDictionary(PdfName
				.AcroForm);
			return af == null ? null : af.Get(PdfName.XFA);
		}

		/// <summary>Serializes a XML document to a byte array.</summary>
		/// <param name="n">the XML document</param>
		/// <returns>the serialized XML document</returns>
		/// <exception cref="System.IO.IOException">on error</exception>
		private static byte[] SerializeDocument(Node n)
		{
			XmlDomWriter xw = new XmlDomWriter();
			MemoryStream fout = new MemoryStream();
			xw.SetOutput(fout, null);
			xw.SetCanonical(false);
			xw.Write(n);
			fout.Close();
			return fout.ToArray();
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="javax.xml.parsers.ParserConfigurationException"/>
		/// <exception cref="org.xml.sax.SAXException"/>
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
			bout.Close();
			InitXfaForm(new MemoryStream(bout.ToArray()));
		}

		/// <exception cref="javax.xml.parsers.ParserConfigurationException"/>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="org.xml.sax.SAXException"/>
		private void InitXfaForm(Stream inputStream)
		{
			DocumentBuilderFactory fact = DocumentBuilderFactory.NewInstance();
			fact.SetNamespaceAware(true);
			DocumentBuilder db = fact.NewDocumentBuilder();
			SetDomDocument(db.Parse(inputStream));
			xfaPresent = true;
		}

		/// <summary>Extracts the nodes from the domDocument.</summary>
		private void ExtractNodes()
		{
			IDictionary<String, Node> xfaNodes = ExtractXFANodes(domDocument);
			if (xfaNodes.ContainsKey("template"))
			{
				templateNode = xfaNodes["template"];
			}
			if (xfaNodes.ContainsKey("datasets"))
			{
				datasetsNode = xfaNodes["datasets"];
				datasetsSom = new Xml2SomDatasets(datasetsNode.GetFirstChild());
			}
			if (datasetsNode == null)
			{
				CreateDatasetsNode(domDocument.GetFirstChild());
			}
		}

		/// <summary>Some XFA forms don't have a datasets node.</summary>
		/// <remarks>
		/// Some XFA forms don't have a datasets node.
		/// If this is the case, we have to add one.
		/// </remarks>
		private void CreateDatasetsNode(Node n)
		{
			while (n.GetChildNodes().GetLength() == 0)
			{
				n = n.GetNextSibling();
			}
			if (n != null)
			{
				Element e = n.GetOwnerDocument().CreateElement("xfa:datasets");
				e.SetAttribute("xmlns:xfa", XFA_DATA_SCHEMA);
				datasetsNode = e;
				n.AppendChild(datasetsNode);
			}
		}

		private Node GetFirstElementNode(Node src)
		{
			Node result = null;
			NodeList list = src.GetChildNodes();
			for (int i = 0; i < list.GetLength(); i++)
			{
				if (list.Item(i).GetNodeType() == Node.ELEMENT_NODE)
				{
					result = list.Item(i);
					break;
				}
			}
			return result;
		}
	}
}
