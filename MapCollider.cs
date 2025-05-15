using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.Mime.MediaTypeNames;

namespace k
{
    public class MapCollider
    {
        public readonly Texture wallTexture;
        public readonly byte[] wallMask;
        private readonly List<Sprite> wallSprite;
        public readonly Vector2f wallScale;
        private const int alphaLimit = 10;

        public MapCollider(string wallTexturePath, List<Sprite> wallTilePositions, int tileSize = 64)
        {
            wallTexture = new Texture(wallTexturePath);
            wallMask = PixelPerfectCollision.CreateMask(wallTexture);
            wallSprite =  wallTilePositions;

            

            wallScale = new Vector2f(64f / wallTexture.Size.X, 64f / wallTexture.Size.Y);
        }

        public (bool,Sprite?) Collides(Sprite tankSprite, byte[] tankMask, Vector2f testPos)
        {
            var oldPos = tankSprite.Position;
            tankSprite.Position = testPos;

            foreach (var wallPos in wallSprite)
            {
                

                if (PixelPerfectCollision.Test(tankSprite, tankMask, wallPos, wallMask, alphaLimit))
                {
                    tankSprite.Position = oldPos; // відновлення позиції
                    return (true,wallPos);
                }
            }

            tankSprite.Position = oldPos; // відновлення позиції
            return (false,null);
        }
    }
}
