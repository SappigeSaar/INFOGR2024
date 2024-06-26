﻿using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace raytracer
{
    internal class Plane : Primitive
    {
        /// <summary>
        /// the planes normal vector
        /// </summary>
        public Vector3 normal;

        //vectors perpendicular to the normal and to each other
        //only used for the pattern
        private Vector3 uNormal;
        private Vector3 vNormal;

        public Vector3 pointZero;

        /// <summary>
        /// the contrast color for the checkerboard that makes up the plane
        /// </summary>
        public Vector3 contrastColor;


        public Plane(Vector3 norm, Vector3 point, Vector3 color, Material material) : base(color, material)
        {
            this.normal = Vector3.Normalize(norm);

            //set the uNormal and the vNormal
            this.uNormal = Vector3.Cross(normal, (normal.Z, normal.X, normal.Y));
            uNormal.Normalize();
            this.vNormal = Vector3.Cross(normal, uNormal);
            vNormal.Normalize();

            this.uNormal = Vector3.Cross(normal, vNormal);
            uNormal.Normalize();


            this.pointZero = point;

            contrastColor = new Vector3(1, 1, 1) - color;

            
        }

        /// <summary>
        /// returns the correct color for the given intersection, to create the checkerboard pattern
        /// </summary>
        /// <param name="intersectionPoint">the intersection for which we want to color</param>
        /// <returns>one of the two colors</returns>
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
