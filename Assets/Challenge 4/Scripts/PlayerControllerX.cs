using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerX : MonoBehaviour
{
    private Rigidbody playerRb;
    private float speed = 2;
    private GameObject focalPoint;

    public ParticleSystem smoke;
    public bool turboAvailable = false; // Initialize as false
    public float turboDuration = 2.0f;
    private float turboSpeedMultiplier = 7f;
    public bool hasPowerup = false; // Initialize as false
    public GameObject powerupIndicator;
    public int powerUpDuration = 5;
    public int turboWait = 5;
    private float powerupStrength = 25; // Assuming this is used elsewhere

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
        // Initially, the power-up indicator should be deactivated
        powerupIndicator.SetActive(false);
    }

    void Update()
    {
        MoveForward();
        UpdatePowerupIndicatorPosition(); // Update the power-up indicator position in the Update method
        UpdateSmokePosition(); // Update the smoke position in the Update method

        // Check for power-up activation (e.g., when the player presses a key)
        if (Input.GetKeyDown(KeyCode.P) && !hasPowerup && turboAvailable)
        {
            Debug.Log("Power-up activation key pressed!"); // Debug statement for testing
            ActivatePowerup();
        }
    }

    private void MoveForward()
    {
        float verticalInput = Input.GetAxis("Vertical");

        if (playerRb != null)
        {
            playerRb.AddForce(focalPoint.transform.forward * verticalInput * speed * Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.Space) && turboAvailable)
            {
                playerRb.AddForce(focalPoint.transform.forward * speed * verticalInput * turboSpeedMultiplier, ForceMode.Impulse);
                StartTurboCooldown();
                smoke.Play();
            }
            else
            {
                playerRb.AddForce(focalPoint.transform.forward * speed * verticalInput);
            }
        }
    }

    private void UpdatePowerupIndicatorPosition()
    {
        // Update the position of the power-up indicator relative to the player
        powerupIndicator.transform.position = transform.position + new Vector3(0, -0.6f, 0);
    }

    private void UpdateSmokePosition()
    {
        // Update the position of the smoke particle system relative to the player
        smoke.transform.position = transform.position + new Vector3(0, -0.6f, 0);
    }

    private void ActivatePowerup()
    {
        hasPowerup = true;
        powerupIndicator.SetActive(true); // Activate the power-up indicator
        StartCoroutine(PowerupCooldown());
    }

    // Method to start the turbo cooldown
    private void StartTurboCooldown()
    {
        turboAvailable = false; // Disable turbo while it's on cooldown
        StartCoroutine(TurboCooldown());
    }

    // Coroutine to count down turbo cooldown duration
    IEnumerator TurboCooldown()
    {
        yield return new WaitForSeconds(turboDuration);
        turboAvailable = true; // Turbo is available again
    }

    // Coroutine to count down power-up duration
    IEnumerator PowerupCooldown()
    {
        yield return new WaitForSeconds(powerUpDuration);
        hasPowerup = false;
        powerupIndicator.SetActive(false); // Deactivate the power-up indicator when the power-up expires
    }

    // OnTriggerEnter is used for power-up collection
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Powerup"))
        {
            Debug.Log("Power-up collected!"); // Debug statement for testing

            // Enable turbo when the power-up is collected
            turboAvailable = true;
            powerupIndicator.SetActive(true); // Activate the power-up indicator

            // Deactivate the collected power-up object (power-up clone)
            other.gameObject.SetActive(false);

            StartCoroutine(PowerupCooldown());
        }
    }

    // OnCollisionEnter is used for collisions with enemies
    private void OnCollisionEnter(Collision other)
    {
        if (hasPowerup && other.gameObject.CompareTag("Enemy"))
        {
            // Handle collision with enemies when the power-up is active
            Rigidbody enemyRigidbody = other.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = (other.gameObject.transform.position - transform.position);
            enemyRigidbody.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
        }
    }
}
