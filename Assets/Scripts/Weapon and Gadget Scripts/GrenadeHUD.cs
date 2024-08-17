using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrenadeHUD : MonoBehaviour
{
    [SerializeField] private Text grenadeText;
    //[SerializeField] private Image grenadeSprite;
    [SerializeField] GameObject spriteHolders;
    public int currentGrenades;

    public void decrementGrenades() {
        currentGrenades --;
        grenadeText.text = currentGrenades.ToString();
    }   
    public void setCurrentGrenades(int cur) {
        currentGrenades = cur;
        grenadeText.text = currentGrenades.ToString();
    }
    public void setSprite(Sprite sprite) {
        GameObject GO = spriteHolders.transform.GetChild(0).gameObject;;
        for (int i = 0; i < spriteHolders.transform.childCount; i ++) {
            spriteHolders.transform.GetChild(i).gameObject.SetActive(false);
            if (spriteHolders.transform.GetChild(i).GetComponent<Image>().sprite == sprite) {
                GO = spriteHolders.transform.GetChild(i).gameObject;
            }
        }
        GO.SetActive(true);
        //grenadeSprite.sprite = sprite;
        //grenadeSprite.SetNativeSize();
        //gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(gameObject.GetComponent<RectTransform>().sizeDelta.x * 30, gameObject.GetComponent<RectTransform>().sizeDelta.y * 30);
    }
}
