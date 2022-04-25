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

    public void ChoiceCard()
    {
        if (GameManager.Incetance.MyDeakCardList.Contains(this))
        {
            GameManager.Incetance.CurrentCard = this;
            GameManager.Incetance.HighlightCard(this);
        }
    }

    public  void ChengeCardRequest()
    {
        if (GameManager.Incetance.CurrentCard != null) 
        {

            if (Index + 1 == GameManager.Incetance.CurrentCard.Index || Index - 1 == GameManager.Incetance.CurrentCard.Index || Index == 13 && GameManager.Incetance.CurrentCard.Index == 1 || Index == 1 && GameManager.Incetance.CurrentCard.Index == 13)
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
