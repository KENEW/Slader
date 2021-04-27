using System;

[Serializable]
public class CharData
{
	public int attackLevel;
	public int manaLevel;
	public int coinLevel;

	public int highScore;
	public int gameCount;
	public int coin;
	public string name;
	public int highWave;

	public CharData()
	{
		this.attackLevel = 1;
		this.manaLevel = 1;
		this.coinLevel = 1;
		this.highScore = 0;
		this.gameCount = 0;
		this.coin = 0;
		this.highWave = 0;
		this.name = "Player";
	}
	public CharData(int attackLevel, int manaLevel, int coinLevel
		, int highScore, int gameCount, int coin, string name, int highWave)
	{
		this.attackLevel = attackLevel;
		this.manaLevel = manaLevel;
		this.coinLevel = coinLevel;
		this.highScore = highScore;
		this.gameCount = gameCount;
		this.coin = coin;
		this.name = name;
		this.highWave = highWave;
	}
}
[Serializable]
public class SettingData
{
	public bool isServerLoad = false;

	public int soundValueBGM = 0;
	public int soundValueSFX = 0;
}