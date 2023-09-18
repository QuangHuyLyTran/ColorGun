using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Security.Cryptography;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] GameObject cameraHolder;
    [SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;
    [SerializeField] Image healthbarImage;
    [SerializeField] GameObject ui;
    [SerializeField] Item[] items;
    [SerializeField] GameObject settingPanel;
    [SerializeField] Button back, leave;
    SingleShotGun singleGun;
    [SerializeField] List<GameObject> ammo = new List<GameObject>();
    IncreateHealth increate;
    [SerializeField] Slider _mouseSensitivity;
    [SerializeField] Slider volume;
    int itemIndex;
    int previousItemIndex = -1;
    int ammoIndex;
    int previousAmmoIndex;
    bool focus;
    float verticalLookRotation;
    bool grounded;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;

    Rigidbody rb;
    bool enterHealth;
    PhotonView PV;
    RoomManager RM;
    public float increateHealth = 30f;
    const float maxHealth = 100f;
    public float currentHealth = maxHealth;
    private AudioSource sound;
    public GameObject stepSound;

    PlayerManager playerManager;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        RM = GetComponent<RoomManager>();
        sound = GetComponent<AudioSource>();

        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();


        leave.onClick.AddListener(() => {           
            PhotonNetwork.Disconnect();
            Destroy(PV);
            Destroy(RM);
            SceneManager.LoadSceneAsync("Lobby");
            //LeaveFromRoom();
        });
    }
    void Update()
    {
        sound.volume = volume.value;
        mouseSensitivity = _mouseSensitivity.value;
        if (!PV.IsMine) return;
        Look();
        Move();
        Jump();
        
        //for(int i = 0; i < items.Length; i++)
        //{
        //    if(Input.GetKeyDown((i + 1).ToString()))
        //    {
        //        EquipItem(i);                             
        //        break;
        //    }
        //}

        if(Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            if(itemIndex >= items.Length - 1)
            {
                EquipItem(0);
                ammo[0].SetActive(true);
                ammo[1].SetActive(false);
            }
            else
            {
                EquipItem(itemIndex + 1);
                ammo[1].SetActive(true);
                ammo[0].SetActive(false);
            }
        }
        else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            if(itemIndex <=0)
            {
                EquipItem(items.Length - 1);
                ammo[1].SetActive(true);
                ammo[0].SetActive(false);

            }
            else
            {
                EquipItem(itemIndex - 1);
                ammo[1].SetActive(false);
                ammo[0].SetActive(true);

            }    
        } 
        
        if(Input.GetMouseButtonDown(0))
        {
            items[itemIndex].Use();          
        }

        if(transform.position.y < -10f)
        {
            Die();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            settingPanel.SetActive(true);
            focus = false;
        }
        back.onClick.AddListener(() => {
            settingPanel.SetActive(false);
            focus = true;
        });
        

        if (focus == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        } 
       if(enterHealth == true)
        {
           
        }
    }

    
    void Start()
    {
        stepSound.SetActive(false);
        focus = true;
        if (PV.IsMine)
        {
            EquipItem(0);
            ammo[0].SetActive(true);
            ammo[1].SetActive(false);
            
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
            Destroy(ui);
        }       
    }
    void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }   

    void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;       
        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
        if(Input.GetKeyDown(KeyCode.W) && grounded)
        {
            stepSound.SetActive(true);

        }else if(Input.GetKeyDown(KeyCode.S) && grounded)
        {
            stepSound.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.A) && grounded)
        {
            stepSound.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.D) && grounded)
        {
            stepSound.SetActive(true);
        }

        if(Input.GetKeyUp(KeyCode.W))
        {
            stepSound.SetActive(false);
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            stepSound.SetActive(false);

        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            stepSound.SetActive(false);

        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            stepSound.SetActive(false);

        }



    }    
     void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(transform.up * jumpForce);
            stepSound.SetActive(true);

        }
        else if( Input.GetKeyUp(KeyCode.Space))
        {
            stepSound.SetActive(false);
        }
    }
    void EquipItem(int _index)
    {
        if (_index == previousItemIndex)
            return;    
        itemIndex = _index;
        items[itemIndex].itemGameObject.SetActive(true);

        if (previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameObject.SetActive(false);
        }

        previousItemIndex = itemIndex;

        if(PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
        
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if(changedProps.ContainsKey("itemIndex") && !PV.IsMine && targetPlayer == PV.Owner)
        {
            EquipItem((int)changedProps["itemIndex"]);
        }
    }
    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }

    public void SetEntere(bool enter)
    {
        enterHealth = enter;
    }

    void FixedUpdate()
    {
        if (!PV.IsMine) 
            return;
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    public void TakeDamage(float damage)
    {
        PV.RPC(nameof(RPC_TakeDamage), PV.Owner, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage, PhotonMessageInfo info)
    {
        if (!PV.IsMine)
            return;
        EnableSound("takeDamage");
        currentHealth -= damage;
        healthbarImage.fillAmount = currentHealth / maxHealth;
        if (currentHealth <= 0)
        {
            Die();
            PlayerManager.Find(info.Sender).GetKill();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Health"))
        {           
            currentHealth = maxHealth;
            healthbarImage.fillAmount = currentHealth / maxHealth;
        }
    }

    //private void OnTriggerExit(Collider other)
    //{     
    //        currentHealth = this.currentHealth;
    //        healthbarImage.fillAmount = currentHealth / maxHealth;       
    //}
    void Die()
    {
        EnableSound("death");
        playerManager.Die();
    }

    public void LeaveFromRoom()
    {
        StartCoroutine(LeaveAndLoad());
    }

    IEnumerator LeaveAndLoad()
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
        {
            yield return null;
        }
        PhotonNetwork.LoadLevel(1);
    }
    public void EnableSound(string soundSource)
    {
        sound.PlayOneShot(Resources.Load<AudioClip>("Audio/" + soundSource));
    }
} 
