using ft_vox.Helpers;

namespace ft_vox.Worlds
{
    class ChunkGeneratorSurface : IChunkGenerator
    {
        private readonly long _seed;
        private readonly PerlinNoiseOctaveHelper _perlin;
        private readonly PerlinNoiseOctaveHelper _cavePerlin;
        //private readonly PerlinNoiseHelper _perlin;

        public ChunkGeneratorSurface(long seed)
        {
            _seed = seed;
            _perlin = new PerlinNoiseOctaveHelper(_seed, 8, 3);
            _cavePerlin = new PerlinNoiseOctaveHelper(_seed + 45678, 8, 3);
        }

        public void PopulateChunk(Chunk chunk, ChunkPosition position)
        {
            var frequency = 0.0005f;
            var caveFrequency = 0.001f;
            for (byte x = 0; x < 16; x++)
                for (byte z = 0; z < 16; z++)
                {
                    var perlinResult = _perlin.Noise(( (position.X * 16 + x)) * frequency, 0, ((position.Z * 16 + z)) * frequency);
                    var perlinHeight = 60 + perlinResult * 100;
                    if (perlinHeight < 0)
                        perlinHeight = 0;
                    else if (perlinHeight > 255)
                        perlinHeight = 255;
                    byte height = (byte)perlinHeight;
                    for (byte y = 0; y < height; y++)
                    {
                        /*var cavePerlinResult = _cavePerlin.Noise(((position.X * 16 + x)) * caveFrequency, y * caveFrequency, ((position.Z * 16 + z)) * caveFrequency);
                        if (cavePerlinResult <= -0.3f)
                            chunk.SetBlockId(x, y, z, 0);
                        else*/ if (y == height - 1)
                            chunk.SetBlockId(x, y, z, 3);
                        else if (y >= height - 4)
                            chunk.SetBlockId(x, y, z, 2);
                        else
                            chunk.SetBlockId(x, y, z, 1);
                    }
                }
        }
    }
}
