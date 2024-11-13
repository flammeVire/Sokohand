using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] ResetButton resetButtonScript;
    public TextMeshProUGUI textActualCount, textGlobalCount,textBestCount;
    public Image Star, ActualLevel,Beer;
    public Sprite[] level;
    public Button resetButton;
    public GameObject ResetPanel;
    Coroutine Reseting;
    public GameObject[] LevelButton;

    [SerializeField] Canvas IngameCanva, MenuCanva;

    public void ChangeUI()
    {
        textActualCount.text = gameManager.ActualMove.ToString() + " / " + gameManager.MaxMove.ToString();
        textGlobalCount.text = gameManager.GlobalMove.ToString();
    }

    public void ResetButtonUI(bool isbutton)
    {
        if (isbutton)
        {
            Reseting = StartCoroutine(ResetButtonLoading());
        }
        else
        {
            StopCoroutine(Reseting);

            ResetPanel.GetComponent<Image>().fillAmount = 1;
        }
    }

    IEnumerator ResetButtonLoading()
    {

        if (ResetPanel.GetComponent<Image>().fillAmount <= 0.1f)
        {
            gameManager.LoadLevel(gameManager.LevelToLoad);
            ResetPanel.GetComponent<Image>().fillAmount = 1;
            StopCoroutine(Reseting);
        }
        else
        {
            ResetPanel.GetComponent<Image>().fillAmount -= 0.1f;
        }
        yield return new WaitForSeconds(0.1f);
        Reseting = StartCoroutine(ResetButtonLoading());

    }

    private void Update()
    {
        if (Input.GetButtonDown("ResetLevel")) 
        {
            ResetButtonUI(true);
        }
        if (Input.GetButtonUp("ResetLevel"))
        {
            ResetButtonUI(false);
        }
    }

    public void LevelText()
    {
        ActualLevel.sprite = level[gameManager.LevelToLoadIndex];
        textBestCount.text = gameManager.LevelToLoad.GoodJobMovement.ToString();
    }

    public void BadJobMovement()
    {
        Star.color = Color.red;
        Star.transform.localScale = new Vector3(1,-1,1);
    }
    public void ResetGoodJob()
    {
        Star.color = Color.green;
        Star.transform.localScale = new Vector3(1,1,1);
    }

    public void ApplyJobToMenu(int index,bool GoodJob)
    {
        
        LevelButton[index].GetComponent<Image>().sprite = Star.sprite;
        if (GoodJob) 
        {
            LevelButton[index].GetComponent<Image>().color = Color.green;
            LevelButton[index].transform.rotation = Quaternion.Euler(0, 0, 180);
            LevelButton[index].transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            LevelButton[index].GetComponent<Image>().color= Color.red;
            LevelButton[index].transform.rotation = Quaternion.Euler(0, 0, 180);
            LevelButton[index].transform.localScale = new Vector3(1,-1,1);
        }
    }

    public void ButtonLevelSelected(int level)
    {
        
        IngameCanva.gameObject.SetActive(true);
        MenuCanva.gameObject.SetActive(false);
        gameManager.LevelToLoad = gameManager.ChangeMapsByIndex(level);
        gameManager.LoadLevel(gameManager.LevelToLoad);
        gameManager.Running = true;
    }

    public void ContinueButton()
    {
        IngameCanva.gameObject.SetActive(true);
        MenuCanva.gameObject.SetActive(false);
        gameManager.Running = true;
        gameManager.LoadLevel(gameManager.LevelToLoad);
    }

    public void ReturnToMenu()
    {
        IngameCanva.gameObject.SetActive(false);
        gameManager.DestroyChild();
        gameManager.player.transform.position = new Vector3(100,100,0);
        MenuCanva.gameObject.SetActive(true);
        gameManager.Running = false;

    }

    public void ExitButton()
    {
        Application.Quit();
    }

    private void Start()
    {
        ReturnToMenu();
    }
}
