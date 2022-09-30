using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Belt : MonoBehaviour
{
    private static int _beltID = 0;

    public Belt beltInSequence;
    public BeltItem beltItem;
    public bool isSpaceTaken;

    private BeltManager _beltManager;

    private void Start()
    {
        _beltManager = FindObjectOfType<BeltManager>();
        beltInSequence = null;
        beltInSequence = FindNextBelt();
        gameObject.name = $"Belt: {_beltID++}";
    }

    private void Update()
    {
        if (beltInSequence == null)
            beltInSequence = FindNextBelt();

        if (beltItem != null && beltItem.item != null)
            StartCoroutine(StartBeltMove());
    }

    public Vector3 GetItemPosition()
    {
        var padding = 0.3f;
        var position = transform.position;
        return new Vector3(position.x, position.y + padding, position.z);
    }

    private IEnumerator StartBeltMove()
    {
        isSpaceTaken = true;

        if (beltItem.item != null && beltInSequence != null && beltInSequence.isSpaceTaken == false)
        {
            Vector3 toPosition = beltInSequence.GetItemPosition();

            beltInSequence.isSpaceTaken = true;

            var step = _beltManager.speed * Time.deltaTime;

            while (beltItem.item.transform.position != toPosition)
            {
                beltItem.item.transform.position = 
                    Vector3.MoveTowards(beltItem.transform.position, toPosition, step);

                yield return null;
            }

            isSpaceTaken = false;
            beltInSequence.beltItem = beltItem;
            beltItem = null;
        }
    }

    private Belt FindNextBelt()
    {
        Transform currentBeltTransform = transform;
        RaycastHit hit;

        var forward = transform.forward;

        Ray ray = new Ray(currentBeltTransform.position, forward);

        if (Physics.Raycast(ray, out hit, 1f))
        {
            Belt belt = hit.collider.GetComponent<Belt>();

            if (belt != null)
                return belt;
        }

        return null;
    }
}
