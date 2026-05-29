using Godot;
using System;
using Vicold.Atmospex.Earth;
using Vicold.Atmospex.Godot.Frame;
using Vicold.Atmospex.Godot.Frame.Services;

public partial class MainCamera3D : Camera3D
{
	private const float _defaultDistance = 10.0f;
	private const float _minDistance = 1.0f;
	private const float _maxDistance = 50.0f;

	private bool isDrag = false;
	private Vector2 startMousePos = Vector2.Zero;
	private Vector3 startCamPosition = Vector3.Zero;
	private Vector3 targetPosition = Vector3.Zero;
	private float distance = _defaultDistance;
	private IEarthModuleService _map;
	private Vector4 _validPadding = new Vector4();
	private bool _isBlockMouse = false;
	private IInteractionService _interaction;

	public override void _Ready()
	{
		targetPosition = Position;
		distance = _defaultDistance;
		// 获取服务
		_map = RenderModuleService.GetService<IEarthModuleService>();
		_interaction = RenderModuleService.GetService<IInteractionService>();
		// 注册事件
		_interaction.Order.Register("OnMapBlock", (isBlockMouse) => {
			_isBlockMouse = (bool)(isBlockMouse.UserData); 
		});
		_interaction.Order.Register("ResetCamera", ResetCamera);
		// 初始化缩放
		_map.ChangeScale(1.0f / distance);
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (_isBlockMouse) return;
		if (@event is InputEventMouseButton eButton)
		{
			if (!IsMouseInValidArea(eButton.Position))
			{
				isDrag = false;
				startMousePos = Vector2.Zero;
				return;
			}
			if (eButton.ButtonIndex == MouseButton.WheelUp)
			{
				// scroll up - zoom in
				startMousePos = Vector2.Zero;
				if (distance <= _minDistance)
				{
					return;
				}

				float distanceOffset = GetDistanceOffset(distance, true);
				distance += distanceOffset;
				UpdateCameraPosition();
				// 更新缩放
				_map.ChangeScale(1.0f / distance);
			}
			else if (eButton.ButtonIndex == MouseButton.WheelDown)
			{
				// scroll down - zoom out
				startMousePos = Vector2.Zero;
				if (distance >= _maxDistance)
				{
					return;
				}

				float distanceOffset = GetDistanceOffset(distance, false);
				distance += distanceOffset;
				UpdateCameraPosition();
				// 更新缩放
				_map.ChangeScale(1.0f / distance);
			}

			if (eButton.ButtonIndex == MouseButton.Left || eButton.ButtonIndex == MouseButton.Middle)
			{
				// left and middle - pan
				if (eButton.IsPressed())
				{
					isDrag = true;
					startMousePos = eButton.Position;
					startCamPosition = targetPosition;
				}
				else
				{
					isDrag = false;
					startMousePos = Vector2.Zero;
				}
			}
			else if (eButton.ButtonIndex == MouseButton.Right)
			{
				// right - rotate (if needed)
			}
		}
		else if (@event is InputEventMouseMotion eMotion)
		{
			// mouse drag - pan
			if (isDrag)
			{
				if (startMousePos != Vector2.Zero)
				{
					var offset = startMousePos - eMotion.Position;
					// Convert screen space offset to world space
					var worldOffset = new Vector3(offset.X * 0.01f, offset.Y * 0.01f, 0);
					targetPosition = startCamPosition + worldOffset;
					UpdateCameraPosition();
				}
			}

			//计算鼠标的世界位置
			var worldPosition = ScreenToWorld(eMotion.Position);
			// 触发鼠标移动事件
			_interaction.MouseMove(new System.Numerics.Vector2((float)worldPosition.X, (float)worldPosition.Y));
			// 通知地球服务鼠标移动
			_map.ChangeMouse((float)worldPosition.X, (float)worldPosition.Y, eMotion.Position.X, eMotion.Position.Y);
		}
		else if (@event is InputEventKey eKey)
		{
			// reset
			if (eKey.Keycode == Key.Space)
			{
				ResetCamera(null);
			}
		}
	}

	public void SetValidPadding(float top = -1, float bottom = -1, float left = -1, float right = -1)
	{
		_validPadding.X = left == -1 ? _validPadding.X : left;
		_validPadding.Y = top == -1 ? _validPadding.Y : top;
		_validPadding.Z = right == -1 ? _validPadding.Z : right;
		_validPadding.W = bottom == -1 ? _validPadding.W : bottom;
	}

	private void ResetCamera(object obj)
	{
		targetPosition = Vector3.Zero;
		distance = _defaultDistance;
		UpdateCameraPosition();
		// 更新缩放
		_map.ChangeScale(1.0f / distance);
	}

	/// <summary>
	/// 获取距离偏移
	/// </summary>
	/// <param name="currentDistance"></param>
	/// <param name="isZoomIn"></param>
	/// <returns></returns>
	private float GetDistanceOffset(float currentDistance, bool isZoomIn)
	{
		int direction = isZoomIn ? -1 : 1;
		var distanceOffset = direction * currentDistance / 15;
		return distanceOffset;
	}

	/// <summary>
	/// 更新相机位置
	/// </summary>
	private void UpdateCameraPosition()
	{
		// 保持相机朝向原点，通过调整Z轴位置来实现缩放
		Position = targetPosition + new Vector3(0, 0, distance);
		LookAt(targetPosition, Vector3.Up);
	}

	/// <summary>
	/// 屏幕坐标转换为世界坐标
	/// </summary>
	/// <param name="screenPos"></param>
	/// <returns></returns>
	private Vector3 ScreenToWorld(Vector2 screenPos)
	{
		// 使用射线检测获取世界坐标
		var cameraRay = ProjectRayNormal(screenPos);
		// 假设地面在Z=0平面
		var t = -Position.Z / cameraRay.Z;
		var worldPos = Position + cameraRay * t;
		return worldPos;
	}

	private bool IsMouseInValidArea(Vector2 mousePosition)
	{
		var viewport = GetViewport().GetVisibleRect();

		return mousePosition.X >= viewport.Position.X + _validPadding.X
			&& mousePosition.Y >= viewport.Position.Y + _validPadding.Y
			&& mousePosition.X <= viewport.Size.X - _validPadding.Z
		&& mousePosition.Y <= viewport.Size.Y - _validPadding.W;
	}
}
