using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SquadSettingsScriptableObject", order = 3)]
public class SquadSettingsScriptableObject : ScriptableObject
{
    [Tooltip(("The movement speed of the squad in units per second."))]
    public float squadMoveSpeed;
    [Tooltip(("The rotation speed of the squad in degree per second."))]
    public float rotationSpeed;

    [Tooltip(("The size of the squad with 0 unit."))]
    public float sizeSquad0Unit;
    [Tooltip(("The extra radius for obstacle avoidance. "))]
    public float extraRadiusForObstacle = 0.75f;
    [Tooltip(("The increase radius value every time the squad number of character increases of 1."))]
    public float squadGrowth;
    [Tooltip(("The decrease radius value every time the squad number of character decrease of 1."))]
    public float squadDecrease;

    [Serializable]
    public class StartingCompo
    {
        [SerializeField]
        public CharacterClass characterclass;
        [SerializeField]
        public int nbUnits;
    }

    [Tooltip(("List of the starting squad, each element represents a type and a number of unit."))]
    public List<StartingCompo> startingCompos;

    [Tooltip(("The time needed to survive to win this level in seconds."))]
    public float timeToSurviveToWin;


}




