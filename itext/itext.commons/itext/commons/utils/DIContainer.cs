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
using System.Collections.Concurrent;

namespace iText.Commons.Utils {
    /// <summary>A simple dependency injection container.</summary>
    /// <remarks>
    /// A simple dependency injection container.
    /// <para />
    /// The container is thread-safe.
    /// </remarks>
    public class DIContainer {
        private static readonly ConcurrentDictionary<Type, Func<Object>> instances = new ConcurrentDictionary<Type
            , Func<Object>>();

        private readonly ConcurrentDictionary<Type, Object> localInstances = new ConcurrentDictionary<Type, Object
            >();

        static DIContainer() {
            DIContainerConfigurations.LoadDefaultConfigurations();
        }

        /// <summary>
        /// Creates a new instance of
        /// <see cref="DIContainer"/>.
        /// </summary>
        public DIContainer() {
        }

        // Empty constructor
        /// <summary>Registers a default instance for a class.</summary>
        /// <param name="clazz">the class</param>
        /// <param name="supplier">supplier of the instance</param>
        public static void RegisterDefault(Type clazz, Func<Object> supplier) {
            instances.Put(clazz, supplier);
        }

        /// <summary>Registers an instance for a class.</summary>
        /// <param name="clazz">the class</param>
        /// <param name="inst">the instance</param>
        public virtual void Register(Type clazz, Object inst) {
            localInstances.Put(clazz, inst);
        }

        /// <summary>Gets an instance of a class.</summary>
        /// <param name="clazz">the class</param>
        /// <typeparam name="T">the type of the class</typeparam>
        /// <returns>the instance</returns>
        public virtual T GetInstance<T>() {
            System.Type clazz = typeof(T);
            Object supplier = localInstances.Get(clazz);
            if (supplier == null) {
                supplier = instances.Get(clazz)();
            }
            if (supplier == null) {
                throw new Exception("No instance registered for class " + clazz.FullName);
            }
            return (T)supplier;
        }

        /// <summary>Checks if an instance is registered for a class.</summary>
        /// <remarks>
        /// Checks if an instance is registered for a class.
        /// If the class is registered but the value is null, it will still return
        /// <see langword="true"/>.
        /// </remarks>
        /// <param name="clazz">the class</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if an instance is registered,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool IsRegistered(Type clazz) {
            return localInstances.ContainsKey(clazz) || instances.ContainsKey(clazz);
        }
    }
}
