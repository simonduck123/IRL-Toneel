using System;
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

    public void SceneLoaderInt(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
        Debug.Log("Scene Loaded: " + sceneIndex);
    }

    public void SceneLoaderAsync(float sceneIndex)
    {
        StartCoroutine(LoadSceneAsync(sceneIndex));
    }

    private IEnumerator LoadSceneAsync(float sceneIndex)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync((int)sceneIndex);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            Debug.Log($"Loading progress: {asyncLoad.progress * 100}%");

            if (asyncLoad.progress >= 0.9f)
            {
                Debug.Log("Scene is ready, activating...");
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0)) SceneManager.LoadScene(0);
        else if (Input.GetKeyDown(KeyCode.Alpha1)) SceneManager.LoadScene(1);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) SceneManager.LoadScene(2);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) SceneManager.LoadScene(3);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) SceneManager.LoadScene(4);
        else if (Input.GetKeyDown(KeyCode.Alpha5)) SceneManager.LoadScene(5);
        else if (Input.GetKeyDown(KeyCode.Alpha6)) SceneManager.LoadScene(6);
        else if (Input.GetKeyDown(KeyCode.Alpha7)) SceneManager.LoadScene(7);
        else if (Input.GetKeyDown(KeyCode.Alpha8)) SceneManager.LoadScene(8);
        else if (Input.GetKeyDown(KeyCode.Alpha9)) SceneManager.LoadScene(9);
        else if (Input.GetKeyDown(KeyCode.Q)) SceneManager.LoadScene(10);
        else if (Input.GetKeyDown(KeyCode.W)) SceneManager.LoadScene(11);
        else if (Input.GetKeyDown(KeyCode.E)) SceneManager.LoadScene(12);
        else if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(13);
        else if (Input.GetKeyDown(KeyCode.T)) SceneManager.LoadScene(14);
        else if (Input.GetKeyDown(KeyCode.Y)) SceneManager.LoadScene(15);
        else if (Input.GetKeyDown(KeyCode.U)) SceneManager.LoadScene(16);
    }
}
