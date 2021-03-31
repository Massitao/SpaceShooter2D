using UnityEngine;

public class Laser : MonoBehaviour
{
    [Header("Laser Properties")]
    [SerializeField] private float laserSpeed = 6f;
    [SerializeField] private float laserBoundY = 10f;

    // Update is called once per frame
    private void Update()
    {
        Move();
    }
    private void Move()
    {
        transform.Translate(Vector3.up * laserSpeed * Time.deltaTime);

        if (transform.position.y >= laserBoundY)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            Destroy(gameObject);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        {
            if (collision.gameObject.TryGetComponent(out Ship playerCollided))
            {
                playerCollided.Damage(1);
                Destroy(gameObject);
            }

            if (collision.gameObject.TryGetComponent(out Enemy enemyCollided))
            {
                enemyCollided.Death();
                Destroy(gameObject);
            }
        }

    }
}