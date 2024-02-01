/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using System.Text.RegularExpressions;
using iText.Commons.Utils;
using iText.Test;

namespace iText.IO.Util {
    /// <summary>
    /// At the moment there is no com.itextpdf.io.util.Matcher class in Java (as we use
    /// java.util.regex.Matcher), but there is one in C# that we are testing
    /// </summary>
    [NUnit.Framework.Category("UnitTest")]
    public class MatcherTest : ExtendedITextTest {
        private const String PATTERN_STRING = "(a+)(b+)?";

        private static readonly Regex PATTERN = iText.Commons.Utils.StringUtil.RegexCompile(PATTERN_STRING);

        private static readonly Regex FULL_MATCH_PATTERN = iText.Commons.Utils.StringUtil.RegexCompile("^" + PATTERN_STRING
             + "$");

        [NUnit.Framework.Test]
        public virtual void MatchesTest() {
            Matcher matched = iText.Commons.Utils.Matcher.Match(FULL_MATCH_PATTERN, "aaabbb");
            NUnit.Framework.Assert.IsTrue(matched.Matches());
            Matcher notMatched = iText.Commons.Utils.Matcher.Match(FULL_MATCH_PATTERN, "aaacbbb");
            NUnit.Framework.Assert.IsFalse(notMatched.Matches());
        }

