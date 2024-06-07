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
using System.IO;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Util;

namespace iText.Layout.Hyphenation {
    /// <summary>This class is the main entry point to the hyphenation package.</summary>
    /// <remarks>
    /// This class is the main entry point to the hyphenation package.
    /// You can use only the static methods or create an instance.
    /// <para />
    /// This work was authored by Carlos Villegas (cav@uniscope.co.jp).
    /// </remarks>
    public sealed class Hyphenator {
        private const char SOFT_HYPHEN = '\u00ad';

        private static readonly Object staticLock = new Object();

        /// <summary>Logging instance.</summary>
        private static ILogger log = ITextLogManager.GetLogger(typeof(iText.Layout.Hyphenation.Hyphenator));

        private static HyphenationTreeCache hTreeCache;

        private static IList<String> additionalHyphenationFileDirectories;

        protected internal String lang;

        protected internal String country;

        internal int leftMin;

        internal int rightMin;

        internal IDictionary<String, String> hyphPathNames;

        /// <summary>Creates a new hyphenator.</summary>
        /// <param name="lang">the language</param>
        /// <param name="country">the optional country code (may be null or "none")</param>
        /// <param name="leftMin">the minimum number of characters before the hyphenation point</param>
        /// <param name="rightMin">the minimum number of characters after the hyphenation point</param>
        public Hyphenator(String lang, String country, int leftMin, int rightMin) {
            this.lang = lang;
            this.country = country;
            this.leftMin = leftMin;
            this.rightMin = rightMin;
        }

        /// <summary>Creates a new hyphenator.</summary>
        /// <param name="lang">the language</param>
        /// <param name="country">the optional country code (may be null or "none")</param>
        /// <param name="hyphPathNames">the map with user-configured hyphenation pattern file names</param>
        /// <param name="leftMin">the minimum number of characters before the hyphenation point</param>
        /// <param name="rightMin">the minimum number of characters after the hyphenation point</param>
        public Hyphenator(String lang, String country, int leftMin, int rightMin, IDictionary<String, String> hyphPathNames
            )
            : this(lang, country, leftMin, rightMin) {
            this.hyphPathNames = hyphPathNames;
        }

        /// <summary>Registers additional file directories.</summary>
        /// <param name="directory">directory to register</param>
        public static void RegisterAdditionalHyphenationFileDirectory(String directory) {
            lock (staticLock) {
                if (additionalHyphenationFileDirectories == null) {
                    additionalHyphenationFileDirectories = new List<String>();
                }
                additionalHyphenationFileDirectories.Add(directory);
            }
        }

        /// <summary>Returns the default hyphenation tree cache.</summary>
        /// <returns>the default (static) hyphenation tree cache</returns>
        public static HyphenationTreeCache GetHyphenationTreeCache() {
            lock (staticLock) {
                if (hTreeCache == null) {
                    hTreeCache = new HyphenationTreeCache();
                }
            }
            return hTreeCache;
        }

        /// <summary>Clears the default hyphenation tree cache.</summary>
        /// <remarks>Clears the default hyphenation tree cache. This method can be used if the underlying data files are changed at runtime.
        ///     </remarks>
        public static void ClearHyphenationTreeCache() {
            lock (staticLock) {
                hTreeCache = new HyphenationTreeCache();
            }
        }

