using ft_vox.Helpers;
using OpenTK;
using System;

namespace ft_vox.Gameplay
{
    class Player
    {
        public Vector3 Position { get; set; }
        public Vector3 Rotations { get; set; }

        public Vector3 Forward { get; private set; }
        private static readonly Vector4 FORWARD = new Vector4(0, 0, 1, 1).Normalized();
        public Vector3 Velocity { get; set; }

        public Player()
        {
            Position = new Vector3(4, 1, 4);
        }

        public void Update(double delta)
        {
            var mouseMovement = MouseHelper.GetMouseMovement();
            Rotations += new Vector3(mouseMovement.Y * 0.001f, mouseMovement.X * 0.001f, 0);

            Forward = new Vector3((float)Math.Sin(Rotations.Y), -(float)Math.Sin(Rotations.X), -(float)Math.Cos(Rotations.Y));

            var isShiftPressed = KeyboardHelper.GetKeyboardState().IsKeyDown(OpenTK.Input.Key.ShiftLeft);

            if (KeyboardHelper.GetKeyboardState().IsKeyDown(OpenTK.Input.Key.W))
            {
                Velocity = Vector3.Lerp(Velocity, Forward * 5 * (isShiftPressed ? 20 : 1), 0.7f);
            }
            else if (KeyboardHelper.GetKeyboardState().IsKeyDown(OpenTK.Input.Key.S))
            {
                Velocity = Vector3.Lerp(Velocity, -Forward * 5 * (isShiftPressed ? 20 : 1), 0.7f);
            }
            else
                Velocity *= 0.3f;
            Position += Velocity * (float)delta;
        }
    }
}
