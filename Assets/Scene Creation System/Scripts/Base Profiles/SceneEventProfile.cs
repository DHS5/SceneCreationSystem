using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dhs5.SceneCreation
{
    public class SceneEventProfile : SceneProfile
    {
        public List<SceneEvent<SceneEventParam>> sceneEvents;

        #region Overrides
        public override void SetUp(SceneVariablesSO _sceneVariablesSO)
        {
            base.SetUp(_sceneVariablesSO);

            sceneEvents.SetUp(sceneVariablesSO);
        }

        public override void Attach(SceneObject _sceneObject)
        {
            base.Attach(_sceneObject);

            _sceneObject.OverrideEvents(this, sceneEvents);
        }

        public override bool CanOverrideListeners => false;
        public override bool CanOverrideEvents => true;
        #endregion

        #region Scene Events
        protected override void RegisterSceneEventsLists()
        {
            Register(sceneEvents, false);
        }
        protected override void RegisterTweens()
        {
            // No Tweens
        }
        #endregion
    }
}
