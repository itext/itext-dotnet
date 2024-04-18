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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using iText.Commons.Utils;
#if NETSTANDARD2_0
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.DependencyModel;
#endif

namespace iText.IO.Util {
    /// <summary>
    /// This file is a helper class for internal usage only.
    /// Be aware that its API and functionality may be changed in future.
    /// </summary>
    public static class ResourceUtil {

        private static List<object> resourceSearch = new List<object>();
        private static ISet<string> iTextResourceAssemblyNames;

        static ResourceUtil() {
            iTextResourceAssemblyNames = new HashSet<string>();
            iTextResourceAssemblyNames.Add("itext.hyph");
            iTextResourceAssemblyNames.Add("itext.font_asian");

            LoadITextResourceAssemblies();
        }
        

        public static void AddToResourceSearch(object obj) {
            lock (resourceSearch) {
                if (obj is Assembly) {
                    resourceSearch.Add(obj);
                } else if (obj is string) {
                    string f = (string) obj;
                    if (Directory.Exists(f) || File.Exists(f))
                        resourceSearch.Add(obj);
                }
            }
        }

        /// <summary>Gets the resource's inputstream.</summary>
        /// <param name="key">the full name of the resource.</param>
        /// <returns>
        /// the
        /// <c>InputStream</c>
        /// to get the resource or
        /// <see langword="null"/>
        /// if not found.
        /// </returns>
        public static Stream GetResourceStream(string key)
        {
            return GetResourceStream(key, null);
        }

        public static Stream GetResourceStream(string key, Type definedClassType) {
            Stream istr = null;
            // Try to use resource loader to load the properties file.
            try {
                Assembly assm = definedClassType != null ? definedClassType.GetAssembly() : typeof(ResourceUtil).GetAssembly();
                istr = assm.GetManifestResourceStream(key);
            } catch {
            }
            if (istr != null)
                return istr;

            int count;
            lock (resourceSearch) {
                count = resourceSearch.Count;
            }
            for (int k = 0; k < count; ++k) {
                object obj;
                lock (resourceSearch) {
                    obj = resourceSearch[k];
                }
                istr = SearchResourceInAssembly(key, obj);
                if (istr != null) {
                    return istr;
                }
            }

#if NETSTANDARD2_0
            try {
                if (DependencyContext.Default != null) {
                    string runtimeId = RuntimeEnvironment.GetRuntimeIdentifier();
                    IEnumerable<AssemblyName> loadedAssemblies = DependencyContext.Default.GetRuntimeAssemblyNames(runtimeId).ToList();
                    foreach (AssemblyName assemblyName in loadedAssemblies) {
                        if (assemblyName.Name.StartsWith("itext")) {
                            try {
                                Assembly assembly = Assembly.Load(assemblyName);
                                istr = SearchResourceInAssembly(key, assembly);
                                if (istr != null) {
                                    return istr;
                                }
                            } catch { }
                        }
                    }
                }
            } catch { }
#endif
            // Fallback to old approach. The one from above might not work in Azure function.
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                if (assembly.GetName().Name.StartsWith("itext")) {
                    istr = SearchResourceInAssembly(key, assembly);
                    if (istr != null) {
                        return istr;
                    }
                }
            }

            return istr;
        }

        private static Stream SearchResourceInAssembly(string key, Object obj) {
            Stream istr = null;
            try
            {
                if (obj is Assembly)
                {
                    istr = ((Assembly)obj).GetManifestResourceStream(key);
                    if (istr != null)
                        return istr;
                }
                else if (obj is string)
                {
                    string dir = (string)obj;
                    try
                    {
#if !NETSTANDARD2_0
                        istr = Assembly.LoadFrom(dir).GetManifestResourceStream(key);
#else
                        istr = AssemblyLoadContextUtil.LoadFromDefaultContextAssemblyPath(key).GetManifestResourceStream(key);
#endif
                    }
                    catch
                    {
                    }
                    if (istr != null)
                        return istr;
                    string modkey = key.Replace('.', '/');
                    string fullPath = Path.Combine(dir, modkey);
                    if (File.Exists(fullPath))
                    {
                        return new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    }
                    int idx = modkey.LastIndexOf('/');
                    if (idx >= 0)
                    {
                        modkey = modkey.Substring(0, idx) + "." + modkey.Substring(idx + 1);
                        fullPath = Path.Combine(dir, modkey);
                        if (File.Exists(fullPath))
                            return new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    }
                }
            }
            catch
            {
            }
            return istr;
        }

        private static void LoadITextResourceAssemblies() {
#if !NETSTANDARD2_0
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where( a=> !a.IsDynamic).ToList();
            List<string> loadedPaths = new List<string>();
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var path = a.Location;
                    loadedPaths.Add(path);
                }
                catch
                {
                    // to skip exceptions for dynamically loaded assemblies without location
                    // such as anonymously hosted dynamicmethods assembly for example
                }
            }

            var referencedPaths = Directory.GetFiles(FileUtil.GetBaseDirectory(), "*.dll");
            var toLoad = referencedPaths.Where(referencePath => !loadedPaths.Any(loadedPath => loadedPath.Equals(referencePath, StringComparison.OrdinalIgnoreCase))).ToList();
            foreach (String path in toLoad)
            {
                try {
                    AssemblyName name = AssemblyName.GetAssemblyName(path);
                    if (iTextResourceAssemblyNames.Contains(name.Name) && !loadedAssemblies.Any(assembly => assembly.GetName().Name.Equals(name.Name))) {
                        loadedAssemblies.Add(AppDomain.CurrentDomain.Load(name));
                    }
                }
                catch
                {
                }
            }
#else
            string runtimeId = RuntimeEnvironment.GetRuntimeIdentifier();
            List<AssemblyName> loadedAssemblies = null;
            try {
                loadedAssemblies = DependencyContext.Default?.GetRuntimeAssemblyNames(runtimeId).ToList();
            } catch { }
            if (loadedAssemblies == null) {
                loadedAssemblies = new List<AssemblyName>();
            }

            if (FileUtil.GetBaseDirectory() != null) {
                var referencedPaths = Directory.GetFiles(FileUtil.GetBaseDirectory(), "*.dll");
                foreach (String path in referencedPaths)
                {
                    try
                    {
                        AssemblyName name = AssemblyLoadContextUtil.GetAssemblyName(path);
                        if (iTextResourceAssemblyNames.Contains(name.Name) && !loadedAssemblies.Any(assembly => assembly.Name.Equals(name.Name))) {
                            Assembly newAssembly = AssemblyLoadContextUtil.LoadFromDefaultContextAssemblyPath(path);
                            loadedAssemblies.Add(newAssembly.GetName());
                        }
                    }
                    catch
                    {
                    }
                }
            }
#endif
        }
    }
}

