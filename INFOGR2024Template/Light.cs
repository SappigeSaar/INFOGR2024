using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace raytracer
{
    internal class Light
    {
        public Vector3 position;
        public float intensity;

        public Light(Vector3 position, float intensity)
        {
            this.position = position;
            this.intensity = intensity;
        }   
    }
}
