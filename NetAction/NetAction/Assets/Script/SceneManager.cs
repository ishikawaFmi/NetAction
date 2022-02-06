using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }       
    }
 
    public void GameSceneLoadAsync()
    {
       var scene = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Game");

       scene.allowSceneActivation = false;

        GameManager.Instance.IsGameStart = scene;
    }
}
