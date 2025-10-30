using CodeBase.Configs.Heroes;
using CodeBase.Infrastructure.Services;

namespace CodeBase.GameLogic
{
    public class PlayerData : IService
    {
        public HeroType chosenHero;
        public string roomName;
        public bool isHost;

        public void Dispose() { }
    }
}