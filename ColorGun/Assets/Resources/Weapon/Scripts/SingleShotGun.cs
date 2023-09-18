using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class SingleShotGun : Gun
{
    [SerializeField] Camera cam;
    PhotonView PV;
    public int mag;
    public int ammo;
    public int magAmmo;
    [SerializeField] Text magAmmoTxt, ammoTxt;
    [SerializeField] Animation reloadAni;
    [SerializeField] AnimationClip reload;
    AudioSource sound;
    [SerializeField] Slider volume;
    
    //[Range(0, 1)]
    //public float recoilPercent = 0.3f;
    [Range(0, 2)]
    public float recoverPercent = 0.7f;

    [Space]
    public float recoilUp = 0.05f;
    public float recoilBack = 0f;

    private Vector3 originalPosition;
    private Vector3 recoilVelocity;

    private bool recoiling;
    public bool recovering;

    private float recoilLength;
    private float recoverLegth;
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        sound = GetComponent<AudioSource>();
        originalPosition = transform.localPosition;

        recoilLength = 0;
        recoverLegth = 1 / recoverPercent;
    }
    public override void Use()
    {
        Shoot();
        
    }
    public void Update()
    {
        sound.volume = volume.value;
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();          
        }
        magAmmoTxt.text = (mag * magAmmo).ToString();
        ammoTxt.text = ammo.ToString();
        if (recoiling)
        {
            Recoil();
        } 
        if (recovering)
        {
            Recovering();
        }
    }

    void Shoot()
    {
        if (ammo != 0 && reloadAni.isPlaying == false)
        {
            EnableSound("gun-shot");
            recoiling = true;
            recovering = false;
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            ray.origin = cam.transform.position;
            ammo--;
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((gunInfo)itemInfo).damage);
                PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
            }
        }               
        
    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        if(colliders.Length != 0)
        {
            EnableSound("gun-shot");
            GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation); ;
            Destroy(bulletImpactObj, 10f);
            bulletImpactObj.transform.SetParent(colliders[0].transform);

        }
    }
    public void Reload()
    {
        EnableSound("reload-sound");
        if(mag > 0)
        {
            reloadAni.Play(reload.name);
            mag--;
            ammo = magAmmo;
        }
    }  
    
    void Recoil()
    {
        Vector3 finalPosition = new Vector3(originalPosition.x, originalPosition.y + 0.1f, originalPosition.z - recoilBack);

        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, finalPosition, ref recoilVelocity, recoilLength);


        if(transform.localPosition == finalPosition)
        {
            recoiling = false;
            recovering = true;
        }
    }

    void Recovering()
    {
        Vector3 finalPosition = originalPosition;

        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, finalPosition, ref recoilVelocity, recoverLegth);


        if (transform.localPosition == finalPosition)
        {
            recoiling = false;
            recovering = false;
        }
    }
    public void EnableSound(string soundSource)
    {
        sound.PlayOneShot(Resources.Load<AudioClip>("Audio/" + soundSource));
    }
}
