using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace BotData
{

    public struct SBVector3
    {
        public sbyte x;
        public sbyte y;
        public sbyte z;
        public SBVector3(int x, int y, int z)
        {
            this.x = (sbyte)x;
            this.y = (sbyte)y;
            this.z = (sbyte)z;
        }
        public override int GetHashCode()
        {
            int temp = ((int)x << 16) | ((int)y << 8) | ((int)z);
            return temp.GetHashCode();
        }
    }

    public delegate void BlockUpdate(BlockData data);
    public sealed class MeshData
    {
        public int offset = 0;
        public Vector3[] vertices;
        public Vector3[] normals;
        public Vector4[] tangents;
        public Color32[] colors;
        public Vector2[] uv1;
        public Vector2[] uv2;
        public int triIndex = 0;
        public int[] triangles;
        public MeshData(int numVertices = 65536, int numTrianglePoints = 62208)
        {
            vertices = new Vector3[numVertices];
            normals = new Vector3[numVertices];
            tangents = new Vector4[numVertices];
            colors = new Color32[numVertices];
            uv1 = new Vector2[numVertices];
            uv2 = new Vector2[numVertices];
            triangles = new int[numTrianglePoints];
        }
    }
    public sealed class BlockData
    {
        public const int ZPOSMASK = 0xFF;
        public const int YPOSMASK = 0xFF00;
        public const int XPOSMASK = 0xFF0000;
        //   [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetX(int pos)
        {
            return (sbyte)((pos & XPOSMASK) >> 16);
        }
        //   [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetY(int pos)
        {
            return (sbyte)((pos & YPOSMASK) >> 8);
        }
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetZ(int pos)
        {
            return (sbyte)(pos & ZPOSMASK);
        }
        //  [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AssemblePos(int x, int y, int z)
        {

            int toReturn = ((byte)x) << 16 | ((byte)y) << 8 | ((byte)z << 0);
            return toReturn;
        }
        /// [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AssemblePos(SBVector3 vec)
        {
            return AssemblePos(vec.x, vec.y, vec.z);
        }
        //   [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte AssembleRotation(Quaternion q)
        {
            return (byte)(((int)((q.eulerAngles.x + .5f) / 90) << 4) | ((int)((q.eulerAngles.y + .5f) / 90) << 2) | ((int)(q.eulerAngles.z + 0.5f) / 90));
        }
        // Represents the location of this block within the bot as signed XYZ.

    }

}