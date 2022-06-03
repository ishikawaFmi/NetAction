using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Image CardImage;

    public Trump.Suit CurrentSuit;

    public int Index;

    public bool IsDeck;

    /// <summary>
    /// カードに情報をセットする
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="suit"></param>
    /// <param name="index"></param>
    /// <param name="isDeck"></param>
    public void SetCard(Sprite sprite, Trump.Suit suit, int index, bool isDeck = true)
    {
        CardImage.sprite = sprite;
        CurrentSuit = suit;
        Index = index;
        IsDeck = isDeck;

        var trigger = GetComponent<EventTrigger>();
        trigger.triggers.Clear();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        if (IsDeck)
        {
            entry.callback.AddListener(_ => ChoiceCard());
         
        }
        else
        {
            entry.callback.AddListener(_ => ChengeCardRequest());
        }

        trigger.triggers.Add(entry);
    }

    /// <summary>
    /// 現在選択中のカードにセットする
    /// </summary>
    public void ChoiceCard()
    {
        if (GameManager.Incetance.MyDeakCardList.Contains(this))
        {
            GameManager.Incetance.CurrentCard = this;
            GameSceneUi.Incetance.HighlightCard(this);
        }
    }

    /// <summary>
    /// サーバーのカードを交換する関数を呼ぶ
    /// </summary>
    public  void ChengeCardRequest()
    {
        if (GameManager.Incetance.CurrentCard != null) 
        {

            if (GameManager.Incetance.ChangeCheak(Index, GameManager.Incetance.CurrentCard.Index))
            {
                var changeCard = new Dictionary<string, object>()
            {
                {"BeforeSuit",(int)CurrentSuit},
                {"BeforeIndex",Index},
                {"AfterSuit",(int)GameManager.Incetance.CurrentCard.CurrentSuit},
                {"AfterIndex",GameManager.Incetance.CurrentCard.Index},
            };
               var cardJson = MiniJSON.Json.Serialize(changeCard);
               WebSocketManager.Incetance.WebSocketSendMessege(new WebSocketManager.Messege("ChengeCard", WebSocketManager.Messege.MessegeState.Game, cardJson));
            }
        }
      
    }
}
