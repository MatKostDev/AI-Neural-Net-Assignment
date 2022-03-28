using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
	public UnityAction<PlayerController> onDie;
	
	[SerializeField]
	private float startSpeed = 5f;

	[SerializeField]
	private float speedUpRate = 0.5f;

	[SerializeField]
	private float jumpHeight = 2f;

	private float m_speed;

	private float m_jumpVelocity;

	private Rigidbody2D m_rigidbody;

	private bool m_isGrounded = true;

	private float m_startPositionX;

	private bool m_isDead = false;

	public float CurrentSpeed
	{
		get => m_speed;
	}

	public bool IsDead
	{
		get => m_isDead;
	}

	public float DistanceTravelled
	{
		//get the total distance travelled (only x axis counts)
		get => transform.position.x - m_startPositionX;
	}

	private void Awake()
	{
		m_rigidbody = GetComponent<Rigidbody2D>();
		
		m_speed = startSpeed;

		//calculate velocity needed in order to reach desired jump height
		m_jumpVelocity = Mathf.Sqrt(Mathf.Abs(jumpHeight * 2f * m_rigidbody.gravityScale * Physics.gravity.y));

		m_startPositionX = transform.position.x; //set initial x position
	}
	
    private void Update()
    {
	    if (m_isDead)
	    {
		    return;
	    }
	    
	    //accelerate over time
	    m_speed += speedUpRate * Time.deltaTime;
		
		//set updated velocity based on speed increase
		Vector2 newVelocity = m_rigidbody.velocity;
		newVelocity.x = m_speed;

		m_rigidbody.velocity = newVelocity;

		//check for jump
	    //if (Input.GetKeyDown(KeyCode.Space))
	    //{
		   // Jump();
	    //}
    }

    private void Jump()
    {
	    //only jump when grounded
	    if (!m_isGrounded)
	    {
		    return;
	    }
	    
	    //add y velocity for jump
	    Vector2 newVelocity = m_rigidbody.velocity;
	    newVelocity.y = m_jumpVelocity;

	    m_rigidbody.velocity = newVelocity;

	    m_isGrounded = false;
    }

    //triggers are used for obstacles
    private void OnTriggerEnter2D(Collider2D a_other)
    {
	    Die();
    }

    private void OnCollisionEnter2D(Collision2D a_collision)
    {
	    ResolveCollision(a_collision.gameObject);
    }

	private void ResolveCollision(GameObject a_other)
    {
	    //if ground is collided with, set grounded to true
		if (a_other.layer != LayerMask.NameToLayer("Ground"))
		{
			return;
		}

		m_isGrounded = true;
	}

	private void Die()
	{
		Debug.Log("dedded");
		m_isDead = true;

		Destroy(m_rigidbody);

		onDie?.Invoke(this);
	}

	public float GetScore()
	{
		return DistanceTravelled * DistanceTravelled;
	}
}
