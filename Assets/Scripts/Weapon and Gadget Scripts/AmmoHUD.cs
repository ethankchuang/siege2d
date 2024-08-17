using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoHUD : MonoBehaviour
{
    [SerializeField] private Text ammoText;
    [SerializeField] private Image gunSprite;
    private int maxAmmo;
    private int currentAmmo;

    public void decrementAmmo() {
        currentAmmo --;
        ammoText.text = currentAmmo + "/" + maxAmmo;
    }   
    public void setAmmoMax(int max) {
        maxAmmo = max;
        ammoText.text = maxAmmo + "/" + maxAmmo;
        setCurrentAmmo(max);
    }
    public void setCurrentAmmo(int cur) {
        currentAmmo = cur;
        ammoText.text = currentAmmo + "/" + maxAmmo;
    }
    public void setSprite(Sprite sprite) {
        gunSprite.sprite = sprite;
    }
}
