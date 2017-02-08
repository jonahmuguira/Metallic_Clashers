using UnityEngine;

namespace Board
{
    [RequireComponent(typeof(RectTransform))]
    public class GridCollectionMono : MonoBehaviour
    {
        [SerializeField]
        private GridCollection m_GridCollection;

        public GridCollection gridCollection { get { return m_GridCollection; } }

        public static GridCollectionMono Create<T>(GridMono gridMono, int index)
            where T : GridCollection, new()
        {
            var newGameObject = new GameObject(typeof(T) + " " + index);
            newGameObject.transform.SetParent(gridMono.transform);

            var newGridCollectionMono = newGameObject.AddComponent<GridCollectionMono>();
            newGridCollectionMono.m_GridCollection = new T { gridMono = gridMono, index = index };

            return newGridCollectionMono;
        }
    }
}
