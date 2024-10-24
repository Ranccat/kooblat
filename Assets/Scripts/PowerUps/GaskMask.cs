public class GaskMask : PowerUp
{
	private int _maskHealth = 6;

	protected override void ApplyPowerUp()
	{
		Game.Player.EquipGasMask(_maskHealth);
	}
}
