using System;
using System.Linq;
using JetBrains.Annotations;
using Model.MovingObjects.Ship.Projectiles;
using UnityEngine;

namespace Model.MovingObjects.Ship.ShipParts.Parts
{
    public class Turret : ShipPart, IShooting
    {
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private int fireCooldownMs = 700;
        [SerializeField] private float shootingRadius = 20;
        [SerializeField] private float maxAngleToTargetDegrees = 45;

        private Cooldown _shootingCooldown;
        private Projectile _projectile;

        [CanBeNull]
        private GameObject GetNearestEnemy()
            => GameObject
                .FindGameObjectsWithTag("Enemy")
                .Where(e =>
                {
                    var directionToEnemy = transform.position - e.transform.position;
                    var facing = transform.up * (-1);
                    var angleToEnemyCos = Vector3.Dot(directionToEnemy, facing) 
                                          / (directionToEnemy.magnitude * facing.magnitude);
                    
                    return Math.Acos(angleToEnemyCos) / Math.PI * 180 <= maxAngleToTargetDegrees;
                })
                .Where(e => (gameObject.transform.position - e.transform.position).magnitude <= shootingRadius)
                .OrderBy(e => (gameObject.transform.position - e.transform.position).magnitude)
                .FirstOrDefault();
        
        private new void Awake()
        {
            base.Awake();
            IsAlive = true;
            
            _shootingCooldown = new Cooldown(fireCooldownMs);
            
            _projectile = projectilePrefab.GetComponent<Projectile>();
            if (_projectile is null)
                throw new InvalidOperationException("Projectile must have a 'Projectile' component");
        }

        public void Shoot()
        {
            if (_shootingCooldown.CoolingDown)
                return;
            
            var enemy = GetNearestEnemy();
            if (enemy is null)
                return;

            var position = gameObject.transform.position;
            
            var directionToEnemy = (enemy.transform.position - position).normalized;
            var projectile = Instantiate(projectilePrefab);
            projectile.transform.position = position + directionToEnemy;
            var projectileRb = projectile.GetComponent<Rigidbody2D>();


            projectileRb.velocity = directionToEnemy * _projectile.BaseSpeed; 
            _shootingCooldown.Start();
        }
    }
}