using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Infrastructure.Services
{
    public class ServiceLocator
    {
        public static ServiceLocator instance => _instance ??= new ServiceLocator();
        
        private static ServiceLocator _instance;
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        private ServiceLocator(){}
        
        public void RegisterService<T>(T service) where T : IService
        {
            var serviceType = typeof(T);
            if (!_services.TryAdd(serviceType, service)) 
                Debug.LogError($"{nameof(ServiceLocator)}: Service of type {serviceType} is already registered.");
        }
        
        public T GetService<T>() where T : IService
        {
            var serviceType = typeof(T);
            if (_services.TryGetValue(serviceType, out var service))
                return (T) service;
            
            Debug.LogError($"{nameof(ServiceLocator)}: Service of type {serviceType} is not registered.");
            return default(T);
        }
        
        public void RemoveService<T>() where T : IService
        {
            var service = GetService<T>();
            if (service == null)
            {
                Debug.LogWarning($"{nameof(ServiceLocator)}: Service of type {typeof(T)} is not found.");
                return;
            }
            
            service.Dispose();
            _services.Remove(service.GetType());
        }
    }
}