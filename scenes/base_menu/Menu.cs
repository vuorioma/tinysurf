using Godot;

// A page shown inside BaseMenu. BaseMenu is assigned before the page enters the tree.
public partial class Menu : Control
{
	public BaseMenu BaseMenu { get; set; }
}
