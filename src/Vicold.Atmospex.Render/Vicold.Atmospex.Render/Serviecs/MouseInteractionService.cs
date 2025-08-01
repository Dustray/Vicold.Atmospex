﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evergine.Framework.Graphics;
using Evergine.Framework.Services;
using Evergine.Mathematics;

namespace Vicold.Atmospex.Render.Serviecs;

public class ResetCameraEventArgs(bool isResetPosition, bool isResetRotation) : EventArgs
{
    public bool IsResetPosition
    {
        get; set;
    } = isResetPosition;
    public bool IsResetRotation
    {
        get; set;
    } = isResetRotation;
}

public class LocalScaleEventArgs(float localScale, Vector3 position) : EventArgs
{
    public float LocalScale
    {
        get; set;
    } = localScale;
    public Vector3 Position
    {
        get; set;
    } = position;
}

public enum MouseScrollMode
{
    Move,
    Scale,
}

public class MouseInteractionService : Service
{
    private const int _defaultZ = 15;
    private const float _minZ = 0.1f;
    private const float _maxZ = 200;
    private const float MaxVerticalAngle = 0.6f; // 最大垂直旋转角度

    private float _x = 0;
    private float _y = 0;
    private float _z = _defaultZ;

    private Point _lastPosition;



    // 旋转
    private float _rotationX = 0;
    private float _rotationY = 0;
    private float _rotationZ = 0;

    public MouseScrollMode ScrollMode
    {
        get; set;
    } = MouseScrollMode.Move;

    public float Displacement
    {
        get; set;
    }

    public int CurrentStepValue
    {
        get; set;
    }

    private float _localStep = 0;
    public float LocalScale
    {
        get; set;
    } = 0;


    public Action<Vector3> CameraPositionChanged
    {
        get; set;
    }

    public Action<Vector3> CameraRotationChanged
    {
        get; set;
    }

    public event EventHandler<ResetCameraEventArgs> CameraReset;
    public event EventHandler CameraHeightChangedEvent;
    public event EventHandler<LocalScaleEventArgs> LocalScaleChangedEvent;

    public void SetPosition(Vector3? initPosition, Vector3? initRotation)
    {
        if (initPosition is { })
        {
            _x = initPosition.Value.X;
            _y = initPosition.Value.Y;
            _z = initPosition.Value.Z;
        }
        CameraHeightChangedEvent?.Invoke(this, null);
        // 重置旋转角度
        if (initRotation is { })
        {
            _rotationX = initRotation.Value.X;
            _rotationY = initRotation.Value.Y;
            _rotationZ = initRotation.Value.Z;
        }
    }

    public void ResetCamera(bool isResetPosition, bool isResetRotation)
    {
        this.CameraReset?.Invoke(this, new(isResetPosition, isResetRotation));
    }

    public void UpdatePosition(float x, float y)
    {
        UpdatePosition(x, y, _z);
    }

    public void UpdatePosition(float x, float y, float z)
    {
        _x = x;
        _y = y;
        _z = z;
        CameraPositionChanged?.Invoke(new Vector3(_x, _y, _z));
    }
    /// <summary>
    /// 鼠标上下滚动 修改高度
    /// </summary>
    /// <param name="position"></param>
    /// <param name="step"></param>
    public void ScrollVertical(Point position, Camera camera, int step)
    {
        if (step == 0) return;

        if ((step > 0 && _z <= _minZ)
            || (step < 0 && _z >= _maxZ))
        {
            return;
        }

        var preZ = _z;
        // 获取缩放偏移
        float scaleOffset = GetScaleOffset(_z, -step);
        CurrentStepValue += step;
        // 计算缩放前的世界坐标
        var worldPositionOffset = Screen2World(position, camera);
        var worldPosition = worldPositionOffset + new Vector2(_x, _y);

        // 计算缩放后的相机高度y
        _z += scaleOffset;
        if (_z < _minZ) _z = _minZ;
        else if (_z > _maxZ) _z = _maxZ;

        #region 压缩前的代码

        //var screen = camera.ScreenViewport;
        //var scaleX = camera.Projection.Scale.X;
        //var scaleY = camera.Projection.Scale.Y;

        //var width = preZoom / scaleX * 2; // 缩放前视野中世界宽度
        //var height = preZoom / scaleY * 2; // 缩放前视野中世界高度

        //var afterWidth = _y / scaleX * 2; // 缩放后视野中世界宽度
        //var afterHeight = _y / scaleY * 2; // 缩放后视野中世界高度


        //var r1x = width / 2 + _x- worldPosition.X; // 缩放前斜边点右长度
        //var r2x = r1x * (afterWidth / 2) / (width / 2); // 缩放后斜边点右长度

        //var r1z = height / 2 + _z- worldPosition.Y;
        //var r2z = r1z * (afterHeight / 2) / (height / 2);

        //_x = worldPosition.X - (afterWidth/2 - r2x);
        //_z = worldPosition.Y - (afterHeight/2 - r2z);

        #endregion

        #region 压缩后的代码

        _x = worldPosition.X + ((_x - worldPosition.X) * _z / preZ);
        _y = worldPosition.Y + ((_y - worldPosition.Y) * _z / preZ);
        UpdatePosition(_x, _y, _z);
        #endregion

        CameraHeightChangedEvent?.Invoke(this, null);
    }

