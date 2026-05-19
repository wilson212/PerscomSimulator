namespace Perscom.AI.Dtos;

/// <summary>
/// A minimal ID + Name reference returned to the AI model after create/update operations.
/// Keeps the JSON response lightweight — the AI only needs to know what was created and its database ID.
/// </summary>
public readonly struct EntityRef
{
    public int Id { get; init; }
    public string Name { get; init; }

    public EntityRef(int id, string name)
    {
        Id = id;
        Name = name;
    }
}