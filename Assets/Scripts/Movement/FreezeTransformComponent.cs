using System;

namespace Practear.Movement
{
    /// <summary>
    /// Simple structure used to hold booleans for x, y and z components of a transform position or rotation.
    /// </summary>
    [Serializable]
    public class FreezeTransformComponent
    {

        #region Instance variables

        /// <summary>
        /// X component
        /// </summary>
        public bool X;

        /// <summary>
        /// Y component
        /// </summary>
        public bool Y;

        /// <summary>
        /// Z component
        /// </summary>
        public bool Z;

        #endregion // Instance variables

    }
}
