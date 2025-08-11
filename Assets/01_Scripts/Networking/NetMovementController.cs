using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Object.Synchronizing;
using FMOD;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using Channel = FishNet.Transporting.Channel;

public struct MoveReplicateData : IReplicateData
{
    public Vector2 Input;
    public bool IsGamepad;

    public MoveReplicateData(Vector2 input, bool isGamepad) : this()
    {
        Input = input;
        IsGamepad = isGamepad;
    }

    private uint _tick;

    public void Dispose()
    {
    }

    public uint GetTick() => _tick;
    public void SetTick(uint tick) => _tick = tick;
}

public struct MoveReconcileData : IReconcileData
{
    public PredictionRigidbody2D PredictionRB;

    public MoveReconcileData(PredictionRigidbody2D prb) : this()
    {
        PredictionRB = prb;
    }

    private uint _tick;

    public void Dispose()
    {
    }

    public uint GetTick() => _tick;
    public void SetTick(uint tick) => _tick = tick;
}

public class NetMovementController : NetworkBehaviour
{
    [SerializeField] private NetBridge bridge;
    [SerializeField] private VisualEffect startDustVFX;

    public PredictionRigidbody2D PredictionRB;

    private readonly SyncVar<float> _inputThrust = new();

    public float InputThrust => _inputThrust.Value;

    private Vector2 _input;
    private Turet _inputAsset;

    public override void OnStartNetwork()
    {
        TimeManager.OnTick += OnTick;
        TimeManager.OnPostTick += OnPostTick;
    }

    public override void OnStopNetwork()
    {
        TimeManager.OnTick -= OnTick;
        TimeManager.OnPostTick -= OnPostTick;
    }

    public override void OnStartClient()
    {
        if (IsOwner)
        {
            _inputAsset.Enable();
        }
        else startDustVFX.gameObject.SetActive(false);
        
    }

    public override void OnStopClient()
    {
        if (IsOwner)
        {
            _inputAsset.Disable();
        }
    }

    private void OnTick()
    {
        if (IsOwner)
        {
            S_SetInputThrust(_input.y);
        }

        RunInputs(CreateReplicateData());
    }

    private MoveReplicateData CreateReplicateData()
    {
        if (!IsOwner) return default;

        Vector2 input = _input;
        bool isGamepad = Gamepad.current != null && Gamepad.current.leftStick.ReadValue().sqrMagnitude > 0.01f;
        return new MoveReplicateData(input, isGamepad);
    }


