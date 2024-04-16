using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Shooting : MonoBehaviour
{
    [SerializeField] public Transform firePoint;
    //[SerializeField] public GameObject bulletPrefab;
    [SerializeField] public GameObject bulletTrail;
    [SerializeField] private float weaponRange = 100f;
    [SerializeField] private Animator muzzleFlashAnimator;

    PhotonView view;

    public float bulletForce = 20f;
    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (view.IsMine)
        {
            //GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            //Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            //rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);

            //muzzleFlashAnimator.SetTrigger("Shoot");

            var hit = Physics2D.Raycast(
                firePoint.position,
                transform.up,
                weaponRange
            );

            var trail = Instantiate(
                bulletTrail,
                firePoint.position,
                transform.rotation
            );

            var trailScript = trail.GetComponent<BulletTrailScript>();

            if (hit.collider != null)
            {
                trailScript.SetTargetPosition(hit.point);
                var hittable = hit.collider.GetComponent<IShootAble>();
                if (hittable != null)
                {
                    hittable.RecieveHit(hit);
                }
            }
            else
            {
                var endPosition = firePoint.position + transform.up * weaponRange;
                trailScript.SetTargetPosition(endPosition);
            }
        }

    }
}
