using Godot;
using PixelDash.Scenes.MainCharacter;

namespace PixelDash.Scripts;

public partial class Chunk : Node2D
{
    // 获取玩家状态以控制速度
    public MainCharacter Player;

    private Area2D _boundary;
    private TileMapLayer _layer;

    private float _speed = 200.0f;

    [Signal]
    public delegate void PlayerPassEventHandler();

    // 下一个chunk的相对x坐标 因为Chunk的Position时刻在变
    public float NextChunkRelX { get; private set; }

    public override void _Ready()
    {
        // 计算下一个chunk的相对x坐标
        _layer = GetNode<TileMapLayer>("TileMapLayer");
        NextChunkRelX = (_layer.GetUsedRect().End.X + 1) * _layer.TileSet.TileSize.X;

        _boundary = GetNode<Area2D>("Boundary");
        // 设置碰撞线的位置为chunk末端
        _boundary.Position = new Vector2(NextChunkRelX, _boundary.Position.Y);
        _boundary.BodyEntered += OnBodyEntered;
    }

    // 场景自动移动
    public override void _PhysicsProcess(double delta)
    {
        if (Player.GetState() != MainCharacter.State.Idle)
        {
            Position = new Vector2(Position.X - _speed * (float)delta, Position.Y);
        }
    }

    private void OnBodyEntered(Node body)
    {
        if (body is MainCharacter)
        {
            CallDeferred(GodotObject.MethodName.EmitSignal, SignalName.PlayerPass);
        }
    }
}