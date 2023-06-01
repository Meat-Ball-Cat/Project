using System;
using System.Collections;
using Assets.Scripts.Model.Levels;
using UnityEngine;
using UnityEngine.Serialization;

namespace Model.MovingObjects
{
    public abstract class MovingObject : MonoBehaviour, IManagedObject
    {
        protected Rigidbody2D Rigidbody;
        protected Vector2 CurrentMoveForce;
        protected float CurrentTurnForce;

        [SerializeField] private float oneLevelChangeTimeSeconds = 0.4f;
        public event EventHandler ChangeLayer;
        
        protected bool DepthChangeLocked { get; private set; } 
    
        [FormerlySerializedAs("_movementSpeed")] [SerializeField]
        protected float movementSpeed = 1;

        [FormerlySerializedAs("_turningSpeed")] [SerializeField]
        protected float turningSpeed = 1.4f;


        protected void Awake()
        {
            if (!gameObject.TryGetComponent(out Rigidbody))
                Rigidbody = gameObject.AddComponent<Rigidbody2D>();
        }

        protected void FixedUpdate()
        {
            Rigidbody.AddRelativeForce(CurrentMoveForce);
            Rigidbody.AddTorque(CurrentTurnForce, ForceMode2D.Force);
        }

        public void Move(Vector2 direction)
        {
            CurrentMoveForce = movementSpeed * direction.normalized;
        }

        public void Turn(float axis)
        {
            CurrentTurnForce = turningSpeed * axis;
        }

        public void Descend()
        {
            LayerManager.Instance.ChangeObjectLayer(this, gameObject.layer + 1);
            ChangeLayer?.Invoke(this, EventArgs.Empty);
        }

        public void Ascend()
        {
            LayerManager.Instance.ChangeObjectLayer(this, gameObject.layer - 1);
            ChangeLayer?.Invoke(this, EventArgs.Empty);
        }

        public void ChangeDepth()
        {
            if (DepthChangeLocked)
                return;

            StartCoroutine(
                ChangeDepth(LayerManager.Instance.GetLayerDepth(gameObject.layer), oneLevelChangeTimeSeconds));
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
