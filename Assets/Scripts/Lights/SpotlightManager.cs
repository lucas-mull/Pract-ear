using Practear.Utils;
using Practear.Utils.Extensions;
using System;
using UnityEngine;
using Practear.Utils.PropertyAttributes;
using System.Collections.Generic;

namespace Practear.Lights
{
    /// <summary>
    /// Simple manager class that holds neutral, positive and negative colors for spotlights in the game.
    /// Also holds methods for easy toggle of spotlights (with our without an audioclip).
    /// </summary>
    sealed public class SpotlightManager : AbstractSingleton<SpotlightManager>
    {

        #region Instance variables

        /// <summary>
        /// The list of spotlights
        /// </summary>
        [Tooltip("The list of spotlights")]
        [SerializeField]
        private Light[] m_SpotLights;

        /// <summary>
        /// The neutral color
        /// </summary>
        [Tooltip("The neutral color")]
        [SerializeField]
        [Button("SwitchAllNeutral", Label = "Test", Placement = ButtonAttribute.EButtonPlacement.Inline)]
        private Color m_NeutralColor;

        /// <summary>
        /// The positive color
        /// </summary>
        [Tooltip("The positive color")]
        [SerializeField]
        [Button("SwitchAllPositive", Label = "Test", Placement = ButtonAttribute.EButtonPlacement.Inline)]
        private Color m_PositiveColor;

        /// <summary>
        /// The negative color
        /// </summary>
        [Tooltip("The negative color")]
        [SerializeField]
        [Button("SwitchAllNegative", Label = "Test", Placement = ButtonAttribute.EButtonPlacement.Inline)]
        private Color m_NegativeColor;

        /// <summary>
        /// The audio clip that plays whenever we enable or disable a spotlight.
        /// </summary>
        [Tooltip("The audio clip that plays whenever we enable or disable a spotlight.")]
        [SerializeField]
        private AudioClip m_SwitchAudioClip;

        #endregion // Instance variables

        #region Properties

        /// <summary>
        /// Acccess the list of lights.
        /// </summary>
        public IEnumerable<Light> Lights { get { return m_SpotLights; } }

        /// <summary>
        /// Access / set the neutral color
        /// </summary>
        public Color NeutralColor
        {
            get { return m_NeutralColor; }
            set { m_NeutralColor = value; }
        }

        /// <summary>
        /// Access / set the positive color
        /// </summary>
        public Color PositiveColor
        {
            get { return m_PositiveColor; }
            set { m_PositiveColor = value; }
        }

        /// <summary>
        /// Access / set the negative color
        /// </summary>
        public Color NegativeColor
        {
            get { return m_NegativeColor; }
            set { m_NegativeColor = value; }
        }

        #endregion // Properties

        #region Methods

        /// <summary>
        /// Enable or disable the spotlights
        /// </summary>
        /// <param name="state">The enable state</param>
        /// <param name="playToggleSound">Whether or not to play the toggle sound. True by default.</param>
        private void EnableAll(bool state, bool playToggleSound = true)
        {
            // Play the audioclip if any (from the manager's audio source).
            if (m_SwitchAudioClip && playToggleSound)
            {
                this.GetOrAddComponent<AudioSource>().PlayOneShot(m_SwitchAudioClip);
            }

            foreach(Light light in m_SpotLights)
            {
                EnableSpotlight(light, state, playToggleSound);
            }
        }

        /// <summary>
        /// Enable all the lights
        /// </summary>
        public void EnableAll(bool playToggleSound = true)
        {
            EnableAll(true, playToggleSound);
        }

        /// <summary>
        /// Disable all the lights.
        /// </summary>
        public void DisableAll(bool playToggleSound = true)
        {
            EnableAll(false, playToggleSound);
        }        

        /// <summary>
        /// Enable a target spotlight while playing the audio clip that was given
        /// </summary>
        /// <param name="light">The target light</param>
        /// <param name="state">The state of the light (disabled or enabled).</param>
        /// <param name="playToggleSound">Optional. Whether or not to play the toggle SFX.</param>
        private void EnableSpotlight(Light light, bool state, bool playToggleSound = true)
        {
            if (light == null)
                throw new ArgumentNullException("light", "Couldn't change light's state. The specified light is null.");

            if (playToggleSound && m_SwitchAudioClip)
            {
                light.GetOrAddComponent<AudioSource>().PlayOneShot(m_SwitchAudioClip);
            }

            light.enabled = state;
            light.SendMessage("OnSwitchLight", state, SendMessageOptions.DontRequireReceiver);
        }

        /// <summary>
        /// Enable the target spotlight
        /// </summary>
        /// <param name="light">The target spotlight</param>
        public void EnableSpotlight(Light light)
        {
            EnableSpotlight(light, true);
        }

        /// <summary>
        /// Disable the target spotlight
        /// </summary>
        /// <param name="light">The target spotlight</param>
        public void DisableSpotlight(Light light)
        {
            EnableSpotlight(light, false);
        }

        /// <summary>
        /// Switch the current state of a spotlight
        /// </summary>
        /// <param name="light">The target spotlight</param>
        public void ToggleSpotlight(Light light)
        {
            EnableSpotlight(light, !light.enabled);
        }

        /// <summary>
        /// Switch all of the spotlights colors to the given one.
        /// </summary>
        /// <param name="target">The target color</param>
        public void SwitchAllLightsColor(Color target)
        {
            foreach(Light light in m_SpotLights)
            {
                SwitchLightColor(light, target);
            }
        }

        /// <summary>
        /// Switch all the lights to the neutral color
        /// </summary>
        public void SwitchAllNeutral()
        {
            SwitchAllLightsColor(m_NeutralColor);
        }

        /// <summary>
        /// Switch all the lights to the positive color
        /// </summary>
        public void SwitchAllPositive()
        {
            SwitchAllLightsColor(m_PositiveColor);
        }

        /// <summary>
        /// Switch all the lights to the negative color
        /// </summary>
        public void SwitchAllNegative()
        {
            SwitchAllLightsColor(m_NegativeColor);
        }

        /// <summary>
        /// Apply a color to a light
        /// </summary>
        /// <param name="light">The light</param>
        /// <param name="target">The color</param>
        public void SwitchLightColor(Light light, Color target)
        {
            if (light == null)
            {
                throw new ArgumentNullException("light", "Couldn't switch the light's color. The specified light is null.");
            }

            light.color = target;
            light.SendMessage("OnSwitchLightColor", target, SendMessageOptions.DontRequireReceiver);
        }

        #endregion // Methods

    }
}


