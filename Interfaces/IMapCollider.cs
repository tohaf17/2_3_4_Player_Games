using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k.Interfaces
{
    internal interface IMapCollider
    {
        bool Collides(Sprite sprite, byte[] mask, Vector2f testPow);
    }
}
