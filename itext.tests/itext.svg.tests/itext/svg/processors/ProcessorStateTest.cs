using System;
using iText.Svg.Dummy.Renderers.Impl;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;

namespace iText.Svg.Processors {
    public class ProcessorStateTest {
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
            NUnit.Framework.Assert.That(() =>  {
                ProcessorState testProcessorState = new ProcessorState();
                testProcessorState.Pop();
            }
            , NUnit.Framework.Throws.TypeOf<InvalidOperationException>());
;
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
            NUnit.Framework.Assert.That(() =>  {
                ProcessorState testProcessorState = new ProcessorState();
                testProcessorState.Pop();
            }
            , NUnit.Framework.Throws.TypeOf<InvalidOperationException>());
;
        }
    }
}
