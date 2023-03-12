using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SquadSettingsScriptableObject;


/// <summary>
/// The squad manager manages all the character and set them at the right place.
/// It also plays the role of a game manager.
/// </summary>
public class SquadManager : MonoBehaviour
{
    //Making the Suad manager to be accessible from everywhere
    public static SquadManager instance;

    public List<ScriptableObjectClass> listClassCharacters;
    List<float> abilityCooldownModifier = new List<float>();

    public SquadSettingsScriptableObject squadSettings;

    public SquadManagerControl squadManagerControl;
    public UIManager uiManager;


    List<List<Character>> charactersOfEachType;

    float radius;
    public int nbCharacters = 0;

    public Transform characterParent;

    public float timeGame;

    bool isOver = false;
    int nbCoin = 0;

    Vector3 prevPosition;
    void Awake()
    {
        instance = this;
        timeGame = 0f;
        InitSquad();
    }

    /// <summary>
    /// Initialize the squad with the starting composition holds in the settings scriptable object.
    /// </summary>
    private void InitSquad()
    {
        //define the starting radius.
        radius = squadSettings.sizeSquad0Unit;

        //Initialize the lists.
        charactersOfEachType = new List<List<Character>>();
        for(int i=0; i< listClassCharacters.Count; i++)
        {
            charactersOfEachType.Add(new List<Character>());
            abilityCooldownModifier.Add(1f);
        }

        //For each starting composition settings, add the right number of character of each class.
        foreach(StartingCompo startCompo in squadSettings.startingCompos)
        {
            for(int i=0; i< listClassCharacters.Count; i++)
            {
                if(listClassCharacters[i].characterClass == startCompo.characterclass)
                {
                    AddCharacterType(i, startCompo.nbUnits, false);
                }
            }
        }

        //Update the radius and the position of each character.
        UpdateSquadRadiusAndOrder();
    }


    void LateUpdate()
    {
        if (isOver) return;

        //Rotate on itself the squad.
        characterParent.Rotate(squadSettings.rotationSpeed * Time.deltaTime * Vector3.up);

        //Save the previous position, used to check if the squad crossed a gate.
        prevPosition = this.transform.position;

        //Move the Squad with the control of joystick.
        DoMovementSquad();

        //Update the timer.
        timeGame += Time.deltaTime;
        uiManager.UpdateTime(timeGame);
        
        //Check the game over conditions.
        if (timeGame > squadSettings.timeToSurviveToWin)
        {
            uiManager.ShowWin();
            isOver = true;
        }
        else if (nbCharacters <= 0)
        {
            isOver = true;
            uiManager.ShowLose();
        }

    }

