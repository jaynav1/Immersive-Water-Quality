using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ClickToMove : MonoBehaviour
{
    public NavMeshAgent agent; // Reference to the NavMeshAgent component
    public float rotationSpeed = 5f; // Speed at which the character rotates
    private bool isRotating = false; // Flag to check if the character is currently rotating
    private Vector3 targetPosition; // Target position to move to
    private Animator animator; // Reference to the Animator component

    void Start()
    {
        // Get the Animator component attached to the GameObject
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the left mouse button is clicked
        if (Input.GetMouseButtonDown(0))
        {
            // Create a ray from the camera to the mouse position
            Ray movePosition = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Check if the ray hits any collider
            if (Physics.Raycast(movePosition, out var hitInfo))
            {
                // Log the hit point and set the target position
                Debug.Log("Hit: " + hitInfo.point);
                targetPosition = hitInfo.point;

                // Start the coroutine to rotate and move the character
                StartCoroutine(RotateAndMove());
            }
            else
            {
                // Log if no hit was detected
                Debug.Log("No hit");
            }
        }

        // Update the Animator with the agent's velocity
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    // Coroutine to rotate the character towards the first waypoint in the path and then move
    private IEnumerator RotateAndMove()
    {
        // Set the destination of the NavMeshAgent to the target position
        agent.SetDestination(targetPosition);

        // Wait until the path is calculated
        yield return new WaitUntil(() => agent.pathPending == false);

        // Get the first waypoint in the path
        if (agent.path.corners.Length > 1)
        {
            Vector3 firstWaypoint = agent.path.corners[1];

            // Set the rotating flag to true and calculate the direction to the first waypoint
            isRotating = true;
            Vector3 direction = (firstWaypoint - transform.position).normalized;

            // Calculate the rotation needed to look at the first waypoint
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

            // Rotate the character until it faces the first waypoint
            while (Quaternion.Angle(transform.rotation, lookRotation) > 0.1f)
            {
                // Smoothly rotate the character
                transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
                yield return null;
            }

            // Set the rotating flag to false
            isRotating = false;
        }

        // The agent will now move towards the target position
    }
}
