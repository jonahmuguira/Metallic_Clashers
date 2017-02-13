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
    public class GridChange : UnityEvent<GridChangeInformation> { }
    [Serializable]
    public class MatchEvent : UnityEvent<MatchInformation> { }

    [Serializable]
    public class SlideEvent : UnityEvent<SlideInformation> { }

    [SerializeField]
    public class CreateGridEvent : UnityEvent<Grid> { }

    [Serializable]
    public class GemList
    {
        public List<Gem> gems = new List<Gem>();

        public Gem this[int index]
        {
            get { return gems[index]; }
            set { gems[index] = value; }
        }

        public static implicit operator List<Gem>(GemList gemList)
        {
            return gemList.gems.ToList();
        }
        public static implicit operator GemList(List<Gem> gemMonoList)
        {
            return new GemList { gems = gemMonoList.ToList() };
        }
    }

    [Serializable]
    public class Grid
    {
        [SerializeField]
        private List<GemList> m_GemLists = new List<GemList>();

        [SerializeField]
        private Vector2 m_Size;

        [SerializeField]
        private List<Column> m_Columns = new List<Column>();
        [SerializeField]
        private List<Row> m_Rows = new List<Row>();

        [SerializeField]
        private MatchEvent m_OnMatch = new MatchEvent();
        [SerializeField]
        private GridChange m_OnGridChange = new GridChange();

        [SerializeField]
        private SlideEvent m_OnSlide = new SlideEvent();

        private readonly List<IComponent> m_Components = new List<IComponent>();

        public static readonly CreateGridEvent onCreate = new CreateGridEvent();

        public List<GemList> gemLists { get { return m_GemLists; } }

        public Vector2 size { get { return m_Size; } }

        public List<Column> columns { get { return m_Columns; } }
        public List<Row> rows { get { return m_Rows; } }

        public MatchEvent onMatch { get { return m_OnMatch; } }
        public GridChange onGridChange { get { return m_OnGridChange; } }

        public SlideEvent onSlide { get { return m_OnSlide; } }

        public List<IComponent> components { get { return m_Components; } }

        public GemList this[int index] { get { return m_GemLists[index]; } }

        private Grid() { }
        public Grid(Vector2 newSize) : this()
        {
            Random.InitState((int)DateTime.Now.Ticks);

            m_Size = newSize;

            onCreate.Invoke(this);

            var numGemTypes = Enum.GetValues(typeof(GemType)).Length;

            for (var y = 0; y < m_Size.y; ++y)
            {
                m_Rows.Add(new Row(this, y));

                var newList = new List<Gem>();
                for (var x = 0; x < m_Size.x; ++x)
                {
                    if (x == 0)
                        m_Columns.Add(new Column(this, y));

                    var gemType = (GemType)Random.Range(0, numGemTypes);

                    newList.Add(
                        new Gem(
                            this, new Vector2(x, y), gemType));
                }
                m_GemLists.Add(newList);
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
            var foundIndex = m_GemLists.FindIndex(gemList => gemList.gems.Contains(gem));

            // If a match was not found
            if (foundIndex == -1)
                return false;

            // Removed the gem from the list which contains it
            m_GemLists[foundIndex].gems.Remove(gem);

            onGridChange.Invoke(new GridChangeInformation { gems = new List<Gem> { gem } });
            return true;
        }
        public bool RemoveAt(Vector2 position)
        {
            var y = (int)position.y;

            if (y >= m_GemLists.Count || y < 0)
                return false;

            var x = (int)position.x;

            if (x >= m_GemLists[y].gems.Count || x < 0)
                return false;

            // Store reference to removed gem
            var gem = m_GemLists[y][x];

            m_GemLists[y].gems.RemoveAt(x);

            // Use that reference when you invoke the onGridChange event
            onGridChange.Invoke(new GridChangeInformation { gems = new List<Gem> { gem } });
            return true;
        }

        public bool Swap(Gem oldGem, Gem newGem)
        {
            var foundY = m_GemLists.FindIndex(gemList => gemList.gems.Contains(oldGem));

            // If a match was not found
            if (foundY == -1)
                return false;

            var foundX = m_GemLists[foundY].gems.FindIndex(gem => gem == oldGem);

            m_GemLists[foundY][foundX] = newGem;

            onGridChange.Invoke(
                new GridChangeInformation { gems = new List<Gem> { oldGem, newGem } });
            return true;
        }
        public bool SwapAt(Vector2 position1, Vector2 position2)
        {
            var y1 = (int)position1.y;
            var y2 = (int)position2.y;

            if (y1 >= m_GemLists.Count || y1 < 0 ||
                y2 >= m_GemLists.Count || y2 < 0)
                return false;

            var x1 = (int)position1.x;
            var x2 = (int)position2.x;

            if (x1 >= m_GemLists[y1].gems.Count || x1 < 0 ||
                x2 >= m_GemLists[y2].gems.Count || x2 < 0)
                return false;

            // Store a reference to the gems about to be swapped
            var gem1 = m_GemLists[y1][x1];
            var gem2 = m_GemLists[y2][x2];

            // Swap them
            m_GemLists[y1][x1] = gem2;
            m_GemLists[y2][x2] = gem1;

            // Use the references of each gem that was changed when invoking the onGridChange event
            onGridChange.Invoke(
                new GridChangeInformation { gems = new List<Gem> { gem1, gem2 } });
            return true;
        }

        public bool SlideRowAt(int index, SlideDirection direction)
        {
            if (index >= m_Rows.Count || index < 0)
                return false;

            return m_Rows[index].Slide(direction);
        }
        public bool SlideColumnAt(int index, SlideDirection direction)
        {
            if (index >= m_Columns.Count || index < 0)
                return false;

            return m_Columns[index].Slide(direction);
        }

        private void OnGemTypeChange(TypeChangeInformation typeChangeInfo)
        {
            //TODO: Check for matches on gems in the same row and column
        }

        public T GetComponent<T>() where T : IComponent
        {
            return (T)components.First(component => component is T);
        }
    }
}
