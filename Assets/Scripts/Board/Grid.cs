namespace Board
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using UnityEngine;
    using UnityEngine.Events;

    using Information;

    using Random = UnityEngine.Random;

    [Serializable]
    public class OnGridChange : UnityEvent<GridChangeInformation> { }
    [Serializable]
    public class OnMatch : UnityEvent<MatchInformation> { }

    [Serializable]
    public class OnSlide : UnityEvent<SlideInformation> { }

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

        [SerializeField]
        private OnSlide m_OnSlide = new OnSlide();

        public List<List<Gem>> gemList { get { return m_GemList; } }

        public Vector2 size { get { return m_Size; } private set { m_Size = value; } }

        public OnMatch onMatch { get { return m_OnMatch; } }
        public OnGridChange onGridChange { get { return m_OnGridChange; } }

        public OnSlide onSlide { get { return m_OnSlide; } }

        private Grid() { }
        public Grid(Vector2 newSize) : this()
        {
            var timeSpan = new TimeSpan(DateTime.Now.Ticks);
            Random.InitState((int)timeSpan.TotalMilliseconds);

            var numGemTypes = Enum.GetValues(typeof(GemType)).Length;

            m_Size = newSize;

            for (var y = 0; y < m_Size.y; ++y)
            {
                var newRow = new List<Gem>();
                for (var x = 0; x < m_Size.x; ++x)
                {
                    var gemType = (GemType)Random.Range(0, numGemTypes);

                    newRow.Add(
                        GemMono.Create(
                            this, gemType, new Vector2(x, y)).gem);
                }
                m_GemList.Add(newRow);
            }
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
            var foundIndex = m_GemList.FindIndex(gems => gems.Contains(gem));

            // If a match was not found
            if (foundIndex == -1)
                return false;

            // Removed the gem from the list which contains it
            m_GemList[foundIndex].Remove(gem);

            onGridChange.Invoke(new GridChangeInformation { gems = new List<Gem> { gem } });
            return true;
        }
        public bool RemoveAt(Vector2 position)
        {
            var y = (int)position.y;

            if (y >= m_GemList.Count || y < 0)
                return false;

            var x = (int)position.x;

            if (x >= m_GemList[y].Count || x < 0)
                return false;

            // Store reference to removed gem
            var gem = m_GemList[y][x];

            m_GemList[y].RemoveAt(x);

            // Use that reference when you invoke the onGridChange event
            onGridChange.Invoke(new GridChangeInformation { gems = new List<Gem> { gem } });
            return true;
        }

        public bool Swap(Gem oldGem, Gem newGem)
        {
            var foundY = m_GemList.FindIndex(gems => gems.Contains(oldGem));

            // If a match was not found
            if (foundY == -1)
                return false;

            var foundX = m_GemList[foundY].FindIndex(gem => gem == oldGem);

            m_GemList[foundY][foundX] = newGem;

            onGridChange.Invoke(new GridChangeInformation { gems = new List<Gem> { oldGem, newGem } });
            return true;
        }
        public bool SwapAt(Vector2 position1, Vector2 position2)
        {
            var y1 = (int)position1.y;
            var y2 = (int)position2.y;

            if (y1 >= m_GemList.Count || y1 < 0 ||
                y2 >= m_GemList.Count || y2 < 0)
                return false;

            var x1 = (int)position1.x;
            var x2 = (int)position2.x;

            if (x1 >= m_GemList[y1].Count || x1 < 0 ||
                x2 >= m_GemList[y2].Count || x2 < 0)
                return false;

            // Store a reference to the gems about to be swapped
            var gem1 = m_GemList[y1][x1];
            var gem2 = m_GemList[y2][x2];

            // Swap them
            m_GemList[y1][x1] = gem2;
            m_GemList[y2][x2] = gem1;

            // Use the references of each gem that was changed when invoking the onGridChange event
            onGridChange.Invoke(new GridChangeInformation { gems = new List<Gem> { gem1, gem2 } });
            return true;
        }

        public bool SlideRowAt(int index, SlideDirection direction)
        {
            if (index >= m_Rows.Count || index < 0)
                return false;

            m_Rows[index].Slide(direction);
            return true;
        }
        public bool SlideColumnAt(int index, SlideDirection direction)
        {
            if (index >= m_Columns.Count || index < 0)
                return false;

            m_Columns[index].Slide(direction);
            return true;
        }

        private void OnGemTypeChange(TypeChangeInformation typeChangeInfo)
        {
            //TODO: Check for matches on gems in the same row and column
        }
    }
}
