/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using iText.Commons.Actions;
using iText.Commons.Utils;
using iText.Test;

namespace iText.Commons.Actions.Contexts {
    [NUnit.Framework.Category("UnitTest")]
    public class ContextManagerTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void GetRecognisedNamespaceForSpecificNamespaceTest() {
            String outerNamespaces = NamespaceConstant.ITEXT.ToLowerInvariant();
            String innerNamespaces = NamespaceConstant.PDF_HTML.ToLowerInvariant();
            NUnit.Framework.Assert.IsTrue(innerNamespaces.StartsWith(outerNamespaces));
            ContextManager managerOuterBeforeInner = new ContextManager();
            managerOuterBeforeInner.RegisterGenericContext(JavaCollectionsUtil.SingletonList(outerNamespaces), JavaCollectionsUtil
                .EmptyList<String>());
            managerOuterBeforeInner.RegisterGenericContext(JavaCollectionsUtil.SingletonList(innerNamespaces), JavaCollectionsUtil
                .EmptyList<String>());
            NUnit.Framework.Assert.AreEqual(outerNamespaces, managerOuterBeforeInner.GetRecognisedNamespace(outerNamespaces
                ));
            NUnit.Framework.Assert.AreEqual(innerNamespaces, managerOuterBeforeInner.GetRecognisedNamespace(innerNamespaces
                ));
            ContextManager managerInnerBeforeOuter = new ContextManager();
            managerInnerBeforeOuter.RegisterGenericContext(JavaCollectionsUtil.SingletonList(innerNamespaces), JavaCollectionsUtil
                .EmptyList<String>());
            managerInnerBeforeOuter.RegisterGenericContext(JavaCollectionsUtil.SingletonList(outerNamespaces), JavaCollectionsUtil
                .EmptyList<String>());
            NUnit.Framework.Assert.AreEqual(outerNamespaces, managerInnerBeforeOuter.GetRecognisedNamespace(outerNamespaces
                ));
            NUnit.Framework.Assert.AreEqual(innerNamespaces, managerInnerBeforeOuter.GetRecognisedNamespace(innerNamespaces
                ));
        }

        [NUnit.Framework.Test]
        public virtual void NotRegisteredNamespaceTest() {
            String notRegisteredNamespace = "com.hello.world";
            NUnit.Framework.Assert.IsNull(ContextManager.GetInstance().GetRecognisedNamespace(notRegisteredNamespace));
        }

        [NUnit.Framework.Test]
        public virtual void UnregisterNamespaceTest() {
            String testNamespace = "com.hello.world";
            String testNamespaceWithCapitals = "com.Bye.World";
            IList<String> testNamespaces = JavaUtil.ArraysAsList(testNamespace, testNamespaceWithCapitals);
            ContextManager manager = new ContextManager();
            NUnit.Framework.Assert.IsNull(manager.GetRecognisedNamespace(testNamespace));
            NUnit.Framework.Assert.IsNull(manager.GetRecognisedNamespace(testNamespaceWithCapitals));
            manager.RegisterGenericContext(testNamespaces, JavaUtil.ArraysAsList("myProduct"));
            NUnit.Framework.Assert.AreEqual(testNamespace, manager.GetRecognisedNamespace(testNamespace + ".MyClass"));
            NUnit.Framework.Assert.AreEqual(testNamespaceWithCapitals.ToLowerInvariant(), manager.GetRecognisedNamespace
                (testNamespaceWithCapitals + ".MyClass"));
            manager.UnregisterContext(testNamespaces);
            NUnit.Framework.Assert.IsNull(manager.GetRecognisedNamespace(testNamespace));
            NUnit.Framework.Assert.IsNull(manager.GetRecognisedNamespace(testNamespaceWithCapitals));
        }

        [NUnit.Framework.Test]
        public virtual void RegisteredNamespaceTest() {
            String registeredNamespace = NamespaceConstant.CORE_LAYOUT + "custompackage";
            NUnit.Framework.Assert.AreEqual(NamespaceConstant.CORE_LAYOUT.ToLowerInvariant(), ContextManager.GetInstance
                ().GetRecognisedNamespace(registeredNamespace));
        }
    }
}
