using UnityEngine;
using UnityEngine.EventSystems;

namespace Practear.UI
{
    /// <summary>
    /// Define an area from which the user can slide a UI in.
    /// </summary>
    public class SlideInUIArea : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {

        #region Internal enums

        /// <summary>
        /// Enum used to describe the available slide actions.
        /// </summary>
        private enum SlideAction
        {
            None,
            SlideUp,
            SlideDown
        }

        /// <summary>
        /// Enum used to describe a slide motion direction
        /// </summary>
        private enum SlideDirection
        {
            None,
            Up,
            Down
        }

        #endregion // Internal enums

        #region Instance variables

        /// <summary>
        /// The reference to the UI.
        /// </summary>
        [Tooltip("The reference to the UI.")]
        [SerializeField]
        private Canvas m_UI;

        /// <summary>
        /// The slide up / down speed.
        /// </summary>
        [Tooltip("The slide up / down speed.")]
        [SerializeField]
        private float m_SlideSpeed = 1f;

        /// <summary>
        /// The rect transform attached to this component.
        /// </summary>
        private RectTransform m_Transform;

        /// <summary>
        /// The initial anchored position of the UI.
        /// </summary>
        private float m_UIInitialOffset;

        /// <summary>
        /// The parent rect transform (the one that is being dragged).
        /// </summary>
        private RectTransform m_Parent;

        /// <summary>
        /// The drag point on the previous call to <see cref="OnDrag(PointerEventData)"/>
        /// </summary>
        private Vector2 m_PreviousDragPoint;

        /// <summary>
        /// The action to perform in <see cref="Update"/>
        /// </summary>
        private SlideAction m_ActionToPerform = SlideAction.None;

        /// <summary>
        /// The last swipe direction performed by the user.
        /// </summary>
        private SlideDirection m_LastDirection = SlideDirection.None;        

        #endregion // Instance variables

        #region Methods

        /// <summary>
        /// Called on load
        /// </summary>
        private void Awake()
        {
            m_Transform = GetComponent<RectTransform>();
            m_Parent = m_Transform.parent.GetComponent<RectTransform>();
            m_UIInitialOffset = m_Parent.offsetMax.y;
        }

        /// <summary>
        /// Called every frame.
        /// </summary>
        private void Update()
        {
            if (m_ActionToPerform == SlideAction.None)
                return;

            // The Y value to reach by sliding up or down.
            float destination = 0f, step = 0f, newYOffset = 0f;
            float currentYOffset = m_Parent.offsetMax.y;
            switch (m_ActionToPerform)
            {
                case SlideAction.SlideUp:    
                    destination = m_UIInitialOffset;
                    step = Mathf.Lerp(0f, destination, Time.deltaTime * m_SlideSpeed);
                    newYOffset = currentYOffset + step;
                    break;
                case SlideAction.SlideDown:
                    destination = 0f;
                    step = m_UIInitialOffset - Mathf.Lerp(m_UIInitialOffset, destination, Time.deltaTime * m_SlideSpeed);
                    newYOffset = currentYOffset - step;
                    break;                    
            }
            
            if (newYOffset > m_UIInitialOffset || newYOffset < 0)

            {
                m_ActionToPerform = SlideAction.None;
                newYOffset = destination;
            }

            m_Parent.offsetMax = new Vector2(m_Parent.offsetMax.x, newYOffset);
            m_Parent.offsetMin = new Vector2(m_Parent.offsetMax.x, newYOffset);
        }

        /// <summary>
        /// <see cref="IBeginDragHandler.OnBeginDrag(PointerEventData)"/>
        /// </summary>
        public void OnBeginDrag(PointerEventData eventData)
        {
            m_PreviousDragPoint = ScreenPointToLocalCanvasPoint(eventData.position);
        }

        /// <summary>
        /// <see cref="IDragHandler.OnDrag(PointerEventData)"/>
        /// </summary>
        public void OnDrag(PointerEventData eventData)
        {
            Vector2 dragPointInCanvas = ScreenPointToLocalCanvasPoint(eventData.position);
            float yDifference = dragPointInCanvas.y - m_PreviousDragPoint.y;
            Vector2 step = new Vector2(0, Mathf.Abs(yDifference) * 4);

            // Swiping down
            if (yDifference < 0)
            {
                m_Parent.offsetMax -= step;   
                m_Parent.offsetMin -= step;

                if (m_Parent.offsetMax.y < 0)
                    m_Parent.offsetMax = new Vector2(m_Parent.offsetMax.x, 0);

                if (m_Parent.offsetMin.y < 0)
                    m_Parent.offsetMin = new Vector2(m_Parent.offsetMin.x, 0);

                m_LastDirection = SlideDirection.Down;
            }
            // Swiping up.
            else if (yDifference > 0)
            {
                m_Parent.offsetMax += step;
                m_Parent.offsetMin += step;

                if (m_Parent.offsetMax.y > m_UIInitialOffset)
                    m_Parent.offsetMax = new Vector2(m_Parent.offsetMax.x, m_UIInitialOffset);

                if (m_Parent.offsetMin.y > m_UIInitialOffset)
                    m_Parent.offsetMin = new Vector2(m_Parent.offsetMax.x, m_UIInitialOffset);

                m_LastDirection = SlideDirection.Up;
            }

            m_PreviousDragPoint = dragPointInCanvas;
        }

        /// <summary>
        /// <see cref="IEndDragHandler.OnEndDrag(PointerEventData)"/>
        /// </summary>
        public void OnEndDrag(PointerEventData eventData)
        {
            // Define the action to perform.
            switch(m_LastDirection)
            {
                case SlideDirection.Down:
                    SlideDown();
                    break;
                case SlideDirection.Up:
                    SlideUp();
                    break;
            }

            m_LastDirection = SlideDirection.None;
        }

        /// <summary>
        /// Perform a slide down.
        /// </summary>
        public void SlideDown()
        {
            PerformAction(SlideAction.SlideDown);
        }

        /// <summary>
        /// Perform a slide up.
        /// </summary>
        public void SlideUp()
        {
            PerformAction(SlideAction.SlideUp);
        }

        /// <summary>
        /// Perform a given action
        /// </summary>
        /// <param name="action">The action to perform.</param>
        private void PerformAction(SlideAction action)
        {
            m_ActionToPerform = action;
        }

        /// <summary>
        /// Convert a point from screen space to canvas space.
        /// </summary>
        /// <param name="screenPoint">The coordinates on the screen.</param>
        /// <returns>The coordinates on the canvas.</returns>
        private Vector2 ScreenPointToLocalCanvasPoint(Vector2 screenPoint)
        {
            Vector3 screenPosition = screenPoint;
            screenPosition.z = m_UI.planeDistance;
            Camera renderCamera = Camera.main;

            return renderCamera.ScreenToWorldPoint(screenPosition);
        }        

        #endregion // Methods

    }
}

