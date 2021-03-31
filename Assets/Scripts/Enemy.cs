using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Move")]
    [SerializeField] private float enemySpeed = 4f;
    [SerializeField] private Vector2 enemyBoundsX;
    [SerializeField] private Vector2 enemyBoundsY;

    [Header("Player Reference")]
    private Ship ship;

    [Header("Score")]
    [SerializeField] private int scoreToGive;


    private void Start()
    {
        ship = FindObjectOfType<Ship>();
    }
    // Update is called once per frame
    private void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.Translate(Vector3.down * enemySpeed * Time.deltaTime);

        if (transform.position.y <= enemyBoundsY.x)
        {
            transform.position = new Vector3(Random.Range(enemyBoundsX.x, enemyBoundsX.y), enemyBoundsY.y, transform.position.z);
        }
    }
    public void Death()
    {
        ship.AddScore(scoreToGive);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Ship player))
        {
            player.Damage(1);
            Death();
        }
    }
}