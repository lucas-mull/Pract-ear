using Practear.Utils.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using Practear.Utils;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace Practear.UI
{

    /// <summary>
    /// Script used to define an Audio Playback slider:
    /// - Play or pause an Audio Clip
    /// - Move forward or backward through the clip using the slider.
    /// (- Move forward or backward through the clip using quick forward / back buttons.)
    /// </summary>
    [RequireComponent(typeof(Slider), typeof(AudioSource))]
    public class AudioPlaybackSlider : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IPointerClickHandler, IPointerDownHandler
    {

        #region Events

        /// <summary>
        /// Invoked when the audio is paused.
        /// </summary>
        public event Action AudioPaused;

        /// <summary>
        /// Invoked when the audio playback reaches the end.
        /// </summary>
        public event Action AudioEnded;

        /// <summary>
        /// Invoked when the audio starts playing.
        /// </summary>
        public event Action AudioStarted;

        /// <summary>
        /// Invoked when the audio resumes from a pause.
        /// </summary>
        public event Action AudioResumed;

        #endregion // Events

        #region Instance variables

        /// <summary>
        /// The "play" icon.
        /// </summary>
        [Tooltip("The 'play' icon.")]
        [SerializeField]
        private Sprite m_PlayIcon;

        /// <summary>
        /// The "pause" icon.
        /// </summary>
        [Tooltip("The 'pause' icon.")]
        [SerializeField]
        private Sprite m_PauseIcon;

        /// <summary>
        /// The button used to pause or resume the audio.
        /// </summary>
        [Tooltip("The button used to pause or resume the audio.")]
        [SerializeField]
        private Button m_PlayButton;

        /// <summary>
        /// The loop toggle.
        /// </summary>
        [Tooltip("The loop toggle.")]
        [SerializeField]
        private Button m_LoopToggle;

        /// <summary>
        /// The rect transform for the background area (what is being filled)
        /// </summary>
        [Tooltip("The rect transform for the background area (what is being filled)")]
        [SerializeField]
        private RectTransform m_BackgroundRect;

        /// <summary>
        /// The initial audio clip
        /// </summary>
        [Tooltip("The initial audio clip.")]
        [SerializeField]
        private AudioClip m_InitialClip;

        /// <summary>
        /// Whether or not to start the playback on awake.
        /// </summary>
        [Tooltip("Whether or not to start the playback on awake.")]
        [SerializeField]
        private bool m_PlayOnAwake;

        /// <summary>
        /// If this is checked, audio will be looped after it's done playing.
        /// </summary>
        [Tooltip("If this is checked, audio will be looped after it's done playing.")]
        [SerializeField]
        private bool m_Loop;

        /// <summary>
        /// The color to use for the toggle when it's active.
        /// </summary>
        [Tooltip("The color to use for the toggle when it's active.")]
        [SerializeField]
        private Color m_ActiveLoopToggleColor;

        /// <summary>
        /// The color to use for the toggle when it's inactive.
        /// </summary>
        [Tooltip("The color to use for the toggle when it's inactive.")]
        [SerializeField]
        private Color m_InactiveLoopToggleColor;

        /// <summary>
        /// The actual slider.
        /// </summary>
        private Slider m_Slider;

        /// <summary>
        /// The AudioSource used to play the clip.
        /// </summary>
        private AudioSource m_AudioSource;

        /// <summary>
        /// Is the playback paused?
        /// </summary>
        private bool m_IsPaused;

        /// <summary>
        /// True whenevr the slider is being dragged by hand.
        /// </summary>
        private bool m_IsDragging;

        /// <summary>
        /// Is the pointer currently down on the slider?
        /// </summary>
        private bool m_IsPointerDown;

        #endregion // Instance variables

        #region Properties

        /// <summary>
        /// Access the current clip.
        /// </summary>
        public AudioClip CurrentClip { get { return m_AudioSource.clip; } }

        /// <summary>
        /// Is the audio playing?
        /// </summary>
        public bool IsPlaying { get { return m_AudioSource.isPlaying; } }

        /// <summary>
        /// Is the playback paused?
        /// </summary>
        public bool IsPaused { get { return m_IsPaused; } }

        /// <summary>
        /// Is the playback completely stopped? (not in paused).
        /// </summary>
        private bool IsStopped { get { return !IsPaused && !IsPlaying; } }

        /// <summary>
        /// Access the slider
        /// </summary>
        private Slider Slider { get { return m_Slider ?? (m_Slider = GetComponent<Slider>()); } }

        /// <summary>
        /// Get the handle image.
        /// </summary>
        public Image HandleImage
        {
            get { return Slider.handleRect.GetComponent<Image>(); }
        }

        /// <summary>
        /// Get the fill area image.
        /// </summary>
        public Image FillAreaImage
        {
            get { return Slider.fillRect.GetComponent<Image>(); }
        }

        /// <summary>
        /// Get the background area image
        /// </summary>
        public Image BackgroundAreaImage
        {
            get
            {
                if (m_BackgroundRect == null)
                    return null;

                return m_BackgroundRect.GetComponent<Image>();
            }
        }

        /// <summary>
        /// Get the 'play' button's image.
        /// </summary>
        public Image PlayButtonImage
        {
            get
            {
                if (m_PlayButton == null)
                    return null;

                return m_PlayButton.GetComponent<Image>();
            }
        }

        /// <summary>
        /// Get the 'loop' button's image.
        /// </summary>
        public Image LoopToggleImage
        {
            get
            {
                if (!m_LoopToggle)
                    return null;

                return m_LoopToggle.GetComponent<Image>();
            }
        }

        /// <summary>
        /// Is the loop toggle active ?
        /// </summary>
        public bool IsLooped
        {
            get { return m_Loop; }
            set { m_Loop = value; }
        }

        #endregion // Properties

        #region Methods

        /// <summary>
        /// Called when this behaviour is first loaded.
        /// </summary>
        private void Awake()
        {
            m_Slider = GetComponent<Slider>();
            m_AudioSource = this.GetOrAddComponent<AudioSource>();
            
            if (m_InitialClip)
                m_AudioSource.clip = m_InitialClip;

            if (PlayButtonImage)
                PlayButtonImage.sprite = m_PlayIcon;

            m_PlayButton.onClick.AddListener(PauseOrResume);
            m_Slider.onValueChanged.AddListener(OnSliderValueChanged);

            if (LoopToggleImage)
            {
                m_LoopToggle.onClick.AddListener(OnLoopToggle);
                LoopToggleImage.color = IsLooped ? m_ActiveLoopToggleColor : m_InactiveLoopToggleColor;
            }

            if (m_PlayOnAwake)
                Play();
        }

        /// <summary>
        /// Called every frame for this behaviour (as long as it's enabled).
        /// </summary>
        private void Update()
        {
            // Don't perform any action while the cursor is being dragged.
            if (m_IsDragging || m_IsPointerDown)
                return;
            
            // Update the cursor every frame while the audio is playing.
            if (IsPlaying)
            {
                SetSliderPosition(GetPlaybackPosition());
            }
        }

        /// <summary>
        /// Called when the user begins a drag on the slider.
        /// </summary>
        public void OnBeginDrag(PointerEventData eventData)
        {
            m_IsDragging = true;
        }

        /// <summary>
        /// Called when the user ends a drag on the slider
        /// </summary>
        public void OnEndDrag(PointerEventData eventData)
        {
            m_IsDragging = false;
        }

        /// <summary>
        /// Called when the user performs a click on the slider
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            // Slider is updated by itself, simply update the audio source's time.
            SetPlaybackPosition(GetSliderPosition());
            m_IsPointerDown = false;
        }

        /// <summary>
        /// Called when the pointer is down on the slider.
        /// </summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            m_IsPointerDown = true;
        }

        /// <summary>
        /// Pause the playback
        /// </summary>
        public void Pause()
        {
            if (IsPlaying)
            {
                m_AudioSource.Pause();
                m_IsPaused = true;
                if (PlayButtonImage)
                {
                    PlayButtonImage.sprite = m_PlayIcon;
                }

                if (AudioPaused != null)
                    AudioPaused.Invoke();
            }
        }

        /// <summary>
        /// Resume the playback
        /// </summary>
        public void Resume()
        {
            if (IsPaused)
            {
                m_AudioSource.UnPause();
                m_IsPaused = false;
                if (PlayButtonImage)
                {
                    PlayButtonImage.sprite = m_PauseIcon;
                }

                if (AudioResumed != null)
                    AudioResumed.Invoke();
            }
            else if (!IsPlaying)
            {
                Play();
            }
        }

        /// <summary>
        /// Start the playback
        /// </summary>
        public void Play()
        {
            if (CurrentClip)
            {
                SetPlaybackPosition(GetSliderPosition());
                m_AudioSource.Play();
                if (PlayButtonImage)
                {
                    PlayButtonImage.sprite = m_PauseIcon;
                }

                if (AudioStarted != null)
                    AudioStarted.Invoke();
            }
        }

        /// <summary>
        /// Pause or resume the audio
        /// </summary>
        public void PauseOrResume()
        {
            if (!IsPlaying)
                Resume();
            else
                Pause();
        }

        /// <summary>
        /// Called whenever the slider's value changes (by script or by hand).
        /// </summary>
        /// <param name="value">The current value</param>
        private void OnSliderValueChanged(float value)
        {
            if (m_IsDragging)
                return;

            if (m_Slider.maxValue - value <= 0.005f)
            {
                // Reset slider to min.
                m_Slider.value = m_Slider.minValue;

                // If we're looping, simply start the playback from 0 again.
                if (IsLooped)
                {
                    SetPlaybackPosition(0);
                }
                else if (PlayButtonImage)
                {
                    // Reset button icon.
                    PlayButtonImage.sprite = m_PlayIcon;
                    if (AudioEnded != null)
                        AudioEnded.Invoke();
                }
            }
        }

        /// <summary>
        /// Action to perform when clicking on the loop toggle
        /// </summary>
        private void OnLoopToggle()
        {
            IsLooped = !IsLooped;
            LoopToggleImage.color = IsLooped ? m_ActiveLoopToggleColor : m_InactiveLoopToggleColor;
        }

        /// <summary>
        /// Set the slider's handle position.
        /// </summary>
        /// <param name="value">The value between 0 and 1.</param>
        private void SetSliderPosition(float value)
        {
            value = Mathf.Clamp01(value);
            float length = m_Slider.maxValue - m_Slider.minValue;
            m_Slider.value = value * length;
        }

        /// <summary>
        /// Get the slider position between 0 and 1.
        /// </summary>
        /// <returns>The position between 0 (start) and 1 (end).</returns>
        private float GetSliderPosition()
        {
            float length = m_Slider.maxValue - m_Slider.minValue;
            return m_Slider.value / length;
        }

        /// <summary>
        /// Get the current playback progression.
        /// </summary>
        /// <returns>A float between 0 and 1 (0 = no progression, 1 = fully done).</returns>
        private float GetPlaybackPosition()
        {
            return m_AudioSource.time / CurrentClip.length;
        }

        /// <summary>
        /// Set the playback progression using a value between 0 and 1.
        /// </summary>
        /// <param name="progression">The progression between 0 and 1 (0 = no progression, 1 = fully done).</param>
        private void SetPlaybackPosition(float progression)
        {
            progression = Mathf.Clamp01(progression);
            float progressionInSeconds = progression * CurrentClip.length;
            m_AudioSource.time = progressionInSeconds;
        }

        #endregion // Methods
        
    }

    #region Custom Editor

