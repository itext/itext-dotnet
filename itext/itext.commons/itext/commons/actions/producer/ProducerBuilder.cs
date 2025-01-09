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
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Actions;
using iText.Commons.Actions.Confirmations;
using iText.Commons.Actions.Processors;
using iText.Commons.Exceptions;
using iText.Commons.Logs;
using iText.Commons.Utils;

namespace iText.Commons.Actions.Producer {
    /// <summary>Class is used for producer line building.</summary>
    public sealed class ProducerBuilder : AbstractITextConfigurationEvent {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Commons.Actions.Producer.ProducerBuilder
            ));

        private static readonly iText.Commons.Actions.Producer.ProducerBuilder INSTANCE = new iText.Commons.Actions.Producer.ProducerBuilder
            ();

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
        /// Note: The escape on '}' is necessary for regex dialect compatibility reasons.
        /// </remarks>
        private const String PATTERN_STRING = "\\$\\{([^}]*)\\}";

        private static readonly Regex PATTERN = iText.Commons.Utils.StringUtil.RegexCompile(PATTERN_STRING);

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
        /// Modifies an old producer line according to events registered for the document.
        /// Events can be either wrapped with
        /// <see cref="iText.Commons.Actions.Confirmations.ConfirmedEventWrapper"/>
        /// or not.
        /// Format of the new producer line will be defined by the first event in the list.
        /// Placeholder will be replaced and merged all together.
        /// </remarks>
        /// <param name="events">list of events registered for the document</param>
        /// <param name="oldProducer">
        /// old producer line. If <c>null</c> or empty, will be replaced
        /// with a new one. Otherwise new line will be attached with
        /// <c>modified using</c> prefix. If old producer line already contains
        /// <c>modified using itext</c> substring with the current version of itext at the end,
        /// no changes will be made
        /// </param>
        /// <returns>modified producer line</returns>
        public static String ModifyProducer<_T0>(IList<_T0> events, String oldProducer)
            where _T0 : AbstractProductProcessITextEvent {
            IList<ConfirmedEventWrapper> confirmedEvents = new List<ConfirmedEventWrapper>();
            if (events != null) {
                foreach (AbstractProductProcessITextEvent @event in events) {
                    if (@event is ConfirmedEventWrapper) {
                        confirmedEvents.Add((ConfirmedEventWrapper)@event);
                    }
                    else {
                        ITextProductEventProcessor processor = INSTANCE.GetActiveProcessor(@event.GetProductName());
                        confirmedEvents.Add(new ConfirmedEventWrapper(@event, processor.GetUsageType(), processor.GetProducer()));
                    }
                }
            }
            String newProducer = BuildProducer(confirmedEvents);
            if (oldProducer == null || String.IsNullOrEmpty(oldProducer)) {
                return newProducer;
            }
            else {
                //if the last time document was modified or created with the itext of the same version,
                //then no changes occur.
                if (oldProducer.Equals(newProducer) || oldProducer.EndsWith(MODIFIED_USING + newProducer)) {
                    return oldProducer;
                }
                else {
                    return oldProducer + MODIFIED_USING + newProducer;
                }
            }
        }

        /// <summary>Configuration events for util internal purposes are not expected to be sent.</summary>
        protected internal override void DoAction() {
            throw new InvalidOperationException("Configuration events for util internal purposes are not expected to be sent"
                );
        }

        private static String BuildProducer(IList<ConfirmedEventWrapper> events) {
            if (events == null || events.IsEmpty()) {
                throw new ArgumentException(CommonsExceptionMessageConstant.NO_EVENTS_WERE_REGISTERED_FOR_THE_DOCUMENT);
            }
            // we expects here that the first event was thrown by
            // the addon which may be considered as entry point of
            // document processing
            String producer = events[0].GetProducerLine();
            return PopulatePlaceholders(producer, events);
        }

        private static String PopulatePlaceholders(String producerLine, IList<ConfirmedEventWrapper> events) {
            int lastIndex = 0;
            Matcher matcher = iText.Commons.Utils.Matcher.Match(PATTERN, producerLine);
            StringBuilder builder = new StringBuilder();
            while (matcher.Find()) {
                builder.Append(producerLine.JSubstring(lastIndex, matcher.Start()));
                lastIndex = matcher.End();
                String placeholder = matcher.Group(1);
                int delimiterPosition = placeholder.IndexOf(FORMAT_DELIMITER);
                String placeholderName;
                String parameter = null;
                if (placeholder.IndexOf(FORMAT_DELIMITER) == -1) {
                    placeholderName = placeholder;
                }
                else {
                    placeholderName = placeholder.JSubstring(0, delimiterPosition);
                    parameter = placeholder.Substring(delimiterPosition + 1);
                }
                IPlaceholderPopulator populator = PLACEHOLDER_POPULATORS.Get(placeholderName);
                if (populator == null) {
                    LOGGER.LogInformation(MessageFormatUtil.Format(CommonsLogMessageConstant.UNKNOWN_PLACEHOLDER_WAS_IGNORED, 
                        placeholderName));
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
