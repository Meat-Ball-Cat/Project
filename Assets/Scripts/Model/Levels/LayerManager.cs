using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Model.Levels
{
    public class LayerManager : MonoBehaviour
    {
        [FormerlySerializedAs("levelDepthDifference")] [SerializeField] private float layersDepthDifference = 20;
        [FormerlySerializedAs("defaultLevelsNumber")] [SerializeField] private int defaultLayersCount = 4;
        [SerializeField] private int maxLayerCount = 50;
        private readonly List<HashSet<Layered>> _layers = new();

        public int MaxLayerCount
            => maxLayerCount;

        public static LayerManager Instance
        {
            get
            {
                var lm = GameObject.FindWithTag("LayerManager").GetComponent<LayerManager>();
                if (lm is null)
                    throw new Exception("No LevelManager present on scene");

                return lm;
            }
        }

        public void GenerateLayers(int number = 1)
        {
            if (number <= 0)
                throw new ArgumentException($"Cannot generate non-positive [{number}] amount of layers.");
            if (number + _layers.Count > maxLayerCount)
                throw new 
                    ArgumentException($"Cannot generate another [{number}] layers, limit is [{maxLayerCount}]");

            for (var i = 0; i < number; i++)
                _layers.Add(new HashSet<Layered>());
        }

        private void Start()
        {
            gameObject.tag = "LayerManager";
            GenerateLayers(defaultLayersCount);
        }

        public void AddObject(Layered obj, int layer)
        {
            if (layer >= _layers.Count)
                GenerateLayers(defaultLayersCount);
            if (layer < 0 || layer >= _layers.Count)
                throw new ArgumentException($"Incorrect layer [{layer}].");

            _layers[layer].Add(obj);
        }

        public void ChangeObjectLayer(Layered obj, int newLayer)
        {
            if (newLayer < 0 || newLayer >= _layers.Count)
                throw new ArgumentException($"Incorrect layer [{newLayer}].");

            if (!_layers[obj.CurrentLayer].Contains(obj))
                throw new ArgumentException($"No such object found.");

            _layers[newLayer].Add(obj);
            _layers[obj.CurrentLayer].Remove(obj);
        }

        public bool LayerExists(int index)
            => index >= 0 && index < _layers.Count;

        public float GetLayerDepth(int layerIndex)
            => layerIndex * layersDepthDifference;
    }
}
