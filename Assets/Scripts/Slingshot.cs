using UnityEngine;
using UnityEngine.UI;

public class Slingshot : MonoBehaviour
{
    static public Slingshot S;

    [Header("Set in Inspector")]
    public GameObject prefabProjectile;
    public float velocityMult = 8f;
    public Text uitButton;
    public LineRenderer line;

    [Header("Set Dinamycally")]
    public GameObject launchPoint;
    public GameObject projectile;
    public Vector3 launchPos;
    public bool aimingMode;
    private Rigidbody projectileRigidBody;

    static public Vector3 LAUNCH_POS
    {
        get
        {
            if (S == null)
                return Vector3.zero;

            return S.launchPos;
        }
    }

    private void Awake()
    {
        S = this;
        Transform launchPointTransform = transform.Find("LaunchPoint");
        launchPoint = launchPointTransform.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTransform.position;
    }

    void Update()
    {
        if (!aimingMode)
            return;

        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        Vector3 mouseDelta = mousePos3D - launchPos;
        float maxMagnide = this.GetComponent<SphereCollider>().radius;
        if (mouseDelta.magnitude > maxMagnide)
        {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnide;
        }

        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;

        line.SetPosition(1, projPos);

        if (Input.GetMouseButtonUp(0))
        {
            aimingMode = false;
            projectileRigidBody.isKinematic = false;
            projectileRigidBody.velocity = -mouseDelta * velocityMult;
            FollowCam.POI = projectile;
            projectile = null;
            MissionDemolition.ShotFired();
            ProjectileLine.S.poi = projectile;
            uitButton.text = "Show Castle";
            line.SetPosition(1, new Vector3(-10, -6, 0));
        }
    }
    void OnMouseEnter()
    {
        launchPoint.SetActive(true);
    }
    void OnMouseExit()
    {
        launchPoint.SetActive(false);
    }
    void OnMouseDown()
    {
        aimingMode = true;
        projectile = Instantiate(prefabProjectile);
        projectile.transform.position = launchPos;

        projectileRigidBody = projectile.GetComponent<Rigidbody>();
        projectileRigidBody.isKinematic = true;
    }
}
