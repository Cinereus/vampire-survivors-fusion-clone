using System;

namespace CodeBase.GameLogic.Models
{
    public abstract class EntityModel<TData> : IDisposable where TData : struct
    {
        public uint id;

        public void Initialize(uint modelId, TData data)
        {
            id = modelId;
            Setup(data);
        }

        public abstract void Setup(TData data);

        public virtual void Dispose(){}
    }
}