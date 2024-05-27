using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Platform.Windows;
using SixLabors.ImageSharp;

namespace raytracer
{
    internal class Raytracer
    {
        public Surface surface;

        /// <summary>
        /// the size of the raytracer screen
        /// </summary>
        private int screenWidth;

        
        //the lists for all the debug ray's
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
        /// updates the raytracer by calling render and debug
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
            
            //get all the primary intersections and calculate their color
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
            
        }

        #region Debug

        /// <summary>
        /// draws the debug window
        /// </summary>
        public void Debug()
        {

            //draw the scene
            foreach (Sphere sphere in scene.sphereList)
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

            //draw the primary rays
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

            debugPrimaryRays.Clear();
            debugMirrorRay.Clear();
            debugLightRay.Clear();
        }

        /// <summary>
        /// turns a sceneposition into a screenposition
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public int TransformX(float x)
        {
            x = x * (float)(screenWidth/ 15.0f);
            x += screenWidth;
            int xInt = (int)x;
            return xInt;
        }

        public int TransformZ(float z)
        {
            float aspectRatio = (float)screenWidth / (float)surface.height;
            z *= 1 * (surface.height / 15.0f); // aspectRatio;
            int zInt = (int)z;
            return zInt;
        }

        private void DrawDebugSphere(Sphere sphere)
        {
            for (int i =0; i < 360; i++)
            {
                double radians = i * (Math.PI / 180);
                int x = TransformX((float)(sphere.scenePosition.X + sphere.radius * Math.Cos(radians)));
                int y = TransformX((float)(sphere.scenePosition.Z + sphere.radius * Math.Sin(radians)));
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
        /// calculates the primary intersection for the given screen-coordinate
        /// </summary>
        /// <param name="x">screen pixel Coordinate x</param>
        /// <param name="y">screeen pixel coordinate y</param>
        /// <returns></returns>
        private Intersection GetPrimaryIntersection(int x, int y)
        {
            float xCoor = (float)(x / (float)screenWidth);
            float yCoor = (float) (y / (float)surface.height);

            //translate the pixel coordinates into a point in 3d space on the camera's image plane
            Vector3 rayOrigin = camera.p0 + xCoor * (camera.p1 - camera.p0) + yCoor * (camera.p2 - camera.p0);

            Vector3 rayDirection = Vector3.Normalize(rayOrigin - camera.position);

            

            //get the closest intersection (which only returns if the length is positive)
            Intersection intersection = ClosestIntersection(camera.position, rayDirection);

            Vector3 debugRay;
            //set the debug line
            if (intersection != null)
                debugRay = intersection.scenePosition;
            else
                debugRay = rayOrigin + 10f * rayDirection; 
            
            //draw the debug rays
            if (yCoor % 4 == 0 && xCoor % 4 == 0)
                debugPrimaryRays.Add((rayOrigin, debugRay));

            //return this intersection//which might be null if there is nothing to intersect
            return intersection;
        }

        /// <summary>
        /// calculates the closest intersection for the given ray
        /// returns null if there is no intersection
        /// </summary>
        /// <param name="rayOrigin">the origin point of the ray</param>
        /// <param name="rayDirection">the normalised direction of the ray</param>
        /// <returns>the closest intersection in a positive direction, null if there is no intersection</returns>
        private Intersection ClosestIntersection(Vector3 rayOrigin, Vector3 rayDirection)
        {
            float length = float.MaxValue;

            Intersection finalIntersection = null;

            //check if the ray intersects with a sphere
            //and set the finalIntersection to the closest sphere
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

            //check if there is a plane witha closer intersection
            foreach (Plane plane in scene.planeList)
            {
                Intersection currentIntersection = IntersectPlane(rayOrigin, rayDirection, plane);
                if (currentIntersection != null && currentIntersection.distance <= length)
                {
                    finalIntersection = currentIntersection;
                    length = finalIntersection.distance;
                }
            }

            //return the finalintersection, which is null if there was no intersection
            return finalIntersection;
        }

        /// <summary>
        /// checks if the given ray intersects with the given sphere
        /// if it does it creates an intersection for the intersection closest to the origin
        /// if there is no sphere intersection it returns null;
        /// </summary>
        /// <param name="rayOrigin">the origin of the ray</param>
        /// <param name="rayDirection">the normalised direction of the ray</param>
        /// <param name="sphere">the sphere we check for intersections</param>
        /// <returns>either null of the closest intersection</returns>
        private Intersection IntersectSphere(Vector3 rayOrigin, Vector3 rayDirection, Sphere sphere)
        {
            float currentlength = float.MaxValue;

            float a = (float)(rayDirection.X * rayDirection.X + rayDirection.Y * rayDirection.Y + rayDirection.Z * rayDirection.Z);
            float b = Vector3.Dot( rayDirection *2f , rayOrigin - sphere.scenePosition);
            float c = Vector3.Dot(rayOrigin - sphere.scenePosition, rayOrigin - sphere.scenePosition) - (float)(sphere.radius * sphere.radius);
            float d = (float)(b * b - 4f * a * c);

            Intersection intersection = null;

            //if d is positive, there has been an intersection
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

                //make sure the length is not negative, so we only "see" things that are in the direction of the ray
                if (currentlength >= 0)
                {
                    intersection = MakeNewIntersection(currentlength, rayOrigin, rayDirection, sphere);
                    intersection.distance = currentlength;
                }
            }
            return intersection;
        }

        /// <summary>
        /// checks if the given ray intersects with the given plane
        /// if it does it creates an intersection
        /// if it does not it returns null
        /// </summary>
        /// <param name="rayOrigin">the origin of the ray</param>
        /// <param name="rayDirection">the normalised direction of the ray</param>
        /// <param name="plane">the sphere we check for intersections</param>
        /// <returns>either null of the closest intersection</returns>
        private Intersection IntersectPlane(Vector3 rayOrigin, Vector3 rayDirection, Plane plane)
        {
            float a = Vector3.Dot(2 * rayDirection, plane.normal);
            float b = Vector3.Dot(2 * rayOrigin, plane.normal);
            float length = (-b / a) / 2;

            Intersection intersection = null;

            //make sure the length is not negative, so we only "see" things that are in the direction of the ray
            if (length>0 )
            {
                intersection = MakeNewIntersection(length, rayOrigin, rayDirection, plane);
                intersection.distance = length;
            }
            return intersection;
        }

        /// <summary>
        /// makes a new intersection object
        /// </summary>
        /// <param name="length">how far th intersection is from its origin point</param>
        /// <param name="rayOrigin">the origin point of the incomming ray</param>
        /// <param name="rayDirection">the normalised direction of the incomming ray</param>
        /// <param name="primitive">the primitive at which the intersection takes place</param>
        /// <returns>the intersectionObject of the given ray</returns>
        private Intersection MakeNewIntersection(float length, Vector3 rayOrigin, Vector3 rayDirection, Primitive primitive)
        {
            rayDirection = Vector3.Normalize(rayDirection);
            Vector3 intersectionCoordinate = rayOrigin + length * rayDirection;
            Intersection intersection;

            //see what kind of primitive this is
            if (primitive is Sphere)
            {
                Sphere sphere = (Sphere)primitive;
                Vector3 normal = Vector3.Normalize(intersectionCoordinate - sphere.scenePosition); 
                intersection = new Intersection(intersectionCoordinate, sphere, normal);
            }
            else
            {
                Plane plane = (Plane)primitive;
                Vector3 normal = plane.normal;
                intersection = new Intersection(intersectionCoordinate, plane, normal);
            }

            //set the intrsection distance
            intersection.distance = length;

            return intersection;
        }

       

        #endregion

        /// <summary>
        /// gets the color for the given intersection
        /// </summary>
        /// <param name="x"> x-coordinate of the pixel on screen</param>
        /// <param name="y"> y-coordinate of the pixel on screen</param>
        /// <param name="intersection">the intersection for the given pixel</param>
        /// <param name="previousIntersection">the origin of the intersectionray</param>
        /// <param name="bounceCount">hown many times we have bounced yet</param>
        /// <returns>the correct color for this intersection</returns>
        private Vector3 GetColor(int x, int y, Intersection intersection, Vector3 previousIntersection, int bounceCount)
        {
            Vector3 primitiveColor = intersection.closestPrimitive.GiveColor(intersection.scenePosition);
            float ambientLight = 0.15f;

            Vector3 colorvalues = (1f, 1f, 1f);

            float red = 0f;
            float green = 0f;
            float blue = 0f;

            //calculate the specular stuff, which is regardless of light hitting it or not)
            if (intersection.closestPrimitive.material == Primitive.Material.specular)
            {
                bounceCount++;
                Vector3 offsetIntersection = intersection.scenePosition + 0.1f * intersection.normal;

                //shoot a ray from this intersection to see if we make an intersection with an other object
                Vector3 viewRay = Vector3.Normalize(offsetIntersection - previousIntersection);
                Vector3 mirrorRay = Vector3.Normalize(viewRay - 2 * (Vector3.Dot(viewRay, intersection.normal)) * intersection.normal);
                
                Intersection reflectedIntersection = ClosestIntersection(offsetIntersection, mirrorRay);

                //if we dont, return an empty color
                if (reflectedIntersection == null)
                    return (0, 0, 0);

                //set the debug rays
                if (y % 10 == 0 && x % 10 == 0)
                {
                    //Vector3 debugLine = intersection.scenePosition;
                    debugMirrorRay.Add((offsetIntersection, reflectedIntersection.scenePosition));

                }

                //and go into recursion for the color of this new intersection
                if (bounceCount < 5)
                {
                    colorvalues = GetColor(x, y, reflectedIntersection, intersection.scenePosition, bounceCount);

                }
                red += colorvalues.X ;
                green += colorvalues.Y;
                blue += colorvalues.Z;
            }

            //color stuff for the non-specular materials
            foreach (Light light in scene.lightList)
            {
                float lightDistance = Vector3.Distance(intersection.scenePosition, light.scenePosition);
                //vector from the lightpoint to the intersection
                Vector3 lightRay = Vector3.Normalize(intersection.scenePosition - light.scenePosition);

                bool intersects = false;
                //check if this lightray does not make any other intersections before comming to the current intersection.
                Intersection otherIntersection = ClosestIntersection(light.scenePosition, lightRay);

                
                //if the intersections are not the same, then there is an obstacle before this light hits the primitive
                if (otherIntersection != null)
                {
                    //the program might get a slightly different position, while still landing on the same primitive,
                    //so we check if the intersection falls within a set margin
                    if (lightDistance >= otherIntersection.distance + 0.0001f && lightDistance <= otherIntersection.distance - 0.0001f)
                        intersects = true;

                    //draw debug line
                    if (otherIntersection.closestPrimitive is Sphere && y % 20 == 0 && x % 20 == 0) //intersection.closestPrimitive is Sphere &&
                    {
                        Vector3 debugLine = otherIntersection.scenePosition;
                        debugLightRay.Add((light.scenePosition, debugLine));
                    }
                }

                //if there is no obstacle intersection, calculate the color and light normally
                if (intersects == false)
                {
                    int n = 20;
                    float ks = 0.8f;
                    
                    Vector3 toLightRay = Vector3.Normalize(light.scenePosition - intersection.scenePosition); 
                    
                    //calculate the colors based on the material
                    if (intersection.closestPrimitive.material == Primitive.Material.diffuse)
                    {
                        float diffuse = light.intensity * (1.0f / (lightDistance * lightDistance)) * Math.Max(0, Vector3.Dot(intersection.normal, toLightRay));
                        red   +=  diffuse * primitiveColor.X;
                        green += diffuse * primitiveColor.Y;
                        blue  += diffuse * primitiveColor.Z;
                    }
                    if (intersection.closestPrimitive.material == Primitive.Material.glossy)
                    {
                        Vector3 r = toLightRay - 2f * Vector3.Dot(toLightRay, intersection.normal) * intersection.normal;
                        Vector3 v = Vector3.Normalize(previousIntersection - intersection.scenePosition);

                        float glossy = light.intensity * (1.0f / (lightDistance * lightDistance)) * (float)Math.Pow(Math.Max(0, Vector3.Dot(v, r)), n);

                        red += glossy * ks;
                        green += glossy * ks;
                        blue += glossy * ks;

                    }
                    if (intersection.closestPrimitive.material == Primitive.Material.diffuseGlossyCombo)
                    {
                        
                        Vector3 r = toLightRay - 2f * Vector3.Dot(toLightRay, intersection.normal) * intersection.normal;
                        Vector3 v = Vector3.Normalize(previousIntersection - intersection.scenePosition);
                        
                        float diffuse = Math.Max(0, Vector3.Dot(intersection.normal, toLightRay));
                        float glossy = (float) Math.Pow(Math.Max(0, Vector3.Dot(v, r)),n);

                        float combo = light.intensity * (1.0f / (lightDistance * lightDistance)) ;

                        red += combo * (primitiveColor.X * diffuse + ks * glossy);
                        green += combo * (primitiveColor.Y * diffuse + ks * glossy);
                        blue += combo * (primitiveColor.Z * diffuse + ks * glossy);

                    }
                }
            }

            //add ambient lighting
            if (intersection.closestPrimitive.material != Primitive.Material.specular)
            
            {
                red += primitiveColor.X * ambientLight;
                green += primitiveColor.Y * ambientLight;
                blue += primitiveColor.Z * ambientLight;
            }

            return new Vector3(red, green, blue);
        }

        /// <summary>
        /// mixes the rgb values into a single int, which the screen knows how to display
        /// makes sure there is no overload fro the values
        /// </summary>
        /// <param name="rgb">the separate red green blue values</param>
        /// <returns>a single int representing the color</returns>
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
