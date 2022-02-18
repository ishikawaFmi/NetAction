using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
abstract public class Trump
{
    public int CardIndex;

    public Suit MySuit = Suit.None;

    public enum Suit
    {
        Heart,
        Diamond,
        Spade,
        Club,
        None,
    }
    abstract public void SetCard(int index);
}
