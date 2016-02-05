using UnityEngine;
using System.Collections;

public class LayoutHexTile : HexTile {

    public int numberOfAvailableUnits;
    public GameObject textNumberPattern;
    private GameObject textNumber;

    void OnMouseEnter() //po najeti mysi na pole
    {
        previousColor = GetComponent<Renderer>().material.color;
        changeColor(Color.green); //zmenime barvu na zelenou
    }

    void OnMouseExit() //jak mile mys neni na poli, tak vratime barvu pole do puvodni barvy
    {
        if (!selected)
        {
            GetComponent<SpriteRenderer>().sprite = outlineSprite;
            GetComponent<Renderer>().material = defaultMaterial;
            if (unit == null || unit.GetComponent<BasicUnit>().side != HexGridFieldManager.instance.playerTurn)
                GetComponent<Renderer>().material.color = orange2;
            else
                GetComponent<Renderer>().material.color = Color.green;
        }
        else
        {
            changeColor(previousColor);
        }
    }

    /*void OnMouseOver() //pokud je mys na poli a klikneme pravym tlacitkem, tak provedeme akci
    {
        
    }*/

    void OnMouseDown()
    {
        if (numberOfAvailableUnits <= 0)
            return;
        
        if (!selected)
        {
            if (HexGridFieldManager.instance.selectedLayoutHex != null) //pokud bylo nejake pole vybrano, tak vyber zrusime
            {
                HexGridFieldManager.instance.selectedLayoutHex.unHighlightTile(true);
            }
            HexGridFieldManager.instance.selectedLayoutHex = this;
            highlightTile(orange, true);
        }
        else //pokud pole bylo vybrano a znovu na nej klikneme, tak zrusime vybrani a zvyrazneni dalsich poli
        {
            unHighlightTile(true);
            HexGridFieldManager.instance.selectedLayoutHex = null;
        }
    }

    public void changeAvailableUnits(int number)
    {
        numberOfAvailableUnits += number;
        textNumber.GetComponent<TextMesh>().text = "Rank " + unit.GetComponent<BasicUnit>().rank + " available: " + numberOfAvailableUnits;
        if(numberOfAvailableUnits <= 0)
        {
            unHighlightTile(true);
            HexGridFieldManager.instance.selectedLayoutHex = null;
        }
    }

    public void init(int playerTurn)
    {
        textNumber = (GameObject)Instantiate(textNumberPattern);
        textNumber.GetComponent<TextMesh>().text = "Rank x available: " + numberOfAvailableUnits;
        textNumber.transform.position = this.transform.position;
        textNumber.transform.position += new Vector3(-1.5f, 0, 1.5f * (playerTurn == 0 ? 1 : -1));
        if (playerTurn == 1)
        {
            textNumber.transform.Rotate(new Vector3(0, 0, 180));
            //unit.transform.Rotate(new Vector3(0, 180, 0));
        }
    }

    public void setAvailableUnits(int number)
    {
        numberOfAvailableUnits = number;
        textNumber.GetComponent<TextMesh>().text = "Rank " + unit.GetComponent<BasicUnit>().rank + " available: " + numberOfAvailableUnits;
        if (numberOfAvailableUnits <= 0)
        {
            unHighlightTile(true);
            HexGridFieldManager.instance.selectedLayoutHex = null;
        }
    }

    public void destroy()
    {
        unit.GetComponent<BasicUnit>().forceDestroyUnit();
        Destroy(textNumber);
    }

    // Use this for initialization
    void Start()
    {
        /*textNumber = (GameObject)Instantiate(textNumberPattern);
        textNumber.GetComponent<TextMesh>().text = "Rank x available: " + numberOfAvailableUnits;
        textNumber.transform.position = this.transform.position;
        textNumber.transform.position += new Vector3(-1.5f, 0, 1.5f);*/
    }

    // Update is called once per frame
    void Update()
    {

    }
}