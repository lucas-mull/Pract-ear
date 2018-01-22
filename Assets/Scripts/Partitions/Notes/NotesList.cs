using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Practear.Partitions
{
    /// <summary>
    /// Scriptable object used to store a list of notes for an instrument.
    /// </summary>
    [CreateAssetMenu(fileName = "New Notes List")]
    public class NotesList : ScriptableObject
    {

        #region Instance variables

        /// <summary>
        /// The list of scales (gammes).
        /// </summary>
        public List<Scale> Scales;

        /// <summary>
        /// The default scale to use in partitions.
        /// </summary>
        public int DefaultScale;

        #endregion // Instance variables

        #region Methods

        /// <summary>
        /// Get the audio clip for a given note.
        /// </summary>
        /// <param name="note">The note</param>
        /// <returns>The audio clip (if set).</returns>
        public AudioClip GetAudioClip(MusicalNote note)
        {
            Scale target = Scales.FirstOrDefault(scale => scale.Octave == note.Octave);
            if (target != null)
                return target.GetClip(note.Name);

            return null;
        }

        /// <summary>
        /// Get the list of complete scales for this note list.
        /// </summary>
        /// <returns>The list of complete scales.</returns>
        public IEnumerable<Scale> GetCompleteScales()
        {
            List<Scale> scales = new List<Scale>();
            foreach (Scale scale in Scales)
            {
                if (scale.IsComplete())
                {
                    scales.Add(scale);
                }
            }

            return scales;
        }

        #endregion // Methods

    }
}
