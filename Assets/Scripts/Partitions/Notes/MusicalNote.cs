using UnityEngine;
using System;

namespace Practear.Partitions
{
    /// <summary>
    /// Simple class to represent a musical note.
    /// TODO - Ajout des octaves ? + Ajout des notes manquantes.
    /// </summary>
    [Serializable]
    public class MusicalNote
    {

        #region Internal enums

        /// <summary>
        /// Enum for note's length.
        /// </summary>
        public enum ELength
        {                   
            Blanche,
            Noire,
            Croche,
            NoirePointee,
            CrochePointee,
            BlanchePointee,
            Ronde
        }

        /// <summary>
        /// Enum for the note's name (FR system).
        /// </summary>
        public enum EName
        {
            Do,
            Re,
            Mi,
            Fa,
            Sol,
            La,
            Si,
            DoDiese,
            ReDiese,
            FaDiese,    
            SolDiese,
            LaDiese
        }

        /// <summary>
        /// Enum for the note's name (US system).
        /// </summary>
        public enum ENameUS
        {
            C,
            D,
            E,
            F,
            G,
            A,
            B,
            Db,
            Eb,
            Gb,
            Ab,
            Bb
        }

        #endregion // Internal enums

        #region Static variables

        /// <summary>
        /// List of octave numbers (FR)
        /// </summary>
        static public int[] OctavesFr = new [] { -2, -1, 1, 2, 3, 4, 5, 6, 7 };

        /// <summary>
        /// List of octave numbers (US)
        /// </summary>
        static public int[] OctavesUs = new [] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

        #endregion // Static variables

        #region Instance variables

        /// <summary>
        /// The note
        /// </summary>
        [SerializeField]
        private EName m_Name;

        /// <summary>
        /// The length
        /// </summary>
        [SerializeField]
        private ELength m_Length;

        /// <summary>
        /// The octave for this note.
        /// </summary>
        [SerializeField]
        private int m_Octave;

        #endregion // Instance variables

        #region Properties

        /// <summary>
        /// Accessor for <see cref="m_Name"/>
        /// </summary>
        public EName Name { get { return m_Name; } }

        /// <summary>
        /// Accessor for <see cref="m_Length"/>
        /// </summary>
        public ELength Length { get { return m_Length; } }

        /// <summary>
        /// Accessor for <see cref="m_Octave"/>
        /// </summary>
        public int Octave
        {
            get { return m_Octave; }
            set { m_Octave = value; }
        }

        #endregion // Properties

        #region Constructors

        /// <summary>
        /// Copy constructor
        /// </summary>
        public MusicalNote(MusicalNote source)
        {
            m_Name = source.Name;
            m_Length = source.Length;
            m_Octave = source.Octave;
        }

        #endregion // Constructors

    }
}


