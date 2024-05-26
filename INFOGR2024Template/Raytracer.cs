using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Platform.Windows;

namespace raytracer
{
    internal class Raytracer
    {
        public Surface surface;

        /// <summary>
        /// the size of the raytracer screen
        /// </summary>
        private int screenWidth;
        List<Intersection> intersections = new List<Intersection>();

        List<(Vector3, Vector3)> debugPrimaryRays = new List<(Vector3, Vector3)>(); 
        List<(Vector3, Vector3)> debugLightRay = new List<(Vector3, Vector3)> ();
        List<(Vector3, Vector3)> debugMirrorRay = new List<(Vector3, Vector3)>();

        private Camera camera;
        private Scene scene;

        public Raytracer(Scene scene, Camera camera, Surface surface)
        {
            this.scene = scene;
            this.camera = camera;
            this.surface = surface;
            
            screenWidth = surface.width /2;
        }

        /// <summary>
        /// updates the raytracer
        /// </summary>
        public void Update()
        {
            Render();
            Debug();
        }

        /// <summary>
        /// renders the scene
        /// </summary>
        public void Render()
        {
            //clear everything that has been drawn/calculated previously
            surface.Clear(0);
            intersections.Clear();
            debugPrimaryRays.Clear();
            debugMirrorRay.Clear();
            debugLightRay.Clear();

            Intersection currentPixel;
            Vector3 rgbValues;
            for(int x = 0; x < screenWidth; x++)
                for(int y = 0; y < surface.height; y++)
                {
                    currentPixel = GetPrimaryIntersection(x, y);
                    if (currentPixel != null)
                    {
                        rgbValues = GetColor(x, y, currentPixel, camera.position, 0);
                        surface.pixels[x + y * surface.width] = MixColor(rgbValues);
                    }
                }
            Debug();
        }

        #region Debug

        /// <summary>
        /// draws the debug window
        /// </summary>
        public void Debug()
        {
            //draw the scene
            foreach(Sphere sphere in scene.sphereList)
            {
                DrawDebugSphere(sphere);
            }
            int xCoor;
            int yCoor;
            for (int x = -1; x < 2; x++)
                for (int y = -1; y <2; y++)
                {
                    xCoor = TransformX(camera.position.X) + x;
                    yCoor = TransformZ(camera.position.Z) + y;
                    try
                    {
                        surface.pixels[xCoor + (surface.height - yCoor) * surface.width] = MixColor((1, 1, 0));
                    }
                    catch
                    {

                    }
                }
            surface.Line(TransformX(camera.p0.X), (surface.height - TransformZ(camera.p0.Z)), TransformX(camera.p1.X), (surface.height - TransformZ(camera.p1.Z)), 0xff0000);

            int itemcount = debugPrimaryRays.Count -1;
            while (itemcount >= 0)
            {
                Vector3 startline = debugPrimaryRays[0].Item1;
                Vector3 endline = debugPrimaryRays[itemcount].Item2;

                int startlinex = TransformX(startline.X);
                int startliney = screenWidth - TransformZ(startline.Z);

                int endlinex = TransformX(endline.X);   
                int endliney = screenWidth - TransformZ(endline.Z);

                surface.Line(startlinex, startliney, endlinex, endliney, MixColor((1, 0, 0)));
                itemcount--;
            }

            //draw the mirror rays
            itemcount = debugMirrorRay.Count - 1;
            while (itemcount >= 0)
            {
                Vector3 startline = debugMirrorRay[0].Item1;
                Vector3 endline = debugMirrorRay[itemcount].Item2;

                int startlinex = TransformX(startline.X);
                int startliney = screenWidth - TransformZ(startline.Z);

                int endlinex = TransformX(endline.X);
                int endliney = screenWidth - TransformZ(endline.Z);

                surface.Line(startlinex, startliney, endlinex, endliney, MixColor((0, 0, 1)));
                itemcount--;
            }

            //draw the lightRays
            itemcount = debugLightRay.Count - 1;
            while (itemcount >= 0)
            {
                Vector3 startline = debugLightRay[0].Item1;
                Vector3 endline = debugLightRay[itemcount].Item2;

                int startlinex = TransformX(startline.X);
                int startliney = screenWidth - TransformZ(startline.Z);

                int endlinex = TransformX(endline.X);
                int endliney = screenWidth - TransformZ(endline.Z);

                surface.Line(startlinex, startliney, endlinex, endliney, MixColor((1, 0, 1)));
                itemcount--;
            }

        }

