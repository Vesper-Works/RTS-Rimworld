using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityToolbag;
public class AnimationComponent : BaseComponent
{
    private List<string> framePaths = new List<string>();
    private Sprite[] frameSprites;
    private float frameDuration;
    private new string name;
    public float FrameDuration { get => frameDuration; set => frameDuration = value; }
    public string Name { get => name; set => name = value; }

    public override void Startup()
    {
        if(gameObject.GetComponent<SpriteFrameAnimator>() == null)
        {
            gameObject.AddComponent<SpriteFrameAnimator>();
        }
        frameSprites = new Sprite[framePaths.Count];
        for (int i = 0; i < framePaths.Count; i++)
        {
            frameSprites[i] = ResourceHandler.LoadSprite(framePaths[i]);
        }
        SpriteFrameAnimation animation = ScriptableObject.CreateInstance<SpriteFrameAnimation>();
        animation.frames = frameSprites;
        animation.frameDuration = frameDuration;
        animation.loop = true;
        animation.name = name;

        gameObject.GetComponent<SpriteFrameAnimator>().AddAnimation(animation);
    }

    public override string ToDetailedString()
    {
        throw new System.NotImplementedException();
    }
}
