using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class poolList : MonoBehaviour
{
    public List<GameObject> objPool = new List<GameObject>();
    public GameObject defaultObj;

    public List<GameObject> list = new List<GameObject>();
    public List<Sequence> sqList = new List<Sequence>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            move();
        }
    }

    public void move()
    {
        foreach (var g in list) 
        {
            float posZ = g.transform.position.z - 3;
            Sequence sq = DOTween.Sequence();
            sq.AppendCallback(delegate
            {

                g.transform.DOMoveZ(posZ, .2f).OnUpdate(delegate
                {
                    if (g.transform.position.z <= -5)
                    {
                        ResetPos(g);
                        sq.Kill();
                    }
                });

            });
        }
    }

    public void ResetPos(GameObject t)
    {
        list.Remove(t);

        float z = list[list.Count - 1].transform.position.z + 10;
        t.transform.position = new Vector3(0, 0, z);
        list.Add(t);
    }

    public GameObject spawn()
    {
        GameObject obj = defaultObj;
        if (objPool != null && objPool.Count > 0)
        {
            obj = objPool[0];
            obj.SetActive(true);
            objPool.Remove(obj);
        }
        return obj;
    }

    public void ReCoverObj(GameObject obj)
    {
        objPool.Add(obj);
        obj.SetActive(false);
    }
}
