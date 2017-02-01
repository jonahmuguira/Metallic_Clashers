using System;
using System.Collections.Generic;
using System.Linq;

using Board.Information;

using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class OnCombatBegin : UnityEvent { }

[Serializable]
public class OnCombatEnd : UnityEvent { }

[Serializable]
public class OnCombatUpdate : UnityEvent { }

[Serializable]
public class OnPlayerTurn : UnityEvent { }

public class CombatManager : SubManager
{
    //public List<Enemy> enemies = new List<>;
    public List<Sprite> gemSprites = new List<Sprite>();

    public OnCombatBegin onCombatBegin = new OnCombatBegin();
    public OnCombatEnd onCombatEnd = new OnCombatEnd();
    public OnPlayerTurn onPlayerTurn = new OnPlayerTurn();

    private void OnSlide(SlideInformation slideInfo)
    {
        
    }
}
