/* Created by and for usage of FF Studios (2021). */

namespace FFStudio
{
    public enum SpriteSetMethod
    {
        Equalize,
        PreserveAspect
    }

    public enum JoyStickMethod
    {
        Vector2,
        Vector3_Y,
        Vector3_Z
    }

	public enum AnimationParameterType
	{
		Trigger,
		Bool,
		Int,
		Float
	}

    public enum VibrateType
    {
        Peek,
        Pop,
        Nope,
        Big
    }

	public enum MovementMode { Local, World }
    
    public enum RotationAxisVector
    {
        Right, Forward, Up
    }
}