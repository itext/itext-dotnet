using Common.Logging;
using iText.IO.Util;

namespace iText.StyledXmlParser.Css.Util {
    /// <summary>Class to store escape characters and their processing logic.</summary>
    /// <remarks>
    /// Class to store escape characters and their processing logic.
    /// This class is used in
    /// <see cref="CssUtils.SplitString(System.String, char, EscapeGroup[])"/>
    /// method.
    /// </remarks>
    public class EscapeGroup {
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(iText.StyledXmlParser.Css.Util.EscapeGroup
            ));

        private readonly char openCharacter;

        private readonly char closeCharacter;

        private int counter = 0;

        /// <summary>
        /// Creates instance of
        /// <see cref="EscapeGroup"/>.
        /// </summary>
        /// <param name="openCharacter">opening escape character</param>
        /// <param name="closeCharacter">closing escape character</param>
        public EscapeGroup(char openCharacter, char closeCharacter) {
            this.openCharacter = openCharacter;
            this.closeCharacter = closeCharacter;
        }

        /// <summary>
        /// Creates instance of
        /// <see cref="EscapeGroup"/>
        /// when opening and closing characters are the same.
        /// </summary>
        /// <param name="escapeChar">opening and closing escape character</param>
        public EscapeGroup(char escapeChar) {
            this.openCharacter = escapeChar;
            this.closeCharacter = escapeChar;
        }

        /// <summary>
        /// Is currently processed character in
        /// <see cref="CssUtils.SplitString(System.String, char, EscapeGroup[])"/>
        /// escaped.
        /// </summary>
        /// <returns>true if escaped, false otherwise</returns>
        internal virtual bool IsEscaped() {
            return counter != 0;
        }

        /// <summary>Processes given character.</summary>
        /// <param name="nextCharacter">next character to process</param>
        internal virtual void ProcessCharacter(char nextCharacter) {
            if (openCharacter == closeCharacter) {
                if (nextCharacter == openCharacter) {
                    if (IsEscaped()) {
                        ++counter;
                    }
                    else {
                        --counter;
                    }
                }
            }
            else {
                if (nextCharacter == openCharacter) {
                    ++counter;
                }
                else {
                    if (nextCharacter == closeCharacter) {
                        --counter;
                        if (counter < 0) {
                            LOGGER.Warn(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.INCORRECT_CHARACTER_SEQUENCE
                                ));
                            counter = 0;
                        }
                    }
                }
            }
        }
    }
}
