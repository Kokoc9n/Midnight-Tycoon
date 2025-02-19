using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Raycaster
{
    public class RaycastReceiver:IDisposable
    {
        public bool IgnoreUI { get; set; }
        protected virtual HashSet<Transform> Transforms { get; set; }
        public virtual Action<Vector3> OnHitUp { get; set; }
        public virtual Action<Vector3> OnHitClick { get; set; }
        public virtual Action<Vector3> OnHitStay { get; set; }
        public virtual Action<Vector3> OnHitDown { get; set; }
        public virtual Action<Vector3> OnHitFirstUp { get; set; }
        public virtual Action<Vector3> OnHitFirstClick { get; set; }
        public virtual Action<Vector3> OnHitFirstStay { get; set; }
        public virtual Action<Vector3> OnHitFirstDown { get; set; }
        public RaycastReceiver(params Transform[] transforms)
        {
            if (transforms == null || transforms.Length < 1)
                throw new ArgumentException("Unable to instantiate RaycastReceiver. It requires at least 1 transform.");
            Transforms = new HashSet<Transform>( transforms);
            InitEvents();
        }
        protected virtual void InitEvents()
        {
            var raycaster = RaycasterFactory.GetRaycaster();
            raycaster.OnGameObjectHit += OnHitStayHandler;
            raycaster.OnGameObjectHitUp += OnHitUpHandler;
            raycaster.OnGameObjectHitDown += OnHitDownHandler;
            raycaster.OnGameObjectClick += OnHitClickHandler;
            raycaster.OnGameObjectFirstHit += OnHitStayFirstHandler;
            raycaster.OnGameObjectFirstHitUp += OnHitUpFirstHandler;
            raycaster.OnGameObjectFirstHitDown += OnHitDownFirstHandler;
            raycaster.OnGameObjectFirstClick += OnHitClickFirstHandler;
        }
        protected virtual void OnHitUpHandler(object sender, RaycastEventArgs args)
        {
            if (!args.HasHits) return;
            if (OnHitUp == null) return;
            if (!IgnoreUI && args.IsPointerOverUI) return;
            if (TryGetSelfHit(args.raycastHits, out var selfHit, out var hasOtherHits))
            {
                OnHitUp?.Invoke(selfHit.point);
            }
        }
        protected virtual void OnHitStayHandler(object sender, RaycastEventArgs args)
        {
            if (!args.HasHits) return;
            if (OnHitStay == null) return;
            if (!IgnoreUI && args.IsPointerOverUI) return;
            if (TryGetSelfHit(args.raycastHits, out var selfHit, out var hasOtherHits))
            {
                OnHitStay?.Invoke(selfHit.point);
            }
        }
        protected virtual void OnHitDownHandler(object sender, RaycastEventArgs args)
        {
            if (!args.HasHits) return;
            if (OnHitDown == null) return;
            if (!IgnoreUI && args.IsPointerOverUI) return;
            if (TryGetSelfHit(args.raycastHits, out var selfHit, out var hasOtherHits))
            {
                OnHitDown?.Invoke(selfHit.point);
            }
        }
        protected virtual void OnHitClickHandler(object sender, RaycastEventArgs args)
        {
            if (!args.HasHits) return;
            if (OnHitClick == null) return;
            if (!IgnoreUI && args.IsPointerOverUI) return;
            if (TryGetSelfHit(args.raycastHits, out var selfHit, out var hasOtherHits))
            {
                OnHitClick?.Invoke(selfHit.point);
            }
        }
        protected virtual void OnHitUpFirstHandler(object sender, FirstRaycastEventArgs args)
        {
            if (!args.HasHit) return;
            if (!IgnoreUI && args.IsPointerOverUI) return;
            if (CheckTransformHit(args))
            {
                OnHitFirstUp?.Invoke(args.hit.point);
            }
        }
        protected virtual void OnHitStayFirstHandler(object sender, FirstRaycastEventArgs args)
        {
            if (!args.HasHit) return;
            if (!IgnoreUI && args.IsPointerOverUI) return;
            if (CheckTransformHit(args))
            {
                OnHitFirstStay?.Invoke(args.hit.point);
            }
        }
        protected virtual void OnHitDownFirstHandler(object sender, FirstRaycastEventArgs args)
        {
            if (!args.HasHit) return;
            if (!IgnoreUI && args.IsPointerOverUI) return;
            if (CheckTransformHit(args))
            {
                OnHitFirstDown?.Invoke(args.hit.point);
            }
        }
        protected virtual void OnHitClickFirstHandler(object sender, FirstRaycastEventArgs args)
        {
            if (!args.HasHit) return;
            if (!IgnoreUI && args.IsPointerOverUI) return;
            if (CheckTransformHit(args))
            {
                OnHitFirstClick?.Invoke(args.hit.point);
            }
        }
        protected virtual bool TryGetSelfHit(IEnumerable<RaycastHit> raycastHits, out RaycastHit selfHit, out bool hasOtherHits)
        {
            hasOtherHits = raycastHits.Count() != 1;
            var intersection = raycastHits.Where(_ => Transforms.Contains(_.transform));
            if(intersection.Count()<1)
            {
                selfHit = default;
                return false;
            }
            //if we have multiple receiver transforms, the last hit will be the first in world
            selfHit = raycastHits.Last(_ => _.transform == Transforms.Contains(_.transform));
            return true;
        }
        protected virtual bool CheckTransformHit(FirstRaycastEventArgs args)
        {
            return Transforms.Contains(args.hit.transform);
        }
        public void Dispose()
        {
            if (Transforms.First() == null)
            {
                ClearRaycasterEvents();
            }
            else if(Transforms.First().gameObject.scene.isLoaded)
            {
                ClearRaycasterEvents();
            }
        }
        private void ClearRaycasterEvents()
        {
            var raycaster = RaycasterFactory.GetRaycaster();
            if (!raycaster)
                return;
            raycaster.OnGameObjectHit -= OnHitStayHandler;
            raycaster.OnGameObjectHitUp -= OnHitUpHandler;
            raycaster.OnGameObjectHitDown -= OnHitDownHandler;
            raycaster.OnGameObjectClick-= OnHitClickHandler;
            raycaster.OnGameObjectFirstHit -= OnHitStayFirstHandler;
            raycaster.OnGameObjectFirstHitUp -= OnHitUpFirstHandler;
            raycaster.OnGameObjectFirstHitDown -= OnHitDownFirstHandler;
            raycaster.OnGameObjectFirstClick -= OnHitClickFirstHandler;
        }
    }
}
