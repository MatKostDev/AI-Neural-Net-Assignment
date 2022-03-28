using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
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

	private PlayerPopulation m_population;

	private void Start()
	{
		m_population = FindObjectOfType<PlayerPopulation>();
	}

    private void Update()
    {
	    PlayerController player = m_population.LivePlayer;
	    if (!player)
	    {
		    return;
	    }

	    float playerPositionX = player.transform.position.x;

		//spawn new obstacle if player travelled the distance between obstacles
		if (playerPositionX - m_lastObstacleDistance > distanceBetweenObstacles)
	    {
		    AddObstacle(playerPositionX);
	    }
	    
	    //spawn new ground if player is past the last ground piece
	    if (playerPositionX > groundObjects[groundObjects.Count - 1].transform.position.x)
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

    private void AddObstacle(float a_playerPositionX)
    {
	    m_lastObstacleDistance = a_playerPositionX; //set new distance for tracking
	    
	    Transform lastTransform = obstacleObjects[obstacleObjects.Count - 1].transform;

	    //create a new obstacle based on desired distance
	    Vector3 newPosition = lastTransform.position + new Vector3(distanceBetweenObstacles, 0f, 0f);

		//create new obstacle and add it to the list
		obstacleObjects.Add(Instantiate(obstaclePrefab, newPosition, Quaternion.identity, lastTransform.parent));

	    //destroy old, no longer needed obstacles to free up memory space
	    if (obstacleObjects.Count > 3)
	    {
		    Destroy(obstacleObjects[0]);
		    obstacleObjects.RemoveAt(0);
		}
    }
}
