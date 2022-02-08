using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
abstract public class Trump : MonoBehaviour
{
    public int CardIndex;

    public Sprite[] CardSprites;

    [NonSerialized] public Suit MySuit = Suit.None;

    public enum Suit
    {
        Heart,
        Diamond,
        Spade,
        Club,
        None,
    }

    void OnValidate()
    {
        if (CardIndex <= 13 && CardIndex >= 1)
        {
            GetComponent<Image>().sprite = CardSprites[CardIndex - 1];
        }
     
    }
    abstract public void SetCard();
}
