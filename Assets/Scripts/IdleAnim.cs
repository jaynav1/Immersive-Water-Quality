using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class IdleAnim : MonoBehaviour
{
    private Animator animator; // Reference to the Animator component
    private NavMeshAgent agent; // Reference to the NavMeshAgent component

    public float maxSpeed; // Maximum speed of the agent

    void Start()
    {
        // Get the Animator component attached to the GameObject
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        // Set random speed for the all animations
        animator.speed = Random.Range(0.9f, 1f);
        agent.speed = Random.Range(0.1f, 0.2f);

        // Start the coroutine to randomly change the idleanim parameter
        StartCoroutine(RandomlyChangeIdleAnim());
    }

    void Update()
    {
        // Update the Animator with the agent's velocity
        animator.SetFloat("Speed", agent.velocity.magnitude / maxSpeed);
    }

    // Coroutine to randomly change the idleanim parameter every 3 seconds
    private IEnumerator RandomlyChangeIdleAnim()
    {
        while (true)
        {
            // Wait for 3 seconds
            yield return new WaitForSeconds(3f);

            // Randomly select a new value for the idleanim parameter between 0 and 1
            float newIdleValue = Random.Range(0f, 1f);

            // Set the new value for the idleanim parameter
            animator.SetFloat("IdleAnim", newIdleValue);
        }
    }

    // Coroutine to rotate the character towards each waypoint in the path and then move
    public IEnumerator RotateAndMove(Vector3 destination)
    {
        // Set initial state
        agent.SetDestination(destination);
        agent.speed = 0f;

        // Wait for path calculation
        yield return new WaitUntil(() => !agent.pathPending);

        // Calculate direction to final destination
        Vector3 direction = (destination - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        // Rotate towards final destination
        while (Quaternion.Angle(transform.rotation, lookRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed);
            yield return null;
        }

        // Start moving
        agent.speed = Random.Range(0.1f, 0.2f);

        // Wait until reaching destination
        yield return new WaitUntil(() => agent.remainingDistance <= agent.stoppingDistance);
    }
}
