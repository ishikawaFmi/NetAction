using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trump : MonoBehaviour
{
    public int cardIndex;

    [SerializeField] Sprite[] heartCardSprites = null;
    [SerializeField] Sprite[] diamondCardSprites = null;
    [SerializeField] Sprite[] spadeCardSprites = null;
    [SerializeField] Sprite[] clubCardSprites = null;

    public Suit suit= Suit.None;
    public enum Suit
    {
        Heart,
        Diamond,
        Spade,
        Club,
        None,
    }
    void Start()
    {
        var cardSprite = GetComponent<Image>().sprite;
        switch (suit)
        {
            case Suit.Heart:
                cardSprite = heartCardSprites[cardIndex - 1];
                break;
            case Suit.Diamond:
                cardSprite = diamondCardSprites[cardIndex - 1];
                break;
            case Suit.Spade:
                cardSprite = spadeCardSprites[cardIndex - 1];
                break;
            case Suit.Club:
                cardSprite = clubCardSprites[cardIndex - 1];
                break;
            case Suit.None:
                break;
            default:
                break;
        }
    }
}
