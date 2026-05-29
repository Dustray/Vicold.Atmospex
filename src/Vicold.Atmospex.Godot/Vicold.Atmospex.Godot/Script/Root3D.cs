using Godot;
using System;
using Vicold.Atmospex.Data;
using Vicold.Atmospex.Godot.Frame;
using Vicold.Atmospex.Godot.Frame.Services;

namespace Vicold.Atmospex.Godot;

public partial class Root3D : Node3D
{
	private Action _openFiledAction;

	public override void _EnterTree()
	{
		// Initialize the application
		App.Initialize();
		base._EnterTree();
		DisplayServer.WindowSetMode(DisplayServer.WindowMode.Maximized);
	}

	public override void _Ready()
	{
		var top = GetNode<Control>("Ui/CanvasLayer/MarginContainer");
		var right = GetNode<Control>("Ui/CanvasLayer/RightFragment");
		var camera = GetNode<MainCamera3D>("Map/Camera3D");
		camera.SetValidPadding(top: (float)top.Size.Y, right: right.Visible ? (float)right.Size.X : 0);


		var fileDialog = GetNode<FileDialog>("Ui/CanvasLayer/OpenFileDialog");
		//fileDialog.GetCancel().Connect("pressed", GetTree(), "quit");
		//fileDialog.GetCloseButton().Connect("pressed", GetTree(), "quit");
		var transport = RenderModuleService.GetService<IInteractionService>();
		transport.Order.RegisterMapBlock("OpenFileDialog", OpenFileDialog);
	}

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//      
	//  }

	public void _OnFileDropped(string[] files, string screen)
	{
		var dataHub = App.GetService<IDataHubService>();
		dataHub.DropFiles(files);
	}

	public void _on_OpenFileDialog_files_selected(string[] files)
	{
		var dataHub = App.GetService<IDataHubService>();
		dataHub.DropFiles(files);
	}

	public void _on_OpenFileDialog_popup_hide()
	{
		_openFiledAction?.Invoke();
		_openFiledAction = null;
	}

	private void OpenFileDialog(OrderInfo info)
	{
		var fileDialog = GetNode<FileDialog>("Ui/CanvasLayer/OpenFileDialog");
		fileDialog.FileMode = FileDialog.FileModeEnum.OpenFile;
		fileDialog.Popup();
		_openFiledAction = info.BlockCompeletedAction;
	}
}
