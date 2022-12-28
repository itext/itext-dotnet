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
using iText.Svg.Dummy.Renderers.Impl;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Processors {
    [NUnit.Framework.Category("UnitTest")]
    public class ProcessorStateTest : ExtendedITextTest {
        /// <summary>Push test</summary>
        [NUnit.Framework.Test]
        public virtual void ProcessorStateTestPush() {
            ProcessorState testProcessorState = new ProcessorState();
            ISvgNodeRenderer renderer = new DummySvgNodeRenderer("test");
            testProcessorState.Push(renderer);
            NUnit.Framework.Assert.IsTrue(testProcessorState.Size() == 1);
        }

        /// <summary>Pop test</summary>
        [NUnit.Framework.Test]
        public virtual void ProcessorStateTestPop() {
            ProcessorState testProcessorState = new ProcessorState();
            ISvgNodeRenderer renderer = new DummySvgNodeRenderer("test");
            testProcessorState.Push(renderer);
            ISvgNodeRenderer popped = testProcessorState.Pop();
            NUnit.Framework.Assert.IsTrue(popped.ToString().Equals("test") && testProcessorState.Empty());
        }

        /// <summary>Peek test</summary>
        [NUnit.Framework.Test]
        public virtual void ProcessorStateTestPeek() {
            ProcessorState testProcessorState = new ProcessorState();
            ISvgNodeRenderer renderer = new DummySvgNodeRenderer("test");
            testProcessorState.Push(renderer);
            ISvgNodeRenderer viewed = testProcessorState.Top();
            NUnit.Framework.Assert.IsTrue(viewed.ToString().Equals("test") && !testProcessorState.Empty());
        }

        /// <summary>Multiple push test</summary>
        [NUnit.Framework.Test]
        public virtual void ProcessorStateTestMultiplePushesPopAndPeek() {
            ProcessorState testProcessorState = new ProcessorState();
            ISvgNodeRenderer rendererOne = new DummySvgNodeRenderer("test01");
            testProcessorState.Push(rendererOne);
            ISvgNodeRenderer rendererTwo = new DummySvgNodeRenderer("test02");
            testProcessorState.Push(rendererTwo);
            ISvgNodeRenderer popped = testProcessorState.Pop();
            bool result = popped.ToString().Equals("test02");
            result = result && testProcessorState.Top().ToString().Equals("test01");
            NUnit.Framework.Assert.IsTrue(result);
        }

        [NUnit.Framework.Test]
        public virtual void ProcessorStateTestPopEmpty() {
            ProcessorState testProcessorState = new ProcessorState();
            NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => testProcessorState.Pop());
        }

        [NUnit.Framework.Test]
        public virtual void ProcessorStateTestPushSameElementTwice() {
            ProcessorState testProcessorState = new ProcessorState();
            ISvgNodeRenderer rendererOne = new DummySvgNodeRenderer("test01");
            testProcessorState.Push(rendererOne);
            testProcessorState.Push(rendererOne);
            ISvgNodeRenderer popped = testProcessorState.Pop();
            bool result = popped.ToString().Equals("test01");
            result = result && testProcessorState.Top().ToString().Equals("test01");
            NUnit.Framework.Assert.IsTrue(result);
        }

        [NUnit.Framework.Test]
        public virtual void ProcessorStateTestPeekEmpty() {
            ProcessorState testProcessorState = new ProcessorState();
            NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => testProcessorState.Pop());
        }
    }
}
