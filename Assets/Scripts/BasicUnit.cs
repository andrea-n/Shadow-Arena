using UnityEngine;
using System.Collections;

public class BasicUnit : MonoBehaviour {

    public int reach; //jak daleko muze jednotka dojit
    protected int speed = 3; //rychlost pohybu pres policka
    public int side; //strana za za kterou jednotka hraje
    public int rank;
    protected int hiddenRank;
    public ArrayList path;
    protected bool moving = false, attacking = false; //je v pohybu
    protected Vector3 target; //souradnice policek na ktera se jednotka postupne presouva
    protected HexTile targetTile, attackingTile; //reference na policko kam se ve finale presunem
    public GameObject mainModel;
    public GameObject hiddenModel;
    private GameObject currentModel;
    public bool visibleModel = false;
    public bool vulnerable = true, moveable = true, passiveAbility = false;
    private Animator animator;


    public void setPath(ArrayList pathList)
    {
        path = (ArrayList) pathList.Clone();
    }

    public void proceedPath()
    {
        foreach (HexTile obj in path)
        {
            target = obj.transform.position;
            targetTile = obj;
            break;
        }
        path.RemoveAt(0);
        moving = true;
    }

    public void proceedAttack(HexTile destinationTile)
    {
        attackingTile = destinationTile;
        attacking = true;
        path.RemoveAt(path.Count - 1);
        if (path.Count > 0) 
            proceedPath();
        else
            attackTile();
    }

    public int attackedGetRank()
    {
        if (passiveAbility)
        {
            useAbility();
        }
        visibleModel = true;
        unhideUnit();
        return hiddenRank;
    }

    public void resetRank()
    {
        hiddenRank = rank;
    }

    private IEnumerator destroy()
    {
        yield return new WaitForSeconds(2);
        Destroy(currentModel);
        Destroy(this);
    }

    public void destroyUnit()
    {
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }
        StartCoroutine(destroy());
    }

    public void attackTile()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        StartCoroutine(attackTileForce());
    }

    private IEnumerator attackTileForce()
    {
        //yield return new WaitForSeconds(1.5f);
        if (passiveAbility)
        {
            useAbility();
        }
        visibleModel = true;
        moving = false;
        attacking = false;
        if (hiddenRank > attackingTile.unit.GetComponent<BasicUnit>().attackedGetRank())
        {
            
            attackingTile.unit.GetComponent<BasicUnit>().destroyUnit();
            attackingTile.unit = null;
            path.Add(attackingTile);
            yield return new WaitForSeconds(1.5f);
            proceedPath();
            resetRank();
        }
        else if(hiddenRank < attackingTile.unit.GetComponent<BasicUnit>().attackedGetRank())
        {
            yield return new WaitForSeconds(1.5f);
            attackingTile.unit.GetComponent<BasicUnit>().resetRank();
            //HexGridFieldManager.instance.selectedHex.unit.GetComponent<BasicUnit>().destroyUnit();
            path.Clear();
            HexGridFieldManager.instance.selectedHex.unHighlightUnitTile();
            HexGridFieldManager.instance.selectedHex.unHighlightTile(true);
            HexGridFieldManager.instance.selectedHex.selectNeighbours(HexGridFieldManager.instance.selectedHex.unit.GetComponent<BasicUnit>().reach, false);
            HexGridFieldManager.instance.selectedHex.unit = null;
            HexGridFieldManager.instance.selectedHex = null;
            destroyUnit();
        }
        else
        {
            yield return new WaitForSeconds(1.5f);
            attackingTile.unit.GetComponent<BasicUnit>().destroyUnit();
            attackingTile.unit = null;
            path.Clear();
            HexGridFieldManager.instance.selectedHex.unHighlightUnitTile();
            HexGridFieldManager.instance.selectedHex.unHighlightTile(true);
            HexGridFieldManager.instance.selectedHex.selectNeighbours(HexGridFieldManager.instance.selectedHex.unit.GetComponent<BasicUnit>().reach, false);
            HexGridFieldManager.instance.selectedHex.unit = null;
            HexGridFieldManager.instance.selectedHex = null;
            destroyUnit();
        }
    }

    public void finishPath()
    {
        moving = false;
        path.Clear();
        HexGridFieldManager.instance.selectedHex.unHighlightTile(true);
        HexGridFieldManager.instance.selectedHex.selectNeighbours(HexGridFieldManager.instance.selectedHex.unit.GetComponent<BasicUnit>().reach, false);
        targetTile.unit = HexGridFieldManager.instance.selectedHex.unit;
        targetTile.highlightUnitTile();
        HexGridFieldManager.instance.selectedHex.unit = null;
        HexGridFieldManager.instance.selectedHex.unHighlightUnitTile();
        if (attacking)
        {
            HexGridFieldManager.instance.selectedHex = targetTile;
            attackTile();
        }
        else
        {
            HexGridFieldManager.instance.selectedHex = null;
        }
    }

    public void setPosition(Vector3 pos)
    {
        transform.position = pos;
        currentModel.transform.position = pos;
    }

    public bool isMoveable()
    {
        return moveable;
    }

    public bool isVulnerable()
    {
        return vulnerable;
    }

    public bool isMoving()
    {
        return moving;
    }

    public void hideUnit()
    {
        if (visibleModel)
            return;
        if(currentModel != null)
        {
            Destroy(currentModel);
        }
        currentModel = (GameObject)Instantiate(hiddenModel);
        animator = currentModel.GetComponent<Animator>();
        currentModel.transform.position = transform.position;
        currentModel.transform.rotation = transform.rotation;
        //GetComponent<MeshFilter>().mesh = hiddenMesh;
    }

    public void unhideUnit()
    {
        if (currentModel != null)
        {
            Destroy(currentModel);
        }
        currentModel = (GameObject)Instantiate(mainModel);
        animator = currentModel.GetComponent<Animator>();
        currentModel.transform.position = transform.position;
        currentModel.transform.rotation = transform.rotation;
        //GetComponent<MeshFilter>().mesh = mainMesh;
    }

    public virtual void useAbility()
    {

    }

    // Use this for initialization
    void Start () {
        path = new ArrayList();
        resetRank();
        animator = mainModel.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            float step = speed * Time.deltaTime;
            setPosition(Vector3.MoveTowards(currentModel.transform.position, target, step));
            //transform.position = Vector3.MoveTowards(transform.position, target, step);
            if (transform.position.Equals(target))
            {
                if (path.Count == 0)
                    finishPath();
                else
                    proceedPath();
            }
        }
    }
}
