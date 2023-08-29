using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dhs5.SceneCreation
{
    [Serializable]
    public abstract class SceneProfile : SceneState.ISceneVarSetupable, SceneState.ISceneObjectBelongable, SceneState.IInitializable
    {
        protected SceneVariablesSO sceneVariablesSO;

        protected SceneObject sceneObject;

        #region SceneObject Override Permissions
        public abstract bool CanOverrideListeners { get; }
        public abstract bool CanOverrideEvents { get; }
        #endregion

        #region Interfaces
        public void Init()
        {
            RegisterSceneEventsLists();
            RegisterTweens();

            InitSceneEventsLists();
        }
        public virtual void SetUp(SceneVariablesSO _sceneVariablesSO)
        {
            sceneVariablesSO = _sceneVariablesSO;

            // Set Up Scene Events, Tweens and Listeners
        }
        public virtual void BelongTo(SceneObject _sceneObject)
        {
            UpdateSceneEventsBelongings(_sceneObject);
            UpdateTweensBelongings(_sceneObject);
        }
        #endregion

        #region Overridable Functions
        public virtual void Attach(SceneObject _sceneObject)
        {
            sceneObject = _sceneObject;

            BelongTo(_sceneObject);
        }
        public virtual void Detach()
        {
            sceneObject = null;

            BelongTo(null);

            UnregisterSceneEvents();
            UnregisterTweens();
        }
        #endregion

        #region Abstract Functions
        /// <summary>
        /// Function where all the <see cref="List{T}"/> of <see cref="BaseSceneEvent"/> should be registered with <see cref="Register{T}(List{T})"/>
        /// </summary>
        protected abstract void RegisterSceneEventsLists();
        /// <summary>
        /// Function where all the <see cref="SceneVarTween"/> should be registered with <see cref="Register(SceneVarTween)"/>
        /// </summary>
        protected abstract void RegisterTweens();
        #endregion

        #region Scene Events Management
        protected List<string> eventsID = new();
        protected List<List<BaseSceneEvent>> sceneEventsList = new();

        protected bool ExistIn(string eventID)
        {
            if (eventsID == null || eventsID.Count <= 0) return false;

            return eventsID.Contains(eventID);
        }

        #region Registration
        protected void Register<T>(List<T> sceneEvents, bool registerEventIDs = true) where T : BaseSceneEvent
        {
            sceneEventsList.Add(sceneEvents.Cast<BaseSceneEvent>().ToList());
            if (registerEventIDs)
                foreach (var s in sceneEvents)
                    if (!string.IsNullOrWhiteSpace(s.eventID))
                        eventsID.Add(s.eventID);
        }
        protected void UnregisterSceneEvents()
        {
            eventsID?.Clear();
            sceneEventsList?.Clear();
        }
        #endregion
        private void InitSceneEventsLists()
        {
            if (sceneEventsList == null || sceneEventsList.Count <= 0) return;

            foreach (var s in sceneEventsList)
            {
                s.Init();
            }
        }
        private void UpdateSceneEventsBelongings(SceneObject _sceneObject)
        {
            if (sceneEventsList == null || sceneEventsList.Count <= 0) return;

            foreach (var s in sceneEventsList)
            {
                s.BelongTo(_sceneObject);
            }
        }
        #endregion

        #region Scene Events Triggering
        /// <summary>
        /// Triggers all the <see cref="List{T}"/> of <see cref="BaseSceneEvent"/> of this profile
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
        public virtual void TriggerEventInProfileAndRemove(string eventID, int triggerNumber)
        {
            if (ExistIn(eventID))
            {
                foreach (var l in sceneEventsList)
                {
                    l.TriggerAndRemove(eventID, triggerNumber);
                }
            }
        }
        public virtual void TriggerAllEventsInProfileAndRemove(int triggerNumber)
        {
            foreach (var l in sceneEventsList)
            {
                l.TriggerAndRemoveAll(triggerNumber);
            }
        }
        public virtual bool TriggerProfileRandom(string filter = null, bool remove = false)
        {
            if (sceneEventsList == null || sceneEventsList.Count <= 0) return false;

            return sceneEventsList[Random.Range(0, sceneEventsList.Count)].TriggerRandom(filter, remove);
        }
        #endregion

        #region Tweens Management
        protected List<SceneVarTween> tweensList = new();

        protected void Register(SceneVarTween tween)
        {
            tweensList.Add(tween);
        }
        protected void UnregisterTweens()
        {
            tweensList?.Clear();
        }
        private void UpdateTweensBelongings(SceneObject _sceneObject)
        {
            if (tweensList == null || tweensList.Count <= 0) return;

            foreach (var t in tweensList)
            {
                t.BelongTo(_sceneObject);
            }
        }
        #endregion
    }
}
