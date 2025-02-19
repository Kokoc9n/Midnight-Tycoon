using UnityEngine;

namespace Raycaster
{
    public struct FirstRaycastEventArgs
    {
        public RaycastHit hit;
        public bool HasHit;
        public bool IsPointerOverUI;
    }
}
