using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static WallGraphicsGenerator;

public class Malia : NPC
{
    private void OnValidate()
    {
        
    }
    void Start()
    {
        StartNPC();
        GetFolder();
        dialogueController = GAME.GetComponent<DialogueController>();
        SetCaptionName("Malia_interaction");
        SetNPCName("Malia_name");
    }
    void Update()
    {
        onChoice = dialogueController.isChatting;
        player.GetComponent<Player>().onNPCUI = onNPCChat;
        if (dialogueController.endedIndex == true && dialogueController.currentNPC == "Malia")
        {
            if (dialogueController.choiceReturned)
            {
                if (chatIndex == 1 && dialogueController.currentChoice == "Sim")
                {
                    dialogueController.currentChoice = "";
                    Item item = player.GetComponent<Player>().playerUI.inventoryUI.SellItems("Peixe Comum", 5);

                    if (item == null)
                    {
                        chatIndex = 3;
                    }
                    else
                    {
                        chatIndex++;
                    }
                    Interact();
                }
                else if (chatIndex == 1 && dialogueController.currentChoice == "Não")
                {
                    chatIndex--;
                    dialogueController.currentChoice = "";
                }
                else if (chatIndex == 3 && dialogueController.currentChoice == "Ok")
                {
                    chatIndex = 0;
                    dialogueController.currentChoice = "";
                }
                dialogueController.choiceReturned = false;
                dialogueController.isChoicing = false;
                onChoice = false;
            }
            else
            {
                switch (chatIndex)
                {
                    case 0:
                        chatIndex++;
                        Interact();
                        break;
                    case 1:
                        break;
                    case 2:
                        chatIndex = 0;
                        break;
                }
            }
            dialogueController.endedIndex = false;

        }
        SetDialogueActive(dialogueController.isChatting);
        //SetChoiceActive(dialogueController.isChoicing);
        if (IsWithinInteractDistance)
        {
            dialogueController.currentNPC = "Malia";
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Interact();
            }
            if (dialogueController.isChatting)
            {
                onChat = true;
                player.GetComponent<Player>().onNPCUI = onChat;
            }
            else if (!dialogueController.isChatting)
            {
                onChat = false;
                player.GetComponent<Player>().onNPCUI = onChat;
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            onChat = false;
            onChoice = false;
        }
        interactionButton.SetActive(IsWithinInteractDistance);
        if (IsWithinInteractDistance)
        {
            if (buttonCounter > 0.25f || buttonCounter < -0.2f)
                YdirButton = !YdirButton;
            multiButton = YdirButton ? 1 : -1;
            buttonCounter += Time.deltaTime * 0.17f * multiButton;
            interactionButton.transform.position = initialButtonPosition + new Vector2(0, buttonCounter);
        }
    }
    public override void Interact()
    {
        dialogueController.typeSpeed = 10;
        dialogueController.DisplayNextParagraph(chatList[chatIndex]);
    }
}
