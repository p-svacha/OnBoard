using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionPrompt_WinGame : ActionPrompt
{
    // how long to pan the camera for, how long to show FX, etc.
    private const float PanDuration = 3f;
    private const float PostPanDelay = 1f;
    private const float FxDelay = 0.5f;

    public override void OnShow()
    {
        Game.Instance.DoWinGame();

        // 1. Freeze gameplay
        CameraHandler.Instance.Freeze();
        GameUI.Instance.HideAllElements();
        CenterCameraOnPlayer();

        // 2–4. Run the cinematic + FX + victory window
        Game.Instance.StartCoroutine(WinSequence());
    }

    public override void OnClose()
    {
        // Unfreeze & restore UI
        CameraHandler.Instance.Unfreeze();
        GameUI.Instance.ShowAllElements();
    }

    private IEnumerator WinSequence()
    {
        // compute board center
        var tilePositions = Board.Instance.Tiles.Select(t => t.transform.position).ToList();
        var boardCenter = new Vector3(
            tilePositions.Average(p => p.x),
            tilePositions.Average(p => p.y),
            tilePositions.Average(p => p.z)
        );

        // 2. Camera cinematic: pan up & back a bit
        var cinematicTarget = boardCenter + Vector3.up * 10f + Vector3.back * 10f;
        CameraHandler.Instance.PanTo(PanDuration, cinematicTarget, postPanFollowEntity: null, unbreakableFollow: false);

        yield return new WaitForSeconds(PanDuration + PostPanDelay);

        // 3. Celebration FX
        var pm = Game.Instance.PlayerMeeple.transform;
        //var confettiPrefab = ResourceManager.LoadPrefab("Prefabs/FX/Confetti");
        //GameObject.Instantiate(confettiPrefab, pm.position + Vector3.up * 1.5f, Quaternion.identity);

        AudioClip clip = ResourceManager.LoadAudioClip("Audio/WinChime");
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
        }

        yield return new WaitForSeconds(FxDelay);

        // 4. Victory Prompt Window
        // (Here we reuse your DraftWindow as a simple message box;
        //  feel free to swap in a dedicated VictoryWindow if you have one.)
        GameUI.Instance.DraftWindow.Show(
            "You Win!",
            "Congratulations, you’ve completed all chapters!",
            options: null,
            isDraft: false,
            callback: _ => OnClose()
        );
    }

    private void CenterCameraOnPlayer()
    {
        var pos = Game.Instance.PlayerMeeple.transform.position;
        CameraHandler.Instance.SetPosition(pos);
    }
}
