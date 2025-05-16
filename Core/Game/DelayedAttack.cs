using Core.Game.Interface;
using Microsoft.Xna.Framework;

namespace Core.Game;

public static class DelayedAttack
{
    public static TimeSpan? DelayedHit(IAttacked target, GameTime gameTime, TimeSpan lastAttackTime, TimeSpan cooldown,
        double attackStrength)
    {
        var currentTime = gameTime.TotalGameTime;
        if (lastAttackTime != TimeSpan.Zero && currentTime - lastAttackTime <= cooldown) return null;
        target.TakeDamage(attackStrength, gameTime);
        return currentTime;
    }
}