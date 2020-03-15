using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

#pragma warning disable CS0618

public static class IOUtility
{
    public static T LoadFromJSON<T>(string path, out bool set)
    {
        UnityWebRequest request = UnityWebRequest.Get(path);
        request.uri = new System.Uri(request.uri.AbsoluteUri.Replace("http://localhost", "file://"));
        request.url = request.url.Replace("http://localhost", "file://");
        UnityWebRequestAsyncOperation asyncOp = request.SendWebRequest();
        while (!asyncOp.isDone)
        {
        }
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log($"COULD NOT LOAD FROM JSON! ERROR : {request.error}, PATH : {path}");
            set = false;
            return default;
        }

        set = true;
        string json = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data, 0, request.downloadHandler.data.Length);
        return JsonUtility.FromJson<T>(json);
    }

    public static void WriteToJSON<T>(string foldername, string filename, T toWrite)
    {
        if (!Directory.Exists(foldername))
        {
            Debug.Log("DIRECTORY DID NOT EXIST! CREATING DIRECTORY");
            Directory.CreateDirectory(foldername);
        }

        string path = Path.Combine(foldername, filename);

        if (!File.Exists(path))
        {
            Debug.Log("FILE DID NOT EXIST! CREATING FILE");
            File.Create(path).Close();
            File.WriteAllText(path, "placeholder");
        }
        string json = JsonUtility.ToJson(toWrite);
        File.WriteAllText(path, json);
    }

    public static byte[] LoadBytes(string path, out bool loaded)
    {
        UnityWebRequest request = UnityWebRequest.Get(path);
        request.uri = new System.Uri(request.uri.AbsoluteUri.Replace("http://localhost", "file://"));
        request.url = request.url.Replace("http://localhost", "file://");
        UnityWebRequestAsyncOperation asyncOp = request.SendWebRequest();
        while (!asyncOp.isDone)
        {
        }
        if (request.isNetworkError || request.isHttpError)
        {
            loaded = false;
            return null;
        }

        loaded = true;
        return request.downloadHandler.data;
    }

    public static void WriteArrayToFile<T>(string path, T[] data)
    {
        byte[] result = new byte[data.Length * System.Runtime.InteropServices.Marshal.SizeOf<T>(default(T))];
        System.Buffer.BlockCopy(data, 0, result, 0, result.Length);

        File.WriteAllBytes(path, result);
    }

    public static T[] ReadArrayFromFile<T>(string path, out bool read)
    {        
        byte[] readBytes = LoadBytes(path, out read);
        if (!read) return null;
        T[] result = new T[readBytes.Length / System.Runtime.InteropServices.Marshal.SizeOf<T>(default(T))];
        System.Buffer.BlockCopy(readBytes, 0, result, 0, result.Length);
        return result;
    }
}
