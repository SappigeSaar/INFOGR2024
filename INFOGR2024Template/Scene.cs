using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace raytracer
{
    internal class Scene
    {
        public List<Sphere> sphereList = new List<Sphere>();
        public List<Light> lightList = new List<Light>();
        public List<Plane> planeList = new List<Plane>();

        public Scene()
        {

        }
    }
}
