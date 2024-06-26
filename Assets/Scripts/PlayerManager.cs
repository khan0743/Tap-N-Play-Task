using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerManager : MonoBehaviour
{
    private Vector3 direction;
    private Camera Cam;
    [SerializeField] private float playerSpeed;
    private Animator PlrAnim;
    [SerializeField] private List<Transform> stuffs = new List<Transform>();
    [SerializeField] private Transform stufPlace;
    private float YAxis, delay;
    public TextMeshProUGUI MoneyCounter;
    public static PlayerManager PlayerManagerInstance;
    void Start()
    {
        Cam = Camera.main;
        PlrAnim = GetComponent<Animator>();
        
        stuffs.Add(stufPlace);

        PlayerManagerInstance = this;
        MoneyCounter.text = PlayerPrefs.GetInt("money").ToString("C0");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Plane plane = new Plane(Vector3.up, transform.position);
            Ray ray = Cam.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray,out var distance))
                direction = ray.GetPoint(distance);

            transform.position = Vector3.MoveTowards(transform.position, new Vector3(direction.x, 0f, direction.z),
                playerSpeed * Time.deltaTime);

            var offset = direction - transform.position;

            if (offset.magnitude > 1f)
                transform.LookAt(direction);

        }

        if (Input.GetMouseButtonDown(0))
        {
            if (stuffs.Count > 1)
            {
                PlrAnim.SetBool("carry",false);
                PlrAnim.SetBool("RunWithStuffs",true);
            }
            else
            {
                PlrAnim.SetBool("run",true);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            PlrAnim.SetBool("run",false);

            if (stuffs.Count > 1)
            {
                PlrAnim.SetBool("carry",true);
                PlrAnim.SetBool("RunWithStuffs",false);
            }

        }

        if (stuffs.Count > 1)
        {
            for (int i = 1; i < stuffs.Count; i++)
            {
                var firstPaper = stuffs.ElementAt(i - 1);
                var secondPaper = stuffs.ElementAt(i);
                
                secondPaper.position = new Vector3(Mathf.Lerp(secondPaper.position.x,firstPaper.position.x,Time.deltaTime * 15f),
                Mathf.Lerp(secondPaper.position.y,firstPaper.position.y + 0.17f,Time.deltaTime * 15f),firstPaper.position.z);
            }
        }

        if (Physics.Raycast(transform.position,transform.forward,out var hit,1f))
        {
            Debug.DrawRay(transform.position,transform.forward * 1f,Color.green);
            
            if (hit.collider.CompareTag("table") && stuffs.Count < 21)
            {
                if (hit.collider.transform.childCount > 0)
                {
                    var stuff = hit.collider.transform.GetChild(0);
                    stuff.rotation = Quaternion.Euler(stuff.rotation.x,Random.Range(0f,180f),stuff.rotation.z);
                    stuffs.Add(stuff);
                    stuff.parent = null;

                    if (hit.collider.transform.parent.GetComponent<Generator>().CountStuffs > 0)
                        hit.collider.transform.parent.GetComponent<Generator>().CountStuffs--;

                    if (hit.collider.transform.parent.GetComponent<Generator>().YAxis > 0f)
                        hit.collider.transform.parent.GetComponent<Generator>().YAxis -= 0.17f;

                    PlrAnim.SetBool("carry",true);
                    PlrAnim.SetBool("run",false);
                }
            }

            if (hit.collider.CompareTag("counter") && stuffs.Count > 1)
            {
                var Counter = hit.collider.transform;

                if (Counter.childCount > 0)
                {
                    YAxis = Counter.GetChild(Counter.childCount - 1).position.y;
                }
                else
                {
                    YAxis = Counter.position.y;
                }

                for (var index = stuffs.Count - 1; index >= 1; index--)
                {
                    stuffs[index].DOJump(new Vector3(Counter.position.x, YAxis, Counter.position.z), 2f, 1, 0.2f).SetDelay(delay).SetEase(Ease.Flash);
                    stuffs.ElementAt(index).parent = Counter;
                    stuffs.RemoveAt(index);
                    Counter.GetComponent<Counter>().noOfStuffs++;
                    YAxis += 0.17f;
                    delay += 0.02f;
                    
                }

                if (stuffs.Count <= 1)
                {
                    PlrAnim.SetBool("idle",true);
                    PlrAnim.SetBool("RunWithStuffs",false);
                }
                Counter.GetComponent<Counter>().HandleBuyEvent();
                
            }
        }
        else
        {
            Debug.DrawRay(transform.position,transform.forward * 1f,Color.red);

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("money"))
        {
            Destroy(other.gameObject);
            
            PlayerPrefs.SetInt("money",PlayerPrefs.GetInt("money") + 5);

            MoneyCounter.text = PlayerPrefs.GetInt("money").ToString("C0");
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("counter"))
        {
            PlrAnim.SetBool("RunWithStuffs",false);
            PlrAnim.SetBool("idle",false);
            PlrAnim.SetBool("run",true);
            delay = 0f;
        }
        
        if (other.CompareTag("table"))
        {

            if (stuffs.Count > 1)
            {
                other.transform.parent.GetComponent<Generator>().RestartGeneration();
                PlrAnim.SetBool("carry",false);
                PlrAnim.SetBool("RunWithStuffs",true);
            }
            else
            {
                PlrAnim.SetBool("run",true);
            }
        }
    }
}
