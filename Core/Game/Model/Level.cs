using Feature;
using Feature.LevelLoader;
using Feature.TextureManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = System.Numerics.Vector2;

namespace Core.Game.Model;

public class Level
{
    private readonly ITextureManager _textureManager = new TextureManager();
    private Dictionary<int, Texture2D> _texturesGw = [];
    private readonly ContentManager _contentManager;
    private Microsoft.Xna.Framework.Vector2 _position;

    public List<Enemy> Enemies { get; private set; } = new List<Enemy>();

    public TileMap TileMap { get; set; }

    public Vector2 FindPlayerPosition(int playerIndex)
    {
        for (var y = 0; y < TileMap.Height; y++)
        {
            var row = TileMap.Tiles[y];
            for (var x = 0; x < TileMap.Width; x++)
            {
                if (row[x] == playerIndex)
                {
                    Console.WriteLine($"Player spawn position found at: ({x}, {y})");
                    return new Vector2(x, y);
                }
            }
        }

        Console.WriteLine("Player spawn position not found. Defaulting to (0, 0).");
        return Vector2.Zero;
    }

    public Level(IServiceProvider serviceProvider)
    {
        _contentManager = new ContentManager(serviceProvider, "Content");
        Initialize();
    }

    #region LoadRegion

    public void LoadTileMap(ILevelLoader levelLoader)
    {
        TileMap = levelLoader.LoadRandomLevel();
    }

    public void LoadMapTextures()
    {
        var mapTextures = _textureManager.LoadMapTextures(_contentManager);
        foreach (var texture in mapTextures)
        {
            _texturesGw.Add(texture.Key, texture.Value);
        }

        Console.WriteLine("Map textures loaded.");
    }

    public void LoadEnemyTextures()
    {
        //TexturesGw.Add(_textureManager.LoadEnemyTextures(_contentManager).Item1,
        //    _textureManager.LoadEnemyTextures(_contentManager).Item2);
    }

    public void LoadEnemies(GameServiceContainer services)
    {
        for (var y = 0; y < TileMap.Height; y++)
        {
            for (var x = 0; x < TileMap.Width; x++)
            {
                if (TileMap.Tiles[y][x] == (int)TileCollision.Enemy)
                {
                    var position = new Vector2(x, y) * GameDefaults.TileSize;
                    Enemies.Add(new Enemy(position, GameDefaults.EyeEnemyHealthPoints,
                        GameDefaults.EyeEnemyAttackStrength, services));
                }
            }
        }
    }

    #endregion

    #region InitializeRegion

    private void Initialize()
    {
        InitLevel();
    }

    private void InitLevel()
    {
        TileMap = new TileMap();
    }

    #endregion

    #region RenderRegion

    public void Draw(SpriteBatch spriteBatch)
    {
        if (TileMap.Width > 0 && TileMap.Height > 0)
        {
            for (var y = 0; y < TileMap.Height; y++)
            {
                for (var x = 0; x < TileMap.Width; x++)
                {
                    var tileKey = TileMap.Tiles[y][x];
                    if (tileKey is 1 or 3) tileKey = 2;

                    if (!_texturesGw.TryGetValue(tileKey, out var texture))
                    {
                        Console.WriteLine($"Текстура для ключа {tileKey} не найдена.");
                        continue;
                    }
                    _position = new Microsoft.Xna.Framework.Vector2(x * GameDefaults.TileSize, y * GameDefaults.TileSize);

                    // if (texture != null)
                    //     spriteBatch.Draw(texture, _position, null, Color.White, 0f, Vector2.Zero, 1f,
                    //         SpriteEffects.None, 0.8f);

                    spriteBatch.Draw(texture, _position, null, Color.White, 0f, Vector2.Zero, 1f,
                        SpriteEffects.None, 0.8f);
                    //
                    // if (!_texturesGw.TryGetValue(tileKey, out var tileTexture)) continue;
                    // _position = new Microsoft.Xna.Framework.Vector2(x * tileTexture.Width, y * tileTexture.Height);
                    // spriteBatch.Draw(tileTexture, _position, null, Color.White, 0f, Vector2.Zero, 1f,
                    //     SpriteEffects.None, 0.8f);
                }
            }
        }
        else
        {
            throw new ArgumentException("tileMap doesn't have a correct size");
        }
    }

    public void DrawEnemies(SpriteBatch spriteBatch)
    {
        foreach (var enemy in Enemies)
        {
            enemy.Draw(spriteBatch);
        }
    }

    #endregion
}