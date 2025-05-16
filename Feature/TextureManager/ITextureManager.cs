using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Feature.TextureManager;

public interface ITextureManager
{
    Dictionary<int, Texture2D> LoadMapTextures(ContentManager contentManager);
    Dictionary<int, Texture2D> LoadEnemyTextures(ContentManager contentManager);
    Dictionary<int, Texture2D> LoadPlayerTextures(ContentManager contentManager);
}