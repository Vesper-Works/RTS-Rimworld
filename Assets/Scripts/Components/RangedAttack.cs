using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : BaseAttack
{
    private string ammunitionCalibre;
    private int projectileSpeed;
    private int capacity;
    private ReloadType reloadType;
    private float sightInaccuracy;
    private float stability;
    private float barrelSpread;
    private string fireSoundEffectPath;
    private AudioClip fireSound;
    private FireType fireType;
    private int burstCount;
    private int rateOfFire;
    private bool firing = false;
    private AudioSource audioSource;
    private float spread { get => sightInaccuracy + stability + barrelSpread; }

    private int currentAmmo;
    private string currentAmmoTypeLoaded;

    public string AmmunitionCalibre { get => ammunitionCalibre; set => ammunitionCalibre = value; }
    public int ProjectileSpeed { get => projectileSpeed; set => projectileSpeed = value; }
    public int Capacity { get => capacity; set => capacity = value; }
    public ReloadType ReloadType { get => reloadType; set => reloadType = value; }
    public float SightInaccuracy { get => sightInaccuracy; set => sightInaccuracy = value; }
    public float Stability { get => stability; set => stability = value; }
    public float BarrelSpread { get => barrelSpread; set => barrelSpread = value; }
    public string FireSoundEffectPath { get => fireSoundEffectPath; set => fireSoundEffectPath = value; }
    public FireType FireType { get => fireType; set => fireType = value; }
    public int BurstCount { get => burstCount; set => burstCount = value; }
    public int RateOfFire { get => rateOfFire; set => rateOfFire = value; }
    public int RPM { get => rateOfFire; set => rateOfFire = value; }


    public override void Startup()
    {
        fireSound = ResourceHandler.LoadAudio(FireSoundEffectPath);
        //currentTypeAmmoLoaded = EntityDefinitions.Instance.EntitiesByTypes["ItemEntity"][ammunitionCalibre].GetComponent<ProjectileData>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = ConnectionController.Instance.GetComponent<UnityEngine.Audio.AudioMixerGroup>();
    }
    public override string ToDetailedString()
    {
        return string.Format(
 @" 
Gun stats:-

Calibre: {0} 
Projectile Speed: {1}
Capacity: {2}
Reload Type: {3}
Sight Inaccuracy: {4}
Stability: {5}
Barrel Spread: {6}
Effective Firing Range {7}"
 , ammunitionCalibre, projectileSpeed, capacity, reloadType, sightInaccuracy, stability, BarrelSpread, Range);
    }

    public override string ToBasicString()
    {
        return string.Format(
 @"Calibre: {0} 
Effective Firing Range {1}"
 , ammunitionCalibre, Range);
    }

    public bool IsFiring()
    {
        return firing;
    }
    public void CancelShooting()
    {
        firing = false;
        StopAllCoroutines();
    }
    public void Fire(Vector2 direction, int shooterID)
    {
        switch (fireType)
        {
            case FireType.Single:
                FireSingle(direction, shooterID);
                break;
            case FireType.Burst:
                FireBurst(direction, shooterID);
                break;
            case FireType.Auto:
                FireAuto(direction, shooterID);
                break;
            default:
                break;
        }
        //if(currentAmmo <= 0) { return; }  
    }
    private void FireSingle(Vector2 direction, int shooterID)
    {
        direction.Normalize();
        Vector2 directionWithSway = direction.Rotate(Random.Range(-spread, spread)).normalized;
        AudioSource.PlayClipAtPoint(fireSound, transform.position);
        //audioSource.pitch = 1 + Random.Range(-0.1f, 0.1f);
        //audioSource.PlayOneShot(fireSound);


        if (projectileSpeed > 75)
        {
            GameController.CreateHitscanRay(transform.position, directionWithSway * projectileSpeed, ammunitionCalibre, shooterID);
        }
        else
        {
            GameController.CreateProjectile(
            transform.position, directionWithSway * projectileSpeed, ammunitionCalibre, shooterID);
        }
    }
    private void FireBurst(Vector2 direction, int shooterID)
    {
        firing = true;
        StartCoroutine(FireMultipleProjectiles(direction, shooterID, burstCount));
    }
    private void FireAuto(Vector2 direction, int shooterID)
    {
        firing = true;
        StartCoroutine(FireMultipleProjectiles(direction, shooterID, capacity));
    }

    private IEnumerator FireMultipleProjectiles(Vector2 direction, int shooterID, int numOfProjectiles)
    {
        int projectileCount = numOfProjectiles;
        if (rateOfFire == 0) { Debug.LogError("No rate of fire for: " + name); }
        while (projectileCount > 0) //currentAmmo > 0 && 
        {
            FireSingle(direction, shooterID);
            currentAmmo--;
            projectileCount--;
            yield return new WaitForSeconds(1f / (rateOfFire / 60f));
        }
        firing = false;
    }
}
