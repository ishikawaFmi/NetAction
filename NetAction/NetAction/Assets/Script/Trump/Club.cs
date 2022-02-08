using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Club : Trump
{
    public Club(int index)
    {
        CardIndex = index;
       
    }
    void Start()
    {
        SetCard();
    }
    public override void SetCard()
    {
        MySuit = Suit.Club;
        GetComponent<Image>().sprite = CardSprites[CardIndex - 1];
    }

}
