using Dhs5.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Dhs5.SceneCreation
{
    public class Collider_SObj : SceneObject
    {
        #region Variables
        [Header("Collider")]
        [Header("Collides with :")]
        public LayerMask layerMask;

        [Header("Collision")]
        public List<SceneEvent<Collision>> onCollisionEnter;
        [Space(15f)]
        public List<SceneEvent<Collision>> onCollisionStay;
        [Space(15f)]
        public List<SceneEvent<Collision>> onCollisionExit;

        [Header("Trigger")]
        public List<SceneEvent<Collider>> onTriggerEnter;
        [Space(15f)]
        public List<SceneEvent<Collider>> onTriggerStay;
        [Space(15f)]
        public List<SceneEvent<Collider>> onTriggerExit;
        #endregion

        #region SceneObject Extension
        protected override void UpdateSceneVariables()
        {
            base.UpdateSceneVariables();

            onCollisionEnter.SetUp(sceneVariablesSO);
            onCollisionStay.SetUp(sceneVariablesSO);
            onCollisionExit.SetUp(sceneVariablesSO);
            onTriggerEnter.SetUp(sceneVariablesSO);
            onTriggerStay.SetUp(sceneVariablesSO);
            onTriggerExit.SetUp(sceneVariablesSO);
        }
        protected override void UpdateBelongings()
        {
            base.UpdateBelongings();

            onCollisionEnter.BelongTo(this);
            onCollisionStay.BelongTo(this);
            onCollisionExit.BelongTo(this);
            onTriggerEnter.BelongTo(this);
            onTriggerStay.BelongTo(this);
            onTriggerExit.BelongTo(this);
        }
        #endregion

        #region Collision
        private void OnCollisionEnter(Collision collision)
        {
            if (((1 << collision.gameObject.layer) & layerMask) != 0)
                onCollisionEnter.Trigger(collision);
        }
        private void OnCollisionStay(Collision collision)
        {
            if (((1 << collision.gameObject.layer) & layerMask) != 0)
                onCollisionStay.Trigger(collision);
        }
        private void OnCollisionExit(Collision collision)
        {
            if (((1 << collision.gameObject.layer) & layerMask) != 0)
                onCollisionExit.Trigger(collision);
        }
        #endregion

        #region Trigger
        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & layerMask) != 0)
                onTriggerEnter.Trigger(other);
        }
        private void OnTriggerStay(Collider other)
        {
            if (((1 << other.gameObject.layer) & layerMask) != 0)
                onTriggerStay.Trigger(other);
        }
        private void OnTriggerExit(Collider other)
        {
            if (((1 << other.gameObject.layer) & layerMask) != 0)
                onTriggerExit.Trigger(other);
        }
        #endregion
    }
}
