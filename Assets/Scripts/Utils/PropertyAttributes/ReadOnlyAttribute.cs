using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Practear.Utils.PropertyAttributes
{
    /// <summary>
    /// Make a field read only in the inspector (disabled).
    /// </summary>
    sealed public class ReadOnlyAttribute : PropertyAttribute
    {        
    }

#if UNITY_EDITOR

    /// <summary>
    /// Property drawer for <see cref="ReadOnlyDrawer"/>
    /// </summary>
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    sealed public class ReadOnlyDrawer : PropertyDrawer
    {
        #region Methods

        /// <summary>
        /// <see cref="PropertyDrawer.OnGUI"/>
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Simply draw the field within a disabled group.
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(position, property, label);
            EditorGUI.EndDisabledGroup();
        }

        #endregion // Methods

    }

#endif

}


