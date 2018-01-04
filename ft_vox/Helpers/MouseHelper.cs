using OpenTK;
using OpenTK.Input;

namespace ft_vox.Helpers
{
    public class MouseHelper
    {
        private static MouseState _lastMouseState;
        private static MouseState _mouseState;

        public static void Update()
        {
            _lastMouseState = _mouseState;
            _mouseState = Mouse.GetState();
        }

        public static Vector2 GetMouseMovement()
        {
            return new Vector2(_mouseState.X - _lastMouseState.X, _mouseState.Y - _lastMouseState.Y);
        }
    }
}
