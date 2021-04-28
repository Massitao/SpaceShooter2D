using System.Collections;
using UnityEngine;
using GalaxyShooter.Interfaces;

namespace GalaxyShooter.Entities.Player
{
    public class PlayerHealth : MonoBehaviour, IHealable
    {
        [Header("Components")]


        [Header("Health")]
        [SerializeField] private int entityHealth;
        public int EntityHealth
        {
            get { return entityHealth; }
            set { entityHealth = Mathf.Clamp(value, 0, EntityMaxHealth); }
        }

        [SerializeField] private int entityMaxHealth = 3;
        public int EntityMaxHealth
        {
            get { return entityMaxHealth; }
            set { entityMaxHealth = Mathf.Clamp(value, 1, int.MaxValue); }
        }


        [Header("Invincibility")]
        private WaitForSeconds invincibilityDuration = new WaitForSeconds(2f);
        private Coroutine invincibilityCoroutine;
        private bool invincible => invincibilityCoroutine != null;


        [Header("Ship Explosion")]
        [SerializeField] private GameObject explosion;


        // Events
        public event System.Action<int> OnEntityHealed;
        public event System.Action<int> OnEntityDamaged;
        public event System.Action<IDamageable> OnEntityKilled;



        private void Awake()
        {

        }

        // Start is called before the first frame update
        void Start()
        {

        }



        public void Heal()
        {
            EntityHealth = EntityMaxHealth;
            OnEntityHealed?.Invoke(EntityHealth);
        }
        public void Heal(int healAmount)
        {
            // Clamp Heal Amount (No negative values or exceeding max health)
            healAmount = Mathf.Clamp(healAmount, 0, EntityMaxHealth);

            // Clamp Health
            EntityHealth = Mathf.Clamp(EntityHealth + healAmount, 0, EntityMaxHealth);

            // Trigger On Heal Event, passing in the Ship's health
            OnEntityHealed?.Invoke(EntityHealth);
        }
        public void TakeDamage(int damageToTake)
        {
            // If the Ship is in Invincibility state, stop executing this method
            if (invincible) return;

            // Clamp Heal Amount (No negative values or exceeding max health)
            damageToTake = Mathf.Clamp(damageToTake, 0, EntityMaxHealth);


            /*
            // If shield is active, make the shield take the damage instead and stop executing this method
            if (shieldActive)
            {
                ShieldTakeDamage(damageToTake);
                return;
            }
            */

            // Take damage and clamp Health
            EntityHealth = Mathf.Clamp(EntityHealth - damageToTake, 0, EntityMaxHealth);

            if (EntityHealth == 0)
            {
                // Camera Shake
                //CameraShake.Instance?.AddStress(1f);

                // Destroy Ship
                Death();
            }
            else
            {
                // Activate Invincibility state
                ActivateInvincibility();

                // Update Engine Animators
                //leftEngineAnim.SetBool(engineAnim_Hurt, EntityHealth < 3);
                //rightEngineAnim.SetBool(engineAnim_Hurt, EntityHealth < 2);

                // Play Explosion Sound
                //shipAudioSource.PlayOneShot(explosionClip);

                // Camera Shake
                switch (EntityHealth)
                {
                    case 2:
                        //CameraShake.Instance?.AddStress(.5f);
                        break;
                    case 1:
                        //CameraShake.Instance?.AddStress(.75f);
                        break;
                }
            }

            // This event is triggered when the Ship gets damaged. Needs to pass in the Ship Health's value
            OnEntityDamaged?.Invoke(EntityHealth);
        }



        public void Death()
        {
            OnEntityKilled?.Invoke(this);

            // Instantiate Explosion and Destroy this GameObject
            Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }






        private void ActivateInvincibility()
        {
            // If the Invincibility Coroutine is running, stop it
            if (invincibilityCoroutine != null)
            {
                StopCoroutine(invincibilityCoroutine);
            }

            // Start Invincibility Coroutine
            invincibilityCoroutine = StartCoroutine(InvincibilityDuration());
        }
        private void StopInvincibility()
        {
            // If the Invincibility coroutine is running, stop it, clear the Coroutine variable and enable collisions
            if (invincibilityCoroutine != null)
            {
                StopCoroutine(invincibilityCoroutine);
                invincibilityCoroutine = null;

                //shipCollider.enabled = true;
            }
        }
        private IEnumerator InvincibilityDuration()
        {
            // Start Invincibility Animation
            //shipAnim.SetTrigger(shipAnim_InvincibilityHash);

            // Disable Collisions
            //shipCollider.enabled = false;

            yield return invincibilityDuration;

            // Enable Collisions
            //shipCollider.enabled = true;

            // Clear Coroutine variable
            invincibilityCoroutine = null;
        }



        // Update is called once per frame
        void Update()
        {

        }


    }
}