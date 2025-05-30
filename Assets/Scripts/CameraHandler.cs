using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class CameraHandler : MonoBehaviour
{
    public static CameraHandler Instance; // Singleton instance
    public Camera Camera { get; private set; }

    private const float TURN_SPEED = 80f;

    private const float MOVE_SPEED = 15f;
    private const float SHIFT_MOVE_SPEED = 50f;

    private const float ZOOM_SPEED = 0.8f;
    private const float CAMERA_ANGLE = 1f; // 1f = 45 degrees, the lower the value the higher the camera
    private const float MIN_ZOOM_HEIGHT = 1.5f;
    private const float MAX_ZOOM_HEIGHT = 60f;

    private const int EDGE_SCROLL_MARGIN = 10; // in pixels from screen edge
    private const float RMB_ROTATE_SPEED = 0.15f;
    private Vector3 lastMousePos;

    // Freeze (disable all movement)
    public bool IsFrozen { get; private set; }

    // Pan animation
    public bool IsPanning { get; private set; }
    private float PanDuration;
    private float PanDelay;
    private Vector3 PanSourcePosition;
    private Vector3 PanTargetPosition;
    private GameObject PostPanFollowEntity;
    private bool EnableUnbreakableFollowAfterPan;

    // Follow
    public GameObject FollowedEntity { get; private set; }
    private bool InUnbreakableFollow; // If true, moving camera is disabled until Unfollow()

    // Camera Position
    public float CurrentAngle { get; private set; }
    public Direction CurrentFacingDirection { get; private set; }
    public float OffsetRadius { get; private set; }
    public float CurrentZoom { get; private set; }
    public Vector3 CurrentPosition { get; private set; } // Camera is currently looking at this position

    // Callbacks
    private System.Action OnPanDoneCallback;

    // Events
    public event System.Action OnFacingDirectionChanged;

    private void Awake()
    {
        Instance = this;
        Camera = GetComponent<Camera>();
    }

    private void Start()
    {
        SetZoom(15f);
        SetAngle(180f);
        UpdatePosition();
    }

    private void Update()
    {
        UpdatePanAnimation();
        UpdateFollow();
        HandleInputs();
    }

    private void UpdatePanAnimation()
    {
        if(IsPanning)
        {
            PanDelay += Time.deltaTime;

            if(PanDelay >= PanDuration) // Pan done
            {
                CurrentPosition = PanTargetPosition;
                UpdatePosition();
                FollowedEntity = PostPanFollowEntity;
                IsPanning = false;
                if(EnableUnbreakableFollowAfterPan) InUnbreakableFollow = true;
                OnPanDoneCallback?.Invoke();
            }

            else // Pan in progress
            {
                CurrentPosition = HelperFunctions.SmoothLerp(PanSourcePosition, PanTargetPosition, (PanDelay / PanDuration));
                UpdatePosition();
            }
        }
    }

    private void UpdateFollow()
    {
        if(FollowedEntity != null)
        {
            CurrentPosition = FollowedEntity.transform.position;
            UpdatePosition();
        }
    }

    private void HandleInputs()
    {
        bool isUiElementFocussed = EventSystem.current.currentSelectedGameObject != null;
        if (isUiElementFocussed) return;

        bool canMoveCamera = !InUnbreakableFollow && !IsPanning && !IsFrozen;
        bool isMouseOverUi = HelperFunctions.IsMouseOverUi();
        bool canZoomCamera = !IsFrozen && !isMouseOverUi;

        float moveSpeed = MOVE_SPEED;
        if (Input.GetKey(KeyCode.LeftShift)) moveSpeed = SHIFT_MOVE_SPEED;

        // === ROTATION (Q/E) ===
        if (Input.GetKey(KeyCode.Q)) // Q - Rotate camera anti-clockwise
        {
            CurrentAngle = CurrentAngle += TURN_SPEED * Time.deltaTime;
            UpdatePosition();
        }
        if (Input.GetKey(KeyCode.E)) // E - Rotate camera clockwise
        {
            CurrentAngle = CurrentAngle -= TURN_SPEED * Time.deltaTime;
            UpdatePosition();
        }

        // === MOVEMENT (WASD) ===
        if (Input.GetKey(KeyCode.W) && canMoveCamera) // W - Move camera forward
        {
            MoveForward(moveSpeed);
        }
        if (Input.GetKey(KeyCode.A) && canMoveCamera) // A - Move camera left
        {
            MoveLeft(moveSpeed);
        }
        if (Input.GetKey(KeyCode.S) && canMoveCamera) // S - Move camera backward
        {
            MoveBackward(moveSpeed);
        }
        if (Input.GetKey(KeyCode.D) && canMoveCamera) // D - Move camera right
        {
            MoveRight(moveSpeed);
        }

        // === EDGE SCROLL ===
        if (canMoveCamera && !isMouseOverUi)
        {
            Vector3 mousePos = Input.mousePosition;

            // Mouse is off-screen
            if (mousePos.x < 0 || mousePos.x > Screen.width ||
                mousePos.y < 0 || mousePos.y > Screen.height) { }

            else
            {
                // Left edge
                if (mousePos.x <= EDGE_SCROLL_MARGIN)
                {
                    MoveLeft(moveSpeed);
                }
                // Right edge
                else if (mousePos.x >= Screen.width - EDGE_SCROLL_MARGIN)
                {
                    MoveRight(moveSpeed);
                }

                // Bottom edge
                if (mousePos.y <= EDGE_SCROLL_MARGIN)
                {
                    MoveBackward(moveSpeed);
                }
                // Top edge
                else if (mousePos.y >= Screen.height - EDGE_SCROLL_MARGIN)
                {
                    MoveForward(moveSpeed);
                }
            }
        }

        // === ZOOM (Mouse Scroll) ===
        if (canZoomCamera && Input.mouseScrollDelta.y < 0 && !Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.LeftShift)) // Scroll down - Zoom out
        {
            CurrentZoom += ZOOM_SPEED;
            UpdatePosition();
        }
        if (canZoomCamera && Input.mouseScrollDelta.y > 0 && !Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.LeftShift)) // Scroll up - Zoom in
        {
            CurrentZoom -= ZOOM_SPEED;
            UpdatePosition();
        }

        // === RMB DRAG ROTATION ===
        if (canMoveCamera && Input.GetMouseButton(1))
        {
            // Calculate how much the mouse moved horizontally
            float deltaX = Input.mousePosition.x - lastMousePos.x;

            // Adjust the camera angle based on that horizontal movement
            CurrentAngle -= deltaX * RMB_ROTATE_SPEED;
            UpdatePosition();
        }

        // Always track the last mouse position (end of frame)
        lastMousePos = Input.mousePosition;
    }

    public void Freeze()
    {
        IsFrozen = true;
    }
    public void Unfreeze()
    {
        IsFrozen = false;
    }

    private void MoveForward(float speed)
    {
        float deltaX = -(speed * Mathf.Sin(Mathf.Deg2Rad * CurrentAngle) * Time.deltaTime);
        float deltaZ = -(speed * Mathf.Cos(Mathf.Deg2Rad * CurrentAngle) * Time.deltaTime);
        CurrentPosition += new Vector3(deltaX, 0f, deltaZ);
        UpdatePosition();
        FollowedEntity = null;
    }
    private void MoveBackward(float speed)
    {
        float deltaX = speed * Mathf.Sin(Mathf.Deg2Rad * CurrentAngle) * Time.deltaTime;
        float deltaZ = speed * Mathf.Cos(Mathf.Deg2Rad * CurrentAngle) * Time.deltaTime;
        CurrentPosition += new Vector3(deltaX, 0f, deltaZ);
        UpdatePosition();
        FollowedEntity = null;
    }
    private void MoveLeft(float speed)
    {
        float deltaX = speed * Mathf.Sin(Mathf.Deg2Rad * (CurrentAngle + 90)) * Time.deltaTime;
        float deltaZ = speed * Mathf.Cos(Mathf.Deg2Rad * (CurrentAngle + 90)) * Time.deltaTime;
        CurrentPosition += new Vector3(deltaX, 0f, deltaZ);
        UpdatePosition();
        FollowedEntity = null;
    }
    private void MoveRight(float speed)
    {
        float deltaX = -(speed * Mathf.Sin(Mathf.Deg2Rad * (CurrentAngle + 90)) * Time.deltaTime);
        float deltaZ = -(speed * Mathf.Cos(Mathf.Deg2Rad * (CurrentAngle + 90)) * Time.deltaTime);
        CurrentPosition += new Vector3(deltaX, 0f, deltaZ);
        UpdatePosition();
        FollowedEntity = null;
    }

    public void UpdatePosition()
    {
        CurrentZoom = Mathf.Clamp(CurrentZoom, MIN_ZOOM_HEIGHT, MAX_ZOOM_HEIGHT);

        OffsetRadius = CAMERA_ANGLE * CurrentZoom;

        float cameraOffsetX = Mathf.Sin(Mathf.Deg2Rad * CurrentAngle) * OffsetRadius;
        float cameraOffsetY = Mathf.Cos(Mathf.Deg2Rad * CurrentAngle) * OffsetRadius;
        Direction prevFacingDirection = CurrentFacingDirection;
        CurrentFacingDirection = HelperFunctions.GetDirection8FromAngle(CurrentAngle, offset: 180);
        if (prevFacingDirection != CurrentFacingDirection) OnFacingDirectionChanged?.Invoke();

        if (Camera == null) Camera = GetComponent<Camera>();
        Camera.transform.position = new Vector3(CurrentPosition.x + cameraOffsetX, CurrentPosition.y + CurrentZoom, CurrentPosition.z + cameraOffsetY);
        Camera.transform.LookAt(CurrentPosition);
    }

    public void SetPosition(Vector3 pos)
    {
        CurrentPosition = pos;
        UpdatePosition();
    }

    public void PanTo(float duration, Vector3 targetPos, GameObject postPanFollowEntity = null, bool unbreakableFollow = false, System.Action callback = null)
    {
        // Init pan
        IsPanning = true;
        PanSourcePosition = CurrentPosition;
        PanTargetPosition = targetPos;
        PanDuration = duration;
        PostPanFollowEntity = postPanFollowEntity;
        PanDelay = 0f;
        EnableUnbreakableFollowAfterPan = unbreakableFollow;
        OnPanDoneCallback = callback;

        // Immediately end pan if we are already very close to target position
        if (Vector3.Distance(CurrentPosition, targetPos) <= 0.1f)
        {
            //Debug.Log("Panning camera skipped because it already is at target position");
            PanDelay = PanDuration;
        }
    }
    public void Unfollow()
    {
        FollowedEntity = null;
        InUnbreakableFollow = false;
    }

    public void SetZoom(float height)
    {
        CurrentZoom = height;
        UpdatePosition();
    }

    public void SetAngle(float angle)
    {
        CurrentAngle = angle;
        UpdatePosition();
    }
}

