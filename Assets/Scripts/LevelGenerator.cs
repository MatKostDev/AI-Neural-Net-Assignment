using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
	[SerializeField]
	private PlayerController player = null;
	
	[SerializeField]
	private GameObject groundPrefab = null;
	
    [SerializeField]
	private GameObject obstaclePrefab = null;

	[SerializeField]
	private float distanceBetweenObstacles = 15f;

	[SerializeField]
	private List<GameObject> groundObjects = new List<GameObject>();

	[SerializeField]
	private List<GameObject> obstacleObjects = new List<GameObject>();

	private float m_lastObstacleDistance;

	private void Awake()
    {
        
    }

    private void Update()
    {
	    //spawn new obstacle if player travelled the distance between obstacles
	    if (player.transform.position.x - m_lastObstacleDistance > distanceBetweenObstacles)
	    {
		    AddObstacle();
	    }
	    
	    //spawn new ground if player is past the last ground piece
	    if (player.transform.position.x > groundObjects[groundObjects.Count - 1].transform.position.x)
	    {
		    AddGround();
	    }
    }

    private void AddGround()
    {
	    Transform lastTransform = groundObjects[groundObjects.Count - 1].transform;
	    
	    //create a new ground object at the end of the current one (pivot is object center, so scale works)
	    Vector3 newPosition = lastTransform.position + new Vector3(lastTransform.localScale.x, 0f, 0f);

	    //create new ground and add it to the list
	    groundObjects.Add(Instantiate(groundPrefab, newPosition, Quaternion.identity, lastTransform.parent));

	    //destroy old, no longer needed ground pieces to free up memory space
		if (groundObjects.Count > 2)
	    {
		    Destroy(groundObjects[0]);
		    groundObjects.RemoveAt(0);
	    }
    }

    private void AddObstacle()
    {
	    m_lastObstacleDistance = player.transform.position.x; //set new distance for tracking
	    
	    Transform lastTransform = obstacleObjects[obstacleObjects.Count - 1].transform;

	    //create a new obstacle based on desired distance
	    Vector3 newPosition = lastTransform.position + new Vector3(distanceBetweenObstacles, 0f, 0f);

		//create new obstacle and add it to the list
		obstacleObjects.Add(Instantiate(obstaclePrefab, newPosition, Quaternion.identity, lastTransform.parent));

	    //destroy old, no longer needed obstacles to free up memory space
	    if (obstacleObjects.Count > 4)
	    {
		    Destroy(obstacleObjects[0]);
		    obstacleObjects.RemoveAt(0);
		}
    }
}
