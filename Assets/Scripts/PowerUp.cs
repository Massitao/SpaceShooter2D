using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator anim;

    [Header("Animator")]
    [SerializeField] private string animPowerUpType;
    private int animTypeHash => Animator.StringToHash(animPowerUpType);

    [Header("PowerUp Type")]
    [SerializeField] private Type type;
    public enum Type { TripleShot, Speed, Shield }

    [Header("PowerUp Move")]
    [SerializeField] protected float speed = 3f;
    [SerializeField] protected float despawnY = -9f;


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

    // Override Power-up type
    public void SetPowerupType(Type newType)
    {
        type = newType;
        anim.SetInteger(animTypeHash, (int)newType);
    }

    // Triggers Ship powerup ability
    private void PickUp(Ship player)
    {
        switch (type)
        {
            case Type.TripleShot:
                player.ActivateTripleShot();
                break;

            case Type.Speed:
                player.ActivateExtraSpeed();
                break;

            case Type.Shield:
                player.ActivateShield();
                break;
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
}