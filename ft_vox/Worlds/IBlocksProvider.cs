namespace ft_vox.Worlds
{
    interface IBlocksProvider
    {
        IBlock GetBlockForId(byte id);

        void RegisterBlock(byte id, IBlock block);
    }
}
