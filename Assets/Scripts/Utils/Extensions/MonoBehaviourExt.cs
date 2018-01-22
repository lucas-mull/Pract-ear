using UnityEngine;

namespace Practear.Utils.Extensions
{
    /// <summary>
    /// Extension methods for the <see cref="Component"/> class.
    /// </summary>
    static public class ComponentExt
    {

        #region Static methods

        /// <summary>
        /// Get a component from a game object, or add one if there is none yet.
        /// </summary>
        /// <typeparam name="T">The type of the component</typeparam>
        /// <param name="component">The target component</param>
        /// <returns>The component (existing or newly created).</returns>
        static public T GetOrAddComponent<T>(this Component component) where T : Component
        {
            T targetComponent = component.GetComponent<T>();
            if (!targetComponent)
            {
                targetComponent = component.gameObject.AddComponent<T>();
            }

            return targetComponent;
        }

        #endregion // Static methods

    }
}
