using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Spade : Trump
{
    public Spade(int index)
    {
        CardIndex = index;
      
    }
    void Start()
    {
        SetCard();
    }
    public override void SetCard()
    {
        MySuit = Suit.Spade;
        GetComponent<Image>().sprite = CardSprites[CardIndex - 1];
    }

}
