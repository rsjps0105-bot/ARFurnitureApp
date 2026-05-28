using UnityEngine;

public class EnemyLocomotionState : EnemyState
{
    public EnemyLocomotionState(Enemy enemy) : base(enemy) { }

    public override void Tick()
    {
        var vision = enemy.Vision;
        var motor = enemy.Motor;

        if (!vision.CanSeeTarget())
        {
            enemy.Anim.SetFloat(enemy.SpeedParam, 0f, 0.12f, Time.deltaTime);
            return;
        }

        // 攻撃レンジ内なら AttackState へ
        if (vision.InAttackRange())
        {
            motor.FaceTo(vision.target.position);
            enemy.Anim.SetFloat(enemy.SpeedParam, 0f, 0.12f, Time.deltaTime);

            // クールダウン中は攻撃しない
            if (Time.time >= enemy.nextAttackTime)
            {
                enemy.SM.ChangeState(enemy.AttackState);
            }
            return;
        }

        // ターゲットに向かって移動
        motor.MoveTo(vision.target.position);
        enemy.Anim.SetFloat(enemy.SpeedParam, 1f, 0.12f, Time.deltaTime);
    }
}