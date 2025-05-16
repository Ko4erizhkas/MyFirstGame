using Microsoft.Xna.Framework;

namespace Core.Game;

public abstract class Entity
{
    private double? _heathPoints;

    public double HealthPoints
    {
        get => _heathPoints ?? 0.0;
        set
        {
            if (_heathPoints == null)
            {
                _heathPoints = value;
                return;
            }
            if (value <= 0 || value >= int.MaxValue)
            {
                _heathPoints = 0.0;
            }
            else if (IsAlive)
            {
                _heathPoints = value;
            }
        }
    }

    public Vector2 Position { get; set; }
    public bool IsAlive => HealthPoints > 0.0;

    protected Entity(Vector2 position, double healthPoints)
    {
        Position = position;
        HealthPoints = healthPoints;
    }
    
}