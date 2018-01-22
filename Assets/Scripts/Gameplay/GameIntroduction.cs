using Practear.Lights;
using System.Collections;
using UnityEngine;
using System.Linq;
using Practear.Utils.Extensions;
using TMPro;
using Practear.Utils.PropertyAttributes;

namespace Practear.Gameplay
{
    /// <summary>
    /// Script used to start a game with a short introduction (spotlights turning on and a countdown).
    /// </summary>
    public class GameIntroduction : MonoBehaviour
    {

        #region Instance variables

        /// <summary>
        /// The game to start.
        /// </summary>
        [Tooltip("The game to start.")]
        [SerializeField]
        private MiniGame m_TargetGame;

        /// <summary>
        /// The countdown time before starting the game.
        /// </summary>
        [Tooltip("The countdown time before starting the game.")]
        [SerializeField]
        [Range(0, 5)]
        private int m_CountdownTime = 5;

        /// <summary>
        /// The countdown text object.
        /// </summary>
        [Tooltip("The countdown text object.")]
        [SerializeField]
        private TMP_Text m_CountdownText;

        /// <summary>
        /// The animator used for the countdown.
        /// </summary>
        [Tooltip("The animator used for the countdown.")]
        [SerializeField]
        private Animator m_CountdownAnimator;

        /// <summary>
        /// The trigger to use each second for the countdown.
        /// </summary>
        [Tooltip("The trigger to use each second for the countdown.")]
        [SerializeField]
        [AnimatorVariable(AnimatorControllerParameterType.Trigger, AnimatorVariable = "m_CountdownAnimator")]
        private string m_ShowCountdownTrigger;        

        #endregion // Instance variables

        #region Properties

        /// <summary>
        /// Property that sets the countdown time and the countdown text in one go.
        /// </summary>
        private int CountdownTime
        {
            get { return m_CountdownTime; }
            set
            {
                m_CountdownTime = value;
                AnimateCountdown();
            }
        }

        #endregion // Properties

        #region Methods

        /// <summary>
        /// Called when this script is first loaded.
        /// </summary>
        private void Awake()
        {
            if (!m_TargetGame)
                throw new UnassignedReferenceException("You must specify a target mini game.");

            SpotlightManager.Instance.DisableAll(false);
            if (m_CountdownText)
                m_CountdownText.enabled = false;

            // Countdown must last until all lights are enabled.
            m_CountdownTime = Mathf.Max(m_CountdownTime, SpotlightManager.Instance.Lights.Count());
        }

        /// <summary>
        /// Called on start.
        /// </summary>
        private IEnumerator Start()
        {
            // Shuffle the lights to enable them in a random order.
            Light[] shuffledLights = SpotlightManager.Instance.Lights.Shuffle().ToArray();
            
            yield return new WaitForSeconds(1f);

            // Show first countdown text.
            if (m_CountdownText)
            {
                m_CountdownText.enabled = true;
                AnimateCountdown();
            }

            // Enable spotlights one after the other.
            foreach (Light spot in shuffledLights)
            {
                SpotlightManager.Instance.EnableSpotlight(spot);
                yield return new WaitForSeconds(1f);

                CountdownTime--;
            }

            // If countdown is not done by then, keep going.
            while (CountdownTime > 0)
            {
                yield return new WaitForSeconds(1f);
                CountdownTime--;
            }

            // Disable countdown text.
            if (m_CountdownText)
                m_CountdownText.enabled = false;

            // Begin the game
            m_TargetGame.Begin();
        }

        /// <summary>
        /// Animate the countdown text.
        /// </summary>
        private void AnimateCountdown()
        {
            if (!m_CountdownText)
                return;

            m_CountdownText.text = m_CountdownTime.ToString();
            if (m_CountdownAnimator)
            {
                m_CountdownAnimator.SetTrigger(m_ShowCountdownTrigger);
            }
        }

        #endregion // Methods

    }
}
