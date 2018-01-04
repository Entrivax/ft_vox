using OpenTK.Input;

namespace ft_vox.Helpers
{
    public static class KeyboardHelper
    {
        private static KeyboardState _lastKeyState;
        private static KeyboardState _keyState;

        public static void Update()
        {
            _lastKeyState = _keyState;
            _keyState = Keyboard.GetState();
        }

        public static bool IsKeyPressed(Key key)
        {
            return _keyState.IsKeyDown(key) && !_lastKeyState.IsKeyDown(key);
        }

        public static KeyboardState GetKeyboardState()
        {
            return _keyState;
        }
    }
}
