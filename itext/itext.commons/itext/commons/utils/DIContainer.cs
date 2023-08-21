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
    }
}
