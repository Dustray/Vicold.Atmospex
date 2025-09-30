using Evergine.Common.Graphics;
using Evergine.Components.Graphics3D;
using Evergine.Framework.Graphics;
using Evergine.Mathematics;
using System;
using Vicold.Atmospex.CoreService;
using Vicold.Atmospex.Layer;
using Vicold.Atmospex.Render.Serviecs;

namespace Vicold.Atmospex.Render.Frame;

public interface IRenderModuleService : IModuleService
{
    /// <summary>
    /// 获取当前视口信息
    /// </summary>
    /// <returns>视口信息对象</returns>
    ViewportInfo Viewport { get; }

    // 接口成员
    //void Bind(ILayerModuleService layerModuleService);

    /// <summary>
    /// 屏幕坐标转换为世界坐标
    /// </summary>
    /// <param name="screenX">屏幕X坐标</param>
    /// <param name="screenY">屏幕Y坐标</param>
    /// <returns>世界坐标</returns>
    Vector2 ScreenToWorld(float screenX, float screenY);

    /// <summary>
    /// 世界坐标转换为屏幕坐标
    /// </summary>
    /// <param name="worldX">世界X坐标</param>
    /// <param name="worldY">世界Y坐标</param>
    /// <returns>屏幕坐标</returns>
    Vector2 WorldToScreen(float worldX, float worldY);

    /// <summary>
    /// 经纬度转换为世界坐标
    /// </summary>
    /// <param name="longitude">经度</param>
    /// <param name="latitude">纬度</param>
    /// <returns>世界坐标</returns>
    Vector2 GeoToWorld(double longitude, double latitude);

    /// <summary>
    /// 世界坐标转换为经纬度
    /// </summary>
    /// <param name="worldX">世界X坐标</param>
    /// <param name="worldY">世界Y坐标</param>
    /// <returns>经纬度（X为经度，Y为纬度）</returns>
    Vector2 WorldToGeo(float worldX, float worldY);

    /// <summary>
    /// 屏幕坐标转换为经纬度
    /// </summary>
    /// <param name="screenX">屏幕X坐标</param>
    /// <param name="screenY">屏幕Y坐标</param>
    /// <returns>经纬度（X为经度，Y为纬度）</returns>
    Vector2 ScreenToGeo(float screenX, float screenY);

    /// <summary>
    /// 经纬度转换为屏幕坐标
    /// </summary>
    /// <param name="longitude">经度</param>
    /// <param name="latitude">纬度</param>
    /// <returns>屏幕坐标</returns>
    Vector2 GeoToScreen(double longitude, double latitude);
}