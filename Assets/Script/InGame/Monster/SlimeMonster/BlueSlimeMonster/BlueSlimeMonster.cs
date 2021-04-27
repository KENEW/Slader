public class BlueSlimeMonster : Monster
{
	protected override void Awake()
	{
		base.Awake();

		maxHp = 450;
		moveSpeed = 0.65f;
		dropCoinValue = 0;

		SetCoopDifficulty();
	}
}
