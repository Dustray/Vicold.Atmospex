using Godot;
using System;
using Vicold.Atmospex.Godot.Frame;
using Vicold.Atmospex.Godot.Frame.Services;

public partial class QuickButtonBar : HBoxContainer
{
  private  IInteractionService _interactionService;
	public override void _Ready()
	{
		_interactionService = RenderModuleService.GetService<IInteractionService>();
	}

	private void OnQuickButtonPressed(string buttonId)
	{
		_interactionService.Order.Execute(buttonId, null);
	}

	private void _on_OpenFileBtn_button_down()
	{
		GD.Print("ssssssssssaaas");

	   _interactionService.Order.Execute("OpenFileDialog", null);
	}

	private void _on_RefreshBtn_button_down()
	{
		_interactionService.Order.Execute("ResetCamera", null);
	}
}
