using UnityEngine;
using UnityEngine.InputSystem;

public class GravityInput : MonoBehaviour
{
    public float grabDistance = 30f;
    public float throwForce = 50f;
    public Transform holdPoint;
    public float moveSpeed = 10f;

    private Camera cam;
    private Rigidbody heldObject;
    private GravityGunInput controls;

    private void Awake()
    {
        cam = Camera.main;
        controls = new GravityGunInput();
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Player.Grab.performed += _ => OnGrab();
        controls.Player.Throw.performed += _ => OnThrow();
    }

    private void OnDisable()
    {
        controls.Player.Grab.performed -= _ => OnGrab();
        controls.Player.Throw.performed -= _ => OnThrow();
        controls.Disable();
    }

    void OnGrab()
    {
        if (heldObject == null)
            TryGrabObject();
        else
            DropObject();
    }

    void OnThrow()
    {
        if (heldObject != null)
            ThrowObject();
    }

    void TryGrabObject()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, grabDistance))
        {
            if (hit.rigidbody != null)
            {
                heldObject = hit.rigidbody;
                heldObject.useGravity = false;
                heldObject.drag = 10;
            }
        }
    }

    void MoveObject()
    {
        Vector3 direction = holdPoint.position - heldObject.position;
        heldObject.velocity = direction * moveSpeed;
    }

    void DropObject()
    {
        heldObject.useGravity = true;
        heldObject.drag = 1;
        heldObject = null;
    }

    void ThrowObject()
    {
        heldObject.useGravity = true;
        heldObject.drag = 1;
        heldObject.AddForce(cam.transform.forward * throwForce, ForceMode.Impulse);
        heldObject = null;
    }

    void Update()
    {
        if (heldObject != null)
        {
            MoveObject();
        }
    }
}
