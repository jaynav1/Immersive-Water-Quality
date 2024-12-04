using System.Collections;
using UnityEngine;

public class IdleAnim : MonoBehaviour
{
    private Animator animator; // Reference to the Animator component

    void Start()
    {
        // Get the Animator component attached to the GameObject
        animator = GetComponent<Animator>();

        // Set random speed for the all animations
        animator.speed = Random.Range(0.9f, 1f);

        // Start the coroutine to randomly change the idleanim parameter
        StartCoroutine(RandomlyChangeIdleAnim());
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
}
