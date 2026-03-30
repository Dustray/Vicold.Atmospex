using Godot;
using System;

public partial class WindowsBtnCtr : HBoxContainer
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	/// <summary>
	/// 是否全屏
	/// </summary>
	/// <param name="isFullscreen"></param>
	public void _on_FullScreenBtn_toggled(bool isFullscreen)
	{
		DisplayServer.WindowSetMode(isFullscreen ? DisplayServer.WindowMode.Fullscreen : DisplayServer.WindowMode.Windowed);
	}

	/// <summary>
	/// 是否收起右边栏
	/// </summary>
	/// <param name="isFullscreen"></param>
	public void _on_ExpandRightBtn_toggled(bool isExpand)
	{
		var right = GetTree().Root.GetNode<MarginContainer>("Root3d/Ui/CanvasLayer/RightFragment");
		right.Visible = isExpand;

		var camera = GetTree().Root.GetNode<MainCamera3D>("Root3d/Map/Camera3D");
		camera.SetValidPadding(right: isExpand ? (float)right.Size.X : 0);
	}
	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//      
	//  }
}
