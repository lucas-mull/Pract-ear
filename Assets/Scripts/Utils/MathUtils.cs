using System;
using UnityEngine;

namespace Practear.Utils.Math
{
    /// <summary>
    /// Utils class for Math methods.
    /// </summary>
    static public class MathUtils
    {

        #region Methods

        /// <summary>
        /// Converts a value that is between a min and a max to be between a new min and max.
        /// Example : 
        /// - 0.5 is the value and is between 0 (min) and 1 (max).
        /// - Call <see cref="ConvertMinMaxValueToNewRange(0.5, 0, 1, 2, 3)"/> will return 2.5f.
        /// </summary>
        /// <param name="value">The current value</param>
        /// <param name="oldMin">The current min boundary</param>
        /// <param name="oldMax">The current max boundary</param>
        /// <param name="newMin">The new min boundary</param>
        /// <param name="newMax">The new max boundary</param>
        /// <param name="invertResult">
        ///     Optional. If set to true, the value will be inverted.
        ///     Example : 0.3f between 0 and 1 will become 0.7f.
        /// </param>
        /// <returns>The new value.</returns>
        static public float ConvertMinMaxValueToNewRange(float value, float oldMin, float oldMax, float newMin, float newMax, bool invertResult = false)
        {
            float oldRange = oldMax - oldMin;
            float newRange = newMax - newMin;

            if (oldRange <= 0 || newRange <= 0)
            {
                throw new ArgumentException("The min MUST be below the max! Check your values and try again");
            }

            float newValue = (((value - oldMin) * newRange) / oldRange) + newMin;
            if (invertResult)
            {
                newValue = newMax - (newValue - newMin);
            }

            return newValue;
        }

        /// <summary>
        /// Transform a rect transform to a rect in screen space.
        /// </summary>
        /// <param name="rectTransform">The rect transform.</param>
        /// <returns>The screen space rect.</returns>
        static public Rect RectTransformToScreenSpace(RectTransform rectTransform)
        {
            Vector2 size = Vector2.Scale(rectTransform.rect.size, rectTransform.lossyScale);

            return new Rect((Vector2)rectTransform.position - (size * 0.5f), size);
        }

        #endregion // Methods

    }
}