using UnityEngine;
using System.Linq;

namespace Practear.Utils
{
    /// <summary>
    /// Basic abstract class to create a singleton class in Unity.
    /// Only one instance can exist in the scene at a given time.
    /// If no instance is present in the scene, 
    /// one will automatically be created at the first call of <see cref="Instance"/>
    /// </summary>
    /// <typeparam name="T">The type of the singleton.</typeparam>
    public class AbstractSingleton<T> : MonoBehaviour where T : AbstractSingleton<T>
    {

        #region Static variables

        /// <summary>
        /// The instance of this singleton.
        /// </summary>
        static private T s_Instance;

        #endregion // Static variables        

        #region Static properties

        /// <summary>
        /// Access the instance of this singleton.
        /// If no instance can be found, one will be created.
        /// </summary>
        static public T Instance
        {
            get
            {
                // If instance is null, try to find an instance in the scene. 
                if (s_Instance == null)
                {
                    var objectsInScene = FindObjectsOfType<T>();
                    if (objectsInScene.Length > 1)
                    {
                        Debug.LogWarning(string.Format("There is more that one instance of {0} in the scene. " +
                            "This should never happen and can lead to undesired behaviours.", typeof(T).Name));

                        // use the first on by default
                        s_Instance = objectsInScene.First();
                    }
                    else if (objectsInScene.Length == 1)
                    {
                        s_Instance = objectsInScene.First();
                    }
                    else
                    {
                        // If there are no instance of the singleton in the scene, create one.
                        GameObject gameObject = new GameObject(typeof(T).Name);
                        s_Instance = gameObject.AddComponent<T>();
                    }

                }

                return s_Instance;
            }
        }

        #endregion // Static properties

        #region Instance variables

        /// <summary>
        /// Whether or not to keep the instance alive across scenes.
        /// </summary>
        [Tooltip("Whether or not to keep the instance alive across scenes.")]
        public new bool DontDestroyOnLoad;

        #endregion // Instance variables

        #region Methods

        /// <summary>
        /// Called on awake of the <see cref="MonoBehaviour"/>
        /// Careful when overriding this method in derived classes.
        /// </summary>        
        virtual public void Awake()
        {
            if (s_Instance == null)
            {
                // If the instance is null at this point, it means that no calls to .Instance have been done yet.
                // Assign the instance manually to avoid the cost of FindGameObjectsOfType.
                s_Instance = (T)this;
            }
            else if (s_Instance != this)
            {
                // Destroy the instance if another one already exists.
                Debug.LogWarning(string.Format("An instance of {0} already exists. Destroy this one.", typeof(T).Name));
                Destroy(this);
                return;
            }

            if (DontDestroyOnLoad)
            {
                // The gameobject must be at the root of the hierarchy for "DontDestroyOnLoad" to work.
                transform.SetParent(null);
                DontDestroyOnLoad(this);
            }
        }

        #endregion // Methods

    }
}

