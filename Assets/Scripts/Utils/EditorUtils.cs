#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

static public class EditorUtils
{

    #region Static methods

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

    #endregion // Static methods

}

#endif