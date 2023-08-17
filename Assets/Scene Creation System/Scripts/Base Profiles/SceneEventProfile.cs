using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dhs5.SceneCreation
{
    public class SceneEventProfile : SceneProfile
    {
        public List<SceneEvent> sceneEvents;

        #region Overrides
        public override void SetUp(SceneVariablesSO _sceneVariablesSO)
        {
            base.SetUp(_sceneVariablesSO);

            sceneEvents.SetUp(sceneVariablesSO);
        }
        public override void Attach(SceneObject _sceneObject)
        {
            base.Attach(_sceneObject);

            sceneEvents.BelongTo(sceneObject);
        }
        public override void Detach()
        {
            base.Detach();

            sceneEvents.BelongTo(null);
        }
        #endregion

        #region Scene Events
        public override void RegisterSceneEventsLists()
        {
            Register(sceneEvents);
        }
        #endregion
    }
}
