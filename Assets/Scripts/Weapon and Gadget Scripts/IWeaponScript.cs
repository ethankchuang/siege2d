using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public interface IWeaponScript
{
    public void shoot(Transform firePoint, AudioSource audioSource);
    public void reload(AudioSource audioSource);
    public void aimDownSight(Light2D light2D);
    public void hipFire(Light2D light2D, float innerAngle, float outerAngle);
    public void onStart();
}
