using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class path
{
    string direction;
    float length, tileSize;
    GameObject thisPath;
    Vector3 scaleAsVector;
    float makeUpForGaps;
    public path(GameObject pathObject, Vector3 pos, Transform parent, float len, string dir, float TileSize)
    {
        //direction is either: posX, negX, posZ, or negZ  this determines the direction of the path
        direction = dir;
        length = len;
        tileSize = TileSize;
        thisPath = GameObject.Instantiate(pathObject, pos * (tileSize + 0.5f), new Quaternion(0, 0, 0, 0), parent);
        setSizeAndScale();
    }

    public void setSizeAndScale()
    {
        switch (direction)
        {
            case ("posZ"):
                scaleAsVector = new Vector3(1, 1 / tileSize, length);
                //you cant only multiply the z value by something, has to be the entire vector, so im making it so when y is multiplied by tilesize they come out to be 1
                makeUpForGaps = (length * 0.5f) - 0.5f; //   this ensures the path segment is of the correct length
                thisPath.transform.localScale = scaleAsVector * tileSize;
                thisPath.transform.localScale += new Vector3(0f, 0f, makeUpForGaps); //makes up for the gaps inbetween grid tiles
                thisPath.transform.position += new Vector3(0f, 0f, length + (makeUpForGaps/2) +(length -2));
                //each time you increase the scale, you need to adjust the position accordingly by using scale increase / 2. 
                break;

            case ("negZ"):
                scaleAsVector = new Vector3(1, 1 / tileSize, length);
                makeUpForGaps = (length * 0.5f) - 0.5f;
                thisPath.transform.localScale = scaleAsVector * tileSize;
                thisPath.transform.localScale += new Vector3(0f, 0f, makeUpForGaps);
                thisPath.transform.position += new Vector3(0f, 0f, -(length + (makeUpForGaps / 2) + (length - 2)));
                break;

            case ("posX"):
                scaleAsVector = new Vector3(length, 1 / tileSize, 1); //swaps all values into the x values instead of the z values
                makeUpForGaps = (length * 0.5f) - 0.5f;
                thisPath.transform.localScale = scaleAsVector * tileSize;
                thisPath.transform.localScale += new Vector3(makeUpForGaps, 0f, 0f);
                thisPath.transform.position += new Vector3(length + (makeUpForGaps / 2) + (length - 2), 0f, 0f);
                break;
            case ("negX"):
                scaleAsVector = new Vector3(length, 1 / tileSize, 1);
                makeUpForGaps = (length * 0.5f) - 0.5f;
                thisPath.transform.localScale = scaleAsVector * tileSize;
                thisPath.transform.localScale += new Vector3(makeUpForGaps, 0f, 0f);
                thisPath.transform.position += new Vector3(-(length + (makeUpForGaps / 2) + (length - 2)), 0f, 0f);
                break;

            default:
                Debug.LogError("Invalid direction in SetSizeAndScale()");
                break;
        }
    }
}
