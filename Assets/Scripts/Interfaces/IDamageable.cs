public interface IDamageable
{
    int EntityHealth { get; set; }
    event System.Action<int> OnEntityDamaged;
    event System.Action<IDamageable> OnEntityKilled;

    void TakeDamage(int damageToTake);
    void Death();
}