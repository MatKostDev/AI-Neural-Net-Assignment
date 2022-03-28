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

		if (m_currentGeneration == 1)
		{
			InitFirstGeneration();
		} 
		else
		{
			StartNewGeneration();
		}
	}

	private void InitFirstGeneration()
	{
		float zPosition = 0f;
		for (int i = 0; i < generationSize; i++)
		{
			Vector3 newSpawnPosition = new Vector3(spawnPosition.x, spawnPosition.y, zPosition);
			
			var newPlayer = Instantiate(playerPrefab, newSpawnPosition, Quaternion.identity);

			m_population.Add(newPlayer);

			zPosition += 1f;
		}

		m_livePopulation = m_population;
	}

	private void StartNewGeneration()
	{
		m_currentGeneration++;
	}

	private void OnPlayerDie(PlayerController a_player)
	{
		if (m_livePopulation.Count == 1)
		{
			m_lastAlive = a_player;
			
			//they all died idk do something
		}
		
		m_livePopulation.Remove(a_player);
	}
}
