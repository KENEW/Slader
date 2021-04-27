using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningPopup : MonoBehaviour
{
    public static WarningPopup Instance;
    public GameObject popup;
    public Text warningText;

    private Dictionary<string, string> warningMessage;
    private bool isInit = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (MyData.Instance.IsDisConnect)
        {
            MyData.Instance.IsDisConnect = false;
            OpenPopup("RoomDisconnect");
        }
    }
    private void Init()
    {
        warningMessage = new Dictionary<string, string>();
        warningMessage.Add("Empty", "");
        warningMessage.Add("Disconnect", "인터넷 연결이 불안정합니다. 다시 시도해주세요");
        warningMessage.Add("RoomDisconnect", "플레이어 한 명이 연결이 끊겨 로비로 돌아옵니다.");
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            OpenPopup("RoomDisconnect");
        }
    }
    public void OpenPopup(string message)
    {
        if (!isInit)
        {
            Init();
            isInit = true;
        }
        popup.SetActive(true);
        warningText.text = warningMessage[message];
        SoundManager.Instance.PlaySFX("ButtonClick");
    }
    public void ClosePopup()
    {
        popup.SetActive(false);
        warningText.text = warningMessage["Empty"];
        SoundManager.Instance.PlaySFX("ButtonClick");
    }
}