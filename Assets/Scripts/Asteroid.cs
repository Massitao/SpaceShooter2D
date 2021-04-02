using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [Header("Asteroid Properties")]
    [SerializeField] private float asteroidMoveSpeed = 2f;
    [SerializeField] private float asteroidRotateAnglePerSecond = 5f;

    [Header("Player Reference")]
    private Ship ship;

    [Header("Score")]
    [SerializeField] private int scoreToGive = 5;


    // Start is called before the first frame update
    private void Start()
    {
        ship = FindObjectOfType<Ship>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Rotate();
    }
    private void Move()
    {
        transform.Translate(Vector3.down * asteroidMoveSpeed * Time.deltaTime, Space.World);
    }
    private void Rotate()
    {
        transform.Rotate(Vector3.forward * asteroidRotateAnglePerSecond * Time.deltaTime);
    }

    public void Explode()
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
