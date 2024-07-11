using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class GameController : MonoBehaviour
{
    private CubePos nowCube = new CubePos(0, 1, 0);
    public float cubeChanhePlaseSpeef = 0.5f;
    private float camMoveToY, camSpeed = 2f;
    private List<Vector3> allCubesPositions = new List<Vector3>
    {
        new Vector3(0,0,0),
        new Vector3(1,0,0),
        new Vector3(-1,0,0),
        new Vector3(0,1,0),
        new Vector3(0,0,1),
        new Vector3(0,0,-1),
        new Vector3(1,0,1),
        new Vector3(-1,0,-1),
        new Vector3(-1,0,1),
        new Vector3(1,0,-1)
    };


    public Transform cubeToPlace;
    public GameObject cubeToCreate, allCubes;
    public GameObject[] canvasStartPage;
    public Color[] bgColors;

    private Rigidbody allCubesRB;
    private bool isLose, firstCube;
    private Coroutine showCubePlace;
    private Transform mainCam;
    private int prevCountMaxHor;
    private Color cameraColor;

    private void Start()
    {
        cameraColor = Camera.main.backgroundColor;
        mainCam = Camera.main.transform;
        camMoveToY = 5.5f + nowCube.y - 1f;

        allCubesRB = allCubes.GetComponent<Rigidbody>();
        showCubePlace = StartCoroutine(ShowCubePlace());
    }

    private void Update()
    {
        //проверка нажания на экран и постройка башни
        if((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && cubeToPlace != null && allCubes != null && !EventSystem.current.IsPointerOverGameObject())
        {
#if !UNITY_EDITOR
            if (Input.GetTouch(0).phase != TouchPhase.Began)
                return; 
#endif
            if (!firstCube)
            {
                firstCube = true;
                foreach(GameObject obj in canvasStartPage)
                    Destroy(obj);  
            }

            GameObject newCube = Instantiate(
                cubeToCreate,
                cubeToPlace.position,
                Quaternion.identity) as GameObject;

            newCube.transform.SetParent(allCubes.transform);
            nowCube.setVector(cubeToPlace.position);
            allCubesPositions.Add(nowCube.getVector());

            allCubesRB.isKinematic = true;
            allCubesRB.isKinematic = false;
            SpawnPosition();

            MoveCameraChangeBg();
        }

        //сделать при падении башни
        if(!isLose && allCubesRB.velocity.magnitude > 0.1f)
        {
            Destroy(cubeToPlace.gameObject);
            isLose = true;
            StopCoroutine(showCubePlace);
        }

        mainCam.localPosition = Vector3.MoveTowards(mainCam.localPosition, 
                                                    new Vector3(mainCam.localPosition.x, camMoveToY, mainCam.localPosition.z), 
                                                    camSpeed * Time.deltaTime);

        if(Camera.main.backgroundColor != cameraColor)
            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, cameraColor, Time.deltaTime/1.5f);

    }
    IEnumerator ShowCubePlace()
    {
        while (true)
        {
            SpawnPosition();
            yield return new WaitForSeconds(cubeChanhePlaseSpeef);
        }
    }
    private void SpawnPosition()
    {
        List<Vector3> positions = new List<Vector3>();
        if (IsPositionEmpty(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z)) && nowCube.x + 1 != cubeToPlace.position.x)
            positions.Add(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z)) && nowCube.x - 1 != cubeToPlace.position.x)
            positions.Add(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z)) && nowCube.y + 1 != cubeToPlace.position.y)
            positions.Add(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z)) && nowCube.y - 1 != cubeToPlace.position.y)
            positions.Add(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1)) && nowCube.z + 1 != cubeToPlace.position.z)
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1)) && nowCube.z - 1 != cubeToPlace.position.z)
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1));

        if (positions.Count > 0)
            cubeToPlace.position = positions[UnityEngine.Random.Range(0, positions.Count)];
        else if (positions.Count == 0)
            isLose = true;
        else
            cubeToPlace.position = positions[0];
    }
    private bool IsPositionEmpty(Vector3 targetPos)
    {
        if(targetPos.y==0) return false;
        foreach (Vector3 pos in allCubesPositions)
        {
            if (pos.x == targetPos.x && pos.y == targetPos.y && pos.z == targetPos.z)
                return false;
        }
        return true;
    }

    private void MoveCameraChangeBg()
    {
        int maxY=0, maxX=0, maxZ=0, maxHor = 0;

        foreach (Vector3 vec in allCubesPositions)
        {
            if (Mathf.Abs(Convert.ToInt32(vec.x)) > maxX)
                maxX = Convert.ToInt32(vec.x);

            if (Mathf.Abs(Convert.ToInt32(vec.y)) > maxY)
                maxY = Convert.ToInt32(vec.y);

            if (Mathf.Abs(Convert.ToInt32(vec.z)) > maxZ)
                maxZ = Convert.ToInt32(vec.z);
        }

        camMoveToY = 5.5f + nowCube.y - 1f;

        maxHor = maxX > maxZ ? maxX : maxZ;

        if(maxHor % 3 == 0 && prevCountMaxHor != maxHor)
        {
            mainCam.localPosition -= new Vector3(0, 0, 3f);
            prevCountMaxHor = maxHor;
        }

        if (maxY >= 7)
            cameraColor = bgColors[2];
        else if (maxY >= 5)
            cameraColor = bgColors[1];
        else
            cameraColor = bgColors[0];
    }


}

struct CubePos
{
    public int x, y, z;
    public CubePos(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public Vector3 getVector()
    {
        return new Vector3(x, y, z);
    }
    public void setVector(Vector3 pos) {
        x = Convert.ToInt32(pos.x);
        y = Convert.ToInt32(pos.y);
        z = Convert.ToInt32(pos.z);
    }
}