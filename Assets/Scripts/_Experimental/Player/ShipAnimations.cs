using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyShooter.Entities.Player
{
    public class ShipAnimations : MonoBehaviour
    {
        [Header("Animators")]
        [SerializeField] private Animator shipAnim;
        [SerializeField] private Animator leftEngineAnim;
        [SerializeField] private Animator rightEngineAnim;
        [SerializeField] private Animator shieldAnim;

        [Header("Components")]
        [SerializeField] private PlayerHealth playerHealth;

        [Header("Animator References")]
        // SHIP
        [SerializeField] private string shipAnim_Input;
        private int shipAnim_InputHash;

        [SerializeField] private string shipAnim_Invincibility;
        private int shipAnim_InvincibilityHash;

        // ENGINES
        [SerializeField] private string engineAnim_Hurt;
        private int engineAnim_HurtHash;

        // SHIELD
        [SerializeField] private string shieldAnim_Health;
        private int shieldAnim_HealthHash;


        private void Awake()
        {
            SetAnimatorHashes();
        }
        private void SetAnimatorHashes()
        {
            shipAnim_InputHash = Animator.StringToHash(shipAnim_Input);
            shipAnim_InvincibilityHash = Animator.StringToHash(shipAnim_Invincibility);
            engineAnim_HurtHash = Animator.StringToHash(engineAnim_Hurt);
            shieldAnim_HealthHash = Animator.StringToHash(shieldAnim_Health);
        }

        private void OnEnable()
        {
            playerHealth.OnEntityDamaged += UpdateEngineAnimators;
            playerHealth.OnEntityHealed += UpdateEngineAnimators;
        }

        private void OnDisable()
        {
            playerHealth.OnEntityDamaged -= UpdateEngineAnimators;
            playerHealth.OnEntityHealed -= UpdateEngineAnimators;
        }

        private void UpdateEngineAnimators(int entityHealth)
        {
            leftEngineAnim.SetBool(engineAnim_Hurt, entityHealth < 3);
            rightEngineAnim.SetBool(engineAnim_Hurt, entityHealth < 2);
        }
    }
}