    /// <summary>
    /// Using the joystick information control, the squad moves and check obstacles on the map.
    /// </summary>
    public void DoMovementSquad()
    {
        //Getting the information of the joystick, orientation and magnitude.
        Vector2 directionMove = squadManagerControl.GetInputJoystickDirection();
        float magnitudeMove = squadManagerControl.GetInputJoystickMagnitude() * squadSettings.squadMoveSpeed * Time.deltaTime;

        //Move the squad like if no obstacle.
        this.transform.position += magnitudeMove * new Vector3(directionMove.x, 0f, directionMove.y);


        //Check all around the squad by ray casting in lot of directions to check if an obstacle is too close.
        int layerMask = 1 << 6;

        //In case no movement, we check all around the squad and not only in front of it.
        float angleMaxToCheck = 135f;
        if(directionMove.sqrMagnitude<0.01f)
        {
            angleMaxToCheck = 180f;
            directionMove.x = 1f;
        }

        float angleToCheckAround = 0f;
        float stepAngle = 20f;
        //Check every angle around the squad.
        while (angleToCheckAround <= angleMaxToCheck)
        {
            Vector3 direction = Quaternion.Euler(angleToCheckAround * Vector3.up) * new Vector3(directionMove.x, 0, directionMove.y);

           RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, (this.radius + squadSettings.extraRadiusForObstacle), layerMask))
            {
                //If a ray touches, it means that the squad is too close from this obstacle, so we move the squad away from it.
                this.transform.position += (hit.distance - (this.radius + squadSettings.extraRadiusForObstacle)) * direction;
 
            }
            angleToCheckAround *= -1;
            if (angleToCheckAround<=0)
            {
                angleToCheckAround -= stepAngle;
            }

        }
    }

    /// <summary>
    /// Called by the gate when crossing a gate with Cooldown Modifier, it will update the cooldown modifier of the good class.
    /// </summary>
    public void ChangeCooldownModifier(CharacterClass characterClass, int valueModifier)
    {
        for (int i = 0; i < listClassCharacters.Count; i++)
        {
            if (listClassCharacters[i].characterClass == characterClass)
            {
                //Update the cooldown modifier
                abilityCooldownModifier[i] -= valueModifier / 100f;
                abilityCooldownModifier[i] = Mathf.Max(0.1f, abilityCooldownModifier[i]);

                //Update the cooldown modifier for every character of this class.
                for (int j = 0; j < charactersOfEachType[i].Count; j++)
                {
                    charactersOfEachType[i][j].SetCooldownModifier(abilityCooldownModifier[i]);
                }
            }
        }
    }

    /// <summary>
    /// Called by the gate when crossing a gate with Change_Number, it will add or remove character of the right class.
    /// </summary>
    public void AddCharacterGate(CharacterClass characterClass, int nb)
    {
        for (int i = 0; i < listClassCharacters.Count; i++)
        {
            if (listClassCharacters[i].characterClass == characterClass)
            {
                //Check if we should add or kill a character.
                if (nb >= 0) AddCharacterType(i, nb);
                else KillCharacterType(i, Mathf.Abs(nb));
            }
        }
    }

    /// <summary>
    /// Adds nb character of the chosen class to the squad, if updateSquad is true, it will update the radius and position of the characters in the squad.
    /// </summary>
    public void AddCharacterType(int listCharacterId, int nb, bool updateSquad = true)
    {
        nbCharacters += nb;
        radius += nb * squadSettings.squadGrowth;

        for(int i=0; i<nb; i++)
        {
            //We create the prefab of the character
            GameObject newCharacterGO = GameObject.Instantiate(listClassCharacters[listCharacterId].prefabCharacter, characterParent);
            //we add the right script to it depending of the class.
            Character newCharacter = FactoryCharacter.AddComponentCharacter(newCharacterGO, listClassCharacters[listCharacterId].characterClass);
            newCharacter.settingsCharacter = listClassCharacters[listCharacterId];
            newCharacter.Init();
            //We set the right cooldown modifier for this class.
            newCharacter.SetCooldownModifier(abilityCooldownModifier[listCharacterId]);

            charactersOfEachType[listCharacterId].Add(newCharacter);
        }

        //If true, update the squad raidus and the position of the characters.
        if (updateSquad)
        {
            UpdateSquadRadiusAndOrder();
        }
    }

    /// <summary>
    /// Kills nb character of the chosen class to the squad, the characetr killed are in priority the ones with the less health point.
    /// /// </summary>
    public void KillCharacterType(int listCharacterId, int nb, bool updateSquad = true)
    {
        

        while(charactersOfEachType[listCharacterId].Count>0 && nb>0)
        {
            //Find the character with the less health point.
            float minHP = charactersOfEachType[listCharacterId][0].GetHealthPoint();
            Character characterToKill = charactersOfEachType[listCharacterId][0];
            
            for(int i=0; i< charactersOfEachType[listCharacterId].Count; i++)
            {
                float hpCharacter = charactersOfEachType[listCharacterId][i].GetHealthPoint();
                if (hpCharacter < minHP)
                {
                    minHP = hpCharacter;
                    characterToKill = charactersOfEachType[listCharacterId][i];
                }
            }

            nb--;
            //We remove the character from the squad
            RemoveCharacterFromSquad(characterToKill, false);

            

        }

        //If true, update the squad raidus and the position of the characters.
        if (updateSquad)
        {
            UpdateSquadRadiusAndOrder();
        }
    }

    /// <summary>
    /// Method to remove fully a character from the squad and the list of the squad, also calls his death method.
    /// </summary>
    public void RemoveCharacterFromSquad(Character characterDead, bool updateSquad = true)
    {
        for(int i=0; i<listClassCharacters.Count; i++)
        {
            if(listClassCharacters[i].characterClass == characterDead.settingsCharacter.characterClass)
            {
                //Remove the characetr from the list.
                charactersOfEachType[i].Remove(characterDead);
            }
        }

        //Remove the character from the character container.
        characterDead.transform.SetParent(null);

        //Reduce the number of character and the radius of the squad.
        nbCharacters--;
        radius -= squadSettings.squadDecrease;

        //Calls the death method of the character.
        characterDead.Death();

        //If true, update the squad raidus and the position of the characters.
        if (updateSquad)
        {
            UpdateSquadRadiusAndOrder();
        }

    }


    /// <summary>
    /// Update the radius and the position of the characters and assuring that every character class are evenly spaced.
    /// </summary>
    public void UpdateSquadRadiusAndOrder()
    {
        int nbSpaceLeft = nbCharacters;
        List<Character> listCharacterInOrder = new List<Character>();

        for(int i=0; i< nbCharacters; i++)
        {
            listCharacterInOrder.Add(null);
        }

        bool[] alreadySelected = new bool[listClassCharacters.Count];

        
        //Place every character in the right order to assure a good space between every characetr of the same class.
        bool shouldUpdatePosition = true;

        while(shouldUpdatePosition)
        {
            int currentMaxValue = 0;
            int currentIdClass = -1;

            //Find the class with the most character inside that ar enot already placed.

            for (int i = 0; i < listClassCharacters.Count; i++)
            {
                if(charactersOfEachType[i].Count> currentMaxValue && !alreadySelected[i])
                {
                    currentMaxValue = charactersOfEachType[i].Count;
                    currentIdClass = i;
                }
            }

            if (currentIdClass != -1)
            {
                alreadySelected[currentIdClass] = true;

                //compute the good space between each character of this class.
                float spaceBetweenEachSpot = nbSpaceLeft / (float)(currentMaxValue);

                int currentPosition = -1;
                int idSpaceList = 0;

                //Place a character every spaceBetweenEachSpot empty spaces.
                for (int i = 0; i < charactersOfEachType[currentIdClass].Count; i++)
                {
                    while(currentPosition<i*(spaceBetweenEachSpot))
                    {
                        //Only counts the empty space.
                        if (listCharacterInOrder[idSpaceList] == null) currentPosition++;
                        idSpaceList++;
                    }
                    listCharacterInOrder[idSpaceList-1] = charactersOfEachType[currentIdClass][i];
                    nbSpaceLeft--;
                }
            }
            else
            {
                shouldUpdatePosition = false;
            }
        }


        //Knowing the order of the characters, position them in a circle way following this order.
        for(int i=0; i< listCharacterInOrder.Count; i++)
        {
            float angle = 2*Mathf.PI * i / ((float)nbCharacters);

            Character character = listCharacterInOrder[i];

            character.transform.localPosition = radius * new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) + character.transform.localPosition.y * Vector3.up;
            character.transform.localEulerAngles = new Vector3(0, 90f-angle * Mathf.Rad2Deg, 0);

        }


    }

    /// <summary>
    /// Return the list of the character in the range around the position.
    /// </summary>
    public List<Character> GetCharacterAround(Vector3 position, float range)
    {
        List<Character> listCharacterInRange = new List<Character>();
        float rangeSqr = range * range;

        //Loop on each class.
        for(int i=0; i< charactersOfEachType.Count; i++)
        {
            //Loop on each character of the class.
            for (int j = 0; j < charactersOfEachType[i].Count; j++)
            {
                Character character = charactersOfEachType[i][j];
                float distance = new Vector2(position.x - character.transform.position.x, position.z - character.transform.position.z).sqrMagnitude;
                //We check the squared distance to avoid using an expensive square root.
                if(distance<= rangeSqr)
                {
                    //Add the character in list that is in the range.
                    listCharacterInRange.Add(character);
                }
            }
        }

        return listCharacterInRange;
    }

    /// <summary>
    /// Return the radius of the squad
    /// </summary>
    public float GetRadiusSquad()
    {
        return this.radius;
    }

    /// <summary>
    /// Do damage to the closest character from the attacker position.
    /// </summary>
    public void DoDamageSquad(Vector3 positionAttacker, float damageValue)
    {
        //Find the closest character
        Character closestCharacter = null;
        float closestDistance = Mathf.Infinity;
        for (int i = 0; i < charactersOfEachType.Count; i++)
        {
            for (int j = 0; j < charactersOfEachType[i].Count; j++)
            {
                Character character = charactersOfEachType[i][j];
                float distance = new Vector2(positionAttacker.x - character.transform.position.x, positionAttacker.z - character.transform.position.z).sqrMagnitude;
                if (distance <= closestDistance* closestDistance)
                {
                    closestDistance = distance;
                    closestCharacter = character;
                }
            }
        }

        //Do damage to the closest character.
        if(closestCharacter!=null)
        {
            closestCharacter.TakeDamage(damageValue);
        }
    }

    /// <summary>
    /// Return if the game is over.
    /// </summary>
    public bool IsGameOver()
    {
        return isOver;
    }

    /// <summary>
    /// Called by a weaponCollectable, this method changes the weapon of each character holding a weapon.
    /// </summary>
    public void ChangeWeapon(WeaponScriptableObject newWeapon)
    {
        //Loop on each class.
        for (int i = 0; i < charactersOfEachType.Count; i++)
        {
            //Loop on each character.
            for (int j = 0; j < charactersOfEachType[i].Count; j++)
            {
                Character character = charactersOfEachType[i][j];
                //Change the wepaon of the character by the new one.
                character.ChangeWeapon(newWeapon);
            }
        }
    }

    /// <summary>
    /// Add a coin to the coin number
    /// </summary>
    public void AddCoin(int nbCoinToAdd)
    {
        nbCoin += nbCoinToAdd;
        uiManager.UpdateCoin(nbCoin);
    }

    /// <summary>
    /// Return the position of the squa the previous frame.
    /// </summary>
    public Vector3 GetPrevPosition()
    {
        return prevPosition;
    }
}
