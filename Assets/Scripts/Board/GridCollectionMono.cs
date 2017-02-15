using UnityEngine;

namespace Board
{
    using System.Linq;

    [RequireComponent(typeof(RectTransform))]
    public class GridCollectionMono : MonoBehaviour, IComponent
    {
        [SerializeField]
        private GridCollection m_GridCollection;

        public GridCollection gridCollection { get { return m_GridCollection; } }

        public static void Init()
        {
            GridCollection.onCreate.AddListener(OnCreateGridCollection);
        }

        private static void OnCreateGridCollection(GridCollection newGridCollection)
        {
            var gridMono =
                newGridCollection.grid.components.First(component => component is GridMono) as GridMono;

            if (gridMono == null)
                return;

            var newGameObject = new GameObject(newGridCollection.GetType() + " " + newGridCollection.index);
            newGameObject.transform.SetParent(gridMono.transform, false);

            var newGridCollectionMono = newGameObject.AddComponent<GridCollectionMono>();

            newGridCollectionMono.m_GridCollection = newGridCollection;
            newGridCollection.components.Add(newGridCollectionMono);
        }
    }
}
