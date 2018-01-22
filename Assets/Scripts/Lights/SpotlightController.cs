using Practear.Utils.PropertyAttributes;
using UnityEngine;

namespace Practear.Lights
{
    /// <summary>
    /// Simple class used to control a spotlight.
    /// </summary>
    [RequireComponent(typeof(Light))]
    public class SpotlightController : MonoBehaviour
    {

        #region Instance variables

        /// <summary>
        /// The animator used to animate the spotlight.
        /// </summary>
        [Tooltip("The animator used to animate the spotlight.")]
        [SerializeField]
        private Animator m_Animator;

        /// <summary>
        /// The name of the boolean used to switch the light on in the animator.
        /// </summary>
        [Tooltip("The name of the trigger used to switch the light on in the animator.")]
        [SerializeField]
        [AnimatorVariable(AnimatorControllerParameterType.Bool, AnimatorVariable = "m_Animator")]
        private string m_SwitchLightBool;

        /// <summary>
        /// The sprite used for the light.
        /// </summary>
        [Tooltip("The sprite used for the light.")]
        [SerializeField]
        private SpriteRenderer m_LightSprite;

        /// <summary>
        /// The point light attached to this object.
        /// </summary>
        [Tooltip("The point light attached to this object.")]
        [SerializeField]
        private Light m_PointLight;

        #endregion // Instance variables

        #region Methods

        /// <summary>
        /// Message sent by the <see cref="SpotlightManager"/> whenever this light has been turned on or off.
        /// </summary>
        /// <param name="isOn">Is the light on ?</param>
        private void OnSwitchLight(bool isOn)
        {
            m_Animator.SetBool(m_SwitchLightBool, isOn);
            m_PointLight.enabled = isOn;
        }

        /// <summary>
        /// Called by the <see cref="SpotlightManager"/> whenever this light's color changes.
        /// </summary>
        /// <param name="newColor">The new color</param>
        private void OnSwitchLightColor(Color newColor)
        {
            if (m_LightSprite)
            {
                m_LightSprite.color = newColor;
            }

            m_PointLight.color = newColor;
        }

        #endregion // Methods

    }

}

