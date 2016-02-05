using UnityEngine;
using System.Collections;

public class RankPortal : BasicUnit
{
    override public void useAbility()
    {
        if(!attacking)
        {
            GameObject.Find("MenuManager").GetComponent<MenuManager>().QuitToMenu();
        }
    }
}
