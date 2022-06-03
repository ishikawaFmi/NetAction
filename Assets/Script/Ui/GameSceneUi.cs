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

    [SerializeField] GameObject _exitUiPanel;

    private void Awake()
    {
        if (Incetance == null)
        {
            Incetance = this;
        }
    }

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_exitUiPanel.gameObject.activeSelf)
            {
                _exitUiPanel.SetActive(false);
            }
            else
            {
                _exitUiPanel.SetActive(true);
            }
            
        }
    }

    /// <summary>
    /// 相手の手札のカードを生成する(見た目だけ)
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

    /// <summary>
    /// 選択したカードを見やすいようにハイライトする
    /// </summary>
    /// <param name="highlightCard"></param>
    public void HighlightCard(Card highlightCard = null)
    {
        foreach (var card in GameManager.Incetance.MyDeakCardList)
        {
            if (highlightCard == null)
            {
                card.CardImage.color = new Color(1, 1, 1, 0.5f);
            }
            else
            {
                if (card.CurrentSuit == highlightCard.CurrentSuit && card.Index == highlightCard.Index)
                {
                    card.CardImage.color = new Color(1, 1, 1, 1);
                }
                else
                {
                    card.gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                }
            }

        }
    }

    /// <summary>
    /// タイトルに戻る
    /// </summary>
    public void ExitButton()
    {
        WebSocketManager.Incetance.WebSocketSendMessege(new WebSocketManager.Messege(("ExitRoom"), WebSocketManager.Messege.MessegeState.Room));
    }
}
