using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace Raycaster
{
    public class UIRaycaster : MonoBehaviour
    {
        public event OnHitHandler OnGameObjectHitUp;
        public event OnHitHandler OnGameObjectHitDown;
        public event OnHitHandler OnGameObjectHit;
        public event OnHitHandler OnGameObjectClick;

        public event Action OnEmptyUp;

        public delegate void OnHitHandler(List<RaycastResult> args);

        [SerializeField] GraphicRaycaster graphicRaycaster;
        [SerializeField] EventSystem eventSystem;
        private PointerEventData pointerEventData;

        private bool DoRaycast(out List<RaycastResult> results)
        {
            pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.position = Input.mousePosition;

            results = new List<RaycastResult>();
            graphicRaycaster.Raycast(pointerEventData, results);
            //eventSystem.RaycastAll(pointerEventData, results);

            return results.Count > 0 ? true : false;
        }
        protected void OnGameObjectHitEvent(List<RaycastResult> args)
        {
            OnGameObjectHit?.Invoke(args);
        }
        protected void OnGameObjectHitUpEvent(List<RaycastResult> args)
        {
            OnGameObjectClickEvent(args);
            OnGameObjectHitUp?.Invoke(args);
        }
        protected void OnGameObjectHitDownEvent(List<RaycastResult> args)
        {
            if (args.Count > 0)
            OnGameObjectHitDown?.Invoke(args);
        }
        protected void OnGameObjectClickEvent(List<RaycastResult> args)
        {
            OnGameObjectClick?.Invoke(args);
        }
        protected void OnEmptyUpEvent()
        {
            OnEmptyUp?.Invoke();
        }
        private void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // Hover
                if (DoRaycast(out var hits))
                {
                    OnGameObjectHitEvent(hits);
                }
                
                if (Input.GetMouseButtonUp(0))
                {
                    if (DoRaycast(out var _hits))
                    {
                        OnGameObjectHitUpEvent(_hits);
                    }
                    else
                    {
                        OnEmptyUpEvent();
                    }
                }
                if (Input.GetMouseButton(0))
                {
                    if (DoRaycast(out var _hits))
                    {
                        OnGameObjectHitEvent(_hits);
                    }
                }
                if (Input.GetMouseButtonDown(0))
                {

                    if (DoRaycast(out var _hits))
                    {
                        OnGameObjectHitDownEvent(_hits);
                    }
                }
            }
            else
            {
                OnEmptyUpEvent();
            }

        }
    }
}
