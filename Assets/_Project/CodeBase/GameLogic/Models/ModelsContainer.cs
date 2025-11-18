using System;
using System.Collections.Generic;
using CodeBase.Configs.Enemies;
using CodeBase.Configs.Heroes;
using UnityEngine;

namespace CodeBase.GameLogic.Models
{
    public class Heroes : ModelsContainer<HeroModel, HeroData>{}
    
    public class Enemies : ModelsContainer<EnemyModel, EnemyData>{}

    public abstract class ModelsContainer<TModel, TData> : IDisposable
        where TData : struct where TModel : EntityModel<TData>, new()
    {
        private readonly Dictionary<uint, TData> _dataMap = new Dictionary<uint, TData>();
        private readonly Dictionary<uint, TModel> _models = new Dictionary<uint, TModel>();

        public void Initialize(List<TData> dataList, Func<TData, uint> keyGetter)
        {
            if (dataList == null)
            {
                Debug.LogError($"{nameof(ModelsContainer<TModel, TData>)} Data list is null!");
                return;
            }

            if (keyGetter == null)
            {
                Debug.LogError($"{nameof(ModelsContainer<TModel, TData>)} KeyGetter is null!");
                return;
            }

            foreach (var data in dataList)
                _dataMap[keyGetter.Invoke(data)] = data;
        }

        public TModel GetBy(uint id) => _models.GetValueOrDefault(id);

        public bool TryGetBy(uint id, out TModel model)
        {
            model = GetBy(id);
            return model != null;
        }

        public TModel Add(uint modelId, uint dataId) => Add(modelId, GetDataBy(dataId));

        public TModel Add(uint id, TData data)
        {
            if (_models.TryGetValue(id, out TModel model))
            {
                Debug.Log($"{nameof(ModelsContainer<TModel, TData>)}: Model with id: {id} already exists");
                return model;
            }

            _models[id] = new TModel();
            _models[id].Initialize(id, data);
            return _models[id];
        }

        public void Remove(uint id) => _models.Remove(id, out _);

        public void Dispose()
        {
            foreach (var model in _models.Values)
                model.Dispose();

            _models.Clear();
            _dataMap.Clear();
        }

        private TData GetDataBy(uint id)
        {
            if (_dataMap.TryGetValue(id, out var data))
                return data;

            Debug.LogError($"{nameof(ModelsContainer<TModel, TData>)}: Data with id: {id} not found");
            return default;
        }
    }
}