using UnityEngine;

public class PowerUp : MonoBehaviour
{
    #region Variables
    [Header("Components")]
    [SerializeField] private Animator anim;

    [Header("Animator")]
    [SerializeField] private string animPowerUpType;
    private int animTypeHash => Animator.StringToHash(animPowerUpType);

    [Header("PowerUp Type")]
    [SerializeField] private Type type;
    public enum Type { TripleShot, Speed, Shield, Ammo }

    [Header("PowerUp Move")]
    [SerializeField] protected float speed = 3f;
    [SerializeField] protected float despawnY = -9f;

    [Header("Audio")]
    [SerializeField] private AudioClip powerUpClip;
    #endregion


    #region MonoBehaviour Methods
    // Update is called once per frame
    private void Update()
    {
        // Move Downwards
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        // Destroy if it reaches the Despawn Point
        if (transform.position.y <= despawnY)
        {
            Destroy(gameObject);
        }
    }

    // Collision with Ship will trigger a ship ability, destroying this GameObject in the process
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Ship player))
        {
            PickUp(player);
            Destroy(gameObject);
        }
    }
    #endregion

    #region Custom Methods
    // Override Power-up type
    public void SetPowerupType(Type newType)
    {
        type = newType;
        anim.SetInteger(animTypeHash, (int)newType);
    }

    // Triggers Ship powerup ability
    private void PickUp(Ship player)
    {
        player.ActivatePowerUp(type);
        AudioManager.Instance?.PlayOneShotClip(powerUpClip);
    }
    #endregion
}