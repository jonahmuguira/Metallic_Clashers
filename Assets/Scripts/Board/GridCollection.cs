namespace Board
{
    using System;
    using System.Collections.Generic;

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

        public abstract List<Gem> gemList { get; }

        public abstract void Slide(SlideDirection direction);
    }

    [Serializable]
    public class Column : GridCollection
    {
        public override List<Gem> gemList
        {
            get
            {
                //TODO: Return list of gems in this column
                return new List<Gem>();
            }
        }

        public override void Slide(SlideDirection direction)
        {
            // TODO: Slide the column in the specified direction
        }
    }

    [Serializable]
    public class Row : GridCollection
    {
        public override List<Gem> gemList
        {
            get
            {
                //TODO: Return list of gems in this row
                return new List<Gem>();
            }
        }

        public override void Slide(SlideDirection direction)
        {
            // TODO: Slide the row in the specified direction
        }
    }
}
