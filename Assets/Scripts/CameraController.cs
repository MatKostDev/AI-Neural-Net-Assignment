using UnityEngine;

class CameraController : MonoBehaviour
{
	[SerializeField]
	private Transform playerTransform = null;

	private float startOffsetX;

	private void Awake()
	{
		startOffsetX = transform.position.x - playerTransform.position.x;
	}

	private void LateUpdate()
	{
		Vector3 newPosition = transform.position;
		newPosition.x = playerTransform.position.x + startOffsetX;

		transform.position = newPosition;
	}
}