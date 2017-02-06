using System;
using System.Collections.Generic;

using Board;
using Board.Information;

using Input.Information;

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
        var ray = Camera.main.ScreenPointToRay(dragInfo.origin);
        //ray.origin = Camera.main.ScreenToWorldPoint(dragInfo.origin);

        Debug.DrawRay(ray.origin, ray.direction * 25f, Color.white, 2f);

        var hit = Physics2D.GetRayIntersection(ray);
        if (hit.collider)
        {
            var gemMono = hit.collider.GetComponent<GemMono>();
            if (gemMono)
            {
                if (Mathf.Abs(dragInfo.end.x - dragInfo.origin.x) > Mathf.Abs(dragInfo.end.y - dragInfo.origin.y))
                {
                    var slideDirection =
                        dragInfo.end.x - dragInfo.origin.x > 0 ?
                        SlideDirection.Backward : SlideDirection.Forward;

                    gemMono.gem.grid.SlideRowAt((int)gemMono.gem.position.y, slideDirection);
                }
                else
                {
                    var slideDirection =
                        dragInfo.end.y - dragInfo.origin.y > 0 ?
                        SlideDirection.Backward : SlideDirection.Forward;

                    gemMono.gem.grid.SlideColumnAt((int)gemMono.gem.position.x, slideDirection);
                }
            }
        }
    }
}
