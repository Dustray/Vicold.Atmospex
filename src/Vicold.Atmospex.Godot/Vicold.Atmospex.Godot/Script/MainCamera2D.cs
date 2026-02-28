using Godot;
using System;

public partial class MainCamera2D : Camera2D
{
	private const int _defaultScale = 15;

	private bool isDrag = false;
	private Vector2 startPos = Vector2.Zero;
	private Vector2 startCamPos = Vector2.Zero;
	private Vector2 nextPos;
	private float scale = _defaultScale;
	private Vector4 _validPadding = new Vector4();
	private bool _isBlockMouse = false;
	// 暂时注释掉，需要根据Atmospex的服务架构进行调整
	// private IInteractionService _interaction;

	public override void _Ready()
	{
		nextPos = GetViewport().GetVisibleRect().Size * 0.5f;
		Position = nextPos;
		Zoom = new Vector2(scale, scale);
		// 暂时注释掉服务获取，需要根据Atmospex的服务架构进行调整
		// _interaction = RenderModuleService.GetService<IInteractionService>();
		// _interaction.Order.Register("OnMapBlock", (isBlockMouse) => {
		// 	//GD.Print(isBlockMouse.UserData);
		// 	_isBlockMouse = (bool)(isBlockMouse.UserData); 
		// });
		// _interaction.Order.Register("ResetCamera", ResetCamera);
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
				startPos = Vector2.Zero;
				return;
			}
			if (eButton.ButtonIndex == MouseButton.WheelUp)
			{
				// scroll up
				startPos = Vector2.Zero;
				if (scale <= 0.01)
				{
					return;
				}

				float scaleOffset = GetScaleOffset(scale, true);
				scale += scaleOffset;
				ZoomAtPoint(eButton.Position);
			}
			else if (eButton.ButtonIndex == MouseButton.WheelDown)
			{
				// scroll down
				startPos = Vector2.Zero;
				if (scale >= 20)
				{
					return;
				}

				float scaleOffset = GetScaleOffset(scale, false);
				scale += scaleOffset;
				ZoomAtPoint(eButton.Position);
			}

			// 不需要缩放动画 执行直接缩放
			Zoom = new Vector2(scale, scale);
			Position = nextPos;

			void ZoomAtPoint(Vector2 point)
			{
				var viewSize = GetViewport().GetVisibleRect().Size;
				var preZoom = Zoom;
				nextPos = Position + ((viewSize * 0.5f) - point) * (new Vector2(scale, scale) - preZoom);
			}

			if (eButton.ButtonIndex == MouseButton.Left || eButton.ButtonIndex == MouseButton.Middle)
			{
				// left and middle
				if (eButton.IsPressed())
				{
					isDrag = true;
					startPos = eButton.Position;
					startCamPos = Position;
				}
				else
				{
					isDrag = false;
					startPos = Vector2.Zero;
				}
			}
			else if (eButton.ButtonIndex == MouseButton.Right)
			{
				// right
			}
		}
		else if (@event is InputEventMouseMotion eMotion)
		{
			// mouse drag
			if (isDrag)
			{
				if (startPos != Vector2.Zero)
				{
					var offset = startPos - eMotion.Position;
					this.Position = startCamPos + offset * scale;
					nextPos = Position;
				}
			}

			//计算鼠标的世界位置
			var worldPosition = GetGlobalMousePosition();
			// 暂时注释掉鼠标移动事件，需要根据Atmospex的服务架构进行调整
			// _interaction.MouseMove(new Vector2((float)worldPosition.X, (float)worldPosition.Y));
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
		nextPos = GetViewport().GetVisibleRect().Size * 0.5f;
		scale = _defaultScale;
		Zoom = new Vector2(scale, scale);
		Position = nextPos;
	}

	/// <summary>
	/// 获取缩放偏移
	/// </summary>
	/// <param name="isZoomIn"></param>
	/// <returns></returns>
	private float GetScaleOffset(float zoomScale, bool isZoomIn)
	{
		int zoomPlusminus = isZoomIn ? -1 : 1;
		var scaleOffset = zoomPlusminus * zoomScale / 15;
		return scaleOffset;
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
