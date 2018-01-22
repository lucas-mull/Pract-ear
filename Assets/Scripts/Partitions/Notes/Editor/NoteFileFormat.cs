using Practear.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Practear.Partitions.Editor
{
    /// <summary>
    /// Specify a format for a note file.
    /// </summary>
    static public class NoteFileFormat
    {

        #region Internal enums

        public enum Format
        {
            /// <summary>
            /// The const id to use the french system for notes and octave numbers.
            /// </summary>
            FR = 0,

            /// <summary>
            /// The const id to use the Trump system for notes and octave numbers.
            /// </summary>
            US = 1
        }

        #endregion // Internal enums

        #region Static methods

        /// <summary>
        /// Get the octaves list for a given format.
        /// </summary>
        /// <param name="format">The format to use.</param>
        /// <returns>The matching octaves list.</returns>
        static public int[] GetOctavesList(Format format)
        {
            switch(format)
            {
                case Format.FR:
                    return MusicalNote.OctavesFr;
                case Format.US:
                    return MusicalNote.OctavesUs;
                default:
                    throw new FormatException(string.Format("Unknown format : {0}", format));
            }
        }

        /// <summary>
        /// Get the note names for a given format.
        /// </summary>
        /// <param name="format">The format to use.</param>
        /// <returns>The note names for this format.</returns>
        static public string[] GetNoteNames(Format format)
        {
            switch(format)
            {
                case Format.FR:
                    return GetEnumStringArray(typeof(MusicalNote.EName));
                case Format.US:
                    return GetEnumStringArray(typeof(MusicalNote.ENameUS));
                default:
                    throw new FormatException(string.Format("Unknown format : {0}", format));
            }
        }

        /// <summary>
        /// Convert an octave to another format.
        /// </summary>
        /// <param name="destination">the destination format.</param>
        /// <param name="octave">The octave to convert.</param>
        /// <returns>The octave number for the destination format.</returns>
        static public int ConvertOctave(Format destination, int octave)
        {
            int[] sourceOctaves = destination == Format.FR ? MusicalNote.OctavesUs : MusicalNote.OctavesFr;
            int[] destinationOctaves = destination == Format.FR ? MusicalNote.OctavesFr : MusicalNote.OctavesUs;
            int index = sourceOctaves.IndexOf(octave);
            if (index < 0)
            {
                throw new KeyNotFoundException(string.Format("The octave number '{0}' does not exist for the given format", octave));
            }

            return destinationOctaves[index];
        }

        /// <summary>
        /// Helper method to convert the enum names to a string array.
        /// </summary>
        /// <param name="enumtype">The type of the enum.</param>
        /// <returns>The string array containing the enum labels.</returns>
        static private string[] GetEnumStringArray(Type enumtype)
        {
            if (!enumtype.IsEnum)
                throw new ArgumentException(string.Format("The type '{0}' is not an enum!", enumtype.Name));

            List<string> names = new List<string>();
            foreach(Enum e in Enum.GetValues(enumtype))
            {
                names.Add(e.ToString());
            }

            return names.ToArray();
        }

        #endregion // Static methods

    }
}
