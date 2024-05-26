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
            Light light;
            light = new Light((5, 6, 6), 20f);
            lightList.Add(light);

            
            Sphere sphere1 = new Sphere((0, 0, 0), 1, (0, 0, 0), Primitive.Material.specular);
            sphereList.Add(sphere1);

            Sphere sphere2 = new Sphere((3, 0, 0), 1, (0, 0, 1), Primitive.Material.diffuse);
            sphereList.Add(sphere2);

            Sphere sphere3 = new Sphere((-3, 0, 0), 1, (1, 0, 0), Primitive.Material.glossy);
            sphereList.Add(sphere3);

            Plane plane1 = new Plane((0, 1, 0), (1, 1, 1), Primitive.Material.diffuse);
            planeList.Add(plane1);
        }
    }
}
