using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    // INSCRIBED
    [Header("Inscribed")]
    public GameObject projectilePrefab;
    public float velocityMult = 10f;
    public GameObject projLinePrefab;

    // DYNAMIC
    [Header("Dynamic")]
    public GameObject launchPoint; 
    public Vector3 launchPos;
    public GameObject projectile;
    public AudioClip snapClip;
    public bool aimingMode;

    [Header("Rubber Bands")]
    public LineRenderer lineL;
    public LineRenderer lineR;
    public Transform armL, armR;

    [Header("Boost Settings")]
    public float boostMult = 2f; // Double the power
    private float aimTimer = 0f;
    private bool isBoosted = false;

    void Awake(){
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;
    }
    
    void OnMouseEnter(){ launchPoint.SetActive(true);}
    void OnMouseExit(){  launchPoint.SetActive(false);}

    void OnMouseDown()
    {
        aimingMode = true;
        projectile = Instantiate(projectilePrefab) as GameObject;
        projectile.transform.position = launchPos;
        projectile.GetComponent<Rigidbody>().isKinematic = true;
    }
    void Update()
    {
        if (!aimingMode) {
        // Hides lines or snap them back to the arms when not aiming
        lineL.enabled = lineR.enabled = false;
        aimTimer = 0f; // Reset timer when not aiming
        isBoosted = false;
        return;
        }
        aimTimer += Time.deltaTime;

        if (aimTimer >= 3f && !isBoosted) {
            isBoosted = true;
            // Visual feedback: Make the projectile glow or change color
            projectile.GetComponent<Renderer>().material.color = Color.red;
        }

        lineL.enabled = lineR.enabled = true;
        // Set start at the arms, end at the projectile
        lineL.SetPosition(0, armL.position);
        lineL.SetPosition(1, projectile.transform.position);
        
        lineR.SetPosition(0, armR.position);
        lineR.SetPosition(1, projectile.transform.position);

        //________________________________________________________________

        // Get the current mouse position in 2D screen coordinates
        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z + launchPos.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);
        
        // Find the delta from the launchPos to the mousePos3D
        Vector3 mouseDelta = mousePos3D - launchPos;
        
        // Limit mousedelta to the radius of the Slingshot SphereCollider
        float maxMagnitude = this. GetComponent<SphereCollider>().radius;

        if (mouseDelta.magnitude > maxMagnitude) {
        mouseDelta.Normalize(); mouseDelta *= maxMagnitude;
        }
        // Move the projectile to this new position
        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;

        if (Input. GetMouseButtonUp(0)) {// This 0 is a zero, not an o
            // The mouse has been released
            AudioSource.PlayClipAtPoint(snapClip, transform.position);
            aimingMode = false;

            
            Rigidbody projRB = projectile.GetComponent<Rigidbody>();
            projRB.isKinematic = false;
            projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;


            // 3. Apply the boost if the timer hit 3 seconds
            float finalVelocity = velocityMult;
            if (isBoosted) {
                finalVelocity *= boostMult;
            }
            projRB.velocity = -mouseDelta * velocityMult;

            FollowCam.SWITCH_VIEW(FollowCam.eView.slingshot);

            FollowCam.POI = projectile; //sets main camera poi to projectile
            Instantiate<GameObject>(projLinePrefab,projectile.transform);
            projectile = null;
            MissionDemolition.SHOT_FIRED();
        }
    }


}
