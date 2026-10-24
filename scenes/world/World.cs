using Godot;

public partial class World : Node3D
{
	private const int CountdownSeconds = 3;
	private const string FinishMenuScene = "res://scenes/finish_menu/FinishMenu.tscn";

	private Player _player;
	private Overlay _overlay;
	private Vector3 _startPosition = Vector3.Zero;
	private Vector3 _endPosition = Vector3.Zero;
	private bool _finished;

	public override void _Ready()
	{
		_player = GetNode<Player>("Player");
		_overlay = GetNode<Overlay>("Overlay");

		var map = State.map;
		var capitalizedMap = map.Length == 0
			? map
			: char.ToUpperInvariant(map[0]) + map[1..];
		var path = $"res://maps/{map}/{capitalizedMap}.glb";
		var packedScene = GD.Load<PackedScene>(path)
			?? throw new System.InvalidOperationException($"Could not load map scene at '{path}'.");
		var scene = packedScene.Instantiate();
		scene.Name = "Map";
		AddChild(scene);

		_startPosition = scene.GetNode<Node3D>("START").GlobalPosition;
		_endPosition = scene.GetNode<Node3D>("END").GlobalPosition;
		_player.Position = _startPosition;
		Input.MouseMode = Input.MouseModeEnum.Captured;
		StartCountdown();
	}

	private async void StartCountdown()
	{
		_player.Frozen = true;
		for (int i = CountdownSeconds; i > 0; i--)
		{
			_overlay.ShowCountdown(i);
			await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
			if (!IsInstanceValid(this))
			{
				return;
			}
		}

		_overlay.HideCountdown();
		_player.Frozen = false;
		_overlay.StartTimer();
	}

	public override void _Process(double delta)
	{
		_overlay.SetSpeed(_player.Velocity.Length());

		var distanceToEnd = _player.Position.DistanceTo(_endPosition);
		if (!_finished && distanceToEnd < 4.0f)
		{
			GD.Print(_startPosition);
			GD.Print(_endPosition);
			GD.Print(distanceToEnd);
			Finish();
		}
	}

	private void Finish()
	{
		_finished = true;
		_player.Frozen = true;
		long time = _overlay.StopTimer();
		Input.MouseMode = Input.MouseModeEnum.Visible;

		var maps = MapMenu.GetMaps();
		int index = maps.IndexOf(State.map);
		string nextMap = index >= 0 && index + 1 < maps.Count ? maps[index + 1] : null;

		var finishMenu = GD.Load<PackedScene>(FinishMenuScene).Instantiate<FinishMenu>();
		AddChild(finishMenu);
		finishMenu.Setup(time, nextMap);
	}
}