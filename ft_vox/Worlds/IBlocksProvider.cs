namespace ft_vox.Worlds
{
    public interface IBlocksProvider
    {
        IBlock GetBlockForId(byte id);

        void RegisterBlock(byte id, IBlock block);
    }
}
