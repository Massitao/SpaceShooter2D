using System.Collections;
using UnityEngine;

public class Ship : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Collider2D shipCollider;

    [Header("Inputs")]
    private Vector2 moveInput;

    [Header("Ship Health")]
    [SerializeField] private int shipHealth = 3;

    [Header("Ship Move")]
    [SerializeField] private float shipSpeed = 3f;
    [SerializeField] private float wrapMoveBoundsX;
    [SerializeField] private Vector2 moveBoundsY;

    [Header("Ship Shoot")]
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Vector3 laserSpawnOffset;
    [SerializeField] private float fireRate = .5f;
    private float fireRateTimer;

    [Header("Abilities")]
    private WaitForSeconds abilityDuration = new WaitForSeconds(3f);

    [Header("Triple Shoot")]
    [SerializeField] private GameObject tripleLaserPrefab;
    private bool tripleLaserActive => tripleShotCoroutine != null;
    private Coroutine tripleShotCoroutine;

    [Header("Extra Speed")]
    [SerializeField] [Range(1f, 3f)] private float extraSpeedMultiplier;
    private bool extraSpeedActive => extraSpeedCoroutine != null;
    private Coroutine extraSpeedCoroutine;

    [Header("Shield")]
    [SerializeField] private Animator shieldAnim;
    [SerializeField] private string shieldAnim_Active;
    private int shieldAnimActiveHash => Animator.StringToHash(shieldAnim_Active);
    private bool shieldActive => shieldCoroutine != null;
    private Coroutine shieldCoroutine;



    // Start is called before the first frame update
    private void Start()
    {
        transform.position = new Vector3(0f, 0f, 0f);
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateInput();
        Move();
        Shoot();
    }

    private void UpdateInput()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void Move()
    {
        // If Extra Speed Power-up is active, apply extra speed. Else, keep normal ship speed
        float speedToApply = extraSpeedActive ? shipSpeed * extraSpeedMultiplier : shipSpeed;
        transform.Translate(moveInput * speedToApply * Time.deltaTime);

        // Wrap ship around X axis
        if (Mathf.Abs(transform.position.x) > wrapMoveBoundsX)
        {
            transform.position = new Vector3(wrapMoveBoundsX * Mathf.Sign(transform.position.x) * -1, transform.position.y);
        }

        // Clamp Y position so the player doesn't leave the screen
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, moveBoundsY.x, moveBoundsY.y), transform.position.z);
    }
    private void Shoot()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (Time.time >= fireRateTimer)
            {
                GameObject laserType = tripleLaserActive ? tripleLaserPrefab : laserPrefab;
                Collider2D[] lasersToIgnore = Instantiate(laserType, transform.position + laserSpawnOffset, Quaternion.identity).GetComponentsInChildren<Collider2D>();

                for (int i = 0; i < lasersToIgnore.Length; i++)
                {
                    Physics2D.IgnoreCollision(shipCollider, lasersToIgnore[i], true);
                }

                fireRateTimer = Time.time + fireRate;
            }
        }
    }
    public void Damage(int damage)
    {
        if (shieldActive)
        {
            StopShield();
            return;
        }

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
        StopExtraSpeed();
        StopShield();
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
        yield return abilityDuration;
        tripleShotCoroutine = null;
    }

    public void ActivateExtraSpeed()
    {
        if (extraSpeedCoroutine != null)
        {
            StopCoroutine(extraSpeedCoroutine);
        }

        extraSpeedCoroutine = StartCoroutine(ExtraSpeedDuration());
    }
    private void StopExtraSpeed()
    {
        if (extraSpeedCoroutine != null)
        {
            StopCoroutine(extraSpeedCoroutine);
            extraSpeedCoroutine = null;
        }
    }
    private IEnumerator ExtraSpeedDuration()
    {
        yield return abilityDuration;
        extraSpeedCoroutine = null;
    }

    public void ActivateShield()
    {
        if (shieldCoroutine != null)
        {
            StopCoroutine(shieldCoroutine);
        }

        shieldCoroutine = StartCoroutine(ShieldDuration());
    }
    private void StopShield()
    {
        if (shieldCoroutine != null)
        {
            StopCoroutine(shieldCoroutine);
            shieldAnim.SetBool(shieldAnimActiveHash, false);
            shieldCoroutine = null;
        }
    }
    private IEnumerator ShieldDuration()
    {
        shieldAnim.SetBool(shieldAnimActiveHash, true);
    
        yield return abilityDuration;

        shieldAnim.SetBool(shieldAnimActiveHash, false);

        shieldCoroutine = null;
    }
}