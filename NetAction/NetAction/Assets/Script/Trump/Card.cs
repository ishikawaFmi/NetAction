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
        if (GameManager.Instance.MyDeakCardList.Contains(this))
        {
            GameManager.Instance.CurrentCard = this;
            GameManager.Instance.HighlightCard(this);
        }
    }

    public async void ChengeCardRequest()
    {
        if (GameManager.Instance.CurrentCard != null) 
        {
            if (Index + 1 == GameManager.Instance.CurrentCard.Index || Index - 1 == GameManager.Instance.CurrentCard.Index)
            {
                var changeCard = new Dictionary<string, object>()
            {
                {"BeforeSuit",(int)CurrentSuit},
                {"BeforeIndex",Index},
                {"AfterSuit",(int)GameManager.Instance.CurrentCard.CurrentSuit},
                {"AfterIndex",GameManager.Instance.CurrentCard.Index},
            };
               var cardJson = MiniJSON.Json.Serialize(changeCard);
               await NetWorkManager.Incetance.SendJsonMessege(new NetWorkManager.Messege("ChengeCard", NetWorkManager.SendMesageState.MetHod, NetWorkManager.Incetance.PlayerId, cardJson));
            }
        }
      
    }
}
