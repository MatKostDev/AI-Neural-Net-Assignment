using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    private void Start()
    {
	    StartCoroutine(StartGameRoutine());
    }

    IEnumerator StartGameRoutine()
    {
	    //load gameplay scene after delay
	    //entry scene is just for initializing player population manager
	    yield return new WaitForSeconds(2f);
	    SceneManager.LoadScene("Gameplay");
    }
}
