using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;
using UnityEngine.UI;

public struct Point {
    public int x, y;

    public Point(int pX, int pY) {
        x = pX;
        y = pY;
    }
}

public class HexGridFieldManager : MonoBehaviour {

    public Camera cam;
    public GameObject hex;
    public GameObject layoutHex;
    public GameObject[] unitObject;
    public Vector2 gridSize;
    public Vector3 layoutHexPosition;
    public HexTile selectedHex;
    public LayoutHexTile selectedLayoutHex;
    public GameObject[] treeObject;
    public int[] rankNumberOfUnits;
    public int playerTurn = 0;
    public int numberOfActionsMax = 1;
    public Text unitRankText, numberOfActionsText;
    public GameObject[] GUIObjects;
    public GameObject NextPlayerButton, StartGameButton;
    public InputField inputText;
    public bool editMode = true;

    private int numberOfActions;
    private Vector3 playerCameraPosition1, playerCameraPosition2;
    private Quaternion playerCameraRotation1, playerCameraRotation2;
    private Hashtable board;
    private Color orange2 = new Color(255f / 255f, 191f / 255f, 54f / 255f, 127f / 255f);
    private GameObject[] layoutHexTiles;

    public static HexGridFieldManager instance = null;

    void inicialization() {
        instance = this;
    }

    public void proceedAction() {
        numberOfActions--;
        numberOfActionsText.text = "Number of actions: " + numberOfActions;
    }

    public bool hasAvailableActions() {
        return numberOfActions > 0;
    }

    public void nextTurn() {
        playerCameraPosition1 = (playerTurn == 0 ? cam.transform.position : playerCameraPosition1);
        playerCameraPosition2 = (playerTurn == 1 ? cam.transform.position : playerCameraPosition2);
        playerCameraRotation1 = (playerTurn == 0 ? cam.transform.localRotation : playerCameraRotation1);
        playerCameraRotation2 = (playerTurn == 1 ? cam.transform.localRotation : playerCameraRotation2);
        playerTurn = (playerTurn == 0 ? 1 : 0);
        numberOfActions = numberOfActionsMax;
        numberOfActionsText.text = "Number of actions: " + numberOfActions;
        PathFinder.instance.reset();
        cam.GetComponent<cameraMovement>().reset();
        cam.transform.position = (playerTurn == 0 ? playerCameraPosition1 : playerCameraPosition2);
        cam.transform.localRotation = (playerTurn == 0 ? playerCameraRotation1 : playerCameraRotation2);
        if (selectedHex != null) {
            selectedHex.unHighlightTile(true);
            selectedHex.selectNeighbours(selectedHex.unit.GetComponent<BasicUnit>().reach, false);
        }

        for (int j = 0; j < gridSize.y; j++) {
            for (int i = 0; i < gridSize.x; i++) {
                HexTile tile = ((GameObject)board[new Point(i, j)]).GetComponent<HexTile>();
                if (tile.unit != null) {
                    if (tile.unit.GetComponent<BasicUnit>().side == playerTurn)
                    {
                        tile.unit.GetComponent<BasicUnit>().unhideUnit();
                        tile.highlightUnitTile();
                    } else {
                        tile.unit.GetComponent<BasicUnit>().hideUnit();
                        tile.unHighlightUnitTile();
                    }
                }
            }
        }
    }

    void createGrid() {
        board = new Hashtable();
        float offsetX = 0;
        int extendedGrid = 0;
        float numberOfTrees = Random.Range(4.0f, 20.0f);
        float numberOfTreesInLine = Random.Range(0.0f, 5.0f);
        float currentNumberOfTrees = 0.0f;
        for (int j = 0; j < gridSize.y; j++) {
            currentNumberOfTrees = numberOfTreesInLine;
            if (j % 2 == 0) {
                offsetX = hex.GetComponent<Renderer>().bounds.size.x / 2;
            } else {
                offsetX = 0;
            }

            //extendedGrid =  ((j < gridSize.y / 2) ? (extendedGrid++) : (extendedGrid--));

            for (int i = 0; i < gridSize.x + extendedGrid; i++) {
                GameObject oneHex = (GameObject)Instantiate(hex);
                oneHex.transform.position = new Vector3(i * hex.GetComponent<Renderer>().bounds.size.x + offsetX - extendedGrid / 2 * hex.GetComponent<Renderer>().bounds.size.x, 0, j * (hex.GetComponent<Renderer>().bounds.size.z * 3 / 4));
                oneHex.GetComponent<HexTile>().changeOutlineColor(orange2);
                if (j > 5 && j < gridSize.y - 5) {
                    if (currentNumberOfTrees > 0 && numberOfTrees > 0 && Random.Range(0.0f, 100.0f) > 95.0f - i * 2) {
                        numberOfTrees--;
                        currentNumberOfTrees--;
                        oneHex.GetComponent<HexTile>().setPlaceHolder((GameObject)Instantiate(treeObject[Random.Range(0, treeObject.Length - 1)]));
                    }
                }
                
                oneHex.GetComponent<HexTile>().setBoardPosition(i, j);
                board.Add(new Point(i, j), oneHex);
            }
            if (j < gridSize.y / 2 - 1)
            {
                extendedGrid++;
            }
            else
            {
                extendedGrid--;
            }
        }

        foreach (GameObject tile in board.Values) {
            tile.GetComponent<HexTile>().FindNeighbours(board, gridSize);
        }
    }

