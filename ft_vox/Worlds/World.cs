using ft_vox.Frustum;
using OpenTK;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using ft_vox.Gameplay;

namespace ft_vox.Worlds
{
    public class World
    {
        public ConcurrentBag<Chunk> ChunksToUnload { get; }
        public ConcurrentDictionary<ChunkPosition, Chunk[]> ChunkBlocks { get; }
        
        public string Name { get; }
        public long Seed { get; }

        public World(string name, long seed)
        {
            Name = name;
            Seed = seed;
            ChunkBlocks = new ConcurrentDictionary<ChunkPosition, Chunk[]>();
            ChunksToUnload = new ConcurrentBag<Chunk>();
        }
    }
}
