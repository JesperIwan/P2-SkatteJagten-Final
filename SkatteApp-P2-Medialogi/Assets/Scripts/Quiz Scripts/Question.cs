using System;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;


public class Question
{
    public string quizType;
    public Sprite preQuestionSprite;
    public string preQuestionHeader;
    public string preQuestionText;
    public Sprite questionSprite;
    public string questionHeader;
    public string questionText;
    public string[] answers;
    public int correctAnswerIndex;
    public string correctAnswerText;
    public string[] wrongAnswerText;
    public Difficulty difficulty;
    public string PQHAudioClip;
    public string QHAudioClip;
    public string FT_CAudioClip;
    public string[] answersBTNAudioClip;
    public string[] wrongAnswersAudioClip;
    

    // Enum to represent the difficulty level of the question
     public enum Difficulty {
        Easy,
        Medium,
        Hard, 
        General
    }

    // Constructor to initialize the question object
    public Question(
        string quizType, 
        Sprite preQuestionSprite, 
        string preQuestionHeader, 
        string preQuestionText, 
        Sprite questionSprite, 
        string questionHeader, 
        string questionText, 
        string[] answers, 
        int correctAnswerIndex, 
        string correctAnswerText, 
        string[] wrongAnswerText, 
        Difficulty difficulty
    ) {
        this.quizType = quizType;
        this.preQuestionSprite = preQuestionSprite;
        this.preQuestionHeader = preQuestionHeader;
        this.preQuestionText = preQuestionText;
        this.questionSprite = questionSprite;
        this.questionHeader = questionHeader;
        this.questionText = questionText;
        this.answers = answers;
        this.correctAnswerIndex = correctAnswerIndex;  
        this.correctAnswerText = correctAnswerText;
        this.wrongAnswerText = wrongAnswerText;
        this.difficulty = difficulty;
    }

    public Question(
        string quizType, 
        Sprite preQuestionSprite, 
        string preQuestionHeader, 
        string preQuestionText, 
        Sprite questionSprite, 
        string questionHeader, 
        string questionText, 
        string[] answers, 
        int correctAnswerIndex, 
        string correctAnswerText, 
        string[] wrongAnswerText, 
        Difficulty difficulty,
        string PQHAudioClip,
        string QHAudioClip,
        string[] answersBTNAudioClip,
        string FT_CAudioClip,
        string[] wrongAnswersAudioClip
    ) {
        this.quizType = quizType;
        this.preQuestionSprite = preQuestionSprite;
        this.preQuestionHeader = preQuestionHeader;
        this.preQuestionText = preQuestionText;
        this.questionSprite = questionSprite;
        this.questionHeader = questionHeader;
        this.questionText = questionText;
        this.answers = answers;
        this.correctAnswerIndex = correctAnswerIndex;  
        this.correctAnswerText = correctAnswerText;
        this.wrongAnswerText = wrongAnswerText;
        this.difficulty = difficulty;
        this.PQHAudioClip = PQHAudioClip;
        this.QHAudioClip = QHAudioClip;
        this.answersBTNAudioClip = answersBTNAudioClip;
        this.FT_CAudioClip = FT_CAudioClip;
        this.wrongAnswersAudioClip = wrongAnswersAudioClip;
    }

    // Method to check if the provided answer index is correct
    public bool IsCorrectAnswer(int answerIndex) {
        return answerIndex == correctAnswerIndex;
    }
}