    public void createLayoutHexTiles()
    {
        for (int k = 0; k < layoutHexTiles.Length; k++)
        {
            int i = k >= 6 ? k - 6 : k;
            int j = k >= 6 ? 1 : 0;
            GameObject layHex = (GameObject)Instantiate(layoutHex);
            if (playerTurn == 0)
            {
                layHex.transform.position = new Vector3(layoutHexPosition.x + i * (hex.GetComponent<Renderer>().bounds.size.x + 1.5f), layoutHexPosition.y, layoutHexPosition.z - j * (hex.GetComponent<Renderer>().bounds.size.z + 1));
            } else {
                layHex.transform.position = new Vector3(layoutHexPosition.x + i * (hex.GetComponent<Renderer>().bounds.size.x + 1.5f), layoutHexPosition.y, layoutHexPosition.z + 40 - j * (hex.GetComponent<Renderer>().bounds.size.z + 1));
            }
            layHex.GetComponent<LayoutHexTile>().changeOutlineColor(orange2);
            layHex.GetComponent<LayoutHexTile>().setUnit((GameObject)Instantiate(unitObject[k]), playerTurn);
            layHex.GetComponent<LayoutHexTile>().unit.GetComponent<BasicUnit>().unhideUnit();
            layHex.GetComponent<LayoutHexTile>().init(playerTurn);
            layHex.GetComponent<LayoutHexTile>().setAvailableUnits(rankNumberOfUnits[k]);
            layoutHexTiles[k] = layHex;
        }
    }

    public void resetGrid()
    {
        foreach (GameObject tile in board.Values)
        {
            if (playerTurn == 0)
            {
                if(tile.GetComponent<HexTile>().boardPosition.y > 4)
                    break;
                tile.GetComponent<HexTile>().resetTile();
            }
            else if (playerTurn == 1)
            {
                if (tile.GetComponent<HexTile>().boardPosition.y > gridSize.y - 5)
                    tile.GetComponent<HexTile>().resetTile();
            }
        }
    }

    public void loadLayout()
    {
        resetGrid();
        for (int k = 0; k < layoutHexTiles.Length; k++)
        {
            layoutHexTiles[k].GetComponent<LayoutHexTile>().setAvailableUnits(0);
        }

		if (!File.Exists (Application.dataPath + "/Resources/" + inputText.text + ".xml")) {
			return;
		}
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(Application.dataPath + "/Resources/" + inputText.text + ".xml");
        XmlNodeList nodeList;
        nodeList = xmlDoc.SelectNodes("/Units/Unit"); ;
        foreach (XmlNode xn in nodeList)
        {
            string unitX = xn["positionX"].InnerText;
            string unitY = xn["positionY"].InnerText;
            string unitID = xn["rank"].InnerText;
            GameObject n;
            if (playerTurn == 0)
            {
                n = (GameObject)board[new Point(int.Parse(unitX), int.Parse(unitY))];
            } else {
            n = (GameObject)board[new Point((int)gridSize.x + (int.Parse(unitY)) - 1 - int.Parse(unitX), (int)gridSize.y - 1 - int.Parse(unitY))];
            }
            n.GetComponent<HexTile>().setUnit((GameObject)Instantiate(unitObject[int.Parse(unitID) + 1]), playerTurn);
            n.GetComponent<HexTile>().setSide(playerTurn, true);
            n.GetComponent<HexTile>().highlightUnitTile();
            n.GetComponent<HexTile>().unit.GetComponent<BasicUnit>().unhideUnit();
        }
        checkAvailableLayoutUnits();
    }

