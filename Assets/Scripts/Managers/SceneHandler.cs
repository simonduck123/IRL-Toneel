using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SceneHandler : MonoBehaviour
{
    public void SceneLoader(float sceneIndex)
    {
        SceneManager.LoadScene((int)sceneIndex);
        Debug.Log("Scene Loaded: " + (int)sceneIndex);
    }
}
