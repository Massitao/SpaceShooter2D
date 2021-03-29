using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [Header("Laser Properties")]
    [SerializeField] private float laserSpeed = 6f;
    [SerializeField] private float laserBoundY = 10f;


    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        transform.Translate(Vector3.up * laserSpeed * Time.deltaTime);

        if (transform.position.y >= laserBoundY)
        {
            Destroy(gameObject);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Enemy enemyCollided))
        {
            enemyCollided.Death();
            Destroy(gameObject);
        }
    }
}
