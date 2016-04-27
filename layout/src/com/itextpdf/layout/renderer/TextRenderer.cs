/*
$Id: cb22d256fe0cf3ab92515bc7982b5fed0e586bab $

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

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
using System.Collections;
using System.Collections.Generic;
using com.itextpdf.io.font;
using com.itextpdf.io.font.otf;
using com.itextpdf.io.util;
using com.itextpdf.kernel.color;
using com.itextpdf.kernel.font;
using com.itextpdf.kernel.geom;
using com.itextpdf.kernel.pdf;
using com.itextpdf.kernel.pdf.canvas;
using com.itextpdf.kernel.pdf.tagutils;
using com.itextpdf.layout.element;
using com.itextpdf.layout.hyphenation;
using com.itextpdf.layout.layout;
using com.itextpdf.layout.property;
using com.itextpdf.layout.splitting;
using java.lang;

namespace com.itextpdf.layout.renderer
{
	/// <summary>
	/// This class represents the
	/// <see cref="IRenderer">renderer</see>
	/// object for a
	/// <see cref="com.itextpdf.layout.element.Text"/>
	/// object. It will draw the glyphs of the textual content on the
	/// <see cref="DrawContext"/>
	/// .
	/// </summary>
	public class TextRenderer : AbstractRenderer
	{
		protected internal const float TEXT_SPACE_COEFF = FontProgram.UNITS_NORMALIZATION;

		private const float ITALIC_ANGLE = 0.21256f;

		private const float BOLD_SIMULATION_STROKE_COEFF = 1 / 30f;

		private const float TYPO_ASCENDER_SCALE_COEFF = 1.2f;

		protected internal float yLineOffset;

		protected internal GlyphLine text;

		protected internal GlyphLine line;

		protected internal String strToBeConverted;

		protected internal bool otfFeaturesApplied = false;

		protected internal float tabAnchorCharacterPosition = -1;

		/// <summary>Creates a TextRenderer from its corresponding layout object.</summary>
		/// <param name="textElement">
		/// the
		/// <see cref="com.itextpdf.layout.element.Text"/>
		/// which this object should manage
		/// </param>
		public TextRenderer(Text textElement)
			: this(textElement, textElement.GetText())
		{
		}

		/// <summary>
		/// Creates a TextRenderer from its corresponding layout object, with a custom
		/// text to replace the contents of the
		/// <see cref="com.itextpdf.layout.element.Text"/>
		/// .
		/// </summary>
		/// <param name="textElement">
		/// the
		/// <see cref="com.itextpdf.layout.element.Text"/>
		/// which this object should manage
		/// </param>
		/// <param name="text">the replacement text</param>
		public TextRenderer(Text textElement, String text)
			: base(textElement)
		{
			this.strToBeConverted = text;
		}

		protected internal TextRenderer(com.itextpdf.layout.renderer.TextRenderer other)
			: base(other)
		{
			this.text = other.text;
			this.line = other.line;
			this.strToBeConverted = other.strToBeConverted;
			this.otfFeaturesApplied = other.otfFeaturesApplied;
			this.tabAnchorCharacterPosition = other.tabAnchorCharacterPosition;
		}

		public override LayoutResult Layout(LayoutContext layoutContext)
		{
			ConvertWaitingStringToGlyphLine();
			LayoutArea area = layoutContext.GetArea();
			Rectangle layoutBox = ApplyMargins(area.GetBBox().Clone(), false);
			ApplyBorderBox(layoutBox, false);
			occupiedArea = new LayoutArea(area.GetPageNumber(), new Rectangle(layoutBox.GetX(
				), layoutBox.GetY() + layoutBox.GetHeight(), 0, 0));
			bool anythingPlaced = false;
			int currentTextPos = text.start;
			float fontSize = GetPropertyAsFloat(Property.FONT_SIZE);
			float textRise = GetPropertyAsFloat(Property.TEXT_RISE);
			float characterSpacing = GetPropertyAsFloat(Property.CHARACTER_SPACING);
			float wordSpacing = GetPropertyAsFloat(Property.WORD_SPACING);
			PdfFont font = GetPropertyAsFont(Property.FONT);
			float hScale = GetProperty(Property.HORIZONTAL_SCALING);
			ISplitCharacters splitCharacters = GetProperty(Property.SPLIT_CHARACTERS);
			float italicSkewAddition = bool.ValueOf(true).Equals(GetPropertyAsBoolean(Property
				.ITALIC_SIMULATION)) ? ITALIC_ANGLE * fontSize : 0;
			float boldSimulationAddition = bool.ValueOf(true).Equals(GetPropertyAsBoolean(Property
				.BOLD_SIMULATION)) ? BOLD_SIMULATION_STROKE_COEFF * fontSize : 0;
			line = new GlyphLine(text);
			line.start = line.end = -1;
			FontMetrics fontMetrics = font.GetFontProgram().GetFontMetrics();
			float ascender;
			float descender;
			if (fontMetrics.GetWinAscender() == 0 || fontMetrics.GetWinDescender() == 0 || fontMetrics
				.GetTypoAscender() == fontMetrics.GetWinAscender() && fontMetrics.GetTypoDescender
				() == fontMetrics.GetWinDescender())
			{
				ascender = fontMetrics.GetTypoAscender() * TYPO_ASCENDER_SCALE_COEFF;
				descender = fontMetrics.GetTypoDescender() * TYPO_ASCENDER_SCALE_COEFF;
			}
			else
			{
				ascender = fontMetrics.GetWinAscender();
				descender = fontMetrics.GetWinDescender();
			}
			float currentLineAscender = 0;
			float currentLineDescender = 0;
			float currentLineHeight = 0;
			int initialLineTextPos = currentTextPos;
			float currentLineWidth = 0;
			int previousCharPos = -1;
			char tabAnchorCharacter = GetProperty(Property.TAB_ANCHOR);
			TextLayoutResult result = null;
			// true in situations like "\nHello World"
			bool isSplitForcedByImmediateNewLine = false;
			// true in situations like "Hello\nWorld"
			bool isSplitForcedByNewLineAndWeNeedToIgnoreNewLineSymbol = false;
			while (currentTextPos < text.end)
			{
				if (NoPrint(text.Get(currentTextPos)))
				{
					currentTextPos++;
					continue;
				}
				int nonBreakablePartEnd = text.end - 1;
				float nonBreakablePartFullWidth = 0;
				float nonBreakablePartWidthWhichDoesNotExceedAllowedWidth = 0;
				float nonBreakablePartMaxAscender = 0;
				float nonBreakablePartMaxDescender = 0;
				float nonBreakablePartMaxHeight = 0;
				int firstCharacterWhichExceedsAllowedWidth = -1;
				for (int ind = currentTextPos; ind < text.end; ind++)
				{
					if (IsNewLine(text, ind))
					{
						isSplitForcedByNewLineAndWeNeedToIgnoreNewLineSymbol = true;
						firstCharacterWhichExceedsAllowedWidth = ind + 1;
						if (text.start == currentTextPos)
						{
							isSplitForcedByImmediateNewLine = true;
							// Notice that in that case we do not need to ignore the new line symbol ('\n')
							isSplitForcedByNewLineAndWeNeedToIgnoreNewLineSymbol = false;
						}
						break;
					}
					Glyph currentGlyph = text.Get(ind);
					if (NoPrint(currentGlyph))
					{
						continue;
					}
					if (tabAnchorCharacter != null && tabAnchorCharacter == text.Get(ind).GetUnicode(
						))
					{
						tabAnchorCharacterPosition = currentLineWidth + nonBreakablePartFullWidth;
						tabAnchorCharacter = null;
					}
					float glyphWidth = GetCharWidth(currentGlyph, fontSize, hScale, characterSpacing, 
						wordSpacing) / TEXT_SPACE_COEFF;
					float xAdvance = previousCharPos != -1 ? text.Get(previousCharPos).GetXAdvance() : 
						0;
					if (xAdvance != 0)
					{
						xAdvance = ScaleXAdvance(xAdvance, fontSize, hScale) / TEXT_SPACE_COEFF;
					}
					if ((nonBreakablePartFullWidth + glyphWidth + xAdvance + italicSkewAddition + boldSimulationAddition
						) > layoutBox.GetWidth() - currentLineWidth && firstCharacterWhichExceedsAllowedWidth
						 == -1)
					{
						firstCharacterWhichExceedsAllowedWidth = ind;
					}
					if (firstCharacterWhichExceedsAllowedWidth == -1)
					{
						nonBreakablePartWidthWhichDoesNotExceedAllowedWidth += glyphWidth + xAdvance;
					}
					nonBreakablePartFullWidth += glyphWidth + xAdvance;
					nonBreakablePartMaxAscender = Math.Max(nonBreakablePartMaxAscender, ascender);
					nonBreakablePartMaxDescender = Math.Min(nonBreakablePartMaxDescender, descender);
					nonBreakablePartMaxHeight = (nonBreakablePartMaxAscender - nonBreakablePartMaxDescender
						) * fontSize / TEXT_SPACE_COEFF + textRise;
					previousCharPos = ind;
					if (nonBreakablePartFullWidth + italicSkewAddition + boldSimulationAddition > layoutBox
						.GetWidth())
					{
						// we have extracted all the information we wanted and we do not want to continue.
						// we will have to split the word anyway.
						break;
					}
					if (splitCharacters.IsSplitCharacter(text, ind) || ind + 1 == text.end || splitCharacters
						.IsSplitCharacter(text, ind + 1) && (char.IsWhiteSpace(text.Get(ind + 1).GetUnicode
						()) || char.IsSpaceChar(text.Get(ind + 1).GetUnicode())))
					{
						nonBreakablePartEnd = ind;
						break;
					}
				}
				if (firstCharacterWhichExceedsAllowedWidth == -1)
				{
					// can fit the whole word in a line
					if (line.start == -1)
					{
						line.start = currentTextPos;
					}
					line.end = Math.Max(line.end, nonBreakablePartEnd + 1);
					currentLineAscender = Math.Max(currentLineAscender, nonBreakablePartMaxAscender);
					currentLineDescender = Math.Min(currentLineDescender, nonBreakablePartMaxDescender
						);
					currentLineHeight = Math.Max(currentLineHeight, nonBreakablePartMaxHeight);
					currentTextPos = nonBreakablePartEnd + 1;
					currentLineWidth += nonBreakablePartFullWidth;
					anythingPlaced = true;
				}
				else
				{
					// check if line height exceeds the allowed height
					if (Math.Max(currentLineHeight, nonBreakablePartMaxHeight) > layoutBox.GetHeight(
						))
					{
						// the line does not fit because of height - full overflow
						com.itextpdf.layout.renderer.TextRenderer[] splitResult = Split(initialLineTextPos
							);
						ApplyBorderBox(occupiedArea.GetBBox(), true);
						ApplyMargins(occupiedArea.GetBBox(), true);
						return new TextLayoutResult(LayoutResult.NOTHING, occupiedArea, splitResult[0], splitResult
							[1]);
					}
					else
					{
						// cannot fit a word as a whole
						bool wordSplit = false;
						bool hyphenationApplied = false;
						HyphenationConfig hyphenationConfig = GetProperty(Property.HYPHENATION);
						if (hyphenationConfig != null)
						{
							int[] wordBounds = GetWordBoundsForHyphenation(text, currentTextPos, text.end, Math
								.Max(currentTextPos, firstCharacterWhichExceedsAllowedWidth - 1));
							if (wordBounds != null)
							{
								String word = text.ToUnicodeString(wordBounds[0], wordBounds[1]);
								Hyphenation hyph = hyphenationConfig.Hyphenate(word);
								if (hyph != null)
								{
									for (int i = hyph.Length() - 1; i >= 0; i--)
									{
										String pre = hyph.GetPreHyphenText(i);
										String pos = hyph.GetPostHyphenText(i);
										float currentHyphenationChoicePreTextWidth = GetGlyphLineWidth(ConvertToGlyphLine
											(pre + hyphenationConfig.GetHyphenSymbol()), fontSize, hScale, characterSpacing, 
											wordSpacing);
										if (currentLineWidth + currentHyphenationChoicePreTextWidth + italicSkewAddition 
											+ boldSimulationAddition <= layoutBox.GetWidth())
										{
											hyphenationApplied = true;
											if (line.start == -1)
											{
												line.start = currentTextPos;
											}
											line.end = Math.Max(line.end, currentTextPos + pre.Length);
											GlyphLine lineCopy = line.Copy(line.start, line.end);
											lineCopy.Add(font.GetGlyph(hyphenationConfig.GetHyphenSymbol()));
											lineCopy.end++;
											line = lineCopy;
											// TODO these values are based on whole word. recalculate properly based on hyphenated part
											currentLineAscender = Math.Max(currentLineAscender, nonBreakablePartMaxAscender);
											currentLineDescender = Math.Min(currentLineDescender, nonBreakablePartMaxDescender
												);
											currentLineHeight = Math.Max(currentLineHeight, nonBreakablePartMaxHeight);
											currentLineWidth += currentHyphenationChoicePreTextWidth;
											currentTextPos += pre.Length;
											break;
										}
									}
								}
							}
						}
						if ((nonBreakablePartFullWidth > layoutBox.GetWidth() && !anythingPlaced && !hyphenationApplied
							) || (isSplitForcedByImmediateNewLine))
						{
							// if the word is too long for a single line we will have to split it
							wordSplit = true;
							if (line.start == -1)
							{
								line.start = currentTextPos;
							}
							currentTextPos = firstCharacterWhichExceedsAllowedWidth;
							line.end = Math.Max(line.end, firstCharacterWhichExceedsAllowedWidth);
							if (nonBreakablePartFullWidth > layoutBox.GetWidth() && !anythingPlaced && !hyphenationApplied)
							{
								currentLineAscender = Math.Max(currentLineAscender, nonBreakablePartMaxAscender);
								currentLineDescender = Math.Min(currentLineDescender, nonBreakablePartMaxDescender
									);
								currentLineHeight = Math.Max(currentLineHeight, nonBreakablePartMaxHeight);
								currentLineWidth += nonBreakablePartWidthWhichDoesNotExceedAllowedWidth;
							}
							else
							{
								// process empty line (e.g. '\n')
								currentLineAscender = ascender;
								currentLineDescender = descender;
								currentLineHeight = (currentLineAscender - currentLineDescender) * fontSize / TEXT_SPACE_COEFF
									 + textRise;
								currentLineWidth += GetCharWidth(line.Get(0), fontSize, hScale, characterSpacing, 
									wordSpacing) / TEXT_SPACE_COEFF;
							}
						}
						if (line.end <= 0)
						{
							result = new TextLayoutResult(LayoutResult.NOTHING, occupiedArea, null, this);
						}
						else
						{
							result = new TextLayoutResult(LayoutResult.PARTIAL, occupiedArea, null, null).SetWordHasBeenSplit
								(wordSplit);
						}
						break;
					}
				}
			}
			if (result != null && result.GetStatus() == LayoutResult.NOTHING)
			{
				return result;
			}
			else
			{
				if (currentLineHeight > layoutBox.GetHeight() && !GetPropertyAsBoolean(Property.FORCED_PLACEMENT
					))
				{
					ApplyBorderBox(occupiedArea.GetBBox(), true);
					ApplyMargins(occupiedArea.GetBBox(), true);
					return new TextLayoutResult(LayoutResult.NOTHING, occupiedArea, null, this);
				}
				yLineOffset = currentLineAscender * fontSize / TEXT_SPACE_COEFF;
				occupiedArea.GetBBox().MoveDown(currentLineHeight);
				occupiedArea.GetBBox().SetHeight(occupiedArea.GetBBox().GetHeight() + currentLineHeight
					);
				occupiedArea.GetBBox().SetWidth(Math.Max(occupiedArea.GetBBox().GetWidth(), currentLineWidth
					));
				layoutBox.SetHeight(area.GetBBox().GetHeight() - currentLineHeight);
				occupiedArea.GetBBox().SetWidth(occupiedArea.GetBBox().GetWidth() + italicSkewAddition
					 + boldSimulationAddition);
				ApplyBorderBox(occupiedArea.GetBBox(), true);
				ApplyMargins(occupiedArea.GetBBox(), true);
				if (result != null)
				{
					com.itextpdf.layout.renderer.TextRenderer[] split;
					if (isSplitForcedByNewLineAndWeNeedToIgnoreNewLineSymbol)
					{
						// ignore '\n'
						split = Split(currentTextPos + 1);
					}
					else
					{
						split = Split(currentTextPos);
					}
					// if (split[1].length() > 0 && split[1].charAt(0) != null && split[1].charAt(0) == '\n') {
					if (isSplitForcedByNewLineAndWeNeedToIgnoreNewLineSymbol)
					{
						result.SetSplitForcedByNewline(true);
					}
					result.SetSplitRenderer(split[0]);
					// no sense to process empty renderer
					if (split[1].text.start != split[1].text.end)
					{
						result.SetOverflowRenderer(split[1]);
					}
				}
				else
				{
					result = new TextLayoutResult(LayoutResult.FULL, occupiedArea, null, null);
				}
			}
			return result;
		}

		public virtual void ApplyOtf()
		{
			ConvertWaitingStringToGlyphLine();
			Character.UnicodeScript script = GetProperty(Property.FONT_SCRIPT);
			FontKerning fontKerning = GetProperty(Property.FONT_KERNING);
			PdfFont font = GetPropertyAsFont(Property.FONT);
			if (!otfFeaturesApplied)
			{
				if (script == null && TypographyUtils.IsTypographyModuleInitialized())
				{
					// Try to autodetect complex script.
					ICollection<Character.UnicodeScript> supportedScripts = TypographyUtils.GetSupportedScripts
						();
					IDictionary<Character.UnicodeScript, int> scriptFrequency = new EnumMap<Character.UnicodeScript
						, int>(typeof(Character.UnicodeScript));
					for (int i = text.start; i < text.end; i++)
					{
						int unicode = text.Get(i).GetUnicode();
						Character.UnicodeScript glyphScript = unicode > -1 ? Character.UnicodeScript.Of(unicode
							) : null;
						if (glyphScript != null)
						{
							if (scriptFrequency.ContainsKey(glyphScript))
							{
								scriptFrequency[glyphScript] = scriptFrequency[glyphScript] + 1;
							}
							else
							{
								scriptFrequency[glyphScript] = 1;
							}
						}
					}
					int max = 0;
					Character.UnicodeScript selectScript = null;
					foreach (KeyValuePair<Character.UnicodeScript, int> entry in scriptFrequency)
					{
						Character.UnicodeScript entryScript = entry.Key;
						if (entry.Value > max && !Character.UnicodeScript.COMMON.Equals(entryScript) && !
							Character.UnicodeScript.UNKNOWN.Equals(entryScript))
						{
							max = entry.Value;
							selectScript = entryScript;
						}
					}
					if (selectScript == Character.UnicodeScript.ARABIC || selectScript == Character.UnicodeScript
						.HEBREW && parent is LineRenderer)
					{
						SetProperty(Property.BASE_DIRECTION, BaseDirection.DEFAULT_BIDI);
					}
					if (selectScript != null && supportedScripts != null && supportedScripts.Contains
						(selectScript))
					{
						script = selectScript;
					}
				}
				if (IsOtfFont(font) && script != null)
				{
					TypographyUtils.ApplyOtfScript(font.GetFontProgram(), text, script);
				}
				if (fontKerning == FontKerning.YES)
				{
					TypographyUtils.ApplyKerning(font.GetFontProgram(), text);
				}
				otfFeaturesApplied = true;
			}
		}

		public override void Draw(DrawContext drawContext)
		{
			base.Draw(drawContext);
			PdfDocument document = drawContext.GetDocument();
			bool isTagged = drawContext.IsTaggingEnabled() && GetModelElement() is IAccessibleElement;
			bool isArtifact = false;
			TagTreePointer tagPointer = null;
			IAccessibleElement accessibleElement = null;
			if (isTagged)
			{
				accessibleElement = (IAccessibleElement)GetModelElement();
				PdfName role = accessibleElement.GetRole();
				if (role != null && !PdfName.Artifact.Equals(role))
				{
					tagPointer = document.GetTagStructureContext().GetAutoTaggingPointer();
					if (!tagPointer.IsElementConnectedToTag(accessibleElement))
					{
						AccessibleAttributesApplier.ApplyLayoutAttributes(accessibleElement.GetRole(), this
							, document);
					}
					tagPointer.AddTag(accessibleElement, true);
				}
				else
				{
					isTagged = false;
					if (PdfName.Artifact.Equals(role))
					{
						isArtifact = true;
					}
				}
			}
			int position = GetPropertyAsInteger(Property.POSITION);
			if (position == LayoutPosition.RELATIVE)
			{
				ApplyAbsolutePositioningTranslation(false);
			}
			float leftBBoxX = occupiedArea.GetBBox().GetX();
			if (line.end > line.start)
			{
				PdfFont font = GetPropertyAsFont(Property.FONT);
				float fontSize = GetPropertyAsFloat(Property.FONT_SIZE);
				Color fontColor = GetPropertyAsColor(Property.FONT_COLOR);
				int textRenderingMode = GetProperty(Property.TEXT_RENDERING_MODE);
				float textRise = GetPropertyAsFloat(Property.TEXT_RISE);
				float characterSpacing = GetPropertyAsFloat(Property.CHARACTER_SPACING);
				float wordSpacing = GetPropertyAsFloat(Property.WORD_SPACING);
				float horizontalScaling = GetProperty(Property.HORIZONTAL_SCALING);
				float[] skew = GetProperty(Property.SKEW);
				bool italicSimulation = bool.ValueOf(true).Equals(GetPropertyAsBoolean(Property.ITALIC_SIMULATION
					));
				bool boldSimulation = bool.ValueOf(true).Equals(GetPropertyAsBoolean(Property.BOLD_SIMULATION
					));
				float strokeWidth = null;
				if (boldSimulation)
				{
					textRenderingMode = PdfCanvasConstants.TextRenderingMode.FILL_STROKE;
					strokeWidth = fontSize / 30;
				}
				PdfCanvas canvas = drawContext.GetCanvas();
				if (isTagged)
				{
					canvas.OpenTag(tagPointer.GetTagReference());
				}
				else
				{
					if (isArtifact)
					{
						canvas.OpenTag(new CanvasArtifact());
					}
				}
				canvas.SaveState().BeginText().SetFontAndSize(font, fontSize);
				if (skew != null && skew.Length == 2)
				{
					canvas.SetTextMatrix(1, skew[0], skew[1], 1, leftBBoxX, GetYLine());
				}
				else
				{
					if (italicSimulation)
					{
						canvas.SetTextMatrix(1, 0, ITALIC_ANGLE, 1, leftBBoxX, GetYLine());
					}
					else
					{
						canvas.MoveText(leftBBoxX, GetYLine());
					}
				}
				if (textRenderingMode != PdfCanvasConstants.TextRenderingMode.FILL)
				{
					canvas.SetTextRenderingMode(textRenderingMode);
				}
				if (textRenderingMode == PdfCanvasConstants.TextRenderingMode.STROKE || textRenderingMode
					 == PdfCanvasConstants.TextRenderingMode.FILL_STROKE)
				{
					if (strokeWidth == null)
					{
						strokeWidth = GetPropertyAsFloat(Property.STROKE_WIDTH);
					}
					if (strokeWidth != null && strokeWidth != 1f)
					{
						canvas.SetLineWidth(strokeWidth);
					}
					Color strokeColor = GetPropertyAsColor(Property.STROKE_COLOR);
					if (strokeColor == null)
					{
						strokeColor = fontColor;
					}
					if (strokeColor != null)
					{
						canvas.SetStrokeColor(strokeColor);
					}
				}
				if (fontColor != null)
				{
					canvas.SetFillColor(fontColor);
				}
				if (textRise != null && textRise != 0)
				{
					canvas.SetTextRise(textRise);
				}
				if (characterSpacing != null && characterSpacing != 0)
				{
					canvas.SetCharacterSpacing(characterSpacing);
				}
				if (wordSpacing != null && wordSpacing != 0)
				{
					canvas.SetWordSpacing(wordSpacing);
				}
				if (horizontalScaling != null && horizontalScaling != 1)
				{
					canvas.SetHorizontalScaling(horizontalScaling * 100);
				}
				GlyphLine.IGlyphLineFilter filter = new _IGlyphLineFilter_545();
				if (HasOwnProperty(Property.REVERSED))
				{
					//We should mark a RTL written text
					IDictionary<GlyphLine, bool> outputs = GetOutputChunks();
					foreach (KeyValuePair<GlyphLine, bool> output in outputs)
					{
						GlyphLine o = output.Key.Filter(filter);
						if (output.Value)
						{
							canvas.OpenTag(new CanvasTag(PdfName.ReversedChars));
						}
						canvas.ShowText(o);
						if (output.Value)
						{
							canvas.CloseTag();
						}
					}
				}
				else
				{
					canvas.ShowText(line.Filter(filter));
				}
				canvas.EndText().RestoreState();
				if (isTagged || isArtifact)
				{
					canvas.CloseTag();
				}
				Object underlines = GetProperty(Property.UNDERLINE);
				if (underlines is IList)
				{
					foreach (Object underline in (IList)underlines)
					{
						if (underline is Underline)
						{
							DrawSingleUnderline((Underline)underline, fontColor, canvas, fontSize, italicSimulation
								 ? ITALIC_ANGLE : 0);
						}
					}
				}
				else
				{
					if (underlines is Underline)
					{
						DrawSingleUnderline((Underline)underlines, fontColor, canvas, fontSize, italicSimulation
							 ? ITALIC_ANGLE : 0);
					}
				}
			}
			if (position == LayoutPosition.RELATIVE)
			{
				ApplyAbsolutePositioningTranslation(false);
			}
			if (isTagged)
			{
				tagPointer.MoveToParent();
				if (isLastRendererForModelElement)
				{
					tagPointer.RemoveElementConnectionToTag(accessibleElement);
				}
			}
		}

		private sealed class _IGlyphLineFilter_545 : GlyphLine.IGlyphLineFilter
		{
			public _IGlyphLineFilter_545()
			{
			}

			public bool Accept(Glyph glyph)
			{
				return !com.itextpdf.layout.renderer.TextRenderer.NoPrint(glyph);
			}
		}

		public override void DrawBackground(DrawContext drawContext)
		{
			Background background = GetProperty(Property.BACKGROUND);
			float textRise = GetPropertyAsFloat(Property.TEXT_RISE);
			float bottomBBoxY = occupiedArea.GetBBox().GetY();
			float leftBBoxX = occupiedArea.GetBBox().GetX();
			if (background != null)
			{
				bool isTagged = drawContext.IsTaggingEnabled() && GetModelElement() is IAccessibleElement;
				PdfCanvas canvas = drawContext.GetCanvas();
				if (isTagged)
				{
					canvas.OpenTag(new CanvasArtifact());
				}
				canvas.SaveState().SetFillColor(background.GetColor());
				canvas.Rectangle(leftBBoxX - background.GetExtraLeft(), bottomBBoxY + textRise - 
					background.GetExtraBottom(), occupiedArea.GetBBox().GetWidth() + background.GetExtraLeft
					() + background.GetExtraRight(), occupiedArea.GetBBox().GetHeight() - textRise +
					 background.GetExtraTop() + background.GetExtraBottom());
				canvas.Fill().RestoreState();
				if (isTagged)
				{
					canvas.CloseTag();
				}
			}
		}

		public override T1 GetDefaultProperty<T1>(Property property)
		{
			switch (property)
			{
				case Property.HORIZONTAL_SCALING:
				{
					return (T1)float.ValueOf(1);
				}

				default:
				{
					return base.GetDefaultProperty(property);
				}
			}
		}

		/// <summary>
		/// Trims any whitespace characters from the start of the
		/// <see cref="com.itextpdf.io.font.otf.GlyphLine"/>
		/// to be rendered.
		/// </summary>
		public virtual void TrimFirst()
		{
			ConvertWaitingStringToGlyphLine();
			if (text != null)
			{
				Glyph glyph;
				while (text.start < text.end && (glyph = text.Get(text.start)).HasValidUnicode() 
					&& char.IsWhiteSpace(glyph.GetUnicode()) && !IsNewLine(text, text.start))
				{
					text.start++;
				}
			}
		}

		/// <summary>
		/// Trims any whitespace characters from the end of the
		/// <see cref="com.itextpdf.io.font.otf.GlyphLine"/>
		/// to
		/// be rendered.
		/// </summary>
		/// <returns>the amount of space in points which the text was trimmed by</returns>
		public virtual float TrimLast()
		{
			float trimmedSpace = 0;
			if (line.end <= 0)
			{
				return trimmedSpace;
			}
			float fontSize = GetPropertyAsFloat(Property.FONT_SIZE);
			float characterSpacing = GetPropertyAsFloat(Property.CHARACTER_SPACING);
			float wordSpacing = GetPropertyAsFloat(Property.WORD_SPACING);
			float hScale = GetProperty(Property.HORIZONTAL_SCALING);
			int firstNonSpaceCharIndex = line.end - 1;
			while (firstNonSpaceCharIndex >= line.start)
			{
				Glyph currentGlyph = line.Get(firstNonSpaceCharIndex);
				if (!currentGlyph.HasValidUnicode() || !char.IsWhiteSpace(currentGlyph.GetUnicode
					()))
				{
					break;
				}
				float currentCharWidth = GetCharWidth(currentGlyph, fontSize, hScale, characterSpacing
					, wordSpacing) / TEXT_SPACE_COEFF;
				float xAdvance = firstNonSpaceCharIndex > line.start ? ScaleXAdvance(line.Get(firstNonSpaceCharIndex
					 - 1).GetXAdvance(), fontSize, hScale) / TEXT_SPACE_COEFF : 0;
				trimmedSpace += currentCharWidth - xAdvance;
				occupiedArea.GetBBox().SetWidth(occupiedArea.GetBBox().GetWidth() - currentCharWidth
					);
				firstNonSpaceCharIndex--;
			}
			line.end = firstNonSpaceCharIndex + 1;
			return trimmedSpace;
		}

		/// <summary>Gets the maximum offset above the base line that this Text extends to.</summary>
		/// <returns>
		/// the upwards vertical offset of this
		/// <see cref="com.itextpdf.layout.element.Text"/>
		/// </returns>
		public virtual float GetAscent()
		{
			return yLineOffset;
		}

		/// <summary>Gets the maximum offset below the base line that this Text extends to.</summary>
		/// <returns>
		/// the downwards vertical offset of this
		/// <see cref="com.itextpdf.layout.element.Text"/>
		/// </returns>
		public virtual float GetDescent()
		{
			return -(occupiedArea.GetBBox().GetHeight() - yLineOffset - GetPropertyAsFloat(Property
				.TEXT_RISE));
		}

		/// <summary>
		/// Gets the position on the canvas of the imaginary horizontal line upon which
		/// the
		/// <see cref="com.itextpdf.layout.element.Text"/>
		/// 's contents will be written.
		/// </summary>
		/// <returns>
		/// the y position of this text on the
		/// <see cref="DrawContext"/>
		/// </returns>
		public virtual float GetYLine()
		{
			return occupiedArea.GetBBox().GetY() + occupiedArea.GetBBox().GetHeight() - yLineOffset
				 - GetPropertyAsFloat(Property.TEXT_RISE);
		}

		/// <summary>Moves the vertical position to the parameter's value.</summary>
		/// <param name="y">the new vertical position of the Text</param>
		public virtual void MoveYLineTo(float y)
		{
			float curYLine = GetYLine();
			float delta = y - curYLine;
			occupiedArea.GetBBox().SetY(occupiedArea.GetBBox().GetY() + delta);
		}

		/// <summary>
		/// Manually sets the contents of the Text's representation on the canvas,
		/// regardless of the Text's own contents.
		/// </summary>
		/// <param name="text">the replacement text</param>
		public virtual void SetText(String text)
		{
			GlyphLine glyphLine = ConvertToGlyphLine(text);
			SetText(glyphLine, glyphLine.start, glyphLine.end);
		}

		/// <summary>
		/// Manually sets a GlyphLine to be rendered with a specific start and end
		/// point.
		/// </summary>
		/// <param name="text">
		/// a
		/// <see cref="com.itextpdf.io.font.otf.GlyphLine"/>
		/// </param>
		/// <param name="leftPos">the leftmost end of the GlyphLine</param>
		/// <param name="rightPos">the rightmost end of the GlyphLine</param>
		public virtual void SetText(GlyphLine text, int leftPos, int rightPos)
		{
			this.text = new GlyphLine(text);
			this.text.start = leftPos;
			this.text.end = rightPos;
			this.otfFeaturesApplied = false;
		}

		public virtual GlyphLine GetText()
		{
			ConvertWaitingStringToGlyphLine();
			return text;
		}

		/// <summary>The length of the whole text assigned to this renderer.</summary>
		/// <returns>the text length</returns>
		public virtual int Length()
		{
			return text == null ? 0 : text.end - text.start;
		}

		public override String ToString()
		{
			return line != null ? line.ToUnicodeString(line.start, line.end) : strToBeConverted;
		}

		/// <summary>Gets char code at given position for the text belonging to this renderer.
		/// 	</summary>
		/// <param name="pos">the position in range [0; length())</param>
		/// <returns>Unicode char code</returns>
		public virtual int CharAt(int pos)
		{
			return text.Get(pos + text.start).GetUnicode();
		}

		public virtual float GetTabAnchorCharacterPosition()
		{
			return tabAnchorCharacterPosition;
		}

		public override IRenderer GetNextRenderer()
		{
			return new com.itextpdf.layout.renderer.TextRenderer((Text)modelElement, null);
		}

		private bool IsNewLine(GlyphLine text, int ind)
		{
			return text.Get(ind).HasValidUnicode() && text.Get(ind).GetUnicode() == '\n';
		}

		private GlyphLine ConvertToGlyphLine(String text)
		{
			PdfFont font = GetPropertyAsFont(Property.FONT);
			return font.CreateGlyphLine(text);
		}

		private bool IsOtfFont(PdfFont font)
		{
			return font is PdfType0Font && font.GetFontProgram() is TrueTypeFont;
		}

		protected internal override float GetFirstYLineRecursively()
		{
			return GetYLine();
		}

		/// <summary>
		/// Returns the length of the
		/// <seealso>line</seealso>
		/// which is the result of the layout call.
		/// </summary>
		protected internal virtual int LineLength()
		{
			return line.end > 0 ? line.end - line.start : 0;
		}

		protected internal virtual int BaseCharactersCount()
		{
			int count = 0;
			for (int i = line.start; i < line.end; i++)
			{
				Glyph glyph = line.Get(i);
				if (!glyph.HasPlacement())
				{
					count++;
				}
			}
			return count;
		}

		protected internal virtual int GetNumberOfSpaces()
		{
			if (line.end <= 0)
			{
				return 0;
			}
			int spaces = 0;
			for (int i = line.start; i < line.end; i++)
			{
				Glyph currentGlyph = line.Get(i);
				if (currentGlyph.HasValidUnicode() && currentGlyph.GetUnicode() == ' ')
				{
					spaces++;
				}
			}
			return spaces;
		}

		protected internal virtual com.itextpdf.layout.renderer.TextRenderer CreateSplitRenderer
			()
		{
			return (com.itextpdf.layout.renderer.TextRenderer)GetNextRenderer();
		}

		protected internal virtual com.itextpdf.layout.renderer.TextRenderer CreateOverflowRenderer
			()
		{
			return (com.itextpdf.layout.renderer.TextRenderer)GetNextRenderer();
		}

		protected internal virtual com.itextpdf.layout.renderer.TextRenderer[] Split(int 
			initialOverflowTextPos)
		{
			com.itextpdf.layout.renderer.TextRenderer splitRenderer = CreateSplitRenderer();
			splitRenderer.SetText(text, text.start, initialOverflowTextPos);
			splitRenderer.line = line;
			splitRenderer.occupiedArea = occupiedArea.Clone();
			splitRenderer.parent = parent;
			splitRenderer.yLineOffset = yLineOffset;
			splitRenderer.otfFeaturesApplied = otfFeaturesApplied;
			splitRenderer.isLastRendererForModelElement = false;
			splitRenderer.AddAllProperties(GetOwnProperties());
			com.itextpdf.layout.renderer.TextRenderer overflowRenderer = CreateOverflowRenderer
				();
			overflowRenderer.SetText(text, initialOverflowTextPos, text.end);
			overflowRenderer.otfFeaturesApplied = otfFeaturesApplied;
			overflowRenderer.parent = parent;
			overflowRenderer.AddAllProperties(GetOwnProperties());
			return new com.itextpdf.layout.renderer.TextRenderer[] { splitRenderer, overflowRenderer
				 };
		}

		protected internal virtual void DrawSingleUnderline(Underline underline, Color fontStrokeColor
			, PdfCanvas canvas, float fontSize, float italicAngleTan)
		{
			Color underlineColor = underline.GetColor() != null ? underline.GetColor() : fontStrokeColor;
			canvas.SaveState();
			if (underlineColor != null)
			{
				canvas.SetStrokeColor(underlineColor);
			}
			float underlineThickness = underline.GetThickness(fontSize);
			if (underlineThickness != 0)
			{
				canvas.SetLineWidth(underlineThickness);
				float yLine = GetYLine();
				float underlineYPosition = underline.GetYPosition(fontSize) + yLine;
				float italicWidthSubstraction = .5f * fontSize * italicAngleTan;
				canvas.MoveTo(occupiedArea.GetBBox().GetX(), underlineYPosition).LineTo(occupiedArea
					.GetBBox().GetX() + occupiedArea.GetBBox().GetWidth() - italicWidthSubstraction, 
					underlineYPosition).Stroke();
			}
			canvas.RestoreState();
		}

		protected internal virtual float CalculateLineWidth()
		{
			return GetGlyphLineWidth(line, GetPropertyAsFloat(Property.FONT_SIZE), GetPropertyAsFloat
				(Property.HORIZONTAL_SCALING), GetPropertyAsFloat(Property.CHARACTER_SPACING), GetPropertyAsFloat
				(Property.WORD_SPACING));
		}

		/// <summary>This method return a LinkedHashMap with glyphlines as its keys.</summary>
		/// <remarks>
		/// This method return a LinkedHashMap with glyphlines as its keys. Values are boolean flags indicating if a
		/// glyphline is written in a reversed order (right to left text).
		/// </remarks>
		/// <returns/>
		private IDictionary<GlyphLine, bool> GetOutputChunks()
		{
			IList<int[]> reversedRange = GetProperty(Property.REVERSED);
			IDictionary<GlyphLine, bool> outputs = new LinkedDictionary<GlyphLine, bool>();
			if (reversedRange != null)
			{
				if (reversedRange[0][0] > 0)
				{
					outputs[line.Copy(0, reversedRange[0][0])] = false;
				}
				for (int i = 0; i < reversedRange.Count; i++)
				{
					int[] range = reversedRange[i];
					outputs[line.Copy(range[0], range[1] + 1)] = true;
					if (i != reversedRange.Count - 1)
					{
						outputs[line.Copy(range[1] + 1, reversedRange[i + 1][0])] = false;
					}
				}
				int lastIndex = reversedRange[reversedRange.Count - 1][1];
				if (lastIndex < line.Size())
				{
					outputs[line.Copy(lastIndex + 1, line.Size())] = false;
				}
			}
			else
			{
				outputs[line] = false;
			}
			return outputs;
		}

		private static bool NoPrint(Glyph g)
		{
			if (!g.HasValidUnicode())
			{
				return false;
			}
			int c = g.GetUnicode();
			return c >= 0x200b && c <= 0x200f || c >= 0x202a && c <= 0x202e || c == '\u00AD';
		}

		private float GetCharWidth(Glyph g, float fontSize, float hScale, float characterSpacing
			, float wordSpacing)
		{
			if (hScale == null)
			{
				hScale = 1f;
			}
			float resultWidth = g.GetWidth() * fontSize * hScale;
			if (characterSpacing != null)
			{
				resultWidth += characterSpacing * hScale * TEXT_SPACE_COEFF;
			}
			if (wordSpacing != null && g.HasValidUnicode() && g.GetUnicode() == ' ')
			{
				resultWidth += wordSpacing * hScale * TEXT_SPACE_COEFF;
			}
			return resultWidth;
		}

		private float ScaleXAdvance(float xAdvance, float fontSize, float hScale)
		{
			return xAdvance * fontSize * hScale;
		}

		private float GetGlyphLineWidth(GlyphLine glyphLine, float fontSize, float hScale
			, float characterSpacing, float wordSpacing)
		{
			float width = 0;
			for (int i = glyphLine.start; i < glyphLine.end; i++)
			{
				float charWidth = GetCharWidth(glyphLine.Get(i), fontSize, hScale, characterSpacing
					, wordSpacing);
				width += charWidth;
				float xAdvance = (i != glyphLine.start) ? ScaleXAdvance(glyphLine.Get(i - 1).GetXAdvance
					(), fontSize, hScale) : 0;
				width += xAdvance;
			}
			return width / TEXT_SPACE_COEFF;
		}

		private int[] GetWordBoundsForHyphenation(GlyphLine text, int leftTextPos, int rightTextPos
			, int wordMiddleCharPos)
		{
			while (wordMiddleCharPos >= leftTextPos && !IsGlyphPartOfWordForHyphenation(text.
				Get(wordMiddleCharPos)) && !IsWhitespaceGlyph(text.Get(wordMiddleCharPos)))
			{
				wordMiddleCharPos--;
			}
			if (wordMiddleCharPos >= leftTextPos)
			{
				int left = wordMiddleCharPos;
				while (left >= leftTextPos && IsGlyphPartOfWordForHyphenation(text.Get(left)))
				{
					left--;
				}
				int right = wordMiddleCharPos;
				while (right < rightTextPos && IsGlyphPartOfWordForHyphenation(text.Get(right)))
				{
					right++;
				}
				return new int[] { left + 1, right };
			}
			else
			{
				return null;
			}
		}

		private bool IsGlyphPartOfWordForHyphenation(Glyph g)
		{
			return g.HasValidUnicode() && (char.IsLetter(g.GetUnicode()) || char.IsDigit(g.GetUnicode
				()) || '\u00ad' == g.GetUnicode());
		}

		private bool IsWhitespaceGlyph(Glyph g)
		{
			return g.HasValidUnicode() && g.GetUnicode() == ' ';
		}

		private void ConvertWaitingStringToGlyphLine()
		{
			if (strToBeConverted != null)
			{
				GlyphLine glyphLine = ConvertToGlyphLine(strToBeConverted);
				SetText(glyphLine, glyphLine.start, glyphLine.end);
				strToBeConverted = null;
			}
		}
	}
}
