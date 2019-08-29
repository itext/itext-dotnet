/*
* Licensed to the Apache Software Foundation (ASF) under one or more
* contributor license agreements.  See the NOTICE file distributed with
* this work for additional information regarding copyright ownership.
* The ASF licenses this file to You under the Apache License, Version 2.0
* (the "License"); you may not use this file except in compliance with
* the License.  You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
using System;
using System.Collections.Generic;

namespace iText.Layout.Hyphenation {
    /// <summary>This is a cache for HyphenationTree instances.</summary>
    public class HyphenationTreeCache {
        /// <summary>Contains the cached hyphenation trees</summary>
        private IDictionary<String, HyphenationTree> hyphenTrees = new Dictionary<String, HyphenationTree>();

        /// <summary>Used to avoid multiple error messages for the same language if a pattern file is missing.</summary>
        private ICollection<String> missingHyphenationTrees;

        /// <summary>Looks in the cache if a hyphenation tree is available and returns it if it is found.</summary>
        /// <param name="lang">the language</param>
        /// <param name="country">the country (may be null or "none")</param>
        /// <returns>the HyhenationTree instance or null if it's not in the cache</returns>
        public virtual HyphenationTree GetHyphenationTree(String lang, String country) {
            String key = ConstructLlccKey(lang, country);
            if (key == null) {
                return null;
            }
            // first try to find it in the cache
            if (hyphenTrees.ContainsKey(key)) {
                return hyphenTrees.Get(key);
            }
            else {
                if (hyphenTrees.ContainsKey(lang)) {
                    return hyphenTrees.Get(lang);
                }
                else {
                    return null;
                }
            }
        }

        /// <summary>Constructs the key for the hyphenation pattern file.</summary>
        /// <param name="lang">the language</param>
        /// <param name="country">the country (may be null or "none")</param>
        /// <returns>the resulting key</returns>
        public static String ConstructLlccKey(String lang, String country) {
            String key = lang;
            // check whether the country code has been used
            if (country != null && !country.Equals("none")) {
                key += "_" + country;
            }
            return key;
        }

        /// <summary>
        /// If the user configured a hyphenation pattern file name
        /// for this (lang,country) value, return it.
        /// </summary>
        /// <remarks>
        /// If the user configured a hyphenation pattern file name
        /// for this (lang,country) value, return it. If not, return null.
        /// </remarks>
        /// <param name="lang">the language</param>
        /// <param name="country">the country (may be null or "none")</param>
        /// <param name="hyphPatNames">the map of user-configured hyphenation pattern file names</param>
        /// <returns>the hyphenation pattern file name or null</returns>
        public static String ConstructUserKey(String lang, String country, IDictionary<String, String> hyphPatNames
            ) {
            String userKey = null;
            if (hyphPatNames != null) {
                String key = ConstructLlccKey(lang, country);
                key = key.Replace('_', '-');
                userKey = hyphPatNames.Get(key);
            }
            return userKey;
        }

        /// <summary>Cache a hyphenation tree under its key.</summary>
        /// <param name="key">the key (ex. "de_CH" or "en")</param>
        /// <param name="hTree">the hyphenation tree</param>
        public virtual void Cache(String key, HyphenationTree hTree) {
            hyphenTrees.Put(key, hTree);
        }

        /// <summary>Notes a key to a hyphenation tree as missing.</summary>
        /// <remarks>
        /// Notes a key to a hyphenation tree as missing.
        /// This is to avoid searching a second time for a hyphenation pattern file which is not
        /// available.
        /// </remarks>
        /// <param name="key">the key (ex. "de_CH" or "en")</param>
        public virtual void NoteMissing(String key) {
            if (missingHyphenationTrees == null) {
                missingHyphenationTrees = new HashSet<String>();
            }
            missingHyphenationTrees.Add(key);
        }

        /// <summary>Indicates whether a hyphenation file has been requested before but it wasn't available.</summary>
        /// <remarks>
        /// Indicates whether a hyphenation file has been requested before but it wasn't available.
        /// This is to avoid searching a second time for a hyphenation pattern file which is not
        /// available.
        /// </remarks>
        /// <param name="key">the key (ex. "de_CH" or "en")</param>
        /// <returns>true if the hyphenation tree is unavailable</returns>
        public virtual bool IsMissing(String key) {
            return (missingHyphenationTrees != null && missingHyphenationTrees.Contains(key));
        }
    }
}
