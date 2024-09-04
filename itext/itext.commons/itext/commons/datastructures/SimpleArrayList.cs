using System;
using System.Collections.Generic;

namespace iText.Commons.Datastructures {
    /// <summary>
    /// Portable implementation of
    /// <see cref="System.Collections.ArrayList{E}"/>.
    /// </summary>
    /// <typeparam name="T">the type of elements in this list</typeparam>
    public class SimpleArrayList<T> : ISimpleList<T> {
        private readonly List<T> list;

        /// <summary>
        /// Creates a new instance of
        /// <see cref="SimpleArrayList{T}"/>.
        /// </summary>
        public SimpleArrayList() {
            this.list = new List<T>();
        }

        /// <summary>
        /// Creates a new instance of
        /// <see cref="SimpleArrayList{T}"/>
        /// with the specified initial capacity.
        /// </summary>
        /// <param name="initialCapacity">the initial capacity of the list</param>
        public SimpleArrayList(int initialCapacity) {
            this.list = new List<T>(initialCapacity);
        }

        /// <summary><inheritDoc/></summary>
        public virtual void Add(T element) {
            list.Add(element);
        }

        /// <summary><inheritDoc/></summary>
        public virtual void Add(int index, T element) {
            list.Add(index, element);
        }

        /// <summary><inheritDoc/></summary>
        public virtual T Get(int index) {
            return list[index];
        }

        /// <summary><inheritDoc/></summary>
        public virtual T Set(int index, T element) {
            T value = list[index];
            list[index] = element;
            return value;
        }

        /// <summary><inheritDoc/></summary>
        public virtual int IndexOf(Object element) {
            return list.IndexOf((T)element);
        }

        /// <summary><inheritDoc/></summary>
        public virtual void Remove(int index) {
            list.JRemoveAt(index);
        }

        /// <summary><inheritDoc/></summary>
        public virtual int Size() {
            return list.Count;
        }

        /// <summary><inheritDoc/></summary>
        public virtual bool IsEmpty() {
            return list.IsEmpty();
        }
    }
}
