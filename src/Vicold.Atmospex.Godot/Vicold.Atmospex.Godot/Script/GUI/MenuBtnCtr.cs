using Godot;
using System;
using Vicold.Atmospex.Godot.Frame;
using Vicold.Atmospex.Godot.Frame.Services;
using Windows.System;

public partial class MenuBtnCtr : Control
{
  private  IInteractionService _interactionService;
    public override void _Ready()
    {
        _interactionService = RenderModuleService.GetService<IInteractionService>();
        var menu1 = GetNode<MenuButton>("MBFile");
        var popup1 = menu1.GetPopup();
        popup1.Connect("id_pressed", new Callable(this, "OnFileMenuItemClick"));

        var menu2 = GetNode<MenuButton>("MBMap");
        var popup2 = menu2.GetPopup();
        popup2.Connect("id_pressed", new Callable(this, "OnMapMenuItemClick"));
    }

    public void OnFileMenuItemClick(int idx)
    {
        var button = GetNode<MenuButton>("MBFile");
        var popup = button.GetPopup();
        GD.Print(popup.GetItemText(idx));
        if (idx == 0)
        {
            _interactionService.Order.Execute("OpenFileDialog", null);
        }
    }

    public void OnMapMenuItemClick(int idx)
    {
        var button = GetNode<MenuButton>("MBFile");
        var popup = button.GetPopup();
        GD.Print(popup.GetItemText(idx));
        if (idx == 0)
        {
            _interactionService.Order.Execute("ResetCamera", null);
        }
    }
}