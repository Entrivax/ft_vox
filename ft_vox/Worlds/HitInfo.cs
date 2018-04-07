namespace ft_vox.Worlds
{
    public class HitInfo
    {
        public int X;
        public int Y;
        public int Z;
        public FaceEnum Face;
        public Chunk Chunk;

        public enum FaceEnum
        {
            Front,
            Back,
            Left,
            Right,
            Top,
            Bottom,
        }
    }
}