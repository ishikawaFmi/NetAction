using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneUi : MonoBehaviour
{
    [SerializeField] GameObject _liscenePanel;

   public void LicenceButton()
    {
        if (_liscenePanel.activeInHierarchy)
        {
            _liscenePanel.SetActive(false);
        }
        else
        {
            _liscenePanel.SetActive(true);
        }
    }
}
