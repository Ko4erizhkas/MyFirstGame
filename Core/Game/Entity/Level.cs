﻿using Feature;
using Feature.LevelLoader;
using Feature.TextureManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = System.Numerics.Vector2;

namespace Core.Game.Entity;

public class Level
{
    private readonly ITextureManager _textureManager = new TextureManager();
    private Dictionary<int, Texture2D> _texturesGw = [];
    private readonly ContentManager _contentManager;
    private Microsoft.Xna.Framework.Vector2 _position;

    public TileMap TileMap { get; set; }

    public Vector2 FindPlayerPosition(int playerIndex)
    {
        for (var y = 0; y < TileMap.Height; y++)
        {
            var row = TileMap.Tiles[y];
            if (row == null || row.Count != TileMap.Width)
            {
                throw new ArgumentException($"Row {y} is invalid or does not match the expected width.");
            }

            for (var x = 0; x < TileMap.Width; x++)
            {
                var tileValue = row[x];
                if (tileValue == playerIndex)
                {
                    return new Vector2(x, y);
                }
            }
        }

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
    }

    public void LoadEnemyTexture()
    {
        _texturesGw.Add(_textureManager.LoadEnemyTextures(_contentManager).Item1,
            _textureManager.LoadEnemyTextures(_contentManager).Item2);
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
                    _texturesGw.TryGetValue(2, out var texture);
                    if (texture != null)
                        spriteBatch.Draw(texture, _position, null, Color.White, 0f, Vector2.Zero, 1f,
                            SpriteEffects.None, 0.8f);
                    
                    var tileKey = TileMap.Tiles[y][x];
                    if (tileKey == 1) tileKey = 2;
                    if (!_texturesGw.TryGetValue(tileKey, out var tileTexture)) continue;
                    _position = new Microsoft.Xna.Framework.Vector2(x * tileTexture.Width, y * tileTexture.Height);
                    spriteBatch.Draw(tileTexture, _position, Color.White);
                }
            }
        }
        else
        {
            throw new ArgumentException("tileMap doesn't have a correct size");
        }
    }

    #endregion
}