﻿using ft_vox.OpenGL;
using System.Collections.Concurrent;

namespace ft_vox
{
    static class StaticReferences
    {
        public static bool ParallelMode = false;

        public static ConcurrentBag<Mesh> MeshesToClean = new ConcurrentBag<Mesh>();

        public static ConcurrentBag<Blocks3D> BlocksToClean = new ConcurrentBag<Blocks3D>();
    }
}
