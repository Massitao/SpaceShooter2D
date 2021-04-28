using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GalaxyShooter.Entities.Modules;

namespace GalaxyShooter.Entities.Player
{
    public class ShipMove : MoveBase
    {
        [Header("Speed")]
        [SerializeField] private float shipSpeed = 8f;

        [Header("Thrusters", order = 0)]
        [SerializeField] private float shipThrusterSpeed = 10f;

        [Header("Thrusters Fuel", order = 1)]
        [SerializeField] [Range(0f, 1f)] private float shipThrusterFuel = 1f;
        [SerializeField] [Range(0f, 1f)] private float shipThrusterMinumumFuelAfterExhaust = 1f;
        private bool usingThrusters = false;

        [Space(8)]

        [SerializeField] [Range(0f, 1f)] private float shipThrusterWasteSpeed = .2f;
        [SerializeField] [Range(0f, 1f)] private float shipThrusterRecoverySpeed = .4f;
        [SerializeField] [Range(0f, 10f)] private float shipThrusterRecoveryCooldown = .4f;



        public override void Move(Vector2 moveDir, float speed)
        {
            transform.Translate(moveDir.normalized * speed * Time.deltaTime);
        }

        private float SpeedSelection()
        {
            // If Player is using the Thrusters, apply Thruster speed. Else, keep normal ship speed
            float selectedSpeed = usingThrusters ? shipThrusterSpeed : shipSpeed;

            // If Extra Speed Power-up is active, apply extra speed. Else, keep selected Speed
            selectedSpeed *= extraSpeedActive ? extraSpeedMultiplier : 1f;

            return selectedSpeed;
        }
        private void MoveHorizontalWrap()
        {
            // Wrap ship around X axis
            if (Mathf.Abs(transform.position.x) > SpaceShooterData.WrapX)
            {
                transform.position = new Vector3(SpaceShooterData.WrapX * Mathf.Sign(transform.position.x) * -1, transform.position.y);
            }
        }
        private void MoveVerticalClamp()
        {
            // Clamp Y position so the player doesn't leave the screen
            transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, SpaceShooterData.PlayerBoundLimitsY.x, SpaceShooterData.PlayerBoundLimitsY.y), transform.position.z);
        }

        private IEnumerator ThrusterBehaviour()
        {
            bool successfulCooldown = false;

            while (true)
            {
                // If the Thrust Action input is pressed, has fuel, and the move input magnitude is higher than 0...
                if (thrustPressed && shipThrusterFuel > 0f && moveInput.magnitude > 0)
                {
                    // Drain Thruster Capacity
                    shipThrusterFuel = Mathf.Clamp(shipThrusterFuel - shipThrusterWasteSpeed * Time.deltaTime, 0f, 1f);

                    // Trigger Thruster Fuel Change Event
                    OnThrusterFuelChange?.Invoke(shipThrusterFuel);

                    // The player is using the thrusters
                    usingThrusters = true;
                }
                else
                {
                    // The ship is not using the thrusters
                    usingThrusters = false;

                    // Else, if the ship used fuel... 
                    if (shipThrusterFuel < 1f)
                    {
                        successfulCooldown = true;

                        // Wait for the recovery cooldown
                        for (float i = shipThrusterRecoveryCooldown; i >= 0f; i -= Time.deltaTime)
                        {
                            // If the player wants to interrupt the thruster cooldown to use them, abort cooldown
                            if (thrustPressed && shipThrusterFuel > 0f && moveInput.magnitude > 0)
                            {
                                successfulCooldown = false;
                                OnThrusterCooldownChange?.Invoke(0f);
                                break;
                            }

                            // Trigger Thruster Cooldown Change Event
                            OnThrusterCooldownChange?.Invoke(Mathf.InverseLerp(shipThrusterRecoveryCooldown, 0f, i));

                            yield return null;
                        }

                        // No longer on cooldown
                        OnThrusterCooldownChange?.Invoke(0f);

                        // If the cooldown was successful...
                        if (successfulCooldown)
                        {
                            // While the ship is not at full fuel capacity, and while the player is not trying to use the thrusters with a minimum amount of fuel....
                            while (shipThrusterFuel < 1f && !(thrustPressed && shipThrusterFuel >= shipThrusterMinumumFuelAfterExhaust && moveInput.magnitude > 0))
                            {
                                // Refill fuel
                                shipThrusterFuel = Mathf.Clamp(shipThrusterFuel + shipThrusterRecoverySpeed * Time.deltaTime, 0f, 1f);

                                // Trigger Thruster Fuel Change Event
                                OnThrusterFuelChange?.Invoke(shipThrusterFuel);

                                yield return null;
                            }
                        }
                    }
                }

                yield return null;
            }




            // Start is called before the first frame update
            void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}

