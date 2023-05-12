using System;
using UnityEngine;

namespace Model.Levels
{
    public abstract class Layered : MonoBehaviour
    {
        private int _currentLayer;
        
        public int CurrentLayer
        {
            get
                => _currentLayer;
            set
            {
                if (!LayerManager.Instance.LayerExists(value))
                    throw new ArgumentOutOfRangeException(nameof(value));
                
                LayerManager.Instance.ChangeObjectLayer(this, value);
                _currentLayer = value;
            }
        }

        public float CurrentTargetDepth
            => LayerManager.Instance.GetLayerDepth(CurrentLayer);

        public void DescendOneLayer()
        {
            if (CurrentLayer + 1 > LayerManager.Instance.MaxLayerCount)
                return;
            
            if (!LayerManager.Instance.LayerExists(CurrentLayer + 1))
                LayerManager.Instance.GenerateLayers();
            CurrentLayer++;
        }

        public void AscendOneLayer()
        {
            if (CurrentLayer <= 0)
                return;

            CurrentLayer--;
        }
    }
}