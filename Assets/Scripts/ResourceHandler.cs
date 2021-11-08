using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class ResourceHandler : MonoBehaviour
{

    private Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
    private Dictionary<string, AudioClip> sounds = new Dictionary<string, AudioClip>();
    private static ResourceHandler instance { get; set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }
    private void Start()
    {
        StartCoroutine("LoadAllAudio");
    }
    public static Texture2D LoadTexture(string path)
    {
        if (instance.textures.ContainsKey(path)) { return instance.textures[path]; }

        Texture2D texture = null;

        byte[] imageBytes;
        string imagePath = Application.dataPath + "/Assets/Art/" + path;
        if (File.Exists(imagePath))
        {
            texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            imageBytes = File.ReadAllBytes(imagePath);
            texture.LoadImage(imageBytes);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;

            instance.textures.Add(path, texture);
        }
        else
        {
            Debug.LogError("Cannot find file: " + imagePath);
            return LoadTexture("MissingTexture.png");
        }


        return texture;
    }

    public static Sprite LoadSprite(string path)
    {
        Texture2D texture = LoadTexture(path);
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    public static AudioClip LoadAudio(string path)
    {
        if (instance.sounds.ContainsKey(path)) { return instance.sounds[path]; }

        return null;
        //using (var www = new WWW(audioPath))
        //{
        //    instance.sounds.Add(path, www.GetAudioClip(false, false, AudioType.WAV));
        //    return instance.sounds[path];
        //}
    }

    IEnumerator LoadAllAudio()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath + "/Assets/SoundEffects");

        foreach (var item in directoryInfo.GetFiles("*.*", SearchOption.AllDirectories))
        {
            if (item.Extension == ".meta") { continue; }
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(item.FullName, AudioType.WAV))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    instance.sounds.Add(item.FullName.Replace(directoryInfo.Parent.FullName, "").Replace("\\", "/").Remove(0, 1).Replace(item.Extension, ""), DownloadHandlerAudioClip.GetContent(www));
                }
            }
        }
    }
}
