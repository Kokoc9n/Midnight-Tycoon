using System.Collections.Generic;
using UnityEngine;

namespace Raycaster
{
    public struct RaycastEventArgs
    {
        public List<RaycastHit> raycastHits;
        public bool HasHits => raycastHits != null && raycastHits.Count != 0;
        public bool IsPointerOverUI;
    }
}
