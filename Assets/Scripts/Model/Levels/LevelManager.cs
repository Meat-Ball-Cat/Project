using System.Collections.Generic;
using UnityEngine;

namespace Model.Levels
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] levels;

        public bool TryGetLevelDepth(int levelIndex, out float depthZ)
        {
            if (levelIndex < 0 || levelIndex >= levels.Length)
            {
                depthZ = 0;
                return false;
            }

            var level = levels[levelIndex];
            depthZ = level.transform.position.z;
            return true;
        }
    }
}