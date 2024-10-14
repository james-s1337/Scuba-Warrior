using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeapMove : MonoBehaviour
{
    [SerializeField] Transform weap;
    [SerializeField] SpriteRenderer weapSprite;
    Sprite defaultSprite;
    [SerializeField] float moveY;

    void Start()
    {
        defaultSprite = weapSprite.sprite;
    }

    public void moveWeapY()
    {
        weap.localPosition += new Vector3(0, moveY, 0);
    }

    public void resetWeapPos()
    {
        weap.localPosition -= new Vector3(0, moveY, 0);
        enableSprite();
    }

    public void disableSprite()
    {
        weapSprite.sprite = null;
    }

    public void enableSprite()
    {
        weapSprite.sprite = defaultSprite;
    }

    public void setNewSprite(Sprite newSprite)
    {
        defaultSprite = newSprite;
        weapSprite.sprite = defaultSprite;
    }
}
