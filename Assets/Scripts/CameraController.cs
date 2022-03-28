using UnityEngine;

class CameraController : MonoBehaviour
{
	[SerializeField]
	private float startOffsetX = 23.5f;

	private PlayerPopulation m_population;

	private void Start()
	{
		//use population to track player
		m_population = FindObjectOfType<PlayerPopulation>();
	}

	private void LateUpdate()
	{
		//find live player
		var player = m_population.LivePlayer;
		if (!player)
		{
			return;
		}

		//update x position to follow player but maintain offset
		Vector3 newPosition = transform.position;
		newPosition.x = player.transform.position.x + startOffsetX;

		transform.position = newPosition;
	}
}