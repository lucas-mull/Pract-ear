#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System;
using Object = UnityEngine.Object;

namespace Practear.Utils
{
    static public class EditorUtils
    {

        #region Static methods

        /// <summary>
        /// Draw the default "script" field that shows in the inspector for every monobehaviour.
        /// </summary>
        /// <typeparam name="T">The type of the script</typeparam>
        /// <param name="target">The target</param>
        static public void DrawDefaultScriptField<T>(Object target) where T : MonoBehaviour
        {
            MonoScript script = MonoScript.FromMonoBehaviour((T)target);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
            EditorGUI.EndDisabledGroup();
        }

        /// <summary>
        /// Draw a header field (bold label).
        /// </summary>
        /// <param name="label">The label content</param>
        static public void DrawHeader(string label)
        {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        }

        /// <summary>
        /// Draw an error message using a red label field.
        /// </summary>
        /// <param name="message">The message to display</param>
        static public void DrawErrorMessage(string message)
        {
            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            style.normal.textColor = Color.red;

            EditorGUILayout.LabelField(message, style);
        }

        /// <summary>
        /// Draw property field(s)
        /// </summary>
        /// <param name="serializedObject">The serialized object</param>
        /// <param name="propertyName">The property's name</param>
        /// <param name="additionalProperties">Additional properties names</param>
        static public void DrawPropertyFields(SerializedObject serializedObject, string propertyName, params string[] additionalProperties)
        {
            SerializedProperty initial = serializedObject.FindProperty(propertyName);
            if (initial == null)
                throw new ArgumentException(string.Format("Couldn't find property '{0}'.", propertyName));

            EditorGUILayout.PropertyField(initial, true);

            foreach (string additionalProperty in additionalProperties)
            {
                SerializedProperty additional = serializedObject.FindProperty(additionalProperty);
                if (additional == null)
                    throw new ArgumentException(string.Format("Couldn't find property '{0}'.", additionalProperty));

                EditorGUILayout.PropertyField(additional, true);
            }
        }

        /// <summary>
        /// Draw a image field using the standard object reference field rather than the sprite field.
        /// </summary>
        /// <param name="label">The label to display</param>
        /// <param name="sourceImage">The sprite.</param>
        static public Sprite DrawNormalSpriteField(string label, Sprite sourceImage, bool allowSceneObjects = false)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            sourceImage = (Sprite)EditorGUILayout.ObjectField(sourceImage, typeof(Sprite), allowSceneObjects);
            EditorGUILayout.EndHorizontal();

            return sourceImage;
        }

        #endregion // Static methods

    }
}

#endif