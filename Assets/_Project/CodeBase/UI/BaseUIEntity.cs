using System;
using UnityEngine;

namespace CodeBase.UI
{
    public abstract class BaseUIEntity : MonoBehaviour, IDisposable
    {
        public bool isNeedCache { get; set; }

        public virtual void Show() => gameObject.SetActive(true);

        public virtual void Hide() => gameObject.SetActive(false);

        public virtual void Dispose() => Destroy(gameObject);
    }
}