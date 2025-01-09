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
using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Signatures {
    [NUnit.Framework.Category("UnitTest")]
    public class SignaturePermissionsTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void DefaultValuesTest() {
            SignaturePermissions permissions = new SignaturePermissions(new PdfDictionary(), null);
            NUnit.Framework.Assert.AreEqual(new List<Object>(), permissions.GetFieldLocks());
            NUnit.Framework.Assert.IsTrue(permissions.IsAnnotationsAllowed());
            NUnit.Framework.Assert.IsFalse(permissions.IsCertification());
            NUnit.Framework.Assert.IsTrue(permissions.IsFillInAllowed());
        }

        [NUnit.Framework.Test]
        public virtual void TransformedMethodDocMDPIsPresentedTest() {
            PdfDictionary dict = new PdfDictionary();
            PdfArray references = new PdfArray();
            PdfDictionary dictWithDocMDP = new PdfDictionary();
            dictWithDocMDP.Put(PdfName.TransformMethod, PdfName.DocMDP);
            dictWithDocMDP.Put(PdfName.TransformParams, new PdfDictionary());
            references.Add(dictWithDocMDP);
            dict.Put(PdfName.Reference, references);
            SignaturePermissions permissions = new SignaturePermissions(dict, null);
            NUnit.Framework.Assert.IsTrue(permissions.IsCertification());
            NUnit.Framework.Assert.AreEqual(new List<Object>(), permissions.GetFieldLocks());
            NUnit.Framework.Assert.IsTrue(permissions.IsAnnotationsAllowed());
            NUnit.Framework.Assert.IsTrue(permissions.IsFillInAllowed());
        }

        [NUnit.Framework.Test]
        public virtual void ActionIsPresentedTest() {
            PdfDictionary dict = new PdfDictionary();
            PdfArray references = new PdfArray();
            PdfDictionary dictWithAction = new PdfDictionary();
            PdfDictionary @params = new PdfDictionary();
            PdfName action = new PdfName("Name");
            PdfArray fields = new PdfArray();
            fields.Add(new PdfString("Value1"));
            fields.Add(new PdfString("Value2"));
            @params.Put(PdfName.Action, action);
            @params.Put(PdfName.Fields, fields);
            dictWithAction.Put(PdfName.TransformParams, @params);
            references.Add(dictWithAction);
            dict.Put(PdfName.Reference, references);
            SignaturePermissions permissions = new SignaturePermissions(dict, null);
            NUnit.Framework.Assert.AreEqual(1, permissions.GetFieldLocks().Count);
            SignaturePermissions.FieldLock fieldLock = permissions.GetFieldLocks()[0];
            NUnit.Framework.Assert.AreEqual(action, fieldLock.GetAction());
            NUnit.Framework.Assert.AreEqual(fields, fieldLock.GetFields());
            NUnit.Framework.Assert.IsTrue(permissions.IsAnnotationsAllowed());
            NUnit.Framework.Assert.IsFalse(permissions.IsCertification());
            NUnit.Framework.Assert.IsTrue(permissions.IsFillInAllowed());
        }

        [NUnit.Framework.Test]
        public virtual void MultipleActionsArePresentedTest() {
            PdfDictionary dict = new PdfDictionary();
            PdfArray references = new PdfArray();
            PdfDictionary dictWithAction = new PdfDictionary();
            PdfDictionary @params = new PdfDictionary();
            PdfName action = new PdfName("Name");
            PdfArray fields = new PdfArray();
            fields.Add(new PdfString("Value1"));
            fields.Add(new PdfString("Value2"));
            @params.Put(PdfName.Action, action);
            @params.Put(PdfName.Fields, fields);
            dictWithAction.Put(PdfName.TransformParams, @params);
            references.Add(dictWithAction);
            references.Add(dictWithAction);
            references.Add(dictWithAction);
            dict.Put(PdfName.Reference, references);
            SignaturePermissions permissions = new SignaturePermissions(dict, null);
            NUnit.Framework.Assert.AreEqual(3, permissions.GetFieldLocks().Count);
            foreach (SignaturePermissions.FieldLock fieldLock in permissions.GetFieldLocks()) {
                NUnit.Framework.Assert.AreEqual(action, fieldLock.GetAction());
                NUnit.Framework.Assert.AreEqual(fields, fieldLock.GetFields());
            }
            NUnit.Framework.Assert.IsTrue(permissions.IsAnnotationsAllowed());
            NUnit.Framework.Assert.IsFalse(permissions.IsCertification());
            NUnit.Framework.Assert.IsTrue(permissions.IsFillInAllowed());
        }

        [NUnit.Framework.Test]
        public virtual void PParamEqualsTo1Test() {
            PdfDictionary dict = new PdfDictionary();
            PdfArray references = new PdfArray();
            PdfDictionary dictWithAction = new PdfDictionary();
            PdfDictionary @params = new PdfDictionary();
            @params.Put(PdfName.P, new PdfNumber(1));
            dictWithAction.Put(PdfName.TransformParams, @params);
            references.Add(dictWithAction);
            dict.Put(PdfName.Reference, references);
            SignaturePermissions permissions = new SignaturePermissions(dict, null);
            NUnit.Framework.Assert.IsFalse(permissions.IsFillInAllowed());
            NUnit.Framework.Assert.IsFalse(permissions.IsAnnotationsAllowed());
            NUnit.Framework.Assert.AreEqual(new List<Object>(), permissions.GetFieldLocks());
            NUnit.Framework.Assert.IsFalse(permissions.IsCertification());
        }

        [NUnit.Framework.Test]
        public virtual void PParamEqualsTo2Test() {
            PdfDictionary dict = new PdfDictionary();
            PdfArray references = new PdfArray();
            PdfDictionary dictWithAction = new PdfDictionary();
            PdfDictionary @params = new PdfDictionary();
            @params.Put(PdfName.P, new PdfNumber(2));
            dictWithAction.Put(PdfName.TransformParams, @params);
            references.Add(dictWithAction);
            dict.Put(PdfName.Reference, references);
            SignaturePermissions permissions = new SignaturePermissions(dict, null);
            NUnit.Framework.Assert.IsFalse(permissions.IsAnnotationsAllowed());
            NUnit.Framework.Assert.AreEqual(new List<Object>(), permissions.GetFieldLocks());
            NUnit.Framework.Assert.IsTrue(permissions.IsFillInAllowed());
            NUnit.Framework.Assert.IsFalse(permissions.IsCertification());
        }

        [NUnit.Framework.Test]
        public virtual void PreviousIsSetTest() {
            PdfDictionary previousDict = new PdfDictionary();
            PdfArray references = new PdfArray();
            PdfDictionary dictWithAction = new PdfDictionary();
            PdfDictionary @params = new PdfDictionary();
            @params.Put(PdfName.P, new PdfNumber(1));
            PdfName action = new PdfName("Name");
            PdfArray fields = new PdfArray();
            fields.Add(new PdfString("Value1"));
            fields.Add(new PdfString("Value2"));
            @params.Put(PdfName.Action, action);
            @params.Put(PdfName.Fields, fields);
            dictWithAction.Put(PdfName.TransformParams, @params);
            references.Add(dictWithAction);
            previousDict.Put(PdfName.Reference, references);
            SignaturePermissions previousPermissions = new SignaturePermissions(previousDict, null);
            SignaturePermissions permissions = new SignaturePermissions(new PdfDictionary(), previousPermissions);
            NUnit.Framework.Assert.AreEqual(1, permissions.GetFieldLocks().Count);
            SignaturePermissions.FieldLock fieldLock = permissions.GetFieldLocks()[0];
            NUnit.Framework.Assert.AreEqual(action, fieldLock.GetAction());
            NUnit.Framework.Assert.AreEqual(fields, fieldLock.GetFields());
            NUnit.Framework.Assert.IsFalse(permissions.IsAnnotationsAllowed());
            NUnit.Framework.Assert.IsFalse(permissions.IsCertification());
            NUnit.Framework.Assert.IsFalse(permissions.IsFillInAllowed());
        }
    }
}