        /// <summary>
        /// turns a sceneposition into a screenposition
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public int TransformX(float x)
        {
            x = x * (float)(screenWidth/ 10.0f);
            x += screenWidth;
            int xInt = (int)x;
            return xInt;
        }

        public int TransformZ(float z)
        {
            float aspectRatio = (float)screenWidth / (float)surface.height;
            z *= 1 * (surface.height / 10.0f); // aspectRatio;
            int zInt = (int)z;
            return zInt;
        }

        private void DrawDebugSphere(Sphere sphere)
        {
            for (int i =0; i < 360; i++)
            {
                double radians = i * (Math.PI / 180);
                int x = TransformX((float)(sphere.position.X + sphere.radius * Math.Cos(radians)));
                int y = TransformX((float)(sphere.position.Z + sphere.radius * Math.Sin(radians)));
                try
                {
                    surface.pixels[x + (surface.height - y) * (surface.width)] = MixColor(sphere.color);
                }
                catch { }
            }
        }

        #endregion

        #region Intersection

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private Intersection GetPrimaryIntersection(int x, int y)
        {
            float xCoor = (float)(x / (float)screenWidth);
            float yCoor = (float) (y / (float)surface.height);
            //translate the pixel coordinates into a point in 3d space on the camera's image plane
            Vector3 rayOrigin = camera.p0 + xCoor * (camera.p1 - camera.p0) + yCoor * (camera.p2 - camera.p0);

            Vector3 rayDirection = Vector3.Normalize(rayOrigin - camera.position);

            Vector3 debugRay;

            //getClosestIntersection (which already checks if the length is positive
            Intersection intersection = ClosestIntersection(rayOrigin, rayDirection);

            if (intersection != null)
            {
                //draw the debug line
                debugRay = intersection.scenePosition;

            }
            else
            {
                debugRay = camera.position + 10f * rayDirection; 
            }
            //if this intersection is not null, add it to the list of intersections??

            //
            if (y == 4 && x % 2 == 0)
                debugPrimaryRays.Add((camera.position, debugRay));

            //return this intersection
            return intersection;
        }

        private Intersection ClosestIntersection(Vector3 rayOrigin, Vector3 rayDirection)
        {
            float length = float.MaxValue;

            Intersection finalIntersection = null;

            //check if the ray intersects with a sphere
            ////and set the finalIntersectio to the closest sphere
            foreach(Sphere sphere in scene.sphereList)
            {
                Intersection currentIntersection = IntersectSphere(rayOrigin, rayDirection, sphere);
                if (currentIntersection != null && currentIntersection.distance <= length)
                {
                    finalIntersection = currentIntersection;
                    length = finalIntersection.distance;
                }
                //length is smallest intersection length
            }

            foreach (Plane plane in scene.planeList)
            {
                Intersection currentIntersection = IntersectPlane(rayOrigin, rayDirection, plane);
                if (currentIntersection != null && currentIntersection.distance <= length)
                {
                    finalIntersection = currentIntersection;
                    length = finalIntersection.distance;
                }
            }
            return finalIntersection;
        }

