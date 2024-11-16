using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scriptable
{
    
    [CreateAssetMenu(fileName = "NewSceneData", menuName = "Scriptable/Thema/SceneData", order = 1)]
    
    public class SceneData : SerializedScriptableObject
    {
        
        public Dictionary<int, CheckpointData[]> sceneCheckpointData = new Dictionary<int, CheckpointData[]>();
        
        public struct CheckpointData
        {
            public int checkpoint;
            public int[] requiredSubScenes;
        }
        
        
    }
}
