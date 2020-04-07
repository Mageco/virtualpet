using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InfiniteHopper;

public class Minigame3 : Minigame
{
    public IPHGameController iPHGameController;
    public override void StartGame()
    {
        iPHGameController.StartGame();
    }


}
