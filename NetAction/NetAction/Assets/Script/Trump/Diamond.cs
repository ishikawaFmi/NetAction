using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Diamond : Trump
{
    public Diamond(int index)
    {
        CardIndex = index;
      
    }
    void Start()
    {
        SetCard();
    }
    public override void SetCard()
    {
        MySuit = Suit.Diamond;
        GetComponent<Image>().sprite = CardSprites[CardIndex - 1];
    }
}
