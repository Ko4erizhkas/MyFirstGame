using Microsoft.Xna.Framework;

namespace Core.Game.Interface;

public interface IAttacking
{
    void Hit(IAttacked target, GameTime gameTime);
}