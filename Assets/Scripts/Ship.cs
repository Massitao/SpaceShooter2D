using System.Collections;
using UnityEngine;

public class Ship : MonoBehaviour
{
    [Header("Inputs")]
    private Vector2 moveInput;

    [Header("Ship Health")]
    [SerializeField] private int shipHealth = 3;

    [Header("Ship Move")]
    [SerializeField] private float shipSpeed = 3f;
    [SerializeField] private Vector2 moveBoundsX;
    [SerializeField] private Vector2 moveBoundsY;

    [Header("Ship Shoot")]
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Vector3 laserSpawnOffset;
    [SerializeField] private float fireRate = .5f;
    private float fireRateTimer;

    [Header("Triple Shoot")]
    [SerializeField] private GameObject tripleLaserPrefab;
    private bool tripleLaserActive => tripleShotCoroutine != null;
    private WaitForSeconds tripleShotDuration = new WaitForSeconds(3f);
    private Coroutine tripleShotCoroutine;



    // Start is called before the first frame update
    private void Start()
    {
        transform.position = new Vector3(0f, 0f, 0f);
    }

    // Update is called once per frame
    private void Update()
    {
        Move();
        Shoot();
    }

    private void Move()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        transform.Translate(moveInput * shipSpeed * Time.deltaTime);

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, moveBoundsX.x, moveBoundsX.y), Mathf.Clamp(transform.position.y, moveBoundsY.x, moveBoundsY.y), transform.position.z);
    }
    private void Shoot()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (Time.time >= fireRateTimer)
            {
                if (tripleLaserActive)
                {
                    Instantiate(tripleLaserPrefab, transform.position + laserSpawnOffset, Quaternion.identity);
                }
                else
                {
                    Instantiate(laserPrefab, transform.position + laserSpawnOffset, Quaternion.identity);
                }

                fireRateTimer = Time.time + fireRate;
            }
        }
    }
    public void Damage(int damage)
    {
        shipHealth -= damage;

        if (shipHealth <= 0)
        {
            Death();
        }
    }
    private void Death()
    {
        SpawnManager.Instance.StopAllSpawns();
        StopTripleShot();
        Destroy(gameObject);
    }

    public void ActivateTripleShot()
    {
        if (tripleShotCoroutine != null)
        {
            StopCoroutine(tripleShotCoroutine);
        }

        tripleShotCoroutine = StartCoroutine(TripleShotDuration());
    }
    private void StopTripleShot()
    {
        if (tripleShotCoroutine != null)
        {
            StopCoroutine(tripleShotCoroutine);
            tripleShotCoroutine = null;
        }
    }

    private IEnumerator TripleShotDuration()
    {
        yield return tripleShotDuration;
        tripleShotCoroutine = null;
    }
}
