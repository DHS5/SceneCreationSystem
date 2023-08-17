using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dhs5.SceneCreation
{
    [Serializable]
    public abstract class SceneProfile : SceneState.ISceneVarSetupable
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

            RegisterSceneEventsLists();
            InitSceneEventsLists();

            // Update Belongings
        }
        public virtual void Detach()
        {
            sceneObject = null;

            UnregisterSceneEvents();

            // Update Belongings
        }
        #endregion

        #region Abstract Functions
        /// <summary>
        /// Function where all the <see cref="List{T}"/> of <see cref="SceneEvent"/> should be registered with <see cref="Register(List{SceneEvent})"/>
        /// </summary>
        public abstract void RegisterSceneEventsLists();
        #endregion

        #region Scene Events Handling
        protected List<string> eventsID = new();
        protected List<List<SceneEvent>> sceneEventsList = new(); // Problem : T

        protected bool ExistIn(string eventID)
        {
            if (eventsID == null || eventsID.Count <= 0) return false;

            return eventsID.Contains(eventID);
        }
        protected void Register(List<SceneEvent> sceneEvents)
        {
            sceneEventsList.Add(sceneEvents);
            foreach (var s in sceneEvents)
                if (!string.IsNullOrWhiteSpace(s.eventID))
                    eventsID.Add(s.eventID);
        }
        protected void UnregisterSceneEvents()
        {
            eventsID = new();
            sceneEventsList = new();
        }
        private void InitSceneEventsLists()
        {
            if (sceneEventsList == null || sceneEventsList.Count <= 0) return;

            foreach (var s in sceneEventsList)
            {
                s.Init();
            }
        }

        /// <summary>
        /// Triggers all the <see cref="List{T}"/> of <see cref="SceneEvent"/> of this profile
        /// </summary>
        public virtual void TriggerProfile()
        {
            if (sceneEventsList == null || sceneEventsList.Count <= 0) return;

            foreach (var s in sceneEventsList)
                s.Trigger();
        }
        public virtual void TriggerEventInProfile(string eventID)
        {
            if (ExistIn(eventID))
            {
                foreach (var l in sceneEventsList)
                {
                    l.Trigger(eventID);
                }
            }
        }
        public virtual bool TriggerProfileRandom(string filter = null, bool remove = false)
        {
            if (sceneEventsList == null || sceneEventsList.Count <= 0) return false;

            return sceneEventsList[Random.Range(0, sceneEventsList.Count)].TriggerRandom(filter, remove);
        }
        #endregion
    }
}
