using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class ReadCurrencies : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown fromDropDown;
    [SerializeField] private TMP_Dropdown toDropDown;

    private string _path;
    private string _jsonString;

    private const string DefaultFrom = "EUR";
    private const string DefaultTo = "TRY";
    
    private void Start ()
    {
        fromDropDown.AddOptions(new List<string> {"LOADING"});    
        toDropDown.AddOptions(new List<string> {"LOADING"});
        
        GET_Currencies();
    }

    #region Online

    private async void GET_Currencies()
    {
        const string url = "https://openexchangerates.org/api/currencies.json";

        using var www = UnityWebRequest.Get(url);
        www.SetRequestHeader("Content-Type", "application/json");

        var operation = www.SendWebRequest();
        while (!operation.isDone) await Task.Yield();

        if (www.result == UnityWebRequest.Result.Success)
        {
            var resultText = www.downloadHandler.text;
            var currencyDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultText);
            
            fromDropDown.ClearOptions();    
            toDropDown.ClearOptions();

            if (currencyDic == null) return;
            foreach (var currency in currencyDic.Keys)
            {
                fromDropDown.AddOptions(new List<string> {currency});    
                toDropDown.AddOptions(new List<string> {currency});    
            }
            
            
            var indexFrom = fromDropDown.options.FindIndex((i) => i.text.Equals(DefaultFrom));
            var indexTo = fromDropDown.options.FindIndex((i) => i.text.Equals(DefaultTo));

            Debug.Log(indexFrom);
        
            fromDropDown.value = indexFrom;
            toDropDown.value = indexTo;
        }
        else
        {
            Debug.LogWarning(www.error);
        }
    }

    #endregion

    #region Offline

    /*private void ReadFromFile()
    {
        _path = Application.dataPath + "/Resources/currencies.json";
        _jsonString = File.ReadAllText (_path); 

        var currencyDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(_jsonString);

        if (currencyDic == null) return;
        foreach (var currency in currencyDic.Keys)
        {
            fromDropDown.AddOptions(new List<string> {currency});    
            toDropDown.AddOptions(new List<string> {currency});    
        }
    }*/

    #endregion
}
