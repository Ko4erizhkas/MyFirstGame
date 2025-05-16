using Core.Game.Interface;
using Feature;
using Feature.TextureManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Core.Game.Model;

public class Enemy : Entity, IAttacked, IAttacking
{
    private readonly ITextureManager _textureManager = new TextureManager();
    private readonly ContentManager _contentManager;
    private Dictionary<int, Texture2D> _enemyTextures = new();
    private Texture2D? _enemyAttackingTexture;
    private Texture2D? _enemyDiedTexture;

    private double _attackStrength;
    private TimeSpan _lastAttackingTime;
    private TimeSpan _attackCooldown;
    private bool _isAttacking = false;

    public Enemy(Vector2 position, double healthPoints, double attackStrength, IServiceProvider serviceProvider)
        : base(position, healthPoints)
    {
        _contentManager = new ContentManager(serviceProvider, "Content");
        _attackStrength = attackStrength;
        _lastAttackingTime = TimeSpan.Zero;
        _attackCooldown = new TimeSpan(0, 0, 1);
        LoadEnemyTextures();
        Reset(position, healthPoints);
    }

    private void Reset(Vector2 position, double healthPoints)
    {
        Position = position;
        HealthPoints = healthPoints;
        Console.WriteLine($"Enemy position initialized to: {Position}");
    }

    private void LoadEnemyTextures()
    {
        _enemyTextures = _textureManager.LoadEnemyTextures(_contentManager);
    }

    private bool IsInAttackRange(Vector2 targetPosition)
    {
        var distance = Vector2.Distance(Position, targetPosition);
        return distance <= GameDefaults.EyeEnemyAttackRange;
    }

    public void Hit(IAttacked target, GameTime gameTime)
    {
        if (target is Player entity)
        {
            if (IsInAttackRange(entity.Position))
            {
                _isAttacking = true;
                _lastAttackingTime = DelayedAttack.DelayedHit(target, gameTime, _lastAttackingTime, _attackCooldown,
                                         _attackStrength) ??
                                     _lastAttackingTime;
            }
            else
            {
                _isAttacking = false;
            }
        }
    }

    public void TakeDamage(double damage, GameTime gameTime)
    {
        HealthPoints -= damage;
        Console.WriteLine(
            $"Вы нанесли {damage}ед. урона врагу, состояние здоровья: ({HealthPoints}/{GameDefaults.EyeEnemyHealthPoints})");
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (_enemyTextures.Count == 0)
        {
            Console.WriteLine("Enemy textures is null. Cannot draw enemy.");
            return;
        }

        _enemyTextures.TryGetValue(3, out var enemyTexture);
        _enemyTextures.TryGetValue(5, out var enemyAttackingTexture);
        _enemyTextures.TryGetValue(6, out var enemyDiedTexture);
        if (IsAlive && _isAttacking)
        {
            spriteBatch.Draw(
                enemyAttackingTexture,
                Position,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.0f);
        } else if (!_isAttacking)
        {
            spriteBatch.Draw(
                enemyTexture,
                Position,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.0f);
        }
        else
        {
            spriteBatch.Draw(
                enemyDiedTexture,
                Position,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.0f);
        }
    }
}