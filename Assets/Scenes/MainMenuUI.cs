using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public GameObject setting;

    public GameObject credit;

    private PlayerInput _input;

    void Awake()
    {
        _input = GetComponent<PlayerInput>();
    }

    void Update()
    {
        if(_input.actions["Back"].WasPerformedThisFrame())
        {
            if(setting.activeSelf)
            {
                setting.SetActive(false);
            }

            if(credit.activeSelf)
            {
                credit.SetActive(false);
            }
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Credit()
    {
        credit.SetActive(true);
    }

    public void Setting()
    {
        setting.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
