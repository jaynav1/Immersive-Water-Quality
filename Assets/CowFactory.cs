using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CowFactory : MonoBehaviour
{
    public GameObject cowPrefab;
    public int cowCount = 3;
    public GameObject cowSpawnPlane;
    public GameObject paddockPlane;

    //list of cows
    private List<GameObject> cows = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        // Spawn cows evenly along the cowSpawnPlane


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Vector3 GetRandomPointOnPlane(GameObject plane)
    {
        // Get the plane's mesh renderer bounds
        Bounds planeBounds = plane.GetComponent<MeshRenderer>().bounds;
        
        // Generate random point within bounds
        float randomX = Random.Range(planeBounds.min.x, planeBounds.max.x);
        float randomZ = Random.Range(planeBounds.min.z, planeBounds.max.z);
        
        // Use the plane's Y position
        float planeY = plane.transform.position.y;
        
        return new Vector3(randomX, planeY, randomZ);
    }

    public void SpawnCows()
    {
        // Clear existing cows
        foreach (GameObject cow in cows)
        {
            Destroy(cow);
        }
        cows.Clear();

        // Spawn new cows
        for (int i = 0; i < cowCount; i++)
        {
            Vector3 randomPoint = GetRandomPointOnPlane(cowSpawnPlane);
            GameObject newCow = Instantiate(cowPrefab, randomPoint, Quaternion.identity);
            
            // Set the parent of the new cow to be the same as the parent of this script
            newCow.transform.SetParent(transform.parent);
            
            cows.Add(newCow);
        }
    }

    public IEnumerator MoveToPlane(GameObject plane)
    {
        // Move cows to new plane
        foreach (GameObject cow in cows)
        {
            // Get agent of Cow
            NavMeshAgent agent = cow.GetComponent<NavMeshAgent>();
            
            // Get random point on new plane
            Vector3 randomPoint = GetRandomPointOnPlane(plane);

            // Move cow to random point
            agent.SetDestination(randomPoint);
        }

        // Check if all cows have reached their destination
        bool allReached = false;

        while (!allReached)
        {
            allReached = true;

            foreach (GameObject cow in cows)
            {
                NavMeshAgent agent = cow.GetComponent<NavMeshAgent>();

                if (agent.pathPending || agent.remainingDistance > agent.stoppingDistance || agent.velocity.sqrMagnitude != 0f)
                {
                    allReached = false;
                    break;
                }
            }

            yield return null;
        }
    }
}
