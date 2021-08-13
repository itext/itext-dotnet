/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
Authors: iText Software.

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
using System.Text;
using System.Text.RegularExpressions;
using Common.Logging;
using iText.Events.Util;
using iText.Kernel.Actions.Events;
using iText.Kernel.Exceptions;
using iText.Kernel.Logs;

namespace iText.Kernel.Actions.Producer {
    /// <summary>Class is used for producer line building.</summary>
    public sealed class ProducerBuilder {
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(iText.Kernel.Actions.Producer.ProducerBuilder
            ));

        private const String CURRENT_DATE = "currentDate";

        private const String USED_PRODUCTS = "usedProducts";

        private const String COPYRIGHT_SINCE = "copyrightSince";

        private const String COPYRIGHT_TO = "copyrightTo";

        private const char FORMAT_DELIMITER = ':';

        private const String MODIFIED_USING = "; modified using ";

        /// <summary>Pattern is used to search a placeholders.</summary>
        /// <remarks>
        /// Pattern is used to search a placeholders. Currently it searches substrings started with
        /// <c>${</c> and ended with <c>}</c> without <c>}</c> character inside.
        /// These substrings are interpreted as placeholders and the first group is the content of the
        /// placeholder.
        /// </remarks>
        private const String PATTERN_STRING = "\\$\\{([^}]*)}";

        private static readonly Regex PATTERN = iText.IO.Util.StringUtil.RegexCompile(PATTERN_STRING);

        private static readonly IDictionary<String, IPlaceholderPopulator> PLACEHOLDER_POPULATORS;

        static ProducerBuilder() {
            IDictionary<String, IPlaceholderPopulator> populators = new Dictionary<String, IPlaceholderPopulator>();
            populators.Put(CURRENT_DATE, new CurrentDatePlaceholderPopulator());
            populators.Put(USED_PRODUCTS, new UsedProductsPlaceholderPopulator());
            populators.Put(COPYRIGHT_SINCE, new CopyrightSincePlaceholderPopulator());
            populators.Put(COPYRIGHT_TO, new CopyrightToPlaceholderPopulator());
            PLACEHOLDER_POPULATORS = JavaCollectionsUtil.UnmodifiableMap(populators);
        }

        private ProducerBuilder() {
        }

        /// <summary>Modifies an old producer line according to events registered for the document.</summary>
        /// <remarks>
        /// Modifies an old producer line according to events registered for the document. Format of the
        /// new producer line will be defined by the first event in the list. Placeholder will be
        /// replaced and merged all together
        /// </remarks>
        /// <param name="events">
        /// list of events wrapped with
        /// <see cref="iText.Kernel.Actions.Events.ConfirmedEventWrapper"/>
        /// registered for
        /// the document
        /// </param>
        /// <param name="oldProducer">
        /// is an old producer line. If <c>null</c> or empty, will be replaced
        /// with a new one. Otherwise new line will be attached with
        /// <c>modified using</c> prefix. If old producer line already contains
        /// <c>modified using</c> substring, it will be overriden with a new one
        /// </param>
        /// <returns>modified producer line</returns>
        public static String ModifyProducer(IList<ConfirmedEventWrapper> events, String oldProducer) {
            String newProducer = BuildProducer(events);
            if (oldProducer == null || String.IsNullOrEmpty(oldProducer)) {
                return newProducer;
            }
            else {
                return oldProducer + MODIFIED_USING + newProducer;
            }
        }

        private static String BuildProducer(IList<ConfirmedEventWrapper> events) {
            if (events == null || events.IsEmpty()) {
                throw new ArgumentException(KernelExceptionMessageConstant.NO_EVENTS_WERE_REGISTERED_FOR_THE_DOCUMENT);
            }
            // we expects here that the first event was thrown by
            // the addon which may be considered as entry point of
            // document processing
            String producer = events[0].GetProducerLine();
            return PopulatePlaceholders(producer, events);
        }

        private static String PopulatePlaceholders(String producerLine, IList<ConfirmedEventWrapper> events) {
            int lastIndex = 0;
            Matcher matcher = iText.Events.Util.Matcher.Match(PATTERN, producerLine);
            StringBuilder builder = new StringBuilder();
            while (matcher.Find()) {
                builder.Append(producerLine.JSubstring(lastIndex, matcher.Start()));
                lastIndex = matcher.End();
                String placeholder = matcher.Group(1);
                int delimiterPosition = placeholder.IndexOf(FORMAT_DELIMITER);
                String placeholderName;
                String parameter;
                if (placeholder.IndexOf(FORMAT_DELIMITER) != -1) {
                    placeholderName = placeholder.JSubstring(0, delimiterPosition);
                    parameter = placeholder.Substring(delimiterPosition + 1);
                }
                else {
                    placeholderName = placeholder;
                    parameter = null;
                }
                IPlaceholderPopulator populator = PLACEHOLDER_POPULATORS.Get(placeholderName);
                if (populator == null) {
                    LOGGER.Info(MessageFormatUtil.Format(KernelLogMessageConstant.UNKNOWN_PLACEHOLDER_WAS_IGNORED, placeholderName
                        ));
                }
                else {
                    builder.Append(populator.Populate(events, parameter));
                }
            }
            builder.Append(producerLine.Substring(lastIndex));
            return builder.ToString();
        }
    }
}
