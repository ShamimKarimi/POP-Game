﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global
{

    public static int maxNumberOfBalloonsOnScreen = 5;
    public static int maxNumberOfBalloonsInTotal = 15;

    public static float balloonAnimationSpeed = 2.0f; //how fast it moves
    public static float balloonAnimationDelta = 0.005f; //how much it moves

    public static float balloonVerticalTranslationDelta = 0.03f;

    public static int timeToNextBalloonMin = 2;
    public static int timeToNextBalloonMax = 4;

    public static int colorX = -1220;
    public static int colorY = 600;

    public static int intervalBetweenErrorSounds = 3;

}
