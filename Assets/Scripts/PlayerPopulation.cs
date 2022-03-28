using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPopulation : MonoBehaviour
{
	[SerializeField]
	private PlayerController playerPrefab = null;
	
	[SerializeField]
	private Vector3 spawnPosition;
	
	[SerializeField]
	private int generationSize = 100;
	
	private TMP_Text m_generationDisplay;
	private TMP_Text m_currentScoreDisplay;
	private TMP_Text m_highScoreDisplay;

	private int m_currentGeneration = 1;

	private List<PlayerController> m_population     = new List<PlayerController>();
	private List<PlayerController> m_livePopulation = new List<PlayerController>();

	private PlayerController m_lastAlive;

	private bool m_loadingDone = false;

	private float m_bestScore = 0f;

	public PlayerController LivePlayer
	{
		get => m_livePopulation.Count > 0 ? m_livePopulation[0] : null;
	}

	private void Awake()
	{
		DontDestroyOnLoad(gameObject); 
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void Update()
	{
		var player = LivePlayer;
		if (!player)
		{
			return;
		}
		
		//update current score display
		float currentScore = player.DistanceTravelled;
		m_currentScoreDisplay.text = "Current Score: " + Mathf.FloorToInt(currentScore);

		if (currentScore > m_bestScore)
		{
			//update high score and display
			m_bestScore = currentScore;

			m_highScoreDisplay.text = "High Score: " + Mathf.FloorToInt(m_bestScore);
		}
	}
	
	void OnSceneLoaded(Scene a_scene, LoadSceneMode a_mode)
	{
		if (a_scene.name != "Gameplay")
		{
			return;
		}

		//find display objects after new scene load
		m_generationDisplay   = GameObject.FindGameObjectWithTag("GenDisplay").GetComponent<TMP_Text>();
		m_currentScoreDisplay = GameObject.FindGameObjectWithTag("CurrentScoreDisplay").GetComponent<TMP_Text>();
		m_highScoreDisplay    = GameObject.FindGameObjectWithTag("HighScoreDisplay").GetComponent<TMP_Text>();

		m_highScoreDisplay.text = "High Score: " + Mathf.FloorToInt(m_bestScore);

		m_loadingDone = true;

		if (m_currentGeneration == 1)
		{
			InitFirstGeneration();
		}
	}

	private void InitFirstGeneration()
	{
		//initialize first generations, does not have any parents to inherit from
		float zPosition = 0f;
		for (int i = 0; i < generationSize; i++)
		{
			//line up along the z axis for 3D
			Vector3 newSpawnPosition = new Vector3(spawnPosition.x, spawnPosition.y, zPosition);
			
			var newPlayer = Instantiate(playerPrefab, newSpawnPosition, Quaternion.identity);

			m_population.Add(newPlayer);

			newPlayer.onDie += OnPlayerDie; //subscribe to die action

			zPosition += 1f;
		}

		m_livePopulation = new List<PlayerController>(m_population);
	}

	private IEnumerator StartNewGeneration()
	{
		//initialize new generation
		m_currentGeneration++;

		//get total score of population
		float scoreSum = 0f;
		for (int i = 0; i < m_population.Count; i++)
		{
			scoreSum += m_population[i].GetScore();
		}

		List<List<float>> timesList = new List<List<float>>();

		//keep best performance of last generation
		List<float> bestPerformance = m_lastAlive.JumpTimes;

		for (int i = 0; i < generationSize; i++)
		{
			//initialize values based on weighted random probability
			timesList.Add(GetPlayerWeighted(scoreSum).JumpTimes);
		}

		m_loadingDone = false;

		SceneManager.LoadScene("Gameplay");
		
		//wait until level is done loading
		yield return new WaitUntil(() => m_loadingDone);
		yield return null;
		
		//update generation display
		m_generationDisplay.text = "Gen: " + m_currentGeneration;

		//create a new population
		List<PlayerController> newPopulation = new List<PlayerController>(generationSize);

		//keep best performer of last generation
		var bestLastGen = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

		//initialize values from parent and mutate it
		bestLastGen.JumpTimes = bestPerformance;
		newPopulation.Add(bestLastGen);
		bestLastGen.onDie += OnPlayerDie; //subscribe to die action

		//spawn new population
		float zPosition = 1f;
		for (int i = 0; i < timesList.Count; i++)
		{
			Vector3 newSpawnPosition = new Vector3(spawnPosition.x, spawnPosition.y, zPosition);

			var newPlayer = Instantiate(playerPrefab, newSpawnPosition, Quaternion.identity);

			//initialize values from parent and mutate it
			newPlayer.InitAndMutateValues(timesList[i]);

			newPopulation.Add(newPlayer);

			newPlayer.onDie += OnPlayerDie; //subscribe to die action

			zPosition += 1f;
		}

		m_population     = new List<PlayerController>(newPopulation);
		m_livePopulation = new List<PlayerController>(m_population);
	}

	private PlayerController GetPlayerWeighted(float a_scoreSum)
	{
		//weighted random algorithm
		float randomScore = Random.Range(0f, a_scoreSum);
		float currentSum  = 0f;
		
		for (int i = 0; i < m_population.Count; i++)
		{
			//keep running sum and compare to random value
			currentSum += m_population[i].GetScore();

			if (currentSum > randomScore)
			{
				return m_population[i];
			}
		}
		
		//choose last index
		return m_population[m_population.Count - 1];
	}

	private void OnPlayerDie(PlayerController a_player)
	{
		//check if it's the last player that's alive
		if (m_livePopulation.Count == 1)
		{
			m_lastAlive = a_player;
			
			//start next generation
			StartCoroutine(StartNewGeneration());
			return;
		}
		
		m_livePopulation.Remove(a_player);
	}
}
