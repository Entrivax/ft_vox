using OpenTK;
using System;

namespace ft_vox.Frustum
{
    public class Plane
    {
        public Vector3 N { get; private set; }
        public float D { get; private set; }

        public Vector4 Nums
        {
            set
            {
                value.Normalize();
                
                D = value.W;
                N = value.Xyz;
            }
        }
    }
}
