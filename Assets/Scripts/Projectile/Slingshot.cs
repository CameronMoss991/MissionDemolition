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
    public bool aimingMode;

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
        if (!aimingMode) return;
        // Get the current mouse position in 2D screen coordinates
        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
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
            aimingMode = false;
            Rigidbody projRB = projectile.GetComponent<Rigidbody>();
            projRB.isKinematic = false;
            projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRB.velocity = -mouseDelta * velocityMult;

            FollowCam.SWITCH_VIEW(FollowCam.eView.slingshot);

            FollowCam.POI = projectile; //sets main camera poi to projectile
            Instantiate<GameObject>(projLinePrefab,projectile.transform);
            projectile = null;
            MissionDemolition.SHOT_FIRED();
        }
    }


}
