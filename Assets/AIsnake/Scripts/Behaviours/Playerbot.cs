using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehaviours/Playerbot")]
public class Playerbot : AIBehaviour
{
       public GameObject seek = default ;
       public GameObject target = default ;
        bool run=false;

    public override void Init(GameObject own, SnakeMovement ownMove)
    {
        base.Init(own, ownMove);
        ownerMovement.StartCoroutine(UpdateDirEveryXSeconds(timeChangeDir));
    }
    //seria interessante ter um controlador com o colisor que define o mundo pra poder gerar pontos dentro desse colisor

    public override void Execute()
    {
        Seeking();
        Flee();
    }



    //ia basica, move, muda de direcao e move
    void MoveForward()
    {
        //MouseRotationSnake();
        owner.transform.position = Vector2.MoveTowards(owner.transform.position, target.transform.position, ownerMovement.speed * Time.deltaTime);
    }

    void MouseRotationSnake()
    {

        direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - owner.transform.position;
        direction.z = 0.0f;

        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
        owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, rotation, ownerMovement.speed * Time.deltaTime);
    }

    void Seeking(){
        var orbs = GameObject.FindGameObjectsWithTag("Orb");
        if(orbs.Length > 0){
            seek = orbs[0];
            for(int i = 1; i <  orbs.Length; i++)
            {
                if (Vector3.Distance(orbs[i].transform.position, owner.transform.position) <
                     Vector3.Distance(seek.transform.position, owner.transform.position))
                {
                    seek = orbs[i];
                }            
            }
            Debug.DrawLine(owner.transform.position, seek.transform.position, Color.green, Time.fixedDeltaTime);
            if(!run){
                owner.transform.position = Vector2.MoveTowards(owner.transform.position, seek.transform.position, ownerMovement.speed * Time.deltaTime);
                direction = (seek.transform.position - ownerMovement.transform.position).normalized;
            }
        }
        
    }

    void Flee(){
       var enemys = GameObject.FindGameObjectsWithTag("Bot");
        
        if(enemys.Length > 0){
            for(int i = 0; i <  enemys.Length; i++)
            {
                if (Vector3.Distance(enemys[i].transform.position, owner.transform.position) <
                     Vector3.Distance(seek.transform.position, owner.transform.position))
                {
                    run = true;
                    target = enemys[i];
                }            
                else{
                    run=false;
                }
            }
                Debug.DrawLine(owner.transform.position, target.transform.position, Color.red, Time.fixedDeltaTime);
                if(run && target!= null){
                    owner.transform.position = Vector2.MoveTowards(owner.transform.position, -target.transform.position, ownerMovement.speed * Time.deltaTime);
                    direction = -((target.transform.position - ownerMovement.transform.position).normalized);
                }
            
        }
        
    }

    IEnumerator UpdateDirEveryXSeconds(float x)
    {
        yield return new WaitForSeconds(x);
        ownerMovement.StopCoroutine(UpdateDirEveryXSeconds(x));
        randomPoint = new Vector3(
                Random.Range(
                    Random.Range(owner.transform.position.x - 10, owner.transform.position.x - 5),
                    Random.Range(owner.transform.position.x + 5, owner.transform.position.x + 10)
                ),
                Random.Range(
                    Random.Range(owner.transform.position.y - 10, owner.transform.position.y - 5),
                    Random.Range(owner.transform.position.y + 5, owner.transform.position.y + 10)
                ),
                0
            );
        direction = randomPoint - owner.transform.position;
        direction.z = 0.0f;

        ownerMovement.StartCoroutine(UpdateDirEveryXSeconds(x));
    }
}
