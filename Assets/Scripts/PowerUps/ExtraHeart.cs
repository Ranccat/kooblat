public class ExtraHeart : PowerUp
{
	protected override void ApplyPowerUp()
	{
		Game.Player.AddExtraHeart();
	}
}
