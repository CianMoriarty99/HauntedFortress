using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    public GameManager GM;
    public int movesMade, castRange, currentX, currentY, health;

    public GameObject target;

    SpriteRenderer spr;
    public Sprite[] sprites;

    public float rangeToTarget;

    public Vector3 offset;

    bool input, key;
    // Start is called before the first frame update
    void Awake()
    {
        castRange = 3;
        movesMade = 0;
        input = true;
        spr = GetComponent<SpriteRenderer>();
        health = 1;
        spr.sprite = sprites[0];

        GM = GameObject.Find("Manager").GetComponent<GameManager>();

    }

    // Update is called once per frame
    void Update()
    {
        //Mouse targeting
        target = null;
        Vector3 mousePos;
        mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3 refPoint = new Vector3 (mousePos.x,mousePos.y, -100);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, refPoint );
        if (hit.collider != null)
        {
            if(hit.collider.tag == "Enemy")
                target = hit.transform.gameObject;
        }

        if(target){

            offset = target.transform.position - this.transform.position;
            rangeToTarget = Mathf.Sqrt(offset.sqrMagnitude);

        }


        //Get Inputs
        int xRaw = (int)Input.GetAxisRaw("Horizontal");
        int yRaw = (int)Input.GetAxisRaw("Vertical");

        if(input){


            if(xRaw != 0 && LevelGenerator.grid[currentX + xRaw, currentY].tileType == 1) {
                LevelGenerator.grid[currentX, currentY].tileType = 1;
                LevelGenerator.grid[currentX + xRaw, currentY].tileType = 4;
                this.transform.position += Vector3.right * xRaw;
                currentX += xRaw;

                StartCoroutine(AdvanceTurn( 0.2f));
            } else if(yRaw != 0 && LevelGenerator.grid[currentX , currentY+ yRaw].tileType == 1) {
                LevelGenerator.grid[currentX, currentY].tileType = 1;
                LevelGenerator.grid[currentX, currentY + yRaw].tileType = 4;
                currentY += yRaw;
                this.transform.position += Vector3.up * yRaw;
                StartCoroutine(AdvanceTurn( 0.2f));
            }
            

            if(Input.GetKeyDown("c") && target && rangeToTarget < castRange && health == 1){
                LevelGenerator.grid[currentX, currentY].tileType = 1;
                AssumeTarget();
                LevelGenerator.grid[currentX, currentY].tileType = 4;
                StartCoroutine(AdvanceTurn( 0.2f));
                
            }


        }

        if(health == 1){
            spr.sprite = sprites[0];
        }

        if(health == 0){
            LevelGenerator.difficulty = 1;
            SceneManager.LoadScene("SampleScene");
        }


    }

    void AssumeTarget(){
        Debug.Log("ASSUMING");
        this.transform.position = target.transform.position;

        var targetScript = target.GetComponent<EnemyController>();

        currentX = targetScript.currentX;
        currentY = targetScript.currentY;

        targetScript.enabled = false;
        health = 2;
        //target.SetActive(false);
        //Disable Enemy sprite
        target.GetComponent<SpriteRenderer>().enabled = false;
        //Change sprite of this
        spr.sprite = sprites[target.GetComponent<EnemyController>().ID + 1];
        //Disable Enemy collider
        target.GetComponent<BoxCollider2D>().enabled = false;
        //Move player to enemy position


    }


    IEnumerator AdvanceTurn(float t){
        input = false;
        GM.turnCounter++;
        yield return new WaitForSeconds(t);
        input = true;
    }

    public void SetCoords(int x, int y){
        currentY = y;
        currentX = x;
    }



    void OnTriggerStay2D(Collider2D col){

        Debug.Log("entering trigger");
        if(col.tag == "Key"){
            key = true;
            Destroy(col.gameObject);
        }

        if(col.tag == "Stairs" && key){
            LevelGenerator.difficulty ++;
            SceneManager.LoadScene("SampleScene");
        }
    }

}


