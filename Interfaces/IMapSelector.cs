using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k
{
    public interface IMapSelector
    {
        public List<Level> SelectMaps(int count);
    }
}
