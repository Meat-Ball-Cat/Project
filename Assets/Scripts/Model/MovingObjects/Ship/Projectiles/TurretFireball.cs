using Model.HealthSystem;

namespace Model.MovingObjects.Ship.Projectiles
{
    public class TurretFireball : Projectile, IDealsDamage
    {
        public override Damage Damage
            => new((DamageType.Explosion, 30));
    }
}