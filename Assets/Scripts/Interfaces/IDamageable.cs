public interface IDamageable
{
    int EntityHealth { get; set; }
    System.Action<int> OnEntityDamaged { get; set; }
    System.Action<IDamageable> OnEntityKilled { get; set; }

    void TakeDamage(int damageToTake);
    void Death();
}