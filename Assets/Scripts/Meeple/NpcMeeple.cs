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
    /// Executes the movement for this meeple for this turn.
    /// </summary>
    public void Move()
    {
        List<MovementOption> movementOptions = Game.Instance.GetMeepleMovementOptions(this, MovementSpeed, allowStops: false);
        if (movementOptions.Count > 0)
        {
            MovementOption movement = movementOptions.RandomElement();
            Game.Instance.ExecuteMovement(movement);
        }
    }

    /// <summary>
    /// Gets executed at the end of the turn after moving.
    /// </summary>
    public virtual void OnTurnPassed() { }

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

    protected virtual int MovementSpeed => Random.Range(Def.MovementSpeedMin, Def.MovementSpeedMax + 1);
    public virtual string Label => Def.Label;
    public string LabelCap => Label.CapitalizeFirst();
    public virtual string Description => Def.Description;
}
