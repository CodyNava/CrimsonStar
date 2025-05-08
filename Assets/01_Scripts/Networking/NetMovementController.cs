using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;
using UnityEngine;

public struct MoveReplicateData : IReplicateData
{
    public Vector2 Input;

    public MoveReplicateData(Vector2 input) : this()
    {
        Input = input;
    }

    private uint _tick;
    public void Dispose() {}
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
    public void Dispose() {}
    public uint GetTick() => _tick;
    public void SetTick(uint tick) => _tick = tick;
}

public class NetMovementController : NetworkBehaviour
{
    [SerializeField] private NetBridge bridge;
    
    public PredictionRigidbody2D PredictionRB;

    private Vector2 _input;
    private Turet _inputAsset;
    
    public override void OnStartNetwork()
    {
        TimeManager.OnTick += OnTick;
        TimeManager.OnPostTick += OnPostTick;
    }

    public override void OnStartClient()
    {
        if (IsOwner)
        {
            _inputAsset.Enable();
        }
    }

    public override void OnStopClient()
    {
        if (IsOwner)
        {
            _inputAsset.Disable();
        }
    }

    public override void OnStopNetwork()
    {
        TimeManager.OnTick -= OnTick;
        TimeManager.OnPostTick -= OnPostTick;
    }

    private void OnTick()
    {
        RunInputs(CreateReplicateData());
    }

    private MoveReplicateData CreateReplicateData()
    {
        if (!IsOwner) return default;

        MoveReplicateData md = new MoveReplicateData(_input);
        return md;
    }

    [Replicate]
    private void RunInputs(MoveReplicateData data, ReplicateState state = ReplicateState.Invalid,
        Channel channel = Channel.Unreliable)
    {
        float deltaTime = (float) TimeManager.TickDelta;
        float inputThrust = data.Input.y;
        float inputSteer = -data.Input.x;

        float angularVelocity = PredictionRB.Rigidbody2D.angularVelocity;
        if (Mathf.Abs(inputSteer) > 0.2f)
        {
            angularVelocity += inputSteer * bridge.ComputeRotationSpeed() * deltaTime;
        }
        else
        {
            angularVelocity *= 1f - bridge.GetAngularDampingCoefficient() / 1000f;
        }
        
        Vector2 linearVelocity = PredictionRB.Rigidbody2D.linearVelocity;
        Vector2 thrust = PredictionRB.Rigidbody2D.transform.up * inputThrust;
        if (Mathf.Abs(inputThrust) > 0.2f)
        {
            linearVelocity += bridge.ComputeMovementSpeed() * deltaTime * thrust;
        }
        else
        {
            linearVelocity *= 1f - bridge.GetLinearDampingCoefficient() / 1000f;
        }
        
        PredictionRB.AngularVelocity(angularVelocity);
        PredictionRB.Velocity(linearVelocity);
        PredictionRB.Simulate();
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
        }
    }

    private void OnDestroy()
    {
        PredictionRB = null;
    }
}
