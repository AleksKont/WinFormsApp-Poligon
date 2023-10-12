using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poligon
{
    public struct Point
    {
        public float X { get; set; }
        public float Y { get; set; }
    }

    public interface IPoligon
    {
        public List<(Point, Point, Point)> CreateMesh(IEnumerable<Point> points,
            Action<Point, Point, Point>? callback = null);
    }
}
