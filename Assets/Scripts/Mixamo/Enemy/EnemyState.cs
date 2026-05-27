public abstract class EnemyState
{
    protected readonly Enemy enemy;
    protected EnemyState(Enemy enemy) { this.enemy = enemy; }

    public virtual void Enter() { }
    public virtual void Tick() { }
    public virtual void Exit() { }
}