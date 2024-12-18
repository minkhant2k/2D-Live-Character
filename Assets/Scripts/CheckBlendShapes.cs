using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBlendShapes : MonoBehaviour
{
    // Start is called before the first frame update
    public SkinnedMeshRenderer smr;
    void Start()
    {
        smr = GetComponent<SkinnedMeshRenderer>();
        CheckingBlendShapes();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void CheckingBlendShapes()
    {

        if (smr != null)
        {
            int blendShapeCount = smr.sharedMesh.blendShapeCount;
            if (blendShapeCount > 0)
            {
                Debug.Log("BlendShapes found: " + blendShapeCount);
                for (int i = 0; i < blendShapeCount; i++)
                {
                    Debug.Log("BlendShape " + i + ": " + smr.sharedMesh.GetBlendShapeName(i));
                }
            }
            else
            {
                Debug.Log("No BlendShapes found on this model.");
            }
        }
        else
        {
            Debug.Log("No SkinnedMeshRenderer found on this GameObject.");
        }
    }

}
