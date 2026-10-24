using Godot;

public partial class FinishMenu : CanvasLayer
{
	private Label _time;
	private Button _nextMap;
	private string _nextMapName;

	public override void _Ready()
	{
		_time = GetNode<Label>("Panel/Margin/Content/Time");
		_nextMap = GetNode<Button>("Panel/Margin/Content/NextMap");

		GetNode<Button>("Panel/Margin/Content/MainMenu").Pressed += () =>
			BaseMenu.Open(GetTree(), BaseMenu.MainMenuScene);
		_nextMap.Pressed += OnNextMapPressed;
	}

	// nextMap is null when this was the last map, which disables the next map button.
	public void Setup(long timeMilliseconds, string nextMap)
	{
		_time.Text = Overlay.FormatMilliseconds(timeMilliseconds);
		_nextMapName = nextMap;
		_nextMap.Disabled = nextMap == null;
	}

	private void OnNextMapPressed()
	{
		State.map = _nextMapName;
		GetTree().ReloadCurrentScene();
	}
}
