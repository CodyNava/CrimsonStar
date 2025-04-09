using _01_Scripts.Ship.ModuleControllers;
using _01_Scripts.Ship.Modules;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private BridgeController _bridgeController;
    private BridgeModuleObject _bridgeModuleObject;
    
    [SerializeField] private float controllerDeadZone = 0.1f;
    
    private Vector2 input;
    private Vector3 velocity;
    private float angularVelocity;

    public void Awake()
    {
        _bridgeController = GetComponent<BridgeController>();
        _bridgeModuleObject = _bridgeController.BridgeObject;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
    }
    
    private void Update()
    {
        if (_bridgeController != null)
        {
            if(_bridgeController.IsCombatActive)
                movePlayer();
        }
        else
        {
            movePlayer();
        }
        
    }

    public void movePlayer()
    {
        if (input.magnitude < controllerDeadZone)
            input = Vector2.zero;

        if (Mathf.Abs(input.x) > 0.01f)
        {
            angularVelocity += -input.x * _bridgeModuleObject.rotationSpeed;
        }
        else
        {
            angularVelocity *= 1f - (_bridgeModuleObject.rotationDamping / 1000f);
        }
        
        angularVelocity = Mathf.Clamp(angularVelocity, -_bridgeModuleObject.maxAngularVelocity, _bridgeModuleObject.maxAngularVelocity);
        
        transform.Rotate(0f, 0f, angularVelocity * Time.deltaTime);
        
        
        if (Mathf.Abs(input.y) > 0.01f)
        {
            Vector3 movement = transform.up * input.y;
            float totalMoveSpeed = Mathf.Max(_bridgeModuleObject.baseMoveSpeed + _bridgeController.MoveSpeedChange, 0f);
            velocity += totalMoveSpeed * movement.normalized;
        }
        else
        {
            velocity *= 1f - (_bridgeModuleObject.movementDamping / 1000f);
        }

        velocity = Vector3.ClampMagnitude(velocity, _bridgeModuleObject.maxSpeed);

        if (velocity.magnitude <= 0.0001f)
        {
            velocity = Vector3.zero;
        }

        transform.Translate(velocity * Time.deltaTime, Space.World);
    }
}
