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

	[SerializeField]
	private float mutationProbability = 0.2f;

	[SerializeField]
	private float minTimeBetweenJumps = 0.5f;

	private float maxTimeBetweenJumps = 5f;

	private float m_speed;

	private float m_jumpVelocity;

	private Rigidbody2D m_rigidbody;

	private bool m_isGrounded = true;

	private float m_startPositionX;

	private bool m_isDead = false;

	private List<float> m_jumpTimes = new List<float>();

	private int m_jumpIndex;

	private float m_timeSinceJump;

	public List<float> JumpTimes
	{
		get => m_jumpTimes;
	}

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

		m_jumpTimes.Add(DetermineJumpTime());
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

		m_timeSinceJump += Time.deltaTime;
		//check for jump
		if (m_timeSinceJump > m_jumpTimes[m_jumpIndex])
		{
			Jump();
		}
    }

    public void InitAndMutateValues(List<float> a_jumpTimes)
    {
	    m_jumpTimes = new List<float>(a_jumpTimes);
	    
	    if (Random.value > mutationProbability)
	    {
			m_jumpTimes[m_jumpTimes.Count - 1] = DetermineJumpTime();
		}
    }

    private float DetermineJumpTime()
    {
	    return Random.Range(minTimeBetweenJumps, maxTimeBetweenJumps);
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

	    m_jumpIndex++;
	    
		if (m_jumpIndex >= m_jumpTimes.Count)
		{
			m_jumpTimes.Add(DetermineJumpTime());
		}

		m_timeSinceJump = 0f;
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
		m_isDead = true;

		Destroy(m_rigidbody);

		onDie?.Invoke(this);
	}

	public float GetScore()
	{
		return DistanceTravelled * DistanceTravelled * DistanceTravelled;
	}
}
