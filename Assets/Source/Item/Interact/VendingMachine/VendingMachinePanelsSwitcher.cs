using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachinePanelsSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject[] _gameObjects;
    private int _currentPage = 1;

    public void NextPage()
    {
        DisablePreviousPanel();
        _currentPage++;
        if (_currentPage >= _gameObjects.Length)
            _currentPage = _gameObjects.Length - 1;
        SwitchPanels();
    }

    public void PreviousPage()
    {
        DisablePreviousPanel();
        _currentPage--;
        if (_currentPage < 0)
            _currentPage = 0;
        SwitchPanels();
    }

    private void DisablePreviousPanel()
    {
        _gameObjects[_currentPage].SetActive(!_gameObjects[_currentPage].activeSelf);
    }
    
    private void SwitchPanels()
    {
        _gameObjects[_currentPage].SetActive(!_gameObjects[_currentPage].activeSelf);
    }
}
