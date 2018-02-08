using ft_vox.Helpers;
using OpenTK;
using System;

namespace ft_vox.Gameplay
{
    class Player
    {
        public Vector3 Position { get; set; }
        public Vector3 Rotations { get; set; }

        public Vector3 EyeForward { get; private set; }
        public Vector3 Forward { get; private set; }
        public Vector3 Right { get; private set; }
        private static readonly Vector3 UP = new Vector3(0, 1, 0).Normalized();
        public Vector3 Velocity { get; set; }

        private static readonly double PION2 = Math.PI / 2;

        public Player()
        {
            Position = new Vector3(4, 1, 4);
        }

        public void Update(double delta)
        {
            var mouseMovement = MouseHelper.GetMouseMovement();
            Rotations += new Vector3(mouseMovement.Y * 0.001f, mouseMovement.X * 0.001f, 0);

            EyeForward = new Vector3((float)Math.Sin(Rotations.Y), -(float)Math.Sin(Rotations.X), -(float)Math.Cos(Rotations.Y));
            Forward = new Vector3((float)Math.Sin(Rotations.Y), 0, -(float)Math.Cos(Rotations.Y));
            Right = new Vector3((float)Math.Sin(Rotations.Y + PION2), 0, -(float)Math.Cos(Rotations.Y + PION2));

            var isShiftPressed = KeyboardHelper.GetKeyboardState().IsKeyDown(OpenTK.Input.Key.ShiftLeft);

            if(KeyboardHelper.GetKeyboardState().IsKeyDown(OpenTK.Input.Key.W) ||
                KeyboardHelper.GetKeyboardState().IsKeyDown(OpenTK.Input.Key.S) ||
                KeyboardHelper.GetKeyboardState().IsKeyDown(OpenTK.Input.Key.A) ||
                KeyboardHelper.GetKeyboardState().IsKeyDown(OpenTK.Input.Key.D) ||
                KeyboardHelper.GetKeyboardState().IsKeyDown(OpenTK.Input.Key.Space) ||
                KeyboardHelper.GetKeyboardState().IsKeyDown(OpenTK.Input.Key.ControlLeft))
            {
                var targetValocity = new Vector3(0);
                if (KeyboardHelper.GetKeyboardState().IsKeyDown(OpenTK.Input.Key.W))
                {
                    targetValocity += Forward * 5 * (isShiftPressed ? 20 : 1);
                }
                if (KeyboardHelper.GetKeyboardState().IsKeyDown(OpenTK.Input.Key.S))
                {
                    targetValocity += -Forward * 5 * (isShiftPressed ? 20 : 1);
                }
                if (KeyboardHelper.GetKeyboardState().IsKeyDown(OpenTK.Input.Key.A))
                {
                    targetValocity += -Right * 5 * (isShiftPressed ? 20 : 1);
                }
                if (KeyboardHelper.GetKeyboardState().IsKeyDown(OpenTK.Input.Key.D))
                {
                    targetValocity += Right * 5 * (isShiftPressed ? 20 : 1);
                }
                if (KeyboardHelper.GetKeyboardState().IsKeyDown(OpenTK.Input.Key.Space))
                {
                    targetValocity += UP * 7 * (isShiftPressed ? 20 : 1);
                }
                if (KeyboardHelper.GetKeyboardState().IsKeyDown(OpenTK.Input.Key.ControlLeft))
                {
                    targetValocity += -UP * 7 * (isShiftPressed ? 20 : 1);
                }
                Velocity = Vector3.Lerp(Velocity, targetValocity, 0.2f);
            }
            else
                Velocity *= 0.8f;
            Position += Velocity * (float)delta;
        }
    }
}
