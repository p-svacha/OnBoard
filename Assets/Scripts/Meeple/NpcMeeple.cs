using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NpcMeeple : Meeple
{
    public MeepleDef Def { get; private set; }

    public void Init(MeepleDef def)
    {
        Def = def;
        OnInit();
    }

    protected virtual void OnInit() { }

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

    public override List<TileInteraction> GetInteractions()
    {
        List<TileInteraction> interactions = base.GetInteractions();
        foreach (TileInteractionDef interactionDef in Def.Interactions)
        {
            TileInteraction interaction = CreateTileInteraction(interactionDef);
            interactions.Add(interaction);
        };
        return interactions;
    }

    public virtual string Label => Def.Label;
    public string LabelCap => Label.CapitalizeFirst();
    public virtual string Description => Def.Description;
}
