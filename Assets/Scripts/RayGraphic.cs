using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayGraphic : MonoBehaviour
{
    LineRenderer lineRenderer;
    public void CreateGraphic(string texturePath, Vector2 origin, Vector2 ending)
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.material.mainTexture = ResourceHandler.LoadTexture(texturePath);
        lineRenderer.material.mainTexture.wrapMode = TextureWrapMode.Repeat;
        lineRenderer.textureMode = LineTextureMode.Tile;
        lineRenderer.widthMultiplier = 1f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, ending);
        StartCoroutine(Animation());
    }

    private IEnumerator Animation()
    {
        float time = 0;
        while (true)
        {
            Gradient gradient = new Gradient();
            GradientColorKey gradientColorKey1 = new GradientColorKey(Color.grey, 0f);
            GradientColorKey gradientColorKey2 = new GradientColorKey(new Color(1 - time, 1 - time, 1 - time, 1 - (time * 1.5f)), 1f);
            GradientColorKey[] colourKeys = { gradientColorKey1, gradientColorKey2 };

            GradientAlphaKey gradientAlphaKey1 = new GradientAlphaKey(0, 0f);
            GradientAlphaKey gradientAlphaKey2 = new GradientAlphaKey(1 - (time * 1.5f), 1f);
            GradientAlphaKey[] alphaKeys = { gradientAlphaKey1, gradientAlphaKey2 };

            gradient.SetKeys(colourKeys, alphaKeys);
            lineRenderer.colorGradient = gradient;


            Keyframe keyframe1 = new Keyframe(0f, 1 + time);
            Keyframe keyframe2 = new Keyframe(1f, 1);
            Keyframe[] keyframes = { keyframe1, keyframe2 };

            AnimationCurve animationCurve = new AnimationCurve(keyframes);
            lineRenderer.widthCurve = animationCurve;

            time += 0.05f;
            if (time >= 1)
            {
                Destroy(gameObject);
            }
            yield return new WaitForSeconds(0.05f);
        }
    }
}
