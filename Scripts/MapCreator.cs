using System;
using System.Collections.Generic;
using Godot;
using PixelDash.Scenes.MainCharacter;

namespace PixelDash.Scripts;

public partial class MapCreator : Node2D
{
    [Export] private MainCharacter _player;

    // 所有chunk的路径
    private readonly List<string> _chunks = new();

    // 所有chunk的数量
    private int _chunksNum;

    // 地图实例数量限制
    private const int ChunksAhead = 3;
    private const int ChunksBehind = 1;

    // 当前被实例化的地图
    private Node2D _currentMap;
    private readonly List<Node2D> _mapsAhead = new();
    private List<Node2D> _mapsBehind = new();

    // 记录当前地图的数量 ? 或许可以被List计数代替
    private int _countAhead, _countBehind;

    public override void _Ready()
    {
        // 获取chunk的路径
        GetChunksPath();

        //初始化第一张地图
        InitializeMap();
    }

    public override void _Process(double delta)
    {
    }

    private void GetChunksPath()
    {
        string[] files = DirAccess.GetFilesAt("res://Scenes/Chunks/");

        foreach (string file in files)
        {
            GD.Print(file);
        }

        foreach (string file in files)
        {
            if (file.EndsWith(".tscn") || file.EndsWith(".scn"))
            {
                _chunks.Add($"res://Scenes/Chunks/{file}");
            }
        }

        _chunksNum = _chunks.Count;
    }

    // 初始化第一张地图
    private void InitializeMap()
    {
        _currentMap = GD.Load<PackedScene>(_chunks[Random.Shared.Next(_chunks.Count)]).Instantiate<Node2D>();
        _currentMap.Position = Vector2.Zero;

        _mapsAhead.Add(GD.Load<PackedScene>(_chunks[Random.Shared.Next(_chunks.Count)]).Instantiate<Node2D>());
        _mapsAhead[^1].Position = new Vector2(_currentMap.Position.X + 960, 0);

        for (var i = 0; i < ChunksAhead - 1; i++)
        {
            int index = Random.Shared.Next(_chunks.Count);
            _mapsAhead.Add(GD.Load<PackedScene>(_chunks[index]).Instantiate<Node2D>());
            _mapsAhead[^1].Position = new Vector2(_mapsAhead[^2].Position.X + 960, 0);
        }

        AddChild(_currentMap);

        for (var i = 0; i < ChunksAhead; i++)
        {
            AddChild(_mapsAhead[i]);
        }
    }
}