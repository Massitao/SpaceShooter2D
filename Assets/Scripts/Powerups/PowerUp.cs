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
    public enum Type { TripleShot, HeatSeek, Beam, Speed, Shield, Life, Ammo, Virus }

    [Header("PowerUp Move")]
    [SerializeField] protected float speed = 3f;
    [SerializeField] protected float virusSpeed = 6f;
    [SerializeField] protected float attractedSpeed = 7f;

    public static bool attractedByPlayer = false;

    [Header("Audio")]
    [SerializeField] private AudioClip powerUpClip;


    [Header("Debug")]
    [SerializeField] private bool initialize;
    #endregion


    #region MonoBehaviour Methods
    private void Start()
    {
        if (initialize)
        {
            SetPowerupType(type);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (attractedByPlayer)
        {
            if (GameManager.Instance != null && GameManager.Instance.playerShip != null)
            {
                Vector3 attraction = (GameManager.Instance.playerShip.transform.position - transform.position).normalized;

                // Move Towards player
                transform.Translate(attraction * attractedSpeed * Time.deltaTime);
            }
            else
            {
                attractedByPlayer = false;
            }
        }
        else
        {
            if (GameManager.Instance != null && GameManager.Instance.playerShip != null && type == Type.Virus)
            {
                Vector3 virusFollow = Vector3.down;
                virusFollow += (GameManager.Instance.playerShip.transform.position.x <= transform.position.x ? Vector3.left : Vector3.right).normalized;

                // Move Towards player
                transform.Translate(virusFollow * virusSpeed * Time.deltaTime);
            }
            else
            {
                // Move Downwards
                transform.Translate(Vector3.down * speed * Time.deltaTime);
            }
        }


        // Destroy if it reaches the Despawn Point
        if (transform.position.y <= -SpaceShooterData.EnemyBoundLimitsY)
        {
            gameObject.SetActive(false);
        }
    }

    // Collision with Ship will trigger a ship ability, destroying this GameObject in the process
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Ship player))
        {
            PickUp(player);
            gameObject.SetActive(false);
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

    public void Explode()
    {
        ObjectPool.Instance?.GetPooledObject(ObjectPool.PoolType.Explosion, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }
    #endregion
}