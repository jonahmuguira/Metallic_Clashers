using System;
using System.Collections.Generic;
using System.Linq;

using Board;
using Board.Information;

using Input.Information;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class OnCombatBegin : UnityEvent { }
[Serializable]
public class OnCombatEnd : UnityEvent { }
[Serializable]
public class OnCombatUpdate : UnityEvent { }
[Serializable]
public class OnPlayerTurn : UnityEvent { }

public class CombatManager : SubManager<CombatManager>
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

    [SerializeField]
    private Grid m_Grid;

    //TODO: public List<Enemy> enemies = new List<>;
    public List<Sprite> gemSprites { get { return m_GemSprites; } }

    public OnCombatBegin onCombatBegin { get { return m_OnCombatBegin; } }
    public OnCombatUpdate onCombatUpdate { get { return m_OnCombatUpdate; } }
    public OnCombatEnd onCombatEnd { get { return m_OnCombatEnd; } }

    public OnPlayerTurn onPlayerTurn { get { return m_OnPlayerTurn; } }

    public Grid grid { get { return m_Grid; } }

    protected override void Init()
    {
        //TODO: Initialize Combat

        m_Grid = new Grid(new Vector2(5f, 5f));

        m_Grid.onSlide.AddListener(OnSlide);
    }

    private void Update()
    {

    }

    private void OnSlide(SlideInformation slideInfo)
    {
        onPlayerTurn.Invoke();
    }

    protected override void OnDrag(DragInformation dragInfo)
    {
        var pointerEventData =
            new PointerEventData(EventSystem.current) { position = dragInfo.origin };

        var hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, hits);

        // If nothing was hit
        if (hits.Count <= 0)
            return;

        var firstHit = hits.First();

        var gemMono = firstHit.gameObject.GetComponent<GemMono>();
        // If we didn't hit a GemMono first
        if (!gemMono)
            return;

        if (Mathf.Abs(dragInfo.end.x - dragInfo.origin.x) >
            Mathf.Abs(dragInfo.end.y - dragInfo.origin.y))
        {
            var slideDirection =
                dragInfo.end.x - dragInfo.origin.x > 0
                    ? SlideDirection.Backward
                    : SlideDirection.Forward;

            gemMono.gem.grid.SlideRowAt((int)gemMono.gem.position.y, slideDirection);
        }
        else
        {
            var slideDirection =
                dragInfo.end.y - dragInfo.origin.y > 0
                    ? SlideDirection.Backward
                    : SlideDirection.Forward;

            gemMono.gem.grid.SlideColumnAt((int)gemMono.gem.position.x, slideDirection);
        }
    }
}
