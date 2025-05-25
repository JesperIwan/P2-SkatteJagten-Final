using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GarageManager : MonoBehaviour{

    //text to show which garage is selected
    public TMP_Text switcherText;
    private string[] garages = { "farve", "lyd", "bling" };
    //sprites for the buttons
    public Sprite[] colorSprites;
    public Sprite[] soundSprites;
    public Sprite[] charmSprites;
    public Sprite[] miniCharmSprites;

    //buttons for the garage switcher
    public Button[] switcherButtons;
    public Button[] selecterButtons;
    //gameobjects for the car, sound and charm
    public GameObject car;
    public GameObject sound;
    public GameObject charm;

    public static int currentColorIndex = 0;
    public static int currentSoundIndex = 0;
    public static int currentCharmIndex = 0;
    public GameObject Buttons;

    void Start() {
        // set the initial selected garage to the color garage
        SwitchGarage(garages[0]);
        // add listeners to the garage switcher buttons
        switcherButtons[0].onClick.AddListener(() => SwitchGarage(garages[0]));
        switcherButtons[1].onClick.AddListener(() => SwitchGarage(garages[1]));
        switcherButtons[2].onClick.AddListener(() => SwitchGarage(garages[2]));
        // set the car, sound and charm gameobjects
        car.GetComponent<SpriteRenderer>().sprite = colorSprites[currentColorIndex];
        sound.GetComponent<SpriteRenderer>().sprite = soundSprites[currentSoundIndex];
        charm.GetComponent<SpriteRenderer>().sprite = charmSprites[currentCharmIndex];
        //play current motor sound 

        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audioSource in audioSources) {
            // Stop each AudioSource
            audioSource.Stop();
        }

        AudioManager.Instance.PlaySFX("motor" + (currentSoundIndex + 1));
    }

    // function to switch between garages
    void SwitchGarage(string garage)
    {
        // set the garage switcher text to the selected garage
        switcherText.text = $"Vælg {garage}";

        foreach (Button btn in switcherButtons) {
            Color green   = new Color(0x57 / 255f, 0x8E / 255f, 0x7E / 255f); // Green
            Color beige = new Color(0xFA / 255f, 0xF7 / 255f, 0xED / 255f); // Beige
            // set the garage switcher button to the selected garage
            btn.GetComponentInChildren<TMP_Text>().color = green;
            btn.GetComponent<Image>().color = beige;
            Debug.Log("text" + btn.GetComponentInChildren<TMP_Text>().text.ToLower());
            Debug.Log("garage" + garage.ToLower());
            if (btn.GetComponentInChildren<TMP_Text>().text.ToLower() == garage.ToLower()) {
                btn.GetComponent<Image>().color = green;
                btn.GetComponentInChildren<TMP_Text>().color = beige;
            }
        }
        
        // changes the selecterButtons for the selsction wheel to the corrosponding sprites
        // based on the garage selected
        if (garage == "farve")
            {
            for (int i = 0; i < selecterButtons.Length; i++) {
                selecterButtons[i].GetComponent<Image>().sprite = colorSprites[i];
                int selectedColorIndex = i;
                // remove all listeners from the button and add a new listener to change the car color
                selecterButtons[i].onClick.RemoveAllListeners();
                selecterButtons[i].onClick.AddListener(() => ChangeCarColor(selectedColorIndex));
                //AudioManager.Instance.PlaySFX("Dyt1");

            }
        } else if (garage == "lyd")
            {
            for (int i = 0; i < selecterButtons.Length; i++) {

                selecterButtons[i].GetComponent<Image>().sprite = soundSprites[i];
                int selectedSoundIndex = i;
                // remove all listeners from the button and add a new listener to change the car sound
                selecterButtons[i].onClick.RemoveAllListeners();
                selecterButtons[i].onClick.AddListener(() => ChangeCarSound(selectedSoundIndex));
            }
        } else if (garage == "bling")
            {
            for (int i = 0; i < selecterButtons.Length; i++) {
                selecterButtons[i].GetComponent<Image>().sprite = miniCharmSprites[i];
                int selectedCharmIndex = i;
                // remove all listeners from the button and add a new listener to change the car charm
                selecterButtons[i].onClick.RemoveAllListeners();
                selecterButtons[i].onClick.AddListener(() => ChangeCarCharm(selectedCharmIndex));
            }
        }
    }
    
    // function to change the car color sprite
    void ChangeCarColor(int selectedColorIndex) {
        currentColorIndex = selectedColorIndex;
        car.GetComponent<SpriteRenderer>().sprite = colorSprites[currentColorIndex];
        AudioManager.Instance.PlaySFX("SpraySFX");
    }
    // function to change the car sound sprite
    void ChangeCarSound(int selectedSoundIndex) {
        currentSoundIndex = selectedSoundIndex;
        for (int i = 0; i < soundSprites.Length; i++) {
            if (i == currentSoundIndex) {
                AudioManager.Instance.PlaySFX("motor" + (i + 1));
            }
        }
        sound.GetComponent<SpriteRenderer>().sprite = soundSprites[currentSoundIndex];
    }
    // function to change the car charm sprite
    void ChangeCarCharm(int selectedCharmIndex) {
        currentCharmIndex = selectedCharmIndex;
        for (int i = 0; i < charmSprites.Length; i++) {
            if (i == currentCharmIndex) {
                AudioManager.Instance.PlaySFX("charm" + (i + 1));
            }
        }
        charm.GetComponent<SpriteRenderer>().sprite = charmSprites[currentCharmIndex];
    }
    
}