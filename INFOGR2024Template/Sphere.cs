using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace raytracer
{
    internal class Sphere : Primitive
    {
        /// <summary>
        /// the position of the centre of the sphere in the scene
        /// </summary>
        public Vector3 scenePosition;

        /// <summary>
        /// the radius of the sphere
        /// </summary>
        public float radius;

        public Sphere(Vector3 position, float radius, Vector3 color, Material material) : base(color, material)
        {
            this.scenePosition = position;
            this.radius = radius;
        }

        public override Vector3 GiveColor(Vector3 intersectionPoint)
        {
            return this.color;
        }
    }
}
