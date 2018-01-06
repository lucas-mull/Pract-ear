using Practear.Instruments;
using Practear.Lights;
using Practear.Partitions;
using Practear.Utils.Extensions;
using Practear.Utils.PropertyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Practear.Simon
{
    /// <summary>
    /// Used to load and manage the Simon mini-game.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class Simon : MonoBehaviour
    {

        #region Internal enums

        /// <summary>
        /// Internal enum used to define the current state of the game - the actions to make.
        /// </summary>
        private enum EGameState
        {
            /// <summary>
            /// Nothing is happening.
            /// </summary>
            None,

            /// <summary>
            /// The sequence must be played to the user.
            /// </summary>
            MustPlaySequence,

            /// <summary>
            /// The sequence is currently being played.
            /// </summary>
            PlayingSequence,

            /// <summary>
            /// Start of the player's turn.
            /// </summary>
            PlayerTurn,

            /// <summary>
            /// The game is waiting on an input from the player.
            /// </summary>
            WaitingOnInput,

            /// <summary>
            /// The player lost.
            /// </summary>
            GameOver,

            /// <summary>
            /// The player won.
            /// </summary>
            Success,

            /// <summary>
            /// The game has ended (game over or success).
            /// </summary>
            GameEnded
        }

        #endregion // Internal enums

        #region Instance variables

        /// <summary>
        /// The current state of the game.
        /// </summary>
        [SerializeField]
        [ReadOnly]
        private EGameState m_CurrentState = EGameState.None;

        /// <summary>
        /// The four instruments of the scene.
        /// </summary>
        [Space]
        [Tooltip("The four instruments of the scene.")]
        [SerializeField]
        private Instrument[] m_Instruments;

        /// <summary>
        /// Whether or not to play a random partition.
        /// </summary>
        [Space]
        [Header("Partition")]
        [Tooltip("Whether or not to play a random partition.")]
        [SerializeField]
        private bool m_PlayRandomPartition = true;

        /// <summary>
        /// The played partition.
        /// </summary>
        [Tooltip("The played partition.")]
        [SerializeField]
        [DisableIf("m_PlayRandomPartition")]
        private Partition m_Partition;

        /// <summary>
        /// The total number of allowed errors before the game ends.
        /// Default is 3.
        /// </summary>
        [Space]
        [Header("Settings")]
        [Tooltip("The total number of allowed errors before the game ends.")]
        [SerializeField]
        [Range(0, 5)]
        private int m_ErrorsAllowed = 3;

        /// <summary>
        /// The number of correct answers in a row required to win the game.
        /// Will never exceed the partition's length.
        /// </summary>
        [Tooltip("The number of correct answers in a row required to win the game. Will never exceed the partition's length.")]
        [SerializeField]
        private int m_SequenceCount = 10;

        /// <summary>
        /// The audio clip to play whenever the player is successful.
        /// </summary>
        [Tooltip("The audio clip to play whenever the player is successful.")]
        [SerializeField]
        private AudioClip m_SuccessClip;

        /// <summary>
        /// The audio clip to play whenever the player failed.
        /// </summary>
        [Tooltip("The audio clip to play whenever the player failed.")]
        [SerializeField]
        private AudioClip m_FailureClip;

        /// <summary>
        /// The order in which the instruments have to be played.
        /// </summary>
        private IList<int> m_InstrumentSequence;

        /// <summary>
        /// The current success count for the player.
        /// </summary>
        private int m_CurrentCount;

        /// <summary>
        /// The current total of notes the user has to play in a row before adding a note.
        /// </summary>
        private int m_CurrentLimit = 1;        

        /// <summary>
        /// The audio source to use for Sound effects.
        /// </summary>
        private AudioSource m_AudioSource;        

        #endregion // Instance variables

        #region Methods

        /// <summary>
        /// Used to validate user input in the inspector.
        /// </summary>
        private void OnValidate()
        {
            if (m_Instruments == null)
                m_Instruments = new Instrument[4];

            // Don't allow resizing of the array.
            if (m_Instruments.Length != 4)
                Array.Resize(ref m_Instruments, 4);

            // Keep the sequence count positive (strictly)
            if (m_SequenceCount < 1)
                m_SequenceCount = 1;
        }

        /// <summary>
        /// Called when this script is first loaded.
        /// </summary>
        private void Awake()
        {
            if (m_PlayRandomPartition)
            {
                m_Partition = Configuration.Instance.GetRandomPartition();
            }
            else
            {
                if (!m_Partition)
                    Debug.LogWarning("PlayRandomPartition is unchecked but no partition has been supplied. Picking random partition.");

                m_Partition = Configuration.Instance.GetRandomPartition();
            }

            m_InstrumentSequence = GenerateSequence();
            m_AudioSource = GetComponent<AudioSource>();

            // Listen to instrument clicks.
            foreach(Instrument instrument in m_Instruments)
            {
                instrument.Clicked.AddListener(OnInstrumentClicked);
            }
        }

        /// <summary>
        /// Called on start.
        /// </summary>
        private void Start()
        {
            m_CurrentState = EGameState.MustPlaySequence;
        }

        /// <summary>
        /// Called each frame.
        /// </summary>
        private void Update()
        {
            switch(m_CurrentState)
            {                
                case EGameState.MustPlaySequence:
                    m_CurrentState = EGameState.PlayingSequence;
                    StartCoroutine(PlaySequence());
                    break;
                case EGameState.PlayerTurn:
                    m_Partition.Rewind();
                    m_CurrentState = EGameState.WaitingOnInput;
                    EnableAllInteractions();
                    break;
                case EGameState.GameOver:
                    // TODO Show game over screen.
                    // Pass on every spotlight to negative state
                    SpotlightManager.Instance.SwitchAllLightsColor(SpotlightManager.Instance.NegativeColor);
                    m_CurrentState = EGameState.GameEnded;
                    break;
                case EGameState.Success:
                    // TODO show success screen.
                    m_CurrentState = EGameState.GameEnded;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Enable interactions for all instruments.
        /// </summary>
        private void EnableAllInteractions()
        {
            foreach (Instrument instrument in m_Instruments)
                instrument.EnableInteractions();
        }

        /// <summary>
        /// Disable interactions for all instruments.
        /// </summary>
        private void DisableAllInteractions()
        {
            foreach (Instrument instrument in m_Instruments)
                instrument.DisableInteractions();
        }

        /// <summary>
        /// Generate the random sequence for the game.
        /// </summary>
        /// <returns>The random sequence of instruments.</returns>
        private IList<int> GenerateSequence()
        {
            IList<int> sequence = new List<int>();
            m_SequenceCount = Mathf.Min(m_Partition.NotesCount, m_SequenceCount);

            int index = 0;
            int lastNumber = -1;
            int currentNumber = -1;
            while (index < m_SequenceCount)
            {
                // Make sure you don't get the same number twice in a row.
                while (currentNumber == lastNumber)
                    currentNumber = Random.Range(0, 4);

                lastNumber = currentNumber;
                sequence.Add(currentNumber);
                index++;
            }

            return sequence;
        }

        /// <summary>
        /// Play the sequence to reproduce to the player.
        /// </summary>
        private IEnumerator PlaySequence()
        {
            m_Partition.Rewind();

            for (int i = 0; i < m_CurrentLimit; i++)
            {
                yield return m_Instruments[m_InstrumentSequence[i]].PlayNote(m_Partition.Next());
            }

            m_CurrentState = EGameState.PlayerTurn;
        }        

        /// <summary>
        /// Called when the user clicked on an instrument
        /// </summary>
        /// <param name="target">The instrument that was clicked.</param>
        private void OnInstrumentClicked(Instrument target)
        {
            if (m_Instruments.IndexOf(target) == m_InstrumentSequence[m_CurrentCount])
            {
                StartCoroutine(CorrectChoice(target));
            }
            else
            {
                StartCoroutine(WrongChoice(target));
            }
        }

        /// <summary>
        /// Correct choice coroutine.
        /// </summary>
        private IEnumerator CorrectChoice(Instrument target)
        {
            // Toggle positive light
            target.ToggleLightPositive();
            m_CurrentCount++;

            // Play the note
            target.Play(m_Partition.Next());

            yield return new WaitForSeconds(0.1f);

            // Reset light color to neutral.
            target.ToggleLightNeutral();
            
            if (m_CurrentCount == m_CurrentLimit)
            {
                m_CurrentCount = 0;

                // Disable interactions
                DisableAllInteractions();

                // Play success sound.
                if (m_SuccessClip)
                {
                    yield return new PlayForDuration(m_AudioSource, m_SuccessClip, 3f);
                }

                // Check if the limit has been reached.
                if (m_CurrentLimit == m_SequenceCount)
                {
                    m_CurrentState = EGameState.Success;
                }
                else
                {
                    // Increment the limit and change the game state.
                    m_CurrentLimit++;
                    m_CurrentState = EGameState.MustPlaySequence;                    
                }                
            }

            yield return null;
        }

        /// <summary>
        /// Wrong choice coroutine.
        /// </summary>
        private IEnumerator WrongChoice(Instrument target)
        {
            m_CurrentCount = 0;

            // Disable interactions as soon as a mistake is made.
            DisableAllInteractions();

            // Toggle negative light
            target.ToggleLightNegative();

            // Play the note
            target.Play(m_Partition.Next());

            yield return new WaitForSeconds(0.1f);

            // Reset light color to neutral.
            target.ToggleLightNeutral();

            if (m_FailureClip)
                yield return new PlayForDuration(m_AudioSource, m_FailureClip, 3f);

            // Game over.
            if (m_ErrorsAllowed == 0)
            {
                m_CurrentState = EGameState.GameOver;
            }
            else
            {
                // TODO Errors prefabs.
                m_ErrorsAllowed--;

                m_CurrentState = EGameState.MustPlaySequence;
            }

            yield return null;
        }

        /// <summary>
        /// Called when the scene is destroyed
        /// </summary>
        private void OnDestroy()
        {
            // Remove all the listeners.
            foreach (Instrument instrument in m_Instruments)
                instrument.Clicked.RemoveAllListeners();
        }

        #endregion // Methods

    }
}
