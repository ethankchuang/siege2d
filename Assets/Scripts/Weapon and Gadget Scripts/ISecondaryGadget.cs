using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISecondaryGadget
{
    public void explode();
    public string getName();
    public void throwGadget(float force, GameObject player);
    public Sprite GetSprite();
}
