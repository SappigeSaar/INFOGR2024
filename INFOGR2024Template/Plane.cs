using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace raytracer
{
    internal class Plane : Primitive
    {
        public Vector3 normal;
        public Vector3 uNormal;
        public Vector3 vNormal;

        public Vector3 contrastColor;


        public Plane(Vector3 norm, Vector3 color, Material material) : base(color, material)
        {
            this.normal = norm;
            this.uNormal = Vector3.Cross(normal, (1, 0, 0));
            uNormal.Normalize();
            this.vNormal = Vector3.Cross(normal, uNormal);
            uNormal.Normalize();

            contrastColor = new Vector3(1, 1, 1) - color;
        }

        public override Vector3 GiveColor(Vector3 intersectionPoint)
        {
            float u = Vector3.Dot(uNormal, intersectionPoint);
            float v = Vector3.Dot(vNormal, intersectionPoint);

            if(((int)u % 2 == 0 || (int) v % 2 == 0) && !((int) u % 2 == 0 && (int)v % 2 ==0 ))
                return contrastColor;
            else
                return color;
        }
    }
}
