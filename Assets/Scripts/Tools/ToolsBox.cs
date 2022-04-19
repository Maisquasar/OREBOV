using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    public class ToolsBox : MonoBehaviour
    {
        public static bool IsInLayerMask(GameObject obj, LayerMask layerMask)
        {
            return ((layerMask.value & (1 << obj.layer)) > 0);
        }
    }

}