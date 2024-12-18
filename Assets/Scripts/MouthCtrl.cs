using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Core;

public class MouthCtrl : MonoBehaviour
{
    public CubismModel model;
    private CubismParameter mouthOpenY;
    private CubismParameter mouthForm;
    // Start is called before the first frame update
    void Start()
    {
        model = GetComponent<CubismModel>();
        mouthOpenY = model.Parameters.FindById("PARAM_MOUTH_OPEN_Y");
        mouthForm = model.Parameters.FindById("PARAM_MOUTH_FORM");
    }

    // Update is called once per frame
    void Update()
    {
        if (mouthOpenY != null)
        {
            // Increase the value dynamically
            mouthOpenY.Value = Mathf.Clamp(mouthOpenY.Value * 1.5f, 0f, 1.5f); // Adjust multiplier as needed
            mouthForm.Value = Mathf.Lerp(0f, 1f, mouthOpenY.Value);
        }
    }
}
