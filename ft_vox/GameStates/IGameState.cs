namespace ft_vox.GameStates
{
    internal interface IGameState
    {
        void OnLoad(int width, int height);
        void OnUnload();
        void OnResize(int width, int height);

        void Draw(double deltaTime);
        void Update(double deltaTime);

        bool CursorVisible { get; }
    }
}
