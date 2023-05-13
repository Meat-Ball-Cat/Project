namespace Model.HealthSystem
{
    public interface IHasHealth
    {
        public float BaseHp { get; }
        public float CurrentHp { get; }
        public bool IsAlive { get; }

        public void EnsureAlive();
        public void TakeDamage(Damage damage);
    }
}