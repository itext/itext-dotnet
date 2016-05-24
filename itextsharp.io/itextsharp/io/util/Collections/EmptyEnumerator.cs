using System;
using System.Collections;
using System.Collections.Generic;

namespace itextsharp.io.util.Collections
{
    internal class EmptyEnumerator<T> : IEnumerator<T>
    {
        public void Dispose() {
        }

        public bool MoveNext() {
            return false;
        }

        public void Reset() {
        }

        public T Current {
            get {
                throw new InvalidOperationException();
            }
        }

        object IEnumerator.Current {
            get { return Current; }
        }
    }

	internal class EmptyEnumerator : IEnumerator
	{
		public void Dispose() {
		}

		public bool MoveNext() {
			return false;
		}

		public void Reset() {
		}

		public object Current {
			get {
				throw new InvalidOperationException();
			}
		}

		object IEnumerator.Current {
			get { return Current; }
		}
	}
}
