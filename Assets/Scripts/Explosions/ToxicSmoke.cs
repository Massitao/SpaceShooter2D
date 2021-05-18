using System.Collections;
using UnityEngine;

public class ToxicSmoke : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private ParticleSystem toxicSmokePS;
    [SerializeField] private CircleCollider2D toxicSmokeAreaCollider;

    [SerializeField] private float smokeDuration;
    [SerializeField] private float smokeTimeToDamage;
    private Coroutine smokeDamageCoroutine;
    private Coroutine smokeDurationCoroutine;


    private void OnEnable()
    {
        StartSmoke();
    }
    private void OnDisable()
    {
        StopSmoke();
        StopSmokeDamage();
    }

    private void Start()
    {
        toxicSmokeAreaCollider.radius = toxicSmokePS.shape.radius;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Ship playerShip))
        {
            StartSmokeDamage(playerShip);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Ship playerShip))
        {
            StopSmokeDamage();
        }
    }

    private void StartSmoke()
    {
        if (smokeDurationCoroutine != null)
        {
            StopCoroutine(smokeDurationCoroutine);
        }

        smokeDurationCoroutine = StartCoroutine(SmokeDurationCoroutine());
    }
    private void StopSmoke()
    {
        if (smokeDurationCoroutine != null)
        {
            StopCoroutine(smokeDurationCoroutine);
            smokeDurationCoroutine = null;
        }
    }

    private void StartSmokeDamage(Ship playerShip)
    {
        if (smokeDamageCoroutine != null)
        {
            StopCoroutine(smokeDamageCoroutine);
        }

        smokeDamageCoroutine = StartCoroutine(SmokeDamageCoroutine(playerShip));
    }
    private void StopSmokeDamage()
    {
        if (smokeDamageCoroutine != null)
        {
            StopCoroutine(smokeDamageCoroutine);
            smokeDamageCoroutine = null;
        }
    }

    private IEnumerator SmokeDurationCoroutine()
    {
        toxicSmokePS.Play();
        toxicSmokeAreaCollider.enabled = true;

        yield return new WaitForSeconds(smokeDuration);

        toxicSmokePS.Stop();
        toxicSmokeAreaCollider.enabled = false;
        StopSmokeDamage();

        yield return new WaitUntil(() => toxicSmokePS.particleCount == 0);

        gameObject.SetActive(false);
    }
    private IEnumerator SmokeDamageCoroutine(Ship playerShip)
    {
        while (playerShip.EntityHealth > 0)
        {
            yield return new WaitForSeconds(smokeTimeToDamage);
            playerShip.TakeDamage(1);
        }

        smokeDamageCoroutine = null;
    }
}