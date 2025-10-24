using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Infrastructure.Services
{
    public class ServiceLocator
    {
        public static ServiceLocator instance => _instance ??= new ServiceLocator();
        
        private static ServiceLocator _instance;

        private readonly Dictionary<ServiceContext, Dictionary<Type, IService>> _serviceContextMap =
            new Dictionary<ServiceContext, Dictionary<Type, IService>>();

        private ServiceLocator(){}
        
        public void Register<T>(T service, ServiceContext context = ServiceContext.Default) where T : IService
        {
            if (!_serviceContextMap.ContainsKey(context))
                _serviceContextMap.Add(context, new Dictionary<Type, IService>());

            Dictionary<Type, IService> services = _serviceContextMap[context];
            var serviceType = typeof(T);
            if (!services.TryAdd(serviceType, service))
            {
                Debug.LogError($"{nameof(ServiceLocator)}: Service of type {serviceType} is already registered.");
            } 
            else if (service is IInitializeService initializeService)
            {
                initializeService.Initialize();
            }
        }
        
        public T Get<T>() where T : IService
        {
            var serviceType = typeof(T);
            foreach (var services in _serviceContextMap.Values)
            {
                if (services.TryGetValue(serviceType, out var service))
                    return (T) service;
            }
            
            Debug.LogError($"{nameof(ServiceLocator)}: Service of type {serviceType} is not registered.");
            return default(T);
        }
        
        public void ClearContext(ServiceContext context)
        {
            if (_serviceContextMap.TryGetValue(context, out var services))
            {
                foreach (var service in services.Values) 
                    service.Dispose();
                
                _serviceContextMap.Remove(context);
            }
            else
            {
                Debug.LogWarning($"{nameof(ServiceLocator)}: Context with id: {context} is not found.");
            }
        }
    }
}