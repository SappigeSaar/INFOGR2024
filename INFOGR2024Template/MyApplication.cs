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
        private Vector3 cameraPosition = (3f, 2f, 0f);
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

        /// <summary>
        /// initialize the application by creating the scene, camera and raytracer with the determined values
        /// </summary>
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
        /// <param name="keyboard">the current keyboardstate passed from the template</param>
        public void Tick(KeyboardState keyboard)
        {
            //clear the screen
            screen.Clear(0);

            camera.Update(keyboard);
            raytracer.Update();
            
        }
    }
}