    public void saveLayout()
    {
        using (XmlWriter writer = XmlWriter.Create(Application.dataPath + "/Resources/" + inputText.text + ".xml"))
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("Units");

            foreach (Point point in board.Keys)
            {
                if ((playerTurn == 0 && point.y > 4))
                    break;
                else if ((playerTurn == 1 && point.y < gridSize.y - 5))
                    continue;
                if (((GameObject)board[point]).GetComponent<HexTile>().unit != null) {
                    HexTile hT = ((GameObject)board[point]).GetComponent<HexTile>();
                    writer.WriteStartElement("Unit");

                    writer.WriteElementString("rank", "" + hT.unit.GetComponent<BasicUnit>().rank);
                    if (playerTurn == 0)
                    {
                        writer.WriteElementString("positionX", "" + hT.boardPosition.x);
                        writer.WriteElementString("positionY", "" + hT.boardPosition.y);
                    } else {
                        writer.WriteElementString("positionX", "" + Mathf.Abs((gridSize.x + Mathf.Abs(gridSize.y - hT.boardPosition.y - 1) - 1 - hT.boardPosition.x)));
                        writer.WriteElementString("positionY", "" + Mathf.Abs(gridSize.y - 1 - hT.boardPosition.y));
                    }
                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
        }
    }

    public void addAvailableUnit(int rank)
    {
        layoutHexTiles[rank + 1].GetComponent<LayoutHexTile>().changeAvailableUnits(1);
    }

    void destroyLayoutHexTiles()
    {
        for (int k = 0; k < layoutHexTiles.Length; k++)
        {
            layoutHexTiles[k].GetComponent<LayoutHexTile>().destroy();
            Destroy(layoutHexTiles[k]);
        }
    }

    void prepareGame()
    {
        GameObject.Find("InputField").SetActive(false);
        GameObject.Find("LoadLayoutButton").SetActive(false);
        GameObject.Find("SaveLayoutButton").SetActive(false);
        GameObject.Find("StartGameButton").SetActive(false);
        foreach(GameObject o in GUIObjects) {
            o.SetActive(true);
        }
        editMode = false;
        numberOfActions = 2;
        numberOfActionsText.text = "Number of actions: " + numberOfActions;
        for (int j = 0; j < gridSize.y; j++)
        {
            for (int i = 0; i < gridSize.x; i++)
            {
                HexTile tile = ((GameObject)board[new Point(i, j)]).GetComponent<HexTile>();
                if (tile.unit != null)
                {
                    if (tile.unit.GetComponent<BasicUnit>().side == playerTurn)
                    {
                        tile.unit.GetComponent<BasicUnit>().unhideUnit();
                        tile.highlightUnitTile();
                    }
                    else
                    {
                        tile.unit.GetComponent<BasicUnit>().hideUnit();
                        tile.unHighlightUnitTile();
                    }
                }
            }
        }
    }

    public bool allUnitsSet()
    {
        for (int k = 0; k < layoutHexTiles.Length; k++)
        {
            if (layoutHexTiles[k].GetComponent<LayoutHexTile>().numberOfAvailableUnits > 0)
                return false;
        }
        return true;
    }

    public void checkAvailableLayoutUnits()
    {
        if (!allUnitsSet())
            return;
        if (playerTurn == 0)
        {
            NextPlayerButton.SetActive(true);
        } else {
            StartGameButton.SetActive(true);
        }
    }

    public void startGame()
    {
        if (!allUnitsSet())
            return;
        playerCameraPosition1 = (playerTurn == 0 ? cam.transform.position : playerCameraPosition1);
        playerCameraPosition2 = (playerTurn == 1 ? cam.transform.position : playerCameraPosition2);
        playerCameraRotation1 = (playerTurn == 0 ? cam.transform.localRotation : playerCameraRotation1);
        playerCameraRotation2 = (playerTurn == 1 ? cam.transform.localRotation : playerCameraRotation2);
        playerTurn = (playerTurn == 0 ? 1 : 0);
        cam.GetComponent<cameraMovement>().reset();
        cam.transform.position = (playerTurn == 0 ? playerCameraPosition1 : playerCameraPosition2);
        cam.transform.localRotation = (playerTurn == 0 ? playerCameraRotation1 : playerCameraRotation2);
        if (playerTurn == 1)
        {
            destroyLayoutHexTiles();
            createLayoutHexTiles();
        }
        else
        {
            destroyLayoutHexTiles();
            prepareGame();
        }
    }

    // Use this for initialization
    void Start () {
        foreach (GameObject o in GUIObjects)
        {
            o.SetActive(false);
        }
        layoutHexTiles = new GameObject[unitObject.Length];
        inicialization();
        createGrid();
        createLayoutHexTiles();
        playerCameraPosition1 = cam.transform.position;
        playerCameraPosition2 = new Vector3(9f, 5f, 33.5f);
        playerCameraRotation1 = cam.transform.localRotation;
        playerCameraRotation2 = Quaternion.Euler(30, 180, 0);
        numberOfActions = 0;
        numberOfActionsText.text = "Number of actions: " + numberOfActions;
        //Invoke("initGame", 0.1f);
    }

    // Update is called once per frame
    void Update () {
	
	}
}
