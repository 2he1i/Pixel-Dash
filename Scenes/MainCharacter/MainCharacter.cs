using Godot;

namespace PixelDash.Scenes.MainCharacter;

public partial class MainCharacter : CharacterBody2D
{
    public enum State
    {
        Idle,
        Run,
        Jump,
        DoubleJump,
        Fall
    }

    private const float Gravity = 1200.0f;
    private const float FallGravityScale = 1.8f;
    private const float JumpForce = -360.0f;

    private float _speed = 200.0f;

    private State _state;

    public State GetState()
    {
        return _state;
    }

    private AnimatedSprite2D _anim;
    private Timer _gameStartTimer;

    public override void _Ready()
    {
        _anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        // Initialize State
        ChangeState(State.Idle);

        // Set StartTimer
        _gameStartTimer = GetNode<Timer>("GameStartTimer");
        _gameStartTimer.Timeout += OnTimeout;
        _gameStartTimer.Start();
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("Jump"))
        {
            switch (_state)
            {
                case State.Run: ChangeState(State.Jump); break;
                case State.Jump: ChangeState(State.DoubleJump); break;
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        // GD.Print(Velocity);

        // 重力
        if (!IsOnFloor())
        {
            if (_state == State.Fall)
            {
                Velocity = new Vector2(Velocity.X, Velocity.Y + Gravity * FallGravityScale * (float)delta);
            }
            else
            {
                Velocity = new Vector2(Velocity.X, Velocity.Y + Gravity * (float)delta);
            }
        }

        // 自动奔跑
        // if (_state != State.Idle)
        // {
        //     Velocity = new Vector2(_speed, Velocity.Y);
        // }

        // 跳跃逻辑
        if (_state == State.Jump)
        {
            if (IsOnFloor())
            {
                Velocity = new Vector2(Velocity.X, JumpForce);
            }

            if (Velocity.Y > 0)
            {
                ChangeState(State.Fall);
            }
        }

        // 下落逻辑
        if (_state == State.Fall)
        {
            if (IsOnFloor())
            {
                ChangeState(State.Run);
            }
        }

        MoveAndSlide();
    }

    private void ChangeState(State newState)
    {
        _state = newState;

        switch (_state)
        {
            case State.Idle: _anim.Play("Idle"); break;
            case State.Run: _anim.Play("Run"); break;
            case State.Jump: _anim.Play("Jump"); break;
            case State.DoubleJump: _anim.Play("DoubleJump"); break;
            case State.Fall: _anim.Play("Fall"); break;
        }
    }

    // 游戏开始计时
    private void OnTimeout()
    {
        ChangeState(State.Run);
    }
}