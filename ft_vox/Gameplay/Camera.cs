using OpenTK;

namespace ft_vox.Gameplay
{
    public class Camera
    {
        public Vector3 Position { get; set; }
        public Vector3 Rotations { get; set; }
        public float Fov { get; set; }

        public Camera(Vector3 position, Vector3 rotations, float fov)
        {
            Position = position;
            Rotations = rotations;
            Fov = fov;
        }

        public void UpdateCameraPosition(Vector3 toPosition, float delta, float speed)
        {
            Position = Vector3.Lerp(Position, toPosition, delta * speed);
        }
        
        public Matrix4 ComputeProjectionMatrix(float ratio)
        {
            return Matrix4.CreatePerspectiveFieldOfView(Fov, ratio, 0.2f, 800f);
        }

        public Matrix4 ComputeViewMatrix()
        {
            return Matrix4.CreateFromQuaternion(new Quaternion(Rotations)) * Matrix4.CreateTranslation(Position);
        }
    }
}
