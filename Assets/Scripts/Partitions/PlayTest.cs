using Practear.Instruments;
using UnityEngine;

namespace Practear.Partitions
{
    /// <summary>
    /// Play test a selected partition.
    /// </summary>
    public class PlayTest : MonoBehaviour
    {

        #region Instance variables

        /// <summary>
        /// The list of instruments.
        /// </summary>
        [Tooltip("The list of instruments.")]
        [SerializeField]
        private Instrument[] m_Instruments;

        /// <summary>
        /// The reference to the partition dropdown.
        /// </summary>
        [Tooltip("The reference to the partition dropdown.")]
        [SerializeField]
        private PartitionDropDown m_Dropdown;

        /// <summary>
        /// The reference to the instrument that's currently playing.
        /// </summary>
        private Instrument m_CurrentInstrument;

        #endregion // Instance variables

        #region Methods

        /// <summary>
        /// Called when the game object is first activated.
        /// </summary>
        private void Awake()
        {
            foreach (Instrument instrument in m_Instruments)
                instrument.Clicked.AddListener(OnInstrumentClicked);
        }

        /// <summary>
        /// Called when an instrument is clicked.
        /// </summary>
        /// <param name="target">The instrument that was clicked.</param>
        private void OnInstrumentClicked(Instrument target)
        {
            if (m_CurrentInstrument)
                m_CurrentInstrument.StopAllSounds();

            m_CurrentInstrument = target;
            target.Play(m_Dropdown.SelectedPartition);
        }

        #endregion // Methods

    }
}

