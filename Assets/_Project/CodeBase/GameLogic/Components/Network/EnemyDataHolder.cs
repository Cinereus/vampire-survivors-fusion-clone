using CodeBase.Configs.Enemies;
using CodeBase.GameLogic.Models;
using CodeBase.Infrastructure;
using Fusion;

namespace CodeBase.GameLogic.Components.Network
{
    public class EnemyDataHolder : NetworkDataHolder<EnemyData>
    {
        [Networked]
        private EnemyData data { get; set; }
        
        private EnemyModel _model;

        public override void Spawned()
        {
            if (HasStateAuthority)
                _model = BehaviourInjector.instance.Resolve<Enemies>().GetBy(Object.Id.Raw);
            
            base.Spawned();
        }

        public override EnemyData GetData() => data;

        protected override void ActualizeData()
        {
            if (HasStateAuthority) 
                data = _model.ToData();
        }
    }
}