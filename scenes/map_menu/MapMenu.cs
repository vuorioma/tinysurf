using System.Collections.Generic;
using Godot;

public partial class MapMenu : Menu
{
	private const string MapsDirectory = "res://maps";

	private readonly List<string> _maps = new();
	private int _selectedIndex;

	private TextureRect _preview;
	private RichTextLabel _name;

	public override void _Ready()
	{
		_preview = GetNode<TextureRect>("MapCard/TextureRect");
		_name = GetNode<RichTextLabel>("MapCard/RichTextLabel");

		GetNode<Control>("MapCard/Control").GuiInput += OnMapCardGuiInput;
		GetNode<Button>("Previous").Pressed += () => SelectMap(_selectedIndex - 1);
		GetNode<Button>("Next").Pressed += () => SelectMap(_selectedIndex + 1);
		GetNode<Button>("Back").Pressed += () => BaseMenu.Navigate(BaseMenu.MainMenuScene);

		_maps.AddRange(GetMaps());

		// Return to the map that was just played, if any.
		SelectMap(Mathf.Max(_maps.IndexOf(State.map), 0));
	}

	// All map folder names under res://maps, in the order they're shown and played.
	public static List<string> GetMaps()
	{
		var maps = new List<string>(DirAccess.GetDirectoriesAt(MapsDirectory));
		maps.Sort();
		return maps;
	}

	private void SelectMap(int index)
	{
		if (_maps.Count == 0)
		{
			_name.Text = "[center]No maps found[/center]";
			return;
		}

		_selectedIndex = Mathf.PosMod(index, _maps.Count);
		string map = _maps[_selectedIndex];

		string screenshotPath = $"{MapsDirectory}/{map}/screenshot.jpg";
		_preview.Texture = ResourceLoader.Exists(screenshotPath)
			? GD.Load<Texture2D>(screenshotPath)
			: null;
		_name.Text = $"[center]{GetMapName(map)}[/center]";
	}

	private static string GetMapName(string map)
	{
		string metadataPath = $"{MapsDirectory}/{map}/metadata.json";
		if (FileAccess.FileExists(metadataPath))
		{
			Variant metadata = Json.ParseString(FileAccess.GetFileAsString(metadataPath));
			if (metadata.VariantType == Variant.Type.Dictionary
				&& metadata.AsGodotDictionary().TryGetValue("name", out Variant name))
			{
				return name.AsString();
			}
		}

		return char.ToUpperInvariant(map[0]) + map[1..];
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_left"))
		{
			SelectMap(_selectedIndex - 1);
		}
		else if (@event.IsActionPressed("ui_right"))
		{
			SelectMap(_selectedIndex + 1);
		}
		else if (@event.IsActionPressed("ui_accept"))
		{
			LoadSelectedMap();
		}
	}

	private void OnMapCardGuiInput(InputEvent @event)
	{
		if (@event is not InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true })
		{
			return;
		}

		LoadSelectedMap();
	}

	private void LoadSelectedMap()
	{
		if (_maps.Count == 0)
		{
			return;
		}

		State.map = _maps[_selectedIndex];
		GD.Print($"{State.map} map selected");
		GetTree().ChangeSceneToFile("res://scenes/world/World.tscn");
		Input.MouseMode = Input.MouseModeEnum.Visible;
	}
}
