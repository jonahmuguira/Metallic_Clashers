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

public class CombatManager : SubManager<CombatManager>
{
    [SerializeField]
    private Canvas m_Canvas;
    [SerializeField]
    private RectTransform m_GridParentRectTransform;
    [SerializeField]
    private List<Sprite> m_GemSprites = new List<Sprite>();

    [Space, SerializeField]
    private UnityEvent m_OnCombatBegin = new UnityEvent();
    [SerializeField]
    private UnityEvent m_OnCombatUpdate = new UnityEvent();
    [SerializeField]
    private UnityEvent m_OnCombatEnd = new UnityEvent();

    [SerializeField]
    private UnityEvent m_OnPlayerTurn = new UnityEvent();

    [SerializeField]
    private GridMono m_GridMono;

    private GridCollectionMono m_LockedGridCollectionMono;

    public Canvas canvas { get { return m_Canvas; } }
    public RectTransform gridParentRectTransform { get { return m_GridParentRectTransform; } }

    public List<Sprite> gemSprites { get { return m_GemSprites; } }

    //TODO: public List<Enemy> enemies = new List<>;

    public UnityEvent onCombatBegin { get { return m_OnCombatBegin; } }
    public UnityEvent onCombatUpdate { get { return m_OnCombatUpdate; } }
    public UnityEvent onCombatEnd { get { return m_OnCombatEnd; } }

    public UnityEvent onPlayerTurn { get { return m_OnPlayerTurn; } }

    public GridMono gridMono { get { return m_GridMono; } }

    protected override void Init()
    {
        if (m_Canvas == null)
            m_Canvas = FindObjectOfType<Canvas>();

        //TODO: Initialize Combat
        if (m_GridParentRectTransform == null)
            m_GridParentRectTransform = m_Canvas.GetComponent<RectTransform>();

        GridMono.Init();
        GridCollectionMono.Init();

        GemMono.Init();

        var newGrid = new Grid(new Vector2(5f, 5f));

        m_GridMono = newGrid.GetComponent<GridMono>();

        m_GridMono.grid.onSlide.AddListener(OnSlide);
    }

    private void Update()
    {
        onCombatUpdate.Invoke();

        //if (m_LockedGridCollectionMono == null)
        //    return;

        //foreach (var gem in m_LockedGridCollectionMono.gridCollection.gems)
        //{
        //    var gemMono = gem.GetComponent<GemMono>();

        //    gemMono.positionOffset += new Vector2(0.5f, 0f);
        //}
    }

    private void OnSlide(SlideInformation slideInfo)
    {
        onPlayerTurn.Invoke();
    }

    protected override void OnBeginDrag(DragInformation dragInfo)
    {
        var hitMono = RaycastToGemMono(dragInfo.origin);
        // If we didn't hit a GemMono first
        if (hitMono == null)
            return;

        m_LockedGridCollectionMono =
            Mathf.Abs(dragInfo.totalDelta.x) > Mathf.Abs(dragInfo.totalDelta.y) ?
            hitMono.rowMono :
            hitMono.columnMono;

        OnDrag(dragInfo);
        //foreach (var gem in m_LockedGridCollectionMono.gridCollection.gems)
        //{
        //    var gemMono = gem.GetComponent<GemMono>();

        //    gemMono.positionOffset +=
        //        new Vector3(
        //            dragInfo.delta.x / m_Canvas.GetComponent<RectTransform>().lossyScale.x,
        //            dragInfo.delta.y / m_Canvas.GetComponent<RectTransform>().lossyScale.y);
        //}
    }

    protected override void OnDrag(DragInformation dragInfo)
    {
        // If we didn't hit a GridCollectionMono at the start of the drag
        if (m_LockedGridCollectionMono == null)
            return;

        foreach (var gem in m_LockedGridCollectionMono.gridCollection.gems)
        {
            var gemMono = gem.GetComponent<GemMono>();

            gemMono.positionOffset +=
                new Vector2(
                    m_LockedGridCollectionMono.gridCollection is Row
                    ? dragInfo.delta.x / m_Canvas.GetComponent<RectTransform>().lossyScale.x : 0f,

                    m_LockedGridCollectionMono.gridCollection is Column
                    ? dragInfo.delta.y / m_Canvas.GetComponent<RectTransform>().lossyScale.y : 0f);
        }
    }

    protected override void OnEndDrag(DragInformation dragInfo)
    {
        // If we didn't hit a GemMono at the start of the drag
        //if (m_LockedGridCollectionMono == null)
        //    return;

        //foreach (var gem in m_LockedGridCollectionMono.gridCollection.gems)
        //{
        //    var gemMono = gem.GetComponent<GemMono>();

        //    gemMono.positionOffset = Vector3.zero;
        //}

        //if (Mathf.Abs(dragInfo.end.x - dragInfo.origin.x) >
        //    Mathf.Abs(dragInfo.end.y - dragInfo.origin.y))
        //{
        //    var slideDirection =
        //        dragInfo.end.x - dragInfo.origin.x > 0
        //            ? SlideDirection.Backward
        //            : SlideDirection.Forward;

        //    m_LockedGridCollectionMono.gridCollection.Slide(slideDirection);
        //}
        //else
        //{
        //    var slideDirection =
        //        dragInfo.end.y - dragInfo.origin.y > 0
        //            ? SlideDirection.Backward
        //            : SlideDirection.Forward;

        //    m_LockedGridCollectionMono.gridCollection.Slide(slideDirection);
        //}
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
