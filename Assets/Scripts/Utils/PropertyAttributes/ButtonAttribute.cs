using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Practear.Utils.PropertyAttributes
{
    /// <summary>
    /// Property attribute used to draw a button over or below a field in the inspector.
    /// </summary>
    public class ButtonAttribute : PropertyAttribute
    {

        #region Internal enums

        /// <summary>
        /// Enum for the button placement relative to the field.
        /// </summary>
        public enum EButtonPlacement
        {            
            Below,
            Over,
            Inline
        }

        #endregion // Internal enums.

        #region Instance variables

        /// <summary>
        /// Name of the method to execute when clicking the button.
        /// </summary>
        private string m_MethodName;

        /// <summary>
        /// The method parameters.
        /// </summary>
        private object[] m_Arguments;

        #endregion // Instance variables

        #region Properties

        /// <summary>
        /// The name of the method to execute.
        /// </summary>
        public string MethodName { get { return m_MethodName; } }

        /// <summary>
        /// Access the list of parameters for the method.
        /// </summary>
        public IEnumerable<object> Parameters { get { return m_Arguments; } }

        /// <summary>
        /// The button's placement
        /// </summary>
        public EButtonPlacement Placement { get; set; }

        /// <summary>
        /// The button label.
        /// </summary>
        public string Label { get; set; }

        #endregion // Properties

        #region Constructors

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="methodName">The name of the method to execute.</param>
        public ButtonAttribute(string methodName, params object[] args)
        {
            m_MethodName = methodName;
            if (m_MethodName == null)
                m_MethodName = string.Empty;

            m_Arguments = args;
        }

        #endregion // Constructors

    }

#if UNITY_EDITOR

    /// <summary>
    /// Property drawer for <see cref="ButtonAttribute"/>
    /// </summary>
    [CustomPropertyDrawer(typeof(ButtonAttribute))]
    public class ButtonDrawer : PropertyDrawer
    {

        #region Constants

        /// <summary>
        /// The size for an inline button.
        /// </summary>
        private const float InlineButtonSize = 100;

        /// <summary>
        /// The inline button margin value.
        /// </summary>
        private const float InlineButtonMargin = 5;

        #endregion // Constants

        #region Properties

        /// <summary>
        /// Access the parent <see cref="ButtonAttribute"/>
        /// </summary>
        private ButtonAttribute Attribute { get { return (ButtonAttribute)attribute; } }

        #endregion // Properties

        #region Methods

        /// <summary>
        /// <see cref="PropertyDrawer.GetPropertyHeight(SerializedProperty, GUIContent)"/>
        /// </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Attribute.Placement == ButtonAttribute.EButtonPlacement.Inline 
                ? base.GetPropertyHeight(property, label)
                : base.GetPropertyHeight(property, label) * 2;
        }

        /// <summary>
        /// <see cref="PropertyDrawer.OnGUI(Rect, SerializedProperty, GUIContent)"/>
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect buttonPosition;
            if (Attribute.Placement == ButtonAttribute.EButtonPlacement.Inline)
            {
                position.width -= InlineButtonSize;
                buttonPosition = position;
                buttonPosition.width = InlineButtonSize - InlineButtonMargin;
                buttonPosition.x = position.x + position.width + InlineButtonMargin;
            }
            else
            {
                position.height /= 2;
                buttonPosition = position;

                if (Attribute.Placement == ButtonAttribute.EButtonPlacement.Below)
                {
                    buttonPosition.y += position.height;
                }
                else
                {
                    position.y += position.height;
                }
            }

            string buttonLabel = Attribute.Label;
            if (string.IsNullOrEmpty(buttonLabel))
                buttonLabel = "Button";

            if (GUI.Button(buttonPosition, buttonLabel))
            {
                ExecuteMethod(property.serializedObject);
            }

            EditorGUI.PropertyField(position, property, label);
        }

        /// <summary>
        /// Execute the method designated by the attribute (if it could be found).
        /// </summary>
        private void ExecuteMethod(SerializedObject serializedObject)
        {
            Type type = serializedObject.targetObject.GetType();

            MethodInfo methodInfo = type.GetMethod(Attribute.MethodName);
            if (methodInfo == null)
                return;

            if (!Attribute.Parameters.Any())
            {
                methodInfo.Invoke(serializedObject.targetObject, null);
            }
            else
            {
                methodInfo.Invoke(serializedObject.targetObject, Attribute.Parameters.ToArray());
            }
        }

        #endregion // Methods

    }

#endif

}

