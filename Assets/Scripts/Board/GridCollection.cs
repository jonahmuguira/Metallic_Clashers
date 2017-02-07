namespace Board
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Information;

    using UnityEngine;

    public enum SlideDirection
    {
        Forward,
        Backward,
    }

    [Serializable]
    public abstract class GridCollection
    {
        [SerializeField]
        protected int m_Index;

        [NonSerialized]
        protected Grid m_Grid;

        public Grid grid { get { return m_Grid; } set { m_Grid = value; } }
        public int index { get { return m_Index; } set { m_Index = value; } }

        public abstract IEnumerable<Gem> gems { get; }

        protected abstract bool CopyAt(List<List<Gem>> tempList, int fromIndex, int toIndex);

        public bool Slide(SlideDirection direction)
        {
            var tempList = m_Grid.gemLists.Select(gemList => gemList.gems.ToList()).ToList();

            var listCount = gems.Count();
            for (var i = 0; i < listCount; i++)
            {
                int nextIndex;
                switch (direction)
                {
                case SlideDirection.Forward:
                    nextIndex = i - 1;
                    if (nextIndex < 0)
                        nextIndex = listCount - 1;
                    break;
                case SlideDirection.Backward:
                    nextIndex = i + 1;
                    if (nextIndex > listCount - 1)
                        nextIndex = 0;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("direction", direction, null);
                }

                var result = CopyAt(tempList, i, nextIndex);
                if (!result)
                    return false;
            }

            m_Grid.onSlide.Invoke(new SlideInformation { gridCollection = this });
            return true;
        }
    }

    [Serializable]
    public class Column : GridCollection
    {
        public override IEnumerable<Gem> gems
        {
            get { return m_Grid.gemLists.Select(gemList => gemList[index]); }
        }

        protected override bool CopyAt(List<List<Gem>> tempList, int fromIndex, int toIndex)
        {
            var listCount = m_Grid.gemLists[0].gems.Count;
            if (fromIndex >= listCount || fromIndex < 0 ||
                toIndex >= listCount || toIndex < 0)
                return false;

            m_Grid.gemLists[toIndex][index] = tempList[fromIndex][index];

            m_Grid.gemLists[toIndex][index].position = new Vector2(index, toIndex);
            return true;
        }
    }

    [Serializable]
    public class Row : GridCollection
    {
        public override IEnumerable<Gem> gems
        {
            get { return m_Grid.gemLists[index].gems; }
        }

        protected override bool CopyAt(List<List<Gem>> tempList, int fromIndex, int toIndex)
        {
            var listCount = m_Grid.gemLists.Count;
            if (fromIndex >= listCount || fromIndex < 0 ||
                toIndex >= listCount || toIndex < 0)
                return false;

            m_Grid.gemLists[index][toIndex] = tempList[index][fromIndex];

            m_Grid.gemLists[index][toIndex].position = new Vector2(toIndex, index);
            return true;
        }
    }
}
