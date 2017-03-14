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
using UnityEngine.UI;

using Input = UnityEngine.Input;

public class CombatManager : SubManager<CombatManager>
{
    [Serializable]
    public class GemMonoInformation
    {
        public Sprite backgroundImage;
        public Sprite midgroundImage;
        public Sprite foregroundImage;

        public List<Color> colors = new List<Color>();
    }

    [SerializeField]
    private Canvas m_Canvas;
    [SerializeField]
    private RectTransform m_GridParentRectTransform;
    [SerializeField]
    private VerticalLayoutGroup m_RowParent;
    [SerializeField]
    private HorizontalLayoutGroup m_ColumnParent;

    [SerializeField]
    private GemMonoInformation m_GemMonoInformation;

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

    private bool m_IsPaused;

    private bool m_HasSlid;

    public Canvas canvas { get { return m_Canvas; } }
    public RectTransform gridParentRectTransform { get { return m_GridParentRectTransform; } }
    public VerticalLayoutGroup rowParent { get { return m_RowParent; } }
    public HorizontalLayoutGroup columnParent { get { return m_ColumnParent; } }

    public GemMonoInformation gemMonoInformation { get { return m_GemMonoInformation; } }

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
        if (UnityEngine.Input.GetKeyDown(KeyCode.P))
            m_IsPaused = !m_IsPaused;

        if (!m_IsPaused)
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
        m_HasSlid = true;
    }

    protected override void OnBeginDrag(DragInformation dragInfo)
    {
        var hitMonos = RayCastToGridCollectionMono(dragInfo.origin).ToList();

        // If we didn't hit a GemMono first
        if (hitMonos.Count == 0)
        {
            m_LockedGridCollectionMono = null;
            return;
        }

        m_LockedGridCollectionMono =
            hitMonos.First(
                hitMono =>
                    Mathf.Abs(dragInfo.totalDelta.x) > Mathf.Abs(dragInfo.totalDelta.y)
                        ? hitMono.gridCollection is Row : hitMono.gridCollection is Column);
    }

    protected override void OnDrag(DragInformation dragInfo)
    {
        // If we didn't hit a GridCollectionMono at the start of the drag
        if (m_LockedGridCollectionMono == null)
            return;

        var addedPositionOffset =
            new Vector2(
                m_LockedGridCollectionMono.gridCollection is Row
                ? dragInfo.delta.x / m_Canvas.scaleFactor : 0f,

                m_LockedGridCollectionMono.gridCollection is Column
                ? dragInfo.delta.y / m_Canvas.scaleFactor : 0f);

        m_LockedGridCollectionMono.positionOffset += addedPositionOffset;
    }

    protected override void OnEndDrag(DragInformation dragInfo)
    {
        if (!m_HasSlid)
            return;

        onPlayerTurn.Invoke();
        Debug.Log("Player Turn");

        m_HasSlid = false;
    }

    private static IEnumerable<GridCollectionMono> RayCastToGridCollectionMono(Vector2 origin)
    {
        var pointerEventData =
            new PointerEventData(EventSystem.current) { position = origin };

        var hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, hits);

        // If nothing was hit
        if (hits.Count <= 0)
            return null;

        // Return the first hit object's GridCollectionMono component
        // Will be null if one was not found on the game object
        return
            hits.
                Where(
                    hit => hit.gameObject.GetComponent<GridCollectionMono>() != null).
                        Select(
                            hit => hit.gameObject.GetComponent<GridCollectionMono>());
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

    [ContextMenu("Test Match")]
    public void TestMatch()
    {
        m_GridMono.grid.CheckMatch();
    }
}
