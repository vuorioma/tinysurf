using Godot;

public partial class BaseMenu : Node2D
{
	public const string BaseMenuScene = "res://scenes/base_menu/BaseMenu.tscn";
	public const string MainMenuScene = "res://scenes/main_menu/MainMenu.tscn";
	public const string MapMenuScene = "res://scenes/map_menu/MapMenu.tscn";
	public const string SettingsMenuScene = "res://scenes/settings_menu/SettingsMenu.tscn";
	public const string ImportMenuScene = "res://scenes/import_menu/ImportMenu.tscn";

	// Page to show the next time BaseMenu is loaded, set by Open().
	private static string _initialMenu = MainMenuScene;

	private Control _content;
	private Menu _currentMenu;

	// Switch from another scene (e.g. World) back to the menus, opening the given page.
	public static void Open(SceneTree tree, string menuScene)
	{
		_initialMenu = menuScene;
		tree.ChangeSceneToFile(BaseMenuScene);
	}

	public override void _Ready()
	{
		_content = GetNode<Control>("CanvasLayer/Content");
		GetNode<Button>("CanvasLayer/Quit").Pressed += () => GetTree().Quit();

		Navigate(_initialMenu);
		_initialMenu = MainMenuScene;
	}

	public void Navigate(string menuScene)
	{
		_currentMenu?.QueueFree();

		_currentMenu = GD.Load<PackedScene>(menuScene).Instantiate<Menu>();
		_currentMenu.BaseMenu = this;
		_content.AddChild(_currentMenu);
	}
}
