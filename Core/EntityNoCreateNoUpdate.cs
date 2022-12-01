namespace shunda.Core;

public class EntityNoCreateNoUpdate
{
    public virtual int Id { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (obj is EntityNoCreateNoUpdate == false)
        {
            return false;
        }

        if (Id == (obj as EntityNoCreateNoUpdate)?.Id)
        {
            return true;
        }

        return false;
    }

    public override int GetHashCode()
    {
        if (Id == 0)
        {
            return base.GetHashCode();
        }

        return Id.GetHashCode();
    }
}