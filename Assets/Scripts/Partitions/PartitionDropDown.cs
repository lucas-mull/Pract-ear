using Practear.Utils.PropertyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Practear.Partitions
{
    /// <summary>
    /// Populate a UI dropdown with the list of all partitions found in the project.
    /// </summary>
    [RequireComponent(typeof(TMP_Dropdown))]
    public class PartitionDropDown : MonoBehaviour
    {

        #region Internal classes

        /// <summary>
        /// Unity event taking a partition as a parameter.
        /// </summary>
        [Serializable]
        public class PartitionEvent : UnityEvent<Partition> { }

        #endregion // Internal classes

        #region Events

        /// <summary>
        /// Event fired whenever a new partition is selected in the dropdown.
        /// </summary>
        public PartitionEvent PartitionSelected = new PartitionEvent();

        #endregion // Events

        #region Instance variables

        /// <summary>
        /// The partition that is currently selected.
        /// </summary>
        [ReadOnly]
        [SerializeField]
        private Partition m_SelectedPartition;

        /// <summary>
        /// Optional. An icon for the dropdown elements.
        /// </summary>
        [Tooltip("Optional. An icon for the dropdown elements.")]
        [SerializeField]
        private Sprite m_DropdownIcon;

        /// <summary>
        /// The dropdown.
        /// </summary>
        private TMP_Dropdown m_Dropdown;        

        /// <summary>
        /// The list of partitions.
        /// </summary>
        private List<Partition> m_Partitions;

        #endregion // Instance variables

        #region Properties

        /// <summary>
        /// Access the selected partition;
        /// </summary>
        public Partition SelectedPartition { get { return m_SelectedPartition; } }

        #endregion // Properties

        #region Methods

        /// <summary>
        /// Called when the game object is first activated.
        /// </summary>
        private void Awake()
        {
            // Fetch the dropdown and the list of partitions.
            m_Dropdown = GetComponent<TMP_Dropdown>();
            m_Partitions = Configuration.Instance.AllPartitions.ToList();

            // Initialize the dropdown content + listener.
            PopulateDropdown();
            m_Dropdown.onValueChanged.AddListener(OnSelectionChanged);
        }

        /// <summary>
        /// Populate the dropdown with the list of partitions.
        /// </summary>
        private void PopulateDropdown()
        {
            m_Dropdown.ClearOptions();
            List<TMP_Dropdown.OptionData> dataList = new List<TMP_Dropdown.OptionData>();
            foreach (Partition partition in m_Partitions)
                dataList.Add(new TMP_Dropdown.OptionData(partition.Title, m_DropdownIcon));

            m_Dropdown.AddOptions(dataList);
            m_Dropdown.value = 0;
            OnSelectionChanged(0);
        }

        /// <summary>
        /// Called when the dropdown selection changes.
        /// </summary>
        /// <param name="index">The index of the new selection.</param>
        private void OnSelectionChanged(int index)
        {
            m_SelectedPartition = m_Partitions[index];
            PartitionSelected.Invoke(m_SelectedPartition);
        }

        /// <summary>
        /// Called when the scene is destroyed.
        /// </summary>
        private void OnDestroy()
        {
            PartitionSelected.RemoveAllListeners();
        }

        #endregion // Methods

    }
}


