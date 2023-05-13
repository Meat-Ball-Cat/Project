using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Model.Levels
{
    internal class Layer
    {
        public readonly int LayerId;
        private readonly HashSet<GameObject> Objects;

        public Layer(int layerId)
        {
            LayerId = layerId;
            Objects = new HashSet<GameObject>();
        }

        public void AddObject(GameObject obj)
        {
            Objects.Add(obj);
        }

        public void RemoveObject(GameObject obj)
        {
            Objects.Remove(obj);
        }

        public bool ContainsObject(GameObject obj) => Objects.Contains(obj);
    }
}
