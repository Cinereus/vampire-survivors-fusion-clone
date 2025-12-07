namespace CodeBase.GameLogic.Services.SaveLoad
{
    public interface ISaveLoadEntity
    {
        public void Save(ref SaveLoadData data);
        public void Load(SaveLoadData data);
    }
}