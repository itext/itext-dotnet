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
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>Token queue tests.</summary>
    public class TokenQueueTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ChompBalanced() {
            TokenQueue tq = new TokenQueue(":contains(one (two) three) four");
            String pre = tq.ConsumeTo("(");
            String guts = tq.ChompBalanced('(', ')');
            String remainder = tq.Remainder();
            NUnit.Framework.Assert.AreEqual(":contains", pre);
            NUnit.Framework.Assert.AreEqual("one (two) three", guts);
            NUnit.Framework.Assert.AreEqual(" four", remainder);
        }

        [NUnit.Framework.Test]
        public virtual void ChompEscapedBalanced() {
            TokenQueue tq = new TokenQueue(":contains(one (two) \\( \\) \\) three) four");
            String pre = tq.ConsumeTo("(");
            String guts = tq.ChompBalanced('(', ')');
            String remainder = tq.Remainder();
            NUnit.Framework.Assert.AreEqual(":contains", pre);
            NUnit.Framework.Assert.AreEqual("one (two) \\( \\) \\) three", guts);
            NUnit.Framework.Assert.AreEqual("one (two) ( ) ) three", TokenQueue.Unescape(guts));
            NUnit.Framework.Assert.AreEqual(" four", remainder);
        }

        [NUnit.Framework.Test]
        public virtual void ChompBalancedMatchesAsMuchAsPossible() {
            TokenQueue tq = new TokenQueue("unbalanced(something(or another");
            tq.ConsumeTo("(");
            String match = tq.ChompBalanced('(', ')');
            NUnit.Framework.Assert.AreEqual("something(or another", match);
        }

        [NUnit.Framework.Test]
        public virtual void Unescape() {
            NUnit.Framework.Assert.AreEqual("one ( ) \\", TokenQueue.Unescape("one \\( \\) \\\\"));
        }

        [NUnit.Framework.Test]
        public virtual void ChompToIgnoreCase() {
            String t = "<textarea>one < two </TEXTarea>";
            TokenQueue tq = new TokenQueue(t);
            String data = tq.ChompToIgnoreCase("</textarea");
            NUnit.Framework.Assert.AreEqual("<textarea>one < two ", data);
            tq = new TokenQueue("<textarea> one two < three </oops>");
            data = tq.ChompToIgnoreCase("</textarea");
            NUnit.Framework.Assert.AreEqual("<textarea> one two < three </oops>", data);
        }

        [NUnit.Framework.Test]
        public virtual void AddFirst() {
            TokenQueue tq = new TokenQueue("One Two");
            tq.ConsumeWord();
            tq.AddFirst("Three");
            NUnit.Framework.Assert.AreEqual("Three Two", tq.Remainder());
        }
    }
}
