using ft_vox.GameManaging;
using ft_vox.OpenGL;
using ft_vox.Worlds;
using System;
using System.Diagnostics;
using ft_vox.Gameplay;
using ft_vox.GameStates;

namespace ft_vox
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            /*try
            {*/
                var gameStateManager = new GameStateManager();
                var blocksProvider = new BlocksProvider();
                blocksProvider.RegisterBlock(1, new BlockSimple("Stone", true));
                blocksProvider.RegisterBlock(2, new BlockSimple("Grass block", true));
                blocksProvider.RegisterBlock(3, new BlockSimple("Dirt", true));
                blocksProvider.RegisterBlock(4, new BlockSimple("Cobblestone", true));
                blocksProvider.RegisterBlock(5, new BlockSimple("Wooden planks", true));
                blocksProvider.RegisterBlock(7, new BlockSimple("Bedrock", true));
                blocksProvider.RegisterBlock(12, new BlockSimple("Sand", true));
                blocksProvider.RegisterBlock(13, new BlockSimple("Gravel", true));
                blocksProvider.RegisterBlock(14, new BlockSimple("Gold Ore", true));
                blocksProvider.RegisterBlock(15, new BlockSimple("Iron Ore", true));
                blocksProvider.RegisterBlock(16, new BlockSimple("Coal Ore", true));
                blocksProvider.RegisterBlock(17, new BlockSimple("Oak Wood", true));
                blocksProvider.RegisterBlock(24, new BlockSimple("Sandstone", true));
                blocksProvider.RegisterBlock(31, new BlockSimple("Grass", false));
                blocksProvider.RegisterBlock(35, new BlockSimple("Wool", false));
                var blockSelector = new BlockSelector();
                blockSelector.AddSelectableBlock(1);
                blockSelector.AddSelectableBlock(2);
                blockSelector.AddSelectableBlock(3);
                blockSelector.AddSelectableBlock(4);
                blockSelector.AddSelectableBlock(5);
                blockSelector.AddSelectableBlock(7);
                blockSelector.AddSelectableBlock(12);
                blockSelector.AddSelectableBlock(13);
                blockSelector.AddSelectableBlock(14);
                blockSelector.AddSelectableBlock(15);
                blockSelector.AddSelectableBlock(16);
                blockSelector.AddSelectableBlock(17);
                blockSelector.AddSelectableBlock(24);
                blockSelector.AddSelectableBlock(31);
                blockSelector.AddSelectableBlock(35);
            
                var chunkManager = new ChunkManager();
                var chunkGenerator = new ChunkGeneratorSurface(chunkManager);
                var worldManager = new WorldManager(blocksProvider, chunkManager, chunkGenerator);
                var chunkPartManager = new ChunkPartManager(worldManager, chunkManager, blocksProvider);
            
                var world = new World("world", new Random().Next());
                var window = new MainWindow(gameStateManager, world);
                
                gameStateManager.SetGameState(new GameStatePlay(gameStateManager, worldManager, chunkManager, chunkPartManager, blockSelector, blocksProvider, world));
                window.Run(60);
                worldManager.Clean(world);
            /*}
            catch (Exception exception)
            {
                Console.Error.WriteLine($"Une exception de type {exception.GetType()} est survenue, message : {exception.Message}");
                Console.Error.WriteLine($"Stacktrace:");
                Console.Error.WriteLine(exception.StackTrace);
                Console.WriteLine("Sortie...");
                Environment.Exit(1);
            }*/
        }
    }
}
