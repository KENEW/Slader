using System.Collections.Generic;

/// <summary>
/// 게임 중 사용할 데이터를 처리합니다.
/// </summary>
public class MyData : SceneSingleTon<MyData>
{
	public UpgradeCost[] upgradeCost;
	public WeaponInfo[] weaponInfo;
	public List<Dictionary<string, object>> waveMonster;
	public SettingData settingData{ get; set; }
	public CharData charData;
	public bool IsAuthInit { get; set; }

	//룸에 있을 동안 플레이어 한명이 서버 접속에 끊기면 방에 있는 플레이어들이 자동으로 로비에 나오게 되냐 체크
	public bool IsAutoServerQuit = false;
	//자동으로 로비에 나왔을경우 로비씬에서 팝업을 뛰우냐 체크
	public bool IsDisConnect = false;
	public string authCode = "";
	public string email = "";

	private void Start()
	{
		upgradeCost = DataLoad.Instance.CostLoad();
		weaponInfo = DataLoad.Instance.WeaponLoad();
		DataLoad.Instance.WaveMonsterLoad();
		
		settingData = new SettingData();
		charData = new CharData();
	}
	public int MyCoin 
	{ 
		get
		{
			return charData.coin;
		}
		set
		{
			charData.coin += value;
		}
	}
	public bool UseMyCoin(int coin)
	{
		if (charData.coin - coin < 0)
		{
			return false;
		}
		else
		{
			charData.coin -= coin;
			return true;
		}
	}
}
