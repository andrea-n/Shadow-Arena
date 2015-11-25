using UnityEngine;
using System.Collections;

public class RankTrap : BasicUnit
{
    override public void useAbility()
    {
        if(!attacking)
        {
            hiddenRank = 99;
        }
    }
}
