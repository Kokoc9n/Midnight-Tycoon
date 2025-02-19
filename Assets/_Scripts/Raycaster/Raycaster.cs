using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Raycaster
{
    public abstract class Raycaster : MonoBehaviour
    {
        public abstract bool IsPointerOverUI { get; protected set; }
        protected LayerMask layerMask;

        public event OnHitHandler OnGameObjectHitUp;
        public event OnHitHandler OnGameObjectHitDown;
        public event OnHitHandler OnGameObjectHit;
        public event OnHitHandler OnGameObjectClick;
        public event OnHitFirstHandler OnGameObjectFirstHitUp;
        public event OnHitFirstHandler OnGameObjectFirstHitDown;
        public event OnHitFirstHandler OnGameObjectFirstHit;
        public event OnHitFirstHandler OnGameObjectFirstClick;
        public event Action OnEmptyUp;
        public delegate void OnHitHandler(object sender, RaycastEventArgs args);
        public delegate void OnHitFirstHandler(object sender, FirstRaycastEventArgs args);
        private Dictionary<int, RaycastHit> clickEventCache = new Dictionary<int, RaycastHit>();//int here = transform instance id
        private List<RaycastHit> reusableCache = new List<RaycastHit>();
        private List<int> reusableIdCache = new List<int>();
        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
            layerMask = ~LayerMask.GetMask("Ignore Raycast");
        }

        protected virtual void OnGameObjectHitEvent(RaycastEventArgs args)
        {
            UpdateCacheClicksStay(args);
            OnGameObjectFirstHit?.Invoke(this, GetFirstEventArgs(args));
            OnGameObjectHit?.Invoke(this, args);
        }
        protected virtual void OnGameObjectHitUpEvent(RaycastEventArgs args)
        {
            var clickEventArgs = FiltertArgsForClickEvent(ref args);
            OnGameObjectClickEvent(clickEventArgs);
            OnGameObjectFirstHitUp?.Invoke(this, GetFirstEventArgs(args));
            OnGameObjectHitUp?.Invoke(this, args);
        }
        protected virtual void OnGameObjectHitDownEvent(RaycastEventArgs args)
        {
            if (args.HasHits)
                CacheClickDownHits(args.raycastHits);
            OnGameObjectFirstHitDown?.Invoke(this, GetFirstEventArgs(args));
            OnGameObjectHitDown?.Invoke(this, args);
        }
        protected virtual void OnGameObjectClickEvent(RaycastEventArgs args)
        {
            OnGameObjectFirstClick?.Invoke(this, GetFirstEventArgs(args));
            OnGameObjectClick?.Invoke(this, args);
        }
        protected virtual void OnEmptyUpEvent()
        {
            OnEmptyUp?.Invoke();
        }
        private void CacheClickDownHits(IEnumerable<RaycastHit> hits)
        {
            clickEventCache.Clear();
            foreach (var hit in hits)
            {
                var transformId = hit.transform.GetInstanceID();
                if(!clickEventCache.ContainsKey(transformId))
                    clickEventCache.Add(transformId, hit);
            }
        }
        private void UpdateCacheClicksStay(RaycastEventArgs stayEventArgs)
        {
            // Pointer left object during stay event, clear clicks.
            if (!stayEventArgs.HasHits && clickEventCache.Count>0)
            {
                clickEventCache.Clear();
            }
            else
            {
                reusableCache.Clear();
                reusableIdCache.Clear();
                if (clickEventCache.Count > 0)
                {
                    // When pointer leaves object marked with OnDown, clear it from click cache.
                    foreach(var transformId in clickEventCache.Keys)
                    {
                        try
                        {
                            var tryGet = stayEventArgs.raycastHits.First(_ => _.transform.GetInstanceID() == transformId);
                        }
                        catch(InvalidOperationException)
                        {
                            reusableIdCache.Add(transformId); // Mark for deletion.
                        }
                    }
                    // Delete.
                    foreach(var transformId in reusableIdCache)
                    {
                        clickEventCache.Remove(transformId);
                    }
                }
            }
                
        }
        private RaycastEventArgs FiltertArgsForClickEvent(ref RaycastEventArgs upEventArgs)
        {
            var args = new RaycastEventArgs();
            args.IsPointerOverUI = upEventArgs.IsPointerOverUI;
            if (clickEventCache.Count == 0 || !upEventArgs.HasHits) return args;
            reusableCache.Clear();
            foreach (var hit in upEventArgs.raycastHits)
            {
                var transformId = hit.transform.GetInstanceID();
                if(clickEventCache.ContainsKey(transformId))
                {
                    reusableCache.Add(hit);
                }
            }
            foreach(var clickHit in reusableCache)
            {
                upEventArgs.raycastHits.RemoveAll(_ => _.transform == clickHit.transform);
            }
            args.raycastHits = reusableCache.ToList();
            return args;
        }
        private FirstRaycastEventArgs GetFirstEventArgs(RaycastEventArgs args)
        {
            FirstRaycastEventArgs firstArgs = new FirstRaycastEventArgs();
            if(!args.HasHits)
            {
                firstArgs.HasHit = false;
                firstArgs.IsPointerOverUI = args.IsPointerOverUI;
                return firstArgs;
            }
            else
            {
                firstArgs.HasHit = true;
                firstArgs.hit = args.raycastHits.First();
                firstArgs.IsPointerOverUI = args.IsPointerOverUI;
                return firstArgs;
            }

        }
        protected abstract void OnRaycastUpdate();
        
        private void Update()
        {
            if (mainCamera == null) return;
            OnRaycastUpdate();
        }
        protected bool TryRaycastAll(out RaycastEventArgs args)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            var hits = RaycastAllOrdered(ray, 99999, layerMask).ToList();
            args = new RaycastEventArgs()
            {
                raycastHits = hits,
                IsPointerOverUI = IsPointerOverUI
            };
            return hits.Count > 0 ? true : false;
        }
        private IEnumerable<RaycastHit> RaycastAllOrdered(Ray ray, float distance, LayerMask mask, bool descending = false)
        {
            var hits = Physics.RaycastAll(ray, distance, mask);
            if (descending)
            {
                return hits.OrderByDescending(_ => Vector3.SqrMagnitude(_.point - ray.origin));
            }
            else
            {
                return hits.OrderBy(_ => Vector3.SqrMagnitude(_.point - ray.origin));
            }
        }
    }
}
