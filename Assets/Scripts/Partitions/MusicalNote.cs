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
        /// Enum for the note's name
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

            /// <summary>
            /// Do grave.
            /// </summary>
            DoPlus,
            DoDiese,
            FaDiese,
            ReDiese,
            LaDiese,
            SolDiese
        }

        #endregion // Internal enums

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

        #endregion // Properties

    }
}


