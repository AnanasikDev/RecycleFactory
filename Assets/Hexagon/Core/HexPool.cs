using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// <para>
/// Pool of objects of the same type. Objects can be enabled and disabled dynamically without unnecessary instantiation and destruction hence without much overhead and extra memory allocation. Needs createFunc and isActiveFunc to be assigned before usage. Can work with any object types due to generics and delegate-powered checks.
/// </para>
/// <para>
/// When an object becomes inactive on client (entity died, destroyed, gone off screen, used etc) it should be disabled for the Pool to recognize it as inactive. Then, whenever it is needed, it can be taken as inactive back and reinited and adapted to match new use case and enabled back. Because disabling and enabling an entity is highly game-dependend it all must be done on the game side.
/// </para>
/// </summary>
public class Pool<T>
{
    public readonly List<T> objects = new List<T>();

    public bool isEmpty { get { return objects.Count == 0; } }

    /// <summary>
    /// Function that will be called upon instantiation of a new object. Is only used in TakeInactiveOrCreate function and its variations.
    /// </summary>
    public Func<T> createFunc;

    /// <summary>
    /// Function that determines whether an object is considered active or not.
    /// </summary>
    public Func<T, bool> isActiveFunc;

    /// <summary>
    /// </summary>
    /// <param name="isActiveFunc">Function that determines whether an object is considered active or not.</param>
    /// <param name="createFunc">Function that will be called on instantiation of a new object. Is only used in TakeInactiveOrCreate function and its variations.</param>
    public Pool(Func<T, bool> isActiveFunc, Func<T> createFunc = null)
    {
        this.isActiveFunc = isActiveFunc;
        this.createFunc = createFunc;
    }

    /// <summary>
    /// Returns the first inactive object from pool or null if there is no active object or the pool is empty.
    /// </summary>
    /// <returns></returns>
    public T TakeInactive() => objects.FirstOrDefault(o => !isActiveFunc(o));

    /// <summary>
    /// Returns the first active object from pool or null if there is no active object or the pool is empty.
    /// </summary>
    public T TakeActive() => objects.FirstOrDefault(o => isActiveFunc(o));

    /// <summary>
    ///  Returns the first inactive object from pool which follows condition or null if there is no suitable object or the pool is empty.
    /// </summary>
    /// <param name="condition">Extra condition on each element which determines whether a certain (inactive) object is suitable or not</param>
    public T TakeInactive(Func<T, bool> condition) => objects.FirstOrDefault(o => !isActiveFunc(o) && condition.Invoke(o));

    /// <summary>
    /// Returns the first active object from pool which follows condition or null if there is no suitable object or the pool is empty.
    /// </summary>
    /// /// <param name="condition">Extra condition on each element which determines whether a certain (active) object is suitable or not</param>
    public T TakeActive(Func<T, bool> condition) => objects.FirstOrDefault(o => isActiveFunc(o) && condition.Invoke(o));

    /// <summary>
    /// Assigns the first inactive object that meets the specified condition to obj, or null if no suitable object is available.
    /// </summary>
    /// <param name="condition">Extra condition on each element which determines whether a certain (inactive) object is suitable or not</param>
    /// <returns>
    /// True if the result object is not null, otherwise false
    /// </returns>
    public bool TryTakeInactive(out T obj, Func<T, bool> condition = null)
    {
        if (condition == null)
            obj = objects.FirstOrDefault(o => !isActiveFunc(o));
        else
            obj = objects.FirstOrDefault(o => !isActiveFunc(o) && condition.Invoke(o));
        return obj != null;
    }

    /// <summary>
    /// Assigns the first active object that meets the specified condition to `obj`, or null if no suitable object is available.
    /// </summary>
    /// <param name="condition">Extra condition on each element which determines whether a certain (active) object is suitable or not</param>
    /// <returns>
    /// True if the result object is not null, otherwise false
    /// </returns>
    public bool TryTakeActive(out T obj, Func<T, bool> condition = null)
    {
        if (condition == null)
            obj = objects.FirstOrDefault(o => isActiveFunc(o));
        else
            obj = objects.FirstOrDefault(o => isActiveFunc(o) && condition.Invoke(o));
        return obj != null;
    }

    /// <summary>
    /// Takes an inactive object from the pool or creates and records a new object using default type contructor.
    /// </summary>
    public T TakeInactiveOrCreate()
    {
        if (TryTakeInactive(out T obj))
            return obj;
        T res = createFunc.Invoke();
        return RecordNew(res);
    }

    /// <summary>
    /// Takes an inactive object from the pool or creates and records a new object using default type contructor.
    /// </summary>
    /// <param name="condition">Extra condition on each element which determines whether a certain (inactive) object is suitable or not</param>
    /// <returns>
    /// Returns an inactive object from the pool or the one created.
    /// </returns>
    public T TakeInactiveOrCreate(Func<T, bool> condition)
    {
        if (TryTakeInactive(out T obj, condition))
            return obj;
        T res = createFunc.Invoke();
        return RecordNew(res);
    }

    /// <summary>
    /// Records a new object to the pool without checking whether it is needed.
    /// </summary>
    /// <returns>
    /// Returns the added object back
    /// </returns>
    public T RecordNew(T newObj)
    {
        objects.Add(newObj);
        return newObj;
    }

    /// <summary>
    /// Removes the given object 
    /// </summary>
    /// <returns>True if the given object was successfully removed</returns>
    public bool Unrecord(T obj)
    {
        return objects.Remove(obj);
    }
}