        /// <summary>
        /// Returns a hyphenation tree for a given language and country,
        /// with fallback from (lang,country) to (lang).
        /// </summary>
        /// <remarks>
        /// Returns a hyphenation tree for a given language and country,
        /// with fallback from (lang,country) to (lang).
        /// The hyphenation trees are cached.
        /// </remarks>
        /// <param name="lang">the language</param>
        /// <param name="country">the country (may be null or "none")</param>
        /// <param name="hyphPathNames">the map with user-configured hyphenation pattern file names</param>
        /// <returns>the hyphenation tree</returns>
        public static HyphenationTree GetHyphenationTree(String lang, String country, IDictionary<String, String> 
            hyphPathNames) {
            String llccKey = HyphenationTreeCache.ConstructLlccKey(lang, country);
            HyphenationTreeCache cache = GetHyphenationTreeCache();
            // If this hyphenation tree has been registered as missing, return immediately
            if (cache.IsMissing(llccKey)) {
                return null;
            }
            HyphenationTree hTree = GetHyphenationTree2(lang, country, hyphPathNames);
            // fallback to lang only
            if (hTree == null && country != null && !country.Equals("none")) {
                String llKey = HyphenationTreeCache.ConstructLlccKey(lang, null);
                if (!cache.IsMissing(llKey)) {
                    hTree = GetHyphenationTree2(lang, null, hyphPathNames);
                    if (hTree != null && log.IsEnabled(LogLevel.Debug)) {
                        log.LogDebug("Couldn't find hyphenation pattern " + "for lang=\"" + lang + "\",country=\"" + country + "\"."
                             + " Using general language pattern " + "for lang=\"" + lang + "\" instead.");
                    }
                    if (hTree == null) {
                        // no fallback; register as missing
                        cache.NoteMissing(llKey);
                    }
                    else {
                        // also register for (lang,country)
                        cache.Cache(llccKey, hTree);
                    }
                }
            }
            if (hTree == null) {
                // (lang,country) and (lang) tried; register as missing
                cache.NoteMissing(llccKey);
                log.LogError("Couldn't find hyphenation pattern " + "for lang=\"" + lang + "\"" + (country != null && !country
                    .Equals("none") ? ",country=\"" + country + "\"" : "") + ".");
            }
            return hTree;
        }

        /// <summary>Returns a hyphenation tree for a given language and country.</summary>
        /// <remarks>Returns a hyphenation tree for a given language and country. The hyphenation trees are cached.</remarks>
        /// <param name="lang">the language</param>
        /// <param name="country">the country (may be null or "none")</param>
        /// <param name="hyphPathNames">the map with user-configured hyphenation pattern file names</param>
        /// <returns>the hyphenation tree</returns>
        public static HyphenationTree GetHyphenationTree2(String lang, String country, IDictionary<String, String>
             hyphPathNames) {
            String llccKey = HyphenationTreeCache.ConstructLlccKey(lang, country);
            HyphenationTreeCache cache = GetHyphenationTreeCache();
            HyphenationTree hTree;
            // first try to find it in the cache
            hTree = GetHyphenationTreeCache().GetHyphenationTree(lang, country);
            if (hTree != null) {
                return hTree;
            }
            String key = HyphenationTreeCache.ConstructUserKey(lang, country, hyphPathNames);
            if (key == null) {
                key = llccKey;
            }
            if (additionalHyphenationFileDirectories != null) {
                foreach (String dir in additionalHyphenationFileDirectories) {
                    hTree = GetHyphenationTree(dir, key);
                    if (hTree != null) {
                        break;
                    }
                }
            }
            if (hTree == null) {
                // get from the default directory
                Stream defaultHyphenationResourceStream = ResourceUtil.GetResourceStream(HyphenationConstants.HYPHENATION_DEFAULT_RESOURCE
                     + key + ".xml");
                if (defaultHyphenationResourceStream != null) {
                    hTree = GetHyphenationTree(defaultHyphenationResourceStream, key);
                }
            }
            // put it into the pattern cache
            if (hTree != null) {
                cache.Cache(llccKey, hTree);
            }
            return hTree;
        }

        /// <summary>Load tree from xml file using configuration settings.</summary>
        /// <param name="searchDirectory">the directory to search the file into</param>
        /// <param name="key">language key for the requested hyphenation file</param>
        /// <returns>the requested HyphenationTree or null if it is not available</returns>
        public static HyphenationTree GetHyphenationTree(String searchDirectory, String key) {
            // try the raw XML file
            String name = key + ".xml";
            try {
                Stream fis = FileUtil.GetInputStreamForFile(searchDirectory + System.IO.Path.DirectorySeparatorChar + name
                    );
                return GetHyphenationTree(fis, name);
            }
            catch (System.IO.IOException ioe) {
                if (log.IsEnabled(LogLevel.Debug)) {
                    log.LogDebug("I/O problem while trying to load " + name + ": " + ioe.Message);
                }
                return null;
            }
        }

