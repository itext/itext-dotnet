/*

This file is part of the iText (R) project.
Copyright (c) 1998-2017 iText Group NV
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
using System.Collections.Generic;

namespace iText.Kernel.Log {
    /// <summary>Factory that creates a counter for every reader or writer class.</summary>
    /// <remarks>
    /// Factory that creates a counter for every reader or writer class.
    /// You can implement your own counter and declare it like this:
    /// <code>CounterFactory.getInstance().setCounter(new SysoCounter());</code>
    /// SysoCounter is just an example of a Counter implementation.
    /// It writes info about files being read and written to the System.out.
    /// <p>
    /// This functionality can be used to create metrics in a SaaS context.
    /// </remarks>
    public class CounterFactory {
        /// <summary>The singleton instance.</summary>
        private static iText.Kernel.Log.CounterFactory instance;

        /// <summary>The current counter implementation.</summary>
        private IList<Counter> counters = new List<Counter>();

        static CounterFactory() {
            instance = new iText.Kernel.Log.CounterFactory();
        }

        /// <summary>The empty constructor.</summary>
        private CounterFactory() {
            RegisterCounter(new DefaultCounter());
        }

        /// <summary>Returns the singleton instance of the factory.</summary>
        public static iText.Kernel.Log.CounterFactory GetInstance() {
            return instance;
        }

        /// <summary>Returns a list of registered counters for specific class.</summary>
        public static IList<Counter> GetCounters(Type cls) {
            List<Counter> result = new List<Counter>();
            foreach (Counter counter in GetInstance().counters) {
                Counter counterInstance = counter.GetCounter(cls);
                if (counterInstance != null) {
                    result.Add(counterInstance);
                }
            }
            return result;
        }

        /// <summary>Register new counter.</summary>
        public virtual void RegisterCounter(Counter counter) {
            counters.Add(counter);
        }
    }
}
