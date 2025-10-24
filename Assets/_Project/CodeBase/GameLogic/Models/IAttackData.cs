namespace CodeBase.GameLogic.Models
{
    public interface IAttackData
    {
        public float damage { get; }
        public float attackCooldown { get; }
    }
}