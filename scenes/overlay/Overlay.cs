using Godot;

public partial class Overlay : Node2D
{
	private long _startTime;
	private long _elapsedMilliseconds;
	private bool _timerRunning;

	private RichTextLabel _timer;
	private Label _countdown;
	private Label _speed;

	public override void _Ready()
	{
		_timer = GetNode<RichTextLabel>("CanvasLayer/Timer");
		_countdown = GetNode<Label>("CanvasLayer/Countdown");
		_countdown.Hide();
		_speed = GetNode<Label>("CanvasLayer/Speed");
	}

	public void SetSpeed(float speed)
	{
		_speed.Text = speed.ToString("F2") + " u/s";
	}

	public void StartTimer()
	{
		_startTime = (long)Time.GetTicksMsec();
		_timerRunning = true;
	}

	// Stops the timer and returns the final time in milliseconds.
	public long StopTimer()
	{
		_elapsedMilliseconds = (long)Time.GetTicksMsec() - _startTime;
		_timerRunning = false;
		return _elapsedMilliseconds;
	}

	public void ShowCountdown(int number)
	{
		_countdown.Text = number.ToString();
		_countdown.Show();
	}

	public void HideCountdown()
	{
		_countdown.Hide();
	}

	public static string FormatMilliseconds(long milliseconds)
	{
		long minutes = milliseconds / 60000;
		long seconds = milliseconds % 60000 / 1000;
		long remainingMilliseconds = milliseconds % 1000;
		return $"{minutes:D2}:{seconds:D2}:{remainingMilliseconds:D3}";
	}

	public override void _Process(double delta)
	{
		if (_timerRunning)
		{
			_elapsedMilliseconds = (long)Time.GetTicksMsec() - _startTime;
		}

		_timer.Text = FormatMilliseconds(_elapsedMilliseconds);
	}
}
