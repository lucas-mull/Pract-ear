using Practear.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Practear.Partitions.Editor
{
    public class NotesImporterWindow : EditorWindow
    {

        #region Static methods

        /// <summary>
        /// Show the editor window
        /// </summary>
        [MenuItem("Window/Notes Importer")]
        static private NotesImporterWindow ShowWindow()
        {
            NotesImporterWindow window = (NotesImporterWindow)GetWindow(typeof(NotesImporterWindow));
            window.titleContent = new GUIContent("Notes importer");
            window.minSize = new Vector2(200, 200);

            return window;
        }

        /// <summary>
        /// Open the window loading a specific asset.
        /// </summary>
        /// <param name="asset"></param>
        static private void OpenWindowFor(NotesList asset)
        {
            NotesImporterWindow window = ShowWindow();
            window.SetAsset(asset);
        }

        /// <summary>
        /// Called when the user double clicks on an asset in the project.
        /// </summary>
        /// <param name="instanceID">The ID of the asset</param>
        /// <param name="line">Not a fucking clue what this is but it is necessary.</param>
        /// <returns>true if the opening of the asset was handled, false otherwise.</returns>
        [OnOpenAsset]
        static private bool OnOpenAsset(int instanceID, int line)
        {
            Object asset = EditorUtility.InstanceIDToObject(instanceID);
            NotesList target = asset as NotesList;
            if (target != null)
            {
                OpenWindowFor(target);

                return true;
            }

            return false;
        }

        #endregion // Static methods

        #region Instance variables

        /// <summary>
        /// The asset loaded and managed by the window.
        /// </summary>
        [SerializeField]
        private NotesList m_Asset;

        /// <summary>
        /// The index of the selected tab (scale) in the window.
        /// </summary>
        private int m_SelectedTabIndex;

        #endregion // Instance variables

        #region Methods

        /// <summary>
        /// Called on enable
        /// </summary>
        private void OnEnable()
        {
            ImportNotesHelper.LoadPrefs();
        }

        /// <summary>
        /// The window code.
        /// </summary>
        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            DrawTitle();
            DrawNotesList();
            DrawAddScaleButton();
            DrawDefaultScale();
            DrawImportSection();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(m_Asset);
                ImportNotesHelper.SavePrefs();
            }
        }

        /// <summary>
        /// Draw the title
        /// </summary>
        private void DrawTitle()
        {
            GUIStyle centerAlignmentStyle = new GUIStyle(EditorStyles.boldLabel);
            centerAlignmentStyle.alignment = TextAnchor.MiddleCenter;
            EditorGUILayout.LabelField(m_Asset.name, centerAlignmentStyle);
        }

        /// <summary>
        /// Draw the notes list.
        /// </summary>
        private void DrawNotesList()
        {
            // Don't draw anything if there is nothing to draw.
            if (!m_Asset || m_Asset.Scales == null || !m_Asset.Scales.Any())
                return;

            string[] tabNames = m_Asset.Scales.Select(scale => scale.Octave.ToString()).ToArray();

            // Consistency check.
            if (m_SelectedTabIndex >= tabNames.Length)
                m_SelectedTabIndex = 0;

            m_SelectedTabIndex = GUILayout.Toolbar(m_SelectedTabIndex, tabNames);

            // Draw the selected scale.
            Scale target = m_Asset.Scales[m_SelectedTabIndex];
            DrawScale(ref target);            
        }

        /// <summary>
        /// Draw a given scale.
        /// </summary>
        /// <param name="scale">The scale (referenced).</param>
        private void DrawScale(ref Scale scale)
        {
            int[] values = GetAvailableOctaves(scale.Octave);
            string[] displayedOptions = values.Select(octave => octave.ToString()).ToArray();
            scale.Octave = EditorGUILayout.IntPopup("Octave", scale.Octave, displayedOptions, values);

            // Draw the notes.
            foreach(Scale.NoteClip noteClip in scale.Notes)
            {
                EditorGUILayout.BeginHorizontal();
                noteClip.Clip = (AudioClip)EditorGUILayout.ObjectField(
                        noteClip.Note.ToString(), 
                        noteClip.Clip, 
                        typeof(AudioClip), 
                        false);

                // Draw test button
                if (GUILayout.Button("Preview", GUILayout.Width(70)))
                {
                    if (noteClip.Clip)
                    {
                        PublicAudioUtils.StopAllClips();
                        PublicAudioUtils.PlayClip(noteClip.Clip);
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            // Delete button.
            if (GUILayout.Button("Supprimer la gamme"))
            {
                m_Asset.Scales.Remove(scale);
            }
        }

        /// <summary>
        /// Draw the "Add scale" button.
        /// </summary>
        private void DrawAddScaleButton()
        {
            // Don't draw the button if all the octaves have been created.
            int[] allAvailableOctaves = GetAvailableOctaves(-15);
            if (!allAvailableOctaves.Any())
                return;

            if (!m_Asset.Scales.Any())
            {
                EditorGUILayout.HelpBox("Aucune gamme n'a encore été définie. Ajoutez en une plus bas" +
                    " ou bien essayez d'importer une liste de notes depuis un dossier", MessageType.Info);
            }

            if (GUILayout.Button("Ajouter une gamme"))
            {
                // Create a new scale for the first available octave.
                m_Asset.Scales.Add(new Scale(allAvailableOctaves.First()));
            }
        }

        /// <summary>
        /// Draw the "default scale" field.
        /// </summary>
        private void DrawDefaultScale()
        {
            int[] completeOctaves = m_Asset.GetCompleteScales().Select(scale => scale.Octave).ToArray();
            if (completeOctaves.Any())
            {
                EditorGUILayout.Space();
                string[] displayedNames = completeOctaves.Select(octave => octave.ToString()).ToArray();

                if (!completeOctaves.Contains(m_Asset.DefaultScale))
                    m_Asset.DefaultScale = completeOctaves[0];

                m_Asset.DefaultScale =
                    EditorGUILayout.IntPopup("Gamme par défaut", m_Asset.DefaultScale, displayedNames, completeOctaves);
            }
        }

        /// <summary>
        /// Draw the import button
        /// </summary>
        private void DrawImportSection()
        {
            EditorGUILayout.Space();
            EditorUtils.DrawHeader("Paramètres d'import");
            ImportNotesHelper.UsedSystem = (NoteFileFormat.Format)EditorGUILayout.EnumPopup("Système utilisé", ImportNotesHelper.UsedSystem);
            ImportNotesHelper.Separator = EditorGUILayout.TextField("Séparateur", ImportNotesHelper.Separator);

            EditorGUILayout.BeginHorizontal();
            ImportNotesHelper.AssetsPath = EditorGUILayout.TextField("Chemin de réception", ImportNotesHelper.AssetsPath);
            if (GUILayout.Button("...", GUILayout.Width(30)))
                ImportNotesHelper.AssetsPath = EditorUtility.OpenFolderPanel("Définir le chemin de réception", ImportNotesHelper.AssetsPath, "");

            EditorGUILayout.EndHorizontal();

            if (string.IsNullOrEmpty(ImportNotesHelper.AssetsPath))
                EditorGUILayout.HelpBox("Le dossier de destination ne peut pas être vide!", MessageType.Error);

            ImportNotesHelper.Overwrite = EditorGUILayout.Toggle("Overwrite", ImportNotesHelper.Overwrite);

            ImportNotesHelper.LookForSpecificOctave = EditorGUILayout.BeginToggleGroup("Importer un octave spécifique", ImportNotesHelper.LookForSpecificOctave);
            int[] octaves = NoteFileFormat.GetOctavesList(ImportNotesHelper.UsedSystem);
            string[] displayedOptions = octaves.Select(octave => octave.ToString()).ToArray();

            // Consistency check.
            if (!octaves.Contains(ImportNotesHelper.TargetOctave))
                ImportNotesHelper.TargetOctave = 1;

            ImportNotesHelper.TargetOctave = EditorGUILayout.IntPopup("Octave cible", ImportNotesHelper.TargetOctave, displayedOptions, octaves);
            EditorGUILayout.EndToggleGroup();

            if (GUILayout.Button("Importer des fichiers audio"))
            {
                string path = EditorUtility.OpenFolderPanel("Find audio files", ImportNotesHelper.ImportPath, string.Empty);
                if (!string.IsNullOrEmpty(path))
                {
                    ImportNotesHelper.ImportPath = path;

                    Scale[] results = ImportNotesHelper.TryImportSoundsFrom();
                    if (results != null && results.Any())
                    {
                        foreach(Scale result in results)
                        {
                            if (!m_Asset.Scales.Any(scale => scale.Octave == result.Octave))
                                m_Asset.Scales.Add(result);
                            else
                            {
                                Scale existing = m_Asset.Scales.First(scale => scale.Octave == result.Octave);
                                foreach(Scale.NoteClip noteClip in existing.Notes)
                                {
                                    if (!noteClip.Clip)
                                        continue;

                                    string dialogueMessage = string.Format("L'octave {0} a déjà un clip pour la note {1}. " +
                                        "La remplacer?", existing.Octave, noteClip.Note);

                                    if (EditorUtility.DisplayDialog("Doublon présent", dialogueMessage, "Oui", "Non"))
                                    {
                                        noteClip.Clip = result.GetClip(noteClip.Note);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("No results were found!");
                    }
                }
            }
        }

        /// <summary>
        /// Set the target to load in the window.
        /// </summary>
        /// <param name="target">The asset file.</param>
        private void SetAsset(NotesList target)
        {
            // If there already is a loaded asset, save it.
            if (m_Asset)
            {
                AssetDatabase.SaveAssets();
            }

            // Assign the new target.
            m_Asset = target;
            if (m_Asset.Scales == null)
                m_Asset.Scales = new List<Scale>();
        }

        /// <summary>
        /// Get the list of octaves that have not been set for this asset.
        /// </summary>
        /// <param name="currentOctave">The currently selected octave</param>
        /// <returns>The list of available assets.</returns>
        private int[] GetAvailableOctaves(int currentOctave)
        {
            IList<int> availableOctaves = MusicalNote.OctavesFr.ToList();
            foreach(int octave in availableOctaves.ToArray())
            {
                // Always keep current octave in the list.
                if (octave == currentOctave)
                    continue;

                if (!IsOctaveAvailable(octave))
                    availableOctaves.Remove(octave);
            }

            return availableOctaves.ToArray();
        }

        /// <summary>
        /// Is an octave available for this asset ?
        /// </summary>
        /// <param name="octave">The octave</param>
        /// <returns>true if available, false otherwise.</returns>
        private bool IsOctaveAvailable(int octave)
        {
            return !m_Asset.Scales.Any(scale => scale.Octave == octave);
        }

        #endregion // Methods

    }
}


