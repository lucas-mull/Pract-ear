using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Practear.Utils.Extensions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Practear.Utils.PropertyAttributes
{
    /// <summary>
    /// Use this property attribute on a string field to display a popup displaying all the variables within an Animator variable.
    /// </summary>
    sealed public class AnimatorVariableAttribute : PropertyAttribute
    {

        #region Instance variables

        /// <summary>
        /// The parameter type (Trigger, Int, Float or Bool). This is mandatory.
        /// </summary>
        private AnimatorControllerParameterType m_ParameterType;

        #endregion // Instance variables

        #region Properties

        /// <summary>
        /// [Optional] The variable containing the animator reference.
        /// </summary>
        public string AnimatorVariable { get; set; }

        /// <summary>
        /// The type of parameter.
        /// </summary>
        public AnimatorControllerParameterType ParameterType { get { return m_ParameterType; } }

        #endregion // Properties

        #region Constructors

        /// <summary>
        /// Simple constructor
        /// </summary>
        /// <param name="parameterType">The type of the variables to display.</param>
        public AnimatorVariableAttribute(AnimatorControllerParameterType parameterType)
        {
            m_ParameterType = parameterType;
        }

        #endregion // Constructors

    }

#if UNITY_EDITOR

    /// <summary>
    /// Custom property drawer for <see cref="AnimatorVariableAttribute"/>
    /// </summary>
    [CustomPropertyDrawer(typeof(AnimatorVariableAttribute))]
    sealed public class AnimatorVariableDrawer : PropertyDrawer
    {

        #region Instance variables

        /// <summary>
        /// Keep a reference to the animator to avoid having to look it up every time.
        /// </summary>
        private Animator m_Animator;

        #endregion // Instance variables

        #region Properties

        /// <summary>
        /// Access the parent attribute.
        /// </summary>
        private AnimatorVariableAttribute Attribute {get { return (AnimatorVariableAttribute) attribute; }}

        #endregion // Properties

        #region Methods

        /// <summary>
        /// <see cref="PropertyDrawer.OnGUI"/>
        /// </summary>
        override public void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // If the property is not a string, draw it normally and exit.
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            // Otherwise check for an animator.
            if (!m_Animator)
                m_Animator = TryGetAnimator(property.serializedObject);

            if (m_Animator)
            {
                string[] parameters = GetParameters().ToArray();

                if (!parameters.Any())
                {
                    Debug.LogWarning(string.Format("There is no parameter of type {0} in the given animator", Attribute.ParameterType));
                    EditorGUI.PropertyField(position, property, label);
                }
                else
                {
                    string selected = parameters.FirstOrDefault(paramName => paramName == property.stringValue) ?? parameters[0];
                    int index = parameters.IndexOf(selected);
                    GUIContent[] contents = parameters.Select(paramName => new GUIContent(paramName)).ToArray();
                    index = EditorGUI.Popup(position, label, index, contents);
                    property.stringValue = parameters[index];
                }
                
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }

        }

        /// <summary>
        /// Try to get the animator from the current object.
        /// </summary>
        /// <param name="obj">The serialized object.</param>
        /// <returns>The animator if it was found. Null otherwise.</returns>
        private Animator TryGetAnimator(SerializedObject obj)
        {
            Animator result = null;
            if (!string.IsNullOrEmpty(Attribute.AnimatorVariable))
            {
                SerializedProperty property = obj.FindProperty(Attribute.AnimatorVariable);
                if (property != null && property.propertyType == SerializedPropertyType.ObjectReference)
                {
                    result = property.objectReferenceValue as Animator;
                }
            }
            else
            {
                result = ((Component) obj.targetObject).GetComponent<Animator>();
            }

            return result;
        }

        /// <summary>
        /// Get the list of parameters matching the criteria.
        /// </summary>
        /// <returns>The list of parameters matching with the attribute type.</returns>
        private IEnumerable<string> GetParameters()
        {
            return m_Animator.parameters.Where(parameter => parameter.type == Attribute.ParameterType).Select(parameter => parameter.name);
        }

        #endregion // Methods

    }


#endif
}
