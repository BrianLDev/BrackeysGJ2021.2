using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public GameObject setting;

    public GameObject credit;

    private PlayerInput _input;

    public GameObject transition;

    void Awake()
    {
        _input = GetComponent<PlayerInput>();
    }

    void Update()
    {
        if(_input.actions["Back"].WasPerformedThisFrame() && credit.activeSelf)
        {
            credit.SetActive(false);
        }
    }

    public void StartGame()
    {
        StartCoroutine(LoadScene(1));
    }

    IEnumerator LoadScene(int buildindex)
    {
        transition.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
    }

    public void Credit()
    {
        credit.SetActive(true);
    }

    public void Setting()
    {
        setting.SetActive(true);
    }

    public void CloseSetting()
    {
        setting.SetActive(false);
    }

    public void SetSFXVolume(float volume)
    {
        //implemetion need for set the SFX voulme to parameter voulme 
    }

    public void SetMusicVolume(float volume)
    {
        //implemetion need for set the Music voulme to parameter voulme 
    }

    public void Quit()
    {
        Application.Quit();
    }
}
