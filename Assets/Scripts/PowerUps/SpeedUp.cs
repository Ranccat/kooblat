public class SpeedUp : PowerUp
{
	private float _speedUpModifier = 2f;

	protected override void ApplyPowerUp()
	{
		Game.Player.SpeedUp(_speedUpModifier);
	}
}
