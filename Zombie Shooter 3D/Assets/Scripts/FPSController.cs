using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    public GameObject cam;
    public Animator anim;
    public AudioClip[] audioClips;
    public AudioSource audioSource;
    public AudioSource ammoPickup;
    public AudioSource healthPickup;
    public AudioSource triggerSound;
    public AudioSource death;
    public AudioSource reload;

    float speed = 0.1f;
    float Xsensitivity = 4f;
    float Ysensitivity = 4f;
    float MinimumX = -90;
    float MaximumX = 90;

    Rigidbody rb;

    CapsuleCollider capsule;

    Quaternion cameraRot;
    Quaternion characterRot;

    bool cursorIsLocked = true;
    bool lockCursor = true;
    bool playingWalking = false;
    bool previouslyGrounded = true; 

    //Inventory
    int ammo = 0;
    int maxAmmo = 40;
    int health = 10;
    int maxHealth = 100;
    int ammoClip = 0;
    int ammoClipMax = 10;


    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        capsule = this.GetComponent<CapsuleCollider>();
        audioSource = this.GetComponent<AudioSource>();

        cameraRot = cam.transform.localRotation;
        characterRot = this.transform.localRotation;

        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            anim.SetBool("arm", !anim.GetBool("arm"));
        }
        if (Input.GetMouseButtonDown(0) && !anim.GetBool("fire"))
        {
            if(ammoClip > 0){
                anim.SetTrigger("fire");
                ammoClip--;
                Debug.Log("Ammo left in Clip: " +ammoClip);
            }
            else if(anim.GetBool("arm")){
                triggerSound.Play();
            }
            
            //shot.Play();
        }
        if (Input.GetKeyDown(KeyCode.R) && anim.GetBool("arm"))
        {
            anim.SetTrigger("reload");
            reload.Play();
            int amountNeed = ammoClipMax - ammoClip;
            int ammoAvailable = amountNeed < ammo ? amountNeed : ammo;
            ammo -= ammoAvailable;
            ammoClip += ammoAvailable; 
            Debug.Log("Ammo Left: " + ammo);
            Debug.Log("Ammo in Clip: " + ammoClip);
        }
        
    }

   

    void FixedUpdate()
    {
        float yRot = Input.GetAxis("Mouse X") * Ysensitivity;
        float xRot = Input.GetAxis("Mouse Y") * Xsensitivity;

        cameraRot *= Quaternion.Euler(-xRot, 0, 0);
        characterRot *= Quaternion.Euler(0, yRot, 0);

        cameraRot = ClampRotationAroundXAxis(cameraRot);

        this.transform.localRotation = characterRot;
        cam.transform.localRotation = cameraRot;


        bool grounded = isGrounded();
        if (Input.GetKeyDown(KeyCode.Space) && grounded) 
        {
            rb.AddForce(0, 300f, 0);
            audioSource.PlayOneShot(audioClips[4]);
            if(anim.GetBool("walking"))
            {
                CancelInvoke("PlayFootStepAudio");
                playingWalking = false;
            }
        }
        else if( !previouslyGrounded && grounded)
        {
            audioSource.PlayOneShot(audioClips[5]);
        }

        previouslyGrounded = grounded;
        float x = Input.GetAxis("Horizontal") * speed;
        float z = Input.GetAxis("Vertical") * speed;

        //walking 
        if (Mathf.Abs(x) > 0 || Mathf.Abs(z) > 0)
        {
            if(!anim.GetBool("walking")){
                anim.SetBool("walking", true);
                InvokeRepeating("PlayFootStepAudio", 0, 0.4f);
            }        
        }
        else if(anim.GetBool("walking"))
        {
            anim.SetBool("walking", false);
            CancelInvoke("PlayFootStepAudio");
            playingWalking = false;
        }

        transform.position += cam.transform.forward * z + cam.transform.right * x;
            
        UpdateCursorLock();
    }

    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

    bool isGrounded()
    {
        RaycastHit hitInfo;
        if(Physics.SphereCast(transform.position, capsule.radius, Vector3.down, out hitInfo,
            (capsule.height / 2f) - capsule.radius + 0.01f))
        {
            return true;
        }
        return false;
    }

     void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.tag == "Ammo" && ammo < maxAmmo){
            ammo = Mathf.Clamp(ammo + 10, 0, maxAmmo );
            Debug.Log("Ammo" + ammo);
            Destroy(collision.gameObject);
            ammoPickup.Play();
        }
        if(collision.gameObject.tag == "MedKit" && health < maxHealth){
            health = Mathf.Clamp(health + 20, 0, maxHealth );
            Debug.Log("Medkit" + health);
            Destroy(collision.gameObject);
            healthPickup.Play();
        }
        else if(collision.gameObject.tag  == "lava")
        {
            health = Mathf.Clamp(health - 25, 0, maxHealth );;
            Debug.Log("Health level: " + health);
            if(health <= 0)
            {
                death.Play();
            }
        }
        if(isGrounded())
        {
            
            audioSource.PlayOneShot(audioClips[5]);
        }
        /*if(anim.GetBool("walking"))
        {
            InvokeRepeating()
        }*/
    }

    public void UpdateCursorLock()
    {
        if (lockCursor)
        {
            InternalLockUpdate();
        }
    }

    void PlayFootStepAudio()
    {
        if(anim.GetBool("walking") && !playingWalking)
        {
            AudioClip footsteps = audioClips[Random.Range(0, 3)];
            audioSource.PlayOneShot(footsteps);
            playingWalking = true;
        }
        
    }
     /*void PlayFootStepAudio()
    {
        AudioSource audiosource = new AudioSource();
        int n = Random.Range(1, footsteps.Length);

        audiosource = footsteps[n];
        audiosource.Play();
        footsteps[n] = footsteps[0];
        footsteps[0] = audiosource;
    }*/

    public void InternalLockUpdate()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            cursorIsLocked = false;
        }
        else if(Input.GetMouseButtonUp(0)){
            cursorIsLocked = true;
        }
        if (cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if(!cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
