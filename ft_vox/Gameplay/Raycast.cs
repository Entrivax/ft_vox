using System;
using ft_vox.Worlds;
using OpenTK;

namespace ft_vox.Gameplay
{
    internal class Raycast
    {
        public class HitInfo
        {
            public int X;
            public int Y;
            public int Z;
            public FaceEnum Face;
            public Chunk Chunk;

            public enum FaceEnum
            {
                Front,
                Back,
                Left,
                Right,
                Top,
                Bottom,
            }
        }

        public static HitInfo Cast(World world, Vector3 origin, Vector3 direction, float maxDistance)
        {
            var x = Math.Floor(origin.X);
            var y = Math.Floor(origin.Y);
            var z = Math.Floor(origin.Z);
            var dx = direction.X;
            var dy = direction.Y;
            var dz = direction.Z;
            if (dx == 0 && dy == 0 && dz == 0)
                throw new ArgumentException("Direction cannot be zero");
            var stepX = dx > 0 ? 1 : dx < 0 ? -1 : 0;
            var stepY = dy > 0 ? 1 : dy < 0 ? -1 : 0;
            var stepZ = dz > 0 ? 1 : dz < 0 ? -1 : 0;
            var tMaxX = IntBound(origin.X, dx);
            var tMaxY = IntBound(origin.Y, dy);
            var tMaxZ = IntBound(origin.Z, dz);
            var tDeltaX = stepX / dx;
            var tDeltaY = stepY / dy;
            var tDeltaZ = stepZ / dz;
            maxDistance /= direction.Length;
            
            var hitInfo = new HitInfo();
            while (y >= 0 && y < 256)
            {
                if (tMaxX < tMaxY)
                {
                    if (tMaxX < tMaxZ)
                    {
                        if (tMaxX > maxDistance)
                            break;
                        x += stepX;
                        tMaxX += tDeltaX;
                        hitInfo.Face = stepX > 0 ? HitInfo.FaceEnum.Left : HitInfo.FaceEnum.Right;
                    }
                    else
                    {
                        if (tMaxZ > maxDistance)
                            break;
                        z += stepZ;
                        tMaxZ += tDeltaZ;
                        hitInfo.Face = stepZ > 0 ? HitInfo.FaceEnum.Back : HitInfo.FaceEnum.Front;
                    }
                }
                else
                {
                    if (tMaxY < tMaxZ)
                    {
                        if (tMaxY > maxDistance)
                            break;
                        y += stepY;
                        tMaxY += tDeltaY;
                        hitInfo.Face = stepY > 0 ? HitInfo.FaceEnum.Bottom : HitInfo.FaceEnum.Top;
                    }
                    else
                    {
                        if (tMaxZ > maxDistance)
                            break;
                        z += stepZ;
                        tMaxZ += tDeltaZ;
                        hitInfo.Face = stepZ > 0 ? HitInfo.FaceEnum.Back : HitInfo.FaceEnum.Front;
                    }
                }
                if (world.GetBlockIdAtForCurrentlyLoadedChunks((int)x, (int)y, (int)z) != 0)
                {
                    hitInfo.Chunk = world.GetChunkAtWorldCoordinates((int)x, (int)z);
                    hitInfo.X = (int)x;
                    hitInfo.Y = (int)y;
                    hitInfo.Z = (int)z;
                    return hitInfo;
                }
            }
            return null;
        }

        private static float IntBound(float s, float ds)
        {
            var sIsInteger = Math.Round(s) == s;
            if (ds < 0 && sIsInteger)
                return 0;

            return (float)(ds > 0 ? Math.Ceiling(s) - s : s - Math.Floor(s)) / Math.Abs(ds);
        }
    }
}