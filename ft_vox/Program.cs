using ft_vox.GameManaging;
using ft_vox.OpenGL;
using ft_vox.Worlds;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using ft_vox.Gameplay;
using ft_vox.GameStates;
using ft_vox.Helpers;
using OpenTK.Graphics.OpenGL;

namespace ft_vox
{
    class Program
    {
        class CommandLineArguments
        {
            [Argument("seed", "s", "World seed")]
            public long Seed { get; set; }
        }
        
        [STAThread]
        static void Main(string[] args)
        {
            CommandLineArguments arguments = null;
            try
            {
                arguments = CommandLineHelper.Parse<CommandLineArguments>(args);
            }
            catch (Exception)
            {
                return;
            }

            /*try
            {*/
                var gameStateManager = new GameStateManager();
                var blockSimples = new[]
                {
                    new Tuple<byte, string, bool>(1, "Stone", true),
                    new Tuple<byte, string, bool>(2, "Grass block", true),
                    new Tuple<byte, string, bool>(3, "Dirt", true),
                    new Tuple<byte, string, bool>(4, "Cobblestone", true),
                    new Tuple<byte, string, bool>(5, "Wooden planks", true),
                    new Tuple<byte, string, bool>(7, "Bedrock", true),
                    new Tuple<byte, string, bool>(12, "Sand", true),
                    new Tuple<byte, string, bool>(13, "Gravel", true),
                    new Tuple<byte, string, bool>(14, "Gold Ore", true),
                    new Tuple<byte, string, bool>(15, "Iron Ore", true),
                    new Tuple<byte, string, bool>(16, "Coal Ore", true),
                    new Tuple<byte, string, bool>(17, "Oak Wood", true),
                    new Tuple<byte, string, bool>(24, "Sandstone", true),
                    new Tuple<byte, string, bool>(31, "Grass", false),
                    new Tuple<byte, string, bool>(35, "Wool", true),
                };
                var blocksProvider = new BlocksProvider();
                var blockSelector = new BlockSelector();
                for (int i = 0; i < blockSimples.Length; i++)
                {
                    blocksProvider.RegisterBlock(blockSimples[i].Item1, new BlockSimple(blockSimples[i].Item2, blockSimples[i].Item3));
                    blockSelector.AddSelectableBlock(blockSimples[i].Item1);
                }
            
                var chunkManager = new ChunkManager();
                var chunkGenerator = new ChunkGeneratorSurface(chunkManager);
                var worldManager = new WorldManager(blocksProvider, chunkManager, chunkGenerator);
                var chunkPartManager = new ChunkPartManager(worldManager, chunkManager, blocksProvider);
            
                var world = new World("world", (arguments?.Seed ?? 0) != 0 ? arguments.Seed : new Random().Next());
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
