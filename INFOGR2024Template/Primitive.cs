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
        public Vector3 color;

        /// <summary>
        /// all the possinle materials a primitive can be
        /// </summary>
        public enum Material { diffuse, glossy, specular, diffuseGlossyCombo};
       
        /// <summary>
        /// the material this primitive is made out of
        /// choice between diffuse, glossy, specular, or a combination of diffuse and glossy
        /// </summary>
        public Material material;

        public Primitive(Vector3 color, Material material)
        {
            this.color = color;
            this.material = material;
        }

        public abstract Vector3 GiveColor(Vector3 intersectionPoint);
        
    }
}