    /// <summary>
    /// 鼠标上下滚动 修改缩放值
    /// </summary>
    /// <param name="position"></param>
    /// <param name="step"></param>
    public void ScrollScale(Point position, Camera camera, int step)
    {
        if (step == 0) { return; }
        var vStep = step / 10f;
        _localStep += step; // step: -1或1
        LocalScale = (float)Math.Pow(1.1, _localStep);

        #region 调整 _x
        var worldPositionOffset = Screen2World(position, camera);// 光标离摄像头的距离
        var worldPosition = new Vector2(_x, _y) + worldPositionOffset; // 鼠标的世界坐标
        var onceScale = (float)Math.Pow(1.1, step);
        _x -= worldPosition.X - worldPosition.X * onceScale;
        // 调整相机世界坐标
        UpdatePosition(_x, _y, _z);
        #endregion

        LocalScaleChangedEvent?.Invoke(this, new(LocalScale, new(_x, _y, _z)));
    }

    private float GetScaleOffset(float zoomScale, int zoomPlusminus)
    {
        return zoomPlusminus * zoomScale / 15;
    }

    public void DragMoving(Point position, Camera camera, bool isTouching)
    {
        if (!isTouching)
        {
            _lastPosition = position;
            return;
        }

        if (position == _lastPosition) return;

        #region 压缩前代码
        //var screen = camera.ScreenViewport;
        //var offset = position - _lastPosition;
        //var scaleX = camera.Projection.Scale.X;
        //var scaleY = camera.Projection.Scale.Y;
        //var width = _y / scaleX * 2;
        //var height = _y / scaleY * 2;
        //var x_off = offset.X * width / screen.Width;
        //var z_off = offset.Y * height / screen.Height;
        //_x -= x_off;
        //_z -= z_off;
        #endregion

        #region 压缩后代码
        var screen = camera.ScreenViewport;
        var offset = position - _lastPosition;
        var scaleX = camera.Projection.Scale.X;
        var scaleY = camera.Projection.Scale.Y;
        _x -= offset.X * (_z / scaleX * 2) / screen.Width;
        _y += offset.Y * (_z / scaleY * 2) / screen.Height;
        UpdatePosition(_x, _y);
        #endregion

        CameraHeightChangedEvent?.Invoke(this, null);
        _lastPosition = position;
    }

    /// <summary>
    /// 新增：鼠标拖动旋转摄像机
    /// </summary>
    /// <param name="position">当前鼠标位置</param>
    /// <param name="isTouching">是否正在拖动</param>
    /// <param name="sensitivity">旋转灵敏度，默认0.5度/像素</param>
    public void DragRotating(Point position, Camera camera, bool isTouching, float sensitivity = 0.001f)
    {
        if (!isTouching)
        {
            _lastPosition = position;
            return;
        }

        if (position == _lastPosition) return;

        // 计算鼠标偏移量
        var offset = position - _lastPosition;

        // 更新旋转角度
        _rotationX += offset.Y * sensitivity;  // 垂直旋转
        _rotationY += offset.X * sensitivity;  // 水平旋转不起作用
        //_rotationZ += offset.X * sensitivity;  // 水平旋转不起作用

        // 限制垂直旋转角度
        _rotationX = Math.Clamp(_rotationX, -MaxVerticalAngle, MaxVerticalAngle);
        _rotationY = Math.Clamp(_rotationY, -MaxVerticalAngle, MaxVerticalAngle);
        //_rotationZ = Math.Clamp(_rotationZ, -MaxVerticalAngle, MaxVerticalAngle);

        CameraRotationChanged?.Invoke(new Vector3(_rotationX, _rotationY, _rotationZ));
        // 触发旋转事件
        _lastPosition = position;
    }

    public Vector2 Screen2World(Point position, Camera camera)
    {
        #region 压缩前代码
        //var screen = camera.ScreenViewport;
        //var scaleX = camera.Projection.Scale.X;
        //var scaleY = camera.Projection.Scale.Y;
        //var width = _y / scaleX * 2;
        //var height = _y / scaleY * 2;

        //var screenOffX = position.X - screen.Width / 2;
        //var screenOffY = position.Y - screen.Height / 2;


        //var worldX = width * screenOffX / screen.Width;
        //var worldZ = height * screenOffY / screen.Height;
        #endregion

        #region 压缩后代码
        var screen = camera.ScreenViewport;
        var scaleX = camera.Projection.Scale.X;
        var scaleY = -camera.Projection.Scale.Y;

        var worldX = (_z / scaleX * 2) * (position.X - screen.Width / 2) / screen.Width;
        var worldY = (_z / scaleY * 2) * (position.Y - screen.Height / 2) / screen.Height;
        #endregion

        return new Vector2(worldX, worldY);
    }

    public Vector3 CameraPosition => new(_x, _y, _z);
}