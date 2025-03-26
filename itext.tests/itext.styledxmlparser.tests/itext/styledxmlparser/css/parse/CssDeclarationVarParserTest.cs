using System;
using iText.Test;

namespace iText.StyledXmlParser.Css.Parse {
    [NUnit.Framework.Category("UnitTest")]
    public class CssDeclarationVarParserTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void SimpleVarTest() {
            String resolved = ParseVar("var(--simple)");
            NUnit.Framework.Assert.AreEqual("var(--simple)", resolved);
        }

        [NUnit.Framework.Test]
        public virtual void VarWithFallbackOnFunctionTest() {
            String resolved = ParseVar("1px      var(    --test  , calc('test str ' + var(--default, var(--test, 2px)))   )"
                );
            NUnit.Framework.Assert.AreEqual("var(    --test  , calc('test str ' + var(--default, var(--test, 2px)))   )"
                , resolved);
        }

        [NUnit.Framework.Test]
        public virtual void VarInsideFunctionTest() {
            String resolved = ParseVar("1px      calc('test str ' + var(--default, var(--test, 2px)))");
            NUnit.Framework.Assert.AreEqual("var(--default, var(--test, 2px))", resolved);
        }

        [NUnit.Framework.Test]
        public virtual void VarInsideFunctionInShorthandWithVarTest() {
            String resolved = ParseVar("1px      calc('test str ' + var(--default, var(--test, 2px)))   var(--dot, dotted)"
                );
            NUnit.Framework.Assert.AreEqual("var(--default, var(--test, 2px))", resolved);
        }

        [NUnit.Framework.Test]
        public virtual void SeveralVarInsideFunctionTest() {
            String resolved = ParseVar("calc(var(--two) + var(--one) + 20px)");
            NUnit.Framework.Assert.AreEqual("var(--two)", resolved);
        }

        [NUnit.Framework.Test]
        public virtual void SeveralVarInsideFunctionWithFallbackTest() {
            String resolved = ParseVar("calc(var(--two, var(--one, 55px)) + var(--one) + 20px)");
            NUnit.Framework.Assert.AreEqual("var(--two, var(--one, 55px))", resolved);
        }

        [NUnit.Framework.Test]
        public virtual void VarInCalculationsTest() {
            String resolved = ParseVar("calc(20px + var(--one) + 20px)");
            NUnit.Framework.Assert.AreEqual("var(--one)", resolved);
        }

        [NUnit.Framework.Test]
        public virtual void VarInCalcSimpleTest() {
            String resolved = ParseVar("calc(var(--one))");
            NUnit.Framework.Assert.AreEqual("var(--one)", resolved);
        }

        [NUnit.Framework.Test]
        public virtual void NestedVarInCalcTest() {
            String resolved = ParseVar("calc(var(--one, var(--two, 20px)))");
            NUnit.Framework.Assert.AreEqual("var(--one, var(--two, 20px))", resolved);
        }

        [NUnit.Framework.Test]
        public virtual void NestedVarInCalcWithSpacesTest() {
            String resolved = ParseVar("calc(    var(--one,       var(--two, 20px   )   )   )");
            NUnit.Framework.Assert.AreEqual("var(--one,       var(--two, 20px   )   )", resolved);
        }

        [NUnit.Framework.Test]
        public virtual void NestedVarsTest() {
            String resolved = ParseVar("calc('test' + 'test') dotted 1px var(var(--value), calc(var(--default) + \"test\")) 1px"
                );
            NUnit.Framework.Assert.AreEqual("var(var(--value), calc(var(--default) + \"test\"))", resolved);
        }

        [NUnit.Framework.Test]
        public virtual void VarInSingleQuotesTest() {
            String resolved = ParseVar("calc('test' + 'var(--value)') calc(var(--default))");
            NUnit.Framework.Assert.AreEqual("var(--default)", resolved);
        }

        [NUnit.Framework.Test]
        public virtual void VarInDoubleQuotesTest() {
            String resolved = ParseVar("calc('test' + \"var(--value)\") calc(var(--default))");
            NUnit.Framework.Assert.AreEqual("var(--default)", resolved);
        }

        private static String ParseVar(String expression) {
            return new CssDeclarationVarParser(expression).GetFirstValidVarToken().GetValue();
        }
    }
}
