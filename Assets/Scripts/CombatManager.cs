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
    [SerializeField]
    private OnCombatBegin m_OnCombatBegin = new OnCombatBegin();
    [SerializeField]
    private OnCombatUpdate m_OnCombatUpdate = new OnCombatUpdate();
    [SerializeField]
    private OnCombatEnd m_OnCombatEnd = new OnCombatEnd();

    [SerializeField]
    private OnPlayerTurn m_OnPlayerTurn = new OnPlayerTurn();

    [SerializeField]
    private List<Sprite> m_GemSprites = new List<Sprite>();

    //TODO: public List<Enemy> enemies = new List<>;
    public List<Sprite> gemSprites { get { return m_GemSprites; } }

    public OnCombatBegin onCombatBegin { get { return m_OnCombatBegin; } }
    public OnCombatUpdate onCombatUpdate { get { return m_OnCombatUpdate; } }
    public OnCombatEnd onCombatEnd { get { return m_OnCombatEnd; } }

    public OnPlayerTurn onPlayerTurn { get { return m_OnPlayerTurn; } }

    private void OnSlide(SlideInformation slideInfo)
    {
        onPlayerTurn.Invoke();
    }
}
