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
        if (MovementType == MeepleMovementType.RandomWander)
        {
            List<MovementOption> movementOptions = Game.Instance.GetMeepleMovementOptions(this, MovementSpeed, allowStops: false);
            if (movementOptions.Count > 0)
            {
                MovementOption movement = movementOptions.RandomElement();
                Game.Instance.ExecuteMovement(movement);
            }
        }

        else if (MovementType == MeepleMovementType.ChasePlayer)
        {
            // 1) Gather all legal movement‐options (including early‐stop if CanMeepleStopHere).
            List<MovementOption> movementOptions = Game.Instance.GetMeepleMovementOptions(this, MovementSpeed, allowStops: true);

            // 2) Identify the tile the player currently occupies:
            Tile targetPosition = Game.Instance.PlayerMeeple.Tile;

            // If there are no legal moves, bail out immediately.
            if (movementOptions.Count == 0)
                return;

            // 3) Perform a quick breadth‐first search (BFS) from `targetPosition` to build a distance map:
            //    distToTarget[tile] = minimum number of hops required to reach `targetPosition`.
            var distToTarget = new Dictionary<Tile, int>();
            var queue = new Queue<Tile>();

            distToTarget[targetPosition] = 0;
            queue.Enqueue(targetPosition);

            while (queue.Count > 0)
            {
                Tile curr = queue.Dequeue();
                int currDist = distToTarget[curr];

                // Explore all neighbors of `curr`:
                foreach (Tile neighbor in curr.ConnectedTiles)
                {
                    if (!distToTarget.ContainsKey(neighbor))
                    {
                        distToTarget[neighbor] = currDist + 1;
                        queue.Enqueue(neighbor);
                    }
                }
            }

            // 4) Scan through all movementOptions.  Pick the one whose endpoint has the smallest
            //    distance to `targetPosition`.  If a move actually lands *on* the player tile, its
            //    distance is 0, and it will win immediately.
            MovementOption bestMove = null;
            int bestDistance = int.MaxValue;

            foreach (var mo in movementOptions)
            {
                Tile endTile = mo.TargetTile;
                int d;
                if (!distToTarget.TryGetValue(endTile, out d))
                {
                    // If `endTile` was never reached in the BFS, treat as "infinitely far".
                    d = int.MaxValue;
                }

                if (d < bestDistance)
                {
                    bestDistance = d;
                    bestMove = mo;
                }
            }

            // 5) If we found a valid bestMove, execute it:
            if (bestMove != null)
            {
                Game.Instance.ExecuteMovement(bestMove);
            }
            // else: no endpoint was reachable (disconnected graph?), do nothing.
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
    protected virtual MeepleMovementType MovementType => Def.MovementType;
    public virtual string Label => Def.Label;
    public string LabelCap => Label.CapitalizeFirst();
    public virtual string Description => Def.Description;
}
