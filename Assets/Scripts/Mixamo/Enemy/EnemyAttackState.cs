using UnityEngine;

public class EnemyAttackState : EnemyState
{
    // 攻撃アニメの終わり判定しきい値（0.95〜0.99くらいが無難）
    private const float END_NORM = 0.95f;
    private bool triggered;

    public EnemyAttackState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        triggered = false;

        // 攻撃開始時にクールダウン予約（連打防止）
        enemy.nextAttackTime = Time.time + enemy.attackCooldown;

        // いったん停止 & 向き合わせ
        var v = enemy.Vision;
        if (v != null && v.CanSeeTarget())
            enemy.Motor.FaceTo(v.target.position);

        enemy.Anim.SetFloat(enemy.SpeedParam, 0f);
    }

    public override void Tick()
    {
        var vision = enemy.Vision;

        if (vision == null || !vision.CanSeeTarget())
        {
            enemy.SM.ChangeState(enemy.LocomotionState);
            return;
        }

        enemy.Motor.FaceTo(vision.target.position);
        enemy.Anim.SetFloat(enemy.SpeedParam, 0f, 0.12f, Time.deltaTime);

        if (!triggered)
        {
            enemy.Anim.ResetTrigger(enemy.AttackParam);
            enemy.Anim.SetTrigger(enemy.AttackParam);
            triggered = true;
        }

        var st = enemy.Anim.GetCurrentAnimatorStateInfo(0);

        // 攻撃ステートを抜けたら戻る（これが最も安全）
        if (triggered && !st.IsTag("Attack"))
        {
            enemy.SM.ChangeState(enemy.LocomotionState);
        }
    }

    public override void Exit()
    {
        // 何か後処理が必要ならここ
    }
}