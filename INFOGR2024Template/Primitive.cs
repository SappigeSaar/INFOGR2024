using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace raytracer
{
    internal abstract class Primitive
    {
        /// <summary>
        /// RGB values of max 1, min 0;
        /// </summary>
        public Vector3 color;//{ get { return color; } set {color = value} };

        public enum Material { diffuse, glossy, specular};
        public Material material;

        public Primitive(Vector3 color, Material material)
        {
            this.color = color;
            this.material = material;
        }

        public abstract Vector3 GiveColor(Vector3 intersectionPoint);
        
    }
}
