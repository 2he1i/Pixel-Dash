using Godot;
using PixelDash.Scenes.MainCharacter;

namespace PixelDash.Scripts;

public partial class Chunk : Node2D
{
    private Area2D _boundary;

    [Signal]
    public delegate void PlayerPassEventHandler();

    public override void _Ready()
    {
        _boundary = GetNode<Area2D>("Area2D");
        _boundary.BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node body)
    {
        if (body is MainCharacter)
        {
            EmitSignal(SignalName.PlayerPass);
        }
    }
}