        /// <summary>Load tree from the stream.</summary>
        /// <param name="in">the input stream to load the tree from</param>
        /// <param name="name">unique key representing country-language combination</param>
        /// <returns>the requested HyphenationTree or null if it is not available</returns>
        public static HyphenationTree GetHyphenationTree(Stream @in, String name) {
            if (@in == null) {
                return null;
            }
            HyphenationTree hTree;
            try {
                hTree = new HyphenationTree();
                hTree.LoadPatterns(@in, name);
            }
            catch (HyphenationException ex) {
                log.LogError("Can't load user patterns from XML file " + name + ": " + ex.Message);
                return null;
            }
            finally {
                try {
                    @in.Dispose();
                }
                catch (Exception) {
                }
            }
            return hTree;
        }

        /// <summary>Hyphenates a word.</summary>
        /// <param name="lang">the language</param>
        /// <param name="country">the optional country code (may be null or "none")</param>
        /// <param name="hyphPathNames">the map with user-configured hyphenation pattern file names</param>
        /// <param name="word">the word to hyphenate</param>
        /// <param name="leftMin">the minimum number of characters before the hyphenation point</param>
        /// <param name="rightMin">the minimum number of characters after the hyphenation point</param>
        /// <returns>the hyphenation result</returns>
        public static iText.Layout.Hyphenation.Hyphenation Hyphenate(String lang, String country, IDictionary<String
            , String> hyphPathNames, String word, int leftMin, int rightMin) {
            // If a word contains soft hyphens, then hyphenation based on soft hyphens has higher priority
            if (WordContainsSoftHyphens(word)) {
                return HyphenateBasedOnSoftHyphens(word, leftMin, rightMin);
            }
            else {
                HyphenationTree hTree = null;
                if (lang != null) {
                    hTree = GetHyphenationTree(lang, country, hyphPathNames);
                }
                return hTree != null ? hTree.Hyphenate(word, leftMin, rightMin) : null;
            }
        }

        /// <summary>Hyphenates a word.</summary>
        /// <param name="lang">the language</param>
        /// <param name="country">the optional country code (may be null or "none")</param>
        /// <param name="word">the word to hyphenate</param>
        /// <param name="leftMin">the minimum number of characters before the hyphenation point</param>
        /// <param name="rightMin">the minimum number of characters after the hyphenation point</param>
        /// <returns>the hyphenation result</returns>
        public static iText.Layout.Hyphenation.Hyphenation Hyphenate(String lang, String country, String word, int
             leftMin, int rightMin) {
            return Hyphenate(lang, country, null, word, leftMin, rightMin);
        }

        /// <summary>Hyphenates a word.</summary>
        /// <param name="word">the word to hyphenate</param>
        /// <returns>the hyphenation result</returns>
        public iText.Layout.Hyphenation.Hyphenation Hyphenate(String word) {
            return Hyphenate(lang, country, hyphPathNames, word, leftMin, rightMin);
        }

        private static bool WordContainsSoftHyphens(String word) {
            return word.IndexOf(SOFT_HYPHEN) >= 0;
        }

        private static iText.Layout.Hyphenation.Hyphenation HyphenateBasedOnSoftHyphens(String word, int leftMin, 
            int rightMin) {
            IList<int> softHyphens = new List<int>();
            int lastSoftHyphenIndex = -1;
            int curSoftHyphenIndex;
            while ((curSoftHyphenIndex = word.IndexOf(SOFT_HYPHEN, lastSoftHyphenIndex + 1)) > 0) {
                softHyphens.Add(curSoftHyphenIndex);
                lastSoftHyphenIndex = curSoftHyphenIndex;
            }
            int leftInd = 0;
            int rightInd = softHyphens.Count - 1;
            while (leftInd < softHyphens.Count && word.JSubstring(0, softHyphens[leftInd]).Replace(SOFT_HYPHEN.ToString
                (), "").Length < leftMin) {
                leftInd++;
            }
            while (rightInd >= 0 && word.Substring(softHyphens[rightInd] + 1).Replace(SOFT_HYPHEN.ToString(), "").Length
                 < rightMin) {
                rightInd--;
            }
            if (leftInd <= rightInd) {
                int[] hyphenationPoints = new int[rightInd - leftInd + 1];
                for (int i = leftInd; i <= rightInd; i++) {
                    hyphenationPoints[i - leftInd] = softHyphens[i];
                }
                return new iText.Layout.Hyphenation.Hyphenation(word, hyphenationPoints);
            }
            else {
                return null;
            }
        }
    }
}
