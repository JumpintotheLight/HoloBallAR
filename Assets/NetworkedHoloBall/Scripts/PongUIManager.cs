﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PongUIManager : NetworkBehaviour
{
    //public NetworkManager3DPong networkManager;

    //fields for UI elements and text values that need to be updated with changes
    public GameObject uiManagerContainer;
    public Text victoryPointText;
    public Slider victorySlider;
    public Dropdown advantageDropdown;
    public Toggle continuousPlayToggle;
    public Dropdown bActiveDropdown;
    public Text bAngleText;
    public Slider bAngleSlider;
    public Text bWidthText;
    public Slider bWidthSlider;
    public Text bHeightText;
    public Slider bHeightSlider;

    private bool controlsInteractalbe = true;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
            if (Mirror3DPongGameDriver.gameDriver.GameState == PongGameState.Setup && !controlsInteractalbe)
            {
                SetControlsInteractable(true);
            }
    }

    public void SetVictoryPoints()
    {
        /*Debug.Log("Set Victory Threshold called");
        Mirror3DPongGameDriver.gameDriver.WinPoint = int.Parse(victoryPointText.text);
        victoryPointText.text = Mirror3DPongGameDriver.gameDriver.WinPoint.ToString();
        */
        Mirror3DPongGameDriver.gameDriver.WinPoint = (int)victorySlider.value;
        victoryPointText.text = Mirror3DPongGameDriver.gameDriver.WinPoint.ToString();
    }

    public void SetAdvantageNeeded()
    {
        Mirror3DPongGameDriver.gameDriver.AdvantageNeeded = advantageDropdown.value;
    }

    public void SetBoundariesActive()
    {
        Mirror3DPongGameDriver.gameDriver.BoundariesActive = bActiveDropdown.value == 0 ? true : false;
    }

    public void SetBoundaryWidth()
    {
        Mirror3DPongGameDriver.gameDriver.BoundaryWidth = bWidthSlider.value;
        bWidthText.text = Mirror3DPongGameDriver.gameDriver.BoundaryWidth.ToString();
    }

    public void SetBoundaryHeight()
    {
        Mirror3DPongGameDriver.gameDriver.BoundaryHeight = bHeightSlider.value;
        bHeightText.text = Mirror3DPongGameDriver.gameDriver.BoundaryHeight.ToString();
    }

    public void SetBoundaryAngle()
    {
        Mirror3DPongGameDriver.gameDriver.BoundaryAngle = bAngleSlider.value;
        bAngleText.text = Mirror3DPongGameDriver.gameDriver.BoundaryAngle.ToString();
    }

    public void StartGame()
    {
        if(Mirror3DPongGameDriver.gameDriver.GameState == PongGameState.Setup && Mirror3DPongGameDriver.gameDriver.PlayerCount == 2)
        {
            Mirror3DPongGameDriver.gameDriver.ContinuousPlay = continuousPlayToggle.isOn;
            Mirror3DPongGameDriver.gameDriver.StartGame();
            SetControlsInteractable(false);
        }
    }

    public void PauseGame()
    {
        if(Mirror3DPongGameDriver.gameDriver.GameState == PongGameState.Paused)
        {
            Mirror3DPongGameDriver.gameDriver.UnPauseGame();
        }
        else
        {
            Mirror3DPongGameDriver.gameDriver.PauseGame();
        }
    }

    public void ResetGame()
    {
        if(Mirror3DPongGameDriver.gameDriver.GameState != PongGameState.Setup)
        {
            Mirror3DPongGameDriver.gameDriver.ContinuousPlay = false;
            Mirror3DPongGameDriver.gameDriver.Reset();
            SetControlsInteractable(true);
        }
    }

    public void HardResetGame()
    {
        Mirror3DPongGameDriver.gameDriver.HardReset();
        SetControlsInteractable(true);
    }

    public void StopServer()
    {
        //networkManager.StopServer();
    }

    public void HideManager()
    {
        uiManagerContainer.SetActive(!uiManagerContainer.activeSelf);
    }

    private void SetControlsInteractable(bool active)
    {
        controlsInteractalbe = active;
        victorySlider.interactable = active;
        continuousPlayToggle.interactable = active;
        advantageDropdown.interactable = active;
        bActiveDropdown.interactable = active;
        bAngleSlider.interactable = active;
        bWidthSlider.interactable = active;
        bHeightSlider.interactable = active;
}
}
