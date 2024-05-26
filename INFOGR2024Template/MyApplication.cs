using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace raytracer
{
    class MyApplication
    {
        // member variables
        public Surface screen;
        public Camera camera;
        public Raytracer raytracer;

        //camera values
        private Vector3 cameraPosition = (2f, 2f, 2f);
        private Vector3 cameraLookDirection = (0f,0f,1f); //Z is forward
        private Vector3 cameraUpDirection = (0f, 1f, 0f); //Y is up/down
        private Vector3 cameraRightDirection = (1f, 0f, 0f); //X is left-right
        private float FOVAngle = 90; //in degrees

        // constructor
        public MyApplication(Surface screen)
        {
            this.screen = screen;
            Init();
        }

        // initialize
        public void Init()
        {
            Scene scene = new Scene();
            camera = new Camera(cameraPosition, FOVAngle, cameraLookDirection, cameraUpDirection, cameraRightDirection);
            raytracer = new Raytracer(scene, camera, screen);
        }

        /// <summary>
        /// tick: renders one frame
        /// render the raytracer and update the camera
        /// </summary>
        /// <param name="keyboard"></param>
        public void Tick(KeyboardState keyboard)
        {
            screen.Clear(0);
            //Matrix4 M = Matrix4.CreatePerspectiveFieldOfView(1.6f, 1.0f, 1f, 1000);
            //GL.LoadMatrix(ref M);
            //GL.Translate(0, 0, -1);
            //GL.Rotate(Math.PI * 180 / Math.PI, 0, 0, 0);

            raytracer.Render();
            camera.Update(keyboard);
            //screen.Print("hello world", 2, 2, 0xffffff);
            //screen.Line(2, 20, 160, 20, 0xff0000);
        }
    }
}