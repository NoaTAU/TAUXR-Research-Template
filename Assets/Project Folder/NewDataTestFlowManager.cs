using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;


public class NewDataTestFlowManager : MonoBehaviour
{
    public TextMeshProUGUI timeSinceStartupText;
    private void Start()
    {
        RunFlow().Forget();
    }

    private async UniTask RunFlow()
    {

    }

    private void Update()
    {
        timeSinceStartupText.text = $"{Time.realtimeSinceStartup}";
    }
}
