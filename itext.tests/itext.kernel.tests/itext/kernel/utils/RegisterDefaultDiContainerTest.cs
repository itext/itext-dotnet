/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Commons.Datastructures;
using iText.IO.Source;
using iText.Kernel.DI.Pagetree;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Utils {
    [NUnit.Framework.Category("UnitTest")]
    public class RegisterDefaultDiContainerTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void Test() {
            RegisterDefaultDiContainer registerDefaultDiContainer = new RegisterDefaultDiContainer();
            NUnit.Framework.Assert.IsNotNull(registerDefaultDiContainer);
        }

        [NUnit.Framework.Test]
        public virtual void TestStaticBlock() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            NUnit.Framework.Assert.IsTrue(pdfDocument.GetDiContainer().GetInstance<IPageTreeListFactory>() is IPageTreeListFactory
                );
        }

        [NUnit.Framework.Test]
        public virtual void TestWithOverWriting() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            NUnit.Framework.Assert.IsTrue(pdfDocument.GetDiContainer().GetInstance<IPageTreeListFactory>() is IPageTreeListFactory
                );
        }

        [NUnit.Framework.Test]
        public virtual void TestWithSettingDocumentProps() {
            DocumentProperties documentProperties = new DocumentProperties();
            documentProperties.RegisterDependency(typeof(IPageTreeListFactory), new RegisterDefaultDiContainerTest.IPageTreeTestImpl
                ());
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()), documentProperties);
            NUnit.Framework.Assert.IsTrue(pdfDocument.GetDiContainer().GetInstance<IPageTreeListFactory>() is RegisterDefaultDiContainerTest.IPageTreeTestImpl
                );
        }

        [NUnit.Framework.Test]
        public virtual void DocumentPropsSetWithNullInstance() {
            DocumentProperties documentProperties = new DocumentProperties();
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => {
                documentProperties.RegisterDependency(typeof(IPageTreeListFactory), null);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void DocumentPropsSetWithNullType() {
            DocumentProperties documentProperties = new DocumentProperties();
            Object dummyObject = new Object();
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => {
                documentProperties.RegisterDependency(null, dummyObject);
            }
            );
        }

//\cond DO_NOT_DOCUMENT
        internal sealed class IPageTreeTestImpl : IPageTreeListFactory {
            public ISimpleList<T> CreateList<T>(PdfDictionary pagesDictionary) {
                return null;
            }
        }
//\endcond
    }
}
