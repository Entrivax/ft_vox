using System;
using ft_vox.OpenGL;
using OpenTK;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ft_vox.Worlds
{
    public class Chunk
    {
        public byte[] Blocks { get; }
        public byte[] BlockMetadatas { get; }
        public byte[] Temperatures { get; }
        public byte[] Humidity { get; }
        public byte[] LightMap { get; }
        public ChunkPart[] ChunkParts { get; }

        public Chunk()
        {
            Blocks = new byte[16 * 16 * 256];
            BlockMetadatas = new byte[16 * 16 * 256];
            Temperatures = new byte[16 * 16];
            Humidity = new byte[16 * 16];
            var chunkPartsNumber = 256 / 16;
            ChunkParts = new ChunkPart[chunkPartsNumber];
            for (int i = 0; i < chunkPartsNumber; i++)
            {
                ChunkParts[i] = new ChunkPart();
            }
        }

        public struct ChunkBlockInformation
        {
            public byte Humidity { get; set; }
            public byte Temperature { get; set; }
            public byte Id { get; set; }
            public byte Metadata { get; set; }
        }

        public class ChunkPart
        {
            public bool Invalidated { get; set; }

            public Blocks3D Blocks { get; set; }
        }
    }
}
