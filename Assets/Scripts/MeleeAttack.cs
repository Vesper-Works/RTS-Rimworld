using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : BaseAttack
{
    private string ammunitionCalibre;
    private int projectileSpeed;
    private int capacity;
    private ReloadType reloadType;
    private float sightsEffectiveness;
    private float swayFactor;
    private int effectiveFiringRange;
    private string fireSoundEffectPath;
    private AudioClip swingSound;

    private int currentAmmo;
    private string currentAmmoTypeLoaded;

    public string AmmunitionCalibre { get => ammunitionCalibre; set => ammunitionCalibre = value; }
    public int ProjectileSpeed { get => projectileSpeed; set => projectileSpeed = value; }
    public int Capacity { get => capacity; set => capacity = value; }
    public ReloadType ReloadType { get => reloadType; set => reloadType = value; }
    public float SightsEffectiveness { get => sightsEffectiveness; set => sightsEffectiveness = value; }
    public float SwayFactor { get => swayFactor; set => swayFactor = value; }
    public int EffectiveFiringRange { get => effectiveFiringRange; set => effectiveFiringRange = value; }
    public string HitSoundEffectPath { get => fireSoundEffectPath; set => fireSoundEffectPath = value; }

    public override void Startup()
    {
        swingSound = ResourceHandler.LoadAudio(HitSoundEffectPath);
    }
    public override string ToDetailedString()
    {
        return string.Format(
 @" 
Melee stats:-
");
    }

    public override string ToBasicString()
    {
        return string.Format(
 @"
");
    }

    public void Hit(Entity target)
    {
        //if (currentAmmo <= 0) { return; }
        //AudioSource.PlayClipAtPoint(swingSound, transform.position);
        //Vector2 directionWithSway = direction.Rotate(Random.Range(-swayFactor, swayFactor)).normalized;

        //GameController.CreateProjectile(
        //    transform.position, directionWithSway * projectileSpeed,
        //    currentTypeAmmoLoaded.Damage, currentTypeAmmoLoaded.Penetration, currentTypeAmmoLoaded.Mass, currentTypeAmmoLoaded.TexturePath);



    }
}

