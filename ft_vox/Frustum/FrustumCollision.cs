using System;
using OpenTK;
using OpenTK.Audio;
using OpenTK.Graphics;

namespace ft_vox.Frustum
{
    public static class FrustumCollision
    {
        public static bool IsInFrustum(Plane[] frustumPlanes, AABB aabb, Vector3 cameraPosition)
        {
            for (int i = 0; i < 6; i++)
            {
                var absPlane = Vector3.ComponentMax(frustumPlanes[i].N, -frustumPlanes[i].N);
                var d = Vector3.Dot((aabb.Max + aabb.Min) / 2, frustumPlanes[i].N);
                var r = Vector3.Dot((aabb.Max - aabb.Min) / 2, absPlane);
                if (d + r < -frustumPlanes[i].D)
                    return false;
            }
            return true;
        }
    }
}
