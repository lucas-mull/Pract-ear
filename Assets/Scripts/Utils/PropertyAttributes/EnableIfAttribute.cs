using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Practear.Utils.PropertyAttributes
{
    /// <summary>
    /// Use this attribute to enable a serialized field in the inspector depending of the value of
    /// another property.
    /// </summary>
    sealed public class EnableIfAttribute : PropertyAttribute
    {

        #region Instance variables

        /// <summary>
        /// The name of the property to depend on.
        /// Has to be located within the same script.
        /// </summary>
        private string m_ConditionPropertyName;

        #endregion // Instance variables

        #region Properties

        /// <summary>
        /// <see cref="m_ConditionPropertyName"/>
        /// </summary>
        public string ConditionPropertyName { get { return m_ConditionPropertyName; } }

        /// <summary>
        /// If this is set to true, disabled fields will be hidden altogether.
        /// </summary>
        public bool HideIfDisabled { get; set; }

        #endregion // Properties

        #region Constructors

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="propertyName">The name of the conditional property.</param>
        public EnableIfAttribute(string propertyName)
        {
            m_ConditionPropertyName = propertyName;
        }

        #endregion // Constructors

    }

#if UNITY_EDITOR

    /// <summary>
    /// Property drawer for <see cref="EnableIfAttribute"/>
    /// </summary>
    [CustomPropertyDrawer(typeof(EnableIfAttribute))]
    public class EnableIfDrawer : PropertyDrawer
    {

        #region Properties

        /// <summary>
        /// Access the attribute.
        /// </summary>
        private EnableIfAttribute Target { get { return (EnableIfAttribute)attribute; } }

        #endregion // Properties

        #region Methods

        /// <summary>
        /// <see cref="PropertyDrawer.GetPropertyHeight"/>
        /// </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!GetConditionalProperty(property.serializedObject) && Target.HideIfDisabled)
            {
                return 0f;
            }
            else
            {
                return base.GetPropertyHeight(property, label);
            }
        }

        /// <summary>
        /// <see cref="PropertyDrawer.OnGUI"/>
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (GetConditionalProperty(property.serializedObject))
            {
                EditorGUI.PropertyField(position, property, label);
            }
            else if (!Target.HideIfDisabled)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.PropertyField(position, property, label);
                EditorGUI.EndDisabledGroup();
            }
        }

        /// <summary>
        /// Access the conditional serialized property
        /// </summary>
        /// <param name="obj">The serialized object holding the property.</param>
        /// <returns>The serialized property (null if not found).</returns>
        private bool GetConditionalProperty(SerializedObject obj)
        {
            SerializedProperty prop = obj.FindProperty(Target.ConditionPropertyName);
            if (prop == null)
                return false;

            switch (prop.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    return prop.boolValue;
                case SerializedPropertyType.ObjectReference:
                    return prop.objectReferenceValue != null;
                default:
                    return false;
            }
        }

        #endregion // Methods

    }

#endif

}

