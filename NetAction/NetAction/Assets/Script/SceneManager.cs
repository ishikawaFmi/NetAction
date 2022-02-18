using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
public class SceneManager : MonoBehaviour
{
    public static SceneManager Incetance { get; private set; }

    void Awake()
    {
        if (Incetance == null) 
        {
            Incetance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }       
       
    }
 
    public void GameSceneLoadAsync()
    {
       var scene = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Game");

       scene.allowSceneActivation = false;

       NetWorkManager.Incetance.GameSceneAsync = scene;
    }

    public void TitleScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
    }
    public void ResultScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Result");
    }
}
