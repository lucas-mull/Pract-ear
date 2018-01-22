using UnityEngine;
using System.Collections.Generic;
using Practear.Utils;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace Practear.Partitions
{
    /// <summary>
    /// Scriptable object used to store a partition (list of notes).
    /// </summary>
    [CreateAssetMenu(fileName = "New Partition")]
    public class Partition : ScriptableObject
    {

        #region Instance variables

        /// <summary>
        /// The title of the song.
        /// </summary>
        [SerializeField]
        private string m_Title;

        /// <summary>
        /// The list of notes
        /// </summary>
        [SerializeField]
        private List<MusicalNote> m_Notes;

        /// <summary>
        /// The index of the current note.
        /// </summary>
        private int m_CurrentNoteIndex;

        #endregion // Instance variables

        #region Properties

        /// <summary>
        /// The note count.
        /// </summary>
        public int NotesCount { get { return m_Notes.Count; } }

        /// <summary>
        /// The title.
        /// </summary>
        public string Title { get { return m_Title; } }

        #endregion // Properties

        #region Methods

        /// <summary>
        /// Get the current note and advance to the next.
        /// </summary>
        /// <returns>The current note.</returns>
        public MusicalNote Next()
        {
            if (m_CurrentNoteIndex >= m_Notes.Count)
            {
                Rewind();
                return null;
            }

            MusicalNote current = m_Notes[m_CurrentNoteIndex];
            m_CurrentNoteIndex++;

            return current;
        }

        /// <summary>
        /// Rewind the partition to the beginning.
        /// </summary>
        public void Rewind()
        {
            m_CurrentNoteIndex = 0;
        }

        /// <summary>
        /// Is the next note the last one ?
        /// </summary>
        /// <returns>true if the next node is the last one, false otherwise.</returns>
        public bool IsLastNote()
        {
            return m_CurrentNoteIndex == NotesCount;
        }

        #endregion // Methods

    }

    #region Custom editor

#if UNITY_EDITOR

    /// <summary>
    /// Custom editor for <see cref="Partition"/>
    /// </summary>
    [CustomEditor(typeof(Partition))]
    public class PartitionEditor : Editor
    {

        #region Instance variables

        /// <summary>
        /// The notes
        /// </summary>
        private ReorderableList m_NoteList;

        #endregion // Instance variables

        #region Methods

        /// <summary>
        /// Called on enable.
        /// </summary>
        private void OnEnable()
        {
            m_NoteList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_Notes"),  true, true, true, true);
            m_NoteList.drawElementCallback = DrawElement;
            m_NoteList.drawHeaderCallback = DrawHeader;
        }

        /// <summary>
        /// <see cref="Editor.OnInspectorGUI"/>
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorUtils.DrawPropertyFields(serializedObject, "m_Title");
            EditorGUILayout.Space();

            Rect currentRect = EditorGUILayout.GetControlRect();
            EditorGUI.LabelField(new Rect(currentRect.x, currentRect.y, currentRect.width / 3f, EditorGUIUtility.singleLineHeight), "Note", EditorStyles.boldLabel);
            EditorGUI.LabelField(new Rect(currentRect.x + currentRect.width / 3f, currentRect.y, currentRect.width / 3f, EditorGUIUtility.singleLineHeight), "Longueur", EditorStyles.boldLabel);
            EditorGUI.LabelField(new Rect(currentRect.x + 2*currentRect.width / 3f, currentRect.y, currentRect.width / 3f, EditorGUIUtility.singleLineHeight), "Octave", EditorStyles.boldLabel);

            m_NoteList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draw the header
        /// </summary>
        /// <param name="rect">The rect</param>
        private void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Notes");
        }

        /// <summary>
        /// Draw a list element
        /// </summary>
        /// <param name="rect">The rect</param>
        /// <param name="index">The index of the element</param>
        /// <param name="isActive">is the element active?</param>
        /// <param name="isFocused">is the element focused?</param>
        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = m_NoteList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width / 3f, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("m_Name"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + rect.width / 3f, rect.y, rect.width / 3f, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("m_Length"), GUIContent.none);

            SerializedProperty octave = element.FindPropertyRelative("m_Octave");
            int[] values = MusicalNote.OctavesFr;
            string[] options = values.Select(value => value.ToString()).ToArray();

            // Consistency check.
            if (!values.Contains(octave.intValue))
                octave.intValue = values[0];

            octave.intValue = EditorGUI.IntPopup(new Rect(rect.x + rect.width / 3f * 2, rect.y, rect.width / 3f, EditorGUIUtility.singleLineHeight),
                octave.intValue, options, values);
        }

        #endregion // Methods

    }

#endif

    #endregion // Custom editor

}