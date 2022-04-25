using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneUi : MonoBehaviour
{
    public static GameSceneUi Incetance { get; private set; }

    [SerializeField] Image _myCard;

    [SerializeField] Image _enemyCard;

    [SerializeField] GameObject _enemyCardPanel;

    [SerializeField] Sprite _blackCard;

    [SerializeField] Sprite _redCard;

    void Start()
    {
        if (GameManager.MyColor == GameManager.TrunpColor.Black)
        {
            _myCard.sprite = _blackCard;
            _enemyCard.sprite = _redCard;
        }
        else
        {
            _myCard.sprite = _redCard;
            _enemyCard.sprite = _blackCard;
        }
    }

    /// <summary>
    /// ‘Šè‚ÌèD‚ÌƒJ[ƒh‚ğ¶¬‚·‚é(Œ©‚½–Ú‚¾‚¯)
    /// </summary>
    public void EnemyCardCreate()
    {
        if (_enemyCardPanel.transform.childCount < 4)
        {
            if (GameManager.Incetance.EnemyTrumpCount > 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    var enemyCard = Instantiate(GameManager.Incetance.CardPrefab, _enemyCardPanel.transform).GetComponent<Image>();
                    enemyCard.sprite = _enemyCard.sprite;
                }

            }
            else
            {
                if (_enemyCardPanel.transform.childCount > 0)
                {
                    Destroy(_enemyCardPanel.transform.GetChild(0));
                }
            }
        }
    }
}
