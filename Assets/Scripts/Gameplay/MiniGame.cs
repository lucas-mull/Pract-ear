using UnityEngine;

namespace Practear.Gameplay
{
    /// <summary>
    /// Common interface for all our mini games.
    /// </summary>
    abstract public class MiniGame : MonoBehaviour
    {

        #region Instance variables

        /// <summary>
        /// Begin the game right away?
        /// </summary>
        [SerializeField]
        private bool m_BeginOnStart;

        /// <summary>
        /// Is the mini game paused ?
        /// </summary>
        private bool m_IsPaused;

        #endregion // Instance variables.

        #region Properties

        /// <summary>
        /// Is the game paused?
        /// </summary>
        public bool IsPaused { get { return m_IsPaused; } }

        #endregion // Properties

        #region Methods

        /// <summary>
        /// Called on start.
        /// </summary>
        protected void Start()
        {
            if (m_BeginOnStart)
                Begin();
        }

        /// <summary>
        /// Called every frame.
        /// </summary>
        protected void Update()
        {
            if (m_IsPaused)
                return;

            OnUpdate();
        }

        /// <summary>
        /// Pause the mini game.
        /// </summary>
        public void Pause()
        {
            OnPause();
            m_IsPaused = true;
        }

        /// <summary>
        /// Resume the mini game.
        /// </summary>
        public void Resume()
        {
            OnResume();
            m_IsPaused = false;
        }

        /// <summary>
        /// Pause or resumes the game depending on its current state.
        /// </summary>
        public void PauseOrResume()
        {
            if (IsPaused)
                Resume();
            else
                Pause();
        }

        /// <summary>
        /// Start the game.
        /// </summary>
        public abstract void Begin();

        /// <summary>
        /// Used this method for the flow of the game.
        /// </summary>
        protected abstract void OnUpdate();

        /// <summary>
        /// Called on pause.
        /// </summary>
        protected abstract void OnPause();

        /// <summary>
        /// Called on resume.
        /// </summary>
        protected abstract void OnResume();

        /// <summary>
        /// Restart the game.
        /// </summary>
        protected abstract void Restart();

        #endregion // Methods

    }
}
