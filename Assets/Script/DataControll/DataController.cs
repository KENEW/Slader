using UnityEngine;
using System.IO;
using LitJson;

public class DataController : SceneSingleTon<DataController>
{
	public string filePath = "/Resources/Data/GameData/";

	public void LoadGameData()
	{
		string filePath_gameData = Application.persistentDataPath + "GameData.json";
		string filePath_settingData = Application.persistentDataPath + "SettingData.json";

		if(File.Exists(filePath_gameData))
		{
			string FromJsonData = File.ReadAllText(filePath_gameData);
			MyData.Instance.charData = JsonUtility.FromJson<CharData>(FromJsonData);
		}
		else
		{
			MyData.Instance.charData = new CharData();
		}
		
		if (File.Exists(filePath_settingData))
		{
			string FromJsonData = File.ReadAllText(filePath_settingData);
			MyData.Instance.settingData = JsonUtility.FromJson<SettingData>(FromJsonData);
		}
		else
		{
			MyData.Instance.settingData = new SettingData();
		}
	}
	public void SaveGameData()
	{
		JsonData jsonData_gameData = JsonMapper.ToJson(MyData.Instance.charData);
		JsonData jsonData_settingData = JsonMapper.ToJson(MyData.Instance.settingData);

		File.WriteAllText(Application.persistentDataPath + "GameData.json", jsonData_gameData.ToString());
		File.WriteAllText(Application.persistentDataPath + "SettingData.json", jsonData_settingData.ToString());

		DebugOptimum.Log("저장 완료");
	}
}
