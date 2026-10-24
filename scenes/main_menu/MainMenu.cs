using Godot;

public partial class MainMenu : Menu
{
	public override void _Ready()
	{
		GetNode<Button>("Buttons/Play").Pressed += () => BaseMenu.Navigate(BaseMenu.MapMenuScene);
		GetNode<Button>("Buttons/Settings").Pressed += () => BaseMenu.Navigate(BaseMenu.SettingsMenuScene);
		GetNode<Button>("Buttons/ImportMap").Pressed += () => BaseMenu.Navigate(BaseMenu.ImportMenuScene);
	}
}
