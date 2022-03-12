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

    public void GameSceneLoad()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
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
