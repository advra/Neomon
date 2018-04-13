using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stun : MonoBehaviour {
    [SerializeField]
    MonsterController mc;
    [SerializeField]
    private int turns;
    [SerializeField]
    MonsterController.State originalState;
    [SerializeField]
    private GameObject monsterWhoStunned;
    [SerializeField]
    private GameObject stunParticlePrefab;
    private GameObject _stunParticle;

    public GameObject Owner
    {
        get { return monsterWhoStunned; }
        set { monsterWhoStunned = value; }
    }


    //called on execute of each attack
    public void Check()
    {
        this.turns -= 1;
        // not <= 0 because it will immediately trigger once the stun component is added
        if (turns <= 0)
        {
            GetComponent<MonsterController>().monsterState = originalState;
            Destroy(_stunParticle);
            Destroy(GetComponent<Stun>());
        }
    }

    //if monster is already stunned then add to it
    public void AddTurns(int n)
    {
        this.turns += n;
    }

    
    public void SetTurn(int n)
    {
        this.turns = n;
    }

    void Start()
    {
        mc = this.gameObject.GetComponent<MonsterController>();
        originalState = mc.monsterState;
        mc.monsterState = MonsterController.State.STUNNED;
        //by default turns are 1 but can use Set turn
        turns = 1;
        stunParticlePrefab = Resources.Load<GameObject>("Particles/StunRootParticle");
        _stunParticle = Instantiate(stunParticlePrefab, this.transform);
        _stunParticle.transform.localPosition = new Vector3(0, 1, 0);
    }
}
