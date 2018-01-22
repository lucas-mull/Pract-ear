using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Practear.Partitions.Editor
{
    /// <summary>
    /// Helper class to import notes from an external folder.
    /// </summary>
    static class ImportNotesHelper
    {

        #region Internal classes

        /// <summary>
        /// Structure used to store a matching result.
        /// </summary>
        private class MatchingResult
        {
            /// <summary>
            /// The path to the audio file
            /// </summary>
            public string FilePath { get; set; }

            /// <summary>
            /// The octave of the note.
            /// </summary>
            public int Octave { get; set; }

            /// <summary>
            /// The note.
            /// </summary>
            public MusicalNote.EName Note { get; set; }

            /// <summary>
            /// The audio clip.
            /// </summary>
            public AudioClip Clip { get; set; }
        }

        #endregion // Internal classes

        #region Constants

        /// <summary>
        /// The prefix to use for all our keys in the editor prefs for <see cref="NotesImporterWindow"/>
        /// </summary>
        private const string PrefsPrefix = "note_import";

        #endregion // Constants

        #region Static variables

        /// <summary>
        /// The list of supported audio extensions.
        /// </summary>
        static private string[] SupportedAudioFormats = new[] 
        {
            ".mp3", ".ogg", ".wav", ".aif", ".aiff", ".mod", ".it", ".s3m", ".xm"
        };

        /// <summary>
        /// Whether to look for a specific octave value during import.
        /// </summary>
        static public bool LookForSpecificOctave = false;

        /// <summary>
        /// The target octave to look for if <see cref="LookForSpecificOctave"/> is true.
        /// </summary>
        static public int TargetOctave;

        /// <summary>
        /// The used system format.
        /// </summary>
        static public NoteFileFormat.Format UsedSystem;

        /// <summary>
        /// The path
        /// </summary>
        static public string AssetsPath;

        /// <summary>
        /// Whether or not to overwrite already present files.
        /// </summary>
        static public bool Overwrite = false;

        /// <summary>
        /// The separator between the note name and the octave number.
        /// </summary>
        static public string Separator = "";

        /// <summary>
        /// The path to the audio files to import.
        /// </summary>
        static public string ImportPath;

        #endregion // Static variables

        #region Static methods

        /// <summary>
        /// Try to import audio files and parse them into music scales from a directory.
        /// </summary>
        /// <param name="path">The path to the directory</param>
        /// <returns>A list of scale if any could be parsed, null otherwise.</returns>
        static public Scale[] TryImportSoundsFrom()
        {
            if (string.IsNullOrEmpty(ImportPath) || !Directory.Exists(ImportPath))
            {
                Debug.LogWarning("The specified path is not valid.");
                return null;
            }

            string[] filePaths = Directory.GetFiles(ImportPath).Where(file => IsFileValid(file)).ToArray();
            if (!filePaths.Any())
            {
                Debug.LogWarning(string.Format("No valid audio file could be found at {0}", ImportPath));
                return null;
            }

            IList<MatchingResult> results;

            if (LookForSpecificOctave)
            {
                results = TryImportTargetOctave(filePaths, TargetOctave, true);
            }
            else
            {
                results = TryImportAllOctaves(filePaths);
            }

            if (results.Any())
            {
                CreateAudioClipsFromResults(ref results);
                return ParseResultsToScales(results);
            }

            return null;
        }

        /// <summary>
        /// Create the audio clips assets from results
        /// </summary>
        /// <param name="results">The results.</param>
        static private void CreateAudioClipsFromResults(ref IList<MatchingResult> results)
        {
            for (int i = 0; i < results.Count; i++)
            {
                // Display progress bar.
                EditorUtility.DisplayProgressBar("Import en cours...", string.Format("Import du fichier {0}/{1}", i + 1, results.Count), (float)(i + 1) / results.Count);

                MatchingResult result = results[i];
                string path = AssetsPath + "/" + result.Octave.ToString();

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string newName = string.Format("{0}{1}", result.Note, Path.GetExtension(result.FilePath));
                path = path + "/" + newName;
                File.Copy(result.FilePath, path, Overwrite);

                // Refresh the database and load the newly created audio clip.
                AssetDatabase.Refresh();
                path = "Assets" + path.Replace(Application.dataPath, string.Empty);
                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                result.Clip = clip;
            }

            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// Create a list of scales from some results.
        /// </summary>
        /// <param name="results">The results to parse</param>
        /// <returns>An array of music scales.</returns>
        static private Scale[] ParseResultsToScales(IList<MatchingResult> results)
        {
            IList<int> createdOctaves = new List<int>();
            IList<Scale> scales = new List<Scale>();
            foreach(MatchingResult result in results)
            {
                if (!createdOctaves.Contains(result.Octave))
                {
                    scales.Add(new Scale(result.Octave));
                    createdOctaves.Add(result.Octave);
                }

                scales.FirstOrDefault(scale => scale.Octave == result.Octave).SetClip(result.Note, result.Clip);
            }

            createdOctaves.Clear();
            createdOctaves = null;

            return scales.ToArray();
        }

        /// <summary>
        /// Fetch the matching results for the target octave for a list of file paths.
        /// </summary>
        /// <param name="filePaths">The file paths.</param>
        /// <param name="targetOctave">The target octave</param>
        /// <param name="lookWithoutOctave">Whether to look for results without the appended octave or not.</param>
        /// <returns>A list of result. No result = empty list.</returns>
        static private List<MatchingResult> TryImportTargetOctave(string[] filePaths, int targetOctave, bool lookWithoutOctave = false)
        {
            List<MatchingResult> results = new List<MatchingResult>();
            string[] notesFormat = NoteFileFormat.GetNoteNames(UsedSystem);
            string[] fileNames = filePaths.Select(filePath => Path.GetFileNameWithoutExtension(filePath)).ToArray();

            int noteIndex = 0;
            foreach (string noteName in notesFormat)
            {
                int fileIndex = 0;
                foreach (string file in fileNames)
                {
                    // Stop at first match for every note, do not take into account further notes.
                    string toFind = string.Format("{0}{1}{2}", noteName, Separator, targetOctave);
                    int index = file.IndexOf(toFind, StringComparison.Ordinal);
                    bool match = false;

                    // Match
                    if (index >= 0)
                    {
                        match = true;
                    }
                    // or else try with just the note name.
                    else if (lookWithoutOctave)
                    {
                        toFind = noteName;
                        index = file.IndexOf(toFind);

                        // Match
                        if (index >= 0)
                        {
                            match = true;
                        }
                    }

                    if (match)
                    {
                        results.Add(new MatchingResult
                        {
                            FilePath = filePaths[fileIndex],
                            Note = (MusicalNote.EName)noteIndex,
                            Octave = UsedSystem == NoteFileFormat.Format.US 
                                ? NoteFileFormat.ConvertOctave(NoteFileFormat.Format.FR, targetOctave) 
                                : targetOctave
                        });

                        break;
                    }

                    fileIndex++;
                }

                noteIndex++;
            }

            return results;
        }

        /// <summary>
        /// Fetch the matching results for the target octave for a list of file paths.
        /// </summary>
        /// <param name="filePaths">The file paths.</param>
        /// <returns>A list of result. No result = empty list.</returns>
        static private IList<MatchingResult> TryImportAllOctaves(string[] filePaths)
        {
            List<MatchingResult> results = new List<MatchingResult>();
            foreach(int octave in NoteFileFormat.GetOctavesList(UsedSystem))
            {
                results.AddRange(TryImportTargetOctave(filePaths, octave));
            }

            return results;
        }

        /// <summary>
        /// Check if the file at path is a valid audio file (valid extension).
        /// </summary>
        /// <param name="filePath">The path to the file</param>
        /// <returns>true if path is valid, false otherwise.</returns>
        static private bool IsFileValid(string filePath)
        {
            return SupportedAudioFormats.Contains(Path.GetExtension(filePath));
        }

        /// <summary>
        /// Save preferences
        /// </summary>
        static public void SavePrefs()
        {
            EditorPrefs.SetInt(string.Format("{0}_format", PrefsPrefix), (int)UsedSystem);
            EditorPrefs.SetString(string.Format("{0}_separator", PrefsPrefix), Separator);
            EditorPrefs.SetString(string.Format("{0}_destination_folder", PrefsPrefix), AssetsPath);
            EditorPrefs.SetBool(string.Format("{0}_overwrite", PrefsPrefix), Overwrite);
            EditorPrefs.SetString(string.Format("{0}_import_path", PrefsPrefix), ImportPath);
        }

        /// <summary>
        /// Load preferences
        /// </summary>
        static public void LoadPrefs()
        {
            UsedSystem = (NoteFileFormat.Format)EditorPrefs.GetInt(string.Format("{0}_format", PrefsPrefix));
            Separator = EditorPrefs.GetString(string.Format("{0}_separator", PrefsPrefix));
            AssetsPath = EditorPrefs.GetString(string.Format("{0}_destination_folder", PrefsPrefix));
            Overwrite = EditorPrefs.GetBool(string.Format("{0}_overwrite", PrefsPrefix));
            ImportPath = EditorPrefs.GetString(string.Format("{0}_import_path", PrefsPrefix));
        }

        #endregion // Static methods

    }
}
