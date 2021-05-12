using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    #region Variables
    #region Components
    [Header("Laser Beam Components")]
    [SerializeField] private SpriteRenderer beamSprite;
    [SerializeField] private SpriteRenderer beamFakeLightSprite;
    [SerializeField] private ParticleSystem beamChargeUpPS;
    [SerializeField] private AudioSource beamAudioSource;
    #endregion

    #region Properties
    [Header("Beam Properties")]
    [SerializeField] private float beamGrowXSpeed;
    [SerializeField] private float beamGrowYSpeed;

    [SerializeField] private float beamMaxSizeX;

    [SerializeField] [Range(0.1f, 5f)] private float beamChargeUpSpeed;
    [SerializeField] [Range(0.1f, 5f)] private float beamChargeRegressSpeed;
    [SerializeField] [Range(0f, 5f)] private float beamDuration;

    private float beamChargePercentage;
    private float beamTimer;
    #endregion

    #region Beam Light
    [Header("Beam Light")]
    [SerializeField] [Range(0f, 1f)] private float beamLightActivationPercentage;

    [Space(6)]

    [SerializeField] private AnimationCurve beamLightPingPong;
    [SerializeField] private float beamPingPongSpeed;
    #endregion

    #region Beam Particles
    [Header("Beam Particles")]
    [SerializeField] [Range(0f, 1f)] private float beamParticleActivationPercentage;
    [SerializeField] [Range(0f, 1f)] private float beamParticleDeactivationPercentage;
    #endregion

    #region Collision Detection
    [Header("Beam Collision Detection")]
    [SerializeField] private LayerMask beamLM;
    private Collider2D[] beamEntitiesToDestroy = new Collider2D[10];
    private int beamEnemiesInside;
    #endregion

    #region Beam Status
    [Header("Beam Status")]
    [SerializeField] private BeamState currentBeamState;
    public enum BeamState { Inactive, ChargeUp, Firing, Regressing }

    private Coroutine beamChargeUpCoroutine;
    private Coroutine beamColliderCheckCoroutine;
    #endregion

    #region Audio
    [Header("Beam Audio")]
    [SerializeField] private AudioClip chargeUpClip;
    [SerializeField] private AudioClip beamFiringClip;
    [SerializeField] private AudioClip beamRegressingClip;
    #endregion
    #endregion


    #region MonoBehaviour Methods
    private void Awake()
    {
        beamSprite.size = Vector2.zero;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(transform.position + new Vector3(0f, beamSprite.size.y / 2, 0f), beamSprite.size);
    }
    #endregion

    #region Custom Methods
    #region Start / Stop Beam
    public void StartBeam()
    {
        if (currentBeamState == BeamState.Regressing)
        {
            StopBeam();
            beamChargeUpCoroutine = StartCoroutine(Beam());
            return;
        }

        if (beamChargeUpCoroutine == null)
        {
            beamChargeUpCoroutine = StartCoroutine(Beam());
        }
        else
        {
            SetTimer(0f);
        }
    }
    public void StopBeam()
    {
        if (beamChargePercentage < beamParticleDeactivationPercentage && beamChargeUpPS.isPlaying)
        {
            beamChargeUpPS.Stop();
        }

        SetFakeLightAlpha(0f);
        SetBeamSize(0f, 0f, 0f, 0f);
        SetTimer(0f);

        StopBeamCollisionCheck();

        currentBeamState = BeamState.Inactive;

        if (beamChargeUpCoroutine != null)
        {
            StopCoroutine(beamChargeUpCoroutine);

            beamChargeUpCoroutine = null;
        }
    }

    private void StartBeamCollisionCheck()
    {
        if (beamColliderCheckCoroutine == null)
        {
            beamColliderCheckCoroutine = StartCoroutine(BeamDestruction());
        }
    }
    private void StopBeamCollisionCheck()
    {
        if (beamColliderCheckCoroutine != null)
        {
            StopCoroutine(beamColliderCheckCoroutine);
            beamColliderCheckCoroutine = null;
        }
    }
    #endregion

    #region Laser Beam Coroutines
    private IEnumerator Beam()
    {
        #region Charge Up Phase
        ChargeUpStart();

        // Charge up beam, activating fake light and Particle System
        while (beamChargePercentage != 1f)
        {
            ChargeUp();
            yield return null;
        }

        ChargeUpComplete();
        #endregion

        #region Fire Phase
        BeamGrowStart();

        // Grow Laser Beam, both in X and Y Axis
        while (beamSprite.size.y < SpaceShooterData.LaserBoundLimitsY - transform.position.y && !Mathf.Approximately(beamSprite.size.y, SpaceShooterData.LaserBoundLimitsY - transform.position.y))
        {
            BeamGrow();
            yield return null;
        }

        // Maintain Laser Beam size until a certain duration
        while (beamTimer < beamDuration)
        {
            BeamGrowIdle();
            yield return null;
        }
        #endregion

        #region Regress Phase
        float alphaStored = beamFakeLightSprite.color.a;
        float xSize = beamSprite.size.x;
        float ySize = beamSprite.size.y;

        BeamRegressStart();

        // Charge up beam, including activating fake light and Particle System
        while (beamChargePercentage != 0f)
        {
            BeamRegress(xSize, ySize, alphaStored);
            yield return null;
        }

        BeamRegressComplete();
        #endregion
    }
    private IEnumerator BeamDestruction()
    {
        while (true)
        {
            beamEnemiesInside = Physics2D.OverlapBoxNonAlloc(transform.position + new Vector3(0f, beamSprite.size.y / 2, 0f), beamSprite.size, 0f, beamEntitiesToDestroy, beamLM);

            for (int enemyIndex = 0; enemyIndex < beamEnemiesInside; enemyIndex++)
            {
                if (beamEntitiesToDestroy[enemyIndex].TryGetComponent(out IDamageable entity) && entity.EntityHealth > 0)
                {
                    entity.TakeDamage(1);
                }

                if (beamEntitiesToDestroy[enemyIndex].TryGetComponent(out LaserBase laser))
                {
                    laser.gameObject.SetActive(false);
                }
            }
            yield return null;
        }
    }
    #endregion

    #region Laser Beam Stages
    private void ChargeUpStart()
    {
        currentBeamState = BeamState.ChargeUp;

        // Start playing Charge up sound
        SetAudioClip(chargeUpClip, false);
    }
    private void ChargeUp()
    {
        if (beamChargePercentage < beamLightActivationPercentage)
        {
            SetFakeLightAlpha(Mathf.InverseLerp(0f, beamLightActivationPercentage, beamChargePercentage));
        }
        else
        {
            SetFakeLightAlpha(beamLightPingPong.Evaluate(Mathf.PingPong(Time.time * beamPingPongSpeed, 1f)));
        }

        if (beamChargePercentage >= beamParticleActivationPercentage && !beamChargeUpPS.isPlaying)
        {
            beamChargeUpPS.Play();
        }

        AddBeamCharge();
    }
    private void ChargeUpComplete()
    {
        if (!beamChargeUpPS.isPlaying)
        {
            beamChargeUpPS.Play();
        }
    }

    private void BeamGrowStart()
    {
        currentBeamState = BeamState.Firing;

        // Start checking for Damagable Entities
        StartBeamCollisionCheck();

        // Start playing Beam Loop sound
        SetAudioClip(beamFiringClip, true);
    }
    private void BeamGrow()
    {
        SetBeamSize(beamSprite.size.x + beamGrowXSpeed * Time.deltaTime, beamSprite.size.y + beamGrowYSpeed * Time.deltaTime, beamMaxSizeX, SpaceShooterData.LaserBoundLimitsY - transform.position.y);
        SetFakeLightAlpha(beamLightPingPong.Evaluate(Mathf.PingPong(Time.time * beamPingPongSpeed, 1f)));
    }
    private void BeamGrowIdle()
    {
        SetBeamSize(beamMaxSizeX, SpaceShooterData.LaserBoundLimitsY - transform.position.y, beamMaxSizeX, SpaceShooterData.LaserBoundLimitsY - transform.position.y);
        SetFakeLightAlpha(beamLightPingPong.Evaluate(Mathf.PingPong(Time.time * beamPingPongSpeed, 1f)));
        SetTimer(Mathf.Clamp(beamTimer + Time.deltaTime, 0f, beamDuration));
    }

    private void BeamRegressStart()
    {
        currentBeamState = BeamState.Regressing;

        SetAudioClip(beamRegressingClip, false);
    }
    private void BeamRegress(float x, float y, float alpha)
    {
        if (beamChargePercentage < beamParticleDeactivationPercentage && beamChargeUpPS.isPlaying)
        {
            beamChargeUpPS.Stop();
        }

        SetFakeLightAlpha(Mathf.InverseLerp(0f, alpha, beamChargePercentage));
        SetBeamSize(Mathf.Lerp(0f, x, beamChargePercentage * 0.5f), Mathf.Lerp(0f, y, beamChargePercentage), beamMaxSizeX, y);
        SubtractBeamCharge();
    }
    private void BeamRegressComplete()
    {
        StopBeam();
    }
    #endregion

    #region Get / Set Methods
    public bool IsBeamCoroutineRunning()
    {
        return beamChargeUpCoroutine != null;
    }
    public BeamState GetBeamState()
    {
        return currentBeamState;
    }

    private void AddBeamCharge() => beamChargePercentage = Mathf.Clamp(beamChargePercentage + beamChargeUpSpeed * Time.deltaTime, 0f, 1f);
    private void SubtractBeamCharge() => beamChargePercentage = Mathf.Clamp(beamChargePercentage - beamChargeRegressSpeed * Time.deltaTime, 0f, 1f);
    private void SetTimer(float value) => beamTimer = value;

    private void SetBeamSize(float x, float y, float maxClampX, float maxClampY)
    {
        beamSprite.size = new Vector2(Mathf.Clamp(x, 0f, maxClampX), Mathf.Clamp(y, 0f, maxClampY));
    }
    private void SetFakeLightAlpha(float newAlpha)
    {
        beamFakeLightSprite.color = new Color(beamFakeLightSprite.color.r, beamFakeLightSprite.color.g, beamFakeLightSprite.color.b, newAlpha);
    }
    private void SetAudioClip(AudioClip clip, bool loop)
    {
        beamAudioSource.clip = clip;
        beamAudioSource.loop = loop;
        beamAudioSource.Play();
    }
    #endregion
    #endregion
}