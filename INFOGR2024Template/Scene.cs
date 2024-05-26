using OpenTK.Mathematics;
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
            
            Light light1 = new Light((-3f, 3f, 3f), 8f);
            lightList.Add(light1);

            Light light2 = new Light((2f, 7f, 0f), 25f);
            lightList.Add(light2);

            
            Sphere sphere1 = new Sphere((0, 2f, 4), 0.8f, (0, 1, 0), Primitive.Material.diffuseGlossyCombo);
            sphereList.Add(sphere1);

            Sphere sphere2 = new Sphere((3, 1.5f, 5), 0.5f, (0, 0, 1), Primitive.Material.glossy);
            sphereList.Add(sphere2);

            Sphere sphere3 = new Sphere((6, 2f, 4), 0.8f, (1, 0, 0), Primitive.Material.diffuse);
            sphereList.Add(sphere3);

            Sphere sphere4 = new Sphere((3, 3, 7), 2f, (1, 1, 1), Primitive.Material.specular);
            sphereList.Add(sphere4);
                
            Plane plane1 = new Plane(Vector3.Normalize((0, 1, 0)), (1, 1, 1), Primitive.Material.diffuse);
            planeList.Add(plane1);
        }
    }
}
