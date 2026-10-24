using Godot;

public partial class SettingsMenu : Menu
{
	public override void _Ready()
	{
		GetNode<Button>("Back").Pressed += () => BaseMenu.Navigate(BaseMenu.MainMenuScene);
	}
}
