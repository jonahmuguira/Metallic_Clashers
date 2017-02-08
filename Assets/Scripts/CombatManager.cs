using System;
using System.Collections.Generic;
using System.Linq;

using Board;
using Board.Information;

using Input.Information;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

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
    private List<Sprite> m_GemSprites = new List<Sprite>();

    [SerializeField]
    private Vector2 m_GemOffset;

    [Space, SerializeField]
    private OnCombatBegin m_OnCombatBegin = new OnCombatBegin();
    [SerializeField]
    private OnCombatUpdate m_OnCombatUpdate = new OnCombatUpdate();
    [SerializeField]
    private OnCombatEnd m_OnCombatEnd = new OnCombatEnd();

    [SerializeField]
    private OnPlayerTurn m_OnPlayerTurn = new OnPlayerTurn();

    [SerializeField]
    private Grid m_Grid;

    private GemMono m_LockedGemMono;

    //TODO: public List<Enemy> enemies = new List<>;
    public List<Sprite> gemSprites { get { return m_GemSprites; } }

    public Vector3 gemOffset { get { return m_GemOffset; } set { m_GemOffset = value; } }

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

    protected override void OnBeginDrag(DragInformation dragInfo)
    {
        var gemMono = RaycastToGemMono(dragInfo.origin);
        // If we didn't hit a GemMono first
        if (gemMono == null)
            return;

        m_LockedGemMono = gemMono;
    }

    protected override void OnDrag(DragInformation dragInfo)
    {
        // If we didn't hit a GemMono at the start of the drag
        if (m_LockedGemMono == null)
            return;

        m_LockedGemMono.positionOffset += (Vector3)dragInfo.delta;
    }

    protected override void OnEndDrag(DragInformation dragInfo)
    {
        // If we didn't hit a GemMono at the start of the drag
        if (m_LockedGemMono == null)
            return;

        m_LockedGemMono.positionOffset = Vector3.zero;

        var gem = m_LockedGemMono.gem;
        if (Mathf.Abs(dragInfo.end.x - dragInfo.origin.x) >
            Mathf.Abs(dragInfo.end.y - dragInfo.origin.y))
        {
            var slideDirection =
                dragInfo.end.x - dragInfo.origin.x > 0
                    ? SlideDirection.Backward
                    : SlideDirection.Forward;

            gem.grid.SlideRowAt((int)gem.position.y, slideDirection);
        }
        else
        {
            var slideDirection =
                dragInfo.end.y - dragInfo.origin.y > 0
                    ? SlideDirection.Backward
                    : SlideDirection.Forward;

            gem.grid.SlideColumnAt((int)gem.position.x, slideDirection);
        }
    }

    [CanBeNull]
    private static GemMono RaycastToGemMono(Vector2 origin)
    {
        var pointerEventData =
            new PointerEventData(EventSystem.current) { position = origin };

        var hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, hits);

        // If nothing was hit
        if (hits.Count <= 0)
            return null;

        var firstHit = hits.First();

        // Return the first hit object's GemMono component
        // Will be null if one was not found on the game object
        return firstHit.gameObject.GetComponent<GemMono>();
    }

    [CanBeNull]
    private static Gem RaycastToGem(Vector2 origin)
    {
        var gemMono = RaycastToGemMono(origin);

        // If we didn't hit a GemMono first
        return gemMono ? gemMono.gem : null;
    }
}
