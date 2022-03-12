using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Spade : Trump
{
    public Spade(int index)
    {
        SetCard(index);
    }
 
    public override void SetCard(int index)
    {
        CardIndex = index;
        MySuit = Suit.Spade;
    }

}
