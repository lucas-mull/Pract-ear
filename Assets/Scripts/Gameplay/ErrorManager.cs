using Practear.Utils;
using Practear.Utils.PropertyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace Practear.Gameplay
{
    /// <summary>
    /// Singleton class to centralize everything error related.
    /// </summary>
    public class ErrorManager : AbstractSingleton<ErrorManager>
    {

        #region Instance variables

        /// <summary>
        /// Where the errors objects will be instantiated.
        /// </summary>
        [Tooltip("Where the errors objects will be instantiated.")]
        [SerializeField]
        private RectTransform m_ErrorContainer;

        /// <summary>
        /// The error prefab.
        /// </summary>
        [Tooltip("The error prefab.")]
        [SerializeField]
        private GameObject m_ErrorPrefab;

        /// <summary>
        /// The name of the trigger for the "win" animation.
        /// </summary>
        [Tooltip("The name of the trigger for the 'win' animation.")]
        [SerializeField]
        [EnableIf("m_ErrorPrefab", HideIfDisabled = true)]
        private string m_SuccessTrigger;

        /// <summary>
        /// The name of the trigger for the "loss" animation.
        /// </summary>
        [Tooltip("The name of the trigger for the 'loss' animation.")]
        [SerializeField]
        [EnableIf("m_ErrorPrefab", HideIfDisabled = true)]
        private string m_ErrorTrigger;

        /// <summary>
        /// The list of errors currently instantiated.
        /// </summary>
        private IList<GameObject> m_InstantiatedErrors;

        #endregion // Instance variables

        #region Properties

        /// <summary>
        /// The number of allowed errors before a game over.
        /// </summary>
        public int ErrorsAllowed { get; set; }

        #endregion // Properties

        #region Methods

        /// <summary>
        /// <see cref="AbstractSingleton{T}.Awake"/>
        /// </summary>
        public override void Awake()
        {
            base.Awake();
            m_InstantiatedErrors = new List<GameObject>();
        }

        /// <summary>
        /// Initialize the errors by instantiating the correct number.
        /// </summary>
        /// <param name="maxErrors"></param>
        public void InitializeErrors(int maxErrors)
        {
            ErrorsAllowed = maxErrors >= 0 ? maxErrors : 0;
            if (m_ErrorPrefab && m_ErrorContainer)
            {
                for (int i = 0; i < ErrorsAllowed; i++)
                {
                    m_InstantiatedErrors.Add(Instantiate(m_ErrorPrefab, m_ErrorContainer));
                }
            }
            else
            {
                Debug.LogWarning("You need to supply both the error prefab and the container in the ErrorManager");
            }
        }

        /// <summary>
        /// Call this from another class to trigger the error animations.
        /// </summary>
        public void OnError()
        {
            TryTriggerAnimation(m_ErrorTrigger);
        }

        /// <summary>
        /// Call this from another class to trigger the success animations.
        /// </summary>
        public void OnSuccess()
        {
            TryTriggerAnimation(m_SuccessTrigger);
        }

        /// <summary>
        /// Try to trigger a specific animation on the instantiated error objects.
        /// </summary>
        /// <param name="triggerName">The name of the trigger.</param>
        private void TryTriggerAnimation(string triggerName)
        {
            foreach (GameObject error in m_InstantiatedErrors)
            {
                // Try triggering the error animation if possible.
                Animator animator = error.GetComponent<Animator>();
                if (animator && !string.IsNullOrEmpty(triggerName))
                {
                    animator.SetTrigger(triggerName);
                }
            }
        }

        #endregion // Methods

    }
}
