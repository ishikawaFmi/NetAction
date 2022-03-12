using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultSceneManager : MonoBehaviour
{
    public static ResultSceneManager Incetance;

    public static string ResultString;

    [SerializeField] Text _resultText;

    void Awake()
    {
        if (Incetance == null)
        {
            Incetance = this;
            
        }
    }

    void Start()
    {
        Result();
    }

    public void Result()
    {
        _resultText.text = ResultString;
    }

   public void TitleScneButtonPush()
    {
        SceneManager.Incetance.TitleScene();
    }
}