    [Replicate]
    private void RunInputs(MoveReplicateData data, ReplicateState state = ReplicateState.Invalid,
        Channel channel = Channel.Unreliable)
    {
        float deltaTime = (float)TimeManager.TickDelta;

        if (data.IsGamepad)
        {
            // Controller
            Vector2 inputDir = data.Input.normalized; 

            if (inputDir.sqrMagnitude > 0.01f)
            {
                float targetAngle = Mathf.Atan2(inputDir.y, inputDir.x) * Mathf.Rad2Deg - 90f; 
                float currentAngle = PredictionRB.Rigidbody2D.rotation; 
                float angleDiff = Mathf.DeltaAngle(currentAngle, targetAngle); 

                float angularVelocity = PredictionRB.Rigidbody2D.angularVelocity;
                
                float rotateForce = angleDiff * 5f - angularVelocity * 0.8f;
                angularVelocity += rotateForce * deltaTime;
                
                float angularDamping = bridge.GetAngularDampingCoefficient() / 1000f;
                angularVelocity *= 1f - angularDamping;
                
                float maxAngular = bridge.GetMaxAngularVelocity();
                angularVelocity = Mathf.Clamp(angularVelocity, -maxAngular, maxAngular);

                PredictionRB.AngularVelocity(angularVelocity);
            }

            Vector2 forward = PredictionRB.Rigidbody2D.transform.up;
            Vector2 thrust = forward * data.Input.magnitude; 

            Vector2 linearVelocity = PredictionRB.Rigidbody2D.linearVelocity; 
            linearVelocity += bridge.ComputeMovementSpeed() * deltaTime * thrust; 

            linearVelocity.x = Mathf.Clamp(linearVelocity.x, -bridge.GetMaxMoveSpeed(), bridge.GetMaxMoveSpeed());
            linearVelocity.y = Mathf.Clamp(linearVelocity.y, -bridge.GetMaxMoveSpeed(), bridge.GetMaxMoveSpeed());

            float dampingX = bridge.GetLinearDampingCoefficient() / 1000f;
            float dampingY = bridge.GetLinearDampingCoefficient() / 1000f;
            if (Mathf.Abs(linearVelocity.x) > Mathf.Abs(linearVelocity.y)) dampingY *= 2f;
            else dampingX *= 2f;

            linearVelocity.x *= 1f - dampingX;
            linearVelocity.y *= 1f - dampingY;
            
            PredictionRB.Velocity(linearVelocity);
            PredictionRB.Simulate();
        }
        else
        {
            //KeyBoard
            float inputThrust = data.Input.y;
            float inputSteer = -data.Input.x;

            float angularVelocity = PredictionRB.Rigidbody2D.angularVelocity;
            if (Mathf.Abs(inputSteer) > 0.2f)
            {
                angularVelocity += inputSteer * bridge.ComputeRotationSpeed() * deltaTime;
                angularVelocity = Mathf.Clamp(angularVelocity, -bridge.GetMaxAngularVelocity(),
                    bridge.GetMaxAngularVelocity());
            }
            else
            {
                angularVelocity *= 1f - bridge.GetAngularDampingCoefficient() / 1000f;
            }

            Vector2 linearVelocity = PredictionRB.Rigidbody2D.linearVelocity;
            Vector2 thrust = PredictionRB.Rigidbody2D.transform.up * inputThrust;
            if (Mathf.Abs(inputThrust) > 0.2f)
            {
                float dampingX = bridge.GetLinearDampingCoefficient() / 1000f;
                float dampingY = bridge.GetLinearDampingCoefficient() / 1000f;

                linearVelocity += bridge.ComputeMovementSpeed() * deltaTime * thrust;
                linearVelocity.x = Mathf.Clamp(linearVelocity.x, -bridge.GetMaxMoveSpeed(), bridge.GetMaxMoveSpeed());
                linearVelocity.y = Mathf.Clamp(linearVelocity.y, -bridge.GetMaxMoveSpeed(), bridge.GetMaxMoveSpeed());

                if (Mathf.Abs(linearVelocity.x) > Mathf.Abs(linearVelocity.y)) dampingY *= 2f;
                else dampingX *= 2f;

                linearVelocity.x *= 1f - dampingX;
                linearVelocity.y *= 1f - dampingY;
            }
            else
            {
                linearVelocity *= 1f - bridge.GetLinearDampingCoefficient() / 1000f;
            }

            PredictionRB.AngularVelocity(angularVelocity);
            PredictionRB.Velocity(linearVelocity);
            PredictionRB.Simulate();
        }
    }

    private void CalculateStarDust(Vector2 linearVelocity)
    {
        startDustVFX.SetFloat("Set_SpeedInput",
            linearVelocity.magnitude / bridge.BridgeModule.Bridge.GetMaxMoveSpeed());
        if (bridge.CameraZoom.IsUnityNull()) return;
        startDustVFX.SetFloat("Set_CameraZoom",
            bridge.CameraZoom.CameraFollow.OffSet.magnitude / bridge.CameraZoom.CameraZoomSettings.MaxDistance);
    }

    private void OnPostTick()
    {
        CreateReconcile();
    }

    public override void CreateReconcile()
    {
        MoveReconcileData rd = new MoveReconcileData(PredictionRB);
        ReconcileState(rd);
    }

    [Reconcile]
    private void ReconcileState(MoveReconcileData data, Channel channel = Channel.Unreliable)
    {
        PredictionRB.Reconcile(data.PredictionRB);
    }

    private void Awake()
    {
        PredictionRB = new PredictionRigidbody2D();
        PredictionRB.Initialize(GetComponent<Rigidbody2D>());
        _inputAsset = new Turet();
        
    }

    private void Update()
    {
        if (IsOwner)
        {
            _input = _inputAsset.Player.Move.ReadValue<Vector2>();
            if (_input.y < 0)
            {
                _input *= new Vector2(1f, 0.5f);
            }
            //CalculateStarDust(PredictionRB.Rigidbody2D.linearVelocity);
        }
    }

    [ServerRpc]
    public void S_SetInputThrust(float thrust)
    {
        _inputThrust.Value = thrust;
    }

    private void OnDestroy()
    {
        PredictionRB = null;
    }
}