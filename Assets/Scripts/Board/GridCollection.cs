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

        public Grid grid { get; set; }
        public int index { get { return m_Index; } set { m_Index = value; } }

        public abstract IEnumerable<Gem> gems { get; }

        protected abstract bool CopyAt(List<List<Gem>> tempList, int fromIndex, int toIndex);

        public bool Slide(SlideDirection direction)
        {
            var tempList = grid.gemLists.Select(gemList => gemList.ToList()).ToList();

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

            grid.onSlide.Invoke(new SlideInformation { gridCollection = this });
            return true;
        }
    }

    [Serializable]
    public class Column : GridCollection
    {
        public override IEnumerable<Gem> gems
        {
            get { return grid.gemLists.Select(gemList => gemList[index]); }
        }

        protected override bool CopyAt(List<List<Gem>> tempList, int fromIndex, int toIndex)
        {
            var listCount = grid.gemLists[0].Count;
            if (fromIndex >= listCount || fromIndex < 0 ||
                toIndex >= listCount || toIndex < 0)
                return false;

            grid.gemLists[toIndex][index] = tempList[fromIndex][index];

            grid.gemLists[toIndex][index].position = new Vector2(index, toIndex);
            return true;
        }
    }

    [Serializable]
    public class Row : GridCollection
    {
        public override IEnumerable<Gem> gems
        {
            get { return grid.gemLists[index]; }
        }

        protected override bool CopyAt(List<List<Gem>> tempList, int fromIndex, int toIndex)
        {
            var listCount = grid.gemLists.Count;
            if (fromIndex >= listCount || fromIndex < 0 ||
                toIndex >= listCount || toIndex < 0)
                return false;

            grid.gemLists[index][toIndex] = tempList[index][fromIndex];

            grid.gemLists[index][toIndex].position = new Vector2(toIndex, index);
            return true;
        }
    }
}
