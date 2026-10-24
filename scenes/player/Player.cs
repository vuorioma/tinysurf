using Godot;

public partial class Player : CharacterBody3D
{
	[Export] public float LookSensitivity { get; set; } = 0.001f;
	[Export] public float JumpVelocity { get; set; } = 6.0f;
	[Export] public bool AutoBhop { get; set; } = true;

	// While frozen the player can neither move nor look around.
	public bool Frozen { get; set; }

	[Export] public float WalkSpeed { get; set; } = 7.0f;
	[Export] public float SprintSpeed { get; set; } = 8.5f;
	[Export] public float GroundAccel { get; set; } = 14.0f;
	[Export] public float GroundDecel { get; set; } = 10.0f;
	[Export] public float GroundFriction { get; set; } = 6.0f;

	[Export] public float AirCap { get; set; } = 0.85f;
	[Export] public float AirAccel { get; set; } = 800.0f;
	[Export] public float AirMoveSpeed { get; set; } = 500.0f;

	private Vector3 _wishDirection = Vector3.Zero;
	private Camera3D _camera;

	public override void _Ready()
	{
		_camera = GetNode<Camera3D>("Camera3D");
	}

	private void ClipVelocity(Vector3 normal)
	{
		for (int i = 0; i < 2; i++)
		{
			float backoff = Velocity.Dot(normal);
			if (backoff >= 0.0f)
			{
				return;
			}

			Velocity -= normal * backoff;
		}
	}

	private static bool IsSurfaceTooSteep(Vector3 normal)
	{
		return normal.Dot(Vector3.Up) < 0.70710678118f;
	}

	private void HandleAirPhysics(float delta)
	{
		float gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity").AsDouble();
		Velocity = new Vector3(Velocity.X, Velocity.Y - gravity * delta, Velocity.Z);

		float currentSpeedInWishDirection = Velocity.Dot(_wishDirection);
		float cappedSpeed = (_wishDirection * AirMoveSpeed).Length();
		cappedSpeed = Mathf.Min(cappedSpeed, AirCap);
		float addSpeedTillCap = cappedSpeed - currentSpeedInWishDirection;
		if (addSpeedTillCap > 0.0f)
		{
			float accelerationSpeed = AirAccel * AirMoveSpeed * delta;
			accelerationSpeed = Mathf.Min(accelerationSpeed, addSpeedTillCap);
			Velocity += accelerationSpeed * _wishDirection;
		}

		if (IsOnWall())
		{
			Vector3 wallNormal = GetWallNormal();
			MotionMode = IsSurfaceTooSteep(wallNormal)
				? MotionModeEnum.Floating
				: MotionModeEnum.Grounded;
			ClipVelocity(wallNormal);
		}
	}

	private void HandleGroundPhysics(float delta)
	{
		float moveSpeed = GetMoveSpeed();
		float currentSpeedInWishDirection = Velocity.Dot(_wishDirection);
		float addSpeedTillCap = moveSpeed - currentSpeedInWishDirection;
		if (addSpeedTillCap > 0.0f)
		{
			float accelerationSpeed = GroundAccel * delta * moveSpeed;
			accelerationSpeed = Mathf.Min(accelerationSpeed, addSpeedTillCap);
			Velocity += accelerationSpeed * _wishDirection;
		}

		float speed = Velocity.Length();
		float control = Mathf.Max(speed, GroundDecel);
		float drop = control * GroundFriction * delta;
		float newSpeed = Mathf.Max(speed - drop, 0.0f);
		if (speed > 0.0f)
		{
			newSpeed /= speed;
		}

		Velocity *= newSpeed;
	}

	private float GetMoveSpeed()
	{
		return Input.IsActionPressed("sprint") ? SprintSpeed : WalkSpeed;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton)
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}
		else if (@event.IsActionPressed("ui_cancel"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}

		if (!Frozen && Input.MouseMode == Input.MouseModeEnum.Captured && @event is InputEventMouseMotion mouseMotion)
		{
			RotateY(-mouseMotion.Relative.X * LookSensitivity);
			_camera.RotateX(-mouseMotion.Relative.Y * LookSensitivity);
			Vector3 cameraRotation = _camera.Rotation;
			cameraRotation.X = Mathf.Clamp(
				cameraRotation.X,
				Mathf.DegToRad(-90.0f),
				Mathf.DegToRad(90.0f));
			_camera.Rotation = cameraRotation;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Frozen)
		{
			Velocity = Vector3.Zero;
			return;
		}

		float physicsDelta = (float)delta;
		Vector2 inputDirection = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down").Normalized();
		_wishDirection = GlobalTransform.Basis * new Vector3(inputDirection.X, 0.0f, inputDirection.Y);

		if (IsOnFloor())
		{
			if (Input.IsActionJustPressed("jump") || (AutoBhop && Input.IsActionPressed("jump")))
			{
				Velocity = new Vector3(Velocity.X, JumpVelocity, Velocity.Z);
			}

			HandleGroundPhysics(physicsDelta);
		}
		else
		{
			HandleAirPhysics(physicsDelta);
		}

		MoveAndSlide();
	}
}
