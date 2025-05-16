using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NpcMeeple : Meeple
{
    public MeepleDef Def { get; private set; }

    public void Init(MeepleDef def)
    {
        Def = def;
    }

    /// <summary>
    /// Gets executed at the end of the turn.
    /// </summary>
    public virtual void OnEndTurn()
    {
        int movementSpeed = Random.Range(Def.MovementSpeedMin, Def.MovementSpeedMax + 1);
        List<MovementOption> movementOptions = Game.Instance.GetMeepleMovementOptions(this, movementSpeed, allowStops: false);
        MovementOption movement = movementOptions.RandomElement();
        Game.Instance.ExecuteMovement(movement);
    }

    public virtual string Label => Def.Label;
    public string LabelCap => Label.CapitalizeFirst();
    public virtual string Description => Def.Description;
}
