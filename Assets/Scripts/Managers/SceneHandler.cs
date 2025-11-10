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

    // 🆕 Additive load version
    public void LoadSceneAdditive(float sceneIndex)
    {
        StartCoroutine(LoadSceneAdditiveAsync(sceneIndex));
    }

    private IEnumerator LoadSceneAdditiveAsync(float sceneIndex)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync((int)sceneIndex, LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            Debug.Log($"[Additive] Loading progress: {asyncLoad.progress * 100}%");

            if (asyncLoad.progress >= 0.9f)
            {
                Debug.Log("[Additive] Scene is ready, activating...");
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        Debug.Log("[Additive] Scene loaded additively!");
    }

    // 🧹 Optional: Unload additive scenes by index
    public void UnloadScene(float sceneIndex)
    {
        SceneManager.UnloadSceneAsync((int)sceneIndex);
        Debug.Log("Scene Unloaded: " + (int)sceneIndex);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0)) SceneManager.LoadScene(0);
        else if (Input.GetKeyDown(KeyCode.Alpha1)) SceneManager.LoadScene(1);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) SceneManager.LoadScene(2);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) SceneManager.LoadScene(3);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) SceneManager.LoadScene(4);
        else if (Input.GetKeyDown(KeyCode.Alpha5)) SceneManager.LoadScene(5);
    }
}
