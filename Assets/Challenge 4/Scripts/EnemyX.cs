using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyX : MonoBehaviour
{
    public float speed;
    private Rigidbody enemyRb;
    public GameObject playerGoal; // Assign the player goal GameObject through the Unity Inspector

    // Start is called before the first frame update
    void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
        playerGoal = GameObject.Find("Player Goal");

        // Check if playerGoal is not assigned and log an error if that's the case
       // if (playerGoal == null)
        //{
          //  Debug.LogError("Player Goal is not assigned. Please assign it in the Inspector.");
        //}
    }

    // Update is called once per frame
    void Update()
    {
        // Check if playerGoal is not assigned or if it doesn't have a Rigidbody
        if (playerGoal == null || !playerGoal.TryGetComponent<Rigidbody>(out Rigidbody playerRb))
        {
            return;
        }

        // Set enemy direction towards player goal and move there
        Vector3 lookDirection = (playerGoal.transform.position - transform.position).normalized;
        playerRb.AddForce(lookDirection * speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision other)
    {
        // If enemy collides with either goal, destroy it
        if (other.gameObject.name == "Enemy Goal")
        {
            Destroy(gameObject);
        } 
        else if (other.gameObject.name == "Player Goal")
        {
            Destroy(gameObject);
        }
    }
}
