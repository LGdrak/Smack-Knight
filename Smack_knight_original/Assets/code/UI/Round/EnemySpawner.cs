using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    // stored rounds
    private Round round;
    public Round round1;
    public Round round2;
    public Round round3;
    public Round round4;
    public Round round5;
    public Round round6;
    public Round round7;
    public Round round8;
    public Round round9;
    public Round round10;
    public Round round11;
    public Round round12;
    public Round round13;
    public Round round14;
    public Round round15;
    public Round round16;
    public Round round17;
    public Round round18;
    public Round round19;
    public Round round20;
    private List<Round> game = new List<Round>();

    public TextMeshProUGUI RoundText;
    public TextMeshProUGUI WaveText;
    // wawes on enemies counted on a counter
    private int waveBreak = 1;
    
    int pos;
    // individual enemi wawes in rounds
    int wawes = 0;
    int placement;
    // enemies in rounds
    int enemyType = 0;
    float waitTime = 30f;
    Vector3 offset;
    private GameObject enemy;
    private RoundControler RC;
    public GameObject entity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        game.Add(round1);
        game.Add(round2);
        game.Add(round3);
        game.Add(round4);
        game.Add(round5);
        game.Add(round6);
        game.Add(round7);
        game.Add(round8);
        game.Add(round9);
        game.Add(round10);
        game.Add(round11);
        game.Add(round12);
        game.Add(round13);
        game.Add(round14);
        game.Add(round15);
        game.Add(round16);
        game.Add(round17);
        game.Add(round18);
        game.Add(round19);
        game.Add(round20);


        RC = gameObject.transform.parent.GetComponent<RoundControler>();
    }

// starts spawning enemyes based on the round
    public void Spawning(int round_){

        if (round_ != 19)
        {
            RoundText.gameObject.SetActive(true);
        }
        if (round_ == 19)
        {
            RC.inRange = false;
            Destroy(gameObject.GetComponent<SpriteRenderer>());
            Destroy(gameObject.transform.parent.gameObject.GetComponent<SpriteRenderer>());
            Destroy(gameObject.transform.parent.gameObject.GetComponent<CircleCollider2D>());
            Destroy(gameObject.transform.parent.gameObject.GetComponent<BoxCollider2D>());
            Instantiate(entity, new Vector3(0, 33 , 0), Quaternion.identity, gameObject.transform);
        }
        RoundText.text = "Round " + (round_ + 1).ToString();
        waveBreak = 1;
        WaveText.text = "wave " + waveBreak.ToString();
        Debug.Log(round_);
        round = game[round_];

        if (round.count[wawes] == 0)
        {
            // waits 30 s
            StartCoroutine(Wait());
        }
        else{
            // spawns a wawe of enemies based on wawes variable
            Spawn();
        }
    }

    private void Spawn()
    {
        enemy = round.enemyes[enemyType];

        for (int i = 0; i < round.count[wawes] ; i++){

// logic of spawning to spawn some enemies near the player
            if(i == 0)
            {
                placement = 0;
                pos = Random.Range(0, transform.GetChild(0).transform.GetChild(placement).childCount);
            }
            else if (i > 0 && i < 5)
            {
                placement = 1;
                pos = Random.Range(0, transform.GetChild(0).transform.GetChild(placement).childCount);
            }
            else if (i > 4 && i < 9)
            {
                placement = 2;
                pos = Random.Range(0, transform.GetChild(0).transform.GetChild(placement).childCount);
            }
            else{
                placement = Random.Range(0, 5);
                pos = Random.Range(0, transform.GetChild(0).transform.GetChild(placement).childCount);
            }
            // offset from the spawn point
            offset = new Vector3(Random.Range(-3,3), Random.Range(-3,3), 0);

// creating a enemy
            Instantiate(enemy, transform.GetChild(0).GetChild(placement).GetChild(pos).position + offset, Quaternion.identity, gameObject.transform);
        }

        wawes ++;
        enemyType ++;

        Decide();
    }

// waits 30 s and increases variables
    IEnumerator Wait(){
        yield return new WaitForSecondsRealtime(waitTime);

        waveBreak ++;
        WaveText.text = "wave " + waveBreak.ToString();
        wawes ++;
        Decide();
    }

// decides to spawn a wawe or end or wait
    private void Decide(){

        if (wawes + 1 > round.count.Length){
            // a boolian for deciding if the round can end
            RC.done = true;
            // counting variables need to be reset
            wawes = 0;
            enemyType = 0;

        }
        else if (round.count[wawes] == 0)
        {
            StartCoroutine(Wait());
        }
        else{
            Spawn();
        }
    }
}
