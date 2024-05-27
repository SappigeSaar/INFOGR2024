using OpenTK.Mathematics;
using System;

namespace raytracer
{
    internal class Triangle : Primitive
    {
        public Vector3 point1;
        public Vector3 point2;
        public Vector3 point3;

        public Vector3 normal;

        public Triangle(Vector3 point1, Vector3 point2, Vector3 point3,Vector3 color, Material material) : base(color, material)
        {
            this.point1 = point1;
            this.point2 = point2;
            this.point3 = point3;

            this.normal = Vector3.Normalize(Vector3.Cross(point2 - point1, point3 - point1));
        }

        public override Vector3 GiveColor(Vector3 intersectionPoint)
        {
            return this.color;
        }
    }
}
