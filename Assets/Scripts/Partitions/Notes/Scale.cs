using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Practear.Partitions
{
    /// <summary>
    /// Represents a music scale (gamme).
    /// This uses the french system for octaves.
    /// </summary>
    [Serializable]
    public class Scale
    {

        #region Internal classes

        /// <summary>
        /// Associate a note and audio clip.
        /// </summary>
        [Serializable]
        public class NoteClip
        {

            #region Instance variables

            /// <summary>
            /// The note's name
            /// </summary>
            public MusicalNote.EName Note;

            /// <summary>
            /// The audio clip
            /// </summary>
            public AudioClip Clip;

            #endregion // Instance variables

        }

        #endregion // Internal classes

        #region Instance variables

        /// <summary>
        /// The octave for this scale.
        /// </summary>
        [SerializeField]
        private int m_Octave;

        /// <summary>
        /// The list of notes and their associated audio clip.
        /// </summary>
        [SerializeField]
        public NoteClip[] Notes;

        #endregion // Instance variables

        #region Properties

        /// <summary>
        /// Access the octave of this scale.
        /// </summary>
        public int Octave
        {
            get { return m_Octave; }
            set { m_Octave = value; }
        }

        #endregion // Properties

        #region Constructors

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="octave">The octave</param>
        public Scale(int octave)
        {
            if (!IsOctaveValid(octave))
                throw new IndexOutOfRangeException(string.Format("Octave {0} is not a valid octave number !", octave));

            m_Octave = octave;
            Notes = InitializeNotes();
        }

        #endregion // Constructors

        #region Methods

        /// <summary>
        /// Does the given octave exist in the french system?
        /// </summary>
        /// <param name="octave">The octave number</param>
        /// <returns>true if the octave is valid, false otherwise</returns>
        private bool IsOctaveValid(int octave)
        {
            return MusicalNote.OctavesFr.Contains(octave);
        }

        /// <summary>
        /// Set an audio clip for a given note
        /// </summary>
        /// <param name="note">The note</param>
        /// <param name="clip">The clip</param>
        public void SetClip(MusicalNote.EName note, AudioClip clip)
        {
            Notes.First(noteClip => noteClip.Note == note).Clip = clip;
        }

        /// <summary>
        /// Get the audio clip for a given note.
        /// </summary>
        /// <param name="note">The note.</param>
        /// <returns>The audio clip (can be null if unset).</returns>
        public AudioClip GetClip(MusicalNote.EName note)
        {
            return Notes.First(noteClip => noteClip.Note == note).Clip;
        }

        /// <summary>
        /// Is this scale complete ? (all the clips are supplied).
        /// </summary>
        /// <returns>true if every note has a matching clip, false otherwise.</returns>
        public bool IsComplete()
        {
            foreach(NoteClip noteClip in Notes)
            {
                if (!noteClip.Clip)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Initialize the note list with all the defined names.
        /// </summary>
        /// <returns>An initialized list.</returns>
        private NoteClip[] InitializeNotes()
        {
            List<NoteClip> notes = new List<NoteClip>();
            foreach(MusicalNote.EName note in Enum.GetValues(typeof(MusicalNote.EName)))
            {
                notes.Add(new NoteClip { Note = note });
            }

            return notes.ToArray();
        }

        #endregion // Methods

    }
}
