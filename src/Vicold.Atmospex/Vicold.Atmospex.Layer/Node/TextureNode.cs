using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Vicold.Atmospex.Layer.Node;
public abstract  class TextureNode : ILayerNode
{
    //private ImageTexture _texture;

    public string ID
    {
        get; set;
    } = string.Empty;

    public float StartX
    {
        get; set;
    }

    public float StartY
    {
        get; set;
    }

    public float WorldWidth
    {
        get; set;
    }

    public float WorldHeight
    {
        get; set;
    }

    public abstract bool Visible
    {
        get;set;
    }

    //public override void _Ready()
    //{
    //    base._Ready();
    //    _texture = InitTexture();
    //}

    //public void ResetImage(Image image)
    //{
    //    TImage?.Dispose();
    //    TImage = image;
    //    _texture?.Dispose();
    //    _texture = InitTexture();
    //    Update();
    //}

    public abstract void SetLevel(int zIndex);


    //public override void _Draw()
    //{
    //    base._Draw();
    //    if (_texture != null)
    //    {
    //        DrawTexture(_texture, new Vector2(StartX, StartY));
    //    }
    //}

    public void ResetScale(float scale)
    {
    }

    //protected override void Dispose(bool disposing)
    //{
    //    base.Dispose(disposing);
    //    if (disposing)
    //    {
    //        TImage?.Dispose();
    //        _texture?.Dispose();
    //        TImage = null;
    //        _texture = null;
    //    }
    //}

    //private ImageTexture InitTexture()
    //{
    //    var texture = new ImageTexture();
    //    texture.Storage = ImageTexture.StorageEnum.CompressLossy;
    //    texture.CreateFromImage(TImage);
    //    texture.SetSizeOverride(new Vector2(WorldWidth, WorldHeight));
    //    texture.Flags = 0;
    //    return texture;
    //}

    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
