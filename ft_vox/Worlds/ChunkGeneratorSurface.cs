using ft_vox.Helpers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ft_vox.Worlds
{
    class ChunkGeneratorSurface : IChunkGenerator
    {
        private readonly long _seed;
        private readonly PerlinNoiseOctaveHelper _perlin;
        private readonly PerlinNoiseOctaveHelper _cavePerlin;

        public ChunkGeneratorSurface(long seed)
        {
            _seed = seed;
            _perlin = new PerlinNoiseOctaveHelper(_seed, 8, 3);
            _cavePerlin = new PerlinNoiseOctaveHelper(_seed + 45678, 5, 3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float MAP(float value, float fromValue, float toValue, float fromResult, float toResult)
        {
            return (value - fromValue) / (toValue - fromValue) * (toResult - fromResult) + fromResult;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float MIN(float a, float b)
        {
            return a < b ? a : b;
        }

        public void PopulateChunk(Chunk chunk, ChunkPosition position)
        {
            if (!StaticReferences.ParallelMode)
            {
                for (byte x = 0; x < 16; x++)
                    for (byte z = 0; z < 16; z++)
                    {
                        PerlinGeneration(chunk, position, x, z);
                    }
            }
            else
            {
                Parallel.For(0, 256, (i) =>
                {
                    PerlinGeneration(chunk, position, (byte)(i % 16), (byte)(i / 16));
                });
            }
        }

        private void PerlinGeneration(Chunk chunk, ChunkPosition position, byte x, byte z)
        {
            var frequency = 0.00005f;
            var caveFrequency = 0.003f;
            var perlinResult = _perlin.Noise(((position.X * 16 + x)) * frequency, 0, ((position.Z * 16 + z)) * frequency);
            var perlinHeight = 60 + perlinResult * 100;
            if (perlinHeight < 0)
                perlinHeight = 0;
            else if (perlinHeight > 255)
                perlinHeight = 255;
            byte height = (byte)perlinHeight;
            for (byte y = 0; y < height; y++)
            {
                /*var cavePerlinResult = ((float)_cavePerlin.Noise(((position.X * 16 + x)) * caveFrequency, y * caveFrequency, ((position.Z * 16 + z)) * caveFrequency) + 0.5f) * ((y) / 64f);
                cavePerlinResult = MIN(MAP(cavePerlinResult, 0f, 0.1f, 0.1f, 0.2f), cavePerlinResult);
                if (cavePerlinResult >= 0.09f && cavePerlinResult <= 0.16f)
                    chunk.SetBlockId(x, y, z, 0);
                else*/
                if (y == height - 1)
                    chunk.SetBlockId(x, y, z, 31);
                else if (y == height - 2)
                    chunk.SetBlockId(x, y, z, 2);
                else if (y >= height - 5)
                    chunk.SetBlockId(x, y, z, 3);
                else
                    chunk.SetBlockId(x, y, z, 1);
            }
        }
    }
}
