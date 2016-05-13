using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace iTextSharp.IO.Util {
    public sealed class ResourceUtil
    {
        private static List<object> resourceSearch = new List<object>();

        private ResourceUtil()
        {
        }

        public static void AddToResourceSearch(object obj)
        {
            lock (resourceSearch)
            {
                if (obj is Assembly)
                {
                    resourceSearch.Add(obj);
                }
                else if (obj is string)
                {
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
            Stream istr = null;
            // Try to use resource loader to load the properties file.
            try
            {
                Assembly assm = Assembly.GetExecutingAssembly();
                istr = assm.GetManifestResourceStream(key);
            }
            catch
            {
            }
            if (istr != null)
                return istr;
            int count;
            lock (resourceSearch)
            {
                count = resourceSearch.Count;
            }
            for (int k = 0; k < count; ++k)
            {
                object obj;
                lock (resourceSearch)
                {
                    obj = resourceSearch[k];
                }
                try
                {
                    if (obj is Assembly)
                    {
                        istr = ((Assembly) obj).GetManifestResourceStream(key);
                        if (istr != null)
                            return istr;
                    }
                    else if (obj is string)
                    {
                        string dir = (string) obj;
                        try
                        {
                            istr = Assembly.LoadFrom(dir).GetManifestResourceStream(key);
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
            }

            return istr;
        }
    }
}