#if UNITY_EDITOR

    /// <summary>
    /// Custom editor for <see cref="AudioPlaybackSlider"/>
    /// </summary>
    [CustomEditor(typeof(AudioPlaybackSlider))]
    public class AudioPlaybackSliderEditor : Editor
    {

        #region Instance variables

        /// <summary>
        /// Is the customisation header foldout?
        /// </summary>
        private bool m_IsCustomisationFoldout;

        /// <summary>
        /// Is the handle icon section foldout?
        /// </summary>
        private bool m_IsHandleIconFoldout;

        /// <summary>
        /// Is the fill area section foldout?
        /// </summary>
        private bool m_IsFillAreaFoldout;

        /// <summary>
        /// Is the fill background section foldout?
        /// </summary>
        private bool m_IsFillBackgroundFoldout;

        /// <summary>
        /// Is the play button section foldout?
        /// </summary>
        private bool m_IsPlayButtonFoldout;

        /// <summary>
        /// Is the loop button section foldout?
        /// </summary>
        private bool m_IsLoopButtonFoldout;

        #endregion // Instance variables

        #region Properties

        /// <summary>
        /// Access the script instance.
        /// </summary>
        private AudioPlaybackSlider Target { get { return (AudioPlaybackSlider)target; } }

        #endregion // Properties

        #region Methods

        /// <summary>
        /// Called when inspector is loaded for this script.
        /// </summary>
        private void OnEnable()
        {
            LoadPrefs();
        }

        /// <summary>
        /// <see cref="Editor.OnInspectorGUI"/>
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorUtils.DrawDefaultScriptField<AudioPlaybackSlider>(target);
            EditorUtils.DrawPropertyFields(serializedObject, "m_PlayButton", "m_LoopToggle", 
                "m_BackgroundRect", "m_InitialClip", "m_PlayOnAwake", "m_Loop");

            DrawCustomisationSection();

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draw the customisation section
        /// </summary>
        private void DrawCustomisationSection()
        {
            m_IsCustomisationFoldout = EditorGUILayout.Foldout(m_IsCustomisationFoldout, "Customization", true);
            if (m_IsCustomisationFoldout)
            {
                EditorGUI.indentLevel++;
                DrawHandleIconSection();
                DrawFillAreaSection();
                DrawBackgroundAreaSection();
                DrawPlayButtonSection();
                DrawLoopToggleSection();
                EditorGUI.indentLevel--;

                SavePrefs();
            }
        }

        /// <summary>
        /// Draw the handle icon section
        /// </summary>
        private void DrawHandleIconSection()
        {
            m_IsHandleIconFoldout = DrawBasicImageSection(m_IsHandleIconFoldout, "Handle", "handle icon", Target.HandleImage);
        }

        /// <summary>
        /// Draw the fill area section.
        /// </summary>
        private void DrawFillAreaSection()
        {
            m_IsFillAreaFoldout = DrawBasicImageSection(m_IsFillAreaFoldout, "Fill Area", "Fill texture", Target.FillAreaImage);
        }

        /// <summary>
        /// Draw the background area section
        /// </summary>
        private void DrawBackgroundAreaSection()
        {
            m_IsFillBackgroundFoldout = DrawBasicImageSection(m_IsFillBackgroundFoldout, "Background area", "Background texture", Target.BackgroundAreaImage);
        }

        /// <summary>
        /// Draw the "play" button section
        /// </summary>
        private void DrawPlayButtonSection()
        {
            if (!Target.PlayButtonImage)
                return;

            m_IsPlayButtonFoldout = EditorGUILayout.Foldout(m_IsPlayButtonFoldout, "Play button", true);
            if (m_IsPlayButtonFoldout)
            {
                EditorUtils.DrawPropertyFields(serializedObject, "m_PlayIcon", "m_PauseIcon");
                Target.PlayButtonImage.color = EditorGUILayout.ColorField("Color", Target.PlayButtonImage.color);
                EditorUtility.SetDirty(Target.PlayButtonImage);
            }
        }

        /// <summary>
        /// Draw the "loop" toggle section
        /// </summary>
        private void DrawLoopToggleSection()
        {
            if (!Target.LoopToggleImage)
                return;

            m_IsLoopButtonFoldout = EditorGUILayout.Foldout(m_IsLoopButtonFoldout, "Loop toggle", true);
            if (m_IsLoopButtonFoldout)
            {
                SerializedProperty loopActiveColor = serializedObject.FindProperty("m_ActiveLoopToggleColor");
                SerializedProperty loopInactiveColor = serializedObject.FindProperty("m_InactiveLoopToggleColor");
                EditorGUILayout.PropertyField(loopActiveColor, true);
                EditorGUILayout.PropertyField(loopInactiveColor, true);

                Color currentColor = Target.IsLooped ? loopActiveColor.colorValue : loopInactiveColor.colorValue;
                Target.LoopToggleImage.color = currentColor;

                Target.LoopToggleImage.sprite = EditorUtils.DrawNormalSpriteField("Loop icon", Target.LoopToggleImage.sprite);
                EditorUtility.SetDirty(Target.LoopToggleImage);
            }
        }

        /// <summary>
        /// Draw a foldout section allowing to change the sprite and the color of an image.
        /// </summary>
        /// <param name="isFoldout">Is the section foldout ?</param>
        /// <param name="header">The section header</param>
        /// <param name="iconLabel">The label for the sprite header.</param>
        /// <param name="targetImage">The target image.</param>
        /// <returns>The foldout state.</returns>
        private bool DrawBasicImageSection(bool isFoldout, string header, string iconLabel, Image targetImage)
        {
            // Don't draw anything if the given image is null.
            if (!targetImage)
                return false;

            isFoldout = EditorGUILayout.Foldout(isFoldout, header, true);
            if (isFoldout)
            {
                EditorGUI.indentLevel++;

                targetImage.sprite = EditorUtils.DrawNormalSpriteField(iconLabel, targetImage.sprite);
                targetImage.color = EditorGUILayout.ColorField("Color", targetImage.color);
                EditorUtility.SetDirty(targetImage);

                EditorGUI.indentLevel--;
            }

            return isFoldout;
        }

        /// <summary>
        /// Save foldout states in editor prefs.
        /// </summary>
        private void SavePrefs()
        {
            EditorPrefs.SetBool("customization", m_IsCustomisationFoldout);
            EditorPrefs.SetBool("handle_icon", m_IsHandleIconFoldout);
            EditorPrefs.SetBool("fill_area", m_IsFillAreaFoldout);
            EditorPrefs.SetBool("background_area", m_IsFillBackgroundFoldout);
            EditorPrefs.SetBool("play_button", m_IsPlayButtonFoldout);
            EditorPrefs.SetBool("loop_button", m_IsLoopButtonFoldout);
        }

        /// <summary>
        /// Load foldout states in editor prefs.
        /// </summary>
        private void LoadPrefs()
        {
            m_IsCustomisationFoldout = EditorPrefs.GetBool("customization", false);
            m_IsHandleIconFoldout = EditorPrefs.GetBool("handle_icon", false);
            m_IsFillAreaFoldout = EditorPrefs.GetBool("fill_area", false);
            m_IsFillBackgroundFoldout = EditorPrefs.GetBool("background_area", false);
            m_IsPlayButtonFoldout = EditorPrefs.GetBool("play_button", false);
            m_IsLoopButtonFoldout = EditorPrefs.GetBool("loop_button", false);
        }

        #endregion // Methods

    }

#endif

#endregion // Custom Editor

}
