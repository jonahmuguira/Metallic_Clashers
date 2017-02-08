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
    public class GemMonoList
    {
        public List<GemMono> gemMonos = new List<GemMono>();

        public GemMono this[int index]
        {
            get { return gemMonos[index]; }
            set { gemMonos[index] = value; }
        }

        public static implicit operator List<GemMono>(GemMonoList gemMonoList)
        {
            return gemMonoList.gemMonos.ToList();
        }
        public static implicit operator GemMonoList(List<GemMono> gemMonoList)
        {
            return new GemMonoList { gemMonos = gemMonoList.ToList() };
        }
    }

    [Serializable]
    public class Grid
    {
        [SerializeField]
        private GridMono m_GridMono;

        [SerializeField]
        private List<GemMonoList> m_GemMonoLists = new List<GemMonoList>();

        [SerializeField]
        private Vector2 m_Size;

        [SerializeField]
        private List<GridCollectionMono> m_Columns = new List<GridCollectionMono>();
        [SerializeField]
        private List<GridCollectionMono> m_Rows = new List<GridCollectionMono>();

        [SerializeField]
        private OnMatch m_OnMatch = new OnMatch();
        [SerializeField]
        private OnGridChange m_OnGridChange = new OnGridChange();

        [SerializeField]
        private OnSlide m_OnSlide = new OnSlide();

        public GridMono gridMono { get { return m_GridMono; } }

        public List<GemMonoList> gemMonoLists { get { return m_GemMonoLists; } }

        public Vector2 size { get { return m_Size; } }

        public List<GridCollectionMono> columns { get { return m_Columns; } }
        public List<GridCollectionMono> rows { get { return m_Rows; } }

        public OnMatch onMatch { get { return m_OnMatch; } }
        public OnGridChange onGridChange { get { return m_OnGridChange; } }

        public OnSlide onSlide { get { return m_OnSlide; } }

        private Grid() { }
        public Grid(GridMono parentGridMono, Vector2 newSize) : this()
        {
            Random.InitState((int)DateTime.Now.Ticks);

            m_Size = newSize;

            m_GridMono = parentGridMono;
        }

        public void InitializeGrid()
        {
            var numGemTypes = Enum.GetValues(typeof(GemType)).Length;

            for (var y = 0; y < m_Size.y; ++y)
            {
                m_Rows.Add(GridCollectionMono.Create<Row>(m_GridMono, y));

                var newList = new List<GemMono>();
                for (var x = 0; x < m_Size.x; ++x)
                {
                    if (x == 0)
                        m_Columns.Add(GridCollectionMono.Create<Column>(m_GridMono, y));

                    var gemType = (GemType)Random.Range(0, numGemTypes);

                    newList.Add(
                        GemMono.Create(
                            m_GridMono, gemType, new Vector2(x, y)));
                }
                m_GemMonoLists.Add(newList);
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

        public bool Remove(GemMono gemMono)
        {
            var foundIndex = m_GemMonoLists.FindIndex(gemList => gemList.gemMonos.Contains(gemMono));

            // If a match was not found
            if (foundIndex == -1)
                return false;

            // Removed the gem from the list which contains it
            m_GemMonoLists[foundIndex].gemMonos.Remove(gemMono);

            onGridChange.Invoke(new GridChangeInformation { gemMonos = new List<GemMono> { gemMono } });
            return true;
        }
        public bool RemoveAt(Vector2 position)
        {
            var y = (int)position.y;

            if (y >= m_GemMonoLists.Count || y < 0)
                return false;

            var x = (int)position.x;

            if (x >= m_GemMonoLists[y].gemMonos.Count || x < 0)
                return false;

            // Store reference to removed gem
            var gemMono = m_GemMonoLists[y][x];

            m_GemMonoLists[y].gemMonos.RemoveAt(x);

            // Use that reference when you invoke the onGridChange event
            onGridChange.Invoke(new GridChangeInformation { gemMonos = new List<GemMono> { gemMono } });
            return true;
        }

        public bool Swap(GemMono oldGemMono, GemMono newGemMono)
        {
            var foundY = m_GemMonoLists.FindIndex(gemList => gemList.gemMonos.Contains(oldGemMono));

            // If a match was not found
            if (foundY == -1)
                return false;

            var foundX = m_GemMonoLists[foundY].gemMonos.FindIndex(gem => gem == oldGemMono);

            m_GemMonoLists[foundY][foundX] = newGemMono;

            onGridChange.Invoke(
                new GridChangeInformation { gemMonos = new List<GemMono> { oldGemMono, newGemMono } });
            return true;
        }
        public bool SwapAt(Vector2 position1, Vector2 position2)
        {
            var y1 = (int)position1.y;
            var y2 = (int)position2.y;

            if (y1 >= m_GemMonoLists.Count || y1 < 0 ||
                y2 >= m_GemMonoLists.Count || y2 < 0)
                return false;

            var x1 = (int)position1.x;
            var x2 = (int)position2.x;

            if (x1 >= m_GemMonoLists[y1].gemMonos.Count || x1 < 0 ||
                x2 >= m_GemMonoLists[y2].gemMonos.Count || x2 < 0)
                return false;

            // Store a reference to the gems about to be swapped
            var gemMono1 = m_GemMonoLists[y1][x1];
            var gemMono2 = m_GemMonoLists[y2][x2];

            // Swap them
            m_GemMonoLists[y1][x1] = gemMono2;
            m_GemMonoLists[y2][x2] = gemMono1;

            // Use the references of each gem that was changed when invoking the onGridChange event
            onGridChange.Invoke(
                new GridChangeInformation { gemMonos = new List<GemMono> { gemMono1, gemMono2 } });
            return true;
        }

        public bool SlideRowAt(int index, SlideDirection direction)
        {
            if (index >= m_Rows.Count || index < 0)
                return false;

            return m_Rows[index].gridCollection.Slide(direction);
        }
        public bool SlideColumnAt(int index, SlideDirection direction)
        {
            if (index >= m_Columns.Count || index < 0)
                return false;

            return m_Columns[index].gridCollection.Slide(direction);
        }

        private void OnGemTypeChange(TypeChangeInformation typeChangeInfo)
        {
            //TODO: Check for matches on gems in the same row and column
        }
    }
}
