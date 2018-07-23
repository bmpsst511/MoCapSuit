using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreControl : MonoBehaviour {

    public int score=0;

    public int times=0;
    public  Text score_view;

    public Text times_view;
    public static ScoreControl Instance;
    // Use this for initialization
    void Awake()
    {
        
    }
    void Start () {
    }

    // Update is called once per frame
    void Update () {
        score_view.text="分數:"+score;
        times_view.text="次數:"+times;
       // force_view.text="Force:"+force;
    }



}
