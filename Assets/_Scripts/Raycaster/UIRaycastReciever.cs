using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Raycaster
{
    public class UIRaycastReciever : IDisposable
    {
        protected virtual RectTransform RectTransform
        {
            get; set;
        }
        public virtual Action<Vector3> OnHitUp
        {
            get; set;
        }
        public virtual Action<Vector3> OnHitClick
        {
            get; set;
        }
        public virtual Action<Vector3> OnHitStay
        {
            get; set;
        }
        public virtual Action<Vector3> OnHitDown
        {
            get; set;
        }
        public virtual Action<Vector3> OnHitFirstUp
        {
            get; set;
        }
        public virtual Action<Vector3> OnHitFirstClick
        {
            get; set;
        }
        public virtual Action<Vector3> OnHitFirstStay
        {
            get; set;
        }
        public virtual Action<Vector3> OnHitFirstDown
        {
            get; set;
        }
        private UIRaycaster raycaster;
        public UIRaycastReciever(RectTransform transform)
        {
            RectTransform = transform;
            raycaster = GameObject.FindAnyObjectByType<UIRaycaster>();
            InitEvents();
        }
        protected void InitEvents()
        {
            raycaster.OnGameObjectHit += OnHitStayHandler;
            raycaster.OnGameObjectHitUp += OnHitUpHandler;
            raycaster.OnGameObjectHitDown += OnHitDownHandler;
            raycaster.OnGameObjectClick += OnHitClickHandler;
        }
        protected void OnHitUpHandler(List<RaycastResult> args)
        {
            if (args.Count < 1) return;
            if (OnHitUp == null) return;
            if (TryGetSelfHit(args, out var selfHit))
            {
                OnHitUp?.Invoke(selfHit.worldPosition);
            }
        }
        protected void OnHitStayHandler(List<RaycastResult> args)
        {
            if (args.Count < 1) return;
            if (OnHitStay == null) return;
            if (TryGetSelfHit(args, out var selfHit))
            {
                OnHitStay?.Invoke(selfHit.worldPosition);
            }
        }
        protected void OnHitDownHandler(List<RaycastResult> args)
        {
            if (args.Count < 1) return;
            if (OnHitDown == null) return;
            if (TryGetSelfHit(args, out var selfHit))
            {
                OnHitDown?.Invoke(selfHit.worldPosition);
            }
        }
        protected void OnHitClickHandler(List<RaycastResult> args)
        {
            if (args.Count < 1) return;
            if (OnHitClick == null) return;
            if (TryGetSelfHit(args, out var selfHit))
            {
                OnHitClick?.Invoke(selfHit.worldPosition);
            }
        }

        protected bool TryGetSelfHit(List<RaycastResult> raycastHits, out RaycastResult selfHit)
        {
            var intersection = raycastHits.Where(hit => hit.gameObject.transform == RectTransform);
            if (intersection.Count() < 1)
            {
                selfHit = default;
                return false;
            }
            //if we have multiple receiver transforms, the last hit will be the first in world
            selfHit = raycastHits.Last(hit => hit.gameObject.transform == RectTransform);
            return true;
        }
        public void Dispose()
        {
            if (RectTransform == null)
            {
                ClearRaycasterEvents();
            }
            else if (RectTransform.gameObject.scene.isLoaded)
            {
                ClearRaycasterEvents();
            }
        }
        private void ClearRaycasterEvents()
        {
            raycaster.OnGameObjectHit -= OnHitStayHandler;
            raycaster.OnGameObjectHitUp -= OnHitUpHandler;
            raycaster.OnGameObjectHitDown -= OnHitDownHandler;
            raycaster.OnGameObjectClick -= OnHitClickHandler;
        }
    }
}