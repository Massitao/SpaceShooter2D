using System.Collections;
using UnityEngine;

public abstract class EnemyShooterBase : EnemyBase
{
    protected abstract void Shoot();
}

[RequireComponent(typeof(Collider2D))]
public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    #region Variables
    [Header("Enemy Health")]
    [SerializeField] protected int entityMaxHealth;
    public int EntityMaxHealth
    {
        get { return entityMaxHealth; }
        set
        {
            entityMaxHealth = Mathf.Clamp(value, 1, int.MaxValue);
            EntityHealth = EntityHealth;
        }
    }

    protected int entityHealth;
    public int EntityHealth
    {
        get { return entityHealth; }
        set
        {
            entityHealth = Mathf.Clamp(value, 0, EntityMaxHealth);
            OnEntityHealthChange?.Invoke(entityHealth);

            if (entityHealth == 0)
            {
                Death();
            }
        }
    }


    [Header("Is Enemy Visible")]
    protected bool isVisible = false;


    [Header("Score")]
    [SerializeField] protected int scoreToGive = 0;
    #endregion

    #region Events
    // Events
    public event System.Action<int> OnEntityHealthChange;
    public event System.Action<IDamageable> OnEntityKilled;
    #endregion


    #region MonoBehaviour Methods
    protected virtual void OnEnable()
    {
        // Setting Entity Health to Max Health
        EntityHealth = EntityMaxHealth;
    }


    protected void OnBecameVisible()
    {
        isVisible = true;
        EnemyVisible();
    }
    protected void OnBecameInvisible()
    {
        isVisible = false;
        EnemyInvisible();
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyCollide(collision);
    }
    #endregion


    #region Custom Methods
    // OnBecameVisible
    protected abstract void EnemyVisible();

    // OnBecameInvisible
    protected abstract void EnemyInvisible();

    // OnTriggerEnter2D
    protected abstract void EnemyCollide(Collider2D collision);


    protected abstract void Move();


    // Some enemies don't die instantly. Remember to add a call to this method
    protected virtual void AddScore()
    {
        GameManager.Instance?.AddScore(scoreToGive);
    }


    // IDamageable - Behaviour to run when the Enemy takes damage
    public virtual void TakeDamage(int damageToTake)
    {
        // Passed in value gets converted to a positive value and clamps the value so it doesn't exceed the Max Health
        damageToTake = Mathf.Clamp(Mathf.Abs(damageToTake), 0, EntityMaxHealth);

        // Subtract health
        EntityHealth -= damageToTake;
    }

    // IDamageable - Behaviour to run when the Enemy has no health left
    public virtual void Death()
    {
        // Triggers Death Event.
        OnEntityKilled?.Invoke(this);
    }


    // Disables the GameObject
    public virtual void DisableEnemy()
    {
        gameObject.SetActive(false);
    }

    // Disables the GameObject with a Delay
    public virtual void DisableEnemy(float delay)
    {
        StartCoroutine(DisableEnemyCoroutine(delay));
    }
    protected IEnumerator DisableEnemyCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        DisableEnemy();
    }
    #endregion
}