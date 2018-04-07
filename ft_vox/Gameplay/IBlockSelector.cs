using ft_vox.Worlds;

namespace ft_vox.Gameplay
{
    interface IBlockSelector
    {
        void AddSelectableBlock(int id);

        int GetNextBlock(int id);
        int GetPreviousBlock(int id);
    }
}