using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField]
	private float startSpeed = 5f;

	[SerializeField]
	private float jumpHeight = 2f;

	private float m_speed;

	private float m_jumpVelocity;

	private Rigidbody2D m_rigidbody;

	private bool m_isGrounded = true;

	private void Awake()
	{
		m_rigidbody = GetComponent<Rigidbody2D>();
		
		m_speed = startSpeed;

		//calculate velocity needed in order to reach desired jump height
		m_jumpVelocity = Mathf.Sqrt(jumpHeight * 2f * m_rigidbody.gravityScale * Physics.gravity.y);
	}
	
    private void Update()
    {
	    m_rigidbody.velocity = m_speed * Vector2.right;

	    if (Input.GetKeyDown(KeyCode.Space))
	    {
		    Jump();
	    }
    }

    private void Jump()
    {
	    if (!m_isGrounded)
	    {
		    return;
	    }
	    
	    Vector3 newVelocity = m_rigidbody.velocity;
	    newVelocity.y = m_jumpVelocity;

	    m_rigidbody.velocity = newVelocity;

	    m_isGrounded = false;
    }

    private void OnCollisionEnter2D(Collision2D a_collision)
    {
	    ResolveCollision(a_collision.gameObject);
    }

	private void ResolveCollision(GameObject a_other)
    {
		if (a_other.layer != LayerMask.NameToLayer("Ground"))
		{
			return;
		}

		m_isGrounded = true;
    }
}
