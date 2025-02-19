using UnityEngine;

namespace Raycaster
{
    public static class RaycasterFactory
    {
        private static Raycaster raycaster;
        public static Raycaster GetRaycaster()
        {
            if (raycaster != null) return raycaster;
            if (Camera.main == null) return null;
            //var raycaster = Camera.main.GetComponent<Raycaster>();
            if (!Camera.main.TryGetComponent(out raycaster))
            {
                raycaster = Camera.main.gameObject.AddComponent<MouseRaycaster>();
            }
            return raycaster;
        }
    }
}
