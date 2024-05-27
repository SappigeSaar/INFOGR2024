using OpenTK.Mathematics;
using System;

namespace raytracer
{
    internal class Light
    {
        /// <summary>
        /// the scenePosition of the light in the scene
        /// </summary>
        public Vector3 scenePosition;

        /// <summary>
        /// the intensity of the light
        /// </summary>
        public float intensity;

        public Light(Vector3 position, float intensity)
        {
            this.scenePosition = position;
            this.intensity = intensity;
        }   
    }
}
