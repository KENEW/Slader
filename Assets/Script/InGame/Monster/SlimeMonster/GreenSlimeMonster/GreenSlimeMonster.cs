public class GreenSlimeMonster : Monster
{
	protected override void Awake()
	{
		base.Awake();

		maxHp = 250;
		moveSpeed = 1.0f;
		dropCoinValue = 100;

		SetCoopDifficulty();
	}
}
