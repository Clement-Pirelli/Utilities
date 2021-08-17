using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class MyExtensions
{
    public static Vector4 asVector4(this Color color)
    {
        return new Vector4(color.r, color.g, color.b, color.a);
    }

    public static Vector4[] asVector4s(this Color[] colors)
    {
        return colors.Select(c => c.asVector4()).ToArray();
    }

    public static Vector4 asPosition(this Vector3 vector3)
    {
        return new Vector4(vector3.x, vector3.y, vector3.z, 1.0f);
    }

    public static Vector4 asDirection(this Vector3 vector3)
    {
        return new Vector4(vector3.x, vector3.y, vector3.z, .0f);
    }
}

public static class Math 
{
    public struct Range<T>
    {
        public T start;
        public T end;
        public Range(T start, T end) { this.start = start; this.end = end; }
    }
    public static float Remap(float value, Range<float> original, Range<float> transformed) 
    {
        float t = Mathf.InverseLerp(original.start, original.end, value);
        return Mathf.Lerp(transformed.start, transformed.end, t);
    }
}

public static class CSUtilities
{
    public static RenderTexture CreateRenderTexture(int resolution, FilterMode filterMode, RenderTextureFormat format)
    {
        RenderTexture texture = new RenderTexture(resolution, resolution, 1, format)
        {
            enableRandomWrite = true,
            filterMode = filterMode,
            wrapMode = TextureWrapMode.Repeat,
            useMipMap = false,
            name = "out",
            dimension = UnityEngine.Rendering.TextureDimension.Tex2D,
            wrapModeU = TextureWrapMode.Repeat,
            wrapModeV = TextureWrapMode.Repeat,
            volumeDepth = 1,
            autoGenerateMips = false
        };

        texture.Create();

        return texture;
    }

    public static RenderTexture Create3DRenderTexture(int resolution, FilterMode filterMode, RenderTextureFormat format)
    {
        RenderTexture texture = new RenderTexture(resolution, resolution, 0, format)
        {
            enableRandomWrite = true,
            filterMode = filterMode,
            wrapMode = TextureWrapMode.Repeat,
            useMipMap = false,
            name = "out",
            dimension = UnityEngine.Rendering.TextureDimension.Tex3D,
            autoGenerateMips = false,
            volumeDepth = resolution,
        };

        texture.Create();

        return texture;
    }
}