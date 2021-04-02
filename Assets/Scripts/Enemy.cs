using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator enemyAnim;
    [SerializeField] private Collider2D enemyCol;

    [Header("Animator")]
    [SerializeField] private string enemyAnim_DeathTrigger;
    private int enemyAnim_DeathTriggerHash => Animator.StringToHash(enemyAnim_DeathTrigger);

    [Header("Enemy Properties")]
    [SerializeField] private float enemySpeed = 4f;
    [SerializeField] private Vector2 enemyBoundsX;
    [SerializeField] private Vector2 enemyBoundsY;
    private bool isExploding;

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
    public void Explode()
    {
        if (!isExploding)
        {
            isExploding = true;
            enemySpeed = 2;
            enemyCol.enabled = false;
            enemyAnim.SetTrigger(enemyAnim_DeathTriggerHash);
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
            Explode();
        }
    }
}