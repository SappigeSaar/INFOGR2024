using System;
using OpenTK.Mathematics;

namespace raytracer
{
    internal class Intersection
    {
        /// <summary>
        /// the intersection's scenePosition in the scene
        /// </summary>
        public Vector3 scenePosition;

        /// <summary>
        /// the primitive ath which this intersection takes place
        /// </summary>
        public Primitive closestPrimitive;

        /// <summary>
        /// the normal vactor from the primitive at the point of intersection
        /// </summary>
        public Vector3 normal;

        /// <summary>
        /// this intersections distance from its origin
        /// </summary>
        public float distance;

        public Intersection(Vector3 scenePos,Primitive prim, Vector3 norm)
        {
            this.scenePosition = scenePos;
            this.closestPrimitive = prim;
            this.normal = norm;
        }
    }
}
