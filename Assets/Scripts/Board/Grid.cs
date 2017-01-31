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
        private OnMatch m_OnMatch = new OnMatch();
        [SerializeField]
        private OnGridChange m_OnGridChange = new OnGridChange();

        public List<List<Gem>> gemList { get { return m_GemList; } }

        public Vector2 size { get { return m_Size; } private set { m_Size = value; } }

        private void CheckMatch()
        {

        }

        private bool Add()
        {
            //TODO: Return if successful
            return false;
        }

        public bool RemoveAt(Vector2 position)
        {
            //TODO: Return if successful
            return false;
        }
        public bool Remove(Gem gem)
        {
            //TODO: Return if successful
            return false;
        }

        public bool Swap(Gem oldGem, Gem newGem)
        {
            //TODO: Return if successful
            return false;
        }


    }
}
