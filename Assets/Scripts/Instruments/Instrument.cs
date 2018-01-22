using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using Practear.Utils.Extensions;
using UnityEngine.Events;
using Practear.Lights;
using Practear.Utils;
using Practear.Partitions;
using System.Collections;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Practear.Instruments
{

    /// <summary>
    /// Describes an instrument's instance at runtime.
    /// Can be used to render the instrument, play the instrument's sounds and animations.
    /// </summary>
    [RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
    public class Instrument : MonoBehaviour, IPointerClickHandler
    {

        #region Internal classes

        /// <summary>
        /// Unity event taking an instrument as its parameter.
        /// </summary>
        [Serializable]
        public class InstrumentEvent : UnityEvent<Instrument> {}

        #endregion // Internal classes

        #region Events

        /// <summary>
        /// Event fired whenever this instrument was clicked on.
        /// </summary>
        public InstrumentEvent Clicked = new InstrumentEvent();

        #endregion // Events

        #region Instance variables

        /// <summary>
        /// The text element used to display the instrument's name at runtime.
        /// </summary>
        [Tooltip("The text element used to display the instrument's name at runtime.")]
        [SerializeField]
        private TMP_Text m_TextComponent;

        /// <summary>
        /// The spotlight associated with this instrument.
        /// </summary>
        [Tooltip("The spotlight associated with this instrument.")]
        [SerializeField]
        private Light m_Spotlight;

        /// <summary>
        /// The particle system attached to this game object.
        /// </summary>
        [Tooltip("The particle system attached to this game object.")]
        [SerializeField]
        private ParticleSystem m_ParticleSystem;

        /// <summary>
        /// The name of the initial instrument
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private string m_InitialInstrumentName;       

        /// <summary>
        /// The sprite renderer used to render this instrument.
        /// </summary>
        private SpriteRenderer m_SpriteRenderer;

        /// <summary>
        /// The audio source used to play the instrument's sounds.
        /// </summary>
        private AudioSource m_AudioSource;

        /// <summary>
        /// The animator attached to this gameObject (if any).
        /// </summary>
        private Animator m_Animator;

        /// <summary>
        /// The collider attached to this gameObject.
        /// </summary>
        private Collider2D m_Collider;        

        /// <summary>
        /// The data regarding the current instrument.
        /// </summary>
        private InstrumentData m_Current;

        #endregion // Instance variables

        #region Properties

        /// <summary>
        /// Does the current object possess an animator ?
        /// </summary>
        public bool HasAnimator { get { return m_AudioSource != null; } }

        /// <summary>
        /// Set the current instrument data.
        /// </summary>
        public InstrumentData Current
        {
            get { return m_Current; }
            set
            {
                m_Current = value;
                Initialize();
            }
        }

        /// <summary>
        /// Access the renderer of this instrument.
        /// </summary>
        private SpriteRenderer Renderer
        {
            get
            {
                if (m_SpriteRenderer == null)
                    m_SpriteRenderer = GetComponent<SpriteRenderer>();

                return m_SpriteRenderer;
            }
        }

        /// <summary>
        /// Is the current instrument emitting particles ?
        /// </summary>
        private bool IsEmitting
        {
            get { return m_ParticleSystem.emission.enabled; }
        }

        #endregion // Properties

        #region Methods

        /// <summary>
        /// Called on start for this behaviour
        /// </summary>
        private void Start()
        {
            // It is mandatory to have an AudioSource. If the object doesn't have one, we add one by default.
            m_AudioSource = GetComponent<AudioSource>();
            if (m_AudioSource == null)
                m_AudioSource = gameObject.AddComponent<AudioSource>();

            // Animator is optional.
            m_Animator = GetComponent<Animator>();

            // Don't emit particles on start.
            StopEmission();

            SetInstrument(m_InitialInstrumentName);
            m_Collider = GetComponent<Collider2D>();
        }

        /// <summary>
        /// Initialize the text component and the sprite for this instrument.
        /// </summary>
        private void Initialize()
        {
            // Don't do anything if no instrument is specified.
            if (Current == null)
                return;

            // Set the text to the current instrument's name.
            m_TextComponent.text = Current.Name;
            Renderer.sprite = Current.Sprite;
        }

        /// <summary>
        /// Called whenever this gameObject's collider has been clicked.
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            // Invoke the event.
            Clicked.Invoke(this);
        }

        /// <summary>
        /// Set the current instrument using its name.
        /// Comparison is not case sensitive.
        /// </summary>
        /// <param name="instrumentName">The name of the instrument.</param>
        public void SetInstrument(string instrumentName)
        {
            InstrumentData foundData = Configuration.Instance.FindInstrumentData(instrumentName);
            if (foundData != null)
                Current = foundData;
        }

        /// <summary>
        /// Play an entire partition.
        /// </summary>
        /// <param name="partition">The partition to play.</param>
        public void Play(Partition partition)
        {
            StartCoroutine(PlayPartition(partition));
        }

        /// <summary>
        /// Coroutine for playing throughout an entire partition.
        /// </summary>
        /// <param name="partition">The partition to play</param>
        public IEnumerator PlayPartition(Partition partition)
        {
            if (m_Current == null)
                yield break;

            MusicalNote current = partition.Next();
            while (current != null)
            {
                bool fadeOut = partition.IsLastNote();

                yield return PlayNote(current, fadeOut);
                current = partition.Next();
            }
        }

        /// <summary>
        /// Play a given musical note for this instrument.
        /// </summary>
        /// <param name="note">The note to play.</param>
        public void Play(MusicalNote note)
        {            
            StartCoroutine(PlayNote(note));
        }

        /// <summary>
        /// Stop every sound that the instrument might be playing (partition or individual notes).
        /// </summary>
        public void StopAllSounds()
        {
            StopAllCoroutines();
            m_AudioSource.Stop();
        }

        /// <summary>
        /// <see cref="Play(MusicalNote)"/>
        /// </summary>
        public IEnumerator PlayNote(MusicalNote note, bool fadeOut = false)
        {
            if (m_Current == null)
                yield break;
            
            if (!IsEmitting)
                StartEmission(1f);

            AudioClip clip = null;
            if (!Configuration.Instance.UseInstrumentDefaultScale)
            {
                 clip = m_Current.Notes.GetAudioClip(note);
            }

            if (!clip)
            {
                MusicalNote defaultNote = new MusicalNote(note);
                defaultNote.Octave = m_Current.Notes.DefaultScale;
                clip = m_Current.Notes.GetAudioClip(defaultNote);
            }

            float length = Configuration.Instance.GetNoteLengthInSeconds(note.Length);
            if (clip == null)
            {
                Debug.LogWarning(string.Format("The note {0} (octave {1} is missing for instrument {2}! " +
                    "The default scale is also invalid.", note.Name, note.Octave, m_Current.Name));

                yield return new WaitForSeconds(length);
                yield break;
            }            

            yield return new PlayForDuration(m_AudioSource, clip, length, fadeOut);
        }

        /// <summary>
        /// Emit quavers from the particle system.
        /// </summary>
        /// <param name="duration">The duration of the emission. Pass a negative value for continuous emission.</param>
        public void StartEmission(float duration = -1f)
        {
            EmitParticles(true);
            if (duration > 0)
            {
                StartCoroutine(StopEmissionDelay(duration));
            }
        }

        /// <summary>
        /// Emit or stop emitting particles.
        /// </summary>
        /// <param name="emit">true to start emission, false to stop emission.</param>
        private void EmitParticles(bool emit)
        {
            if (m_ParticleSystem)
            {
                ParticleSystem.EmissionModule emissionModule = m_ParticleSystem.emission;
                emissionModule.enabled = emit;
            }
        }

        /// <summary>
        /// Stop the emission after waiting a given delay.
        /// </summary>
        /// <param name="delay">The delay.</param>
        private IEnumerator StopEmissionDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            StopEmission();
        }

        /// <summary>
        /// Stop the emission of particules.
        /// </summary>
        public void StopEmission()
        {
            EmitParticles(false);
        }

        /// <summary>
        /// Toggle the spotlight associated with this gameobject
        /// </summary>
        public void ToggleSpotlight()
        {
            if (m_Spotlight)
            {
                SpotlightManager.Instance.ToggleSpotlight(m_Spotlight);
            }
        }

        /// <summary>
        /// Toggle spotlight color to "positive"
        /// </summary>
        public void ToggleLightPositive()
        {
            SpotlightManager.Instance.SwitchLightColor(m_Spotlight, SpotlightManager.Instance.PositiveColor);
        }

        /// <summary>
        /// Toggle spotlight color to "negative"
        /// </summary>
        public void ToggleLightNegative()
        {
            SpotlightManager.Instance.SwitchLightColor(m_Spotlight, SpotlightManager.Instance.NegativeColor);
        }

        /// <summary>
        /// Toggle spotlight color to "neutral"
        /// </summary>
        public void ToggleLightNeutral()
        {
            SpotlightManager.Instance.SwitchLightColor(m_Spotlight, SpotlightManager.Instance.NeutralColor);
        }

        /// <summary>
        /// Disable the interactions for this instrument
        /// </summary>
        public void DisableInteractions()
        {
            m_Collider.enabled = false;
        }

        /// <summary>
        /// Enable interactions for this instrument
        /// </summary>
        public void EnableInteractions()
        {
            m_Collider.enabled = true;
        }

        /// <summary>
        /// Called when the scene is destroyed.
        /// </summary>
        private void OnDestroy()
        {
            Clicked.RemoveAllListeners();
        }

        #endregion // Methods
    }

    #region Custom editor

