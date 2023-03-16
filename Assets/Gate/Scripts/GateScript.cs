using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for the gate
/// </summary>
public class GateScript : MonoBehaviour
{
    //Values to change the visual of the gate in 3D.
    public float widthGate;
    public float heightGate;
    public float sizePole;


    public Transform modelContainer;
    public Transform infoBannerContainer;

    public Material[] positiveEffectMaterials;
    public Material[] negativeEffectMaterials;

    public enum EffectTypeGate
    {
        CHANGE_NUMBER, COOLDOWN_REDUCTION
    }
    [Tooltip("CHANGE_NUMBER will add or decrease the number of character of the choosen class\nCOOLDOWN_REDUCTION will decrease or increase the cooldown ability of the choosen class")]
    public EffectTypeGate effectType;
    public CharacterClass characterClassEffect;

    [Tooltip("A positive value will be a positive effect, for CHANGE_NUMBER : an add of character and for COOLDOWN_REDUCTION a cooldown reduce")]
    public int valueEffect;

    public Vector3 p1Gate;
    public Vector3 p2Gate;

    void Start()
    {
        p1Gate = this.transform.position + this.transform.right * widthGate * 0.5f;
        p2Gate = this.transform.position - this.transform.right * widthGate * 0.5f;
        UpdateVisual();
    }

    void Update()
    {
        foreach (SquadManager squad in EntityManager.instance.GetListAllSquads())
        {
            
            //Check if the squad crossed the gate.
            if (CheckIfCrossed(squad.GetPrevPosition(), squad.transform.position))
            {
                //Do the effect of the gate.
                DoEffect(squad);
                //Destry the gate and avoid that the effect is made more than 1 time.
                Destroy(this.gameObject);
            }
        }
       
    }

    /// <summary>
    /// Using the settings of the gate, do the effect associated.
    /// </summary>
    public void DoEffect(SquadManager squad)
    {
        if(effectType==EffectTypeGate.CHANGE_NUMBER)
        {
            squad.AddCharacterGate(characterClassEffect, valueEffect);
        }
        else if (effectType == EffectTypeGate.COOLDOWN_REDUCTION)
        {
            squad.ChangeCooldownModifier(characterClassEffect, valueEffect);
        }
    }

    /// <summary>
    /// Using the previous and the current position of the squad, return true if the squad crossed the gate.
    /// </summary>
    public bool CheckIfCrossed(Vector3 prevPosition, Vector3 positionNow)
    {
        if (prevPosition == positionNow) return false;

        float s1_x, s1_y, s2_x, s2_y;
        s1_x = p2Gate.x - p1Gate.x; 
        s1_y = p2Gate.z - p1Gate.z;

        s2_x = positionNow.x - prevPosition.x; 
        s2_y = positionNow.z - prevPosition.z;

        float s, t;
        s = (-s1_y * (p1Gate.x - prevPosition.x) + s1_x * (p1Gate.z - prevPosition.z)) / (-s2_x * s1_y + s1_x * s2_y);
        t = (s2_x * (p1Gate.z - prevPosition.z) - s2_y * (p1Gate.x - prevPosition.x)) / (-s2_x * s1_y + s1_x * s2_y);

        if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
        {
            
            return true;
        }

        return false;
    }

    //Allow to see how the gate will look like in editor.
    public void OnDrawGizmos()
    {
        UpdateVisual();

    }

    /// <summary>
    /// Update the 3D visual of the gate using the size specified and the type of effect.
    /// </summary>
    public void UpdateVisual()
    {
        //Adapt the poles and the banner with the size specified.
        modelContainer.GetChild(0).transform.localPosition = 0.5f * new Vector3(-widthGate + sizePole, heightGate);
        modelContainer.GetChild(0).transform.localScale = new Vector3(sizePole, heightGate, sizePole);
        modelContainer.GetChild(1).transform.localPosition = 0.5f * new Vector3(widthGate - sizePole, heightGate);
        modelContainer.GetChild(1).transform.localScale = new Vector3(sizePole, heightGate, sizePole);
        modelContainer.GetChild(2).transform.localPosition = new Vector3(0, heightGate * 0.5f, 0);
        modelContainer.GetChild(2).GetChild(0).transform.localScale = new Vector3(widthGate - 2 * sizePole, heightGate * 0.8f, 0.2f);
        modelContainer.GetChild(2).GetChild(1).transform.localScale = Mathf.Min(widthGate / 2f, heightGate) * Vector3.one;


        //If the effect is positive, show the positive material else the negative material.
        modelContainer.GetChild(0).GetComponent<Renderer>().material = valueEffect >= 0 ? positiveEffectMaterials[1] : negativeEffectMaterials[1];
        modelContainer.GetChild(1).GetComponent<Renderer>().material = valueEffect >= 0 ? positiveEffectMaterials[1] : negativeEffectMaterials[1];
        modelContainer.GetChild(2).GetChild(0).GetComponent<Renderer>().material = valueEffect >= 0 ? positiveEffectMaterials[0] : negativeEffectMaterials[0];


        //Display the correct information on the gate effect.
        for (int i = 0; i < infoBannerContainer.childCount; i++)
        {
            Transform effectCategory = infoBannerContainer.GetChild(i);
            effectCategory.gameObject.SetActive(i == (int)(effectType));

            if (i == (int)(effectType))
            {
                for (int j = 0; j < effectCategory.childCount; j++)
                {
                    Transform effectCategoryClass = effectCategory.GetChild(j);
                    effectCategoryClass.gameObject.SetActive(j == (int)(characterClassEffect));

                    if (j == (int)(characterClassEffect))
                    {


                        effectCategoryClass.GetChild(0).gameObject.SetActive(valueEffect >= 0);
                        effectCategoryClass.GetChild(1).gameObject.SetActive(valueEffect < 0);

                        int activeInfo = valueEffect >= 0 ? 0 : 1;

                        string textValue = Mathf.Abs(valueEffect) + "";
                        if (effectType == EffectTypeGate.COOLDOWN_REDUCTION) textValue = Mathf.Abs(valueEffect) + "%";

                        effectCategoryClass.GetChild(activeInfo).GetChild(0).GetComponent<TMPro.TextMeshPro>().text = textValue;
                    }

                }
            }
        }
    }


}
