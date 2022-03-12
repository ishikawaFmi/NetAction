using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Diamond : Trump
{
    public Diamond(int index)
    {
        SetCard(index);
        
    }

    public override void SetCard(int index)
    {
        CardIndex = index;
        MySuit = Suit.Diamond;
    }
}
