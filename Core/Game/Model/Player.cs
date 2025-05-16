using Core.Game.Interface;
using Feature;
using Feature.MovementManager;
using Feature.TextureManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Core.Game.Model
{
    public class Player : Entity, IAttacked, IAttacking
    {
        private readonly ITextureManager _textureManager = new TextureManager();
        private readonly IMovementManager _movementManager = new MovementManager();
        private readonly ContentManager _contentManager;

        private Dictionary<int, Texture2D> _playerTextures = new();
        private Level _level;

        private Vector2 _position;
        private double _attackStrength;
        private TimeSpan _lastAttackingTime;
        private bool _isAttacking = false;
        private (bool, TimeSpan) _isDamaged, _damagedTime;
        
        private TimeSpan _attackCooldown;
        private TimeSpan _damagedRenderCooldown;


        public Player(Vector2 position, double healthPoints, double attackStrength, IServiceProvider serviceProvider,
            Level level) : base(position, healthPoints)
        {
            _contentManager = new ContentManager(serviceProvider, "Content");
            _level = level;
            _attackStrength = attackStrength;
            _lastAttackingTime = TimeSpan.Zero;
            _isDamaged = (false, TimeSpan.Zero);
            _attackCooldown = new TimeSpan(0, 0, 0, 1);
            _damagedRenderCooldown = new TimeSpan(0, 0, 2);
            LoadPlayerTextures();
            Reset(position, healthPoints);
        }

        private void Reset(Vector2 position, double healthPoints)
        {
            Position = position * GameDefaults.TileSize;
            _position = Position;
            HealthPoints = healthPoints;
            Console.WriteLine($"Player position initialized to: {_position}");
        }

        private void LoadPlayerTextures()
        {
            _playerTextures = _textureManager.LoadPlayerTextures(_contentManager) ??
                              throw new ContentLoadException("Не удалось загрузить текстуру игрока");
        }

        public void Update(GameTime gameTime)
        {
            var (x, y) = _movementManager.HandleMovement();
            if (IsAlive)
                MovePlayer(x, y);
            if (gameTime.TotalGameTime - _damagedTime.Item2 > _damagedRenderCooldown)
            {
                _isDamaged = (false, TimeSpan.Zero);
            }
        }

        private void MovePlayer(float x, float y)
        {
            if (_playerTextures.Count == 0) return;

            var movement = new Vector2(x, y) * GameDefaults.PlayerMovementSpeed;

            var horizontalRect = new Rectangle(
                (int)(_position.X + movement.X),
                (int)_position.Y,
                _playerTextures.First().Value.Width,
                _playerTextures.First().Value.Height
            );

            if (!_movementManager.CheckCollision(horizontalRect, _level.TileMap))
            {
                _position.X += movement.X;
            }

            var verticalRect = new Rectangle(
                (int)_position.X,
                (int)(_position.Y + movement.Y),
                _playerTextures.First().Value.Width,
                _playerTextures.First().Value.Height
            );

            if (!_movementManager.CheckCollision(verticalRect, _level.TileMap))
            {
                _position.Y += movement.Y;
            }

            Position = _position;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_playerTextures.Count == 0)
            {
                Console.WriteLine("Player texture is null. Cannot draw player.");
                return;
            }

            _playerTextures.TryGetValue(0, out var playerTexture);
            _playerTextures.TryGetValue(7, out var playerAttackingTexture);
            _playerTextures.TryGetValue(8, out var playerDiedTexture);
            _playerTextures.TryGetValue(9, out var playerDamagedTexture);

            if (IsAlive && _isDamaged.Item1)
            {
                spriteBatch.Draw(
                    playerDamagedTexture,
                    _position,
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    1f,
                    SpriteEffects.None,
                    0.0f);
                return;
            }
            
            if (IsAlive && !_isAttacking)
            {
                spriteBatch.Draw(
                    playerTexture,
                    _position,
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    1f,
                    SpriteEffects.None,
                    0.0f);
            }
            else if (IsAlive && _isAttacking)
            {
                spriteBatch.Draw(
                    playerAttackingTexture,
                    _position,
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    1f,
                    SpriteEffects.None,
                    0.0f);
            }
            else if (!IsAlive)
            {
                spriteBatch.Draw(
                    playerDiedTexture,
                    _position,
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    1f,
                    SpriteEffects.None,
                    0.0f);
            }
        }

        private bool IsInAttackRange(Vector2 targetPosition)
        {
            return Vector2.Distance(Position, targetPosition) <= GameDefaults.PlayerAttackRange;
        }

        public void TakeDamage(double damage, GameTime gameTime)
        {
            HealthPoints -= damage;
            Console.WriteLine(
                $"Получен урон: {damage}ед., состояние здоровья: ({HealthPoints}/{GameDefaults.PlayerHeathPoints})");
            _isDamaged = (true, gameTime.TotalGameTime);
        }

        public void Hit(IAttacked target, GameTime gameTime)
        {
            if (target is Enemy enemy)
            {
                if (IsInAttackRange(enemy.Position))
                {
                    _lastAttackingTime =
                        DelayedAttack.DelayedHit(target, gameTime, _lastAttackingTime, _attackCooldown,
                            _attackStrength) ??
                        _lastAttackingTime;
                    _isAttacking = true;
                }
                else
                {
                    _isAttacking = false;
                }
            }
        }

        public void SetAttacking(bool isAttacking)
        {
            _isAttacking = isAttacking;
        }
    }
}