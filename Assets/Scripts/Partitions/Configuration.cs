using UnityEngine;
using System;
using Practear.Utils;
using System.Collections.Generic;
using System.Linq;
using Practear.Instruments;
using System.IO;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace Practear.Partitions
{
    /// <summary>
    /// Use this singleton to configure and adjust general settings.
    /// Default settings are applied when first loading this script in the editor.
    /// To make sure your changes are not erased, don't forget to make a prefab of the singleton once everything
    /// is setup properly.
    /// </summary>
    [ExecuteInEditMode]
    public class Configuration : AbstractSingleton<Configuration>
    {

        #region Internal classes

        /// <summary>
        /// Used to pair a <see cref="MusicalNote.ELength"/> element with an actual length in seconds.
        /// </summary>
        [Serializable]
        private class Length
        {
            /// <summary>
            /// The label of the length
            /// </summary>
            public MusicalNote.ELength Label;

            /// <summary>
            /// The actual length in seconds
            /// </summary>
            public float LengthInSeconds;
        }

        #endregion // Internal classes

        #region Instance variables

        /// <summary>
        /// The instrument database to use for the game.
        /// </summary>
        [Tooltip("The instrument database to use for the game.")]
        [SerializeField]
        private InstrumentDatabase m_InstrumentDatabase;

        /// <summary>
        /// The list of lengths.
        /// </summary>
        [SerializeField]
        private Length[] m_Lengths;

        /// <summary>
        /// The list of all the playable partitions in game.
        /// </summary>
        [SerializeField]
        private List<Partition> m_Partitions;

        /// <summary>
        /// Check this to force the instruments to use their default scales in partitions.
        /// Otherwise, the right octave will be fetched whenever available.
        /// </summary>
        [Tooltip("Check this to force the instruments to use their default scales in partitions. Otherwise, the right octave will be fetched whenever available.")]
        public bool UseInstrumentDefaultScale;

        #endregion // Instance variables

        #region Properties

        /// <summary>
        /// Get the list of all partitions.
        /// TODO - Check for partitions located in asset bundles?
        /// </summary>
        public IEnumerable<Partition> AllPartitions { get { return m_Partitions; } }

        /// <summary>
        /// Access the database of instruments.
        /// </summary>
        public InstrumentDatabase InstrumentDatabase { get { return m_InstrumentDatabase; } }

        /// <summary>
        /// Get the total number of existing lengths for musical notes.
        /// </summary>
        private int TotalLengths { get { return Enum.GetValues(typeof(MusicalNote.ELength)).Length; } }

        /// <summary>
        /// Get the total number of existing labels for musical notes.
        /// </summary>
        private int TotalNames { get { return Enum.GetValues(typeof(MusicalNote.EName)).Length; } }

        #endregion // Properties

        #region Methods

        /// <summary>
        /// Called when the script is loaded (also executed in the editor for this script).
        /// </summary>
        override public void Awake()
        {
            base.Awake();

            if (m_Lengths == null || m_Lengths.Length != TotalLengths)
            {
                m_Lengths = InitializeAllLengths();
            }
        }

        /// <summary>
        /// Get an initialized array of all the lengths with default values in seconds.
        /// </summary>
        /// <returns>A list of all the lengths with their default values in seconds.</returns>
        private Length[] InitializeAllLengths()
        {
            List<Length> lengths = new List<Length>();
            lengths.Add(new Length { Label = MusicalNote.ELength.Blanche, LengthInSeconds = 1f });
            lengths.Add(new Length { Label = MusicalNote.ELength.Noire, LengthInSeconds = 0.5f });
            lengths.Add(new Length { Label = MusicalNote.ELength.Croche, LengthInSeconds = 0.25f });
            lengths.Add(new Length { Label = MusicalNote.ELength.NoirePointee, LengthInSeconds = 0.75f });
            lengths.Add(new Length { Label = MusicalNote.ELength.CrochePointee, LengthInSeconds = 0.375f });
            lengths.Add(new Length { Label = MusicalNote.ELength.BlanchePointee, LengthInSeconds = 1.5f });
            lengths.Add(new Length { Label = MusicalNote.ELength.Ronde, LengthInSeconds = 2f });

            return lengths.ToArray();
        }

        /// <summary>
        /// Get a length's value in seconds.
        /// </summary>
        /// <param name="target">The target length label</param>
        /// <returns>The length in seconds.</returns>
        public float GetNoteLengthInSeconds(MusicalNote.ELength target)
        {
            return m_Lengths.Single(length => length.Label == target).LengthInSeconds;
        }

        /// <summary>
        /// Get a partition at random.
        /// </summary>
        /// <returns>A random partition.</returns>
        public Partition GetRandomPartition()
        {
            if (m_Partitions == null || m_Partitions.Count == 0)
                throw new NullReferenceException("Couldn't get a random partition. The list of partition is empty.");

            int index = Random.Range(0, m_Partitions.Count - 1);

            return m_Partitions[index];
        }

        /// <summary>
        /// Find instrument data within the database
        /// </summary>
        /// <param name="instrumentName">The name of the instrument to find.</param>
        /// <returns>The instrument data if found, null otherwise.</returns>
        public InstrumentData FindInstrumentData(string instrumentName)
        {
            if (m_InstrumentDatabase == null || m_InstrumentDatabase.Instruments == null)
                return null;

            return m_InstrumentDatabase.Instruments.FirstOrDefault(i => i.Name.Equals(instrumentName, StringComparison.InvariantCultureIgnoreCase));
        }

#if UNITY_EDITOR

        /// <summary>
        /// Editor method. Find all the partitions in the assets of the project.
        /// </summary>
        /// <returns>A list of all partitions found.</returns>
        static public List<Partition> FindAllPartitions()
        {
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(Partition).Name));
            string[] paths = guids.Select(id => AssetDatabase.GUIDToAssetPath(id)).ToArray();

            return paths.Select(path => AssetDatabase.LoadAssetAtPath<Partition>(path)).ToList();            
        }

        /// <summary>
        /// Initialize <see cref="m_Partitions"/> with the array of partitions found in the assets.
        /// </summary>
        public void InitializeAllPartitions()
        {
            m_Partitions = FindAllPartitions();
        }

