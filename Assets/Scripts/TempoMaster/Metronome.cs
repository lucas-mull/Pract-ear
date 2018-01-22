using Practear.Utils.Extensions;
using Practear.Utils.Math;
using Practear.Utils.PropertyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Practear.Tempo
{
    /// <summary>
    /// Script used to control the metronome sprite.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class Metronome : MonoBehaviour
    {

        #region Constants

        /// <summary>
        /// The minimum BPM value.
        /// </summary>
        private const int MinBpm = 40;

        /// <summary>
        /// The max BPM value.
        /// </summary>
        private const int MaxBpm = 208;

        #endregion // Constants

        #region Instance variables

        /// <summary>
        /// The current beats per minute for the metronome.
        /// Displayed in inspector for debug purposes.
        /// </summary>
        [SerializeField]
        [ReadOnly]
        private int m_CurrentBeatsPerMinute;

        /// <summary>
        /// Start the metronome on awake?
        /// </summary>
        [Tooltip("Start the metronome on awake?")]
        [SerializeField]
        private bool m_StartOnAwake;

        /// <summary>
        /// The slider used to control the metronome's rythm.
        /// </summary>
        [Tooltip("The slider used to control the metronome's rythm.")]
        [SerializeField]
        private Slider m_Slider;        

        /// <summary>
        /// The weight attached to the pendulum. Its placement on the pendulum is controlled by the slider.
        /// </summary>
        [Space]
        [Header("Weight")]
        [Tooltip("The weight attached to the pendulum. Its placement on the pendulum is controlled by the slider.")]
        [SerializeField]
        private Transform m_Weight;

        /// <summary>
        /// The highest position the weight can take on the Y axis.
        /// </summary>
        [Tooltip("The highest position the weight can take on the Y axis.")]
        [SerializeField]
        [EnableIf("m_Weight")]
        private float m_HighestWeightposition;

        /// <summary>
        /// The lowest position the weight can take on the Y axis.
        /// </summary>
        [Tooltip("The lowest position the weight can take on the Y axis.")]
        [SerializeField]
        [EnableIf("m_Weight")]
        private float m_LowestWeightPosition;  

        /// <summary>
        /// The base of the  pendulum. The part that rotates.
        /// </summary>
        [Space]
        [Header("Pendulum")]
        [Tooltip("The base of the  pendulum. The part that rotates.")]
        [SerializeField]
        private Transform m_Base;

        /// <summary>
        /// The max angle the metronome's pendulum can reach when balancing.
        /// </summary>
        [Tooltip("The max angle the metronome's pendulum can reach when balancing.")]
        [SerializeField]
        [Range(0, 90)]
        private int m_MaxAngle;

        /// <summary>
        /// The click sound done by the metronome.
        /// </summary>
        [Space]
        [Header("Rhythm")]
        [Tooltip("The click sound done by the metronome.")]
        [SerializeField]
        private AudioClip m_BeatClip;

        /// <summary>
        /// The max value (Beats per minute) for the metronome.
        /// </summary>        
        [Tooltip("The max value (Beats per minute) for the metronome.")]
        [SerializeField]
        [Range(40, 208)]
        private int m_MaxBpm = 208;

        /// <summary>
        /// The min value (Beats per minute) for the metronome.
        /// </summary>
        [Tooltip("The min value (Beats per minute) for the metronome.")]
        [SerializeField]
        [Range(40, 208)]
        private int m_MinBpm = 40;

        /// <summary>
        /// The text used to display the current rythm.
        /// </summary>
        [Tooltip("The text used to display the current rythm.")]
        [SerializeField]
        private TMP_Text m_Text;

        /// <summary>
        /// The button used to increase the rhythm.
        /// </summary>
        [Tooltip("The button used to increase the rhythm.")]
        [SerializeField]
        private Button m_IncreaseButton;

        /// <summary>
        /// The button used to decrease the rhythm.
        /// </summary>
        [Tooltip("The button used to decrease the rhythm.")]
        [SerializeField]
        private Button m_DecreaseButton;

        /// <summary>
        /// When the next beat is due.
        /// </summary>
        private float m_NextBeat;

        /// <summary>
        /// The next angle to reach for the pendulum.
        /// </summary>
        private int m_NextAngle;

        /// <summary>
        /// The timer.
        /// </summary>
        private float m_Timer;

        /// <summary>
        /// The audio source.
        /// </summary>
        private AudioSource m_AudioSource;

        /// <summary>
        /// Is the metronome clicking currently?
        /// </summary>
        private bool m_IsClicking;

        #endregion // Instance variables

        #region Properties

        /// <summary>
        /// Get or set the current pendulum rotation.
        /// </summary>
        private float CurrentBaseRotation
        {
            get
            {
                float zRotation = m_Base.localEulerAngles.z;
                return zRotation > 180 ? zRotation - 360 : zRotation;
            }
            set
            {
                Vector3 eulerAngles = m_Base.localEulerAngles;
                eulerAngles.z = value;
                m_Base.localEulerAngles = eulerAngles;
            }
        }

        /// <summary>
        /// Get or set the current weight position on the pendulum.
        /// </summary>
        private float CurrentWeightPosition
        {
            get
            {
                return m_Weight.localPosition.y;
            }
            set
            {
                Vector3 position = m_Weight.localPosition;
                position.y = value;
                m_Weight.localPosition = position;
            }
        }

        /// <summary>
        /// The current BPM value.
        /// </summary>
        public int CurrentBpmValue
        {
            get { return m_CurrentBeatsPerMinute; }
            set
            {
                m_CurrentBeatsPerMinute = value < m_MinBpm ? m_MinBpm : (value > m_MaxBpm ? m_MaxBpm : value); ;
                if (m_Text)
                {
                    m_Text.text = string.Format("{0} Bpm", m_CurrentBeatsPerMinute);
                }
            }
        }

        /// <summary>
        /// Get or set the max BPM limit.
        /// </summary>
        public int MaxBpmValue
        {
            get
            {
                return m_MaxBpm;
            }
            set
            {
                m_MaxBpm = value < MinBpm ? MinBpm : (value > MaxBpm ? MaxBpm : value);
                m_Slider.maxValue = m_MaxBpm;                
            }
        }

        /// <summary>
        /// Get or set the max BPM limit.
        /// </summary>
        public int MinBpmValue
        {
            get
            {
                return m_MinBpm;
            }
            set
            {
                m_MinBpm = value < MinBpm ? MinBpm : (value > MaxBpm ? MaxBpm : value);
                m_Slider.minValue = m_MinBpm;
            }
        }

        #endregion // Properties

        #region Methods

        /// <summary>
        /// Called when the game object is first active.
        /// </summary>
        private void Awake()
        {
            m_Slider.wholeNumbers = true;
            m_Slider.minValue = m_MinBpm;
            m_Slider.maxValue = m_MaxBpm;

            m_Slider.onValueChanged.AddListener(OnSliderValueChanged);            
            m_AudioSource = this.GetOrAddComponent<AudioSource>();            
        }

        /// <summary>
        /// Called on start.
        /// </summary>
        private void Start()
        {           
            // Initialize every value to the min.
            m_Slider.value = m_Slider.minValue;
            CurrentBaseRotation = -m_MaxAngle;
            CurrentBpmValue = m_MinBpm;
            CurrentWeightPosition = m_HighestWeightposition;

            // Initialize next angle and next beat.
            m_NextAngle = m_MaxAngle;
            m_NextBeat = 0f;

            if (m_StartOnAwake)
                StartClicking();
        }

        /// <summary>
        /// Called every frame.
        /// </summary>
        private void Update()
        {
            if (!m_IsClicking)
                return;

            // Compute when next beat is due.
            if (m_NextBeat == 0f)
            {
                m_NextBeat = GetBeatTimeInSeconds();
                m_Timer = 0f;

                // Assign the next angle.
                m_NextAngle = m_NextAngle == m_MaxAngle ? -m_MaxAngle : m_MaxAngle;
            }
            else if (m_NextBeat > 0f)
            {
                if (m_Timer >= m_NextBeat)
                {
                    // Play the beat
                    m_AudioSource.PlayOneShot(m_BeatClip);
                    m_NextBeat = 0f;
                }
                else
                {
                    // Advance the angle cycle
                    float angleStep = m_MaxAngle * 2 * Time.deltaTime / m_NextBeat;

                    // Rotate the base according to the new angle.
                    CurrentBaseRotation = m_NextAngle < CurrentBaseRotation 
                        ? CurrentBaseRotation - angleStep 
                        : CurrentBaseRotation + angleStep;

                    // Don't allow the angle to go over the target.
                    if (Mathf.Abs(CurrentBaseRotation) > m_MaxAngle)
                        CurrentBaseRotation = m_NextAngle;

                    m_Timer += Time.deltaTime;
                }
            }
        }

        /// <summary>
        /// Compute the time in seconds it must take for one beat to occur
        /// </summary>
        /// <returns>The time of a complete beat in seconds.</returns>
        private float GetBeatTimeInSeconds()
        {
            return 60f / CurrentBpmValue;
        }

        /// <summary>
        /// Called when the slider's value is changed by the user (drag or click).
        /// </summary>
        /// <param name="value">The new value.</param>
        private void OnSliderValueChanged(float value)
        {
            CurrentBpmValue = (int)value;
            CurrentWeightPosition = 
                MathUtils.ConvertMinMaxValueToNewRange(value, m_Slider.minValue, m_Slider.maxValue, m_LowestWeightPosition, m_HighestWeightposition, true);
        }

        /// <summary>
        /// Called when the scene is destroyed.
        /// </summary>
        private void OnDestroy()
        {
            m_Slider.onValueChanged.RemoveAllListeners();
        }

        /// <summary>
        /// Start the metronome
        /// </summary>
        public void StartClicking()
        {
            m_IsClicking = true;
        }

        /// <summary>
        /// Stop the metronome.
        /// </summary>
        public void StopClicking()
        {
            m_IsClicking = false;
        }

        #endregion // Methods

    }
}
