using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using PixelDash.Scenes.MainCharacter;

namespace PixelDash.Scripts;

public partial class MapCreator : Node2D
{
    // 访问Player的状态
    [Export] private MainCharacter _player;

    // 专门用于Debug的Label
    [Export] private Label _debug;

    // 所有chunk的路径
    private string[] _chunks;

    // 所有chunk的数量
    private int _chunksNum;

    // 地图实例数量限制
    private const int AheadLimit = 3;
    private const int BehindLimit = 1;

    // 当前被实例化的地图
    private Chunk _currentMap;
    private readonly List<Chunk> _chunksAhead = new();
    private readonly List<Chunk> _chunksBehind = new();

    public override void _Ready()
    {
        // 获取chunk的路径
        GetChunksPath();

        //初始化第一张地图
        InitializeMap();
    }

    public override void _Process(double delta)
    {
        _debug.Text =
            $"FPS: {Engine.GetFramesPerSecond()}\n后方:{_chunksBehind.Count}当前:{_currentMap}前方{_chunksAhead.Count}";
    }

    private void GetChunksPath()
    {
        string[] files = DirAccess.GetFilesAt("res://Scenes/Chunks/");

        _chunks = files
            .Where(f => f.EndsWith("tscn") || f.EndsWith("scn"))
            .Select(f => $"res://Scenes/Chunks/{f}")
            .ToArray();

        _chunksNum = _chunks.Length;
    }

    // 初始化第一张地图
    private void InitializeMap()
    {
        // 起始chunk
        _currentMap = GD.Load<PackedScene>(_chunks[Random.Shared.Next(_chunksNum)]).Instantiate<Chunk>();
        _currentMap.Position = Vector2.Zero;
        _currentMap.Player = _player;
        _currentMap.PlayerPass += UpdateMap;
        // 添加到场景树中以便访问ChunkLength属性 因为_Ready函数会在添加到场景树后执行
        AddChild(_currentMap);

        // 前方第一个chunk
        _chunksAhead.Add(GD.Load<PackedScene>(_chunks[Random.Shared.Next(_chunksNum)]).Instantiate<Chunk>());
        _chunksAhead[^1].Position = new Vector2(_currentMap.Position.X + _currentMap.NextChunkRelX, 0);
        _chunksAhead[^1].Player = _player;

        AddChild(_chunksAhead[^1]);

        // 前方2,3...n个chunk
        for (var i = 0; i < AheadLimit - 1; i++)
        {
            int index = Random.Shared.Next(_chunksNum);
            _chunksAhead.Add(GD.Load<PackedScene>(_chunks[index]).Instantiate<Chunk>());
            _chunksAhead[^1].Position = new Vector2(_chunksAhead[^2].Position.X + _chunksAhead[^2].NextChunkRelX, 0);
            _chunksAhead[^1].Player = _player;

            AddChild(_chunksAhead[^1]);
        }
    }

    // 玩家通过一个chunk后更新地图
    private void UpdateMap()
    {
        // 将已通过chunk移出当前map
        _chunksBehind.Add(_currentMap);

        // 清理前面的chunk
        if (_chunksBehind.Count == BehindLimit + 1)
        {
            GD.Print("清理");
            _chunksBehind[0].QueueFree();
            _chunksBehind.RemoveAt(0);
        }

        // 当前map指向新的chunk
        _currentMap = _chunksAhead[0];
        _chunksAhead.RemoveAt(0);

        // 订阅玩家通过信号
        _currentMap.PlayerPass += UpdateMap;

        // 添加新的chunk到map
        _chunksAhead.Add(GD.Load<PackedScene>(_chunks[Random.Shared.Next(_chunksNum)]).Instantiate<Chunk>());
        _chunksAhead[^1].Position = new Vector2(_chunksAhead[^2].Position.X + _chunksAhead[^2].NextChunkRelX, 0);
        _chunksAhead[^1].Player = _player;

        AddChild(_chunksAhead[^1]);
    }
}