using UnityEngine;
using System;
using Practear.Partitions;

namespace Practear.Instruments
{
    /// <summary>
    /// Used to hold the data concerning an instrument.
    /// </summary>
    [Serializable]
    public class InstrumentData
    {

        #region Instance variables

        /// <summary>
        /// The name of the instrument.
        /// </summary>
        [Tooltip("The name of the instrument.")]
        public string Name;

        // TODO Add category management

        /// <summary>
        /// The instrument's sprite
        /// </summary>
        [Tooltip("The instrument's sprite")]
        public Sprite Sprite;

        /// <summary>
        /// The notes for this instrument.
        /// </summary>
        [Tooltip("The notes for this instrument.")]
        public NotesList Notes;

        #endregion // Instance variables

    }
}