#if UNITY_EDITOR

    /// <summary>
    /// Custom inspector for <see cref="Instrument"/>
    /// </summary>
    [CustomEditor(typeof(Instrument))]
    public class InstrumentEditor : Editor
    {

        #region Constants

        /// <summary>
        /// Constant string for the "none" option.
        /// </summary>
        private const string NoneOption = "None";

        #endregion // Constants

        #region Instance variables

        /// <summary>
        /// The reference to <see cref="Configuration"/> that we use to access the database.
        /// </summary>
        private Configuration m_DatabaseManager;
        
        /// <summary>
        /// The current instrument data (instrument names).
        /// </summary>
        private string[] m_CurrentData;

        /// <summary>
        /// SerializedProperty for <see cref="Instrument.m_InitialInstrumentName"/>
        /// </summary>
        private SerializedProperty m_InitialNameProp;

        /// <summary>
        /// SerializedProperty for <see cref="Instrument.m_TextComponent"/>
        /// </summary>
        private SerializedProperty m_TextMeshProp;

        #endregion // Instance variables

        #region Properties       

        /// <summary>
        /// Access the <see cref="Configuration"/> in the scene.
        /// </summary>
        private Configuration DatabaseManager
        {
            get
            {
                return m_DatabaseManager ?? 
                    (m_DatabaseManager = FindObjectOfType<Configuration>());
            }
        }

        /// <summary>
        /// Access the target <see cref="Instrument"/> script of this editor script.
        /// </summary>
        private Instrument Target { get { return (Instrument)target; } }

        /// <summary>
        /// Access the current data.
        /// </summary>
        private IEnumerable<string> CurrentData
        {
            get
            {
                if (DatabaseManager != null)
                {
                    if (m_CurrentData == null)
                    {
                        // Add the "None" option.
                        IList<string> data = DatabaseManager.InstrumentDatabase.Instruments.Select(i => i.Name).ToList();
                        data.Insert(0, NoneOption);

                        m_CurrentData = data.ToArray();
                    }

                    return m_CurrentData;
                }

                return null;
            }
        }

        #endregion // Properties

        #region Methods

        /// <summary>
        /// Called when the editor script is called for the first time.
        /// </summary>
        private void OnEnable()
        {
            m_InitialNameProp = serializedObject.FindProperty("m_InitialInstrumentName");
            m_TextMeshProp = serializedObject.FindProperty("m_TextComponent");
        }

        /// <summary>
        /// <see cref="Editor.OnInspectorGUI"/>
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();
            TryDrawInstrument();

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Try and draw the instrument data.
        /// </summary>
        private void TryDrawInstrument()
        {
            // Dont draw anything if the database manager is null.
            if (DatabaseManager == null)
            {
                EditorUtils.DrawErrorMessage("Couldn't find a InstrumentsDatabaseManager in the scene. " +
                    "Add one and specify the database to use.");
                return;
            }

            // If no database has been specified, display an error message and a button to assign it.
            if (DatabaseManager.InstrumentDatabase == null)
            {
                EditorUtils.DrawErrorMessage("Please specify a database to use in the configuration");
                if (GUILayout.Button("Go to Configuration"))
                {
                    EditorGUIUtility.PingObject(DatabaseManager);
                }

                return;
            }

            if (serializedObject.FindProperty("m_TextComponent").objectReferenceValue == null)
            {
                EditorUtils.DrawErrorMessage("Please make sure the text component is assigned above");
                return;
            }

            // Draw a dropdown for all the available instruments.
            if (CurrentData != null && CurrentData.Any())
            {
                string selectedName = m_InitialNameProp == null ? string.Empty : m_InitialNameProp.stringValue;
                int selectedIndex = CurrentData.IndexOf(selectedName);

                selectedIndex = EditorGUILayout.Popup("Initial instrument", selectedIndex, CurrentData.ToArray());

                // Reassign the current data according to the selected index.
                m_InitialNameProp.stringValue = CurrentData.ElementAtOrDefault(selectedIndex);
                InstrumentData newData = DatabaseManager.FindInstrumentData(m_InitialNameProp.stringValue);
                if (Target.Current != newData)
                {
                    Target.Current = newData;
                    EditorUtility.SetDirty(m_TextMeshProp.objectReferenceValue);
                }
            }            
        }

        #endregion // Methods

    }

#endif

    #endregion // Custom editor
}


