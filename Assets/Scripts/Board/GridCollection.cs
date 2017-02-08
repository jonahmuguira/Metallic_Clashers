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

        public abstract IEnumerable<GemMono> gemMonos { get; }

        protected abstract bool CopyAt(List<List<GemMono>> tempList, int fromIndex, int toIndex);

        public bool Slide(SlideDirection direction)
        {
            var tempList = m_Grid.gemMonoLists.Select(gemMonoList => gemMonoList.gemMonos.ToList()).ToList();

            var listCount = gemMonos.Count();
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
        public override IEnumerable<GemMono> gemMonos
        {
            get { return m_Grid.gemMonoLists.Select(gemMonoList => gemMonoList[index]); }
        }

        protected override bool CopyAt(List<List<GemMono>> tempList, int fromIndex, int toIndex)
        {
            var listCount = m_Grid.gemMonoLists[0].gemMonos.Count;
            if (fromIndex >= listCount || fromIndex < 0 ||
                toIndex >= listCount || toIndex < 0)
                return false;

            m_Grid.gemMonoLists[toIndex][index] = tempList[fromIndex][index];

            m_Grid.gemMonoLists[toIndex][index].gem.position = new Vector2(index, toIndex);
            return true;
        }
    }

    [Serializable]
    public class Row : GridCollection
    {
        public override IEnumerable<GemMono> gemMonos
        {
            get { return m_Grid.gemMonoLists[index].gemMonos; }
        }

        protected override bool CopyAt(List<List<GemMono>> tempList, int fromIndex, int toIndex)
        {
            var listCount = m_Grid.gemMonoLists.Count;
            if (fromIndex >= listCount || fromIndex < 0 ||
                toIndex >= listCount || toIndex < 0)
                return false;

            m_Grid.gemMonoLists[index][toIndex] = tempList[index][fromIndex];

            m_Grid.gemMonoLists[index][toIndex].gem.position = new Vector2(toIndex, index);
            return true;
        }
    }
}
