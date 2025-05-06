using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class NPC : MonoBehaviour, IInteractable
{
    /// <summary>
    /// Pfv não suma.
    /// </summary>
    public dynamic chatList = new ArrayList();

    private Transform _playerTransform;
    public float maxDistance = 5f;
    public KeyCode interactKey;

    public DialogueController dialogueController;
    public DialogueText dialogueText;

    public GameObject canvas;
    public GameObject captionSpace;
    public GameObject choiceSpace;
    public GameObject player;

    public GameObject interactionButton;
    public GameObject interactionText;
    public GameObject interactionNPCName;

    public Vector2 initialButtonPosition;
    public float buttonCounter = 0;
    public int multiButton = 1;
    public bool YdirButton;
    public int chatIndex = 0;

    public bool onChat = false;
    public bool onChoice = false;
    public bool onNPCChat
    {
        get
        {
            return onChat || onChoice;
        }
    }

    public void GetFolder()
    {
        for (int i = 0; i < Resources.LoadAll("Texts/" + GetType().Name).Length; i++)
        {
            chatList.Add(Resources.LoadAll("Texts/" + GetType().Name)[i]);
        }
        for (int i = 0; i < chatList.Count; i++)
        {
            /*print(chatList[i].GetType());
            if (chatList[i] is DialogueText)
            {
                DialogueText reference = (DialogueText)chatList[i];
                chatList[i] = reference;
            }
            else if (chatList[i] is DialogueChoice)
            {
                DialogueChoice reference = (DialogueChoice)chatList[i];
                chatList[i] = reference;
            }*/
            if (chatList[i] is DialogueText)
                print(chatList[i].paragraphs[0]);
            else if (chatList[i] is DialogueChoice)
                print(chatList[i].choices[0]);
        }
    }
    public void StartNPC()
    {
        canvas = GameObject.Find("Canvas");
        captionSpace = canvas.transform.Find("Dialogue").gameObject;
        choiceSpace = canvas.transform.Find("ChoiceTab").gameObject;

        player = GameObject.Find("Player");

        interactionButton = transform.Find("Button").gameObject;
        initialButtonPosition = interactionButton.transform.position;

        _playerTransform = player.transform;

        interactKey = KeyCode.Q;

        interactionText = captionSpace.transform.Find("NPCDialogueSpace").gameObject.transform.Find("Text").gameObject;
        interactionNPCName = captionSpace.transform.Find("NPCSpace").transform.Find("Name").gameObject;
    }
    public void SetCaptionName(string name)
    {
        
    }
    public void SetNPCName(string name)
    {
        
    }
    public void SwitchDialogueState()
    {
        onChat = !onChat;
        captionSpace.SetActive(onChat);
    }
    public void SetDialogueActive(bool state)
    {
        onChat = state;
        captionSpace.SetActive(onChat);
    }
    public void SetChoiceActive(bool state)
    {
        onChoice = state;
        choiceSpace.SetActive(onChoice);
    }
    public abstract void Interact();

    public bool IsWithinInteractDistance
    {
        get
        {
            return Vector2.Distance(_playerTransform.position, transform.position) < maxDistance;
        }
        private set { }
    }


}
