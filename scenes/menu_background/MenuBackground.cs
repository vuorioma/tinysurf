using Godot;

public partial class MenuBackground : CanvasLayer
{
	private const string VideoPath = "res://resources/videos/bg.ogv";
	private static readonly Vector2 VideoSize = new(1920.0f, 1080.0f);

	private VideoStreamPlayer _video;

	public override void _Ready()
	{
		_video = GetNode<VideoStreamPlayer>("Video");
		if (!ResourceLoader.Exists(VideoPath))
		{
			GD.PushWarning($"Background video not found at '{VideoPath}'.");
			return;
		}

		_video.Stream = GD.Load<VideoStream>(VideoPath);
		_video.Finished += _video.Play;
		_video.Play();

		GetViewport().SizeChanged += FitVideo;
		FitVideo();
	}

	public override void _ExitTree()
	{
		GetViewport().SizeChanged -= FitVideo;
	}

	// Scale the video to cover the whole screen, cropping the overflow instead of distorting it.
	private void FitVideo()
	{
		Vector2 screenSize = GetViewport().GetVisibleRect().Size;
		float scale = Mathf.Max(screenSize.X / VideoSize.X, screenSize.Y / VideoSize.Y);
		Vector2 size = VideoSize * scale;
		_video.Size = size;
		_video.Position = (screenSize - size) / 2.0f;
	}
}
