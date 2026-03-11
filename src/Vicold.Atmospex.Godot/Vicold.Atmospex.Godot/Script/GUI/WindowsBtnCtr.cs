﻿﻿﻿﻿using Godot;
using System;

public partial class WindowsBtnCtr : HBoxContainer
{
    public override void _Ready()
    {
        // 暂时注释掉，需要根据Atmospex的服务架构进行调整
    }

    private void OnMinimizeButtonPressed()
    {
        // 暂时注释掉，需要根据Atmospex的服务架构进行调整
        // OS.WindowMinimize();
    }

    private void OnMaximizeButtonPressed()
    {
        // 暂时注释掉，需要根据Atmospex的服务架构进行调整
        // OS.WindowFullscreen = !OS.WindowFullscreen;
    }

    private void OnCloseButtonPressed()
    {
        // 暂时注释掉，需要根据Atmospex的服务架构进行调整
        // GetTree().Quit();
    }

    private void _on_FullScreenBtn_toggled(bool pressed)
    {
        // 实现全屏切换功能
        DisplayServer.WindowSetMode(pressed ? DisplayServer.WindowMode.Fullscreen : DisplayServer.WindowMode.Windowed);
    }

    private void _on_ExpandRightBtn_toggled(bool isExpand)
    {
        // 实现右侧面板展开/收起功能
        var right = GetTree().Root.GetNode<Control>("Root/CanvasLayer/RightFragment");
        if (right != null)
        {
            right.Visible = isExpand;
        }
    }
}