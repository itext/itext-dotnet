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
using System.IO;
using System.Text;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>Test suite for character reader.</summary>
    public class CharacterReaderTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void Consume() {
            CharacterReader r = new CharacterReader("one");
            NUnit.Framework.Assert.AreEqual(0, r.Pos());
            NUnit.Framework.Assert.AreEqual('o', r.Current());
            NUnit.Framework.Assert.AreEqual('o', r.Consume());
            NUnit.Framework.Assert.AreEqual(1, r.Pos());
            NUnit.Framework.Assert.AreEqual('n', r.Current());
            NUnit.Framework.Assert.AreEqual(1, r.Pos());
            NUnit.Framework.Assert.AreEqual('n', r.Consume());
            NUnit.Framework.Assert.AreEqual('e', r.Consume());
            NUnit.Framework.Assert.IsTrue(r.IsEmpty());
            NUnit.Framework.Assert.AreEqual(CharacterReader.EOF, r.Consume());
            NUnit.Framework.Assert.IsTrue(r.IsEmpty());
            NUnit.Framework.Assert.AreEqual(CharacterReader.EOF, r.Consume());
        }

        [NUnit.Framework.Test]
        public virtual void Unconsume() {
            CharacterReader r = new CharacterReader("one");
            NUnit.Framework.Assert.AreEqual('o', r.Consume());
            NUnit.Framework.Assert.AreEqual('n', r.Current());
            r.Unconsume();
            NUnit.Framework.Assert.AreEqual('o', r.Current());
            NUnit.Framework.Assert.AreEqual('o', r.Consume());
            NUnit.Framework.Assert.AreEqual('n', r.Consume());
            NUnit.Framework.Assert.AreEqual('e', r.Consume());
            NUnit.Framework.Assert.IsTrue(r.IsEmpty());
            r.Unconsume();
            NUnit.Framework.Assert.IsFalse(r.IsEmpty());
            NUnit.Framework.Assert.AreEqual('e', r.Current());
            NUnit.Framework.Assert.AreEqual('e', r.Consume());
            NUnit.Framework.Assert.IsTrue(r.IsEmpty());
            NUnit.Framework.Assert.AreEqual(CharacterReader.EOF, r.Consume());
            r.Unconsume();
            // read past, so have to eat again
            NUnit.Framework.Assert.IsTrue(r.IsEmpty());
            r.Unconsume();
            NUnit.Framework.Assert.IsFalse(r.IsEmpty());
            NUnit.Framework.Assert.AreEqual('e', r.Consume());
            NUnit.Framework.Assert.IsTrue(r.IsEmpty());
            NUnit.Framework.Assert.AreEqual(CharacterReader.EOF, r.Consume());
            NUnit.Framework.Assert.IsTrue(r.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void Mark() {
            CharacterReader r = new CharacterReader("one");
            r.Consume();
            r.Mark();
            NUnit.Framework.Assert.AreEqual(1, r.Pos());
            NUnit.Framework.Assert.AreEqual('n', r.Consume());
            NUnit.Framework.Assert.AreEqual('e', r.Consume());
            NUnit.Framework.Assert.IsTrue(r.IsEmpty());
            r.RewindToMark();
            NUnit.Framework.Assert.AreEqual(1, r.Pos());
            NUnit.Framework.Assert.AreEqual('n', r.Consume());
            NUnit.Framework.Assert.IsFalse(r.IsEmpty());
            NUnit.Framework.Assert.AreEqual(2, r.Pos());
        }

        [NUnit.Framework.Test]
        public virtual void ConsumeToEnd() {
            String @in = "one two three";
            CharacterReader r = new CharacterReader(@in);
            String toEnd = r.ConsumeToEnd();
            NUnit.Framework.Assert.AreEqual(@in, toEnd);
            NUnit.Framework.Assert.IsTrue(r.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void NextIndexOfChar() {
            String @in = "blah blah";
            CharacterReader r = new CharacterReader(@in);
            NUnit.Framework.Assert.AreEqual(-1, r.NextIndexOf('x'));
            NUnit.Framework.Assert.AreEqual(3, r.NextIndexOf('h'));
            String pull = r.ConsumeTo('h');
            NUnit.Framework.Assert.AreEqual("bla", pull);
            r.Consume();
            NUnit.Framework.Assert.AreEqual(2, r.NextIndexOf('l'));
            NUnit.Framework.Assert.AreEqual(" blah", r.ConsumeToEnd());
            NUnit.Framework.Assert.AreEqual(-1, r.NextIndexOf('x'));
        }

        [NUnit.Framework.Test]
        public virtual void NextIndexOfString() {
            String @in = "One Two something Two Three Four";
            CharacterReader r = new CharacterReader(@in);
            NUnit.Framework.Assert.AreEqual(-1, r.NextIndexOf("Foo"));
            NUnit.Framework.Assert.AreEqual(4, r.NextIndexOf("Two"));
            NUnit.Framework.Assert.AreEqual("One Two ", r.ConsumeTo("something"));
            NUnit.Framework.Assert.AreEqual(10, r.NextIndexOf("Two"));
            NUnit.Framework.Assert.AreEqual("something Two Three Four", r.ConsumeToEnd());
            NUnit.Framework.Assert.AreEqual(-1, r.NextIndexOf("Two"));
        }

        [NUnit.Framework.Test]
        public virtual void NextIndexOfUnmatched() {
            CharacterReader r = new CharacterReader("<[[one]]");
            NUnit.Framework.Assert.AreEqual(-1, r.NextIndexOf("]]>"));
        }

        [NUnit.Framework.Test]
        public virtual void ConsumeToChar() {
            CharacterReader r = new CharacterReader("One Two Three");
            NUnit.Framework.Assert.AreEqual("One ", r.ConsumeTo('T'));
            NUnit.Framework.Assert.AreEqual("", r.ConsumeTo('T'));
            // on Two
            NUnit.Framework.Assert.AreEqual('T', r.Consume());
            NUnit.Framework.Assert.AreEqual("wo ", r.ConsumeTo('T'));
            NUnit.Framework.Assert.AreEqual('T', r.Consume());
            NUnit.Framework.Assert.AreEqual("hree", r.ConsumeTo('T'));
        }

        // consume to end
        [NUnit.Framework.Test]
        public virtual void ConsumeToString() {
            CharacterReader r = new CharacterReader("One Two Two Four");
            NUnit.Framework.Assert.AreEqual("One ", r.ConsumeTo("Two"));
            NUnit.Framework.Assert.AreEqual('T', r.Consume());
            NUnit.Framework.Assert.AreEqual("wo ", r.ConsumeTo("Two"));
            NUnit.Framework.Assert.AreEqual('T', r.Consume());
            // To handle strings straddling across buffers, consumeTo() may return the
            // data in multiple pieces near EOF.
            StringBuilder builder = new StringBuilder();
            String part;
            do {
                part = r.ConsumeTo("Qux");
                builder.Append(part);
            }
            while (!String.IsNullOrEmpty(part));
            NUnit.Framework.Assert.AreEqual("wo Four", builder.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Advance() {
            CharacterReader r = new CharacterReader("One Two Three");
            NUnit.Framework.Assert.AreEqual('O', r.Consume());
            r.Advance();
            NUnit.Framework.Assert.AreEqual('e', r.Consume());
        }

        [NUnit.Framework.Test]
        public virtual void ConsumeToAny() {
            CharacterReader r = new CharacterReader("One &bar; qux");
            NUnit.Framework.Assert.AreEqual("One ", r.ConsumeToAny('&', ';'));
            NUnit.Framework.Assert.IsTrue(r.Matches('&'));
            NUnit.Framework.Assert.IsTrue(r.Matches("&bar;"));
            NUnit.Framework.Assert.AreEqual('&', r.Consume());
            NUnit.Framework.Assert.AreEqual("bar", r.ConsumeToAny('&', ';'));
            NUnit.Framework.Assert.AreEqual(';', r.Consume());
            NUnit.Framework.Assert.AreEqual(" qux", r.ConsumeToAny('&', ';'));
        }

        [NUnit.Framework.Test]
        public virtual void ConsumeLetterSequence() {
            CharacterReader r = new CharacterReader("One &bar; qux");
            NUnit.Framework.Assert.AreEqual("One", r.ConsumeLetterSequence());
            NUnit.Framework.Assert.AreEqual(" &", r.ConsumeTo("bar;"));
            NUnit.Framework.Assert.AreEqual("bar", r.ConsumeLetterSequence());
            NUnit.Framework.Assert.AreEqual("; qux", r.ConsumeToEnd());
        }

        [NUnit.Framework.Test]
        public virtual void ConsumeLetterThenDigitSequence() {
            CharacterReader r = new CharacterReader("One12 Two &bar; qux");
            NUnit.Framework.Assert.AreEqual("One12", r.ConsumeLetterThenDigitSequence());
            NUnit.Framework.Assert.AreEqual(' ', r.Consume());
            NUnit.Framework.Assert.AreEqual("Two", r.ConsumeLetterThenDigitSequence());
            NUnit.Framework.Assert.AreEqual(" &bar; qux", r.ConsumeToEnd());
        }

        [NUnit.Framework.Test]
        public virtual void Matches() {
            CharacterReader r = new CharacterReader("One Two Three");
            NUnit.Framework.Assert.IsTrue(r.Matches('O'));
            NUnit.Framework.Assert.IsTrue(r.Matches("One Two Three"));
            NUnit.Framework.Assert.IsTrue(r.Matches("One"));
            NUnit.Framework.Assert.IsFalse(r.Matches("one"));
            NUnit.Framework.Assert.AreEqual('O', r.Consume());
            NUnit.Framework.Assert.IsFalse(r.Matches("One"));
            NUnit.Framework.Assert.IsTrue(r.Matches("ne Two Three"));
            NUnit.Framework.Assert.IsFalse(r.Matches("ne Two Three Four"));
            NUnit.Framework.Assert.AreEqual("ne Two Three", r.ConsumeToEnd());
            NUnit.Framework.Assert.IsFalse(r.Matches("ne"));
            NUnit.Framework.Assert.IsTrue(r.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void MatchesIgnoreCase() {
            CharacterReader r = new CharacterReader("One Two Three");
            NUnit.Framework.Assert.IsTrue(r.MatchesIgnoreCase("O"));
            NUnit.Framework.Assert.IsTrue(r.MatchesIgnoreCase("o"));
            NUnit.Framework.Assert.IsTrue(r.Matches('O'));
            NUnit.Framework.Assert.IsFalse(r.Matches('o'));
            NUnit.Framework.Assert.IsTrue(r.MatchesIgnoreCase("One Two Three"));
            NUnit.Framework.Assert.IsTrue(r.MatchesIgnoreCase("ONE two THREE"));
            NUnit.Framework.Assert.IsTrue(r.MatchesIgnoreCase("One"));
            NUnit.Framework.Assert.IsTrue(r.MatchesIgnoreCase("one"));
            NUnit.Framework.Assert.AreEqual('O', r.Consume());
            NUnit.Framework.Assert.IsFalse(r.MatchesIgnoreCase("One"));
            NUnit.Framework.Assert.IsTrue(r.MatchesIgnoreCase("NE Two Three"));
            NUnit.Framework.Assert.IsFalse(r.MatchesIgnoreCase("ne Two Three Four"));
            NUnit.Framework.Assert.AreEqual("ne Two Three", r.ConsumeToEnd());
            NUnit.Framework.Assert.IsFalse(r.MatchesIgnoreCase("ne"));
        }

        [NUnit.Framework.Test]
        public virtual void ContainsIgnoreCase() {
            CharacterReader r = new CharacterReader("One TWO three");
            NUnit.Framework.Assert.IsTrue(r.ContainsIgnoreCase("two"));
            NUnit.Framework.Assert.IsTrue(r.ContainsIgnoreCase("three"));
            // weird one: does not find one, because it scans for consistent case only
            NUnit.Framework.Assert.IsFalse(r.ContainsIgnoreCase("one"));
        }

        [NUnit.Framework.Test]
        public virtual void MatchesAny() {
            char[] scan = new char[] { ' ', '\n', '\t' };
            CharacterReader r = new CharacterReader("One\nTwo\tThree");
            NUnit.Framework.Assert.IsFalse(r.MatchesAny(scan));
            NUnit.Framework.Assert.AreEqual("One", r.ConsumeToAny(scan));
            NUnit.Framework.Assert.IsTrue(r.MatchesAny(scan));
            NUnit.Framework.Assert.AreEqual('\n', r.Consume());
            NUnit.Framework.Assert.IsFalse(r.MatchesAny(scan));
        }

        [NUnit.Framework.Test]
        public virtual void CachesStrings() {
            CharacterReader r = new CharacterReader("Check\tCheck\tCheck\tCHOKE\tA string that is longer than 16 chars"
                );
            String one = r.ConsumeTo('\t');
            r.Consume();
            String two = r.ConsumeTo('\t');
            r.Consume();
            String three = r.ConsumeTo('\t');
            r.Consume();
            String four = r.ConsumeTo('\t');
            r.Consume();
            String five = r.ConsumeTo('\t');
            NUnit.Framework.Assert.AreEqual("Check", one);
            NUnit.Framework.Assert.AreEqual("Check", two);
            NUnit.Framework.Assert.AreEqual("Check", three);
            NUnit.Framework.Assert.AreEqual("CHOKE", four);
            NUnit.Framework.Assert.AreSame(one, two);
            NUnit.Framework.Assert.AreSame(two, three);
            NUnit.Framework.Assert.AreNotSame(three, four);
            NUnit.Framework.Assert.AreNotSame(four, five);
            NUnit.Framework.Assert.AreEqual(five, "A string that is longer than 16 chars");
        }

        [NUnit.Framework.Test]
        public virtual void RangeEquals() {
            CharacterReader r = new CharacterReader("Check\tCheck\tCheck\tCHOKE");
            NUnit.Framework.Assert.IsTrue(r.RangeEquals(0, 5, "Check"));
            NUnit.Framework.Assert.IsFalse(r.RangeEquals(0, 5, "CHOKE"));
            NUnit.Framework.Assert.IsFalse(r.RangeEquals(0, 5, "Chec"));
            NUnit.Framework.Assert.IsTrue(r.RangeEquals(6, 5, "Check"));
            NUnit.Framework.Assert.IsFalse(r.RangeEquals(6, 5, "Chuck"));
            NUnit.Framework.Assert.IsTrue(r.RangeEquals(12, 5, "Check"));
            NUnit.Framework.Assert.IsFalse(r.RangeEquals(12, 5, "Cheeky"));
            NUnit.Framework.Assert.IsTrue(r.RangeEquals(18, 5, "CHOKE"));
            NUnit.Framework.Assert.IsFalse(r.RangeEquals(18, 5, "CHIKE"));
        }

        [NUnit.Framework.Test]
        public virtual void Empty() {
            CharacterReader r = new CharacterReader("One");
            NUnit.Framework.Assert.IsTrue(r.MatchConsume("One"));
            NUnit.Framework.Assert.IsTrue(r.IsEmpty());
            r = new CharacterReader("Two");
            String two = r.ConsumeToEnd();
            NUnit.Framework.Assert.AreEqual("Two", two);
        }

        [NUnit.Framework.Test]
        public virtual void ConsumeToNonexistentEndWhenAtAnd() {
            CharacterReader r = new CharacterReader("<!");
            NUnit.Framework.Assert.IsTrue(r.MatchConsume("<!"));
            NUnit.Framework.Assert.IsTrue(r.IsEmpty());
            String after = r.ConsumeTo('>');
            NUnit.Framework.Assert.AreEqual("", after);
            NUnit.Framework.Assert.IsTrue(r.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void NotEmptyAtBufferSplitPoint() {
            CharacterReader r = new CharacterReader(new StringReader("How about now"), 3);
            NUnit.Framework.Assert.AreEqual("How", r.ConsumeTo(' '));
            NUnit.Framework.Assert.IsFalse(r.IsEmpty());
            NUnit.Framework.Assert.AreEqual(' ', r.Consume());
            NUnit.Framework.Assert.IsFalse(r.IsEmpty());
            NUnit.Framework.Assert.AreEqual(4, r.Pos());
            NUnit.Framework.Assert.AreEqual('a', r.Consume());
            NUnit.Framework.Assert.AreEqual(5, r.Pos());
            NUnit.Framework.Assert.AreEqual('b', r.Consume());
            NUnit.Framework.Assert.AreEqual('o', r.Consume());
            NUnit.Framework.Assert.AreEqual('u', r.Consume());
            NUnit.Framework.Assert.AreEqual('t', r.Consume());
            NUnit.Framework.Assert.AreEqual(' ', r.Consume());
            NUnit.Framework.Assert.AreEqual('n', r.Consume());
            NUnit.Framework.Assert.AreEqual('o', r.Consume());
            NUnit.Framework.Assert.AreEqual('w', r.Consume());
            NUnit.Framework.Assert.IsTrue(r.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void BufferUp() {
            String note = "HelloThere";
            // + ! = 11 chars
            int loopCount = 64;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < loopCount; i++) {
                sb.Append(note);
                sb.Append("!");
            }
            String s = sb.ToString();
            CharacterReader r = new CharacterReader(new StringReader(s));
            for (int i = 0; i < loopCount; i++) {
                String pull = r.ConsumeTo('!');
                NUnit.Framework.Assert.AreEqual(note, pull);
                NUnit.Framework.Assert.AreEqual('!', r.Current());
                r.Advance();
            }
            NUnit.Framework.Assert.IsTrue(r.IsEmpty());
        }
    }
}
