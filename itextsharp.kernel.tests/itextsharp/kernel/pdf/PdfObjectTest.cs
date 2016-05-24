using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace iTextSharp.Kernel.Pdf
{
	public class PdfObjectTest
	{
		[Test]
		public virtual void IndirectsChain1()
		{
			MemoryStream baos = new MemoryStream();
			PdfWriter writer = new PdfWriter(baos);
			PdfDocument document = new PdfDocument(writer);
			document.AddNewPage();
			PdfDictionary catalog = document.GetCatalog().GetPdfObject();
			catalog.Put(new PdfName("a"), ((PdfDictionary)new PdfDictionary(new _Dictionary_24
				()).MakeIndirect(document)).GetIndirectReference().MakeIndirect(document).GetIndirectReference
				().MakeIndirect(document));
			PdfObject @object = ((PdfIndirectReference)catalog.Get(new PdfName("a"), false)).
				GetRefersTo(true);
			NUnit.Framework.Assert.IsTrue(@object is PdfDictionary);
			document.Close();
		}

		private sealed class _Dictionary_24 : Dictionary<PdfName, PdfObject>
		{
			public _Dictionary_24()
			{
				{
					this[new PdfName("b")] = new PdfName("c");
				}
			}
		}

		[Test]
		public virtual void IndirectsChain2()
		{
			MemoryStream baos = new MemoryStream();
			PdfWriter writer = new PdfWriter(baos);
			PdfDocument document = new PdfDocument(writer);
			document.AddNewPage();
			PdfDictionary catalog = document.GetCatalog().GetPdfObject();
			PdfDictionary dictionary = new PdfDictionary(new _Dictionary_39());
			PdfObject @object = dictionary;
			for (int i = 0; i < 200; i++)
			{
				@object = @object.MakeIndirect(document).GetIndirectReference();
			}
			catalog.Put(new PdfName("a"), @object);
			((PdfIndirectReference)catalog.Get(new PdfName("a"))).GetRefersTo(true);
			NUnit.Framework.Assert.IsNotNull(((PdfIndirectReference)catalog.Get(new PdfName("a"
				))).GetRefersTo(true));
			document.Close();
		}

		private sealed class _Dictionary_39 : Dictionary<PdfName, PdfObject>
		{
			public _Dictionary_39()
			{
				{
					this[new PdfName("b")] = new PdfName("c");
				}
			}
		}

		[Test]
		public virtual void IndirectsChain3()
		{
			MemoryStream baos = new MemoryStream();
			PdfWriter writer = new PdfWriter(baos);
			PdfDocument document = new PdfDocument(writer);
			document.AddNewPage();
			PdfDictionary catalog = document.GetCatalog().GetPdfObject();
			PdfDictionary dictionary = new PdfDictionary(new _Dictionary_59());
			PdfObject @object = dictionary;
			for (int i = 0; i < 31; i++)
			{
				@object = @object.MakeIndirect(document).GetIndirectReference();
			}
			catalog.Put(new PdfName("a"), @object);
			@object = catalog.Get(new PdfName("a"), true);
			NUnit.Framework.Assert.IsTrue(@object is PdfDictionary);
			NUnit.Framework.Assert.AreEqual(new PdfName("c").ToString(), ((PdfDictionary)@object
				).Get(new PdfName("b")).ToString());
			document.Close();
		}

		private sealed class _Dictionary_59 : Dictionary<PdfName, PdfObject>
		{
			public _Dictionary_59()
			{
				{
					this[new PdfName("b")] = new PdfName("c");
				}
			}
		}

		[Test]
		public virtual void IndirectsChain4()
		{
			MemoryStream baos = new MemoryStream();
			PdfWriter writer = new PdfWriter(baos);
			PdfDocument document = new PdfDocument(writer);
			document.AddNewPage();
			PdfDictionary catalog = document.GetCatalog().GetPdfObject();
			PdfDictionary dictionary = new PdfDictionary(new _Dictionary_80());
			PdfObject @object = dictionary;
			for (int i = 0; i < 31; i++)
			{
				@object = @object.MakeIndirect(document).GetIndirectReference();
			}
			PdfArray array = new PdfArray();
			array.Add(@object);
			catalog.Put(new PdfName("a"), array);
			@object = ((PdfArray)catalog.Get(new PdfName("a"))).Get(0, true);
			NUnit.Framework.Assert.IsTrue(@object is PdfDictionary);
			NUnit.Framework.Assert.AreEqual(new PdfName("c").ToString(), ((PdfDictionary)@object
				).Get(new PdfName("b")).ToString());
			document.Close();
		}

		private sealed class _Dictionary_80 : Dictionary<PdfName, PdfObject>
		{
			public _Dictionary_80()
			{
				{
					this[new PdfName("b")] = new PdfName("c");
				}
			}
		}

		[Test]
		public virtual void PdfIndirectReferenceFlags()
		{
			PdfIndirectReference reference = new PdfIndirectReference(null, 1);
			reference.SetState(PdfObject.FREE);
			reference.SetState(PdfObject.READING);
			reference.SetState(PdfObject.MODIFIED);
			NUnit.Framework.Assert.AreEqual(true, reference.CheckState(PdfObject.FREE), "Free"
				);
			NUnit.Framework.Assert.AreEqual(true, reference.CheckState(PdfObject.READING), "Reading"
				);
			NUnit.Framework.Assert.AreEqual(true, reference.CheckState(PdfObject.MODIFIED), "Modified"
				);
			NUnit.Framework.Assert.AreEqual(true, reference.CheckState((byte)(PdfObject.FREE 
				| PdfObject.MODIFIED | PdfObject.READING)), "Free|Reading|Modified");
			reference.ClearState(PdfObject.FREE);
			NUnit.Framework.Assert.AreEqual(false, reference.CheckState(PdfObject.FREE), "Free"
				);
			NUnit.Framework.Assert.AreEqual(true, reference.CheckState(PdfObject.READING), "Reading"
				);
			NUnit.Framework.Assert.AreEqual(true, reference.CheckState(PdfObject.MODIFIED), "Modified"
				);
			NUnit.Framework.Assert.AreEqual(true, reference.CheckState((byte)(PdfObject.READING
				 | PdfObject.MODIFIED)), "Reading|Modified");
			NUnit.Framework.Assert.AreEqual(false, reference.CheckState((byte)(PdfObject.FREE
				 | PdfObject.READING | PdfObject.MODIFIED)), "Free|Reading|Modified");
			reference.ClearState(PdfObject.READING);
			NUnit.Framework.Assert.AreEqual(false, reference.CheckState(PdfObject.FREE), "Free"
				);
			NUnit.Framework.Assert.AreEqual(false, reference.CheckState(PdfObject.READING), "Reading"
				);
			NUnit.Framework.Assert.AreEqual(true, reference.CheckState(PdfObject.MODIFIED), "Modified"
				);
			NUnit.Framework.Assert.AreEqual(false, reference.CheckState((byte)(PdfObject.FREE
				 | PdfObject.READING)), "Free|Reading");
			reference.ClearState(PdfObject.MODIFIED);
			NUnit.Framework.Assert.AreEqual(false, reference.CheckState(PdfObject.FREE), "Free"
				);
			NUnit.Framework.Assert.AreEqual(false, reference.CheckState(PdfObject.READING), "Reading"
				);
			NUnit.Framework.Assert.AreEqual(false, reference.CheckState(PdfObject.MODIFIED), 
				"Modified");
			NUnit.Framework.Assert.AreEqual(true, !reference.IsFree(), "Is InUse");
			reference.SetState(PdfObject.FREE);
			NUnit.Framework.Assert.AreEqual(false, !reference.IsFree(), "Not IsInUse");
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void PdtIndirectReferenceLateInitializing1()
		{
			MemoryStream baos = new MemoryStream();
			PdfWriter writer = new PdfWriter(baos);
			PdfDocument document = new PdfDocument(writer);
			document.AddNewPage();
			PdfDictionary catalog = document.GetCatalog().GetPdfObject();
			PdfIndirectReference indRef = document.CreateNextIndirectReference();
			catalog.Put(new PdfName("Smth"), indRef);
			PdfDictionary dictionary = new PdfDictionary();
			dictionary.Put(new PdfName("A"), new PdfString("a"));
			dictionary.MakeIndirect(document, indRef);
			document.Close();
			MemoryStream bais = new MemoryStream(baos.ToArray());
			PdfReader reader = new PdfReader(bais);
			document = new PdfDocument(reader);
			PdfObject @object = document.GetCatalog().GetPdfObject().Get(new PdfName("Smth"));
			NUnit.Framework.Assert.IsTrue(@object is PdfDictionary);
			dictionary = (PdfDictionary)@object;
			PdfString a = (PdfString)dictionary.Get(new PdfName("A"));
			NUnit.Framework.Assert.IsTrue(a.GetValue().Equals("a"));
			document.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void PdtIndirectReferenceLateInitializing2()
		{
			MemoryStream baos = new MemoryStream();
			PdfWriter writer = new PdfWriter(baos);
			PdfDocument document = new PdfDocument(writer);
			document.AddNewPage();
			PdfDictionary catalog = document.GetCatalog().GetPdfObject();
			PdfIndirectReference indRef1 = document.CreateNextIndirectReference();
			PdfIndirectReference indRef2 = document.CreateNextIndirectReference();
			catalog.Put(new PdfName("Smth1"), indRef1);
			catalog.Put(new PdfName("Smth2"), indRef2);
			PdfArray array = new PdfArray();
			array.Add(new PdfString("array string"));
			array.MakeIndirect(document, indRef2);
			document.Close();
			MemoryStream bais = new MemoryStream(baos.ToArray());
			PdfReader reader = new PdfReader(bais);
			document = new PdfDocument(reader);
			PdfDictionary catalogDict = document.GetCatalog().GetPdfObject();
			PdfObject object1 = catalogDict.Get(new PdfName("Smth1"));
			PdfObject object2 = catalogDict.Get(new PdfName("Smth2"));
			NUnit.Framework.Assert.IsTrue(object1 is PdfNull);
			NUnit.Framework.Assert.IsTrue(object2 is PdfArray);
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void PdtIndirectReferenceLateInitializing3()
		{
			MemoryStream baos = new MemoryStream();
			PdfWriter writer = new PdfWriter(baos);
			PdfDocument document = new PdfDocument(writer);
			document.AddNewPage();
			PdfDictionary catalog = document.GetCatalog().GetPdfObject();
			PdfIndirectReference indRef1 = document.CreateNextIndirectReference();
			PdfIndirectReference indRef2 = document.CreateNextIndirectReference();
			PdfArray array = new PdfArray();
			catalog.Put(new PdfName("array1"), array);
			PdfString @string = new PdfString("array string");
			array.Add(@string);
			array.Add(indRef1);
			array.Add(indRef2);
			PdfDictionary dict = new PdfDictionary();
			dict.MakeIndirect(document, indRef1);
			PdfArray arrayClone = (PdfArray)array.Clone();
			PdfObject object0 = arrayClone.Get(0, false);
			PdfObject object1 = arrayClone.Get(1, false);
			PdfObject object2 = arrayClone.Get(2, false);
			NUnit.Framework.Assert.IsTrue(object0 is PdfString);
			NUnit.Framework.Assert.IsTrue(object1 is PdfDictionary);
			NUnit.Framework.Assert.IsTrue(object2 is PdfNull);
			PdfString string1 = (PdfString)object0;
			NUnit.Framework.Assert.IsTrue(@string != string1);
			NUnit.Framework.Assert.IsTrue(@string.GetValue().Equals(string1.GetValue()));
			PdfDictionary dict1 = (PdfDictionary)object1;
			NUnit.Framework.Assert.IsTrue(dict1.GetIndirectReference().GetObjNumber() == dict
				.GetIndirectReference().GetObjNumber());
			NUnit.Framework.Assert.IsTrue(dict1.GetIndirectReference().GetGenNumber() == dict
				.GetIndirectReference().GetGenNumber());
			NUnit.Framework.Assert.IsTrue(dict1 == dict);
			document.Close();
		}
	}
}
