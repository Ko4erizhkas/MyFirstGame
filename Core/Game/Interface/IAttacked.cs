using Microsoft.Xna.Framework;

namespace Core.Game.Interface;

public interface IAttacked
{
    void TakeDamage(double damage, GameTime gameTime);
}