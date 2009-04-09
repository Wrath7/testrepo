using System;
using System.Collections;
using System.Text;

namespace Illuminate.Collections
{
	/// <summary>
	/// Base collection Class
	/// </summary>
	public class Collection : CollectionBase
	{
		/// <summary>
		/// Adds an object to the InnerList 
		/// </summary>
		/// <param name="o"></param>
		public void Add(object o)
		{
			this.InnerList.Add(o);
		}

		/// <summary>
		/// Checks to ensure an Index passed to a default property is not out of range
		/// </summary>
		/// <param name="Index"></param>
		protected void OutOfRangeCheck(int Index)
		{
			if (Index < 0 || Index > Count - 1)
			{
				throw new IndexOutOfRangeException();
			}
		}
	}
}
