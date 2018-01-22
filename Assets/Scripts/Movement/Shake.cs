using UnityEngine;

namespace Practear.Movement
{
    /// <summary>
    /// Simple script to make a object shake.
    /// </summary>
    public class Shake : MonoBehaviour
    {

        #region Instance variables

        /// <summary>
        /// If this is checked, the object will always shake (as long as the script is active).
        /// </summary>
        [Tooltip("If this is checked, the object will always shake (as long as the script is active).")]
        [SerializeField]
        private bool m_AlwaysShake;

        /// <summary>
        /// Does shaking affect the position?
        /// </summary>
        [Tooltip("Does shaking affect the position?")]
        [SerializeField]
        private bool m_ShakePosition = true;

        /// <summary>
        /// Does shaking affect the rotation?
        /// </summary>
        [Tooltip("Does shaking affect the rotation?")]
        [SerializeField]
        private bool m_ShakeRotation = true;

        /// <summary>
        /// Define the rotation axes to freeze
        /// </summary>
        [Tooltip("Define the rotation axes to freeze")]
        [SerializeField]
        private FreezeTransformComponent m_FreezeRotationOptions;

        /// <summary>
        /// How much it shakes
        /// </summary>
        [Tooltip("How much it shakes")]
        [SerializeField]
        private float m_Amount = 0.1f;

        /// <summary>
        /// Used to store the initial position of the object.
        /// </summary>
        private Vector3 m_InitialPosition;

        /// <summary>
        /// Used to store the initial rotation of the object
        /// </summary>
        private Quaternion m_InitialRotation;

        /// <summary>
        /// The duration of the shake.
        /// </summary>
        private float m_ShakeDuration;

        /// <summary>
        /// Whether or not the object must be shaking.
        /// </summary>
        private bool m_MustShake;

        /// <summary>
        /// Current timer
        /// </summary>
        private float m_Timer;

        #endregion // Instance variables

        #region Methods

        /// <summary>
        /// Called on awake of this behaviour (first load).
        /// </summary>
        private void Awake()
        {
            m_InitialPosition = transform.position;
            m_InitialRotation = transform.rotation;

            if (m_AlwaysShake)
            {
                m_MustShake = true;
                m_ShakeDuration = -1f;
            }

            m_Timer = 0f;
        }

        /// <summary>
        /// Called once per frame for this behaviour.
        /// </summary>
        private void Update()
        {
            if (m_MustShake)
            {
                DoShake();

                if (m_ShakeDuration > 0f && m_Timer >= m_ShakeDuration)
                {
                    StopShaking();
                }
                else
                {
                    m_Timer += Time.deltaTime;
                }
            }
        }

        /// <summary>
        /// Shake the object following the given amount.
        /// </summary>
        private void DoShake()
        {
            if (m_ShakePosition)
            {
                transform.position = m_InitialPosition + Random.insideUnitSphere * m_Amount;
            }

            if (m_ShakeRotation)
            {
                float x = m_FreezeRotationOptions.X ? m_InitialRotation.x : m_InitialRotation.x + Random.Range(-m_Amount, m_Amount) * 0.2f;
                float y = m_FreezeRotationOptions.Y ? m_InitialRotation.y : m_InitialRotation.y + Random.Range(-m_Amount, m_Amount) * 0.2f;
                float z = m_FreezeRotationOptions.Z ? m_InitialRotation.z : m_InitialRotation.z + Random.Range(-m_Amount, m_Amount) * 0.2f;
                transform.rotation = new Quaternion(
                    x,
                    y,
                    z,
                    m_InitialRotation.w + Random.Range(-m_Amount, m_Amount) * 0.2f
                );
            }            
        }

        /// <summary>
        /// Shake the object
        /// </summary>
        /// <param name="duration">Optional. The duration of the shake. Default is -1 => shaking doesn't stop automatically.</param>        
        public void StartShaking(float duration = -1f)
        {
            m_Timer = 0f;
            m_ShakeDuration = duration;
            m_MustShake = true;
        }

        /// <summary>
        /// End the shaking process by resetting the position to the original.
        /// </summary>
        public void StopShaking()
        {
            m_MustShake = false;
            transform.position = m_InitialPosition;
            transform.rotation = m_InitialRotation;
            m_Timer = 0f;
        }

        #endregion // Methods

    }
}

