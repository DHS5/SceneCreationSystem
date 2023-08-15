using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dhs5.SceneCreation
{
    public class Collider_SObj : SceneObject
    {
        #region Variables
        [Header("Collider")]
        [SerializeField] new private Collider collider;
        [Tooltip("Mask deciding which layers to collide with")]
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
        protected virtual bool CollisionValid(Collision collision)
        {
            return ((1 << collision.gameObject.layer) & layerMask) != 0;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (CollisionValid(collision))
                onCollisionEnter.Trigger(collision);
        }
        private void OnCollisionStay(Collision collision)
        {
            if (CollisionValid(collision))
                onCollisionStay.Trigger(collision);
        }
        private void OnCollisionExit(Collision collision)
        {
            if (CollisionValid(collision))
                onCollisionExit.Trigger(collision);
        }
        #endregion

        #region Trigger
        protected virtual bool TriggerValid(Collider collider)
        {
            return ((1 << collider.gameObject.layer) & layerMask) != 0;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (TriggerValid(other))
                onTriggerEnter.Trigger(other);
        }
        private void OnTriggerStay(Collider other)
        {
            if (TriggerValid(other))
                onTriggerStay.Trigger(other);
        }
        private void OnTriggerExit(Collider other)
        {
            if (TriggerValid(other))
                onTriggerExit.Trigger(other);
        }
        #endregion

        #region Collider Management
        protected override void OnValidate_S()
        {
            base.OnValidate_S();

            if (collider == null)
            {
                collider = GetComponent<Collider>();
            }
            if (collider)
            {
                collider.includeLayers = layerMask;
            }
        }
        #endregion
    }
}
