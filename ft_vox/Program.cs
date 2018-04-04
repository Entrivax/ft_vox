using ft_vox.GameManaging;
using ft_vox.OpenGL;
using ft_vox.Worlds;
using System;
using System.Diagnostics;

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
                blocksProvider.RegisterBlock(1, new BlockSimple(blocksProvider, 1));
                blocksProvider.RegisterBlock(3, new BlockSimple(blocksProvider, 2));
                blocksProvider.RegisterBlock(12, new BlockSimple(blocksProvider, 18));
                blocksProvider.RegisterBlock(2, new BlockSimpleMultiTextured(blocksProvider, 0, 2, 3));
                blocksProvider.RegisterBlock(24, new BlockSimpleMultiTextured(blocksProvider, 176, 208, 192));
                blocksProvider.RegisterBlock(31, new BlockTallGrass(39));
                var chunkGenerator = new ChunkGeneratorSurface(new Random().Next());
                var chunkProvider = new ChunkProvider(blocksProvider, chunkGenerator);
                var world = new World(chunkProvider);
                new MainWindow(gameStateManager, world).Run(60);
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
