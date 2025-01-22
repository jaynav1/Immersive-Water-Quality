using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ClickToMove : MonoBehaviour
{
    public NavMeshAgent agent; // Reference to the NavMeshAgent component
    public float rotationSpeed; // Speed at which the character rotates
    public float maxSpeed; // Maximum speed of the agent
    public float minSpeed; // Minimum speed of the agent when not facing forward
    public float flockRadius = 1.0f; // Radius for flocking behavior
    public float minDistance = 7.0f; // Minimum distance to maintain from the clicked point
    public bool useFlocking = false; // Flag to toggle between flocking and herding behavior
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
            Debug.Log("Mouse button pressed");
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
        animator.SetFloat("Speed", agent.velocity.magnitude / maxSpeed);

        // Adjust the agent's speed based on the angle between its forward direction and its velocity direction
        AdjustAgentSpeed();
    }

    // Coroutine to rotate the character towards each waypoint in the path and then move
    private IEnumerator RotateAndMove()
    {
        Vector3 destination;

        if (useFlocking)
        {
            // Calculate an offset position for flocking behavior
            destination = CalculateOffsetPosition(targetPosition);
        }
        else
        {
            // Calculate a herding position
            destination = CalculateHerdingPosition(targetPosition);
        }

        // Set the destination of the NavMeshAgent to the calculated position
        agent.SetDestination(destination);

        // Wait until the path is calculated
        yield return new WaitUntil(() => agent.pathPending == false);

        // Iterate through each waypoint in the path
        for (int i = 1; i < agent.path.corners.Length; i++)
        {
            Vector3 waypoint = agent.path.corners[i];

            // Set the rotating flag to true and calculate the direction to the waypoint
            isRotating = true;
            Vector3 direction = (waypoint - transform.position).normalized;

            // Calculate the rotation needed to look at the waypoint
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

            // Rotate the character until it faces the waypoint
            while (Quaternion.Angle(transform.rotation, lookRotation) > 0.1f)
            {
                // Smoothly rotate the character
                transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
                yield return null;
            }

            // Set the rotating flag to false
            isRotating = false;

            // Move to the waypoint
            agent.SetDestination(waypoint);

            // Wait until the agent reaches the waypoint
            yield return new WaitUntil(() => Vector3.Distance(transform.position, waypoint) <= agent.stoppingDistance);
        }

        // The agent will now move towards the final calculated position
        agent.SetDestination(destination);
    }

    // Adjust the agent's speed based on the angle between its forward direction and its velocity direction
    private void AdjustAgentSpeed()
    {
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            // Calculate the angle between the agent's forward direction and its velocity direction
            float angle = Vector3.Angle(transform.forward, agent.velocity);

            // Adjust the agent's speed based on the angle
            if (angle > 45f)
            {
                agent.speed = minSpeed;
            }
            else
            {
                agent.speed = maxSpeed;
            }
        }
    }

    // Calculate an offset position for flocking behavior
    private Vector3 CalculateOffsetPosition(Vector3 target)
    {
        // Generate a random angle in radians
        float angle = Random.Range(0f, Mathf.PI * 2);

        // Calculate the x and z coordinates on the circumference of the circle
        float x = Mathf.Cos(angle) * flockRadius;
        float z = Mathf.Sin(angle) * flockRadius;

        // Return the target position with the offset
        return new Vector3(target.x + x, target.y, target.z + z);
    }

    private Vector3 CalculateHerdingPosition(Vector3 target)
    {
        // Calculate the vector from the current position to the target
        Vector3 direction = (agent.transform.position - target).normalized;

        // Calculate the distance to the target
        float distance = Vector3.Distance(agent.transform.position, target);

        // If the distance is less than the minimum distance, calculate the herding position
        if (distance < minDistance)
        {
            // Calculate the herding position by moving along the direction vector
            Vector3 herdingPosition = agent.transform.position + direction * (minDistance - distance);
            return herdingPosition;
        }

        // If the distance is greater than or equal to the minimum distance, return the current position
        return agent.transform.position;
    }
}
