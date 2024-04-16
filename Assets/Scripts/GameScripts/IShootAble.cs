using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Photon.Pun;

//[PunRPC]
public interface IShootAble
{
    public void RecieveHit(RaycastHit2D hit);

    public void TakeDamage();
}
