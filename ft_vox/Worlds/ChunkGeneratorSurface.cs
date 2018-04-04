using System;
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
        private readonly PerlinNoiseOctaveHelper _mountainPerlin;
        private readonly PerlinNoiseOctaveHelper _temperaturePerlin;
        private readonly PerlinNoiseOctaveHelper _humidityPerlin;

        public ChunkGeneratorSurface(long seed)
        {
            _seed = seed;
            var rand = new Random((int)_seed);
            _perlin = new PerlinNoiseOctaveHelper((rand.Next() << 16) | rand.Next(), 8, 3);
            _mountainPerlin = new PerlinNoiseOctaveHelper((rand.Next() << 16) | rand.Next(), 8, 3);
            _cavePerlin = new PerlinNoiseOctaveHelper((rand.Next() << 16) | rand.Next(), 5, 3);
            _temperaturePerlin = new PerlinNoiseOctaveHelper((rand.Next() << 16) | rand.Next(), 5, 3);
            _humidityPerlin = new PerlinNoiseOctaveHelper((rand.Next() << 16) | rand.Next(), 5, 3);
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
            var rand = new Random((int)_seed);
            rand = new Random((position.X * 16 + x) * rand.Next() * (position.Z * 16 + z) * rand.Next());
            var frequency = 0.0005f;
            var moutainFrequency = 0.00005f;
            var caveFrequency = 0.003f;
            var temperatureFrequency = 0.0003f;
            var temperature = (byte)MAP((float)_temperaturePerlin.Noise((position.X * 16 + x) * temperatureFrequency, 0, (position.Z * 16 + z) * temperatureFrequency), -0.5f, 0.5f, 0, 255);
            chunk.SetTemperature(x, z, temperature);
            var humidityFrequency = 0.0003f;
            var humidity = (byte)MAP((float)_humidityPerlin.Noise((position.X * 16 + x) * humidityFrequency, 0, (position.Z * 16 + z) * humidityFrequency), -0.5f, 0.5f, 0, 255);
            chunk.SetHumidity(x, z, humidity);
            
            var perlinResult = _perlin.Noise(((position.X * 16 + x)) * frequency, 0, ((position.Z * 16 + z)) * frequency);
            var mountainPerlinResult = _mountainPerlin.Noise(((position.X * 16 + x)) * moutainFrequency, 0, ((position.Z * 16 + z)) * moutainFrequency);
            var perlinHeight = MAP((float)perlinResult, -0.5f, 0.5f, 60, 70) * MAP((float)((mountainPerlinResult + 0.5f) * (temperature / 255f)), 0, 1f, 0.5f, 4);
            if (perlinHeight < 0)
                perlinHeight = 0;
            else if (perlinHeight > 255)
                perlinHeight = 255;
            byte height = (byte)perlinHeight;
            for (byte y = 0; y < height; y++)
            {
                var cavePerlinResult = ((float)_cavePerlin.Noise(((position.X * 16 + x)) * caveFrequency, y * caveFrequency, ((position.Z * 16 + z)) * caveFrequency) + 0.5f) * ((y) / 64f);
                cavePerlinResult = MIN(MAP(cavePerlinResult, 0f, 0.1f, 0.1f, 0.2f), cavePerlinResult);
                if (cavePerlinResult > 0.04f)
                {
                    if (temperature > 160 || humidity > 64)
                    {
                        if (y == height - 1)
                        {
                            if (rand.Next(10) == 0)
                                chunk.SetBlockId(x, y, z, 31);
                        }
                        else if (y == height - 2)
                            chunk.SetBlockId(x, y, z, 2);
                        else if (y >= height - 5)
                            chunk.SetBlockId(x, y, z, 3);
                        else
                            chunk.SetBlockId(x, y, z, 1);
                    }
                    else
                    {
                        if (y == height - 1)
                        {
                            if (rand.Next(30) == 0)
                                chunk.SetBlockId(x, y, z, 31);
                        }
                        else if (y < height - 1 && y >= height - 4)
                            chunk.SetBlockId(x, y, z, 12);
                        else if (y >= height - 6)
                            chunk.SetBlockId(x, y, z, 24);
                        else
                            chunk.SetBlockId(x, y, z, 1);
                    }
                }
            }
        }
    }
}