        //extrra arg float length??
        private Intersection IntersectSphere(Vector3 rayOrigin, Vector3 rayDirection, Sphere sphere)
        {
            float currentlength = float.MaxValue;

            float a = (float)(rayDirection.X * rayDirection.X + rayDirection.Y * rayDirection.Y + rayDirection.Z * rayDirection.Z);
            float b = Vector3.Dot( rayDirection *2f , rayOrigin - sphere.position);
            float c = Vector3.Dot(rayOrigin - sphere.position, rayOrigin - sphere.position);
            float d = (float)(b * b - 4f * a * c);

            Intersection intersection = null;

            if (d>= 0)
            {
                //if there are two intersections, set the length to the closest intersection
                if (d>0)
                {
                    float l1 = (float)((-b - Math.Sqrt(d)) / (2 * a));
                    float l2 = (float)((-b + Math.Sqrt(d)) / (2 * a));

                    if (l1 <= l2)
                        currentlength = l1;
                    else
                        currentlength = l2;
                }
                //if there is one intersection, take that length
                else if (d ==0)
                    currentlength = (float)(-b / (2*a));

                //if (currentlength <= length)
                intersection = MakeNewIntersection(currentlength, rayOrigin, rayDirection, sphere);
                intersection.distance = currentlength;
            }
            return intersection;
        }

        private Intersection IntersectPlane(Vector3 rayOrigin, Vector3 rayDirection, Plane plane)
        {
            float a = Vector3.Dot(2 * rayDirection, plane.normal);
            float b = Vector3.Dot(2 * rayOrigin, plane.normal);
            float length = (-b / a) / 2;

            Intersection intersection = null;

            if (length>0 && length<30)
            {
                intersection = MakeNewIntersection(length, rayOrigin, rayDirection, plane);
                intersection.distance = length;
            }
            return intersection;
        }

        /// <summary>
        /// makes a new intersection object
        /// </summary>
        /// <param name="length"></param>
        /// <param name="point"></param>
        /// <param name="ray"></param>
        /// <param name="Primitive">the primitive at which the intersection takes place</param>
        /// <returns></returns>
        private Intersection MakeNewIntersection(float length, Vector3 rayOrigin, Vector3 rayNormal, Primitive primitive)
        {
            Vector3 intersectionCoordinate = rayOrigin + length * rayNormal;

            if (primitive is Sphere)
            {
                Sphere sphere = (Sphere)primitive;
                Vector3 normal = (intersectionCoordinate - sphere.position) / sphere.radius;
                return new Intersection(intersectionCoordinate, sphere, normal);
            }
            else
            {
                Plane plane = (Plane)primitive;
                Vector3 normal = plane.normal;
                return new Intersection(intersectionCoordinate, plane, normal);
            }

        }

        #endregion

