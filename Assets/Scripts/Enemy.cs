using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Move")]
    [SerializeField] private float enemySpeed = 4f;
    [SerializeField] private Vector2 enemyBoundsX;
    [SerializeField] private Vector2 enemyBoundsY;


    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        transform.Translate(Vector3.down * enemySpeed * Time.deltaTime);

        if (transform.position.y <= enemyBoundsY.x)
        {
            transform.position = new Vector3(Random.Range(enemyBoundsX.x, enemyBoundsX.y), enemyBoundsY.y, transform.position.z);
        }
    }

    public void Death()
    {
        Debug.Log($"{gameObject.name} has died!");
        Destroy(gameObject);
    }
}
