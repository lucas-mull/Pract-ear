using System.Collections.Generic;

namespace Practear.Utils.Extensions
{
    static public class IEnumerableExt
    {

        #region Static methods

        /// <summary>
        /// Use this method to find the index of an element in a <see cref="IEnumerable{T}" /> collection.
        /// </summary>
        /// <typeparam name="T">The type within the collection</typeparam>
        /// <param name="collection">The collection of elements</param>
        /// <param name="element">The element to find.</param>
        /// <returns>The index of the element if it was found in the collection, -1 otherwise.</returns>
        static public int IndexOf<T>(this IEnumerable<T> collection, T element)
        {
            int index = 0;
            foreach(T obj in collection)
            {
                if (element.Equals(obj))
                    return index;

                index++;
            }

            return -1;
        }

        #endregion // Static methods

    }
}

