using System;
using OpenTK.Mathematics;

namespace raytracer
{
    internal class Intersection
    {
        public Vector3 scenePosition;
        public Vector2 screenPosition;

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
