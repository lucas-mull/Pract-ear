using UnityEngine;
using Practear.Utils;
using System.Linq;
using System;

namespace Practear.Instruments
{
    /// <summary>
    /// Simple manager class to define the instrument database to use at runtime.
    /// </summary>
    sealed public class InstrumentDatabaseManager : AbstractSingleton<InstrumentDatabaseManager>
    {

        #region Instance variables

        /// <summary>
        /// The instrument database to use.
        /// </summary>
        [Tooltip("The instrument database to use.")]
        [SerializeField]
        private InstrumentDatabase m_Database;

        #endregion // Instance variables

        #region Properties

        /// <summary>
        /// Access the instrument database
        /// </summary>
        public InstrumentDatabase InstrumentDatabase { get { return m_Database; } }

        #endregion // Properties

        #region Methods

        /// <summary>
        /// Find instrument data within the database manager
        /// </summary>
        /// <param name="instrumentName">The name of the instrument to find.</param>
        /// <returns>The instrument data if found, null otherwise.</returns>
        public InstrumentData FindInstrumentData(string instrumentName)
        {
            if (InstrumentDatabase == null || InstrumentDatabase.Instruments == null)
                return null;

            return InstrumentDatabase.Instruments.FirstOrDefault(i => i.Name.Equals(instrumentName, StringComparison.InvariantCultureIgnoreCase));
        }

        #endregion // Methods

    }
}

