using System;

namespace CodeBase.Infrastructure.Services
{
    public interface IService : IDisposable { }

    public interface IInitializeService : IService
    {
        public void Initialize();
    }
}