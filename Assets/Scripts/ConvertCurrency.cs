using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using HtmlAgilityPack;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class ConvertCurrency : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TMP_Text errorText;
    [SerializeField] private TMP_Text resultText;
    
    [Header("Input")]
    [SerializeField] private TMP_InputField inputAmount;
    [SerializeField] private TMP_Dropdown fromDropDown;
    [SerializeField] private TMP_Dropdown toDropDown;
    
    private GameObject _errorObject;
    private WaitForSeconds _errorWait;
    
    private const float ErrorAlphaChangeRate = .02f;
    private const float ErrorAlphaChangeTime = .01f;

    private void Start()
    {
        _errorObject = errorText.gameObject;
        _errorWait = new WaitForSeconds(ErrorAlphaChangeTime);
    }

    public async void Convert()
    {
        _errorObject.SetActive(false);
        var amount = inputAmount.text;

        try
        {
            var amountVal = int.Parse(amount);
            if (amountVal < 0)
            {
                StartCoroutine(ShowError());
                return;
            }
			
			resultText.text = "Loading...";

            var from = fromDropDown.options[fromDropDown.value].text;
            var to = toDropDown.options[toDropDown.value].text;

            var newUrl = $"https://www.x-rates.com/calculator/?from={from}&to={to}&amount={amount}";
            
            using var www = UnityWebRequest.Get(newUrl);

            var operation = www.SendWebRequest();
            while (!operation.isDone) await Task.Yield();

            if (www.result == UnityWebRequest.Result.Success)
            {
                var stream = new MemoryStream ( www.downloadHandler.data );
                var doc = new HtmlDocument ();
                doc.Load ( stream );
                var result = doc.DocumentNode.SelectNodes("//span[@class='ccOutputRslt']");
                foreach (var test in result)
                {
                    resultText.text = test.InnerText;
                }
            }
            else
            {
                Debug.Log(www.error);
            }
        }
        catch (Exception)
        {
            StartCoroutine(ShowError());
        }
    }

    private IEnumerator ShowError()
    {
        _errorObject.SetActive(true);
        while (errorText.alpha > 0)
        {
            errorText.alpha -= ErrorAlphaChangeRate;
            yield return _errorWait;
        }
        _errorObject.SetActive(false);
        errorText.alpha = 1f;
    }
}
