namespace com.itextpdf.io.util
{
	public class GenericArray<T>
	{
		T[] array;

		public GenericArray(int size)
		{
			array = new T[size];
    	}

		public virtual T Get(int index)
		{
			return array[index];
		}

		public virtual T Set(int index, T element)
		{
			return array[index] = element;
		}
	}
}
