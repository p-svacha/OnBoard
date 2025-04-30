using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class InputHandler
{
    public static void HandleInputs()
    {
        Game game = Game.Instance;

        if (Input.GetKeyDown(KeyCode.Space) && game.GameState == GameState.PreDraw && !game.IsTokenPouchOpen) game.DrawInitialTokens();

        if (Input.GetMouseButtonDown(0)) LeftClick();
    }

    private static void LeftClick()
    {
        if (WorldManager.HoveredBoardTile != null) LeftClickBoardTile();
        if (WorldManager.HoveredThrownToken != null) LeftClickThrownToken();
    }

    private static void LeftClickBoardTile()
    {
        if (Game.Instance.GameState == GameState.MovingPhase)
        {
            MovementOption movement = Game.Instance.MovementOptions.FirstOrDefault(o => o.TargetTile == WorldManager.HoveredBoardTile);
            if (movement != null)
            {
                Game.Instance.ExecuteMovement(movement);
            }
        }
    }

    private static void LeftClickThrownToken()
    {
        if(Game.Instance.GameState == GameState.DrawingPhase)
        {
            if(Game.Instance.RemainingRedraws > 0)
            {
                Game.Instance.RedrawToken(WorldManager.HoveredThrownToken);
            }
        }
    }
}
