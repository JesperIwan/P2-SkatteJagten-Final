using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class QuizManager : MonoBehaviour {
    public DataStore data;
    public DifficultyAdjuster difficultyAdjuster;
    
    // The sprite for the pre-question information page
    public GameObject preQuestionSprite; 
    // The header for the pre-question information page
    public TMP_Text preQuestionHeader; 
    // The text for the pre-question information page
    public TMP_Text preQuestionText; 
    // The sprite for the question page
    public GameObject questionSprite; 
    // The header for the question page
    public TMP_Text questionHeader; 
    // The text for the question page
    public TMP_Text questionText; 
    // The button list for the answers
    public Button[] answerButtons; 
    // The continue button
    public Button continueButton; 
    //The button list for the end quiz page
    public Button[] EndButtons;
    // The header for the feedback page
    public TMP_Text feedbackHeader; 
    // The text for the feedback page
    public TMP_Text feedbackText;
    // The index of the selected answer
    private int currentAnswerIndex; 
    // The index of the current question
    private int currentQuestionIndex = 0;
    // The index of the current quiz
    public static int quizNumber = 0; // USE FOR SELECTING THE QUIZ - 1 for Forskudsopgørelse, 2 for Årsopgørelse
    // The current question based on the difficulty level 
    // Found in the method updateQuestionInfoBasedOnDifficulty()
    private Question currentQuestion;
    // dict of questions by difficulty
    //List of questions for the general introduction quiz
    Dictionary<Question.Difficulty, List<Question>> questionsByDifficulty;
    // Lists for easy, medium and hard questions
    private List<Question> easyQuestions; 
    private List<Question> mediumQuestions;
    private List<Question> hardQuestions;
    private List<Question> generalQuestions;
    // The current user level for the quiz
    // This is used for the Bayesian difficulty scaling
    [SerializeField] private Question.Difficulty currentUserLevel = Question.Difficulty.Hard; // USE FOR THE BAYESIAN DIFFICULTY SCALING - CHOSE EITHER EASY, MEDIUM OR HARD


    // Start the quiz
    void Start() {
        // Select chosen quiz based on the quizNumber
        ChooseQuiz(quizNumber);
        // Resets all conponents to not active
        SetAllComponentsNotActive();
        // Display the first information page
        DisplayNewInformation();
    }

    void Update() {
        ChangeTextColor();
        disableContinueButtonOnNoAnswer();
    }

    // method for displaying the new information page
    void DisplayNewInformation() {
        //set the right components to active
        preQuestionSprite.gameObject.SetActive(true);
        preQuestionHeader.gameObject.SetActive(true);
        preQuestionText.gameObject.SetActive(true);
        continueButton.gameObject.SetActive(true);

        // Ensure preQuestionSprite is assigned and has an Image component
        if (preQuestionSprite == null) {
            Debug.LogError("preQuestionSprite is not assigned in the Inspector!");
            return;
        }
        Image preQuestionImage = preQuestionSprite.GetComponent<Image>();
        if (preQuestionImage == null) {
            Debug.LogError("preQuestionSprite does not have an Image component!");
            return;
        }

        // Update the question based on the current user level
        // Set the current question based on the current question index for testing purposes
        //if (currentQuestionIndex == 0) {
        //    currentUserLevel  = Question.Difficulty.Easy;
        //} else if (currentQuestionIndex == 1) {
        //    currentUserLevel  = Question.Difficulty.Medium;
        //} else if (currentQuestionIndex == 2) {
        //    currentUserLevel  = Question.Difficulty.Hard;
        //}
        updateQuestionInfoBasedOnDifficulty(currentUserLevel);

      
        // use the sprite stored on the Question
        if (currentQuestion.preQuestionSprite == null) {
            Debug.LogError("The sprite for the current question is null!");
        } else {
            preQuestionImage.sprite = currentQuestion.preQuestionSprite;
        }
        preQuestionHeader.SetText(currentQuestion.preQuestionHeader);
        preQuestionText.SetText(currentQuestion.preQuestionText);


        // Position questionText in relation to the size of the header
        RectTransform preHeaderRect = preQuestionHeader.rectTransform; // Get the RectTransform of the header
        LayoutRebuilder.ForceRebuildLayoutImmediate(preHeaderRect); // Make sure the header’s layout is up-to-date
        float headerHeight = LayoutUtility.GetPreferredHeight(preHeaderRect); // Compute preferred height for the header
        preHeaderRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, headerHeight); // Set the height of the header
        RectTransform textRect = preQuestionText.rectTransform; // Get the RectTransform of the text
        float newY = preHeaderRect.anchoredPosition.y - (preHeaderRect.sizeDelta.y/2) - 50f - (textRect.sizeDelta.y/2); // Compute the y‐position: bottom of header minus 50px
        textRect.anchoredPosition = new Vector2(textRect.anchoredPosition.x, newY); // Apply it to the text’s anchored position

        // update the onClick event for the continue button
        continueButton.onClick.RemoveAllListeners();
        
        continueButton.onClick.AddListener(() => {
            if (currentQuestionIndex < questionsByDifficulty[currentUserLevel].Count) {
                SetAllComponentsNotActive(); 
                AudioManager.Instance.PlaySFX("Click");
                DisplayQuestion(); 
            }
        });

        //try to play the pre-question audio clips
        TriggerPreQuestionAudio();
    }

    // method for displaying the Question for teh multiple choice answer page
    void DisplayQuestion() {
        //set the right components to active
        questionSprite.gameObject.SetActive(true);
        questionHeader.gameObject.SetActive(true);
        questionText.gameObject.SetActive(true);
        foreach (Button button in answerButtons) {
            button.gameObject.SetActive(true);
        }
        continueButton.gameObject.SetActive(true);

        // Set the question sprite, header, and text
        questionSprite.GetComponent<Image>().sprite = currentQuestion.questionSprite;
        questionHeader.SetText(currentQuestion.questionHeader);
        questionText.SetText(currentQuestion.questionText);

        // Position questionText in relation to the size of the header
        RectTransform preHeaderRect = questionHeader.rectTransform; // Get the RectTransform of the header
        LayoutRebuilder.ForceRebuildLayoutImmediate(preHeaderRect); // Make sure the header’s layout is up-to-date
        float headerHeight = LayoutUtility.GetPreferredHeight(preHeaderRect); // Compute preferred height for the header
        preHeaderRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, headerHeight); // Set the height of the header
        RectTransform textRect = questionText.rectTransform; // Get the RectTransform of the text
        float newY = preHeaderRect.anchoredPosition.y - (preHeaderRect.sizeDelta.y/2) - 50f - (textRect.sizeDelta.y/2); // Compute the y‐position: bottom of header minus 50px
        textRect.anchoredPosition = new Vector2(textRect.anchoredPosition.x, newY); // Apply it to the text’s anchored position

        // populate the answer buttons and update layout
        for (int i = 0; i < answerButtons.Length; i++) {
            if (i < currentQuestion.answers.Length) {

                //populate the button with the answer
                answerButtons[i].gameObject.SetActive(true);
                answerButtons[i].GetComponentInChildren<TMP_Text>().text = currentQuestion.answers[i];

                // Rezise the button to fit the amount of text lines
                RectTransform buttonRect = answerButtons[i].GetComponent<RectTransform>(); // Get the RectTransform of the button
                RectTransform textRectTransform = answerButtons[i].GetComponentInChildren<TMP_Text>().rectTransform; // Get the RectTransform of the text
                float preferredHeight = LayoutUtility.GetPreferredHeight(textRectTransform); // Compute preferred height
                float buttonHeight = preferredHeight + 53f; // Add padding
                buttonRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, buttonHeight); // Set the height of the button
                LayoutRebuilder.ForceRebuildLayoutImmediate(buttonRect); // Make sure the button's layout is up-to-date
                // Position each answer button in relation to eachother
                var continueRect = continueButton.GetComponent<RectTransform>(); // Get the RectTransform of the continue button
                float baseY = continueRect.anchoredPosition.y + 250f; // Get the y-position of the continue button
                float spacing = 70f; // Spacing between buttons
                // apply new anchoredPosition
                buttonRect.anchoredPosition = new Vector2(
                    buttonRect.anchoredPosition.x,
                    baseY + i * spacing + preferredHeight * i
                );
                //update the onClick event for the button
                int answerIndex = i; 
                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(() => 
                {
                    AudioManager.Instance.PlaySFX("Click"); //answer button click sound
                    //play read sound if quiz is general 
                    TriggerQuestionButtonAudio(answerIndex);
                    currentAnswerIndex = answerIndex;
                });
            } else {   
                // if there are more buttons than answers, set the button to not active
                answerButtons[i].gameObject.SetActive(false);
            }
        //  update the onClick button for the continue button
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(() => 
            {
                AudioManager.Instance.PlaySFX("Click");
                SubmitAnswer(currentAnswerIndex);
            });
        }
        currentAnswerIndex = -1; // Reset the current answer index
        //resize and reposition the continuebutton
        RectTransform rt = continueButton.GetComponent<RectTransform>();
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 146f); // Set height to 146
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y - 2f); // Move the button down by 2 pixels

        //play read sound if quiz is general 
        TriggerQuestionHeaderAudio();
    }

    // method for changing the text color of the answer buttons through the EventSystem
    // this method is called in the Update method
    void ChangeTextColor() {
        // define reusable colors
        Color defaultTextColor   = new Color(0x57 / 255f, 0x8E / 255f, 0x7E / 255f); // Green
        Color highlightedTextColor = new Color(0xFA / 255f, 0xF7 / 255f, 0xED / 255f); // Beige
        // Set the text color based on the current selected button, no button is selected, set the selected button to the current answer index
        if (EventSystem.current.currentSelectedGameObject == null) {
            for (int i = 0; i < answerButtons.Length; i++) {
                Button button = answerButtons[i];
                if (i == currentAnswerIndex) {
                    button.GetComponentInChildren<TMP_Text>().color = highlightedTextColor;
                    EventSystem.current.SetSelectedGameObject(button.gameObject);
                } else {
                    button.GetComponentInChildren<TMP_Text>().color = defaultTextColor;
                }
            }
            // If a button is selected, set the text color to highlighted and all other buttons to default
        } else {

            Button selectedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            if (selectedButton != null) {
                foreach (Button button in answerButtons) {
                button.GetComponentInChildren<TMP_Text>().color = defaultTextColor;
                }
                selectedButton.GetComponentInChildren<TMP_Text>().color = highlightedTextColor;
            }
        }
    }

    // method for disabling the continue button if no answer is selected
    // this method is called in the Update method
    //this methos is reset at the end of the DisplayQuestion method
    void disableContinueButtonOnNoAnswer() {
        // Disable the continue button if no answer is selected
        if (currentAnswerIndex == -1) {
            continueButton.interactable = false;
        } else {
            continueButton.interactable = true;
            RectTransform rt = continueButton.GetComponent<RectTransform>();
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 150f); // Set height to 200
        }
    }

    // method for displaying the feedback based on the answer
    void DisplayFeedback(bool isCorrect) {
        //set the right components to active
        feedbackHeader.gameObject.SetActive(true);
        feedbackText.gameObject.SetActive(true);
        continueButton.gameObject.SetActive(true);

        // Set the feedback header and text based on the answer
        if (isCorrect) {
            AudioManager.Instance.PlaySFX("Right");
            //play sound if quiz is general 
            TriggerFeedbackAudio(isCorrect);
            feedbackHeader.text = "Korrekt";
            feedbackText.text = currentQuestion.correctAnswerText;
        } else {
            AudioManager.Instance.PlaySFX("Wrong");
            //play sound if quiz is general 
            TriggerFeedbackAudio(isCorrect);
            feedbackHeader.text = "Det var ikke helt korrekt";
            feedbackText.text = currentQuestion.wrongAnswerText[currentAnswerIndex];
        }

        // Position questionText 50px below the bottom of questionHeader
        // dynamically size header based on its text and add one line
        RectTransform preHeaderRect = feedbackHeader.rectTransform;
        // Make sure the header’s layout is up‑to‑date
        LayoutRebuilder.ForceRebuildLayoutImmediate(preHeaderRect);
        // Compute preferred height and add one extra line’s worth of padding to avoid clipping
        float headerHeight = LayoutUtility.GetPreferredHeight(preHeaderRect);
        preHeaderRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, headerHeight);

        RectTransform textRect = feedbackText.rectTransform;
        // Compute the y‐position: bottom of header minus 50px
        float newY = preHeaderRect.anchoredPosition.y - (preHeaderRect.sizeDelta.y/2) - 50f - (textRect.sizeDelta.y/2);
        // Apply it to the text’s anchored position
        textRect.anchoredPosition = new Vector2(textRect.anchoredPosition.x, newY);

        continueButton.gameObject.SetActive(true);
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(() => 
        {
            AudioManager.Instance.PlaySFX("Click");
            SetAllComponentsNotActive(); 
            currentQuestionIndex++; 
            // only continue if there are more questions at this difficulty
            if (questionsByDifficulty.TryGetValue(currentUserLevel, out var list) && currentQuestionIndex < list.Count){
                DisplayNewInformation();
            } else {
                EndQuiz();
            }
        });

        
    }

    void TriggerPreQuestionAudio() {

        if (currentUserLevel == Question.Difficulty.General) {
            //play the question header audio
            if (currentQuestion.PQHAudioClip != null) {
                AudioManager.Instance.PlaySFX(currentQuestion.PQHAudioClip);
            } else {
                Debug.LogError("The audio clip for the question header is null!");
            }
        } 
    }
    
    void TriggerQuestionButtonAudio(int answerIndex) {
        if (currentUserLevel == Question.Difficulty.General) {
            StopAllSounds(); // Stop all currently playing sounds
           
            //play the btn answer audio
            if (currentQuestion.answersBTNAudioClip[answerIndex] != null) {
                AudioManager.Instance.PlaySFX(currentQuestion.answersBTNAudioClip[answerIndex]);
            } else {
                Debug.LogError("The audio clip for the answer button is null!");
            }
        } 
    }

    void TriggerQuestionHeaderAudio() {
        if (currentUserLevel == Question.Difficulty.General) {
        
            //play the question header audio
            if (currentQuestion.QHAudioClip != null) {
                AudioManager.Instance.PlaySFX(currentQuestion.QHAudioClip);
            } else {
                Debug.LogError("The audio clip for the question header is null!");
            }
        } 
    }

    void TriggerFeedbackAudio(bool isCorrect) {
        if (isCorrect) {
            if (currentUserLevel == Question.Difficulty.General) {
                if (currentQuestion.FT_CAudioClip != null) {
                    //play the correct answer audio
                    AudioManager.Instance.PlaySFX(currentQuestion.FT_CAudioClip);
                } else {
                    Debug.LogError("The audio clip for the correct answer is null!");
                }
            } 
        } else {
            if (currentUserLevel == Question.Difficulty.General) {
                if (currentQuestion.wrongAnswersAudioClip[currentAnswerIndex] != null) {
                    //play the wrong answer audio
                    AudioManager.Instance.PlaySFX(currentQuestion.wrongAnswersAudioClip[currentAnswerIndex]);
                } else {
                    Debug.LogError("The audio clip for the wrong answer is null!");
                }
            } 
        }
    }
    // method for setting all components to not active
    void SetAllComponentsNotActive() {
        preQuestionSprite.gameObject.SetActive(false);
        preQuestionHeader.gameObject.SetActive(false);
        preQuestionText.gameObject.SetActive(false);
        questionSprite.gameObject.SetActive(false);
        questionHeader.gameObject.SetActive(false);
        questionText.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        feedbackHeader.gameObject.SetActive(false);
        feedbackText.gameObject.SetActive(false);
        StopAllSounds(); // Stop all currently playing sounds
        foreach (Button button in answerButtons) {
            button.gameObject.SetActive(false);
            button.gameObject.GetComponentInChildren<TMP_Text>().color = new Color(0x57/255f, 0x8E/255f, 0x7E/255f);
        }
        foreach (Button btn in EndButtons) {
            btn.gameObject.SetActive(false);
        }
    }

    // method for submitting the answer
    void SubmitAnswer(int answerIndex) {
        SetAllComponentsNotActive();
        // display feedback based on the answer
        if (currentQuestion.IsCorrectAnswer(answerIndex)) {

            DisplayFeedback(true);
            if (currentUserLevel != Question.Difficulty.General)
            {
                data.LogQuestion(currentQuestion.questionHeader + ": " + currentQuestion.correctAnswerText, 1);
                difficultyAdjuster.difficultyAdjust(true);
            }
        } else {
            DisplayFeedback(false);
            if (currentUserLevel != Question.Difficulty.General)
            {
                data.LogQuestion(currentQuestion.questionHeader+": Forkert", 0);
                difficultyAdjuster.difficultyAdjust(false);
            }
        }
    }

    // method for ending the quiz
    void EndQuiz() {
        data.onKlik();

        questionText.gameObject.SetActive(true);
        questionText.text = "Quiz Complete!";
        foreach (Button button in answerButtons) {
            button.gameObject.SetActive(false);
        }
        foreach (Button btn in EndButtons) {
            btn.gameObject.SetActive(true);
        }
    }

    public void updatedPrior(float prior)
    {
        if (prior <= 0.3)
        {
            currentUserLevel = Question.Difficulty.Easy;
            Debug.Log(currentUserLevel);
        }
        else if(prior >0.3 && prior <= 0.7)
        {
            currentUserLevel = Question.Difficulty.Medium;
            Debug.Log(currentUserLevel);
        }
        else if (prior > 0.7)
        {
            currentUserLevel = Question.Difficulty.Hard;
            Debug.Log(currentUserLevel);
        }

    }

    //  method for updating the question info based on current user difficulty defined in the top of the script
    // this method is called in the DisplayNewInformation method
    void updateQuestionInfoBasedOnDifficulty(Question.Difficulty currentDifficulty) {
        // Update the question info based on the selected difficulty
        if (currentDifficulty == Question.Difficulty.Easy) {
            currentQuestion = easyQuestions[currentQuestionIndex];
        } else if (currentDifficulty == Question.Difficulty.Medium) {
            currentQuestion = mediumQuestions[currentQuestionIndex];
        } else if (currentDifficulty == Question.Difficulty.Hard) {
            currentQuestion = hardQuestions[currentQuestionIndex];
        } else if (currentDifficulty == Question.Difficulty.General) {
            currentQuestion = generalQuestions[currentQuestionIndex];
        } else {
            Debug.LogError("Invalid difficulty level selected.");
        }
    } 


      //method for choosing the quiz, as well as defining the questions for the quiz
    void ChooseQuiz(int quizNumber) {
        if (quizNumber == 0) {
            // Set the quiz to the general introduction quiz
            currentUserLevel = Question.Difficulty.General;

            questionsByDifficulty = new Dictionary<Question.Difficulty, List<Question>> {
                { 
                    Question.Difficulty.General, new List<Question>() {
                        new Question(
                            "OneAnswer",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/GenQ1"),
                            "Hvad er skat?",
                            "Skat er de beløb, alle, der bor i Danmark og har en indkomst, betaler til staten. Derudover indbetaler du indirekte skatter som moms og punktafgifter, når du forbruger varer og tjenester.",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ4"),
                            "Hvad består skattegrundlaget af?",
                            " ",
                            new string[] {
                                "Kun direkte skatter på indkomst",
                                "Kun indirekte skatter på forbrug",
                                "Både direkte og indirekte skatter",
                                "Tariffer på importerede varer"
                            },
                            2,
                            "Godt klaret!\nSkat består af direkte skatter på indkomst og indirekte skatter på forbrug.",
                            new string[] {
                                "Direkte skatter alene udgør ikke hele skattegrundlaget.\nSkat består af direkte skatter på indkomst og indirekte skatter på forbrug.",
                                "Indirekte skatter alene udgør ikke hele skattegrundlaget.\nSkat består af direkte skatter på indkomst og indirekte skatter på forbrug.",
                                "NA",
                                "Importerede varer bliver ofte beskattet, men dette udgører ikke alene hele skattegrundlaget.\nSkat består af direkte skatter på indkomst og indirekte skatter på forbrug."
                            },
                            Question.Difficulty.General,
                            "Q1-PQH+PQT",
                            "Q1-QH",
                            new string[] {
                                "Q1-BTN-0",
                                "Q1-BTN-1",
                                "Q1-BTN-2",
                                "Q1-BTN-3"
                            },
                            "Q1-FT-C",
                            new string[] {
                                "Q1-FT-0",
                                "Q1-FT-1",
                                "Q1-FT-1",
                                "Q1-FT-3"
                            }           
                        ),

                        new Question(
                            "OneAnswer",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/GenQ2"),
                            "Hvorfor betaler vi skat?",
                            "Vi lever i et velfærdssamfund, som koster mange penge at drive. Skatterne, ca. 1.200 mia. kr. om året, finansierer uddannelse, hospitaler, ældrepleje, veje og en lang række andre offentlige ydelser. Dette sikrer at de fleste i Danmark har mulighed for at deltage i samfundet på lige vilkår.",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ4"),
                            "Hvad er formålet med skat?",
                            " ",
                            new string[] {
                                "For at finansiere velfærd",
                                "For at støtte private virksomheder",
                                "For at nedbringe privat gæld",
                                "For at øge statens investeringer"
                            },
                            0,
                            "Godt klaret!\nFordi skatteindtægterne sikrer finansiering af skolegang, sundhedsvæsen, social sikring, infrastruktur mv.",
                            new string[] {
                                "NA",
                                "Private virksomheder finansieres primært gennem privat kapital.\nVi betaler skat for at finansiere vores velfærdssamfund.",
                                "Privat gæld nedbringes ikke gennem skat.\nVi betaler skat for at finansiere vores velfærdssamfund.",
                                "Investeringer i virksomheder finansieres ikke direkte via skat.\nVi betaler skat for at finansiere vores velfærdssamfund."
                            },
                            Question.Difficulty.General,
                            "Q2-PQH+PQT",
                            "Q2-QH",
                            new string[] {
                                "Q2-BTN-0",
                                "Q2-BTN-1",
                                "Q2-BTN-2",
                                "Q2-BTN-3"
                            },
                            "Q2-FT-C",
                            new string[] {
                                "Q2-FT-0",
                                "Q2-FT-1",
                                "Q2-FT-1",
                                "Q2-FT-3"
                            }     
                        ),

                        new Question(
                            "OneAnswer",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/GenQ3"),
                            "Hvad går skattekronerne til?",
                            "Skatten fordeles på en række kategorier i det danske velfærdssamfund, hvor den største andel går til social beskyttelse, mens også sundhed, undervisning, offentlige tjenester og forsvar er med.",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ4"),
                            "Hvilke områder finansieres direkte af skatten?",
                            " ",
                            new string[] {
                                "Landbrug og fiskeri",
                                "Privat forbrug",
                                "Social beskyttelse, sundhed, undervisning",
                                "Turisme og underholdning"
                            },
                            2,
                            "Godt klaret!\nDen største post er social beskyttelse (41,6 %), herefter sundhed (17,8 %) og undervisning (11,8 %).",
                            new string[] {
                                "Landbrug og fiskeri finansieres ikke hovedsageligt af skat.\nDet rigtige svar var 'Social beskyttelse, sundhed og undervisning'.",
                                "Privat forbrug finansieres ikke af skat.\nDet rigtige svar var 'Social beskyttelse, sundhed og undervisning'.",
                                "NA",
                                "Turisme og underholdning finansieres primært privat.\nDet rigtige svar var 'Social beskyttelse, sundhed og undervisning'."
                            },
                            Question.Difficulty.General,
                            "Q3-PQH+PQT",
                            "Q3-QH",
                            new string[] {
                                "Q3-BTN-0",
                                "Q3-BTN-1",
                                "Q3-BTN-2",
                                "Q3-BTN-3"
                            },
                            "Q3-FT-C",
                            new string[] {
                                "Q3-FT-0",
                                "Q3-FT-1",
                                "Q3-FT-1",
                                "Q3-FT-3"
                            }       
                        ),

                        new Question(
                            "OneAnswer",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/GenQ4"),
                            "Hvad er forskellen på direkte og indirekte skatter?",
                            "Direkte skatter (fx A-skat og arbejdsmarkedsbidrag) trækkes direkte af din indkomst, mens indirekte skatter (fx moms og punktafgifter) lægges oven i prisen på varer og ydelser du forbruger.",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ4"),
                            "Hvordan skelner man mellem direkte og indirekte skatter?",
                            " ",
                            new string[] {
                                "Direkte på forbrug, indirekte på indkomst",
                                "Direkte på indkomst, indirekte på forbrug",
                                "Begge trækkes fra indkomst",
                                "Indirekte modtages ikke af SKAT"
                            },
                            1,
                            "Godt klaret!\nDirekte skatter udgør ca. 65 % af alle skatteindtægter; indirekte skatter ca. 35 %.",
                            new string[] {
                                "Rækkefølgen er omvendt.\nDirekte skatter trækkes på indkomst, indirekte skatter trækkes på forbrug.",
                                "NA",
                                "Indirekte skatter trækkes ikke fra indkomst.\nDirekte skatter trækkes på indkomst, indirekte skatter trækkes på forbrug.",
                                "Hverken indirekte- eller direkte skatter modtages af SKAT. Det er derimod den andel af din indkomst du skal betale til SKAT.nDirekte skatter trækkes på indkomst, indirekte skatter trækkes på forbrug."
                            },
                            Question.Difficulty.General,
                            "Q4-PQH+PQT",
                            "Q4-QH",
                            new string[] {
                                "Q4-BTN-0",
                                "Q4-BTN-1",
                                "Q4-BTN-2",
                                "Q4-BTN-3"
                            },
                            "Q4-FT-C",
                            new string[] {
                                "Q4-FT-0",
                                "Q4-FT-1",
                                "Q4-FT-1",
                                "Q4-FT-3"
                            }      
                        ),

                        new Question(
                            "OneAnswer",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/GenQ5"),
                            "Hvordan hjælper denne app dig med at forstå SKAT?",
                            "Appen giver dig en interaktiv introduktion til skattesystemet gennem korte forklaringer, eksempler og quizzer. Du kan teste din viden, følge med i relevante regler og få øvelse i at anvende skattebegreber – så du bliver bedre rustet til dine egne skatteforhold.",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ4"),
                            "Hvordan forbedrer appen din skattemæssige viden?",
                            " ",
                            new string[] {
                                "Interaktive quizzer og forklaringer",
                                "Automatisk skattemodellering",
                                "Direkte kontakt til SKAT",
                                "Gratis skatterådgivning"
                            },
                            0,
                            "Godt klaret!\nVed at kombinere teori fra SKAT med praktiske øvelser og quizzer lærer du skattebegreber og regler på en engagerende måde.",
                            new string[] {
                                "NA",
                                "Appen modellerer ikke automatisk skat.\nDen giver i stedet adgang til at kombinere teori fra SKAT med praktiske øvelser og quizzer så du kan styrke dine færdigheder.",
                                "Direkte kontakt til SKAT tilbydes ikke i appen.\nDen giver i stedet adgang til at kombinere teori fra SKAT med praktiske øvelser og quizzer så du kan styrke dine færdigheder.",
                                "Appen tilbyder ikke gratis skatterådgivning.\nDen giver i stedet adgang til at kombinere teori fra SKAT med praktiske øvelser og quizzer så du kan styrke dine færdigheder."
                            },
                            Question.Difficulty.General,
                            "Q5-PQH+PQT",
                            "Q5-QH",
                            new string[] {
                                "Q5-BTN-0",
                                "Q5-BTN-1",
                                "Q5-BTN-2",
                                "Q5-BTN-3"
                            },
                            "Q5-FT-C",
                            new string[] {
                                "Q5-FT-0",
                                "Q5-FT-1",
                                "Q5-FT-1",
                                "Q5-FT-3"
                            }      
                            )
                    } 
                }
            };
            generalQuestions = questionsByDifficulty[Question.Difficulty.General];
            
        } else if (quizNumber == 1) {
            questionsByDifficulty = new Dictionary<Question.Difficulty, List<Question>> {
                { 
                    Question.Difficulty.Easy, new List<Question>() {
                        new Question(
                        "TrueFalse",
                        Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ1"),
                        "Hvad er en forskudsopgørelse?",
                        "Forskudsopgørelsen hjælper dig med at vide, hvor meget skat du skal betale næste år. Den baseres både på tidligere indkomst og næste års forventninger.",
                        Resources.Load<Sprite>("Sprites/quiz_sprites/ForQSF"),
                        "Sandt eller falsk?",
                        "'Din forskudsopgørelse ser kun på det, du tjente sidste år.'",
                        new string[] { "Sandt", "Falsk" },
                        1,
                        "Godt klaret! Din forskudsopgørelse er baseret både på information fra tidligere år og næste år.",
                        new string[] { 
                            "Forskudeopgørelsen ser også på, hvad du forventer at tjene næste år.", 
                            "NA" 
                        },
                        Question.Difficulty.Easy
                        ),

                        new Question(
                            "OneAnswer",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ2"),
                            "Hvad indeholder din forskudsopgørelse?",
                            "Din forskudsopgørelse viser, hvor meget du regner med at tjene, og hvilke fradrag og skatteprocenter der gælder for dig i det kommende år.",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ4"),
                            "Hvad hører IKKE til i forskudsopgørelsen?",
                            " ",
                            new string[] { "Forventet indkomst", "Fradrag", "Ferieplanlægning", "Skattesats" },
                            2,
                            "Ferieplanlægning indgår ikke i din forskudsopgørelse.",
                            new string[] {
                                "Din indkomst skal med, så du ikke betaler for meget eller for lidt i skat. Din ferieplanlægning skal derimod ikke indberettes til SKAT",
                                "Fradrag hjælper med at sænke din skat, så de skal med i forskudsopgørelsen. Din ferieplanlægning skal derimod ikke indberettes til SKAT",
                                "NA",
                                "Din skatteprocent afgør, hvor meget du skal betale – den er vigtig at have med. Din ferieplanlægning skal derimod ikke indberettes til SKAT"
                             },
                            Question.Difficulty.Easy
                        ),

                        new Question(
                            "OneAnswer",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ3"),
                            "Hvorfor er forskudsopgørelsen vigtig?",
                            "Din forskudsopgørelse hjælper med at sikre, at du betaler den rigtige skat i løbet af året. Hvis den er forkert, kan du få en uventet ekstra regning senere. Dette kaldes restskat.",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ4"),
                            "Vælg det manglende ord:",
                            "En forkert forskudsopgørelse kan resultere i uventet _____ sidst på året.",
                            new string[] { "Forventet indkomst", "Fradrag", "Restskat", "Skattesats" },
                            2,
                            "En restskat er, når du har betalt for lidt i skat og skal betale ekstra senere.",
                            new string[] {
                                "Din forventede indkomst skal indgå i din forskudsopgørelse – Men restskat er det korrekte ord i denne sammenhæng. Dette betyder at du har indbetalt for lidt til skal i løbet af året, og derfor skal indbetale det resterende til skat når din årsopgørelse er parat.",
                                "Fradrag er en del af forskudsopgørelsen og hjælper med at justere din skat - Men restskat er det korrekte ord i denne sammenhæng. Dette betyder at du har indbetalt for lidt til skal i løbet af året, og derfor skal indbetale det resterende til skat når din årsopgørelse er parat.",
                                "NA",
                                "Skattesatsen er et vigtigt element, ikke det manglende ord her - Men restskat er det korrekte ord i denne sammenhæng. Dette betyder at du har indbetalt for lidt til skal i løbet af året, og derfor skal indbetale det resterende til skat når din årsopgørelse er parat."
                            },
                            Question.Difficulty.Easy
                        ),

                        new Question(
                            "OneAnswer",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ5"),
                            "Hvordan justerer du din forskudsopgørelse?",
                            "Log ind på skat.dk, vælg 'Forskudsopgørelse', og ret dine beløb for indkomst eller fradrag – ændringerne gemmes straks.",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ4"),
                            "Hvor kan du nemt justere din forskudsopgørelse?",
                            " ",
                            new string[] { "I din lokale bank", "På skat.dk", "Netbanken", "Din lokale kommune" },
                            1,
                            "Forskudsopgørelsen kan opdateres på skat.dk.",
                            new string[] {
                                "Det er ikke muligt at opdatere forskudsopgørelsen i din lokale bank. Brug skat.dk i stedet.",
                                "NA",
                                "Det er ikke muligt at opdatere forskudsopgørelsen i din netbank. Brug skat.dk i stedet.",
                                "Det er ikke muligt at opdatere forskudsopgørelsen i din lokale kommune. Brug skat.dk i stedet."
                            },
                            Question.Difficulty.Easy
                        ),

                        new Question(
                            "OneAnswer",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ6"),
                            "Hvornår kan du finde din forskudsopgørelse?",
                            "Hvert år bliver forskudsopgørelsen for næste år tilgængelig på skat.dk i november. Tjek den gerne med det samme, så du undgår overraskelser.",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ4"),
                            "Hvornår er din forskudsopgørelse tilgængelig?",
                            " ",
                            new string[] { "Januar", "Maj", "December", "November" },
                            3,
                            "Forskudsopgørelsen bliver offentliggjort i november.",
                            new string[] {
                                "Forskudsopgørelsen bliver offentliggjort i november, ikke januar.",
                                "Forskudsopgørelsen bliver offentliggjort i november, ikke maj.",
                                "Forskudsopgørelsen bliver offentliggjort i november, ikke december.",
                                "NA"
                            },
                            Question.Difficulty.Easy
                        ),

                        new Question(
                            "TrueFalse",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ7"),
                            "Hvem skal bruge en forskudsopgørelse?",
                            "Alle, der har en indkomst i Danmark, bør kontrollere deres forskudsopgørelse. Det gælder både lønmodtagere, personer på overførselsindkomster, selvstændige og pensionister.",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQSF"),
                            "Sandt eller falsk?",
                            "'Kun lønmodtagere skal bruge en forskudsopgørelse.'",
                            new string[] { "Sandt", "Falsk" },
                            1,
                            "Både lønmodtagere, personer på overførselsindkomster, selvstændige og pensionister bør tjekke deres forskudsopgørelse.",
                            new string[] {
                                "Det er ikke kun lønmodtagere. Personer på overførselsindkomster, selvstændige og pensionister bør også kontrollere deres forskudsopgørelse.",
                                "NA"
                            },
                            Question.Difficulty.Easy
                        ),

                        new Question(
                            "OneAnswer",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ8"),
                            "Hvad sker der, hvis du ikke opdaterer din forskudsopgørelse?",
                            "Uden opdatering vil SKAT basere dine løbende batalinger til skat på forældede oplysninger, hvilket kan føre til både restskat eller unødvendigt høje tilbagebetalinger.",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ4"),
                            "Hvilke konsekvenser kan det have ikke at opdatere din forskudsopgørelse?",
                            " ",
                            new string[] { "Du kan ende med restskat", "Du kan få unødvendigt store tilbagebetalinger", "Begge nedenstående svar er korrekte" },
                            2,
                            "Uden korrekte oplysninger risikerer du både at skylde ekstra skat (restskat) eller få en unødvendig stor tilbagebetaling.",
                            new string[] {
                                "Restskat er en mulighed, men du kan også modtage en unødvendig stor tilbagebetaling.",
                                "Unødvendigt store tilbagebetaling er en mulighed, men du kan også risikere at blive pålagt restskat.",
                                "NA"
                            },
                            Question.Difficulty.Easy
                        ),
                    }
                }, { 
                    Question.Difficulty.Medium, new List<Question>() {
                        new Question(
                            "TrueFalse",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ1"),
                            "Hvad er en forskudsopgørelse?",
                            "Din forskudsopgørelse vurderer din skat baseret på tidligere og forventede indkomster.",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQSF"),
                            "Sandt eller falsk?",
                            "'Din forskudsopgørelse baseres alene på sidste års økonomiske oplysninger.'",
                            new string[] { "Sandt", "Falsk" },
                            1,
                            "Rigtigt! Den inkluderer både tidligere indkomst og fremtidige forventninger.",
                            new string[] { 
                                "Forskudeopgørelsen tager både højde for din forventede indkomst næste år, samt tidligere registrerede indkomster.", 
                                "NA" 
                            },
                            Question.Difficulty.Medium
                        ),

                        new Question(
                            "OneAnswer",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ2"),
                            "Centrale elementer som indgår i din forskudsopgørelse:",
                            "Forventet indkomst (løn, bonusser osv.)\n Fradrag (kørselsfradrag, renteudgifter osv.)\n Skattesats og estimerede betalinger",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ4"),
                            "Hvilken af følgende indgår IKKE i forskudsopgørelsen?",
                            " ",
                            new string[] { "Forventet indkomst", "Fradrag", "Ferieplanlægning", "Skattesats" },
                            2,
                            "Ferieplanlægning er irrelevant i denne sammenhæng.",
                            new string[] {
                                "Din forventede indkomst skal indgå i din forskudsopgørelse. Din ferieplanlægning er derimod irrelevent for SKAT of skal derfor ikke indberettes",
                                "Fradrag er nødvendige for korrekt skatteberegning. Din ferieplanlægning er derimod irrelevent for SKAT of skal derfor ikke indberettes",
                                "NA",
                                "Skattesats er en vigtig del af opgørelsen. Din ferieplanlægning er derimod irrelevent for SKAT of skal derfor ikke indberettes"
                            },
                            Question.Difficulty.Medium
                        ),

                        new Question(
                            "OneAnswer",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ3"),
                            "Hvorfor er forskudsopgørelsen vigtig?",
                            "En præcis forskudsopgørelse sikrer, at du betaler den rette mængde skat hele året. Hvis oplysningerne er forkerte eller forældede, kan du risikere at ende med en restskat ved årets afslutning.",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ4"),
                            "Udfyld det manglende ord:",
                            "En forkert forskudsopgørelse kan resultere i uventet _____ sidst på året.",
                            new string[] { "Forventet indkomst", "Fradrag", "Restskat", "Skattesats" },
                            2,
                            "En restskat er resultatet af for lavt indbetalt skat i løbet af året, og skal betales ved årsopgørelsen.",
                            new string[] {
                                "Din forventede indkomst skal med i din forskudsopgørelse - Men det er ikke det manglende ord i denne sammenhæng. Det korrekte ord var restskat.",
                                "Fradrag reducerer din skat og skal indgå i din forskudsopgørelse - Men det er ikke det manglende ord i denne sammenhæng. Det korrekte ord var restskat.",
                                "NA",
                                "Skattesats er den andel af dine penge du skal betale i skat - Men det er ikke det manglende ord i denne sammenhæng. Det korrekte ord var restskat."
                            },
                            Question.Difficulty.Medium
                        ),

                        new Question(
                            "OneAnswer",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ5"),
                            "Hvordan opdaterer du dine skatteoplysninger i forskudsopgørelsen?",
                            "Via skat.dk’s tast-selv-service logger du ind med NemID/MitID, opdaterer forventet indkomst, fradrag og personlige oplysninger. Systemet genberegner automatisk dine acontobetalinger.",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ4"),
                            "Hvor kan du nemt justere din forskudsopgørelse?",
                            " ",
                            new string[] { "I din lokale bank", "På skat.dk", "Netbanken", "Din lokale kommune" },
                            1,
                            "Forskudsopgørelsen kan opdateres på skat.dk.",
                            new string[] {
                                "Din lokale bank har ikke adgang til SKATs tast-selv-service. Brug skat.dk. i stedte",
                                "NA",
                                "Netbanken kan ikke opdatere din forskudsopgørelse. Brug skat.dk. i stedet",
                                "Din kommune håndterer ikke forskudsopgørelser. Brug skat.dk. i stedet"
                            },
                            Question.Difficulty.Medium
                        ),

                        new Question(
                            "OneAnswer",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ6"),
                            "Hvornår går forskudsopgørelsen for det kommende år live på skat.dk?",
                            "Ifølge SKAT publiceres næste års forskudsopgørelse hvert år i november. Du bør gennemgå og eventuelt justere den med det samme.",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ4"),
                            "Hvornår er din forskudsopgørelse tilgængelig?",
                            " ",
                            new string[] { "Januar", "Maj", "December", "November" },
                            3,
                            "Forskudsopgørelsen bliver offentliggjort i november.",
                            new string[] {
                                "Forskudsopgørelsen bliver offentliggjort i november, ikke januar.",
                                "Forskudsopgørelsen bliver offentliggjort i november, ikke maj.",
                                "Forskudsopgørelsen bliver offentliggjort i november, ikke december.",
                                "NA"
                            },
                            Question.Difficulty.Medium
                        ),

                        new Question(
                            "TrueFalse",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ7"),
                            "Hvem er omfattet af pligten til at have en forskudsopgørelse?",
                            "Forskudsopgørelsen gælder for alle indkomsttagere i Danmark. Det beyder at både lønmodtagere, personer på overførselsindkomster, selvstændige og pensionister bør gennemgå den.",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQSF"),
                            "Sandt eller falsk?",
                            "'Kun lønmodtagere skal bruge en forskudsopgørelse.'",
                            new string[] { "Sandt", "Falsk" },
                            1,
                            "Forskudsopgørelsen bør gennegås af alle med skattepligtig indkomst.",
                            new string[] {
                                "Det er ikke kun lønmodtagere. Personer på overførselsindkomster, selvstændige og pensionister skal også bruge forskudsopgørelsen.",
                                "NA"
                            },
                            Question.Difficulty.Medium
                        ),

                        new Question(
                            "OneAnswer",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ8"),
                            "Hvilke følger kan manglende opdatering af forskudsopgørelsen få?",
                            "Hvis du ikke ajourfører dine forventede indkomster og fradrag, kan SKATs beregninger være forkerte. Dette betyder at du både kan ende med at få restskat eller en uhensigtsmæssig stor tilbagebetaling.",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ4"),
                            "Hvilke konsekvenser kan det have ikke at opdatere din forskudsopgørelse?",
                            " ",
                            new string[] { "Du kan ende med restskat", "Du kan få for store tilbagebetalinger", "Begge nedenstående svar er korrekte" },
                            2,
                            "Manglende opdatering kan føre til både restskat eller en for uhensigtsmæssig stor tilbagebetaling.",
                            new string[] {
                                "Det er korrekt at du kan ende med en restskat, men der er også risiko for en uhensigtsmæssig stor tilbagebetaling, hvis du ikke opdaterer din forskudsopgørelse løbende.",
                                "Det er korrekt at du kan ende med en uhensigtsmæssig stor tilbagebetaling, men der er også risiko for at skulle betale en restskat, hvis du ikke opdaterer din forskudsopgørelse løbende.",
                                "NA"
                            },
                            Question.Difficulty.Medium
                        ),
                    } 
                }, { 
                    Question.Difficulty.Hard, new List<Question>() {
                        new Question(
                            "TrueFalse",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ1"),
                            "Formålet med forskudsopgørelsen",
                            "Forskudsopgørelsen anvender estimater kombineret med tidligere økonomiske data til at beregne næste års skat.",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQSF"),
                            "Sandt eller falsk?",
                            "'Forskudsopgørelsen beregnes udelukkende på grundlag af din tidligere registrerede indkomst.'",
                            new string[] { "Sandt", "Falsk" },
                            1,
                            "Helt rigtigt! Den inkluderer også dine forventede fremtidige indkomster.",
                            new string[] { 
                                "Forskudsopgørelsen medtager både dine fremtidige forventninger, samt tidligere registrerede indkomster.", 
                                "NA" 
                            },
                            Question.Difficulty.Hard
                        ), 

                        new Question(
                            "OneAnswer",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ2"),
                            "Centrale elementer og beregningsgrundlag i forskudsopgørelsen:",
                            "Forventet indkomst (løn, bonusser osv.), personlige fradrag (transport, renter mv.) samt skatteprocent baseret på dine oplysninger – dette danner grundlag for dine acontobetalinger.",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ4"),
                            "Hvilket af følgende elementer bør IKKE fremgå i en korrekt forskudsopgørelse?",
                            " ",
                            new string[] { "Forventet indkomst", "Fradrag", "Ferieplanlægning", "Skattesats" },
                            2,
                            "Ferieplanlægning er irrelevant i denne sammenhæng.",
                            new string[] {
                                "Forventet indkomst er essentiel for korrekt skatteberegning. Din ferieplanlægning er derimod irrelevent for SKAT og skal derfor ikke indberettes",
                                "Fradrag påvirker dit rådighedsbeløb og skatteprocent. Din ferieplanlægning er derimod irrelevent for SKAT og skal derfor ikke indberettes",
                                "NA",
                                "Din skattesats afhænger af dine indberettede oplysninger. Din ferieplanlægning er derimod irrelevent for SKAT og skal derfor ikke indberettes"
                            },
                            Question.Difficulty.Hard
                        ),

                        new Question(
                            "OneAnswer",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ3"),
                            "Restskat og betydningen af korrekt forskudsopgørelse",
                            "En upræcis forskudsopgørelse, hvor dine forventede indkomster eller fradrag ikke er korrekt oplyst, kan resultere i underbetaling af skat i løbet af året. Dette vil resultere i en restskat som opgøres i din årsopgørelse. Denne skal tilbage betales til SKAT.",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ4"),
                            "Udfyld det mest passende begreb:",
                            "En forkert forskudsopgørelse kan resultere i uventet _____ sidst på året.",
                             new string[] { "Forventet indkomst", "Fradrag", "Restskat", "Skattesats" },
                            2,
                            "Det korrekte begreb er restskat, hvilket skyldes underbetaling i årets løb.",
                            new string[] {
                                "Forventet indkomst er en faktor - Men her spørges til konsekvensen af forkerte oplysninger. Det korrekte begreb var restskat.",
                                "Fradrag påvirker din skat og kan sænke din skattesats - Men her spørges til konsekvensen af forkerte oplysninger. Det korrekte begreb var restskat.",
                                "NA",
                                "Skattesats refererer til procentsatsen ad din indkomst du skal betale i skat – Men her spørges til konsekvensen af forkerte oplysninger. Det korrekte begreb var restskat."
                            },
                            Question.Difficulty.Hard
                        ),

                        new Question(
                            "OneAnswer",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ5"),
                            "Gennem hvilken digitale tjeneste indsender du ændringer til din forskudsopgørelse?",
                            "Du logger på skat.dk med MitID, vælger 'Ret forskudsopgørelse', indtaster nye indkomst- og fradrags­tal, og systemet beregner straks dine månedlige acontobetalinger.",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ4"),
                            "Hvor kan du nemt justere din forskudsopgørelse?",
                            " ",
                            new string[] { "I din lokale bank", "På skat.dk", "Netbanken", "Din lokale kommune" },
                            1,
                            "Det er kun muligt at opdatere forskudsopgørelsen via skat.dk med MitID.",
                            new string[] {
                                "Banker har ikke adgang til at ændre forskudsopgørelsen. Brug skat.dk.",
                                "NA",
                                "Netbanker understøtter ikke ændring af forskudsopgørelsen. Brug skat.dk.",
                                "Kommuner håndterer ikke digitale skatteindberetninger. Brug skat.dk."
                            },
                            Question.Difficulty.Hard
                        ),

                        new Question(
                            "OneAnswer",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ6"),
                            "På hvilket tidspunkt offentliggør SKAT næste års forskudsopgørelse?",
                            "SKAT frigiver forskudsopgørelsen for det kommende år senest i november måned. Husk at kontrollere beløb og fradrag straks, så du undgår restskat.",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ4"),
                            "Hvornår er din forskudsopgørelse tilgængelig?",
                            " ",
                            new string[] { "Januar", "Maj", "December", "November" },
                            3,
                            "Forskudsopgørelsen er tilgengelig i november",
                            new string[] {
                                "SKAT offentliggører senest forskudsopgørelsen i november.",
                                "SKAT offentliggører senest forskudsopgørelsen i november.",
                                "SKAT offentliggører senest forskudsopgørelsen i november.",
                                "NA"
                            },
                            Question.Difficulty.Hard
                        ),

                        new Question(
                            "TrueFalse",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ7"),
                            "Hvilke grupper bør sikre, at deres forskudsopgørelse er korrekt?",
                            "Enhver, der modtager skattepligtig indkomst i Danmark, herunder lønmodtagere, personer på overførselsindkomster, selvstændige og pensionister, skal kontrollere deres forskudsopgørelse.",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQSF"),
                            "Sandt eller falsk?",
                            "'Kun lønmodtagere skal bruge en forskudsopgørelse.'",
                            new string[] { "Sandt", "Falsk" },
                            1,
                            "Alle skattepligtige indkomsttagere i Danmark skal gennemgå deres forskudsopgørelse.",
                            new string[] {
                                "Det er ikke kun lønmodtagere. Personer på overførselsindkomster, selvstændige og pensionister skal også bruge forskudsopgørelsen.",
                                "NA"
                            },
                            Question.Difficulty.Hard
                        ),

                        new Question(
                            "OneAnswer",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ8"),
                            "Hvilke risici indebærer uændret forskudsopgørelse?",
                            "Uden at ajourføre forventet indkomst og fradrag vil SKATs acontoberegning være baseret på ukorrekte data, hvilket betyder risiko for både restskat eller en unødvendig stor tilbagebetaling.",
                            Resources.Load<Sprite>("Sprites/quiz_sprites/ForQ4"),
                            "Hvilke konsekvenser kan det have ikke at opdatere din forskudsopgørelse?",
                            " ",
                            new string[] { "Du kan ende med restskat", "Du kan få for store tilbagebetalinger", "Begge nedenstående svar er korrekte" },
                            2,
                            "En ukorrigeret forskudsopgørelse kan medføre enten restskat eller en unødvendig stor tilbagebetaling.",
                            new string[] {
                                "Det er korrekt at du kan ende med en restskat, men der er også risiko for en uhensigtsmæssig stor tilbagebetaling, hvis du ikke opdaterer din forskudsopgørelse løbende.",
                                "Det er korrekt at du kan ende med en uhensigtsmæssig stor tilbagebetaling, men der er også risiko for at skulle betale en restskat, hvis du ikke opdaterer din forskudsopgørelse løbende.",
                                "NA"
                            },
                            Question.Difficulty.Hard
                        ),
                    } 
                }
            };
            // Assigning the questions to the respective difficulty levels
            easyQuestions = questionsByDifficulty[Question.Difficulty.Easy];
            mediumQuestions = questionsByDifficulty[Question.Difficulty.Medium];
            hardQuestions = questionsByDifficulty[Question.Difficulty.Hard];

        } else if (quizNumber == 2) {
             Debug.Log("Quiz number is 2 - Not implemented yet.");
            SceneManager.LoadScene(1);
        }
    }

    void StopAllSounds() {
        // Find all active AudioSource components in the scene
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audioSource in audioSources) {
            // Stop each AudioSource
            audioSource.Stop();
        }
    }

}