        [NUnit.Framework.Test]
        public virtual void TwoGroupsFindTest() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "aabbcaaacc");
            NUnit.Framework.Assert.IsTrue(matcher.Find());
            NUnit.Framework.Assert.AreEqual(0, matcher.Start());
            NUnit.Framework.Assert.AreEqual(4, matcher.End());
            NUnit.Framework.Assert.AreEqual("aabb", matcher.Group());
            NUnit.Framework.Assert.AreEqual("aabb", matcher.Group(0));
            NUnit.Framework.Assert.AreEqual("aa", matcher.Group(1));
            NUnit.Framework.Assert.AreEqual("bb", matcher.Group(2));
            NUnit.Framework.Assert.IsTrue(matcher.Find());
            NUnit.Framework.Assert.AreEqual(5, matcher.Start());
            NUnit.Framework.Assert.AreEqual(8, matcher.End());
            NUnit.Framework.Assert.AreEqual("aaa", matcher.Group());
            NUnit.Framework.Assert.AreEqual("aaa", matcher.Group(0));
            NUnit.Framework.Assert.AreEqual("aaa", matcher.Group(1));
            NUnit.Framework.Assert.IsNull(matcher.Group(2));
            NUnit.Framework.Assert.IsFalse(matcher.Find());
        }

        [NUnit.Framework.Test]
        public virtual void TwoGroupsFindWithIndexTest() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "aabbcaaacc");
            NUnit.Framework.Assert.IsTrue(matcher.Find(6));
            NUnit.Framework.Assert.AreEqual(6, matcher.Start());
            NUnit.Framework.Assert.AreEqual(8, matcher.End());
            NUnit.Framework.Assert.AreEqual("aa", matcher.Group());
            NUnit.Framework.Assert.AreEqual("aa", matcher.Group(0));
            NUnit.Framework.Assert.AreEqual("aa", matcher.Group(1));
            NUnit.Framework.Assert.IsNull(matcher.Group(2));
            NUnit.Framework.Assert.IsFalse(matcher.Find());
            NUnit.Framework.Assert.IsFalse(matcher.Find(9));
        }

        [NUnit.Framework.Test]
        public virtual void StartBeforeSearchTest() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "aabb");
            NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => matcher.Start());
        }

        [NUnit.Framework.Test]
        public virtual void StartWhenFindFailsTest() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "aabb");
            while (matcher.Find()) {
            }
            NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => matcher.Start());
        }

        [NUnit.Framework.Test]
        public virtual void EndBeforeSearchTest() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "aabb");
            NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => matcher.End());
        }

        [NUnit.Framework.Test]
        public virtual void EndWhenFindFailsTest() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "aabb");
            while (matcher.Find()) {
            }
            NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => matcher.End());
        }

        [NUnit.Framework.Test]
        public virtual void GroupBeforeSearchTest() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "aabb");
            NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => matcher.Group());
        }

        [NUnit.Framework.Test]
        public virtual void GroupWhenFindFailsTest() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "aabb");
            while (matcher.Find()) {
            }
            NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => matcher.Group());
        }

        [NUnit.Framework.Test]
        public virtual void GroupWithIndexBeforeSearchTest() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "aabb");
            NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => matcher.Group(0));
        }

        [NUnit.Framework.Test]
        public virtual void GroupWithIndexWhenFindFailsTest() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "aabb");
            while (matcher.Find()) {
            }
            NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => matcher.Group(0));
        }

        [NUnit.Framework.Test]
        public virtual void GroupNegativeIndexTest() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "aabb");
            NUnit.Framework.Assert.IsTrue(matcher.Find());
            NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => matcher.Group(-1));
        }

        [NUnit.Framework.Test]
        public virtual void GroupIndexGraterThanGroupCountTest() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "aabb");
            NUnit.Framework.Assert.IsTrue(matcher.Find());
            NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => matcher.Group(3));
        }

        [NUnit.Framework.Test]
        public virtual void FindNegativeIndexTest() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "aabb");
            NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => matcher.Find(-1));
        }

        [NUnit.Framework.Test]
        public virtual void FindIndexGraterThanInputLengthTest() {
            String input = "aabb";
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, input);
            NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => matcher.Find(input.Length + 1));
        }

        [NUnit.Framework.Test]
        public virtual void FindIndexEqualInputLengthTest() {
            String input = "aabb";
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, input);
            NUnit.Framework.Assert.IsFalse(matcher.Find(input.Length));
        }

        [NUnit.Framework.Test]
        public virtual void MatchesFullyAndOnceTest() {
            String testPattern = "(\\d+)-(\\d+)?";
            String input = "5-15";
            Matcher matcher = iText.Commons.Utils.Matcher.Match(iText.Commons.Utils.StringUtil.RegexCompile(testPattern
                ), input);
            NUnit.Framework.Assert.IsTrue(matcher.Find());
            NUnit.Framework.Assert.AreEqual("5-15", matcher.Group(0));
            NUnit.Framework.Assert.AreEqual("5", matcher.Group(1));
            NUnit.Framework.Assert.AreEqual("15", matcher.Group(2));
            NUnit.Framework.Assert.IsFalse(matcher.Find());
        }

        [NUnit.Framework.Test]
        public virtual void MatchesOnceTest() {
            String testPattern = "(\\d+)-(\\d+)?";
            String input = "5-15-";
            Matcher matcher = iText.Commons.Utils.Matcher.Match(iText.Commons.Utils.StringUtil.RegexCompile(testPattern
                ), input);
            NUnit.Framework.Assert.IsTrue(matcher.Find());
            NUnit.Framework.Assert.AreEqual("5-15", matcher.Group(0));
            NUnit.Framework.Assert.AreEqual("5", matcher.Group(1));
            NUnit.Framework.Assert.AreEqual("15", matcher.Group(2));
            NUnit.Framework.Assert.IsFalse(matcher.Find());
        }

        [NUnit.Framework.Test]
        public virtual void MatchesTwiceTest() {
            String testPattern = "a*b";
            String input = "abb";
            Matcher matcher = iText.Commons.Utils.Matcher.Match(iText.Commons.Utils.StringUtil.RegexCompile(testPattern
                ), input);
            NUnit.Framework.Assert.IsTrue(matcher.Find());
            NUnit.Framework.Assert.AreEqual("ab", matcher.Group(0));
            NUnit.Framework.Assert.IsTrue(matcher.Find());
            NUnit.Framework.Assert.AreEqual("b", matcher.Group(0));
            NUnit.Framework.Assert.IsFalse(matcher.Find());
        }

        [NUnit.Framework.Test]
        public virtual void MatchesTwiceEmptyMatchTest() {
            String testPattern = "a*b*";
            String input = "abb";
            Matcher matcher = iText.Commons.Utils.Matcher.Match(iText.Commons.Utils.StringUtil.RegexCompile(testPattern
                ), input);
            NUnit.Framework.Assert.IsTrue(matcher.Find());
            NUnit.Framework.Assert.AreEqual("abb", matcher.Group(0));
            NUnit.Framework.Assert.IsTrue(matcher.Find());
            NUnit.Framework.Assert.AreEqual("", matcher.Group(0));
            NUnit.Framework.Assert.IsFalse(matcher.Find());
        }

        [NUnit.Framework.Test]
        public virtual void GroupOutOfBoundsTest() {
            String testPattern = "(\\d+)";
            String input = "123";
            Matcher matcher = iText.Commons.Utils.Matcher.Match(iText.Commons.Utils.StringUtil.RegexCompile(testPattern
                ), input);
            NUnit.Framework.Assert.IsTrue(matcher.Find());
            NUnit.Framework.Assert.AreEqual("123", matcher.Group(0));
            NUnit.Framework.Assert.AreEqual("123", matcher.Group(1));
            NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => matcher.Group(2));
        }

        [NUnit.Framework.Test]
        public virtual void GroupWhenNoMatchTest() {
            String testPattern = "(\\d+)";
            String input = "abc";
            Matcher matcher = iText.Commons.Utils.Matcher.Match(iText.Commons.Utils.StringUtil.RegexCompile(testPattern
                ), input);
            NUnit.Framework.Assert.IsFalse(matcher.Find());
            NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => matcher.Group(0));
        }

        [NUnit.Framework.Test]
        public virtual void AlternativeGroupsTest() {
            String testPattern = "((\\d+)|(ab))cd(a*)e";
            String input = "abcdefg";
            Matcher matcher = iText.Commons.Utils.Matcher.Match(iText.Commons.Utils.StringUtil.RegexCompile(testPattern
                ), input);
            NUnit.Framework.Assert.IsTrue(matcher.Find());
            NUnit.Framework.Assert.AreEqual("abcde", matcher.Group(0));
            NUnit.Framework.Assert.AreEqual("ab", matcher.Group(1));
            NUnit.Framework.Assert.IsNull(matcher.Group(2));
            NUnit.Framework.Assert.AreEqual("ab", matcher.Group(3));
            NUnit.Framework.Assert.AreEqual("", matcher.Group(4));
            NUnit.Framework.Assert.IsFalse(matcher.Find());
        }

        [NUnit.Framework.Test]
        public virtual void StartEndIndicesTest() {
            String testPattern = "cd";
            String input = "abcde";
            Matcher matcher = iText.Commons.Utils.Matcher.Match(iText.Commons.Utils.StringUtil.RegexCompile(testPattern
                ), input);
            NUnit.Framework.Assert.IsTrue(matcher.Find());
            NUnit.Framework.Assert.AreEqual(2, matcher.Start());
            NUnit.Framework.Assert.AreEqual(4, matcher.End());
        }

        [NUnit.Framework.Test]
        public virtual void StartIndexNotFoundTest() {
            String testPattern = "ef";
            String input = "abcde";
            Matcher matcher = iText.Commons.Utils.Matcher.Match(iText.Commons.Utils.StringUtil.RegexCompile(testPattern
                ), input);
            NUnit.Framework.Assert.IsFalse(matcher.Find());
            NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => matcher.Start());
        }

        [NUnit.Framework.Test]
        public virtual void EndIndexNotFoundTest() {
            String testPattern = "ef";
            String input = "abcde";
            Matcher matcher = iText.Commons.Utils.Matcher.Match(iText.Commons.Utils.StringUtil.RegexCompile(testPattern
                ), input);
            NUnit.Framework.Assert.IsFalse(matcher.Find());
            NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => matcher.End());
        }

        [NUnit.Framework.Test]
        public virtual void FindMatchStartingFromIndexTest() {
            String testPattern = "ab|bc";
            String input = "00abcde";
            int startIndex = 3;
            Matcher matcher = iText.Commons.Utils.Matcher.Match(iText.Commons.Utils.StringUtil.RegexCompile(testPattern
                ), input);
            NUnit.Framework.Assert.IsTrue(matcher.Find(startIndex));
            NUnit.Framework.Assert.AreEqual("bc", matcher.Group(0));
            NUnit.Framework.Assert.AreEqual(3, matcher.Start());
            NUnit.Framework.Assert.AreEqual(5, matcher.End());
            NUnit.Framework.Assert.IsFalse(matcher.Find());
        }

        [NUnit.Framework.Test]
        public virtual void FindNextMatchStartingFromIndexTest() {
            String testPattern = "ab|bc";
            String input = "ab00abcde";
            Matcher matcher = iText.Commons.Utils.Matcher.Match(iText.Commons.Utils.StringUtil.RegexCompile(testPattern
                ), input);
            NUnit.Framework.Assert.IsTrue(matcher.Find());
            int startIndex = 5;
            NUnit.Framework.Assert.IsTrue(matcher.Find(startIndex));
            NUnit.Framework.Assert.AreEqual("bc", matcher.Group(0));
            NUnit.Framework.Assert.AreEqual(5, matcher.Start());
            NUnit.Framework.Assert.AreEqual(7, matcher.End());
            NUnit.Framework.Assert.IsFalse(matcher.Find());
        }

        [NUnit.Framework.Test]
        public virtual void FindMatchStartingFromAfterInputStringTest() {
            String testPattern = "ab";
            String input = "cab";
            int startIndex = 3;
            Matcher matcher = iText.Commons.Utils.Matcher.Match(iText.Commons.Utils.StringUtil.RegexCompile(testPattern
                ), input);
            NUnit.Framework.Assert.IsFalse(matcher.Find(startIndex));
        }

        [NUnit.Framework.Test]
        public virtual void FindNextMatchStartingFromAfterInputStringTest() {
            String testPattern = "ab";
            String input = "abc";
            Matcher matcher = iText.Commons.Utils.Matcher.Match(iText.Commons.Utils.StringUtil.RegexCompile(testPattern
                ), input);
            NUnit.Framework.Assert.IsTrue(matcher.Find());
            int startIndex = 3;
            NUnit.Framework.Assert.IsFalse(matcher.Find(startIndex));
        }

        [NUnit.Framework.Test]
        public virtual void FindMatchStartingFromIndexOutOfBoundsTest() {
            String testPattern = "ab";
            String input = "cab";
            int startIndex = 4;
            Matcher matcher = iText.Commons.Utils.Matcher.Match(iText.Commons.Utils.StringUtil.RegexCompile(testPattern
                ), input);
            NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => matcher.Find(startIndex));
        }

        [NUnit.Framework.Test]
        public virtual void FindNextMatchStartingFromIndexOutOfBoundsTest() {
            String testPattern = "ab";
            String input = "cab";
            Matcher matcher = iText.Commons.Utils.Matcher.Match(iText.Commons.Utils.StringUtil.RegexCompile(testPattern
                ), input);
            NUnit.Framework.Assert.IsTrue(matcher.Find());
            int startIndex = 4;
            NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => matcher.Find(startIndex));
        }

        [NUnit.Framework.Test]
        public virtual void FindMatchStartingFromNegativeIndexTest() {
            String testPattern = "ab";
            String input = "cab";
            int startIndex = -1;
            Matcher matcher = iText.Commons.Utils.Matcher.Match(iText.Commons.Utils.StringUtil.RegexCompile(testPattern
                ), input);
            NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => matcher.Find(startIndex));
        }

        [NUnit.Framework.Test]
        public virtual void FindNextMatchStartingFromNegativeIndexTest() {
            String testPattern = "ab";
            String input = "cab";
            Matcher matcher = iText.Commons.Utils.Matcher.Match(iText.Commons.Utils.StringUtil.RegexCompile(testPattern
                ), input);
            NUnit.Framework.Assert.IsTrue(matcher.Find());
            int startIndex = -1;
            NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => matcher.Find(startIndex));
        }

        [NUnit.Framework.Test]
        public virtual void FindNextMatchStartingFromIndexContinuouslyTest() {
            String testPattern = "ab";
            String input = "cabbabcaba";
            int startIndex1 = 2;
            Matcher matcher = iText.Commons.Utils.Matcher.Match(iText.Commons.Utils.StringUtil.RegexCompile(testPattern
                ), input);
            NUnit.Framework.Assert.IsTrue(matcher.Find(startIndex1));
            NUnit.Framework.Assert.AreEqual(4, matcher.Start());
            NUnit.Framework.Assert.AreEqual(6, matcher.End());
            int startIndex2 = 7;
            NUnit.Framework.Assert.IsTrue(matcher.Find(startIndex2));
            NUnit.Framework.Assert.AreEqual(7, matcher.Start());
            NUnit.Framework.Assert.AreEqual(9, matcher.End());
            int startIndex3 = input.Length;
            NUnit.Framework.Assert.IsFalse(matcher.Find(startIndex3));
        }

        [NUnit.Framework.Test]
        public virtual void FindNextMatchStartingFromIndexMovingBackwardsTest() {
            String testPattern = "ab";
            String input = "cabbabcaba";
            int startIndex1 = 7;
            Matcher matcher = iText.Commons.Utils.Matcher.Match(iText.Commons.Utils.StringUtil.RegexCompile(testPattern
                ), input);
            NUnit.Framework.Assert.IsTrue(matcher.Find(startIndex1));
            NUnit.Framework.Assert.AreEqual(7, matcher.Start());
            NUnit.Framework.Assert.AreEqual(9, matcher.End());
            int startIndex2 = 4;
            NUnit.Framework.Assert.IsTrue(matcher.Find(startIndex2));
            NUnit.Framework.Assert.AreEqual(4, matcher.Start());
            NUnit.Framework.Assert.AreEqual(6, matcher.End());
            int startIndex3 = 1;
            NUnit.Framework.Assert.IsTrue(matcher.Find(startIndex3));
            NUnit.Framework.Assert.AreEqual(1, matcher.Start());
            NUnit.Framework.Assert.AreEqual(3, matcher.End());
            NUnit.Framework.Assert.IsTrue(matcher.Find());
            NUnit.Framework.Assert.AreEqual(4, matcher.Start());
            NUnit.Framework.Assert.AreEqual(6, matcher.End());
            int startIndex4 = input.Length;
            NUnit.Framework.Assert.IsFalse(matcher.Find(startIndex4));
        }

        [NUnit.Framework.Test]
        public virtual void MatchesSuccessAfterFindFinish() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "aaabbb");
            NUnit.Framework.Assert.IsTrue(matcher.Find());
            NUnit.Framework.Assert.IsFalse(matcher.Find());
            NUnit.Framework.Assert.IsTrue(matcher.Matches());
            NUnit.Framework.Assert.IsFalse(matcher.Find());
        }

        [NUnit.Framework.Test]
        public virtual void FindAfterMatchesSuccess() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "aaabbb");
            NUnit.Framework.Assert.IsTrue(matcher.Matches());
            NUnit.Framework.Assert.IsFalse(matcher.Find());
        }

        [NUnit.Framework.Test]
        public virtual void RegionTest() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "abbbbbabbbbbbbbbbbbbbb");
            matcher.Region(6, 13);
            // abbbbbb [6, 13)
            NUnit.Framework.Assert.IsTrue(matcher.Find());
            NUnit.Framework.Assert.IsFalse(matcher.Find());
            NUnit.Framework.Assert.IsTrue(matcher.Matches());
        }

        [NUnit.Framework.Test]
        public virtual void RegionSeveralMatchesTest() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "abbbbbabababbbbbbbbbbb");
            matcher.Region(6, 13);
            // ab [6, 8)
            NUnit.Framework.Assert.IsTrue(matcher.Find());
            // ab [8, 10)
            NUnit.Framework.Assert.IsTrue(matcher.Find());
            // abb [10, 13)
            NUnit.Framework.Assert.IsTrue(matcher.Find());
            NUnit.Framework.Assert.IsFalse(matcher.Find());
            NUnit.Framework.Assert.IsFalse(matcher.Matches());
        }

        [NUnit.Framework.Test]
        public virtual void StringMatchesButRegionDoesNotMatchTest() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "abbbbbbbbbbbbbbbbbbbbb");
            NUnit.Framework.Assert.IsTrue(matcher.Matches());
            matcher.Region(6, 13);
            NUnit.Framework.Assert.IsFalse(matcher.Matches());
        }

        [NUnit.Framework.Test]
        public virtual void NegativeStartOfRegionTest() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "abbbbbbbbbbbbbbbbbbbbb");
            NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => matcher.Region(-1, 10));
        }

        [NUnit.Framework.Test]
        public virtual void TooLargeStartOfRegionTest() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "abbbbbbbbbbbbbbbbbbbbb");
            NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => matcher.Region(24, 24));
        }

        [NUnit.Framework.Test]
        public virtual void NegativeEndOfRegionTest() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "abbbbbbbbbbbbbbbbbbbbb");
            NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => matcher.Region(1, -1));
        }

        [NUnit.Framework.Test]
        public virtual void TooLargeEndOfRegionTest() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "abbbbbbbbbbbbbbbbbbbbb");
            NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => matcher.Region(1, 24));
        }

        [NUnit.Framework.Test]
        public virtual void EndGreaterThenStartRegionTest() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "abbbbbbbbbbbbbbbbbbbbb");
            NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => matcher.Region(10, 9));
        }

        [NUnit.Framework.Test]
        public virtual void StartAndEndEqualRegionTest() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "abbbbbbbbbbbbbbbbbbbbb");
            matcher.Region(9, 9);
            // *empty string* [9, 9)
            NUnit.Framework.Assert.IsFalse(matcher.Matches());
        }

        [NUnit.Framework.Test]
        public virtual void StartAndEndEqualRegionMatchTest() {
            Regex patternAcceptingEmptyString = iText.Commons.Utils.StringUtil.RegexCompile("(a+)?");
            Matcher matcher = iText.Commons.Utils.Matcher.Match(patternAcceptingEmptyString, "abbbbbbbbbbbbbbbbbbbbb");
            matcher.Region(9, 9);
            // *empty string* [9, 9)
            NUnit.Framework.Assert.IsTrue(matcher.Matches());
        }

        [NUnit.Framework.Test]
        public virtual void SeveralRegionCallsTest() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "abbbbbabababbbbbbbbbbb");
            matcher.Region(6, 13);
            // abababb [6, 13)
            NUnit.Framework.Assert.IsFalse(matcher.Matches());
            matcher.Region(0, 3);
            // abb [0, 3)
            NUnit.Framework.Assert.IsTrue(matcher.Matches());
            matcher.Region(0, 4);
            // abbb [0, 4)
            NUnit.Framework.Assert.IsTrue(matcher.Matches());
            matcher.Region(0, 7);
            // abbbbba [0, 7)
            NUnit.Framework.Assert.IsFalse(matcher.Matches());
        }

        [NUnit.Framework.Test]
        public virtual void StartEndFullRegionMatchesTest() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "abbbbbabbbbbbbbbbbbbbb");
            matcher.Region(6, 13);
            // ab [6, 13)
            NUnit.Framework.Assert.IsTrue(matcher.Find());
            NUnit.Framework.Assert.AreEqual(6, matcher.Start());
            NUnit.Framework.Assert.AreEqual(13, matcher.End());
        }

        [NUnit.Framework.Test]
        public virtual void StartEndPartiallyRegionMatchesTest() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "abbbbbbbbabbabbbbbbbbb");
            matcher.Region(6, 13);
            // abb [9, 12)
            NUnit.Framework.Assert.IsTrue(matcher.Find());
            NUnit.Framework.Assert.AreEqual(9, matcher.Start());
            NUnit.Framework.Assert.AreEqual(12, matcher.End());
        }

        [NUnit.Framework.Test]
        public virtual void StartRegionDoesNotMatchesTest() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "abbbbbbbbbbbbbbbbbbbbb");
            matcher.Region(6, 13);
            NUnit.Framework.Assert.IsFalse(matcher.Find());
            NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => matcher.Start());
        }

        [NUnit.Framework.Test]
        public virtual void EndRegionDoesNotMatchesTest() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "abbbbbbbbbbbbbbbbbbbbb");
            matcher.Region(6, 13);
            NUnit.Framework.Assert.IsFalse(matcher.Find());
            NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => matcher.End());
        }

        [NUnit.Framework.Test]
        public virtual void GroupsAndRegionTest() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "abbbbbabababbbbbbbbbbb");
            matcher.Region(6, 8);
            // ab [6, 8)
            NUnit.Framework.Assert.IsTrue(matcher.Find());
            NUnit.Framework.Assert.AreEqual("ab", matcher.Group());
            NUnit.Framework.Assert.AreEqual("ab", matcher.Group(0));
            NUnit.Framework.Assert.AreEqual("a", matcher.Group(1));
            NUnit.Framework.Assert.AreEqual("b", matcher.Group(2));
        }

        [NUnit.Framework.Test]
        public virtual void RegionResetsSearchTest() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "bbbbbbabbbbbbbbbabbbbb");
            // abbbbbbbbb [6, 16)
            NUnit.Framework.Assert.IsTrue(matcher.Find());
            NUnit.Framework.Assert.AreEqual(6, matcher.Start());
            NUnit.Framework.Assert.AreEqual(16, matcher.End());
            // abbbbb [16, 22)
            NUnit.Framework.Assert.IsTrue(matcher.Find());
            NUnit.Framework.Assert.AreEqual(16, matcher.Start());
            NUnit.Framework.Assert.AreEqual(22, matcher.End());
            matcher.Region(6, 13);
            // abbbbbb [6, 16)
            NUnit.Framework.Assert.IsTrue(matcher.Find());
            NUnit.Framework.Assert.AreEqual(6, matcher.Start());
            NUnit.Framework.Assert.AreEqual(13, matcher.End());
        }

        [NUnit.Framework.Test]
        public virtual void FindWithParamResetsRegionTest() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "abbbbbbbbbbbbbbbbbbbbb");
            matcher.Region(6, 13);
            // bbbbbbb [6, 13)
            NUnit.Framework.Assert.IsFalse(matcher.Find());
            NUnit.Framework.Assert.IsTrue(matcher.Find(0));
            NUnit.Framework.Assert.AreEqual("abbbbbbbbbbbbbbbbbbbbb", matcher.Group());
            NUnit.Framework.Assert.AreEqual(0, matcher.Start());
            NUnit.Framework.Assert.AreEqual(22, matcher.End());
        }

        [NUnit.Framework.Test]
        public virtual void StartAfterRegionThrowsExceptionTest() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "abbbbbbbbbbbbbbbbbbbbb");
            matcher.Find();
            matcher.Region(6, 13);
            NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => matcher.Start());
        }

        [NUnit.Framework.Test]
        public virtual void EndAfterRegionThrowsExceptionTest() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "abbbbbbbbbbbbbbbbbbbbb");
            matcher.Find();
            matcher.Region(6, 13);
            NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => matcher.End());
        }

        [NUnit.Framework.Test]
        public virtual void GroupAfterRegionThrowsExceptionTest() {
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, "abbbbbbbbbbbbbbbbbbbbb");
            matcher.Find();
            matcher.Region(6, 13);
            NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => matcher.Group());
        }
    }
}
