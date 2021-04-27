using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct UpgradeCost
{
	public int manaSoulCost;
	public int attackCost;
	public int coinCost;
}
public struct WaveMonster
{
	public Dictionary<string, int> waveMonsterInfo;
}
public struct WeaponInfo
{
	public int index;

	public string name;
	public string type;
	public string info;

	public int offensiveAttack;
	public float attackSpeed;
}
public class DataLoad : MonoSingleton<DataLoad>
{
	public float FloatParse(object obj)
	{
		return float.Parse(obj.ToString());
	}
	public UpgradeCost[] CostLoad()
	{
		List<Dictionary<string, object>> data = CSVReader.Read("Data/UpgradeCost");
		List<UpgradeCost> upgradeCost = new List<UpgradeCost>();

		for(int i = 0; i < data.Count; i++)
		{
			UpgradeCost t_upgradeCost = new UpgradeCost();

			t_upgradeCost.attackCost = (int)data[i]["Offensive_Attack"];
			t_upgradeCost.coinCost = (int)data[i]["Coin"];
			t_upgradeCost.manaSoulCost = (int)data[i]["Mana_Soul"];

			upgradeCost.Add(t_upgradeCost);
		}

		return upgradeCost.ToArray();
	}
	public void WaveMonsterLoad()
	{
		MyData.Instance.waveMonster = CSVReader.Read("Data/WaveMonsterData");
	}
	public WeaponInfo[] WeaponLoad()
	{
		List<Dictionary<string, object>> data = CSVReader.Read("Data/WeaponData");
		List<WeaponInfo> upgradeCost = new List<WeaponInfo>();

		for (int i = 0; i < data.Count; i++)
		{
			WeaponInfo t_waveMonster = new WeaponInfo();

			t_waveMonster.index = ((int)data[i]["ID"]);
			t_waveMonster.name = ((string)data[i]["Name"]);
			t_waveMonster.type = ((string)data[i]["Type"]);
			t_waveMonster.info = ((string)data[i]["Info"]);
			t_waveMonster.offensiveAttack = ((int)data[i]["Offensive_Attack"]);
			t_waveMonster.attackSpeed = FloatParse(data[i]["Attack_Speed"]);

			upgradeCost.Add(t_waveMonster);
		}

		return upgradeCost.ToArray();
	}
}