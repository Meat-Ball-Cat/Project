using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model.Levels
{
    public class Layer
    {
        public readonly int LayerId;
        private readonly HashSet<GameObject> _objects;

        public Layer(int layerId)
        {
            LayerId = layerId;
            _objects = new HashSet<GameObject>();
        }

        public void AddObject(GameObject obj)
        {
            _objects.Add(obj);
        }

        public void RemoveObject(GameObject obj)
        {
            _objects.Remove(obj);
        }

        public bool ContainsObject(GameObject obj)
            => _objects.Contains(obj);

        public IEnumerable<GameObject> GetObjects()
            => _objects;
    }
}
