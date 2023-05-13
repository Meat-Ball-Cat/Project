using System;
using System.Collections.Generic;
using System.Linq;
using Model.MovingObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Assets.Scripts.Model.Levels
{
    public class LayerManager : MonoBehaviour
    {
        [FormerlySerializedAs("levelDepthDifference")] [SerializeField] private float layersDepthDifference = 2;
        [FormerlySerializedAs("defaultLevelsNumber")] [SerializeField] private int layersCount = 4;
        private readonly List<Layer> _layers = new();
        private Layer _currentLayer;

        internal Layer CurrentLayer { 
            get => _currentLayer;
            set
            {
                if (!_layers.Contains(value))
                    throw new ArgumentException();
                _currentLayer = value;

                var offLayers = _layers.TakeWhile(layer => layer != CurrentLayer);
                foreach (var layer in offLayers)
                foreach (var obj in layer.GetObjects())
                    obj.SetActive(false);

                var onLayers = _layers.SkipWhile(layer => layer != CurrentLayer);
                foreach (var layer in onLayers)
                foreach (var obj in layer.GetObjects())
                    obj.SetActive(true);
            }
        }

        public void SetCurrentLayer(int layerId)
        {
            if (!TryGetLayer(layerId, out var layer))
                throw new ArgumentException();
            CurrentLayer = layer;
        }

        [SerializeField] private GameObject[] _maps;

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

        public void GenerateLayers()
        {
            var layers = new GameObject("Layers");
            layers.AddComponent<Grid>();
            for (var i = 0; i < layersCount; i++)
            {
                var newLayer = new Layer(i  + 10);
                _layers.Add(newLayer);
                if (_maps == null || _maps.Length < i) continue;

                if (i > 0)
                {
                    var map = Instantiate(_maps[i - 1]);
                    map.transform.SetParent(layers.transform);
                    AddObject(map, newLayer);
                }
            }
        }

        private void Awake()
        {
            gameObject.tag = "LayerManager";
            GenerateLayers();
            _currentLayer = _layers.First();
        }

        public void AddObject(GameObject obj, int layerId)
        {
            if (TryGetLayer(layerId, out var layer))
            {
                AddObject(obj, layer);
            }
        }

        private void AddObject(GameObject obj, Layer layer)
        {
            layer.AddObject(obj);
            ChangeLayer(obj, layer.LayerId);
            obj.transform.position = new Vector3(
                obj.transform.position.x,
                obj.transform.position.y,
                GetLayerDepth(layer.LayerId));
        }

        public virtual void AddObject(GameObject obj)
        {
            AddObject(obj, _currentLayer);
        }

        public bool ChangeObjectLayer(MovingObject obj, int newLayerId)
        {
            if (!TryGetLayer(newLayerId, out var newLayer))
                return false;

            if (!TryGetLayer(obj.gameObject.layer, out var oldLayer) && !oldLayer.ContainsObject(obj.gameObject))
                throw new ArgumentException($"No such object found.");

            oldLayer.RemoveObject(obj.gameObject);
            ChangeLayer(obj.gameObject, newLayer.LayerId);
            newLayer.AddObject(obj.gameObject);
            obj.ChangeDepth();

            return true;
        }

        public bool LayerExists(int index)
            => index >= 0 && index < _layers.Count;

        public float GetLayerDepth(int layerIndex)
            => (layerIndex - 10) * layersDepthDifference;

        private bool TryGetLayer(int layerId, out Layer layer)
        {
            layer = default;

            var layerNumber = layerId - 10;
            if (layerNumber < 0 || layerNumber >= _layers.Count)
                return false;
            layer = _layers[layerNumber];
            return true;
        }

        public static void ChangeLayer(GameObject obj, int layerId)
        {
            var objects = new Queue<Transform>();
            objects.Enqueue(obj.transform);

            while (objects.Count > 0)
            {
                var currentObj = objects.Dequeue();
                currentObj.gameObject.layer = layerId;
                foreach (Transform child in currentObj.transform)
                    objects.Enqueue(child);
            }
        }
    }
}
