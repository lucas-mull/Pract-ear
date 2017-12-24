using UnityEngine;

namespace Practear.Instruments
{
    /// <summary>
    /// Used to store information relative to multiple instruments.
    /// </summary>
    [CreateAssetMenu(fileName = "New Instrument Database")]
    public class InstrumentDatabase : ScriptableObject
    {
        #region Instance variables

        /// <summary>
        /// All the instruments in this database.
        /// </summary>
        public InstrumentData[] Instruments;

        #endregion // Instance variables

    }
}
