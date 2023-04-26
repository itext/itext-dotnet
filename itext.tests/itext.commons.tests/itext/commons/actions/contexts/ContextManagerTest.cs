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
