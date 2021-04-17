using UnityEngine;

public class CameraShake : MonoSingleton<CameraShake>
{
    [Header("Translation")]
    [SerializeField] private Vector3 maxTranslationShake = Vector3.one;
    [SerializeField] private float maxTranslationStress;

    [SerializeField] private float translationFrequency;


    [Header("Rotation")]
    [SerializeField] private Vector3 maxRotationShake = Vector3.one;
    [SerializeField] private float maxRotationStress;

    [SerializeField] private float rotationFrequency;


    [Header("Trauma")]
    [SerializeField] [Range(0f, 1f)] private float trauma;
    [SerializeField] private AnimationCurve traumaSmoother;
    [SerializeField] private float traumaRecoverySpeed;



    // Update is called once per frame
    private void Update()
    {
        // Smoothed Shake, shake won't end abruptly using this
        float shake = traumaSmoother.Evaluate(trauma);

        // Translation shake. Perlin Noise is used to further smooth the displacement
        transform.localPosition = new Vector3(
            maxTranslationShake.x * maxTranslationStress * (Mathf.PerlinNoise(0, Time.time * translationFrequency) * 2 - 1),
            maxTranslationShake.x * maxTranslationStress * (Mathf.PerlinNoise(1, Time.time * translationFrequency) * 2 - 1),
            maxTranslationShake.x * maxTranslationStress * (Mathf.PerlinNoise(2, Time.time * translationFrequency) * 2 - 1)
            ) * shake;

        // Rotation shake.
        transform.localRotation = Quaternion.Euler(new Vector3(
            maxRotationShake.x * maxRotationStress * (Mathf.PerlinNoise(3, Time.time * maxRotationStress) * 2 - 1),
            maxRotationShake.x * maxRotationStress * (Mathf.PerlinNoise(4, Time.time * maxRotationStress) * 2 - 1),
            maxRotationShake.x * maxRotationStress * (Mathf.PerlinNoise(5, Time.time * maxRotationStress) * 2 - 1)
            ) * shake);


        // Decreases trauma
        trauma = Mathf.Clamp01(trauma - traumaRecoverySpeed * Time.deltaTime);
    }

    public void AddStress(float stressToAdd)
    {
        trauma = Mathf.Clamp01(trauma + stressToAdd);
    }
    public void SetTrauma(float traumaToSet)
    {
        trauma = Mathf.Clamp01(traumaToSet);
    }
}
