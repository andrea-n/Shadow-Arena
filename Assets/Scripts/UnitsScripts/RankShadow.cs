using UnityEngine;
using System.Collections;

public class RankShadow : BasicUnit
{
    override public void useAbility()
    {
        if(attacking)
        {
            hiddenRank = 98;
        }
    }
}
