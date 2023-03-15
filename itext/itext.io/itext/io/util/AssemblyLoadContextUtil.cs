/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2023 Apryse Group NV
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
using System.Reflection;
using System.Text;

namespace iText.IO.Util
{
#if NETSTANDARD2_0
    /// <summary>This file is a helper class for internal usage only.</summary>
    /// <remarks>
    /// This file is a helper class for internal usage only.
    /// Be aware that its API and functionality may be changed in future.
    /// <br/>
    /// <br/>
    /// Ussage of this class may throw TypeInitializationException
    /// in .NET Standard enviroments that doesn't support System.Runtime.Loader
    /// </remarks>
    public static class AssemblyLoadContextUtil {

        public static AssemblyName GetAssemblyName(String path) {
            return System.Runtime.Loader.AssemblyLoadContext.GetAssemblyName(path);
        }

        public static Assembly LoadFromDefaultContextAssemblyPath(String path) {
            return System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
        }

        public static void RegisterUnloadingEvent(Action<object> onUnloading) {
            RegisterUnloadingEvent(onUnloading, null);
        }

        public static void RegisterUnloadingEvent(Action<object> onUnloading, Assembly assembly) {
            if (assembly == null) {
                System.Runtime.Loader.AssemblyLoadContext.Default.Unloading += onUnloading;
            } else {
                System.Runtime.Loader.AssemblyLoadContext.GetLoadContext(assembly).Unloading += onUnloading;
            }
        }
    }
#endif
}
