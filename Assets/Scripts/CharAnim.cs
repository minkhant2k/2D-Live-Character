using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharAnim : MonoBehaviour
{
     private Animator charAnim;
     private Live2D.Cubism.Framework.Expression.CubismExpressionController expressionControl;
    // Start is called before the first frame update
    void Start()
    {
        charAnim = GetComponent<Animator>();
        expressionControl = GetComponent<Live2D.Cubism.Framework.Expression.CubismExpressionController>();
    }

    // Update is called once per frame
     void Update()
    { 
        CharacterMotion();
        FacialExpression();
    }

    void CharacterMotion()
    {
         if(Input.GetKeyDown(KeyCode.Q))
         {
            charAnim.SetTrigger("madTrigger");
         }else if(Input.GetKeyDown(KeyCode.W))
         {
            charAnim.SetTrigger("embarassedTrigger");
         }
    }

    void FacialExpression()
    {
         if(Input.GetKeyDown(KeyCode.Alpha0))
         {
            expressionControl.CurrentExpressionIndex = 0;

         }else if(Input.GetKeyDown(KeyCode.Alpha1))
         {
            expressionControl.CurrentExpressionIndex = 1;

         }else if(Input.GetKeyDown(KeyCode.Alpha2))
         {
            expressionControl.CurrentExpressionIndex = 2;
         }

    }
}
