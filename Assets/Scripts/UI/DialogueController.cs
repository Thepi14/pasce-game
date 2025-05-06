using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueController : MonoBehaviour
{
    public TextMeshProUGUI NPCName;
    public TextMeshProUGUI NPCDialogue;
    private GameObject canvas;
    private GameObject captionSpace;
    private GameObject captionImage;
    private GameObject choiceSpace;
    public GameObject[] choices;

    private GameObject player;

    public DialogueText dialogue_Text;
    public DialogueChoice dialogue_Choice;

    public float typeSpeed;

    public Queue<string> paragraphs = new Queue<string>();
    public int currentIndex = 0;
    public bool conversationEnded = true;
    public bool isChatting;
    public bool isChoicing;
    public bool isTyping;

    public bool endedIndex = false;
    public string currentNPC = "";

    public bool choiceReturned = false;
    public string currentChoice = "";

    private const string HTML_ALPHA = "<color=#00000000>";
    private const float MAX_TYPE_TIME = 0.1f;

    public Coroutine typeDialogueCoroutine;

    public string p;

    public void Start()
    {
        player = GameObject.Find("Player");
        canvas = GameObject.Find("Canvas");
        captionSpace = canvas.transform.Find("Dialogue").gameObject;

        NPCDialogue = captionSpace.transform.Find("NPCDialogueSpace").gameObject.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
        captionImage = captionSpace.transform.Find("NPCSpace").gameObject;
        NPCName = captionImage.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>();

        choiceSpace = canvas.transform.Find("ChoiceTab").gameObject;

        choices = new GameObject[3];
        for (int i = 0; i < choices.Length; i++)
        {
            choices[i] = choiceSpace.transform.Find(i + "Choice").gameObject;
        }
        for (int i = 0; i < choices.Length; i++)
        {
            int i2 = i + 0;
            choices[i].GetComponent<Button>().onClick.AddListener(delegate{ ReturnChoice(i2); });
        }
    }
    public void ReturnChoice(int index)
    {
        currentChoice = choices[index].name;
        isChoicing = false;
        choiceReturned = true;
        endedIndex = true;
        choiceSpace.SetActive(false);
    }
    public void ConvertChoice(int i, string value)
    {
        choices[i].name = value;
        choices[i].transform.Find("text").gameObject.GetComponent<TextMeshProUGUI>().text = value;
    }
    public void DisplayChoices(int m)
    {
        Debug.Log(m);
        for (int i = 0; i < choices.Length; i++)
        {
            Debug.Log("i:" + i);
            if (i < m)
            {
                Debug.Log("true");
                choices[i].SetActive(true);
            }
            else
            {
                Debug.Log("false");
                choices[i].SetActive(false);
            }
        }
    }
    public void DisplayNextParagraph(DialogueChoice dialogueText)
    {
        dialogue_Choice = dialogueText;

        isChatting = false;
        isChoicing = true;

        choiceSpace.transform.Find("text").GetComponent<TextMeshProUGUI>().text = dialogueText.choiceText;

        choiceSpace.SetActive(true);

        DisplayChoices(dialogueText.choices.Length);

        for (int i = 0; i < dialogueText.choices.Length; i++)
        {
            ConvertChoice(i, dialogueText.choices[i]);
        }

        endedIndex = (conversationEnded && isTyping == false) || (choiceReturned);
    }
    public void DisplayNextParagraph(DialogueText dialogueText)
    {
        dialogue_Text = dialogueText;
        isChoicing = false;

        choiceSpace.SetActive(false);

        if (paragraphs.Count == 0)
        {
            if (conversationEnded)
            {
                isChatting = true;
                StartConversation(dialogueText);
                endedIndex = false;
            }
            else if (!isTyping && !conversationEnded)
            {
                isChatting = false;
                EndConversation();
            }
        }
        if (!isTyping)
        {
            if (paragraphs.Count > 0)
            {
                p = paragraphs.Dequeue();
                typeDialogueCoroutine = StartCoroutine(TypeDialogueText(p));
            }
        }
        else
        {
            FinishParagraphEarly();
        }

        //NPCDialogue.text = p;

        /*if (paragraphs.Count == 0)
        {
            conversationEnded = true;
        }*/

        endedIndex = (conversationEnded && isTyping == false) || (choiceReturned);
    }

    public void Update()
    {
        
    }

    private IEnumerator TypeDialogueText(string p)
    {
        currentIndex++;
        if (currentIndex < dialogue_Text.speakerImage.Length)
            captionImage.GetComponent<Image>().sprite = dialogue_Text.speakerImage[currentIndex];

        NPCName.text = dialogue_Text.speakerName[0];
        isTyping = true;

        NPCDialogue.text = "";

        string originalText = p;
        string displayedText = "";
        int alphaIndex = 0;

        foreach(char c in p.ToCharArray())
        {
            alphaIndex++;
            NPCDialogue.text = originalText;

            displayedText = NPCDialogue.text.Insert(alphaIndex, HTML_ALPHA);
            NPCDialogue.text = displayedText;

            yield return new WaitForSeconds(MAX_TYPE_TIME / typeSpeed);
        }

        isTyping = false;
    }

    public void FinishParagraphEarly()
    {
        StopCoroutine(typeDialogueCoroutine);

        NPCDialogue.text = p;

        isTyping = false;
    }

    public void StartConversation(DialogueText dialogueText)
    {
        currentIndex = 0;
        if (currentIndex < dialogue_Text.speakerImage.Length)
            captionImage.GetComponent<Image>().sprite = dialogue_Text.speakerImage[currentIndex];

        for (int i = 0; i < dialogueText.paragraphs.Length; i++)
        {
            paragraphs.Enqueue(dialogueText.paragraphs[i]);
        }
        conversationEnded = false;
    }
    public void EndConversation()
    {
        //currentIndex = 0;
        NPCName.text = "";
        NPCDialogue.text = "";
        paragraphs.Clear();
        conversationEnded = true;
    }
}