        /// <summary>
        /// gets the color for the given intersection
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="intersection"></param>
        /// <param name="previousIntersection"></param>
        /// <param name="bounceCount"></param>
        /// <returns></returns>
        private Vector3 GetColor(int x, int y, Intersection intersection, Vector3 previousIntersection, int bounceCount)
        {
            Vector3 primitiveColor = intersection.closestPrimitive.GiveColor(intersection.scenePosition);
            float ambientLight = 0.15f;

            Vector3 colorvalues = (1f, 1f, 1f);

            float red = 0f;
            float green = 0f;
            float blue = 0f;

            //specular stuff is regardless of light hitting it or not)
            if (intersection.closestPrimitive.material == Primitive.Material.specular)
            {
                bounceCount++;
                Vector3 offsetIntersection = intersection.scenePosition = 1f * intersection.normal;

                //shoot a ray from this intersection to see if we make an intersection with an other object
                Vector3 viewRay = Vector3.Normalize(offsetIntersection - previousIntersection);
                Vector3 mirrorRay = Vector3.Normalize(viewRay - (2 * Vector3.Dot(viewRay, intersection.normal) * intersection.normal));
                Intersection reflectedIntersection = ClosestIntersection(mirrorRay, offsetIntersection);

                //if we dont, return an empty color
                if (reflectedIntersection == null)
                    return (0, 0, 0);
                if (y == 2 && x % 4 == 0)
                {
                    //Vector3 debugLine = intersection.scenePosition;
                    debugMirrorRay.Add((offsetIntersection, reflectedIntersection.scenePosition));

                }
                //and go into recursion for the color of this new intersection
                if (bounceCount < 5)
                {
                    colorvalues = GetColor(x, y, reflectedIntersection, intersection.scenePosition, bounceCount);

                }
                red += colorvalues.X * 1f;
                green += colorvalues.Y * 1f;
                blue += colorvalues.Z * 1f;
            }

            foreach (Light light in scene.lightList)
            {
                float lightDistance = Vector3.Distance(intersection.scenePosition, light.position);
                //vector from the ligtpoint to the intersection
                Vector3 lightRay = Vector3.Normalize(intersection.scenePosition - light.position);

                bool intersects = false;
                //check if this lightray does not make any other intersections before comming to the current intersection.

                Intersection otherIntersection = ClosestIntersection(light.position, lightRay);
                //draw debug line
                if (intersection.closestPrimitive is Sphere && y == 2 && x % 4 == 0)
                {
                    Vector3 debugLine = intersection.scenePosition;
                    debugLightRay.Add((light.position, debugLine));
                }
                    
                if (otherIntersection != intersection)
                
                {
                    //shadow debug ray??
                    intersects = true;
                }

                
                if (!intersects)
                {
                    //mNormal = toLightRay
                    Vector3 toLightRay = Vector3.Normalize(light.position - intersection.scenePosition);
                    float dotproduct = Vector3.Dot(toLightRay, intersection.normal);
                    float diffuse = dotproduct;
                    //float diffuse = Mat(0, dotproduct);

                    
                    if (intersection.closestPrimitive.material == Primitive.Material.diffuse)
                    {
                        //just diffuse
                        red += ((light.intensity * (1.0f / (lightDistance * lightDistance)) * ((diffuse * primitiveColor.X) + (0.8f * 0.1f))));
                        green += ((light.intensity * (1.0f / (lightDistance * lightDistance)) * ((diffuse * primitiveColor.X) + (0.8f * 0.1f))));
                        blue += ((light.intensity * (1.0f / (lightDistance * lightDistance)) * ((diffuse * primitiveColor.X) + (0.8f * 0.1f))));
                    }
                    if (intersection.closestPrimitive.material == Primitive.Material.glossy)
                    {
                        //diffuse + glossy
                        Vector3 r = toLightRay - 2 * dotproduct * intersection.normal;
                        Vector3 v = previousIntersection - intersection.scenePosition;
                        dotproduct = Vector3.Dot(v, r);
                        red += ((light.intensity * (1.0f / (lightDistance * lightDistance)) * ((diffuse * primitiveColor.X) + (0.8f * dotproduct))));
                        green += ((light.intensity * (1.0f / (lightDistance * lightDistance)) * ((diffuse * primitiveColor.X) + (0.8f * dotproduct))));
                        blue += ((light.intensity * (1.0f / (lightDistance * lightDistance)) * ((diffuse * primitiveColor.X) + (0.8f * dotproduct))));

                    }
                }
            }

            //add ambient lighting
            if (intersection.closestPrimitive.material == Primitive.Material.specular)
            {
                //can be moved to the spec function>>
                red += colorvalues.X * 0.8f;
                green += colorvalues.Y * 0.8f;
                blue += colorvalues.Z * 0.8f;
            }
            else
            {
                red += primitiveColor.X * ambientLight;
                green += primitiveColor.Y * ambientLight;
                blue += primitiveColor.Z * ambientLight;
            }

            return new Vector3(red, green, blue);
        }

        /// <summary>
        /// mixes the rgb values into a single int, which the screen knows how to dispplay
        /// </summary>
        /// <param name="rgb"></param>
        /// <returns></returns>
        private int MixColor(Vector3 rgb)
        {
            int red = (int)(rgb.X * 255);
            int green = (int)(rgb.Y * 255);
            int blue = (int)(rgb.Z * 255);

            if (red > 255)
                red = 255;
            else if (red < 0)
                red = 0;
            if (green > 255)
                green = 255;
            else if (green < 0)
                green = 0;
            if (blue > 255)
                blue = 255;
            else if (blue < 0)
                blue = 0;


            return (red << 16) + (green <<8) + blue;
        }



    }
}
