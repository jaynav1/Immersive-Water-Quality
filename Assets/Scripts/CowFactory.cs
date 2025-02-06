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

    public void SpawnCows()
    {
        // Clear existing cows
        foreach (var cow in cows.Keys)
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
            newCow.transform.SetParent(transform);

            // Reset local scale
            newCow.transform.localScale = Vector3.one;

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
            IdleAnim idleAnim = cow.GetComponent<IdleAnim>();
            
            // Get random point on new plane
            Vector3 randomPoint = GetRandomPointOnPlane(plane);

            // Move cow to random point
            //StartCoroutine(idleAnim.RotateAndMove(randomPoint));
            cow.GetComponent<NavMeshAgent>().SetDestination(randomPoint);
        }

        // Check if all cows have reached their destination
        bool allReached = false;

        while (!allReached)
        {
            allReached = true;

            foreach (GameObject cow in cows.Keys)
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

    public IEnumerator ReturnCows()
    {
        // Move cows to start point
        foreach ((GameObject cow, Vector3 spawnPoint) in cows)
        {
            // Get agent of Cow
            IdleAnim idleAnim = cow.GetComponent<IdleAnim>();
            
            // Move cow to spawn point
            //StartCoroutine(idleAnim.RotateAndMove(spawnPoint));
            cow.GetComponent<NavMeshAgent>().SetDestination(spawnPoint);
        }

        // Check if all cows have reached their destination
        bool allReached = false;

        while (!allReached)
        {
            allReached = true;

            foreach (GameObject cow in cows.Keys)
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
