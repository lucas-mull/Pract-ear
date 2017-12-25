using Practear.Utils;
using Practear.Utils.Extensions;
using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
        private Color m_NeutralColor;

        /// <summary>
        /// The positive color
        /// </summary>
        [Tooltip("The positive color")]
        [SerializeField]
        private Color m_PositiveColor;

        /// <summary>
        /// The negative color
        /// </summary>
        [Tooltip("The negative color")]
        [SerializeField]
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
        /// <param name="state"></param>
        private void EnableAll(bool state)
        {
            // Play the audioclip if any (from the manager's audio source).
            if (m_SwitchAudioClip)
            {
                gameObject.GetOrAddComponent<AudioSource>().PlayOneShot(m_SwitchAudioClip);
            }

            foreach(Light light in m_SpotLights)
            {
                light.enabled = state;
            }
        }

        /// <summary>
        /// Enable all the lights
        /// </summary>
        public void EnableAll()
        {
            EnableAll(true);
        }

        /// <summary>
        /// Disable all the lights.
        /// </summary>
        public void DisableAll()
        {
            EnableAll(false);
        }        

        /// <summary>
        /// Enable a target spotlight while playing the audio clip that was given
        /// </summary>
        /// <param name="light">The target light</param>
        /// <param name="state">The state of the light (disabled or enabled).</param>
        private void EnableSpotlight(Light light, bool state)
        {
            if (light == null)
                throw new ArgumentNullException("light", "Couldn't change light's state. The specified light is null.");

            if (m_SwitchAudioClip)
            {
                light.gameObject.GetOrAddComponent<AudioSource>().PlayOneShot(m_SwitchAudioClip);
            }

            light.enabled = state;
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
                light.color = target;
            }
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
        }

        #endregion // Methods

    }

    #region Custom editor

#if UNITY_EDITOR

    /// <summary>
    /// Custom editor for <see cref="SpotlightManager"/>
    /// </summary>
    [CustomEditor(typeof(SpotlightManager))]
    public class SpotlightManagerEditor : Editor
    {
        
        #region Properties

        /// <summary>
        /// Access the target script
        /// </summary>
        public SpotlightManager Target { get { return (SpotlightManager)target; } }

        #endregion // Properties

        #region Methods

        /// <summary>
        /// <see cref="Editor.OnInspectorGUI"/>
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();
            DrawTestButtons();

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draw all the test buttons.
        /// </summary>
        private void DrawTestButtons()
        {
            DrawTestingButton("Test neutral", Target.NeutralColor);
            DrawTestingButton("Test positive", Target.PositiveColor);
            DrawTestingButton("Test negative", Target.NegativeColor);
        }

        /// <summary>
        /// Draw a GUI Button to test a given color for spotlights in the editor.
        /// </summary>
        /// <param name="label">The label of the button</param>
        /// <param name="target">The target color</param>
        private void DrawTestingButton(string label, Color target)
        {
            if (GUILayout.Button(label))
            {
                Target.SwitchAllLightsColor(target);
            }
        }

        #endregion // Methods

    }

#endif

#endregion // Custom editor
}


