using ft_vox.Worlds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ft_vox.Helpers
{
    // https://github.com/Maxopoly/Caveworm/blob/master/src/main/java/com/github/maxopoly/caveworm/worms/SimplexNoiseWorm.java
    public class Worm
    {
        private Location _currentLocation;
        private PerlinNoiseOctaveHelper _xNoise;
        private PerlinNoiseOctaveHelper _yNoise;
        private PerlinNoiseOctaveHelper _zNoise;
        private readonly float _xFrequency;
        private readonly float _yFrequency;
        private readonly float _zFrequency;
        private readonly float _xThreshold;
        private readonly float _yThreshold;
        private readonly float _zThreshold;
        private readonly float _xAmplitude;
        private readonly float _yAmplitude;
        private readonly float _zAmplitude;
        private readonly int _maxLength;
        private readonly List<Location> _path;

        private bool _finished;
        private int _length;

        public Worm(int x, int y, int z,
                    int xOctaves, int yOctaves, int zOctaves,
                    float xPersistance, float yPersistance, float zPersistance,
                    float xFrequency, float yFrequency, float zFrequency,
                    float xThreshold, float yThreshold, float zThreshold,
                    float xAmplitude, float yAmplitude, float zAmplitude,
                    int xSeed, int ySeed, int zSeed,
                    int maxLength)
        {
            _currentLocation = new Location(x, y, z);
            _xNoise = new PerlinNoiseOctaveHelper(xSeed, xOctaves, xPersistance);
            _yNoise = new PerlinNoiseOctaveHelper(ySeed, yOctaves, yPersistance);
            _zNoise = new PerlinNoiseOctaveHelper(zSeed, zOctaves, zPersistance);
            _xFrequency = xFrequency;
            _yFrequency = yFrequency;
            _zFrequency = zFrequency;
            _xThreshold = xThreshold;
            _yThreshold = yThreshold;
            _zThreshold = zThreshold;
            _xAmplitude = xAmplitude;
            _yAmplitude = yAmplitude;
            _zAmplitude = zAmplitude;
            _maxLength = maxLength;
            _finished = false;
            _length = 0;
            _path = new List<Location>();
        }
        
        public Location? GetNext()
        {
            if (_finished)
                return null;

            var result = _currentLocation;

            var x = _xNoise.Noise(_currentLocation.X * _xFrequency, _currentLocation.Y * _xFrequency, _currentLocation.Z * _xFrequency) * _xAmplitude;

            if (x >= _xThreshold)
            {
                _currentLocation = new Location(_currentLocation.X + 1, _currentLocation.Y, _currentLocation.Z);
            }
            else if (x <= (-1 * _xThreshold))
            {
                _currentLocation = new Location(_currentLocation.X - 1, _currentLocation.Y, _currentLocation.Z);
            }

            var y = _yNoise.Noise(_currentLocation.X * _yFrequency, _currentLocation.Y * _yFrequency, _currentLocation.Z * _yFrequency) * _yAmplitude;

            if (y >= _yThreshold)
            {
                _currentLocation = new Location(_currentLocation.X, _currentLocation.Y + 1, _currentLocation.Z);
            }
            else if (y <= (-1 * _yThreshold))
            {
                _currentLocation = new Location(_currentLocation.X, _currentLocation.Y - 1, _currentLocation.Z);
            }

            var z = _zNoise.Noise(_currentLocation.X * _zFrequency, _currentLocation.Y * _zFrequency, _currentLocation.Z * _zFrequency) * _zAmplitude;

            if (z >= _zThreshold)
            {
                _currentLocation = new Location(_currentLocation.X, _currentLocation.Y, _currentLocation.Z + 1);
            }
            else if (z <= (-1 * _zThreshold))
            {
                _currentLocation = new Location(_currentLocation.X, _currentLocation.Y, _currentLocation.Z - 1);
            }

            if (_length++ >= _maxLength)
            {
                _finished = true;
            }

            if (_path.Any(l => l.X == _currentLocation.X && l.Y == _currentLocation.Y && l.Z == _currentLocation.Z))
            {
                _finished = true;
            }
            else
            {
                _path.Add(_currentLocation);
            }

            return result;
        }
    }
}
