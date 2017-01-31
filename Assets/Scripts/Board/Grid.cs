namespace Board
{
    using System;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.Events;

    using Information;

    [Serializable]
    public class OnGridChange : UnityEvent<GridChangeInformation> { }
    [Serializable]
    public class OnMatch : UnityEvent<MatchInformation> { }

    [Serializable]
    public class Grid
    {
        [SerializeField]
        private readonly List<List<Gem>> m_GemList = new List<List<Gem>>();

        [SerializeField]
        private Vector2 m_Size;

        [SerializeField]
        private List<Column> m_Columns = new List<Column>();
        [SerializeField]
        private List<Row> m_Rows = new List<Row>();

        [SerializeField]
        private OnMatch m_OnMatch = new OnMatch();
        [SerializeField]
        private OnGridChange m_OnGridChange = new OnGridChange();

        public List<List<Gem>> gemList { get { return m_GemList; } }

        public Vector2 size { get { return m_Size; } private set { m_Size = value; } }

        public OnMatch onMatch { get { return m_OnMatch; } }
        public OnGridChange onGridChange { get { return m_OnGridChange; } }

        private Grid() { }
        public Grid(Vector2 newSize)
        {
            //TODO: Create grid of specified size
        }

        private void CheckMatch()
        {
            //TODO: Check for matches in the entire grid
        }

        private bool Add()
        {
            //TODO: Return if successful
            return false;
        }

        public bool Remove(Gem gem)
        {
            //TODO: Return if successful
            return false;
        }
        public bool RemoveAt(Vector2 position)
        {
            //TODO: Return if successful
            return false;
        }

        public bool Swap(Gem oldGem, Gem newGem)
        {
            //TODO: Return if successful
            return false;
        }
        public bool SwapAt(Vector2 position1, Vector2 position2)
        {
            //TODO: Return if successful
            return false;
        }

        private void OnGemTypeChange(TypeChangeInformation typeChangeInfo)
        {
            //TODO: Check for matches on gems in the same row and column
        }
    }
}
