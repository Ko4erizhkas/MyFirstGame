using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Feature.TextureManager;

public class TextureManager : ITextureManager
{
    public Dictionary<int, Texture2D> LoadMapTextures(ContentManager contentManager)
    {
        return new Dictionary<int, Texture2D>
        {
            { 0, contentManager.Load<Texture2D>("resGW/wallV")},
            { 2, contentManager.Load<Texture2D>("resGW/floorV")},
            { 4, contentManager.Load<Texture2D>("resGW/chestV1")}
        };
    }

    public Dictionary<int, Texture2D> LoadEnemyTextures(ContentManager contentManager)
    {
        return new Dictionary<int, Texture2D>
        {
            { 3, contentManager.Load<Texture2D>("resGW/enemyV1") },
            { 5, contentManager.Load<Texture2D>("resGW/eyeEnemyAttacking") },
            { 6, contentManager.Load<Texture2D>("resGW/eyeEnemyDied") }
        };
    }

    public Dictionary<int, Texture2D> LoadPlayerTextures(ContentManager contentManager)
    {
        return new Dictionary<int, Texture2D>
        {
            { 0, contentManager.Load<Texture2D>("resGW/playerV1")},
            { 7, contentManager.Load<Texture2D>("resGW/playerAttacking")},
            { 8, contentManager.Load<Texture2D>("resGW/playerDied") },
            { 9, contentManager.Load<Texture2D>("resGW/playerDamaged") },
        };
    }
}