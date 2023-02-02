using NHibernate.Classic;

namespace shunda.Core;

public abstract class EntityNoCreateNoUpdate : IValidatable {
    public virtual int Id { get; set; }

    public override bool Equals(object? obj) {
        if (obj == null) {
            return false;
        }

        if (obj is EntityNoCreateNoUpdate == false) {
            return false;
        }

        if (Id == (obj as EntityNoCreateNoUpdate)?.Id) {
            return true;
        }

        return false;
    }

    public override int GetHashCode() {
        if (Id == 0) {
            return base.GetHashCode();
        }

        return Id.GetHashCode();
    }

    // 当前提供默认实现，不强制所有Entity都实现这个方法。
    // 要强制的话，使用下面的abstract方法
    // public abstract void Validate();

    public virtual void Validate() {
    }
}