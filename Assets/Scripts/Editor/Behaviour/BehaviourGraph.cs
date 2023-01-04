using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using GraphProcessor;

namespace Behaviour
{
    [CreateAssetMenu(fileName = "BehaviourGraph", menuName = "FishyTown/Behaviour Graph", order = 1)]
    public class BehaviourGraph : BaseGraph
    {
        [OnOpenAsset(0)]
        public static bool OnBaseGraphOpened(int instanceID, int line)
        {
            var asset = EditorUtility.InstanceIDToObject(instanceID) as BehaviourGraph;
            if (!asset)
                return false;

            EditorWindow.GetWindow<BehaviourGraphWindow>().InitializeGraph(asset);
            return true;
        }
    }
}
