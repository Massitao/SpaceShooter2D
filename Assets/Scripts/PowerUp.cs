using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("PowerUp Move")]
    [SerializeField] protected float powerSpeed = 3f;
    [SerializeField] protected float powerDespawnY = -9f;


    // Update is called once per frame
    protected virtual void Update()
    {
        transform.Translate(Vector3.down * powerSpeed * Time.deltaTime);

        if (transform.position.y <= powerDespawnY)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void PickUp(Ship player)
    {
        // Code goes here
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Ship player))
        {
            PickUp(player);
            Destroy(gameObject);
        }
    }
}