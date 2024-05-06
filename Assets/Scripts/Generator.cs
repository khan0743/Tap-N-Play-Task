using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class Generator : MonoBehaviour
{
   [SerializeField] private Transform[] StuffPlace = new Transform[10];
   [SerializeField] private GameObject stuff;
   public float StuffDeliveryTime,YAxis;
   public int CountStuffs, maxStuffs;
    void Start()
    {
        for (int i = 0; i < StuffPlace.Length; i++)
        {
            StuffPlace[i] = transform.GetChild(0).GetChild(i);
        }

        StartCoroutine(GenerateStuff(StuffDeliveryTime));
    }

    public IEnumerator GenerateStuff(float Time)
    {
        yield return new WaitForSeconds(2f);
        var stuffIndex = 0;
        
        while (CountStuffs < maxStuffs)
        {
            GameObject NewStuff = Instantiate(stuff, new Vector3(transform.position.x, -3f, transform.position.z),
                quaternion.identity, transform.GetChild(1));
                
            CountStuffs++;

            NewStuff.transform.DOJump(new Vector3(StuffPlace[stuffIndex].position.x, StuffPlace[stuffIndex].position.y + YAxis,
                StuffPlace[stuffIndex].position.z), 2f, 1, 0.5f).SetEase(Ease.OutQuad);

            if (stuffIndex < 9)
                stuffIndex++;
            else
            {
                stuffIndex = 0;
                YAxis += 0.17f;
            }
            
            yield return new WaitForSecondsRealtime(Time);

        }
    }

    public void RestartGeneration(){
        StartCoroutine(GenerateStuff(StuffDeliveryTime));
    }
}
