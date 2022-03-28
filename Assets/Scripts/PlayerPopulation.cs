using System.Collections;
using System.Collections.Generic;
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

	private int m_currentGeneration = 1;

	private List<PlayerController> m_population     = new List<PlayerController>();
	private List<PlayerController> m_livePopulation = new List<PlayerController>();

	private PlayerController m_lastAlive;

	private bool m_loadingDone = false;

	public PlayerController LivePlayer
	{
		get => m_livePopulation.Count > 0 ? m_livePopulation[0] : null;
	}

	private void Awake()
	{
		DontDestroyOnLoad(gameObject); 
		SceneManager.sceneLoaded += OnSceneLoaded;
	}
	
	void OnSceneLoaded(Scene a_scene, LoadSceneMode a_mode)
	{
		if (a_scene.name != "Gameplay")
		{
			return;
		}

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

		//keep best performer of last generation
		timesList.Add(m_lastAlive.JumpTimes);

		for (int i = 1; i < generationSize; i++)
		{
			//initialize values based on weighted random probability
			timesList.Add(GetPlayerWeighted(scoreSum).JumpTimes);
		}

		m_loadingDone = false;

		SceneManager.LoadScene("Gameplay");
		
		//wait until level is done loading
		yield return new WaitUntil(() => m_loadingDone);
		yield return null;

		//create a new population
		List<PlayerController> newPopulation = new List<PlayerController>(generationSize);

		//keep best performer of last generation
		var bestLastGen = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

		//initialize values from parent and mutate it
		bestLastGen.InitAndMutateValues(timesList[0]);
		newPopulation.Add(bestLastGen);
		bestLastGen.onDie += OnPlayerDie; //subscribe to die action

		//spawn new population
		float zPosition = 1f;
		for (int i = 1; i < timesList.Count; i++)
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
