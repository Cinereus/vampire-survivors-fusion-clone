using CodeBase.Configs;
using Cysharp.Threading.Tasks;

namespace CodeBase.Infrastructure.Services.Configs
{
    public interface IConfigProvider
    {
        public UniTask Initialize();
        public T GetConfig<T>() where T : IConfig;
    }
}