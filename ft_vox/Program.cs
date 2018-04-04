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
                blocksProvider.RegisterBlock(1, new BlockSimple(true));
                blocksProvider.RegisterBlock(3, new BlockSimple(true));
                blocksProvider.RegisterBlock(12, new BlockSimple(true));
                blocksProvider.RegisterBlock(2, new BlockSimple(true));
                blocksProvider.RegisterBlock(24, new BlockSimple(true));
                blocksProvider.RegisterBlock(31, new BlockSimple(false));
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
