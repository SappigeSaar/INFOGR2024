using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace raytracer
{
    internal class Camera
    {
        /// <summary>
        /// camera position
        /// </summary>
        public Vector3 position;

        /// <summary>
        /// image plane center
        /// </summary>
        public Vector3 center;


        //image plane corners
        /// <summary>
        /// topleft
        /// </summary>
        public Vector3 p0;
        /// <summary>
        /// topright
        /// </summary>
        public Vector3 p1;
        /// <summary>
        /// bottemleft
        /// </summary>
        public Vector3 p2; 

        /// <summary>
        /// orthonormal basis
        /// </summary>
        public Vector3 viewDirection;
        private Vector3 upDirection;
        private Vector3 rightDirection;

        /// <summary>
        /// focal length
        /// </summary>
        public float FOV;

        public Camera(Vector3 pos, float focalAngle, Vector3 viewDir, Vector3 upDir, Vector3 rightDir)
        {
            position = pos;
            viewDirection = viewDir;
            upDirection = upDir;
            rightDirection = rightDir;
            FOV = SetFOV(focalAngle);
            SetCenterAndCorners(1);

        }

        /// <summary>
        /// sets the focal length based on the given angle(in degrees)
        /// </summary>
        /// <param name="angle">FOV angle in degrees</param>
        /// <returns>focallength</returns>
        public float SetFOV(float angle)
        {
            return (1 / (float)Math.Tan((Math.PI / 180.0d) * (angle / 2)));
        }

        /// <summary>
        /// updates the camera's position and direction based on the keyboardState
        /// </summary>
        /// <param name="keyboard">takes the keyboardState to update the camera's position</param>
        public void Update(KeyboardState keyboard)
        {
            ChangeCameraOrientation(keyboard);
            ChangeCamerapPosition(keyboard);  

            SetCenterAndCorners(FOV);
        }

        //does it need to be 1 at the start??
        /// <summary>
        /// sets the values for the image plane based on the current directions
        /// </summary>
        /// <param name="FOV"></param>
        private void SetCenterAndCorners(float FOV)
        {
            center = position + FOV * viewDirection;

            p0 = center + upDirection - rightDirection; //topleft
            p1 = center + upDirection + rightDirection; //top right
            p2 = center - upDirection - rightDirection; //botem left
        }

        

        /// <summary>
        /// moves the camera based on the W A S D keys and shift and space
        /// W A S D is for horizontal movement, shift is for down and shift is for up
        /// (based on game controlls)
        /// </summary>
        /// <param name="keyboard">the current keyboardState</param>
        private void ChangeCamerapPosition(KeyboardState keyboard)
        {
            float moveSpeed = 1f;

            //move left or right
            if (keyboard[Keys.D])
                position.X += moveSpeed;
            else if (keyboard[Keys.A])
                position.X -= moveSpeed;

            //move forward or backwards
            if (keyboard[Keys.W])
                position.Z += moveSpeed;
            else if (keyboard[Keys.S])
                position.Z -= moveSpeed;

            //move up or down
            if(keyboard[Keys.LeftShift])
                position.Y += moveSpeed;
            else if (keyboard[Keys.Space])
                position.Y -= moveSpeed;

        }

        /// <summary>
        /// changes where the camera is pointed based on the keyboardstate
        /// </summary>
        /// <param name="keyboard">the current keyboardState</param>
        private void ChangeCameraOrientation(KeyboardState keyboard)
        {
            float moveSpeed = 1f;

            //point up or down
            if (keyboard[Keys.Up])
                MoveVertical(moveSpeed);
            else if (keyboard[Keys.Down])
                MoveVertical(-moveSpeed);

            //point left or right
            if (keyboard[Keys.Left])
                MoveHorizontal(moveSpeed);
            if (keyboard[Keys.Right])
                MoveHorizontal(-moveSpeed);
        }

        /// <summary>
        /// move the camera vertically
        /// </summary>
        /// <param name="distance">hown much the camere should move</param>
        private void MoveVertical(float distance)
        {
            double angle = (Math.PI / 180.0d) + distance;
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            //set the new viewdirection
            viewDirection = new Vector3(viewDirection.X,
                                        cos * viewDirection.Y - sin * viewDirection.Z,
                                        cos * viewDirection.Z + sin * viewDirection.Y);
            viewDirection.Normalize();

            //set the new updirection
            upDirection = new Vector3(upDirection.X,
                                      cos * upDirection.Y - sin * upDirection.Z,
                                      cos * upDirection.Z + sin * upDirection.Y);
            upDirection.Normalize();
        }

        /// <summary>
        /// move the camera horizontally
        /// </summary>
        /// <param name="distance"></param>
        private void MoveHorizontal(float distance)
        {
            double angle = (Math.PI / 180.0d) + distance;
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            //set the new viewdirection
            viewDirection = new Vector3(cos * viewDirection.X - sin * viewDirection.Z,
                                         viewDirection.Y,
                                         cos * viewDirection.Z + sin * viewDirection.X);
            viewDirection.Normalize();

            //set the new rightdirection
            rightDirection = new Vector3(cos * rightDirection.X - sin * rightDirection.Z,
                                         rightDirection.Y,
                                         cos * rightDirection.Z + sin * rightDirection.X);
            rightDirection.Normalize();
        }

    }
}
