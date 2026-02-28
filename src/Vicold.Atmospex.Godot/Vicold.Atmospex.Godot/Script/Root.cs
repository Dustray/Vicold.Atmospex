using Godot;
using System;

public partial class Root : Node
{
	public override void _Ready()
	{
		// 暂时注释掉，需要根据Atmospex的服务架构进行调整
		// Launcher.Init();
	}

	public void _Process(float delta)
	{
		// 暂时注释掉，需要根据Atmospex的服务架构进行调整
		// base._Process(delta);
	}

	private void OnTestButtonPressed()
	{
		// 暂时注释掉，需要根据Atmospex的服务架构进行调整
		// var interaction = RenderModuleService.GetService<IInteractionService>();
		// interaction.Order.Execute("TestCommand", null);
	}

	public void _on_OpenFileDialog_files_selected(string[] files)
	{
		// 暂时注释掉，需要根据Atmospex的服务架构进行调整
	}

	public void _on_OpenFileDialog_popup_hidden()
	{
		// 暂时注释掉，需要根据Atmospex的服务架构进行调整
	}
}
