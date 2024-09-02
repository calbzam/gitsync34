using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class BatteryPickup : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Vector2 _rightOffset = new Vector2(0, 0.6f);
    [SerializeField] private float _zRotation = 8;
    private Vector2 _leftOffset;
    private Quaternion _leftRotation, _rightRotation;

    public RigidbodyConstraints2D OrigRbConstraints { get; private set; } // unused at the moment

    public bool IsPickable { get; set; }
    public bool PlayerIsInRange { get; set; }
    public bool IsHeldByPlayer { get; private set; }
    public bool BatteryIsInBox { get; private set; }

    private void OnEnable()
    {
        CentralInputReader.Input.Player.PickupActivate.started += PickupActivateStarted;
    }

    private void OnDisable()
    {
        CentralInputReader.Input.Player.PickupActivate.started -= PickupActivateStarted;
    }

    private void Start()
    {
        _rb.simulated = false;
        _leftOffset = new Vector2(-_rightOffset.x, _rightOffset.y);
        _leftRotation = Quaternion.Euler(new Vector3(0, 0, -_zRotation));
        _rightRotation = Quaternion.Euler(new Vector3(0, 0, _zRotation));

        IsPickable = true;
        PlayerIsInRange = false;
        IsHeldByPlayer = false;
        BatteryIsInBox = true;
        OrigRbConstraints = _rb.constraints;
    }

    private void Update()
    {
        if (IsHeldByPlayer) SetBatteryFacingDir();
    }

    private void PickupActivateStarted(InputAction.CallbackContext ctx)
    {
        if (IsPickable) ToggleAttachBatteryToPlayer();
    }

    public void SetBatteryParent(Transform toParent)
    {
        _rb.simulated = (toParent == null) ? true : false;
        transform.SetParent(toParent);
    }

    public void SetLocalTransform(Vector3 offset, Quaternion quatRot)
    {
        transform.localPosition = offset;
        transform.rotation = quatRot;
    }

    private void SetBatteryFacingDir()
    {
        if (PlayerLogic.PlayerAnim.FaceDirX < 0)
            SetLocalTransform(_leftOffset, _leftRotation);

        else if (PlayerLogic.PlayerAnim.FaceDirX > 0)
            SetLocalTransform(_rightOffset, _rightRotation);
    }

    public void ToggleAttachBatteryToPlayer()
    {
        if (IsHeldByPlayer)
        {
            DetachFromPlayer();
        }
        else
        {
            if (PlayerIsInRange) AttachToPlayer();
        }
    }

    public void DetachFromPlayer()
    {
        SetBatteryParent(null);
        IsHeldByPlayer = false;
    }

    public void AttachToPlayer()
    {
        SetBatteryParent(PlayerLogic.Player.transform);
        SetBatteryFacingDir();

        if (BatteryIsInBox) BatteryIsInBox = false;
        IsHeldByPlayer = true;
    }
}
