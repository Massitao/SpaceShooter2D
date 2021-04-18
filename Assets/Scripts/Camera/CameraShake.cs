using UnityEngine;

public class CameraShake : MonoSingleton<CameraShake>
{
    #region Variables
    [Header("Translation")]
    [SerializeField] private Vector3 maxTranslationShake = Vector3.one;
    [SerializeField] private float maxTranslationStress = 0.3f;
    [SerializeField] private float translationFrequency = 25f;


    [Header("Rotation")]
    [SerializeField] private Vector3 maxRotationShake = Vector3.one;
    [SerializeField] private float maxRotationStress = 0.3f;
    [SerializeField] private float rotationFrequency = 10f;


    [Header("Trauma")]
    [SerializeField] [Range(0f, 1f)] private float trauma = 0f;
    [SerializeField] private float traumaRecoverySpeed = 0.2f;
    [SerializeField] private AnimationCurve traumaSmoother;


    [Header("Shake Coroutine")]
    private Coroutine shakeCoroutine;
    #endregion

    #region Custom Methods
    /// <summary>
    /// Adds shake.
    /// </summary>
    /// <param name="stressToAdd"></param>
    public void AddStress(float stressToAdd)
    {
        // Add stress to trauma, and clamp it between 0...1
        trauma = Mathf.Clamp01(trauma + stressToAdd);

        // If the Shake Coroutine is not running, and trauma is higher than 0, start the coroutine
        if (shakeCoroutine == null && trauma > 0f)
        {
            shakeCoroutine = StartCoroutine(Shake());
        }
    }

    /// <summary>
    /// Direct value set to Trauma.
    /// </summary>
    /// <param name="traumaToSet"></param>
    public void SetTrauma(float traumaToSet)
    {
        // Add stress to trauma, and clamp it between 0...1
        trauma = Mathf.Clamp01(traumaToSet);

        // If trauma is equal to 0...
        if (trauma == 0)
        {
            // ...and the shake coroutine is running...
            if (shakeCoroutine != null)
            {
                // Stop the coroutine and clear the Coroutine variable
                StopCoroutine(shakeCoroutine);
                shakeCoroutine = null;
            }
        }
        // If trauma is higher than 0...
        else
        {
            // ...and the shake coroutine is not running...
            if (shakeCoroutine == null)
            {
                // Start the coroutine
                shakeCoroutine = StartCoroutine(Shake());
            }
        }
    }

    /// <summary>
    /// Starts Shaking the Camera Container.
    /// </summary>
    /// <returns></returns>
    private System.Collections.IEnumerator Shake()
    {
        // Quantity of displacement
        float shake = 0f;

        while (trauma != 0)
        {
            // Smoothed Shake, changing the trauma value to one from the AnimationCurve
            shake = traumaSmoother.Evaluate(trauma);

            /// To create a shake, the Camera Container must be moved using the following values
            /// maxTranslationalShake or maxRotationShake defines a Vector which the camera will not overshoot
            /// maxTranslationalStress or maxRotationStress is a constant multiplier that increases the shake amount by modifying the maxTranslationalShake or maxRotationShake
            /// Mathf.PerlinNoise is used for the random displacement. The values are always presented in wave forms, giving always smoothed values in contrast to Random.Range.
            /// For PerlinNoise, the X parameter uses a random value to randomize it, and the Y value uses Time.time to get the Y value over time. It is multiplied by frequency to increase the wave speed.
            /// And finally everything is multiplied by 2 and then, subtracted by 1. This will convert the values into a -1...1 range (PerlinNoise returns between 0...1)

            // Translation shake
            transform.localPosition = new Vector3(
                maxTranslationShake.x * maxTranslationStress * (Mathf.PerlinNoise(0, Time.time * translationFrequency) * 2 - 1),
                maxTranslationShake.x * maxTranslationStress * (Mathf.PerlinNoise(1, Time.time * translationFrequency) * 2 - 1),
                maxTranslationShake.x * maxTranslationStress * (Mathf.PerlinNoise(2, Time.time * translationFrequency) * 2 - 1)
                ) * shake;

            // Rotation shake
            transform.localRotation = Quaternion.Euler(new Vector3(
                maxRotationShake.x * maxRotationStress * (Mathf.PerlinNoise(3, Time.time * rotationFrequency) * 2 - 1),
                maxRotationShake.x * maxRotationStress * (Mathf.PerlinNoise(4, Time.time * rotationFrequency) * 2 - 1),
                maxRotationShake.x * maxRotationStress * (Mathf.PerlinNoise(5, Time.time * rotationFrequency) * 2 - 1)
                ) * shake);


            // Decreases trauma over time
            trauma = Mathf.Clamp01(trauma - traumaRecoverySpeed * Time.deltaTime);

            // Wait a frame
            yield return null;
        }

        // Reset Camera Container Position
        transform.localPosition = Vector3.zero;

        // Clear Coroutine, so it can be started again
        shakeCoroutine = null;
    }
    #endregion
}
