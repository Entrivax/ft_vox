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
                blocksProvider.RegisterBlock(3, new BlockSimple("Dirt", true));
                blocksProvider.RegisterBlock(12, new BlockSimple("Sand", true));
                blocksProvider.RegisterBlock(2, new BlockSimple("Grass block", true));
                blocksProvider.RegisterBlock(24, new BlockSimple("Sandstone", true));
                blocksProvider.RegisterBlock(31, new BlockSimple("Grass", false));
                var blockSelector = new BlockSelector();
                blockSelector.AddSelectableBlock(1);
                blockSelector.AddSelectableBlock(3);
                blockSelector.AddSelectableBlock(12);
                blockSelector.AddSelectableBlock(2);
                blockSelector.AddSelectableBlock(24);
                blockSelector.AddSelectableBlock(31);
                var chunkGenerator = new ChunkGeneratorSurface(new Random().Next());
                var chunkProvider = new ChunkProvider(blocksProvider, chunkGenerator);
                var world = new World(chunkProvider);
                var window = new MainWindow(gameStateManager, world);
                
                gameStateManager.SetGameState(new GameStatePlay(gameStateManager, blockSelector, blocksProvider, world));
                window.Run(60);
                chunkProvider.Clean();
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
