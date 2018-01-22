using System;
using System.Collections.Generic;
using System.Linq;

namespace Practear.Utils.Extensions
{
    static public class IEnumerableExt
    {

        #region Static variables

        /// <summary>
        /// Random number generator.
        /// </summary>
        static private Random s_Rng = new Random();

        #endregion // Static variables

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

        /// <summary>
        /// Randomly shuffle a collection of elements.
        /// <seealso cref="https://stackoverflow.com/questions/273313/randomize-a-listt"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection</typeparam>
        /// <param name="collection">The list of elements</param>
        /// <returns>A shuffled list.</returns>
        static public IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection)
        {
            List<T> tempList = collection.ToList();
            int n = tempList.Count;
            while (n > 1)
            {
                n--;
                int k = s_Rng.Next(n + 1);
                T value = tempList[k];
                tempList[k] = tempList[n];
                tempList[n] = value;
            }

            return tempList;
        }

        #endregion // Static methods

    }
}

