using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class XRBody : MonoBehaviour
{
    public static bool CanMove = true;
    public static bool CanRotate = true;

    [Header("Transforms")]
    [SerializeField]
    Transform bodyTransform;
    [SerializeField]
    Transform bodyCollider;

    [Header("Height")]
    [SerializeField]
    float bodyHeight = 1.8f;

    [Header("Move")]
    [SerializeField]
    float moveSpeed = 3f;

    [Header("Rotate")]
    [SerializeField]
    float rotateAngle = 5f;
    [SerializeField]
    float rotateTime = 0.02f;
    [SerializeField]
    float rotateDelay = 0.01f;


    Transform cameraT;
    Rigidbody body;
    bool isRotate = false;

    private void OnEnable()
    {
        XRManager.OnXrStarted += OnXrStarted;
    }
    
    private void OnDisable()
    {
        XRManager.OnXrStarted -= OnXrStarted;
    }

    private void OnXrStarted()
    {
        SetBodyHeight();
    }

    private void Start()
    {
        cameraT = Camera.main.transform;
        body = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        Rotate();
    }

    private void FixedUpdate()
    {
        Move();
    }
    
    private void LateUpdate()
    {
        bodyCollider.position = new Vector3(cameraT.position.x, bodyCollider.position.y, cameraT.position.z);
    }

    void Move()
    {
        if (!CanMove)
            return;

        Vector2 joystick = XRHand.Left.joystick;
        Vector3 step = new Vector3(joystick.x, 0, joystick.y);

        step = Quaternion.AngleAxis(cameraT.eulerAngles.y, Vector3.up) * step;
        step = new Vector3(step.x, 0, step.z) * moveSpeed;
#if UNITY_6000_0_OR_NEWER
        step.y = body.linearVelocity.y;
        body.linearVelocity = step;
#else
        step.y = body.velocity.y;
        body.velocity = step;
#endif
    }

    void Rotate()
    {
        if (!CanRotate)
            return;
        if (isRotate)
            return;


        float rotate = XRHand.Right.joystick.x;
        if (Mathf.Abs(rotate) < 0.2f)
            return;

        StartCoroutine(IRotate(rotate));
    }

    IEnumerator IRotate(float delta)
    {
        isRotate = true;

        float time = rotateTime;

        float deltaAngle = (rotateAngle / rotateTime) * Mathf.Sign(delta);
        while (time > 0)
        {
            yield return null;
            transform.RotateAround(cameraT.position, Vector3.up, deltaAngle * Time.deltaTime);
            time -= Time.deltaTime;
        }

        yield return new WaitForSeconds(rotateDelay);

        isRotate = false;
    }

    void SetBodyHeight()
    {
        Vector3 cam = transform.InverseTransformPoint(cameraT.position);
        Vector3 pos = bodyTransform.localPosition;

        pos.y = bodyHeight - cam.y;
        bodyTransform.localPosition = pos;
        //Logger.Log($"[XRBody] Set body height to {bodyTransform.localPosition.y} (cam y: {cameraT.localPosition.y})");
    }
}
