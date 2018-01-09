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

        /// <summary>
        /// Used to pair a <see cref="MusicalNote.EName"/> element with its prefix in an instrument folder.
        /// </summary>
        [Serializable]
        private class Name
        {
            /// <summary>
            /// The label of the note
            /// </summary>
            public MusicalNote.EName Label;
            
            /// <summary>
            /// The prefix
            /// </summary>
            public string Prefix;
        }

        #endregion // Internal classes

        #region Instance variables

        /// <summary>
        /// The list of lengths.
        /// </summary>
        [SerializeField]
        private Length[] m_Lengths;

        /// <summary>
        /// The list of prefixes.
        /// </summary>
        [SerializeField]
        private Name[] m_Prefixes;

        /// <summary>
        /// The separator character between the note prefix and the instrument's name.
        /// </summary>
        [SerializeField]
        private char m_Separator = '_';

        /// <summary>
        /// The list of all the playable partitions in game.
        /// </summary>
        [SerializeField]
        private List<Partition> m_Partitions;

        #endregion // Instance variables

        #region Properties

        /// <summary>
        /// Get the list of all partitions.
        /// TODO - Check for partitions located in asset bundles?
        /// </summary>
        public IEnumerable<Partition> AllPartitions { get { return m_Partitions; } }

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

            if (m_Prefixes == null || m_Prefixes.Length != TotalNames)
            {
                m_Prefixes = InitializeAllPrefixes();
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
        /// Initialize all the prefixes with default values.
        /// </summary>
        /// <returns>A list of all the note names with their default prefix.</returns>
        private Name[] InitializeAllPrefixes()
        {
            List<Name> prefixes = new List<Name>();
            prefixes.Add(new Name { Label = MusicalNote.EName.Do, Prefix = "do" });
            prefixes.Add(new Name { Label = MusicalNote.EName.DoDiese, Prefix = "do#" });
            prefixes.Add(new Name { Label = MusicalNote.EName.DoPlus, Prefix = "do+" });
            prefixes.Add(new Name { Label = MusicalNote.EName.Fa, Prefix = "fa" });
            prefixes.Add(new Name { Label = MusicalNote.EName.FaDiese, Prefix = "fa#" });
            prefixes.Add(new Name { Label = MusicalNote.EName.La, Prefix = "la" });
            prefixes.Add(new Name { Label = MusicalNote.EName.Mi, Prefix = "mi" });
            prefixes.Add(new Name { Label = MusicalNote.EName.Re, Prefix = "re" });
            prefixes.Add(new Name { Label = MusicalNote.EName.ReDiese, Prefix = "re#" });
            prefixes.Add(new Name { Label = MusicalNote.EName.Si, Prefix = "si" });
            prefixes.Add(new Name { Label = MusicalNote.EName.Sol, Prefix = "sol" });
            prefixes.Add(new Name { Label = MusicalNote.EName.LaDiese, Prefix = "la#" });
            prefixes.Add(new Name { Label = MusicalNote.EName.SolDiese, Prefix = "sol#" });

            return prefixes.ToArray();
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
        /// Get a note's prefix.
        /// </summary>
        /// <param name="target">The target note label</param>
        /// <returns>The prefix for this note in the instrument folders.</returns>
        private string GetNotePrefix(MusicalNote.EName target)
        {
            return m_Prefixes.Single(name => name.Label == target).Prefix;
        }

        /// <summary>
        /// Get a note's audio clip for a given instrument.
        /// </summary>
        /// <param name="instrument">The target instrument</param>
        /// <param name="note">The target note</param>
        /// <returns>The audioclip for this note.</returns>
        public AudioClip GetNote(InstrumentData instrument, MusicalNote.EName note)
        {
            string fileName = string.Format("{0}{1}{2}", GetNotePrefix(note), m_Separator, instrument.Name.ToLower());
            string path = Path.Combine(instrument.NotesFolderName, fileName);

            if (NotesCache.Contains(path))
            {
                return NotesCache.GetCachedValue(path);
            }

            AudioClip clip = Resources.Load<AudioClip>(path);
            NotesCache.AddOrUpdate(path, clip);

            return clip;
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

#if UNITY_EDITOR

        /// <summary>
        /// Editor method. Find all the partitions in the assets of the project.
        /// </summary>
        /// <returns>A list of all partitions found.</returns>
        private List<Partition> FindAllPartitions()
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
        /// Serialized property for <see cref="Configuration.m_Prefixes"/>
        /// </summary>
        private SerializedProperty m_PrefixesProperty;

        /// <summary>
        /// Are the lengths foldout ?
        /// </summary>
        private bool m_AreLengthsFoldout;

        /// <summary>
        /// Are the prefixes foldout ?
        /// </summary>
        private bool m_ArePrefixesFoldout;

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
            m_PrefixesProperty = serializedObject.FindProperty("m_Prefixes");

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
            DrawLengths();
            DrawPrefixes();

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
        /// Draw the list of prefixes
        /// </summary>
        private void DrawPrefixes()
        {
            if (m_PrefixesProperty.arraySize == 0)
                return;

            m_ArePrefixesFoldout = EditorGUILayout.Foldout(m_ArePrefixesFoldout, "Prefixes", true);

            if (m_ArePrefixesFoldout)
            {
                EditorGUI.indentLevel++;

                EditorUtils.DrawPropertyFields(serializedObject, "m_Separator");
                for (int i = 0; i < m_PrefixesProperty.arraySize; i++)
                {
                    SerializedProperty name = m_PrefixesProperty.GetArrayElementAtIndex(i);

                    SerializedProperty nameEnum = name.FindPropertyRelative("Label");
                    string enumValue = nameEnum.enumNames[nameEnum.enumValueIndex];

                    SerializedProperty prefix = name.FindPropertyRelative("Prefix");

                    prefix.stringValue = EditorGUILayout.TextField(enumValue, prefix.stringValue);
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


