using System;
using System.Collections;
using System.Collections.Generic;
using Model.Levels;
using UnityEngine;
using UnityEngine.Serialization;

namespace Model.MovingObjects
{
    public abstract class MovingObject : Layered, IManagedObject
    {
        protected new Rigidbody2D Rigidbody;
        protected Vector2 CurrentMoveForce;
        protected float CurrentTurnForce;

        public const float OneLevelChangeTimeSeconds = 0.4f;
        
        protected bool DepthChangeLocked { get; private set;  } 
    
        [FormerlySerializedAs("_movementSpeed")] [SerializeField]
        protected float movementSpeed;

        [FormerlySerializedAs("_turningSpeed")] [SerializeField]
        protected float turningSpeed;


        protected void Awake()
        {
            if (!gameObject.TryGetComponent(out Rigidbody))
                Rigidbody = gameObject.AddComponent<Rigidbody2D>();
            LayerManager.Instance.AddObject(this, 0);
        }

        protected void FixedUpdate()
        {
            Rigidbody.AddRelativeForce(CurrentMoveForce);
            Rigidbody.AddTorque(CurrentTurnForce, ForceMode2D.Force);
        }

        public void Move(Vector2 direction)
        {
            CurrentMoveForce = movementSpeed * direction;
        }

        public void Turn(float axis)
        {
            CurrentTurnForce = turningSpeed * axis;
        }

        public void Descend()
        {
            if (DepthChangeLocked)
                return;
            
            DescendOneLayer();
            StartCoroutine(
                ChangeDepth(CurrentTargetDepth, OneLevelChangeTimeSeconds));
        }

        public void Ascend()
        {
            if (DepthChangeLocked)
                return;
            
            AscendOneLayer();
            StartCoroutine(
                ChangeDepth(CurrentTargetDepth, OneLevelChangeTimeSeconds));
        }
        
        private IEnumerator ChangeDepth(float targetDepth, float timeSeconds)
        {
            DepthChangeLocked = true;
            var startPos = this.transform.position;
            var endPos = new Vector3(startPos.x, startPos.y, targetDepth);
            for (float t = 0; t < 1; t += Time.deltaTime / timeSeconds)
            {
                transform.position = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }

            transform.position = endPos;

            DepthChangeLocked = false;
        }
    }
}
