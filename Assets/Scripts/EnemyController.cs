using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameManager GM;
    public PlayerController PC;
    SpriteRenderer m_SpriteRenderer;
    
    public int castRange, sightRange, movesMade, currentX, currentY, spellCooldown;

    public float rangeToTarget;

    public GameObject target;

    public Sprite [] sprites;

    public int ID;

    public Vector3 offset;

    public bool moved, enabled;

    public GameObject spellIndicator;

    public List<Vector2> spells = new List<Vector2>();

    public List<GameObject> indicators = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {

        ID = Random.Range(0,3);
        GM = GameObject.Find("Manager").GetComponent<GameManager>();
        movesMade = 0;
        enabled = true;
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_SpriteRenderer.sprite = sprites[ID];
    }

    // Update is called once per frame
    void Update()
    {
        

        if(!target){
            target = GameObject.Find("Player(Clone)");
            PC = target.GetComponent<PlayerController>();
        }

        if(GM.turnCounter > movesMade){
            if (spellCooldown >= 0) 
                spellCooldown --;
            moved = false;
            offset = target.transform.position - this.transform.position;
            rangeToTarget = Mathf.Sqrt(offset.sqrMagnitude);
            ResolveSpells();

            if(enabled){
                Move();
                movesMade++;

            }

        }



        // if(sprites.Length > 1)
        // {
        //     if (PC.target == this.gameObject)
        //     {
        //         m_SpriteRenderer.sprite = sprites[1]; //highlighted
        //     }
        //     else
        //     {
        //         m_SpriteRenderer.sprite = sprites[0]; //regular
        //     }
        // }
    }

    void Move(){

        if(rangeToTarget < 2){
            Attack();
        }else if(rangeToTarget <= castRange && spellCooldown < 0){
            CastSpell();
        } else if (rangeToTarget <= sightRange) {
            MoveTowardsTarget();
        } else { 
            return; 
        }

    }

    void ResolveSpells(){

        //Play animation of splode
        Vector2 playerLocation = new Vector2(PC.currentX, PC.currentY);

        if(spells.Contains(playerLocation)){
            PC.health --;
        }

        spells = new List<Vector2>();

        //Remove bubbles
        foreach (GameObject g in indicators){
            Destroy(g);
        }
        
    }

    void CastSpell(){
        if(ID == 0){
            int x = 2;
            Vector2 spellPos = new Vector2(PC.currentX + x, PC.currentY);
            spells.Add(spellPos);

            spellPos = new Vector2(PC.currentX - x, PC.currentY);
            spells.Add(spellPos);

            spellPos = new Vector2(PC.currentX, PC.currentY - x);
            spells.Add(spellPos);

            spellPos = new Vector2(PC.currentX , PC.currentY + x);
            spells.Add(spellPos);

            spellPos = new Vector2(PC.currentX , PC.currentY);
            spells.Add(spellPos);
        }
            

        if(ID == 1){
            //Flag a line through the player
            int r = Random.Range(0,2);

            if(r == 1){
                for(int x = -3; x < 4; x++){
                    Vector2 spellPos = new Vector2(PC.currentX + x , PC.currentY);
                    spells.Add(spellPos);
                }
            } else {
                for(int y = -3; y < 4; y++){
                    Vector2 spellPos = new Vector2(PC.currentX , PC.currentY + y);
                    spells.Add(spellPos);
                }


            }

        }
        if(ID == 2){
            //Flag a star around the player
            //5x5
            for(int x = -2; x < 3; x++){
                Vector2 spellPos = new Vector2(PC.currentX + x , PC.currentY + x);
                spells.Add(spellPos);
                spellPos = new Vector2(PC.currentX + x , PC.currentY - x);
                spells.Add(spellPos);
                
            }
        }
        if(ID == 3){
            //Flag whole square through player
            //3x3
        }

        for (int x =0; x<spells.Count; x++){
            indicators.Add(Instantiate(spellIndicator,spells[x] - LevelGenerator.roomSizeWorldUnits / 2 ,Quaternion.identity ));
        }
        
        
        spellCooldown = 3;
    }

    void MoveTowardsTarget(){

        if(!moved){

            if(offset.x > 0){

                if(LevelGenerator.grid[currentX + 1, currentY].tileType == 1){
                    moved = true;
                    this.transform.position += Vector3.right * 1;
                    LevelGenerator.grid[currentX,currentY].tileType = 1;
                    LevelGenerator.grid[currentX + 1,currentY].tileType = 3;
                    currentX ++;
                }
        
            } 

        }

        if(!moved){
        
            if(offset.x < 0) {

                if(LevelGenerator.grid[currentX -1, currentY].tileType == 1){
                    moved = true;
                    this.transform.position += Vector3.left * 1;
                    LevelGenerator.grid[currentX,currentY].tileType = 1;
                    LevelGenerator.grid[currentX -1 ,currentY].tileType = 3;
                    currentX --;
                }
                

            } 
        }
        
        if(!moved){

            if (offset.y > 0){

                if(LevelGenerator.grid[currentX, currentY  + 1].tileType == 1){
                    moved = true;
                    this.transform.position += Vector3.up * 1;
                    LevelGenerator.grid[currentX,currentY].tileType = 1;
                    LevelGenerator.grid[currentX,currentY + 1].tileType = 3;
                    currentY ++;

                }     
                    
            } 
        }
        
        
        if(!moved){ 

            if (offset.y < 0) {

                if(LevelGenerator.grid[currentX , currentY - 1].tileType == 1)
                    moved = true;
                    this.transform.position += Vector3.down * 1;
                    LevelGenerator.grid[currentX,currentY].tileType = 1;
                    LevelGenerator.grid[currentX,currentY - 1].tileType = 3;
                    currentY --;
            }

        }

        


    }

    void Attack(){
        //Play attack animation
        PC.health --;
    }


    public void SetCoords(int x, int y){
        currentY = y;
        currentX = x;
    }
}
