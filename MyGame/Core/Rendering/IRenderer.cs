namespace MyGame.Core.Rendering;

public interface IRenderer<in T>
{
    void Draw(T model, FrameTime frameTime);
}
