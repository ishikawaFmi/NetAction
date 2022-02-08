using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Heart : Trump
{
    public Heart(int index)
    {
        CardIndex = index;
      
    }
    void Start()
    {
        SetCard();
    }
    public override void SetCard()
    {
        MySuit = Suit.Heart;
        GetComponent<Image>().sprite = CardSprites[CardIndex - 1];
    }
}
