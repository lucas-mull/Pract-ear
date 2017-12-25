using UnityEngine;

namespace Practear.Utils.Extensions
{
    /// <summary>
    /// Extension methods for the <see cref="GameObject"/> class.
    /// </summary>
    static public class GameObjectExt
    {

        #region Static methods

        /// <summary>
        /// Get a component from a game object, or add one if there is none yet.
        /// </summary>
        /// <typeparam name="T">The type of the component</typeparam>
        /// <param name="go">The target gameobject</param>
        /// <returns>The component (existing or newly created).</returns>
        static public T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            T component = go.GetComponent<T>();
            if (!component)
            {
                component = go.AddComponent<T>();
            }

            return component;
        }

        #endregion // Static methods

    }
}
