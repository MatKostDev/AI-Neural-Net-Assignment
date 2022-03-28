using UnityEngine;

class CameraController : MonoBehaviour
{
	[SerializeField]
	private float startOffsetX = 23.5f;

	private PlayerPopulation m_population;

	private void Start()
	{
		m_population = FindObjectOfType<PlayerPopulation>();
	}

	private void LateUpdate()
	{
		var player = m_population.LivePlayer;
		if (!player)
		{
			return;
		}

		Vector3 newPosition = transform.position;
		newPosition.x = player.transform.position.x + startOffsetX;

		transform.position = newPosition;
	}
}