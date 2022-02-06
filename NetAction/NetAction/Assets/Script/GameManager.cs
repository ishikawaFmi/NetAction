using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    Trump[] trumps = new Trump[26];

    public Subject<Unit> GamePreparation = new Subject<Unit>();



    public Subject<Unit> GameStart = new Subject<Unit>();

  

    public AsyncOperation IsGameStart;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            GamePreparation.Subscribe(_ => NetWorkManager.Incetance.SendJsonMessege(new NetWorkManager.Messege("Rogin", NetWorkManager.SendMesageState.RogIn)));
            GamePreparation.Subscribe(_ => SceneManager.Instance.GameSceneLoadAsync());

            GameStart.Subscribe(_ => IsGameStart.allowSceneActivation = true);
            
        }
        
    }
}