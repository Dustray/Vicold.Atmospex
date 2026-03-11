using Godot;
using System;

public partial class Root : Node
{
	private Action _openFiledAction;

	public override void _EnterTree()
	{
		base._EnterTree();
		DisplayServer.WindowSetMode(DisplayServer.WindowMode.Maximized);
	}

	public override void _Ready()
	{
		GetTree().Connect("files_dropped", new Callable(this, "_OnFileDropped"));
		var top = GetNode<Control>("CanvasLayer/MarginContainer");
		var right = GetNode<Control>("CanvasLayer/RightFragment");
		var camera = GetNode<MainCamera2D>("Map/Camera2D");
		camera.SetValidPadding(top: (float)top.Size.Y, right: right.Visible ? (float)right.Size.X : 0);


		var fileDialog = GetNode<FileDialog>("CanvasLayer/OpenFileDialog");
		//fileDialog.GetCancel().Connect("pressed", GetTree(), "quit");
		//fileDialog.GetCloseButton().Connect("pressed", GetTree(), "quit");

		//var transport = _launcher.Bus.GetTransport<IInteractionTransport>();
		//transport.Order.RegisterMapBlock("OpenFileDialog", OpenFileDialog);

	}

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//      
	//  }

	public void _OnFileDropped(string[] files, string screen)
	{
		//var dataHub = _launcher.Bus.GetTransport<IDataHubTransport>();
		//dataHub.DropFiles(files);
	}

	public void _on_OpenFileDialog_files_selected(string[] files)
	{
		//var dataHub = _launcher.Bus.GetTransport<IDataHubTransport>();
		//dataHub.DropFiles(files);
	}

	public void _on_OpenFileDialog_popup_hide()
	{
		_openFiledAction?.Invoke();
		_openFiledAction = null;
	}

	//	private void OpenFileDialog(OrderInfo info)
	//	{
	//		var fileDialog = GetNode<FileDialog>("CanvasLayer/OpenFileDialog");
	//		fileDialog.Popup();
	//		_openFiledAction = info.BlockCompeletedAction;
	//	}
}
