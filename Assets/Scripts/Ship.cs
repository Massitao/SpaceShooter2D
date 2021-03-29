using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    [Header("Inputs")]
    private Vector2 moveInput;

    [Header("Ship Move")]
    [SerializeField] private float shipSpeed = 3f;
    [SerializeField] private Vector2 moveBoundsX;
    [SerializeField] private Vector2 moveBoundsY;

    [Header("Ship Shoot")]
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Vector3 laserSpawnOffset;
    [SerializeField] private float fireRate = .5f;
    private float fireRateTimer;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Shoot();
    }

    void Move()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        transform.Translate(moveInput * shipSpeed * Time.deltaTime);

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, moveBoundsX.x, moveBoundsX.y), Mathf.Clamp(transform.position.y, moveBoundsY.x, moveBoundsY.y), transform.position.z);
    }

    void Shoot()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (Time.time >= fireRateTimer)
            {
                Instantiate(laserPrefab, transform.position + laserSpawnOffset, Quaternion.identity);
                fireRateTimer = Time.time + fireRate;
            }
        }
    }
}
