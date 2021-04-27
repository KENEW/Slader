public class RedSlimeMonster : Monster
{
	protected override void Awake()
	{
		base.Awake();

		maxHp = 850;
		moveSpeed = 0.3f;
		dropCoinValue = 170;

		SetCoopDifficulty();
	}
}
