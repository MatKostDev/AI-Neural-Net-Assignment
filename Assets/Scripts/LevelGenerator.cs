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
	private float groundHeight = -50f;

	[SerializeField]
	private float obstacleHeight = -28.7f;

	[SerializeField]
	private List<GameObject> groundObjects = new List<GameObject>();

	[SerializeField]
	private List<GameObject> obstacleObjects = new List<GameObject>();

	private void Awake()
    {
        
    }

    private void Update()
    {
	    if (player.transform.position.x > groundObjects[groundObjects.Count - 1].transform.position.x)
	    {
		    AddGround();
	    }
    }

    private void AddGround()
    {
	    Transform lastTransform = groundObjects[groundObjects.Count - 1].transform;

	    const float offset = 0.2f;
	    
	    Vector3 newPosition = lastTransform.position + new Vector3(lastTransform.localScale.x - offset, 0f, 0f);

	    groundObjects.Add(Instantiate(groundPrefab, newPosition, Quaternion.identity, lastTransform.parent));

		if (groundObjects.Count > 2)
	    {
		    Destroy(groundObjects[0]);
	    }
    }

    private void AddObstacle()
    {
	    if (obstacleObjects.Count > 5)
	    {
		    Destroy(obstacleObjects[0]);
	    }
    }
}
