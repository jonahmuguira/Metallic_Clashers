namespace StageSelection
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Tree : MonoBehaviour
    {
        public List<Node> nodes = new List<Node>();
        private void Awake()
        {
            foreach (MonoNode n in transform)
            {
                nodes.Add(n.node);
            }
        }
    }
}
