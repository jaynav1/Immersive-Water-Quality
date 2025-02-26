using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CowFactory : MonoBehaviour
{
    public GameObject cowPrefab;
    public int cowCount = 3;
    public GameObject cowSpawnPlane;
    public GameObject paddockPlane;

    //Materials to choose from
    public Material[] materials;

    //Dict of cows
    private Dictionary<GameObject, Vector3> cows = new Dictionary<GameObject, Vector3>();

    // Start is called before the first frame update
    void Start()
    {
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
   
    public void DestroyCows()
    {
        // Destroy all cows
        foreach (GameObject cow in cows.Keys)
        {
            Destroy(cow);
        }

        cows.Clear();
    }

    public void SpawnCows()
    {
        DestroyCows();

        // Spawn new cows
        for (int i = 0; i < cowCount; i++)
        {
            Vector3 randomPoint = GetRandomPointOnPlane(cowSpawnPlane);
            GameObject newCow = Instantiate(cowPrefab, randomPoint, Quaternion.identity);
            
            // Set the parent of the new cow to be the same as the parent of this script
            newCow.transform.SetParent(transform);

            // Reset local scale
            newCow.transform.localScale = Vector3.one;

            //Set main material of cow
            newCow.GetComponentInChildren<SkinnedMeshRenderer>().material = materials[Random.Range(0, materials.Length)];

            // Rotate cow 90 degrees
            newCow.transform.Rotate(Vector3.up, -90f);

            cows.Add(newCow, randomPoint);
        }
    }

    public IEnumerator MoveToPlane(GameObject plane)
    {
        // Move cows to new plane
        foreach (GameObject cow in cows.Keys)
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
        float tolerance = 0.01f;
        float startTime = Time.time;

        while (!allReached && Time.time - startTime < 10f)
        {
            allReached = true;

            foreach (GameObject cow in cows.Keys)
            {
                NavMeshAgent agent = cow.GetComponent<NavMeshAgent>();

                if (agent.pathPending || agent.remainingDistance > tolerance)
                {
                    allReached = false;
                    break;
                }
            }

            yield return null;
        }
    }

    public IEnumerator ReturnCows()
    {
        // Move cows to start point
        foreach ((GameObject cow, Vector3 spawnPoint) in cows)
        {
            // Get agent of Cow
            NavMeshAgent agent = cow.GetComponent<NavMeshAgent>();

            // Move cow to spawn point
            agent.SetDestination(spawnPoint);
        }

        // Check if all cows have reached their destination
        bool allReached = false;
        float tolerance = 0.5f;
        float startTime = Time.time;

        while (!allReached && Time.time - startTime < 10f)
        {
            allReached = true;

            foreach (GameObject cow in cows.Keys)
            {
                NavMeshAgent agent = cow.GetComponent<NavMeshAgent>();

                if (agent.pathPending || agent.remainingDistance > tolerance)
                {
                    allReached = false;
                    break;
                }
            }

            yield return null;
        }
    }
}
