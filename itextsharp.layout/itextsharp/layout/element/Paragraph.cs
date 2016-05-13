/*
$Id: 3dc8d839c802088dc0549ef7b4f567aa337a241d $

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
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Tagutils;
using iTextSharp.Layout.Property;
using iTextSharp.Layout.Renderer;

namespace iTextSharp.Layout.Element
{
	/// <summary>
	/// A layout element that represents a self-contained block of textual and
	/// grpahical information.
	/// </summary>
	/// <remarks>
	/// A layout element that represents a self-contained block of textual and
	/// grpahical information.
	/// It is a
	/// <see cref="BlockElement{T}"/>
	/// which essentially acts as a container for
	/// <see cref="ILeafElement">leaf elements</see>
	/// .
	/// </remarks>
	public class Paragraph : BlockElement<iTextSharp.Layout.Element.Paragraph>
	{
		protected internal PdfName role = PdfName.P;

		protected internal AccessibilityProperties tagProperties;

		/// <summary>Creates a Paragraph.</summary>
		public Paragraph()
		{
		}

		/// <summary>Creates a Paragraph, initialized with a piece of text.</summary>
		/// <param name="text">
		/// the initial textual content, as a
		/// <see cref="System.String"/>
		/// </param>
		public Paragraph(String text)
			: this(new Text(text))
		{
		}

		/// <summary>Creates a Paragraph, initialized with a piece of text.</summary>
		/// <param name="text">
		/// the initial textual content, as a
		/// <see cref="Text"/>
		/// </param>
		public Paragraph(Text text)
		{
			Add(text);
		}

		/// <summary>Adds a piece of text to the Paragraph</summary>
		/// <param name="text">
		/// the content to be added, as a
		/// <see cref="System.String"/>
		/// </param>
		/// <returns>this Paragraph</returns>
		public virtual iTextSharp.Layout.Element.Paragraph Add(String text)
		{
			return Add(new Text(text));
		}

		/// <summary>Adds a layout element to the Paragraph.</summary>
		/// <param name="element">
		/// the content to be added, any
		/// <see cref="ILeafElement"/>
		/// </param>
		/// <returns>this Paragraph</returns>
		public virtual iTextSharp.Layout.Element.Paragraph Add(ILeafElement element)
		{
			childElements.Add(element);
			return this;
		}

		/// <summary>
		/// Adds a
		/// <see cref="System.Collections.IList{E}"/>
		/// of layout elements to the Paragraph.
		/// </summary>
		/// <param name="elements">
		/// the content to be added, any
		/// <see cref="ILeafElement"/>
		/// </param>
		/// <returns>this Paragraph</returns>
		public virtual iTextSharp.Layout.Element.Paragraph AddAll<T2>(IList<T2> elements)
			where T2 : ILeafElement
		{
			foreach (ILeafElement element in elements)
			{
				Add(element);
			}
			return this;
		}

		/// <summary>Adds an unspecified amount of tabstop elements as properties to the Paragraph.
		/// 	</summary>
		/// <param name="tabStops">
		/// the
		/// <see cref="TabStop">tabstop(s)</see>
		/// to be added as properties
		/// </param>
		/// <returns>this Paragraph</returns>
		/// <seealso cref="TabStop"/>
		public virtual iTextSharp.Layout.Element.Paragraph AddTabStops(params TabStop[] tabStops
			)
		{
			AddTabStopsAsProperty(iTextSharp.IO.Util.JavaUtil.ArraysAsList(tabStops));
			return this;
		}

		/// <summary>
		/// Adds a
		/// <see cref="System.Collections.IList{E}"/>
		/// of tabstop elements as properties to the Paragraph.
		/// </summary>
		/// <param name="tabStops">
		/// the list of
		/// <see cref="TabStop"/>
		/// s to be added as properties
		/// </param>
		/// <returns>this Paragraph</returns>
		/// <seealso cref="TabStop"/>
		public virtual iTextSharp.Layout.Element.Paragraph AddTabStops(IList<TabStop> tabStops
			)
		{
			AddTabStopsAsProperty(tabStops);
			return this;
		}

		/// <summary>
		/// Removes a tabstop position from the Paragraph, if it is present in the
		/// <see cref="iTextSharp.Layout.Property.Property.TAB_STOPS"/>
		/// property.
		/// </summary>
		/// <param name="tabStopPosition">
		/// the
		/// <see cref="TabStop"/>
		/// position to be removed.
		/// </param>
		/// <returns>this Paragraph</returns>
		/// <seealso cref="TabStop"/>
		public virtual iTextSharp.Layout.Element.Paragraph RemoveTabStop(float tabStopPosition
			)
		{
			IDictionary<float, TabStop> tabStops = ((IDictionary<float, TabStop>)GetProperty(
				iTextSharp.Layout.Property.Property.TAB_STOPS));
			if (tabStops != null)
			{
				tabStops.Remove(tabStopPosition);
			}
			return this;
		}

		public override T1 GetDefaultProperty<T1>(int property)
		{
			switch (property)
			{
				case iTextSharp.Layout.Property.Property.LEADING:
				{
					return (T1)new Leading(Leading.MULTIPLIED, childElements.Count == 1 && childElements
						[0] is Image ? 1 : 1.35f);
				}

				case iTextSharp.Layout.Property.Property.FIRST_LINE_INDENT:
				{
					return (T1)float.ValueOf(0);
				}

				case iTextSharp.Layout.Property.Property.MARGIN_TOP:
				case iTextSharp.Layout.Property.Property.MARGIN_BOTTOM:
				{
					return (T1)float.ValueOf(4);
				}

				case iTextSharp.Layout.Property.Property.TAB_DEFAULT:
				{
					return (T1)float.ValueOf(50);
				}

				default:
				{
					return base.GetDefaultProperty(property);
				}
			}
		}

		/// <summary>
		/// Sets the indent value for the first line of the
		/// <see cref="Paragraph"/>
		/// .
		/// </summary>
		/// <param name="indent">
		/// the indent value that must be applied to the first line of
		/// the Paragraph, as a <code>float</code>
		/// </param>
		/// <returns>this Paragraph</returns>
		public virtual iTextSharp.Layout.Element.Paragraph SetFirstLineIndent(float indent
			)
		{
			SetProperty(iTextSharp.Layout.Property.Property.FIRST_LINE_INDENT, indent);
			return this;
		}

		/// <summary>
		/// Sets the leading value, using the
		/// <see cref="iTextSharp.Layout.Property.Leading.FIXED"/>
		/// strategy.
		/// </summary>
		/// <param name="leading">the new leading value</param>
		/// <returns>this Paragraph</returns>
		/// <seealso cref="iTextSharp.Layout.Property.Leading"/>
		public virtual iTextSharp.Layout.Element.Paragraph SetFixedLeading(float leading)
		{
			SetProperty(iTextSharp.Layout.Property.Property.LEADING, new Leading(Leading.FIXED
				, leading));
			return this;
		}

		/// <summary>
		/// Sets the leading value, using the
		/// <see cref="iTextSharp.Layout.Property.Leading.MULTIPLIED"/>
		/// strategy.
		/// </summary>
		/// <param name="leading">the new leading value</param>
		/// <returns>this Paragraph</returns>
		/// <seealso cref="iTextSharp.Layout.Property.Leading"/>
		public virtual iTextSharp.Layout.Element.Paragraph SetMultipliedLeading(float leading
			)
		{
			SetProperty(iTextSharp.Layout.Property.Property.LEADING, new Leading(Leading.MULTIPLIED
				, leading));
			return this;
		}

		protected internal override IRenderer MakeNewRenderer()
		{
			return new ParagraphRenderer(this);
		}

		private void AddTabStopsAsProperty(IList<TabStop> newTabStops)
		{
			IDictionary<float, TabStop> tabStops = ((IDictionary<float, TabStop>)GetProperty(
				iTextSharp.Layout.Property.Property.TAB_STOPS));
			if (tabStops == null)
			{
				tabStops = new SortedDictionary<float, TabStop>();
				SetProperty(iTextSharp.Layout.Property.Property.TAB_STOPS, tabStops);
			}
			foreach (TabStop tabStop in newTabStops)
			{
				tabStops[tabStop.GetTabPosition()] = tabStop;
			}
		}

		public override PdfName GetRole()
		{
			return role;
		}

		public override void SetRole(PdfName role)
		{
			this.role = role;
			if (PdfName.Artifact.Equals(role))
			{
				PropagateArtifactRoleToChildElements();
			}
		}

		public override AccessibilityProperties GetAccessibilityProperties()
		{
			if (tagProperties == null)
			{
				tagProperties = new AccessibilityProperties();
			}
			return tagProperties;
		}
	}
}
