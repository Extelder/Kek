using System;
using UnityEngine;

public class HungerColorController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Quota _qouta;
    [SerializeField] private GameObject[] _object;

    [Header("Colors")]
    [SerializeField] private Color _fullColor;
    [SerializeField] private Color _fullColor70;
    [SerializeField] private Color _fullColor50;
    [SerializeField] private Color _fullColor30;
    [SerializeField] private Color _fullColor5;

    [Header("Settings")]
    [SerializeField] private float _lerpSpeed = 5f;

    private Renderer[] _renderers;

    private void OnEnable()
    {
        _qouta.ValueChanged += SetNewColor;
    }

    private void OnDisable()
    {
        _qouta.ValueChanged -= SetNewColor;
    }

    
    //Ӟ͈̯͉̞͕͈̗̳̥̗̖̟͍̣́̒̀̊́̇̚Д͈̯͖͉͓̤͇̋̀͌͛̃̎̆͗̋̋̀Е̞̣̪̱̱̬͉̙͖̩͕͓̞̾̆̂͑̇̃̔̐̐͋͛С͇̘̤͍̠̲́̓́̾̏̆̆̀̈́̆͑́̈́́̅Ь̗̣̮̣̯̪͇͖͒̌̃̂̈́̊̾̅ Б̠̥͙̬̙͕̖̖̙͇̥̯̰͑̾̀̔̒͋̀̉̿̇̇̌̽Ы̟̲̤̣̣̠̔̊͌̓̅̔̈̀́́Л̯̯͈̞͇̲͙̱̲̬̖̥̝͓̩͔͌̏͑̿̃̂͗̒̌̉̀̾̒̀ К̗̟͍̩͚̦̬͚̙̖̘̥̗̯̪̅̾̓̽́̄̒А͓͇͍͓͚̜̗̰̘̝̦̂͆̊̋͂͒̊͊̽̐̃Р̜̭̲̰̣͉̜̭͕̭̝͗̏̍̑͌̌̀̃̽И̙̱͉̖̍̀͊̈̆̔̈́̑̔̌̽̽̄Т̱̫̳̩͈̗͍̌̎̍͑̏̇̋̏̈ͅͅЕ̗͚̥̩̲͈̠̳̤͖̘͉̘͎̉̔͊̏̐̓͌͂̑̚̚ͅЙ͓̤͎͕̤̭͙̔̏͗̈́͗̍̓̿̐̋̄̈̒̆̽.̦̟̣̱̥̱̝̬̗̮̜̠͈̮͕͚̀͊̀̌̏̽͒̃͋́.̮̫͉̥̥͙̥̖̓͊͌̈́̐̃͆̉̊̉.͍̘͈̬͎̏̆͊͗.̙̯̲̗̩̗̲̙͇̥̪̗̗͓̟̇̂͐̓̀͐́̄̈̾̐͆̌̄͛̚2̠̟͎͚͔̦͕̗͔͆̒͐̿͒̔̍̓3̞̣͖͖̟̭̪͖̯̟̳͇̥̖͌̈͊͂̎̀́̃̒̍̔̌̄̋̇̚ͅ
    
    
    private void SetNewColor(float obj)
    {
        if (_qouta.TryBuy(0))
        {
            SetColor(_fullColor);
            return;
        }
        if (!_qouta.TryBuy(70))
        {
            SetColor(_fullColor70);
        }
        if (!_qouta.TryBuy(50))
        {
            SetColor(_fullColor50);
        }
        if (!_qouta.TryBuy(30))
        {
            SetColor(_fullColor30);
        }
        if (!_qouta.TryBuy(5))
        {
            SetColor(_fullColor5);
        }
    }

    private void SetColor(Color color)
    {
        for (int i = 0; i < _object.Length; i++)
        {
            _object[i].GetComponent<Renderer>().material.color = color;
        }
    }
}
