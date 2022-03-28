using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    private void Start()
    {
	    //load gameplay scene immediately
	    //entry scene is just for initializing player population manager
        SceneManager.LoadScene("Gameplay");
    }
}
