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
