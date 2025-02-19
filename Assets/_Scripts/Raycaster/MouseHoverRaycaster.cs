using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

namespace Raycaster
{
    public class MouseHoverRaycaster : Raycaster
    {
        public override bool IsPointerOverUI { get ; protected set; }
        private bool active = true;
        public void SetActive(bool state)
        {
            active = state;
        }
        protected override void OnRaycastUpdate()
        {
            IsPointerOverUI = EventSystem.current.IsPointerOverGameObject();
            if (active)
            {
                if (TryRaycastAll(out var hits))
                {
                    OnGameObjectHitEvent(hits);
                }
                else
                {
                    OnEmptyUpEvent();
                }

            }
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
