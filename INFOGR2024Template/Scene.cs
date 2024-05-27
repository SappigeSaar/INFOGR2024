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
        /// <summary>
        /// the list with all the spheres
        /// </summary>
        public List<Sphere> sphereList = new List<Sphere>();

        /// <summary>
        /// the list with all the lights
        /// </summary>
        public List<Light> lightList = new List<Light>();

        /// <summary>
        /// the list with all the planes
        /// </summary>
        public List<Plane> planeList = new List<Plane>();


        public List<Triangle> triangleList = new List<Triangle>();

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

            Sphere sphere5 = new Sphere((6, 4, 7), 0.8f, (1, 1, 1), Primitive.Material.specular);
            sphereList.Add((sphere5));
                
            Plane plane1 = new Plane(Vector3.Normalize((0, 1, 0)), (1, 0, 1),(1, 1, 1), Primitive.Material.diffuse);
            planeList.Add(plane1);

            Plane plane2 = new Plane((1, 1, 0), (-5, 0, 0), (0, 0.1f, 0.5f), Primitive.Material.diffuseGlossyCombo);
            plane2.contrastColor = (1, 1, 1);
            planeList.Add(plane2);

            Triangle triangle1 = new Triangle((0, 0.5f, 5), (1.5f, 0.5f, 6), (0.5f, 5, 6), (1f, 1, 0), Primitive.Material.diffuse);
            triangleList.Add(triangle1);

            Triangle triangle2 = new Triangle((0, 0.5f, 5), (-3f, 0.5f, 6), (0.5f, 5, 6), (1f, 1, 0), Primitive.Material.diffuse);
            triangleList.Add(triangle2);

        }
    }
}
