using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Counter : MonoBehaviour
{
    [SerializeField] private Transform MoneyPlace, CustomerPlace;
    [SerializeField] private GameObject Money;
    public float YAxis;
    private IEnumerator makeMoneyIE;
    public int noOfStuffs;
    public List<GameObject> Customers = new List<GameObject>();
    public bool isOccupied, secondOccupied, thirdOccupied;
    public GameObject currentCustomer, secondCustomer, thirdCustomer;
    public int MoneyPlaceIndex = 0;


    private void Start()
    {
        GameObject[] customerObjects = GameObject.FindGameObjectsWithTag("customer");
        foreach (GameObject customer in customerObjects)
        {
            Customers.Add(customer);
        }
        InvokeRepeating("callCustomer",5f,5f);
    }
    void callCustomer(){
        if(!isOccupied){
            currentCustomer = FindClosestCustomer(CustomerPlace.GetChild(0).position);
            isOccupied = true;
            currentCustomer.GetComponent<Customer>().target = CustomerPlace.GetChild(0);
            currentCustomer.GetComponent<Customer>().buying = true;
            currentCustomer.GetComponent<Customer>().OnBuy += HandleBuyEvent;
            currentCustomer.GetComponent<Customer>().SetDestination();
        }
        else if(!secondOccupied){
            secondCustomer = FindClosestCustomer(CustomerPlace.GetChild(1).position);
            secondOccupied = true;
            secondCustomer.GetComponent<Customer>().target = CustomerPlace.GetChild(1);
            secondCustomer.GetComponent<Customer>().buying = true;
            secondCustomer.GetComponent<Customer>().SetDestination();
        }
        else if(!thirdOccupied){
            thirdCustomer = FindClosestCustomer(CustomerPlace.GetChild(1).position);
            thirdOccupied = true;
            thirdCustomer.GetComponent<Customer>().target = CustomerPlace.GetChild(2);
            thirdCustomer.GetComponent<Customer>().buying = true;
            thirdCustomer.GetComponent<Customer>().SetDestination();
        }
    }

    public GameObject FindClosestCustomer(Vector3 point)
    {
        GameObject closestCustomer = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject customer in Customers)
        {
            float distance = Vector3.Distance(customer.transform.position, point);
            if (distance < closestDistance && !customer.GetComponent<Customer>().buying)
            {
                closestCustomer = customer;
                closestDistance = distance;
            }
        }

        return closestCustomer;
    }

    public void HandleBuyEvent(){
        if(currentCustomer != null)
            StartCoroutine(SellStuffs());   
    }

    IEnumerator SellStuffs(){
        yield return new WaitForSeconds(2f);

        var Customer = currentCustomer.GetComponent<Customer>();
        int startIndex = Mathf.Max(transform.childCount - Customer.buyAmount, 0);
        if(noOfStuffs >= Customer.buyAmount){
            // Destroy(transform.GetChild(transform.childCount - Customer.buyAmount).gameObject,1f);

            for (int i = transform.childCount - 1; i >= startIndex; i--){
                Destroy(transform.GetChild(i).gameObject);
            }
            noOfStuffs -= Customer.buyAmount;

            yield return new WaitForSeconds(1f);
            var counter = 0;

            while (counter < Customer.buyAmount)
            {
                GameObject NewMoney = Instantiate(Money, new Vector3(MoneyPlace.GetChild(MoneyPlaceIndex).position.x,
                        YAxis, MoneyPlace.GetChild(MoneyPlaceIndex).position.z),
                    MoneyPlace.GetChild(MoneyPlaceIndex).rotation);

                counter++;

                NewMoney.transform.DOScale(new Vector3(0.4f, 0.4f, 0.6f), 1f).SetEase(Ease.OutElastic);

                if (MoneyPlaceIndex < MoneyPlace.childCount - 1)
                {
                    MoneyPlaceIndex++;
                }
                else
                {
                    MoneyPlaceIndex = 0;
                    YAxis += 0.5f;
                }
            }
            yield return new WaitForSeconds(2f);
            Customer.target = null;
            Customer.SetDestination();
            Customer.buying = false;
            isOccupied = false;
            NextCustomer();
        }
    }

    void NextCustomer(){
        if(secondCustomer != null){
            currentCustomer = secondCustomer;
            isOccupied = true;
            currentCustomer.GetComponent<Customer>().target = CustomerPlace.GetChild(0);
            currentCustomer.GetComponent<Customer>().buying = true;
            currentCustomer.GetComponent<Customer>().OnBuy += HandleBuyEvent;
            currentCustomer.GetComponent<Customer>().isWalking = true; 
            currentCustomer.GetComponent<Customer>().SetDestination();

            if(thirdCustomer != null){
                secondCustomer = thirdCustomer;
                secondOccupied = true;
                secondCustomer.GetComponent<Customer>().target = CustomerPlace.GetChild(1);
                secondCustomer.GetComponent<Customer>().buying = true;
                secondCustomer.GetComponent<Customer>().isWalking = true; 
                secondCustomer.GetComponent<Customer>().SetDestination();

                thirdCustomer = null;
                thirdOccupied = false;
            }
            else{
                secondCustomer = null;
                secondOccupied = false;
            }
        }
    }
}
