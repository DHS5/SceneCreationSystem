using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dhs5.SceneCreation
{
    public abstract class SceneProfile
    {
        protected SceneVariablesSO sceneVariablesSO;

        protected SceneObject sceneObject;

        #region Overridable Functions
        public virtual void SetUp(SceneVariablesSO _sceneVariablesSO)
        {
            sceneVariablesSO = _sceneVariablesSO;

            // Set Up Scene Events and Listeners
        }
        public virtual void Attach(SceneObject _sceneObject)
        {
            sceneObject = _sceneObject;

            // Update Belongings
        }
        #endregion

        #region Abstract Functions
        /// <summary>
        /// Function where all the <see cref="List{T}"/> of <see cref="SceneEvent"/> should be registered with <see cref="Register(List{SceneEvent})"/>
        /// </summary>
        public abstract void RegisterSceneEventsLists();
        #endregion

        #region Listener Functions
        /// <summary>
        /// Triggers the SceneEvents contains by <see cref="sceneObject"/>
        /// </summary>
        /// <param name="eventID"></param>
        public void TriggerSObj(string eventID)
        {
            sceneObject.TriggerSceneEvent(eventID);
        }
        public void TriggerSObj_Random(string filter)
        {
            sceneObject.TriggerRandom(filter);
        }
        public void TriggerSObj_RandomAndRemove(string filter)
        {
            sceneObject.TriggerRandomAndRemove(filter);
        }
        #endregion

        #region Scene Events Handling
        private List<string> eventsID = new();
        private List<List<SceneEvent>> sceneEventsList = new(); // Problem : T

        public bool ExistIn(string eventID)
        {
            return eventsID.Contains(eventID);
        }
        protected void Register(List<SceneEvent> sceneEvents)
        {
            sceneEventsList.Add(sceneEvents);
            foreach (var s in sceneEvents)
                if (!string.IsNullOrWhiteSpace(s.eventID))
                    eventsID.Add(s.eventID);
        }

        /// <summary>
        /// Triggers all the <see cref="List{T}"/> of <see cref="SceneEvent"/> of this profile
        /// </summary>
        public virtual void TriggerProfile()
        {
            foreach (var s in sceneEventsList)
                s.Trigger();
        }
        #endregion
    }
}
