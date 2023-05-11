using System;
using System.Collections;
using System.Collections.Generic;
using Model.Levels;
using UnityEngine;
using UnityEngine.Serialization;

namespace Model.MovingObjects
{
    public abstract class MovingObject : MonoBehaviour, IManagedObject
    {
        protected new Rigidbody2D Rigidbody;
        protected Vector2 CurrentMoveForce;
        protected float CurrentTurnForce;
        protected LevelManager LevelManager;

        public const float OneLevelChangeTimeSeconds = 0.4f;
        
        public int CurrentLevel { get; private set; }
        
        protected bool ControlsLocked { get; private set;  } 
    
        [FormerlySerializedAs("_movementSpeed")] [SerializeField]
        protected float movementSpeed;

        [FormerlySerializedAs("_turningSpeed")] [SerializeField]
        protected float turningSpeed;


        protected void Awake()
        {
            if (!gameObject.TryGetComponent(out Rigidbody))
                Rigidbody = gameObject.AddComponent<Rigidbody2D>();
            this.LevelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        }


        protected void FixedUpdate()
        {
            Rigidbody.AddRelativeForce(CurrentMoveForce);
            Rigidbody.AddTorque(CurrentTurnForce, ForceMode2D.Force);
        }

        public void Move(Vector2 direction)
        {
            if (ControlsLocked)
                return;
            CurrentMoveForce = movementSpeed * direction;
        }

        public void Turn(float axis)
        {
            if (ControlsLocked)
                return;
            CurrentTurnForce = turningSpeed * axis;
        }

        public void DescendOneLevel()
        {
            if (this.LevelManager.TryGetLevelDepth(CurrentLevel + 1, out var depth))
            {
                StartCoroutine(ChangeDepth(depth, OneLevelChangeTimeSeconds));
                CurrentLevel++;
                ControlsLocked = true;
            }
        }
        
        public void AscendOneLevel()
        {
            if (this.LevelManager.TryGetLevelDepth(CurrentLevel - 1, out var depth))
            {
                StartCoroutine(ChangeDepth(depth, OneLevelChangeTimeSeconds));
                CurrentLevel--;
                ControlsLocked = true;
            }
        }

        private IEnumerator ChangeDepth(float targetDepth, float timeSeconds)
        {
            var startPos = this.transform.position;
            var endPos = new Vector3(startPos.x, startPos.y, targetDepth);
            for (float t = 0; t < 1; t += Time.deltaTime / timeSeconds)
            {
                transform.position = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }

            ControlsLocked = false;
        }
    }
}
