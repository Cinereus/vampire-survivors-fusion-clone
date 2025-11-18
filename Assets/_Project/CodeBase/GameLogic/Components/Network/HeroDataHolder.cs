using CodeBase.Configs.Heroes;
using CodeBase.GameLogic.Models;
using CodeBase.Infrastructure;
using Fusion;

namespace CodeBase.GameLogic.Components.Network
{
    public class HeroDataHolder : NetworkDataHolder<HeroData>
    {
        [Networked]
        private HeroData data { get; set; }
        
        private HeroModel _model;

        public override void Spawned()
        {
            if (HasStateAuthority)
                _model = BehaviourInjector.instance.Resolve<Heroes>().GetBy(Object.Id.Raw);
            
            base.Spawned();
        }

        public override HeroData GetData() => data;

        protected override void ActualizeData()
        {
            if (HasStateAuthority) 
                data = _model.ToData();
        }
    }
}