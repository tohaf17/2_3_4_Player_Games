using SFML.System;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace k
{
    interface IBox
    {
        public bool IsExpired();
        public Clock Timer { get; set; }
        public Transformable boxObject { get; set; }
        public bool InUse { get; set; }
        public void Draw(RenderWindow window);
        
        
    }
}
