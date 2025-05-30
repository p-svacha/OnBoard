using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class InputHandler
{
    public static void HandleInputs()
    {
        Game game = Game.Instance;

        if (Input.GetKeyDown(KeyCode.Space) && game.GameState == GameState.PreTurn && !game.IsTokenPouchOpen) game.DrawInitialTokens();

        if (Input.GetMouseButtonDown(0)) LeftClick();
    }

    private static void LeftClick()
    {
        if (HelperFunctions.IsMouseOverUi()) return;

        // Selection
        GameUI.Instance.SelectionPanel.Deselect();
        if (WorldManager.HoveredObject != null)
        {
            // Check if hovered object is a Selectable
            ISelectable selectable = WorldManager.HoveredObject.GetComponent<ISelectable>();
            if (selectable == null) selectable = WorldManager.HoveredObject.GetComponentInParent<ISelectable>();

            // Apply selection
            if (selectable != null)
            {
                GameUI.Instance.SelectionPanel.ApplySelection(selectable);
            }
        }

        // Actions
        if (WorldManager.HoveredBoardTile != null) LeftClickBoardTile();
        if (WorldManager.HoveredThrownToken != null) LeftClickThrownToken();
    }

    private static void LeftClickBoardTile()
    {
        if (Game.Instance.GameState == GameState.ActionPhase)
        {
            if (!Game.Instance.InActionPrompt)
            {
                MovementOption movement = Game.Instance.MovementOptions.FirstOrDefault(o => o.TargetTile == WorldManager.HoveredBoardTile);
                if (movement != null)
                {
                    Game.Instance.ExecuteMovement(movement);
                }
            }
        }
    }

    private static void LeftClickThrownToken()
    {
        if(Game.Instance.GameState == GameState.PreparationPhase)
        {
            if(Game.Instance.RemainingRedraws > 0)
            {
                Game.Instance.RedrawToken(WorldManager.HoveredThrownToken);
            }
        }
    }
}
