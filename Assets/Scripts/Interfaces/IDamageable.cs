public interface IDamageable
{
    // Tracks the current health status of the entity
    int EntityHealth { get; set; }

    // Sets a Max Health for the Entity
    int EntityMaxHealth { get; set; }


    // Use this event to alert listeners when this Entity gets Damaged / Healed
    event System.Action<int> OnEntityHealthChange;

    // Use this event to alert listeners when this Entity dies
    event System.Action<IDamageable> OnEntityKilled;


    // Deals damage to this entity
    void TakeDamage(int damageToTake);

    // Kills entity
    void Death();
}

public interface IHealable : IDamageable
{
    void Heal(int amountToHeal);
}