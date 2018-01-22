using System.Timers;
using UnityEngine;

namespace Practear.Partitions
{
    /// <summary>
    /// Play an audio clip for a given duration.
    /// </summary>
    public class PlayForDuration : CustomYieldInstruction
    {

        #region Constants

        /// <summary>
        /// The fade speed.
        /// </summary>
        private const float FadeSpeed = 2f;

        /// <summary>
        /// The volume at which to stop the sound.
        /// </summary>
        private const float FadeOutThreshold = 0.05f;

        #endregion // Constants

        #region Instance variables

        /// <summary>
        /// The audio source
        /// </summary>
        private AudioSource m_AudioSource;

        /// <summary>
        /// The original clip that was attached to the audiosource.
        /// </summary>
        private AudioClip m_OriginalClip;

        /// <summary>
        /// The duration of the clip.
        /// </summary>
        private float m_Duration;

        /// <summary>
        /// The original audiosource volume.
        /// </summary>
        private float m_OriginalVolume;

        /// <summary>
        /// Whether to fade out the audio at the end or not.
        /// </summary>
        private bool m_FadeOut;

        #endregion // Instance variables

        #region Properties

        /// <summary>
        /// <see cref="CustomYieldInstruction.keepWaiting"/>
        /// </summary>
        public override bool keepWaiting
        {
            get
            {
                if (!m_AudioSource.isPlaying)
                {
                    ResetAudioSource();
                    return false;
                }

                if (m_AudioSource.time >= m_Duration)
                {
                    if (!m_FadeOut)
                    {
                        m_AudioSource.Stop();
                        ResetAudioSource();
                        return false;
                    }

                    if (m_AudioSource.volume > FadeOutThreshold)
                    {
                        m_AudioSource.volume -= Time.deltaTime * FadeSpeed;
                    }
                    else
                    {
                        m_AudioSource.Stop();
                        ResetAudioSource();

                        return false;
                    }               
                }

                return true;
            }
        }

        #endregion // Properties

        #region Constructors

        /// <summary>
        /// Play an audio clip for a specific amount of time.
        /// </summary>
        /// <param name="audioSource">The audio source</param>
        /// <param name="clip">The audio clip</param>
        /// <param name="duration">The duration</param>
        /// <param name="fadeOut">Whether to fade out the clip at the end or not. Default is true.</param>
        public PlayForDuration(AudioSource audioSource, AudioClip clip, float duration, bool fadeOut = false)
        {
            m_Duration = duration;
            m_AudioSource = audioSource;
            m_OriginalClip = m_AudioSource.clip;
            m_OriginalVolume = m_AudioSource.volume;
            m_FadeOut = fadeOut;
            m_AudioSource.clip = clip;
            m_AudioSource.Play();
        }

        #endregion // Constructors

        #region Methods

        /// <summary>
        /// Reset the audio source to its original state.
        /// </summary>
        private void ResetAudioSource()
        {
            if (m_AudioSource)
            {
                m_AudioSource.clip = m_OriginalClip;
                m_AudioSource.volume = m_OriginalVolume;
            }
        }

        #endregion // Methods

    }
}

