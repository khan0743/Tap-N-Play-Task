using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Unlock : MonoBehaviour
{
    [SerializeField] private GameObject unlockProgressObj;
       [SerializeField] private GameObject newItem;
       [SerializeField] private Image progressBar;
       [SerializeField] private TextMeshProUGUI moneyAmount;
       [SerializeField] private int itemPrice,itemRemainingPrice;
       [SerializeField] private float ProgressValue;
       public GameObject buildNavMesh;
       public bool upgrade;
    
    void Start()
    {
        moneyAmount.text = itemPrice.ToString("C0");
        itemRemainingPrice = itemPrice;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && PlayerPrefs.GetInt("money") > 0)
        {
            ProgressValue = Mathf.Abs(1f - CalculateMoney() / itemPrice) ;

            if (PlayerPrefs.GetInt("money") >= itemRemainingPrice)
            {
                PlayerPrefs.SetInt("money", PlayerPrefs.GetInt("money") - itemRemainingPrice);

                itemRemainingPrice = 0;
            }
            else
            {
                itemRemainingPrice -= PlayerPrefs.GetInt("money");
                PlayerPrefs.SetInt("money", 0);
            }

            progressBar.fillAmount = ProgressValue;

            PlayerManager.PlayerManagerInstance.MoneyCounter.text = PlayerPrefs.GetInt("money").ToString("C0");
            moneyAmount.text = itemRemainingPrice.ToString("C0");

            if (itemRemainingPrice <= 0)
            {
                newItem.SetActive(true);

                if(upgrade){
                    newItem.GetComponent<Generator>().maxStuffs *= 2;
                    newItem.GetComponent<Generator>().RestartGeneration();
                }
                // newItem.transform.DOScale(new Vector3(0.4f, 0.4f, 0.6f), 1f).SetEase(Ease.OutElastic);

                newItem.transform.DOScale(1.3f, 1f).SetEase(Ease.OutElastic);
                newItem.transform.DOScale(1.2f, 1.2f).SetDelay(1.1f).SetEase(Ease.OutElastic);
                
                unlockProgressObj.SetActive(false);
            }

        }
    }

    private float CalculateMoney()
    {
        return itemRemainingPrice - PlayerPrefs.GetInt("money");
    }
}