#endif

#endregion // Methods

    }

    #region Custom editor

#if UNITY_EDITOR

    /// <summary>
    /// Custom editor for <see cref="Configuration"/>
    /// </summary>
    [CustomEditor(typeof(Configuration))]
    public class ConfigurationEditor : Editor
    {

        #region Instance variables

        /// <summary>
        /// Serialized property for <see cref="Configuration.m_Lengths"/>
        /// </summary>
        private SerializedProperty m_LengthsProperty;

        /// <summary>
        /// Are the lengths foldout ?
        /// </summary>
        private bool m_AreLengthsFoldout;

        /// <summary>
        /// A reorderable list for the partitions.
        /// </summary>
        private ReorderableList m_PartitionsList;

        #endregion // Instance variables

        #region Methods

        /// <summary>
        /// Called on enable
        /// </summary>
        private void OnEnable()
        {
            m_LengthsProperty = serializedObject.FindProperty("m_Lengths");

            m_PartitionsList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_Partitions"), 
                true, true, true, true);
            m_PartitionsList.drawHeaderCallback = DrawPartitionHeader;
            m_PartitionsList.drawElementCallback = DrawPartitionElement;
        }

        /// <summary>
        /// <see cref="Editor.OnInspectorGUI"/>
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorUtils.DrawDefaultScriptField<Configuration>(target);
            EditorUtils.DrawPropertyFields(serializedObject, "DontDestroyOnLoad", "m_InstrumentDatabase", "UseInstrumentDefaultScale");
            DrawLengths();

            EditorGUILayout.Space();
            m_PartitionsList.DoLayoutList();
            if (GUILayout.Button("Find all partitions"))
            {
                ((Configuration)target).InitializeAllPartitions();
            }

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draw the lengths.
        /// </summary>
        private void DrawLengths()
        {
            if (m_LengthsProperty.arraySize == 0)
                return;

            m_AreLengthsFoldout = EditorGUILayout.Foldout(m_AreLengthsFoldout, "Lengths", true);

            if (m_AreLengthsFoldout)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < m_LengthsProperty.arraySize; i++)
                {
                    SerializedProperty length = m_LengthsProperty.GetArrayElementAtIndex(i);

                    SerializedProperty lengthEnum = length.FindPropertyRelative("Label");
                    string enumValue = lengthEnum.enumNames[lengthEnum.enumValueIndex];

                    SerializedProperty lengthInSeconds = length.FindPropertyRelative("LengthInSeconds");
                    
                    lengthInSeconds.floatValue = EditorGUILayout.FloatField(enumValue, lengthInSeconds.floatValue);
                }
                EditorGUI.indentLevel--;                
            }
        }

        /// <summary>
        /// Draw the partitions' list header.
        /// </summary>
        /// <param name="rect">The rect</param>
        private void DrawPartitionHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Partitions");
        }

        /// <summary>
        /// Draw a partition's list element.
        /// </summary>
        /// <param name="rect">The position</param>
        /// <param name="index">The index of the element in the list</param>
        /// <param name="isActive">is the element active?</param>
        /// <param name="isFocused">is the element focused?</param>
        private void DrawPartitionElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = m_PartitionsList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            element.objectReferenceValue = EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                element.objectReferenceValue, typeof(Partition), false);
        }

        #endregion // Methods

    }

#endif

    #endregion // Custom editor

}


