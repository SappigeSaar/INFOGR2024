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
        public Vector3 position;
        public float radius;

        public Sphere(Vector3 position, float radius, Vector3 color, Material material) : base(color, material)
        {
            this.position = position;
            this.radius = radius;
        }

        public override Vector3 GiveColor(Vector3 intersectionPoint)
        {
            return this.color;
        }
    }
}
