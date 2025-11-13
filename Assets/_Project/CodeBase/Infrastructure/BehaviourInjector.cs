using VContainer;

namespace CodeBase.Infrastructure
{
    public class BehaviourInjector
    {
        public static BehaviourInjector instance => _instance ??= new BehaviourInjector();
        
        private static BehaviourInjector _instance;
        private IObjectResolver _resolver;
        
        private BehaviourInjector(){}
        
        public void SetupResolver(IObjectResolver resolver) => _resolver = resolver;
        
        public T Resolve<T>() => _resolver.Resolve<T>();
    }
}