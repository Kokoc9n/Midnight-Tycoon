using UnityEngine;
using UnityEngine.EventSystems;

namespace Raycaster
{
    public class MouseRaycaster : Raycaster
    {
        public override bool IsPointerOverUI { get ; protected set; }
        protected override void OnRaycastUpdate()
        {
            IsPointerOverUI = EventSystem.current.IsPointerOverGameObject(); // Make sure we are not hitting ui.
            
            if (Input.GetMouseButtonUp(0))
            {
                if (TryRaycastAll(out var hits))
                {
                    //Debug.Log("UI State1 " + IsPointerOverUI);
                    OnGameObjectHitUpEvent(hits);
                }
                else
                {
                    OnEmptyUpEvent();
                }
            }
            if (Input.GetMouseButton(0))
            {
                if (TryRaycastAll(out var hits))
                {
                    OnGameObjectHitEvent(hits);
                }
            }
            if (Input.GetMouseButtonDown(0))
            {

                if (TryRaycastAll(out var hits))
                {
                    OnGameObjectHitDownEvent(hits);
                }
            }
        }
    }
}
