using System;
using System.Collections.Generic;

namespace RueI.Displays;

using RueI.Extensions;
using RueI.Elements;
using RueI.Displays.Interfaces;

/// <summary>
/// Represents a basic display attached to a <see cref="DisplayCore"/>.
/// </summary>
/// <include file='docs.xml' path='docs/displays/members[@name="display"]/Display/*'/>
public class Display : DisplayBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Display"/> class.
    /// </summary>
    /// <param name="hub">The <see cref="ReferenceHub"/> to assign the display to.</param>
    public Display(ReferenceHub hub)
        : base(hub)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Display"/> class.
    /// </summary>
    /// <param name="coordinator">The <see cref="DisplayCore"/> to assign the display to.</param>
    public Display(DisplayCore coordinator)
        : base(coordinator)
    {
    }

    /// <summary>
    /// Gets the elements of this display.
    /// </summary>
    public Dictionary<IElemReference<Element>, Element> Elements { get; } = new();

    /// <inheritdoc/>
    public override IEnumerable<Element> GetAllElements() => Elements.Values.FilterDisabled();
    
    /// <summary>
    /// Adds an <see cref="Element"/> as an <see cref="IElemReference{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="Element"/> to add.</typeparam>
    /// <param name="reference">The <see cref="IElemReference{T}"/> to use.</param>
    /// <param name="element">The <see cref="Element"/> to add.</param>
    public void AddAsReference<T>(IElemReference<T> reference, T element)
        where T : Element
    {
        Elements[reference] = element;
    }

    /// <summary>
    /// Removes a <see cref="IElemReference{T}"/> from this <see cref="DisplayCore"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="Element"/> to remove.</typeparam>
    /// <param name="reference">The <see cref="IElemReference{T}"/> to remove.</param>
    public void RemoveReference<T>(IElemReference<T> reference)
        where T : Element
    {
        _ = Elements.Remove(reference);
    }

    /// <summary>
    /// Gets an <see cref="Element"/> as <typeparamref name="T"/> if the <see cref="IElemReference{T}"/> exists within this <see cref="DisplayCore"/>'s element references.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="Element"/> to get.</typeparam>
    /// <param name="reference">The <see cref="IElemReference{T}"/> to use.</param>
    /// <returns>The instance of <typeparamref name="T"/> if the <see cref="Element"/> exists within the <see cref="DisplayCore"/>'s element references, otherwise null.</returns>
    public T? GetElement<T>(IElemReference<T> reference)
        where T : Element
    {
        if (Elements.TryGetValue(reference, out Element value))
        {
            return value as T;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Gets an <see cref="Element"/> as <typeparamref name="T"/>, or creates it.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="Element"/> to get.</typeparam>
    /// <param name="reference">The <see cref="IElemReference{T}"/> to use.</param>
    /// <param name="creator">A function that creates a new instance of <typeparamref name="T"/> if it does not exist.</param>
    /// <returns>The instance of <typeparamref name="T"/>.</returns>
    public T GetElementOrNew<T>(IElemReference<T> reference, Func<T> creator)
        where T : Element
    {
        if (Elements.TryGetValue(reference, out Element value) && value is T casted)
        {
            return casted;
        }
        else
        {
            T created = creator();
            Elements.Add(reference, created);
            return created;
        }
    }
}