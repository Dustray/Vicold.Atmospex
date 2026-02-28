﻿using Godot;
using System;

public partial class WindowsBtnCtr : Control
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
}